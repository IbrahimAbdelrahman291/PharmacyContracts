# Frontend implementation prompt for all uncommitted backend updates

Implement the following frontend changes against the current ASP.NET Core API. These requirements cover **all frontend-relevant backend changes currently present in the uncommitted working tree**. Preserve the existing frontend architecture, authentication handling, localization, styling, route guards, API client conventions, loading states, empty states, confirmation dialogs, and error presentation. API validation failures normally use `{ "errors": ["message"] }`. JSON is shown below in camelCase.

## 1. Cheque creation and management

### Updated calculation rules

- Tax is always `1%` of `correctAmount`; do not allow the user to edit the tax percentage.
- `administrativeExpensesPercentage` is retained as the API field name for compatibility, but it is now a **money amount**, not a percentage.
- The pharmacy must enter this administrative-expense amount for each cheque allocation.
- Display the calculated tax amount as `correctAmount * 0.01`.
- Calculate/display `finalAmount = correctAmount - administrativeExpensesPercentage - taxAmount`.
- The API recalculates and persists the authoritative values.

`GET /api/v1/cheques/prepare?companyName={name}&month={month}&year={year}` now returns `taxPercentage: 1`, `administrativeExpensesPercentage: 0`, and a preliminary `finalAmount` with only the 1% tax deducted. Recalculate the displayed final amount as soon as the pharmacy enters administrative expenses.

For `POST /api/v1/cheques/claims/{claimId}`, every allocation must now contain:

```json
{
  "departmentName": null,
  "amount": 10000,
  "administrativeExpensesPercentage": 2000,
  "chequeNumber": "optional",
  "bankName": "optional"
}
```

Validate that the administrative-expense amount is zero or positive and does not exceed 99% of the allocation amount.

### Actual amount and payment difference

When receiving a cheque, the pharmacy enters `actualAmount`. The API calculates:

```text
paymentDifference = actualAmount - finalAmount
```

The difference is signed: negative means underpayment, positive means overpayment, and zero means equal. Continue displaying `paymentDifferenceType` (`Decrease`, `Increase`, or `Equal`) if useful, but do not convert the difference to an absolute value.

### Negative-payment workflow

Add the following cheque statuses:

- `CorporateInsolvencies`
- `ReturnedToReviewer`

If `paymentDifference < 0`, show two actions:

1. Send to Corporate Insolvencies: PATCH `/api/v1/cheques/{id}/status` with `{ "status": "CorporateInsolvencies" }`.
2. Return to reviewer: require a non-empty reason and PATCH the same endpoint with:

```json
{
  "status": "ReturnedToReviewer",
  "returnToReviewerReason": "Reason entered by the pharmacy"
}
```

Cheque responses now include `returnToReviewerReason`. Display it in the cheque details/status history area when present. Do not show these two actions unless the stored `paymentDifference` is negative.

### Cheque list filters

Ensure the cheque list uses:

```http
GET /api/v1/cheques?status={status}&companyName={companyName}&month={1-12}&year={year}
```

All filters are optional and combinable. Include the two new statuses in the status filter. Validate month as 1–12 and year as 1–9999 before sending.

## 2. Claim-review differences and report

Difference request and response objects now include:

```json
{
  "employeeName": "optional depending on reason",
  "pharmacyName": "optional depending on reason"
}
```

Apply these conditional form rules:

- `ContractualDeduction`: `employeeName` and `pharmacyName` are required.
- `DeferredToNextMonth`: `pharmacyName` is required; `employeeName` is optional.
- `AccountingDeficit` and `Other`: both fields are optional.

Add a Claims Reviewer report page or section using:

```http
GET /api/v1/claims-management/review-differences/report?companyName={optional}&pageNumber=1&pageSize=10
```

This endpoint requires the `ClaimsReviewer` role. The response is paginated:

```json
{
  "items": [
    {
      "companyName": "Company",
      "totalAmount": 1200.50,
      "notes": ["note 1", "note 2"]
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

Provide company-name filtering, pagination controls, total amount formatting, and readable rendering of the notes list.

## 3. Digital receipts

Build digital-receipt management for Claims Reviewer and Pharmacy users.

### Claims Reviewer actions

Create a request:

```http
POST /api/v1/digital-receipts
```

```json
{
  "companyId": "guid",
  "amount": 1500,
  "month": 10,
  "year": 2026
}
```

Rules: amount cannot be negative, month is 1–12, future periods are rejected, and only one request per company/month/year is allowed.

Cancel a pending request:

```http
DELETE /api/v1/digital-receipts/{id}
```

Cancellation is allowed only while status is `PendingUpload`.

### Pharmacy actions

List requests:

```http
GET /api/v1/digital-receipts
```

Upload or replace a receipt PDF:

```http
POST /api/v1/digital-receipts/{id}/file
Content-Type: multipart/form-data
form field: file
```

Only a real PDF with MIME type `application/pdf` and size at most 2 MB is accepted. Show upload progress and validation before submission.

Download an available file:

```http
GET /api/v1/digital-receipts/{id}/file
```

Handle this as a binary file response and preserve the server-provided filename.

### Receipt model and statuses

```ts
type DigitalReceiptStatus = "PendingUpload" | "Available" | "Expired";

interface DigitalReceipt {
  id: string;
  companyId: string;
  pharmacyId: string;
  companyName: string;
  amount: number;
  month: number;
  year: number;
  status: DigitalReceiptStatus;
  originalFileName: string | null;
  uploadedAt: string | null;
  expiresAt: string | null;
  createdAt: string;
}
```

The first successful upload sets expiry to 60 days after upload. Re-upload does not extend an already assigned expiry. Disable download when unavailable/expired and clearly show expiry/status badges.

## 4. Cosmetics companies ledger

This feature is completely independent from claim/insurance companies. It is available only to the `Pharmacy` role. Add a dedicated cosmetics ledger area with company management, transaction-name management, transactions, balances, and reports.

Base route: `/api/v1/cosmetics`.

### Cosmetics companies

- `GET /companies` returns `[{ id, name, balance }]` and must be used as the cosmetics-company selector endpoint.
- `POST /companies` with `{ "name": "Company name" }`.
- `PUT /companies/{id}` with `{ "name": "Updated name" }`.
- `DELETE /companies/{id}`.

Company names are unique per pharmacy. A company with transactions cannot be deleted. Provide add/edit dialogs, delete confirmation, current-balance display, loading/empty states, and backend error handling.

### Transaction names

Names are pharmacy-managed reusable transaction categories/descriptions.

- `GET /names` returns `[{ id, name }]`.
- `POST /names` with `{ "name": "Transaction name" }`.
- `PUT /names/{id}` with `{ "name": "Updated name" }`.
- `DELETE /names/{id}`.

Names are unique per pharmacy and cannot be deleted while used by a transaction. Provide a management UI and use this endpoint to populate transaction-name selectors.

### Transactions

Create:

```http
POST /api/v1/cosmetics/transactions
```

Update:

```http
PUT /api/v1/cosmetics/transactions/{id}
```

Both use:

```json
{
  "companyId": "guid",
  "nameId": "guid",
  "amount": 2000,
  "notes": "optional, maximum 1000 characters",
  "date": "2026-10-28T00:00:00",
  "isNegative": false
}
```

Delete with `DELETE /api/v1/cosmetics/transactions/{id}` after confirmation.

The amount must always be greater than zero. `isNegative` controls the direction:

```text
false → balance + amount
true  → balance - amount
```

Use an explicit positive/negative direction selector; do not ask users to type a negative amount. Editing, deleting, changing company, or changing the date can recalculate this transaction and all later running balances.

Transaction responses have:

```ts
interface CosmeticsTransaction {
  id: string;
  companyId: string;
  companyName: string;
  nameId: string;
  name: string;
  amount: number;
  notes: string | null;
  date: string;
  balance: number;
  isNegative: boolean;
}
```

Render negative entries and balances clearly, and format all money consistently.

### Cosmetics report

```http
GET /api/v1/cosmetics/reports
```

Optional, combinable query parameters:

- `companyId`
- `nameId`
- `name` (exact transaction-name text)
- `from`
- `to`

The `to` date includes the full selected day. Response:

```json
{
  "companyBalances": [
    {
      "companyId": "guid",
      "companyName": "Company",
      "balance": 5000
    }
  ],
  "transactions": []
}
```

Build filter controls using the cosmetics company and name endpoints, a balance summary per company, and a transaction table with edit/delete actions. Refresh the company list and report after every transaction mutation so recalculated balances appear immediately.

## 5. General frontend requirements

- Add navigation items only for authorized roles.
- Keep claim companies and cosmetics companies in separate stores, selectors, routes, and terminology.
- Use server values as authoritative after every create/update operation.
- Refresh or invalidate relevant cached queries after mutations.
- Confirm destructive actions.
- Display API `errors` messages instead of replacing them with generic failures.
- Support responsive layouts and the project’s current Arabic/English and RTL conventions where applicable.
- Add/update API types, hooks/services, forms, validation schemas, tables, status badges, tests, and route guards.
- Do not hard-code pharmacy IDs; they are derived by the backend from the authenticated token.
