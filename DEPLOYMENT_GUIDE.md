# Talky Backend Deployment Guide

## 📋 İçindekiler
1. [Backend Build](#backend-build)
2. [Database Hazırlığı](#database-hazırlığı)
3. [Deployment Seçenekleri](#deployment-seçenekleri)
4. [Production Ayarları](#production-ayarları)
5. [Frontend Build ve Deploy](#frontend-build-ve-deploy)

---

## 🔨 Backend Build

### 1. Production Build Oluşturma

```bash
cd back/TalkyAPI

# Release mode'da build
dotnet build --configuration Release

# Publish (tüm bağımlılıklarla birlikte)
dotnet publish --configuration Release --output ./publish
```

Bu komut `back/TalkyAPI/publish/` klasöründe deploy edilmeye hazır dosyalar oluşturur.

### 2. Self-Contained Build (Sunucuda .NET yüklü değilse)

```bash
# Windows için
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish

# Linux için
dotnet publish -c Release -r linux-x64 --self-contained true -o ./publish

# macOS için
dotnet publish -c Release -r osx-x64 --self-contained true -o ./publish
```

---

## 🗄️ Database Hazırlığı

### 1. Production Database Connection String

`appsettings.Production.json` dosyası oluşturun:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=TalkyDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "YOUR_SUPER_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG",
    "Issuer": "TalkyAPI",
    "Audience": "TalkyClient",
    "ExpiryInDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Cors": {
    "AllowedOrigins": ["https://yourdomain.com", "https://www.yourdomain.com"]
  }
}
```

### 2. Database Migration Uygulama

Production sunucuda:

```bash
# Migration'ları uygula
dotnet ef database update --connection "YOUR_CONNECTION_STRING"

# Veya publish klasöründen
cd publish
dotnet TalkyAPI.dll --migrate
```

---

## 🚀 Deployment Seçenekleri

### Seçenek 1: IIS (Windows Server)

#### Adımlar:

1. **IIS Kurulumu**
   - Windows Server'da IIS'i etkinleştirin
   - .NET Core Hosting Bundle'ı yükleyin: https://dotnet.microsoft.com/download/dotnet/8.0

2. **Site Oluşturma**
   - IIS Manager'ı açın
   - Sites → Add Website
   - Physical path: `C:\inetpub\wwwroot\TalkyAPI` (publish klasörünüzün yolu)
   - Binding: HTTP (80) veya HTTPS (443)

3. **Application Pool Ayarları**
   - .NET CLR Version: No Managed Code
   - Managed Pipeline Mode: Integrated

4. **Dosyaları Kopyalama**
   ```bash
   # Publish klasörünü sunucuya kopyalayın
   xcopy /E /I publish C:\inetpub\wwwroot\TalkyAPI
   ```

5. **web.config Kontrolü**
   Otomatik oluşturulur, ancak kontrol edin:
   ```xml
   <?xml version="1.0" encoding="utf-8"?>
   <configuration>
     <system.webServer>
       <handlers>
         <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
       </handlers>
       <aspNetCore processPath="dotnet" arguments=".\TalkyAPI.dll" stdoutLogEnabled="false" stdoutLogFile=".\logs\stdout" hostingModel="inprocess" />
     </system.webServer>
   </configuration>
   ```

---

### Seçenek 2: Linux Server (Ubuntu/Debian)

#### 1. .NET Runtime Kurulumu

```bash
# .NET 8 SDK/Runtime yükle
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0

# Veya apt ile
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

#### 2. Nginx Reverse Proxy Kurulumu

```bash
sudo apt-get install nginx
```

Nginx config (`/etc/nginx/sites-available/talky`):

```nginx
server {
    listen 80;
    server_name yourdomain.com www.yourdomain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

```bash
# Config'i etkinleştir
sudo ln -s /etc/nginx/sites-available/talky /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

#### 3. Systemd Service Oluşturma

`/etc/systemd/system/talky.service`:

```ini
[Unit]
Description=Talky API
After=network.target

[Service]
WorkingDirectory=/var/www/talky
ExecStart=/usr/bin/dotnet /var/www/talky/TalkyAPI.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=talky-api
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

```bash
# Service'i başlat
sudo systemctl enable talky
sudo systemctl start talky
sudo systemctl status talky
```

#### 4. SSL/HTTPS (Let's Encrypt)

```bash
sudo apt-get install certbot python3-certbot-nginx
sudo certbot --nginx -d yourdomain.com -d www.yourdomain.com
```

---

### Seçenek 3: Docker

#### Dockerfile Oluşturma

`back/TalkyAPI/Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TalkyAPI.csproj", "./"]
RUN dotnet restore "TalkyAPI.csproj"
COPY . .
RUN dotnet build "TalkyAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TalkyAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TalkyAPI.dll"]
```

#### Docker Compose

`docker-compose.yml`:

```yaml
version: '3.8'

services:
  talky-api:
    build:
      context: ./back/TalkyAPI
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=TalkyDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;
    depends_on:
      - db
    restart: unless-stopped

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
    restart: unless-stopped

volumes:
  sqldata:
```

```bash
# Build ve çalıştır
docker-compose up -d

# Logları görüntüle
docker-compose logs -f talky-api
```

---

### Seçenek 4: Azure App Service

#### 1. Azure CLI ile Deploy

```bash
# Azure'a login
az login

# Resource group oluştur
az group create --name TalkyResourceGroup --location eastus

# App Service plan oluştur
az appservice plan create --name TalkyPlan --resource-group TalkyResourceGroup --sku B1 --is-linux

# Web app oluştur
az webapp create --resource-group TalkyResourceGroup --plan TalkyPlan --name talky-api --runtime "DOTNETCORE:8.0"

# Deploy
cd back/TalkyAPI
az webapp up --name talky-api --resource-group TalkyResourceGroup
```

#### 2. Azure SQL Database

```bash
# SQL Server oluştur
az sql server create --name talky-sql-server --resource-group TalkyResourceGroup --location eastus --admin-user sqladmin --admin-password YourPassword123!

# Database oluştur
az sql db create --resource-group TalkyResourceGroup --server talky-sql-server --name TalkyDB --service-objective S0

# Connection string'i app settings'e ekle
az webapp config connection-string set --name talky-api --resource-group TalkyResourceGroup --connection-string-type SQLAzure --settings DefaultConnection="Server=tcp:talky-sql-server.database.windows.net,1433;Database=TalkyDB;User ID=sqladmin;Password=YourPassword123!;Encrypt=True;"
```

---

## ⚙️ Production Ayarları

### 1. appsettings.Production.json Kontrol Listesi

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "PRODUCTION_CONNECTION_STRING"
  },
  "Jwt": {
    "Key": "STRONG_SECRET_KEY_MIN_32_CHARS",
    "Issuer": "TalkyAPI",
    "Audience": "TalkyClient",
    "ExpiryInDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Cors": {
    "AllowedOrigins": [
      "https://yourdomain.com",
      "https://www.yourdomain.com"
    ]
  }
}
```

### 2. Program.cs CORS Güncellemesi

```csharp
// Production için CORS ayarları
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
            ?? new[] { "https://yourdomain.com" };
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
```

### 3. HTTPS Zorunlu Kılma

```csharp
// Program.cs'de
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
    app.UseHsts();
}
```

---

## 🌐 Frontend Build ve Deploy

### 1. Frontend Build

```bash
# Production build
npm run build

# Build çıktısı: dist/ klasörü
```

### 2. Environment Variables

`.env.production` dosyası oluşturun:

```env
VITE_API_URL=https://api.yourdomain.com
```

`vite.config.ts` güncelleyin:

```typescript
export default defineConfig({
  server: {
    proxy: {
      '/api': {
        target: process.env.VITE_API_URL || 'http://localhost:5282',
        changeOrigin: true,
        secure: false,
      }
    }
  }
})
```

### 3. Frontend Deploy Seçenekleri

#### A. Netlify

```bash
# Netlify CLI
npm install -g netlify-cli
netlify login
netlify deploy --prod --dir=dist
```

#### B. Vercel

```bash
# Vercel CLI
npm install -g vercel
vercel --prod
```

#### C. Nginx (Kendi Sunucunuz)

```nginx
server {
    listen 80;
    server_name yourdomain.com;

    root /var/www/talky/dist;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /api {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

---

## 🔍 Deployment Sonrası Kontroller

### 1. Health Check

```bash
# API health check
curl https://api.yourdomain.com/api/health

# SignalR bağlantı testi
curl https://api.yourdomain.com/chatHub
```

### 2. Database Bağlantı Testi

```bash
# SQL Server bağlantısını test et
sqlcmd -S your-server.database.windows.net -U sqladmin -P YourPassword -d TalkyDB -Q "SELECT @@VERSION"
```

### 3. Log Kontrolü

```bash
# Linux systemd logs
sudo journalctl -u talky -f

# Docker logs
docker-compose logs -f talky-api

# IIS logs
# C:\inetpub\logs\LogFiles\
```

---

## 🛡️ Güvenlik Önerileri

1. **Secrets Management**
   - Hassas bilgileri (connection string, JWT key) environment variables'da saklayın
   - Azure Key Vault veya AWS Secrets Manager kullanın

2. **HTTPS Zorunlu**
   - Tüm trafiği HTTPS'e yönlendirin
   - HSTS header'ları ekleyin

3. **Rate Limiting**
   ```csharp
   // Program.cs
   builder.Services.AddRateLimiter(options =>
   {
       options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
           RateLimitPartition.GetFixedWindowLimiter(
               partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
               factory: partition => new FixedWindowRateLimiterOptions
               {
                   AutoReplenishment = true,
                   PermitLimit = 100,
                   QueueLimit = 0,
                   Window = TimeSpan.FromMinutes(1)
               }));
   });
   ```

4. **Database Backup**
   - Otomatik backup planı oluşturun
   - Backup'ları farklı bir lokasyonda saklayın

---

## 📊 Monitoring ve Logging

### Application Insights (Azure)

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### Serilog

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
```

```csharp
// Program.cs
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));
```

---

## 🚨 Troubleshooting

### Problem: 502 Bad Gateway
- Backend çalışıyor mu kontrol edin: `systemctl status talky`
- Port'ların açık olduğunu kontrol edin: `netstat -tulpn | grep 5000`

### Problem: Database Connection Failed
- Connection string'i kontrol edin
- Firewall kurallarını kontrol edin
- SQL Server'ın çalıştığını kontrol edin

### Problem: CORS Errors
- AllowedOrigins'i kontrol edin
- Credentials ayarlarını kontrol edin
- Preflight request'leri kontrol edin

---

## 📝 Deployment Checklist

- [ ] Production connection string güncellendi
- [ ] JWT secret key güncellendi
- [ ] CORS allowed origins güncellendi
- [ ] HTTPS yapılandırıldı
- [ ] Database migration'ları uygulandı
- [ ] Environment variables ayarlandı
- [ ] Logging yapılandırıldı
- [ ] Backup planı oluşturuldu
- [ ] Health check endpoint'i test edildi
- [ ] SignalR bağlantısı test edildi
- [ ] Frontend API URL'i güncellendi
- [ ] SSL sertifikası yüklendi
- [ ] Monitoring kuruldu

---

## 📞 Yardım ve Destek

Deployment sırasında sorun yaşarsanız:
- .NET Documentation: https://docs.microsoft.com/aspnet/core/host-and-deploy/
- SignalR Deployment: https://docs.microsoft.com/aspnet/core/signalr/scale
- Azure Deployment: https://docs.microsoft.com/azure/app-service/

---

**Not:** Bu rehber genel bir deployment guide'dır. Özel sunucu yapılandırmanıza göre ayarlamalar gerekebilir.
