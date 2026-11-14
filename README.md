# ChainSight — Supply Chain Intelligence Platform

A .NET 8 / C# / Blazor Server portfolio MVP for manufacturing logistics and financial exposure.

## Included
- Blazor operations dashboard
- Logistics shipment monitoring and shipment impact assessment
- Financial exposure linked to affected production orders
- EF Core entities: suppliers, shipments, inventory, production orders, recommendations
- Synthetic seed data for a delayed critical component
- JSON endpoints: `/health`, `/api/dashboard`, `/api/shipments`, `/api/shipments/{id}/impact`
- Demo recommendation approval (records status only; no external action)
- SQLite local database; PostgreSQL connection support
- Dockerfile and Azure deployment target notes

## Run
Install the .NET 8 SDK, then:
```bash
dotnet restore
dotnet run
```
Open the HTTPS address printed in the terminal. The app creates `chainsight.db` and seeds demo data.

## PostgreSQL
Set `ConnectionStrings__SupplyChain` to a PostgreSQL connection string, e.g. `Host=<host>;Port=5432;Database=chainsight;Username=<user>;Password=<secret>;SSL Mode=Require`. Never commit credentials; use Key Vault/managed identity in Azure.

## Safety and assumptions
All data is synthetic. Revenue at risk means the value associated with affected production orders; it is not recognized loss, profit or a cash-flow forecast. Recovery cost is illustrative. The approval action records a demo approval only. It does not place purchase orders, change schedules or trigger payments.

## Next build increments
1. Add automated tests and EF Core migrations.
2. Add Microsoft Entra ID authentication and role-based authorization.
3. Add Key Vault, managed identity, private networking and Application Insights.
4. Add typed ERP, supplier and carrier integration adapters.
5. Add audited, idempotent approval commands with explicit authorization.
6. Add deterministic risk scoring and scenario comparison.
7. Add Azure OpenAI explanation/summary only after business calculations are testable; the model must not invent amounts or autonomously commit business actions.
