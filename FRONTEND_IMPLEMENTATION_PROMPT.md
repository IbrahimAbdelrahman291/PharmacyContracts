# Frontend implementation prompt

Implement the following features in the existing frontend. Reuse its current framework, API client, authentication/token handling, query-cache library, components, styling, localization, and route guards. Do not introduce a second architecture or hard-code the API base URL.

All requests require the existing bearer token. Send query parameters only when they have a value, URL-encode `companyName`, show loading/empty/error states, and display backend `errors` messages returned by HTTP 400/404. API JSON uses camelCase.

## 1. Company profile

Add a company-profile screen available to the `Pharmacy` role.

Request:

`GET /api/v1/company-profile?companyName={name}&month={1-12}&year={year}&pageNumber={page}&pageSize={size}`

- `companyName` is required.
- `month` and `year` are optional.
- Support pagination and reset `pageNumber` to 1 whenever a filter changes.
- Render summary-friendly currency columns and a paginated table containing `saleDate`, `importedItemsTotal`, `localItemsTotal`, `grossTotal`, `discountOnTotal`, `discountOnItems`, `subTotal`, and `remainingAmount`.
- The response is `{ items, pageNumber, pageSize, totalCount, totalPages, hasPreviousPage, hasNextPage }`.

## 2. Claim review by reviewer

For users whose role is `ClaimsReviewer`, add a claims-review workflow. Never show or enable the create-review action for other roles.

1. Load claims with `GET /api/v1/claims?month={1-12}&year={year}`. Both filters are optional.
2. Allow the reviewer to open one claim and submit exactly one review using `POST /api/v1/claims/{claimId}/reviews`.
3. The request body is:

```json
{
  "isAccurate": false,
  "correctedAmount": 1000.00,
  "correctedPrescriptionsCount": 25,
  "differences": [
    { "value": 50.00, "reason": "ContractualDeduction" }
  ],
  "notes": "Optional note"
}
```

When `isAccurate` is true, send `correctedAmount: null`, `correctedPrescriptionsCount: null`, and `differences: []`. When false, corrected amount/count are required, values must be non-negative, every difference value must be greater than zero, and the sum of difference values must equal `abs(correctedAmount - claimAmount)`. Allowed reasons are `ContractualDeduction`, `DeferredToNextMonth`, `AccountingDeficit`, and `Other`.

After success, invalidate/refetch claims and show the returned review. Existing reviews can be loaded by `GET /api/v1/claims/{claimId}/reviews`. Treat “review already exists” as a handled business error and prevent duplicate submission.

## 3. Month/year filters for all five reports

Create a shared report-filter component with an optional month selector (`1` through `12`), optional year selector, Apply, and Clear. Keep the selected values in URL search parameters so refresh/back navigation preserve them. Validate month/year before requesting, reset dependent pagination when filters change, and refetch only when Apply is selected.

Connect it to these five reports:

- Upcoming: `GET /api/v1/cheques/upcoming-due?days={days}&month={month}&year={year}`
- Total balance: `GET /api/v1/claims/reports/total-balance?month={month}&year={year}`
- Company balance: `GET /api/v1/claims/reports/company-balance?companyName={name}&month={month}&year={year}`
- Aging: `GET /api/v1/claims/reports/aging?companyName={optionalName}&month={month}&year={year}`
- Debtors: `GET /api/v1/claims/reports/top-debtors?top={count}&month={month}&year={year}`

Month/year filter the claim period (`month`/`year` on claims and `claimMonth`/`claimYear` on cheques), not the cheque due-date month. Omitting both parameters preserves the all-time result. Month without year and year without month are both supported.

Expected report responses:

- Upcoming: array of cheque objects using the revised cheque contract in section 4, including `id`, `companyName`, `finalAmount`, `endDate`, `status`, and receipt fields.
- Total balance: `{ totalClaimed, totalCollected, balance }`.
- Company balance: `{ companyName, totalClaimed, totalCollected, balance }`.
- Aging: `{ notYetDue, overdue0To30, overdue31To60, overdue60Plus, totalOutstanding }`.
- Debtors: array of company-balance objects, ordered by descending balance.

All five report endpoints are restricted to `Pharmacy`. Use route guards and hide their navigation/actions for `ClaimsReviewer` users. Preserve the current report presentation, currency formatting, responsiveness, accessibility, and localization. Add/update API types, service functions, query keys (including every filter), component tests, API-mocking tests, and role-visibility tests.

Acceptance criteria:

- Applying or clearing month/year updates every report request correctly.
- Stale results are not reused across different filter combinations.
- Company profile loads, filters, and paginates from its endpoint.
- A `ClaimsReviewer` can create a valid review and cannot submit an invalid or duplicate review.
- A `Pharmacy` user cannot access the create-review UI, and a reviewer cannot access pharmacy-only reports.
- Existing behavior still works when month/year are omitted.

## 4. Revised cheque financial and receipt workflow

Update all cheque API types, forms, tables, cards, calculations, labels, tests, and mocks to use the revised backend contract. Remove every use of the old cheque response fields `amount`, `amountAfterDiscount`, `discountDifference`, and `paidAmount`.

The cheque response returned by list, upcoming-due, create, and status-update endpoints now has this financial shape:

```ts
type Cheque = {
  id: string;
  companyName: string;
  departmentName: string | null;
  chequeNumber: string | null;
  bankName: string | null;
  claimMonth: number;
  claimYear: number;
  amountBeforeDiscount: number;
  correctAmount: number;
  amountDifference: number;
  taxPercentage: number;
  administrativeExpensesPercentage: number;
  finalAmount: number;
  actualAmount: number | null;
  paymentDifference: number | null;
  paymentDifferenceType: "Equal" | "Increase" | "Decrease" | null;
  chequeDate: string | null;
  startDate: string;
  endDate: string;
  settlementDays: number;
  status: "Pending" | "PaidInFull" | "PartiallyPaid" | "Deferred" | "Overdue";
  remainingAmount: number | null;
  createdAt: string;
};
```

Display the backend-calculated values; do not independently persist or override them. Their definitions are:

- `amountDifference = abs(correctAmount - amountBeforeDiscount)`.
- `finalAmount = correctAmount * (1 - (taxPercentage + administrativeExpensesPercentage) / 100)`, rounded to two decimals.
- After receipt, `paymentDifference = abs(actualAmount - finalAmount)`.
- `paymentDifferenceType` is `Increase` when actual is greater than final, `Decrease` when actual is less than final, and `Equal` when they match.

`GET /api/v1/cheques/prepare` now returns `claimId`, `companyName`, `amountBeforeDiscount`, `correctAmount`, `amountDifference`, `taxPercentage`, `administrativeExpensesPercentage`, `finalAmount`, `settlementDays`, and `departments`.

Cheque creation remains `POST /api/v1/cheques/claims/{claimId}`. Continue sending `startDate` and `allocations`. Each allocation's `amount` represents its share of the claim's correct amount; the sum must equal the claim's corrected amount. `actualAmount` and `chequeDate` are not entered during creation.

When the pharmacy receives a cheque, update it with:

`PATCH /api/v1/cheques/{id}/status`

```json
{
  "status": "PaidInFull",
  "actualAmount": 920.00,
  "chequeDate": "2026-09-25T00:00:00Z",
  "remainingAmount": null,
  "chequeNumber": "CH-1001",
  "bankName": "Example Bank"
}
```

For `PaidInFull` and `PartiallyPaid`, require non-negative `actualAmount` and a valid `chequeDate`. For `PartiallyPaid`, also require a non-negative `remainingAmount` no greater than `finalAmount`. The backend clears receipt fields when changing back to `Pending` or `Deferred`. The PATCH now returns the complete updated `Cheque` object with HTTP 200 instead of HTTP 204; replace the cached cheque with this response and invalidate affected cheque/report queries.

Show receipt fields as pending/not received when their value is null. Render the difference type as a localized badge: neutral for `Equal`, positive/overpayment for `Increase`, and warning/shortfall for `Decrease`. Continue showing unchanged fields: company, department, cheque number, bank, claim month/year, start/end dates, settlement days, status, and remaining amount.

Add or update tests for creation types, receipt validation, nullable pre-receipt values, all three difference flags, the HTTP 200 PATCH response, cache refresh, and removal of old cheque fields.
