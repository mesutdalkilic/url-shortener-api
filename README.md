# URL Shortener API (.NET 8)

Production-ready URL Shortener API built with ASP.NET Core (.NET 8).  
Includes caching, rate limiting, and clean architecture principles.

---

## 🔥 Features

- 🔗 URL shortening (Base62 encoding)
- 🔁 Redirection system
- 📊 Click tracking (analytics)
- ⚡ Redis caching (performance optimization)
- 🚦 Rate limiting (429 + JSON response)
- 🧱 Clean API design (DTO pattern)
- ✅ Validation & error handling
- 🗄️ Entity Framework Core (Code First)

## 🛠️ Tech Stack

- ⚙️ ASP.NET Core 8
- 🗄️ Entity Framework Core
- 💾 SQL Server (LocalDB)
- ⚡ Redis (Docker)
- 📄 Swagger (OpenAPI)

## 🚀 Live Demo (Local)

- Swagger UI:  
  👉 https://localhost:7013/swagger

---

## ⚙️ Installation & Setup

🔹 **1. Clone the repository**

git clone https://github.com/mesutdalkilic/UrlShortener.git
`cd UrlShortener`

🔹 2. Install dependencies
`dotnet restore`

🔹 3. Run Redis (Docker required)

👉 Open PowerShell / CMD / VS Terminal and run:
`docker run -d -p 6379:6379 --name redis redis`

👉 Check if running:
`docker ps`

👉 Optional test:
`docker exec -it redis redis-cli`
PING

Expected output:
PONG

🔹 4. Run the application
`dotnet run`

🔹 5. Open Swagger
https://localhost:7013/swagger


📌 API Endpoints

🔗 Create Short URL

POST /api/url/shorten

Body:
{
  "url": "https://google.com"
}

🔗 Redirect

GET /{shortCode}


🚦 Rate Limiting
Limit: 3 requests per 10 seconds
Returns:
{
  "error": "Too many requests"
}

⚡ Redis Caching
- Cache key: url:{shortCode}
- Expiration: 10 minutes
- Improves performance by reducing DB calls

🧠 What I Learned
- 🧩 RESTful API design
- ⚡ Rate limiting strategies
- 🧠 Distributed caching with Redis
- 🔌 Dependency Injection in .NET
- 🧱 Clean architecture practices

🔒 Security
- 🔐 Sensitive files excluded via .gitignore
- ❌ No secrets committed to repository

🚀 Future Improvements
- 🔐 Authentication & Authorization (JWT)
- ⚡ Distributed rate limiting (Redis-based)
- 📊 Logging (Serilog)
- 🐳 Docker Compose setup

📂 Project Structure
```powershell
UrlShortener/
│
├── Controllers/
├── DTOs/
├── Models/
├── Data/
├── Utils/
│
├── Program.cs
├── UrlShortener.csproj
├── README.md
├── .gitignore
