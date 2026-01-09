# Talky Backend Documentation

## 📋 Genel Bakış

Talky backend'i, gerçek zamanlı mesajlaşma platformu için .NET 8 Web API ve SignalR kullanarak geliştirilmiştir.

## 🏗️ Mimari

### Teknoloji Stack
- **.NET 8** - Web API Framework
- **ASP.NET Core** - Web framework
- **SignalR** - Real-time WebSocket iletişimi
- **Entity Framework Core** - ORM
- **SQL Server / PostgreSQL** - Database
- **JWT Bearer** - Authentication
- **BCrypt.Net** - Password hashing
- **Swagger** - API documentation

### Katmanlı Mimari
```
┌─────────────────────────────────────┐
│         Controllers                 │  ← HTTP Endpoints
├─────────────────────────────────────┤
│         Services (BLL)              │  ← Business Logic
├─────────────────────────────────────┤
│         Data (DAL)                  │  ← Data Access
├─────────────────────────────────────┤
│         Database                    │  ← SQL Server/PostgreSQL
└─────────────────────────────────────┘

         SignalR Hub (Real-time)
```

## 📁 Proje Yapısı

```
TalkyAPI/
├── Controllers/              # API Endpoints
│   ├── AuthController.cs     # Kayıt, giriş, token
│   ├── UsersController.cs    # Kullanıcı işlemleri
│   ├── MessagesController.cs # Mesaj geçmişi
│   └── CallsController.cs    # Arama geçmişi
│
├── Hubs/                     # SignalR Hubs
│   └── ChatHub.cs            # Real-time mesajlaşma
│
├── Models/                   # Database Entities
│   ├── User.cs               # Kullanıcı modeli
│   ├── Message.cs            # Mesaj modeli
│   ├── Call.cs               # Arama modeli
│   └── Contact.cs            # Kişi modeli
│
├── DTOs/                     # Data Transfer Objects
│   ├── Auth/
│   │   ├── LoginDto.cs
│   │   ├── RegisterDto.cs
│   │   └── AuthResponseDto.cs
│   ├── Message/
│   │   ├── MessageDto.cs
│   │   └── SendMessageDto.cs
│   └── User/
│       └── UserDto.cs
│
├── Data/                     # Database Context
│   └── AppDbContext.cs       # EF Core DbContext
│
├── Services/                 # Business Logic
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IMessageService.cs
│   │   └── IUserService.cs
│   └── Implementations/
│       ├── AuthService.cs
│       ├── MessageService.cs
│       └── UserService.cs
│
├── Helpers/                  # Utility Classes
│   ├── JwtHelper.cs          # JWT token oluşturma
│   └── PasswordHelper.cs     # Password hashing
│
├── Middleware/               # Custom Middleware
│   └── ErrorHandlingMiddleware.cs
│
├── Program.cs                # Application entry point
├── appsettings.json          # Configuration
└── TalkyAPI.csproj           # Project file
```

## 🗄️ Database Schema

### Users Table
```sql
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Avatar NVARCHAR(255) NULL,
    Bio NVARCHAR(500) NULL,
    IsOnline BIT DEFAULT 0,
    LastSeen DATETIME2 NULL,
    CreatedAt DATETIME2 DEFAULT GETDATE(),
    UpdatedAt DATETIME2 DEFAULT GETDATE()
);
```

### Messages Table
```sql
CREATE TABLE Messages (
    Id INT PRIMARY KEY IDENTITY(1,1),
    SenderId INT NOT NULL,
    ReceiverId INT NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    IsRead BIT DEFAULT 0,
    SentAt DATETIME2 DEFAULT GETDATE(),
    ReadAt DATETIME2 NULL,
    FOREIGN KEY (SenderId) REFERENCES Users(Id),
    FOREIGN KEY (ReceiverId) REFERENCES Users(Id)
);
```

### Calls Table
```sql
CREATE TABLE Calls (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CallerId INT NOT NULL,
    ReceiverId INT NOT NULL,
    CallType NVARCHAR(20) NOT NULL, -- 'voice' or 'video'
    Status NVARCHAR(20) NOT NULL,   -- 'missed', 'completed', 'rejected'
    StartedAt DATETIME2 DEFAULT GETDATE(),
    EndedAt DATETIME2 NULL,
    Duration INT NULL,              -- seconds
    FOREIGN KEY (CallerId) REFERENCES Users(Id),
    FOREIGN KEY (ReceiverId) REFERENCES Users(Id)
);
```

### Contacts Table
```sql
CREATE TABLE Contacts (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL,
    ContactUserId INT NOT NULL,
    AddedAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (ContactUserId) REFERENCES Users(Id),
    UNIQUE(UserId, ContactUserId)
);
```

## 🔌 API Endpoints

### Authentication

#### POST /api/auth/register
Yeni kullanıcı kaydı
```json
Request:
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}

Response: 201 Created
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": 1,
    "username": "johndoe",
    "email": "john@example.com",
    "avatar": null,
    "isOnline": true
  }
}
```

#### POST /api/auth/login
Kullanıcı girişi
```json
Request:
{
  "email": "john@example.com",
  "password": "SecurePass123!"
}

Response: 200 OK
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": 1,
    "username": "johndoe",
    "email": "john@example.com",
    "avatar": null,
    "isOnline": true
  }
}
```

#### GET /api/auth/me
Mevcut kullanıcı bilgisi (JWT gerekli)
```json
Response: 200 OK
{
  "id": 1,
  "username": "johndoe",
  "email": "john@example.com",
  "avatar": null,
  "bio": "Hello world!",
  "isOnline": true,
  "lastSeen": "2026-01-09T10:30:00Z"
}
```

### Users

#### GET /api/users
Tüm kullanıcıları listele (JWT gerekli)
```json
Response: 200 OK
[
  {
    "id": 2,
    "username": "janedoe",
    "avatar": null,
    "isOnline": true,
    "lastSeen": "2026-01-09T10:25:00Z"
  },
  {
    "id": 3,
    "username": "bobsmith",
    "avatar": null,
    "isOnline": false,
    "lastSeen": "2026-01-09T09:15:00Z"
  }
]
```

#### GET /api/users/{id}
Belirli kullanıcı bilgisi (JWT gerekli)
```json
Response: 200 OK
{
  "id": 2,
  "username": "janedoe",
  "email": "jane@example.com",
  "avatar": null,
  "bio": "Software developer",
  "isOnline": true,
  "lastSeen": "2026-01-09T10:25:00Z"
}
```

### Messages

#### GET /api/messages/{userId}
Belirli kullanıcı ile mesaj geçmişi (JWT gerekli)
```json
Response: 200 OK
[
  {
    "id": 1,
    "senderId": 1,
    "receiverId": 2,
    "content": "Hey, how are you?",
    "isRead": true,
    "sentAt": "2026-01-09T10:00:00Z",
    "readAt": "2026-01-09T10:01:00Z"
  },
  {
    "id": 2,
    "senderId": 2,
    "receiverId": 1,
    "content": "I'm good, thanks!",
    "isRead": true,
    "sentAt": "2026-01-09T10:02:00Z",
    "readAt": "2026-01-09T10:03:00Z"
  }
]
```

#### POST /api/messages
Yeni mesaj gönder (JWT gerekli)
```json
Request:
{
  "receiverId": 2,
  "content": "Hello there!"
}

Response: 201 Created
{
  "id": 3,
  "senderId": 1,
  "receiverId": 2,
  "content": "Hello there!",
  "isRead": false,
  "sentAt": "2026-01-09T10:30:00Z",
  "readAt": null
}
```

### Calls

#### GET /api/calls
Arama geçmişi (JWT gerekli)
```json
Response: 200 OK
[
  {
    "id": 1,
    "callerId": 1,
    "receiverId": 2,
    "callerName": "johndoe",
    "receiverName": "janedoe",
    "callType": "video",
    "status": "completed",
    "startedAt": "2026-01-09T09:00:00Z",
    "endedAt": "2026-01-09T09:15:00Z",
    "duration": 900
  }
]
```

#### POST /api/calls
Yeni arama kaydı oluştur (JWT gerekli)
```json
Request:
{
  "receiverId": 2,
  "callType": "voice",
  "status": "completed",
  "duration": 300
}

Response: 201 Created
{
  "id": 2,
  "callerId": 1,
  "receiverId": 2,
  "callType": "voice",
  "status": "completed",
  "startedAt": "2026-01-09T10:30:00Z",
  "endedAt": "2026-01-09T10:35:00Z",
  "duration": 300
}
```

## 🔄 SignalR Hub

### Connection
```javascript
// Frontend connection
const connection = new HubConnectionBuilder()
  .withUrl("https://localhost:7183/chatHub", {
    accessTokenFactory: () => localStorage.getItem("token")
  })
  .build();
```

### Hub Methods

#### Client → Server

**SendMessage**
```csharp
// Backend
public async Task SendMessage(int receiverId, string content)

// Frontend
await connection.invoke("SendMessage", receiverId, "Hello!");
```

**TypingIndicator**
```csharp
// Backend
public async Task TypingIndicator(int receiverId, bool isTyping)

// Frontend
await connection.invoke("TypingIndicator", receiverId, true);
```

**MarkAsRead**
```csharp
// Backend
public async Task MarkAsRead(int messageId)

// Frontend
await connection.invoke("MarkAsRead", messageId);
```

#### Server → Client

**ReceiveMessage**
```javascript
connection.on("ReceiveMessage", (message) => {
  console.log("New message:", message);
  // message: { id, senderId, receiverId, content, sentAt }
});
```

**UserOnline**
```javascript
connection.on("UserOnline", (userId) => {
  console.log(`User ${userId} is online`);
});
```

**UserOffline**
```javascript
connection.on("UserOffline", (userId) => {
  console.log(`User ${userId} is offline`);
});
```

**TypingIndicator**
```javascript
connection.on("TypingIndicator", (userId, isTyping) => {
  console.log(`User ${userId} is ${isTyping ? 'typing' : 'stopped typing'}`);
});
```

## 🔐 Authentication & Authorization

### JWT Token Structure
```json
{
  "sub": "1",                    // User ID
  "email": "john@example.com",
  "username": "johndoe",
  "exp": 1704801600,             // Expiration (7 days)
  "iss": "TalkyAPI",             // Issuer
  "aud": "TalkyClient"           // Audience
}
```

### Authorization Header
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Protected Endpoints
Tüm `/api/users`, `/api/messages`, `/api/calls` ve SignalR hub bağlantıları JWT token gerektirir.

## ⚙️ Configuration (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=TalkyDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-min-32-characters-long",
    "Issuer": "TalkyAPI",
    "Audience": "TalkyClient",
    "ExpirationDays": 7
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5174",
      "http://localhost:3000"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## 🚀 Kurulum ve Çalıştırma

### 1. Gerekli Paketleri Yükle
```bash
cd back/TalkyAPI

dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package BCrypt.Net-Next
```

### 2. Database Migration
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 3. Uygulamayı Çalıştır
```bash
dotnet run
```

API: https://localhost:7183
Swagger: https://localhost:7183/swagger

## 🧪 Test

### Swagger UI
Swagger UI üzerinden tüm endpoint'leri test edebilirsiniz:
https://localhost:7183/swagger

### Postman Collection
1. Register ile kullanıcı oluştur
2. Login ile token al
3. Token'ı Authorization header'a ekle
4. Diğer endpoint'leri test et

## 📊 Error Handling

### Standard Error Response
```json
{
  "statusCode": 400,
  "message": "Validation error",
  "errors": {
    "Email": ["Email is required"],
    "Password": ["Password must be at least 6 characters"]
  }
}
```

### HTTP Status Codes
- `200 OK` - Başarılı
- `201 Created` - Kaynak oluşturuldu
- `400 Bad Request` - Geçersiz istek
- `401 Unauthorized` - Kimlik doğrulama gerekli
- `403 Forbidden` - Yetki yok
- `404 Not Found` - Kaynak bulunamadı
- `500 Internal Server Error` - Sunucu hatası

## 🔧 Geliştirme Notları

### CORS Policy
Frontend'in backend'e erişebilmesi için CORS yapılandırması gereklidir.

### SignalR Connection
SignalR bağlantısı JWT token ile korunmalıdır.

### Password Security
Şifreler BCrypt ile hash'lenerek saklanır, asla plain text olarak tutulmaz.

### Database Indexing
Performans için `Users.Email`, `Users.Username`, `Messages.SenderId`, `Messages.ReceiverId` alanlarına index eklenmelidir.

## 📝 Sonraki Adımlar

- [ ] Email verification
- [ ] Password reset
- [ ] File/image upload
- [ ] Group chat support
- [ ] Message reactions
- [ ] Push notifications
- [ ] Rate limiting
- [ ] Caching (Redis)
- [ ] Logging (Serilog)
- [ ] Unit tests
- [ ] Integration tests

## 📞 İletişim

Proje: Talky - Real-time Messaging Platform
Repository: https://github.com/tofiqdev/Talky_FinalProject
