# QuébecInsider.ca

QuébecInsider.ca is a bilingual Quebec-focused digital publication for useful life hacks, savings, local services, deals, food, activities, travel, technology and everyday Quebec information.

This repository is structured as a serious full-stack application:

```text
frontend/   Angular, Angular Router, SSR, NgRx, SCSS
backend/    ASP.NET Core Web API, EF Core, SignalR, Swagger
tests/      Reserved for cross-cutting/e2e suites
scripts/    PowerShell development automation
```

## Current Slice

- Fully separated Angular and ASP.NET Core projects.
- Bilingual route model with `/fr`, `/en`, localized category routes and article routes.
- Backend multilingual content architecture using `Article`, `ArticleTranslation`, `Category` and `CategoryTranslation`.
- DTO-only API surface; EF entities are not exposed directly.
- Seeded French and English article/category content through the backend.
- Article list, article detail, category filtering, trending endpoint and language-aware search endpoint.
- Reusable `Insider Tip` and newsletter components.
- Angular SSR configuration and metadata updates for SEO.
- Swagger/OpenAPI and SignalR hub placeholder for real-time trending updates.
- xUnit tests covering language-aware search and translated article slug lookup.

## Local Setup

Backend:

```powershell
dotnet restore .\QuebecInsider.sln
dotnet run --project .\backend\src\QuebecInsider.Api
```

If no SQL Server connection string is configured, the API uses an in-memory database and seeds demo content. For SQL Server Developer Edition, set `ConnectionStrings:QuebecInsider` in user secrets or environment configuration, then add EF Core migrations.

Frontend:

```powershell
cd .\frontend
npm install
npm start
```

The Angular app expects the API at `http://localhost:5295/api`.

## Useful URLs

- Frontend: `http://localhost:4200/fr`
- API Swagger: `http://localhost:5295/swagger`
- French article route example: `/fr/articles/12-avantages-gratuits-quebecois`
- English article route example: `/en/articles/12-free-perks-quebecers`

## Verification

```powershell
dotnet test .\QuebecInsider.sln
cd .\frontend
npm test
npm run build
```
