# Talky Backend - Deployment Guide

## 📦 Published Files

Bu klasör production-ready backend dosyalarını içerir.

### ✅ Build Bilgileri
- **Framework**: .NET 8.0
- **Build Type**: Release
- **Build Date**: 1 Şubat 2026
- **Architecture**: N-Tier (Core → Entity → DAL → BLL → API)

## 🚀 Deployment Seçenekleri

### 1. IIS (Windows Server)
```powershell
# IIS'e deploy etmek için:
1. IIS Manager'ı aç
2. Yeni bir Application Pool oluştur (.NET CLR Version: No Managed Code)
3. Yeni bir Website/Application oluştur
4. Physical path olarak bu klasörü seç
5. Application Pool'u seç
6. appsettings.json'da connection string'i güncelle
```

### 2. Kestrel (Self-Hosted)
```bash
# Doğrudan çalıştırmak için:
dotnet Talky_API.dll

# Veya Windows'ta:
Talky_API.exe
```

### 3. Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY . .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Talky_API.dll"]
```

### 4. Azure App Service
1. Azure Portal'da yeni App Service oluştur
2. Deployment Center → FTP veya GitHub Actions
3. Bu klasörü upload et
4. Connection string'i Application Settings'den ayarla

## ⚙️ Konfigürasyon

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TalkyDB;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-here-min-32-characters",
    "Issuer": "TalkyAPI",
    "Audience": "TalkyClient",
    "ExpirationInDays": 7
  }
}
```

### Environment Variables (Production)
```bash
ConnectionStrings__DefaultConnection="Server=your-server;Database=TalkyDB;User Id=sa;Password=***;TrustServerCertificate=true;"
JwtSettings__SecretKey="your-production-secret-key-min-32-chars"
ASPNETCORE_ENVIRONMENT=Production
```

## 🗄️ Database Setup

### SQL Server Migration
```bash
# Migration'ları uygulamak için:
dotnet ef database update --project DAL.csproj --startup-project Talky_API.csproj

# Veya SQL Script oluştur:
dotnet ef migrations script --output migration.sql
```

### Mevcut Migration'lar
- InitialCreate
- UpdateAvatarColumnsToMax
- FixAvatarMaxLength
- AddMovieRoomFeature

## 📡 API Endpoints

### Base URL
- Development: `http://localhost:5135`
- Production: `https://your-domain.com`

### Swagger UI
- URL: `/swagger` veya `/`
- Sadece Development ortamında aktif

### SignalR Hubs
- Chat Hub: `/chatHub`
- Movie Room Hub: `/movieRoomHub`

## 🔐 Security Checklist

- [ ] JWT SecretKey değiştirildi (min 32 karakter)
- [ ] Connection string production'a güncellendi
- [ ] CORS policy production domain'e güncellendi
- [ ] HTTPS sertifikası yapılandırıldı
- [ ] Database backup stratejisi oluşturuldu
- [ ] Logging yapılandırıldı (Application Insights, Serilog, vb.)

## 📊 API Features

### Authentication
- JWT Bearer Token
- 7 gün geçerlilik
- BCrypt password hashing

### Real-time Communication
- SignalR WebSocket/LongPolling
- Chat Hub (direkt mesajlaşma)
- Movie Room Hub (senkronize video izleme)

### Database
- SQL Server / LocalDB
- Entity Framework Core 8.0
- Code-First approach
- Soft delete pattern

### API Endpoints (84 total)
- Auth: 4 endpoints
- Users: 8 endpoints
- Messages: 5 endpoints
- Groups: 16 endpoints
- Stories: 6 endpoints
- Calls: 5 endpoints
- Contacts: 6 endpoints
- BlockedUsers: 7 endpoints
- GroupMember: 5 endpoints
- GroupMessage: 5 endpoints
- StoryView: 5 endpoints
- MovieRooms: 11 endpoints
- SignalR Hubs: 2 hubs

## 🐛 Troubleshooting

### Port Already in Use
```bash
# Windows'ta port'u kontrol et:
netstat -ano | findstr :5135

# Process'i kapat:
taskkill /PID <process_id> /F
```

### Database Connection Error
- Connection string'i kontrol et
- SQL Server'ın çalıştığından emin ol
- Firewall kurallarını kontrol et

### SignalR Connection Failed
- CORS policy'yi kontrol et
- WebSocket desteğini kontrol et (IIS için)
- JWT token'ın geçerli olduğundan emin ol

## 📝 Logs

### Log Locations
- Console (Development)
- Application Insights (Production)
- File System (opsiyonel)

### Log Levels
- Information: Genel bilgiler
- Warning: Uyarılar
- Error: Hatalar
- Critical: Kritik hatalar

## 🔄 Updates

### Yeni Versiyon Deploy
1. Uygulamayı durdur
2. Yeni dosyaları kopyala
3. appsettings.json'ı koru
4. Database migration'ları çalıştır
5. Uygulamayı başlat

## 📞 Support

- GitHub: https://github.com/tofiqdev
- Email: support@talky.com

---

**Version**: 1.0.0  
**Build Date**: 1 Şubat 2026  
**Framework**: .NET 8.0
