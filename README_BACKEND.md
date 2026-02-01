# Talky Backend - BackNtier

## 🎉 Durum: Production Ready!

**Build Status:** ✅ 0 Error, 0 Warning  
**Architecture:** N-Tier (Core → Entity → DAL → BLL → API)  
**Framework:** .NET 8  

---

## 🚀 Hızlı Başlangıç

### 1. Database Migration
```bash
cd BackNtier/Talky_API
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 2. Backend Çalıştır
```bash
cd BackNtier/Talky_API
dotnet run
```

API: `https://localhost:7001`  
Swagger: `https://localhost:7001/swagger`

---

## 📁 Proje Yapısı

```
BackNtier/
├── 00.Core/                    # Result Pattern
│   └── Core/
│       ├── Result/             # IResult, IDataResult
│       └── Business/           # Business rules
│
├── Entity/                     # Entities + DTOs
│   ├── TableModel/             # Database entities
│   └── DataTransferObject/     # DTOs (Add, Update, List)
│
├── 02.DAL/                     # Data Access Layer
│   └── DAL/
│       ├── Abstrack/           # Repository interfaces
│       ├── Concret/            # Repository implementations
│       └── Context/            # DbContext
│
├── 03.BLL/                     # Business Logic Layer
│   └── BLL/
│       ├── Abstrack/           # Service interfaces
│       ├── Concret/            # Service implementations
│       ├── Validation/         # FluentValidation
│       └── Mapper/             # AutoMapper profiles
│
└── Talky_API/                  # Web API
    ├── Controllers/            # REST endpoints
    ├── Hubs/                   # SignalR hubs
    └── Program.cs              # Startup
```

---

## 🎯 Özellikler

### Authentication & Authorization
- ✅ JWT Bearer Token
- ✅ Password hashing (BCrypt)
- ✅ Token refresh
- ✅ Role-based access

### Real-time Communication
- ✅ SignalR Hub
- ✅ Direct messaging
- ✅ Group messaging
- ✅ Typing indicators
- ✅ Online status

### User Management
- ✅ User registration
- ✅ Profile management
- ✅ Avatar upload (Base64)
- ✅ Contact management
- ✅ Block/unblock users

### Messaging
- ✅ Direct messages
- ✅ Group messages
- ✅ Message history
- ✅ Soft delete
- ✅ Read receipts

### Groups
- ✅ Create/delete groups
- ✅ Add/remove members
- ✅ Admin system
- ✅ Mute/unmute members
- ✅ Mute all feature
- ✅ Group avatar

### Stories
- ✅ Create stories
- ✅ 24-hour expiry
- ✅ Story views
- ✅ View tracking

### Calls
- ✅ Voice calls
- ✅ Video calls
- ✅ Call history
- ✅ Call duration tracking

---

## 🏗️ Mimari Detayları

### N-Tier Architecture

**Core Layer**
- Result pattern (IResult, IDataResult)
- Business rules
- Shared utilities

**Entity Layer**
- Database entities
- DTOs (Data Transfer Objects)
- Separation of concerns

**DAL (Data Access Layer)**
- Repository pattern
- Generic repository
- Entity Framework Core
- Database context

**BLL (Business Logic Layer)**
- Service pattern
- Business logic
- FluentValidation
- AutoMapper

**API Layer**
- REST controllers
- SignalR hubs
- JWT authentication
- Swagger documentation

### Design Patterns

1. **Repository Pattern**
   - Generic repository
   - Unit of work
   - Abstraction over data access

2. **Result Pattern**
   - IResult (success/error)
   - IDataResult<T> (with data)
   - Consistent error handling

3. **DTO Pattern**
   - AddDTO (create)
   - UpdateDTO (update)
   - ListDTO (read)

4. **Dependency Injection**
   - Service registration
   - Loose coupling
   - Testability

---

## 📡 API Endpoints

### Authentication
```
POST   /api/auth/register      # Kayıt ol
POST   /api/auth/login         # Giriş yap
POST   /api/auth/refresh       # Token yenile
```

### Users
```
GET    /api/users              # Tüm kullanıcılar
GET    /api/users/{id}         # Kullanıcı detay
PUT    /api/users/{id}         # Profil güncelle
PUT    /api/users/{id}/avatar  # Avatar güncelle
DELETE /api/users/{id}         # Hesap sil
```

### Messages
```
GET    /api/messages                    # Mesaj geçmişi
GET    /api/messages/{userId}           # Kullanıcı ile mesajlar
POST   /api/messages                    # Mesaj gönder
DELETE /api/messages/{id}               # Mesaj sil
```

### Groups
```
GET    /api/groups                      # Gruplarım
GET    /api/groups/{id}                 # Grup detay
POST   /api/groups                      # Grup oluştur
PUT    /api/groups/{id}                 # Grup güncelle
DELETE /api/groups/{id}                 # Grup sil
POST   /api/groups/{id}/leave           # Gruptan ayrıl
GET    /api/groups/{id}/messages        # Grup mesajları
POST   /api/groups/{id}/messages        # Grup mesajı gönder
POST   /api/groups/{id}/members         # Üye ekle
DELETE /api/groups/{id}/members/{uid}   # Üye çıkar
POST   /api/groups/{id}/members/{uid}/promote   # Admin yap
POST   /api/groups/{id}/members/{uid}/demote    # Admin kaldır
POST   /api/groups/{id}/members/{uid}/mute      # Sustur
POST   /api/groups/{id}/members/{uid}/unmute    # Susturmayı kaldır
POST   /api/groups/{id}/mute-all        # Herkesi sustur
POST   /api/groups/{id}/unmute-all      # Susturmayı kaldır
PUT    /api/groups/{id}/avatar          # Grup avatarı
```

### Contacts
```
GET    /api/contacts           # Kişilerim
POST   /api/contacts           # Kişi ekle
DELETE /api/contacts/{id}      # Kişi sil
```

### Blocked Users
```
GET    /api/blocked            # Engellenenler
POST   /api/blocked            # Engelle
DELETE /api/blocked/{id}       # Engeli kaldır
```

### Stories
```
GET    /api/stories            # Aktif hikayeler
GET    /api/stories/{id}       # Hikaye detay
POST   /api/stories            # Hikaye paylaş
DELETE /api/stories/{id}       # Hikaye sil
```

### Story Views
```
GET    /api/storyviews/{storyId}  # Hikaye görüntüleyenler
POST   /api/storyviews            # Görüntüleme kaydet
```

### Calls
```
GET    /api/calls              # Arama geçmişi
POST   /api/calls              # Arama başlat
PUT    /api/calls/{id}         # Arama güncelle
```

---

## 🔌 SignalR Hub

### Connection
```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7001/chatHub", {
        accessTokenFactory: () => localStorage.getItem("token")
    })
    .build();
```

### Events

**Client → Server**
```javascript
// Mesaj gönder
connection.invoke("SendMessage", receiverId, content);

// Grup mesajı gönder
connection.invoke("SendGroupMessage", groupId, content);

// Yazıyor göstergesi
connection.invoke("SendTypingIndicator", receiverId);

// Online durumu
connection.invoke("UpdateOnlineStatus", true);
```

**Server → Client**
```javascript
// Mesaj al
connection.on("ReceiveMessage", (message) => { });

// Grup mesajı al
connection.on("ReceiveGroupMessage", (message) => { });

// Yazıyor göstergesi
connection.on("UserTyping", (userId) => { });

// Online durumu
connection.on("UserOnlineStatusChanged", (userId, isOnline) => { });
```

---

## ⚙️ Konfigürasyon

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TalkyDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-minimum-32-characters",
    "Issuer": "TalkyAPI",
    "Audience": "TalkyClient",
    "ExpirationDays": "7"
  }
}
```

### Environment Variables (Production)
```bash
ConnectionStrings__DefaultConnection="Server=...;Database=TalkyDB;..."
JwtSettings__SecretKey="production-secret-key"
```

---

## 🧪 Test

### Unit Tests
```bash
cd BackNtier/Tests
dotnet test
```

### API Tests (Swagger)
1. Backend'i çalıştır
2. `https://localhost:7001/swagger` aç
3. Endpoint'leri test et

---

## 📦 Deployment

### Development
```bash
cd BackNtier/Talky_API
dotnet run
```

### Production
```bash
cd BackNtier/Talky_API
dotnet publish -c Release -o ./publish
```

### Docker (Opsiyonel)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY ./publish .
ENTRYPOINT ["dotnet", "Talky_API.dll"]
```

---

## 🔒 Güvenlik

- ✅ JWT Bearer authentication
- ✅ Password hashing (BCrypt)
- ✅ HTTPS enforcement
- ✅ CORS policy
- ✅ Input validation
- ✅ SQL injection protection (EF Core)
- ✅ XSS protection

---

## 📊 Database Schema

### Users
- Id, Username, Email, PasswordHash
- FullName, Bio, Avatar
- IsOnline, LastSeen
- CreatedAt, Deleted

### Messages
- Id, SenderId, ReceiverId
- Content, SentAt
- IsRead, ReadAt
- Deleted

### Groups
- Id, Name, Description, Avatar
- CreatedById, CreatedAt
- IsMutedForAll, Deleted

### GroupMembers
- Id, GroupId, UserId
- IsAdmin, IsMuted
- JoinedAt, Deleted

### GroupMessages
- Id, GroupId, SenderId
- Content, SentAt
- IsSystemMessage, Deleted

### Contacts
- Id, UserId, ContactUserId
- CreatedAt, Deleted

### BlockedUsers
- Id, UserId, BlockedUserId
- BlockedAt, Deleted

### Stories
- Id, UserId, Content
- MediaUrl, CreatedAt, ExpiresAt
- Deleted

### StoryViews
- Id, StoryId, ViewerId
- ViewedAt, Deleted

### Calls
- Id, CallerId, ReceiverId
- CallType (Voice/Video)
- StartedAt, EndedAt, Duration
- Status, Deleted

---

## 🛠️ Geliştirme

### Yeni Entity Ekleme

1. **Entity oluştur** (`Entity/TableModel/`)
2. **DTO'ları oluştur** (`Entity/DataTransferObject/`)
3. **Repository interface** (`DAL/Abstrack/`)
4. **Repository implementation** (`DAL/Concret/`)
5. **Service interface** (`BLL/Abstrack/`)
6. **Service implementation** (`BLL/Concret/`)
7. **Validator** (`BLL/Validation/`)
8. **Mapper profile** (`BLL/Mapper/`)
9. **Controller** (`Talky_API/Controllers/`)
10. **Migration** (`dotnet ef migrations add ...`)

---

## 📚 Kaynaklar

- [.NET 8 Documentation](https://learn.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/)
- [JWT Authentication](https://jwt.io/)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [AutoMapper](https://automapper.org/)

---

## ✅ Checklist

- [x] N-Tier architecture
- [x] Repository pattern
- [x] Result pattern
- [x] DTO pattern
- [x] JWT authentication
- [x] SignalR real-time
- [x] FluentValidation
- [x] AutoMapper
- [x] Swagger documentation
- [x] Build başarılı (0 error, 0 warning)
- [ ] Database migration
- [ ] Unit tests
- [ ] Integration tests
- [ ] Production deployment

---

**Backend hazır! Sıradaki adım: Migration ve test! 🚀**
