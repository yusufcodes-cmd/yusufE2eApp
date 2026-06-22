# 💰 Personal Finance Tracker

A full-stack personal finance management application built with **Angular 19**, **ASP.NET Core (.NET 10)**, **SQL Server**, and **Azure**.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Angular 19, TypeScript, SCSS |
| Backend | ASP.NET Core 10, C#, Entity Framework Core |
| Database | SQL Server (Docker) |
| Cloud | Microsoft Azure (Free Tier) |
| DevOps | Docker, Docker Compose, GitHub Actions |

## Architecture

Clean Architecture with separation of concerns:

```
src/
├── FinanceTracker.API/            # Web API - Controllers, Middleware
├── FinanceTracker.Core/           # Domain - Entities, Interfaces
├── FinanceTracker.Infrastructure/ # Data Access - EF Core, Repositories
└── finance-tracker-ui/            # Angular 19 SPA
```

## Features

- 📊 Dashboard with spending analytics
- 🏦 Multi-account management (Checking, Savings, Credit)
- 💳 Transaction tracking (Income, Expense, Transfer)
- 📁 Category-based organisation
- 💰 Monthly budget planning & tracking
- 📈 Charts and reporting

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js 20+
- Docker Desktop
- Angular CLI

### Run Locally

```bash
# Start SQL Server
docker-compose up -d

# Run API
cd src/FinanceTracker.API
dotnet run

# Run Angular app
cd src/finance-tracker-ui
npm install
ng serve
```

### URLs

- **Frontend**: http://localhost:4200
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

## Author

**Yusuf Ibrahim** - Full Stack .NET Developer  
[GitHub](https://github.com/yusufcodes-cmd) | [LinkedIn](https://linkedin.com/in/yusuf-m-ibrahim-)
