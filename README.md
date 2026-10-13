# 💊 PharmacyPro

> **Pharmacy financial accounting REST API** built with ASP.NET Core, Clean Architecture and PostgreSQL.
> Track daily sales, expenses and payments to suppliers, and get an instant monthly profit/loss report.

![.NET](https://img.shields.io/badge/.NET-8%2F9-512BD4?logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-5C2D91)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-EF%20Core-336791?logo=postgresql&logoColor=white)
![JWT](https://img.shields.io/badge/Auth-JWT%20%2B%20Refresh-000000?logo=jsonwebtokens)
![Swagger](https://img.shields.io/badge/Docs-Swagger-85EA2D?logo=swagger&logoColor=black)

---

## 🌐 About / Дар бораи лоиҳа / О проекте

| Language | Description |
|---|---|
| 🇹🇯 **Тоҷикӣ** | **PharmacyPro** — барномаи ҳисоботи молиявии дорухона. Фурӯши рӯзона, хароҷот (свет, аренда, маош, барқ) ва пардохт ба фирмаҳоро сабт мекунад ва ҳисоботи моҳона бо фоида ё зиён медиҳад. |
| 🇬🇧 **English** | **PharmacyPro** is a financial accounting backend for pharmacies. It records daily sales, operating expenses and payments to supplier firms, and produces monthly and weekly reports with net profit or loss. |
| 🇷🇺 **Русский** | **PharmacyPro** — бэкенд для финансового учёта аптеки. Ведёт учёт ежедневных продаж, расходов и оплат поставщикам, формирует месячные и недельные отчёты с прибылью или убытком. |

---

## ✨ Features

- 📊 **Monthly report**: revenue, paid to firms, operating expenses, total costs, net profit/loss
- 📈 **Weekly sales chart** data (weeks 1–5 of the month)
- 🏢 **Firms** management and payments per firm
- 🧾 **Expenses** by category (Свет, Аренда, Маош, Барқ and custom)
- 💰 **Sales** tracking with payment method (Cash / Card / Transfer)
- 🔐 **JWT authentication** with **refresh token rotation** and reuse detection
- 👥 **Role-based authorization** (Admin, Pharmacist)
- 🔎 **Filtering and pagination** on all list endpoints
- ⚡ **In-memory caching** for reports with automatic invalidation
- 🔁 **Background services**: report cache warm-up, expired refresh token cleanup
- 🛡️ Global exception handling, request logging, rate limiting on auth endpoints
- 🕐 Timezone-aware reports (Asia/Dushanbe, UTC+5)
- 🗑️ Soft delete for all financial data
- 📚 Swagger UI with Bearer authentication

---

## 🧱 Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core Web API |
| Language | C# |
| Database | PostgreSQL (Npgsql) |
| ORM | Entity Framework Core + Migrations |
| Auth | ASP.NET Core Identity, JWT Bearer, Refresh Tokens |
| Docs | Swagger (Swashbuckle) |
| Patterns | Clean Architecture, Repository, DTO, Dependency Injection |

---

## 🏗️ Architecture

```
PharmacyPro/
├── src/
│   ├── PharmacyPro.Domain/            # Entities, Enums, Constants (no dependencies)
│   │   ├── Common/                    # BaseEntity
│   │   ├── Entities/                  # Firm, FirmPayment, Sale, Expense, ExpenseCategory
│   │   ├── Enums/                     # PaymentMethod
│   │   └── Constants/                 # Roles
│   │
│   ├── PharmacyPro.Application/       # Business logic
│   │   ├── Common/                    # Mappings, DateHelper, QueryExtensions, BusinessSettings
│   │   ├── DTOs/
│   │   ├── Filters/                   # Filtering + pagination
│   │   ├── Interfaces/                # IRepository, IServices, ICurrentUserService
│   │   ├── Responses/                 # Response<T>, PagedResponse<T>
│   │   └── Services/                  # Firm, Sale, Expense, FirmPayment, Report (+ cache)
│   │
│   ├── PharmacyPro.Infrastructure/    # External concerns
│   │   ├── BackgroundServices/        # ReportWarmup, RefreshTokenCleanup
│   │   ├── Data/                      # AppDbContext, Configurations, Migrations, DbInitializer
│   │   ├── Identity/                  # ApplicationUser, JwtSettings, TokenService, RefreshToken
│   │   ├── Repositories/              # Generic Repository<T>
│   │   └── Services/                  # AuthService, CurrentUserService
│   │
│   └── PharmacyPro.API/               # Presentation
│       ├── Controllers/
│       ├── Extensions/                # Swagger setup
│       ├── Middlewares/               # Exception handling, Request logging
│       └── Program.cs
```

**Dependency rule:** `API → Infrastructure → Application → Domain`

---

## 🗄️ Domain Model

| Entity | Purpose |
|---|---|
| `Firm` | Supplier company |
| `FirmPayment` | Payment made to a firm |
| `Sale` | Revenue record |
| `ExpenseCategory` | Свет, Аренда, Маош, Барқ, ... |
| `Expense` | Operating expense |
| `ApplicationUser` | Identity user (Admin / Pharmacist) |
| `RefreshToken` | Hashed refresh tokens |

**Report formula**

```
TotalCosts = PaidToFirms + OperatingExpenses
NetProfit  = TotalRevenue − TotalCosts
```

---

## 🔌 API Endpoints

### Auth

| Method | Endpoint | Access | Description |
|---|---|---|---|
| POST | `/api/auth/login` | Public | Get access + refresh token |
| POST | `/api/auth/refresh` | Public | Rotate refresh token |
| POST | `/api/auth/logout` | Authorized | Revoke refresh token |
| POST | `/api/auth/register` | Admin | Create a user |
| GET | `/api/auth/me` | Authorized | Current user info |
| POST | `/api/auth/change-password` | Authorized | Change password (revokes all sessions) |

### Resources

| Resource | Endpoints | Pharmacist | Admin |
|---|---|---|---|
| Firms | `GET/POST/PUT/DELETE /api/firms` | Read | Full |
| Expense categories | `GET/POST/PUT/DELETE /api/expense-categories` | Read | Full |
| Sales | `GET/POST/PUT/DELETE /api/sales` | Read, Create | Full |
| Expenses | `GET/POST/PUT/DELETE /api/expenses` | ✗ | Full |
| Firm payments | `GET/POST/PUT/DELETE /api/firm-payments` | ✗ | Full |
| Reports | `GET /api/reports/monthly`, `GET /api/reports/weekly` | ✗ | Full |

### Query parameters

```http
GET /api/sales?From=2026-10-01&To=2026-10-31&Method=Cash&MinAmount=100&PageNumber=1&PageSize=20
GET /api/expenses?CategoryId=2&From=2026-10-01
GET /api/reports/monthly?year=2026&month=10
```

### Response format

```json
{
  "isSuccess": true,
  "statusCode": 200,
  "message": null,
  "data": { },
  "errors": null
}
```

Paged responses additionally include `pageNumber`, `pageSize`, `totalCount`, `totalPages`.

### Sample monthly report

```json
{
  "year": 2026,
  "month": 10,
  "totalRevenue": 0,
  "totalPaidToFirms": 0,
  "totalOperatingExpenses": 0,
  "totalCosts": 0,
  "netProfit": 0,
  "isLoss": false,
  "expensesByCategory": [],
  "firmShares": []
}
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (8.0 or newer)
- [PostgreSQL](https://www.postgresql.org/download/) 14+
- `dotnet-ef` tool

```bash
dotnet tool install --global dotnet-ef
```

### 1. Clone

```bash
git clone https://github.com/MuhammadsolehNusratzoda/PharmacyPro_2026.git
cd PharmacyPro_2026
```

### 2. Configure

Edit `src/PharmacyPro.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PharmacyProDb;Username=postgres;Password=YOUR_PASSWORD"
  },
  "Jwt": {
    "Key": "CHANGE_ME_TO_A_RANDOM_SECRET_AT_LEAST_32_CHARS_LONG",
    "Issuer": "PharmacyPro",
    "Audience": "PharmacyProClient",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 7
  },
  "SeedAdmin": {
    "Email": "admin@pharmacypro.tj",
    "Password": "Admin@12345"
  },
  "Business": { "TimeZoneId": "Asia/Dushanbe" },
  "Cors": { "Origins": [ "http://localhost:3000", "http://localhost:5173" ] }
}
```

Keep secrets out of git, use user-secrets in development:

```bash
dotnet user-secrets init --project src/PharmacyPro.API
dotnet user-secrets set "Jwt:Key" "<random-64-char-secret>" --project src/PharmacyPro.API
```

### 3. Run

```bash
dotnet build
dotnet run --project src/PharmacyPro.API
```

On first start the app automatically:

1. Applies EF Core migrations
2. Creates roles `Admin` and `Pharmacist`
3. Seeds the default admin user
4. Seeds default expense categories (Свет, Аренда, Маош, Барқ)

Open Swagger: `https://localhost:<port>/swagger`

### 4. Migrations (manual)

```bash
dotnet ef migrations add <Name> -p src/PharmacyPro.Infrastructure -s src/PharmacyPro.API -o Data/Migrations
dotnet ef database update -p src/PharmacyPro.Infrastructure -s src/PharmacyPro.API
```

---

## 🔐 Security

- Passwords hashed by ASP.NET Core Identity, lockout after 5 failed attempts (15 min)
- Short-lived access token (15 min) + refresh token (7 days)
- Refresh tokens stored as **SHA-256 hashes**, rotated on every use
- Reuse of a revoked refresh token revokes all sessions of that user
- Rate limiting on `login` and `refresh` (10 requests/minute per IP)
- Generic login error message (no user enumeration)
- Change the seeded admin password and `Jwt:Key` before deploying

---

## ⚙️ Background Services

| Service | Interval | Job |
|---|---|---|
| `ReportWarmupService` | 9 min | Keeps current-month report cache warm |
| `RefreshTokenCleanupService` | 24 h | Deletes refresh tokens expired/revoked more than 30 days ago |

---

## 🕐 Date & Time Notes

- All dates are stored in **UTC**
- Send dates with an offset: `2026-10-11T14:30:00+05:00`
- Monthly and weekly boundaries are calculated in the configured business timezone (`Asia/Dushanbe`)
- Weeks: 1 = days 1–7, 2 = 8–14, 3 = 15–21, 4 = 22–28, 5 = 29–end

---

## 🗺️ Roadmap

- [ ] Docker + docker-compose (API + PostgreSQL)
- [ ] Export reports to Excel / PDF
- [ ] Inventory and medicine catalog
- [ ] Mobile client (React Native / Flutter)
- [ ] Multi-pharmacy (multi-tenant) support

---

## 👤 Author

**Muhammadsoleh Nusratzoda**
Back-end C#/.NET developer · Tajikistan

Skills: C#, ASP.NET Core, EF Core, PostgreSQL, JWT, Middleware, Clean Architecture

---

## 📄 License

This project is licensed under the MIT License.
