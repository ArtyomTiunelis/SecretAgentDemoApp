# Secret Agent Demo App

A .NET 10 ASP.NET Core Web API demo with MongoDB persistence and an embedded Tailwind checkout page. It is designed as a deterministic incident-triage scenario.

## Prerequisites

- .NET 10 SDK
- MongoDB running locally or a MongoDB Atlas connection string

## Setup

1. Update the `MongoDb` values in `SecretAgentDemoApp/appsettings.json` if MongoDB is not available at `mongodb://localhost:27017`.
2. Seed the demo database from the repository root:

   ```powershell
   mongosh < .\SecretAgentDemoApp\seed.js
   ```

3. Run the API:

   ```powershell
   dotnet run --project .\SecretAgentDemoApp\SecretAgentDemoApp.csproj
   ```

4. Open the application URL shown by the server, typically `https://localhost:xxxx`.

## Demo flow

The checkout page submits the pre-filled `WELCOME20` code for new user `USR-5512` to:

`POST /api/v1/promos/redeem`

The seeded user intentionally has `promo_history: null`. The endpoint deliberately calls `PromoHistory.Contains(...)` without a null check, causing a `System.NullReferenceException`.

Global exception middleware captures that exception, writes a record to MongoDB's `error_logs` collection, and returns a structured HTTP 500 response for the frontend diagnostic modal.

## API request

```json
{
  "userId": "USR-5512",
  "code": "WELCOME20",
  "ticketId": "INC-30219"
}
```

Use the `X-Ticket-ID` request header to override the incident ticket identifier.
