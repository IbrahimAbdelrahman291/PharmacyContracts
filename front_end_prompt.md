# Frontend implementation prompts

Use each prompt independently in the existing frontend repository. In every case, reuse the current framework, API client, bearer-token handling, route guards, query/cache library, components, styling, localization, and error-display conventions. API JSON uses camelCase. Do not hard-code the API base URL or introduce a second frontend architecture.

## Prompt 1 — Apply the claim `subTotal` change

Update the pharmacy claim-generation and claims-review UI for the new `subTotal` property.

Backend contract:

- `POST /api/v1/claims/generate` accepts `{ "month": number, "year": number }` and returns generated claims.
- `GET /api/v1/claims?month={month}&year={year}` returns claims for both `Pharmacy` and `ClaimsReviewer` users.
- Every claim now includes `subTotal: number`. It is the sum of `SalesRecords.subTotal` for that claim's pharmacy, company, month, and year.
- A claim continues to include `claimAmount`, `claimAmountAfterDiscount`, prescription counts, correction fields, status, and dates.
- During review, `correctedAmount` is compared with `claim.subTotal`, not `claimAmount`/remaining amount.
- The required explained difference is `abs(correctedAmount - subTotal)`.
- Difference direction is `Increase` when `correctedAmount > subTotal`, `Decrease` when it is lower, and `NoDifference` when equal.

Required frontend changes:

1. Add `subTotal: number` to the claim API type and every claim mock/fixture.
2. Show a clearly labelled Subtotal value in pharmacy claim results and in the reviewer claim details. Use the existing localized money formatter.
3. In the reviewer form for `POST /api/v1/claims/{claimId}/reviews`, calculate the previewed amount difference from `subTotal`.
4. When `isAccurate` is false, require non-negative `correctedAmount` and `correctedPrescriptionsCount`; require every difference item to be greater than zero; validate the explained difference total against `abs(correctedAmount - subTotal)` using the frontend's existing money/rounding convention.
5. When `isAccurate` is true, send `correctedAmount: null`, `correctedPrescriptionsCount: null`, and `differences: []`.
6. Do not silently fall back to `claimAmount` if `subTotal` is absent. Treat that as an invalid/outdated API response so incorrect review calculations cannot be submitted.
7. After generating claims or creating/updating a review, invalidate and refetch all affected claim queries.
8. Update component, validation, API-mocking, and role-visibility tests. Include increase, decrease, equal, and decimal-value cases that prove the baseline is `subTotal` rather than `claimAmount`.

Acceptance example: for a claim with `subTotal = 1,000`, `claimAmount = 900`, and `correctedAmount = 950`, the displayed difference must be `50` and `Decrease`, never `50 Increase` based on `claimAmount`.

## Prompt 2 — Apply company profile

Add a company-profile screen for the `Pharmacy` role.

Use this endpoint:

`GET /api/v1/company-profile?companyName={name}&month={month}&year={year}&pageNumber={page}&pageSize={size}`

Contract and behavior:

- `companyName` is required and must be URL-encoded.
- `month` and `year` are optional. Send a query parameter only when it has a value.
- `pageNumber` and `pageSize` control server-side pagination.
- Response shape: `{ items, pageNumber, pageSize, totalCount, totalPages, hasPreviousPage, hasNextPage }`.
- Each item contains `status`, `customerCompanyName`, `saleDate`, `importedItemsTotal`, `localItemsTotal`, `grossTotal`, `discountOnTotal`, `discountOnItems`, `subTotal`, `remainingAmount`, and `branchName`.

Required frontend changes:

1. Add a pharmacy-only route and navigation entry using the existing authorization system.
2. Provide a required company selector/search plus optional month and year filters. Validate month as 1–12 and reset `pageNumber` to 1 when any filter or page size changes.
3. Keep applied filters and pagination in URL search parameters so refresh and browser navigation preserve the view.
4. Render a responsive, accessible paginated table. Include status, company, branch, sale date, all monetary fields, and localized currency/date formatting.
5. Distinguish `subTotal` and `remainingAmount` with unambiguous localized labels.
6. Include loading, empty, retry, and error states. Display backend `errors` messages returned by HTTP 400.
7. Put every request parameter in the query key so results are never reused for a different company, period, page, or page size.
8. Add API type/service tests, filter and pagination tests, URL-state tests, empty/error-state tests, and a route/visibility test proving non-Pharmacy users cannot access the page.

## Prompt 3 — Apply cheque workflow

Implement or update the pharmacy cheque workflow using the backend contract below. Remove all frontend use of legacy cheque fields `amount`, `amountAfterDiscount`, `discountDifference`, and `paidAmount`.

Cheque response type:

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

Endpoints and requirements:

1. Prepare creation with `GET /api/v1/cheques/prepare?companyName={name}&month={month}&year={year}`. It returns `claimId`, `companyName`, `amountBeforeDiscount`, `correctAmount`, `amountDifference`, `taxPercentage`, `administrativeExpensesPercentage`, `finalAmount`, `settlementDays`, and `departments`. Display backend-calculated values; do not override them locally.
2. Create with `POST /api/v1/cheques/claims/{claimId}` using `startDate` and `allocations`. Each allocation includes its department and amount plus supported cheque/bank fields. Require positive allocation amounts and ensure their sum equals `correctAmount`. If there are no departments, submit exactly one allocation with `departmentName: null`.
3. List with `GET /api/v1/cheques`, supporting optional `companyName`, `month`, `year`, and `status` parameters.
4. Load upcoming cheques with `GET /api/v1/cheques/upcoming-due?days={days}&month={month}&year={year}`. Month/year refer to the claim period.
5. Record receipt/status through `PATCH /api/v1/cheques/{id}/status`. For `PaidInFull` and `PartiallyPaid`, require non-negative `actualAmount` and a valid `chequeDate`. For `PartiallyPaid`, also require a non-negative `remainingAmount` no greater than `finalAmount`. Support `chequeNumber` and `bankName` together. The endpoint returns the complete updated cheque with HTTP 200.
6. Treat nullable receipt fields as “not received/pending,” not zero. Show localized badges for payment difference: neutral `Equal`, positive/overpayment `Increase`, and warning/shortfall `Decrease`.
7. Display company, department, cheque/bank details, claim period, dates, settlement days, status, all current financial fields, and remaining amount with the existing localization and currency formatting.
8. Replace the cached cheque with the PATCH response and invalidate affected cheque, upcoming-due, balance, aging, and debtor queries.
9. Restrict all cheque screens/actions to `Pharmacy` users and display backend `errors` messages for rejected business rules.
10. Update types, forms, tables/cards, mocks, and tests. Cover nullable pre-receipt values, allocation totals, status validation, partial-payment limits, all three payment-difference types, the HTTP 200 update response, cache refresh, and absence of all legacy fields.
