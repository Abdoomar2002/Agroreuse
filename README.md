# ♻️ Agroreuse — Agricultural Waste Reuse Platform

A circular-economy marketplace that connects farmers, buyers and recyclers to **reuse agricultural waste and by-products** instead of discarding them — with real-time updates and push notifications.

Built as a clean, layered **ASP.NET Core (.NET 10)** solution with an **Angular** client.

---

## ✨ Features

- 🛒 **Marketplace** for listing, browsing and trading agricultural waste / by-products
- 🔔 **Real-time updates & notifications** (SignalR + Firebase Cloud Messaging)
- 📧 Transactional **email** (MailKit / MimeKit)
- 🔐 **JWT authentication** with role-based access
- 📑 **OpenAPI** documentation via Scalar

## 🏗️ Architecture

Clean Architecture with clear separation of concerns and **CQRS** via MediatR:

```
Domain          → entities, value objects, domain events
Application     → CQRS handlers (MediatR), DTOs, validation, interfaces
Infrastructure  → EF Core, Firebase, email, external services
Presentation    → API controllers / endpoints
Server          → composition root & startup
```

## 🛠️ Tech Stack

| Layer | Technologies |
|-------|--------------|
| **Backend** | ASP.NET Core (.NET 10), C# |
| **Architecture** | Clean Architecture, CQRS (MediatR), Domain Events |
| **Data** | Entity Framework Core, SQL Server |
| **Real-time** | SignalR |
| **Auth** | JWT |
| **Notifications** | FirebaseAdmin (FCM push), MailKit / MimeKit (email) |
| **Docs** | Scalar (OpenAPI) |
| **Frontend** | Angular, TypeScript |

## 🚀 Getting Started

### Prerequisites
- [.NET SDK 10](https://dotnet.microsoft.com/download)
- SQL Server (or LocalDB)
- [Node.js](https://nodejs.org/) + Angular CLI (for the client)

### Backend
```bash
# from the solution root
dotnet restore
dotnet ef database update            # apply migrations
dotnet run --project src/Server      # adjust to your startup project
```
Configure your connection string, JWT settings and Firebase credentials in `appsettings.json` / user-secrets.

### Frontend
```bash
cd client          # Angular app folder
npm install
ng serve
```

---

> Part of my portfolio of full-stack .NET work — see more at [my portfolio](https://portfolio-abdelrahman-omar.vercel.app/).
