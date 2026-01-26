# IIS Deployment - Adım Adım Rehber

## ✅ Ön Hazırlık (Tamamlandı)
- [x] IIS kuruldu
- [x] Backend build edildi (`back/TalkyAPI/publish/`)
- [x] `appsettings.Production.json` oluşturuldu

---

## 📁 1. Dosyaları Sunucuya Kopyalama

### Lokal Makinede:
1. `back/TalkyAPI/publish/` klasörünün tamamını kopyalayın
2. Sunucuya transfer edin (RDP, FTP, veya network share ile)

### Sunucuda:
1. Hedef klasör oluşturun: `C:\inetpub\wwwroot\TalkyAPI`
2. Publish klasöründeki tüm dosyaları buraya kopyalayın

```powershell
# PowerShell ile kopyalama (lokal makineden sunucuya)
Copy-Item -Path "C:\Users\OnOff-06122024\Desktop\Talky\back\TalkyAPI\publish\*" -Destination "\\SERVER_NAME\C$\inetpub\wwwroot\TalkyAPI\" -Recurse
```

---

## 🌐 2. IIS'de Site Oluşturma

### IIS Manager'ı Açın:
1. Windows tuşu + R → `inetmgr` → Enter
2. Veya: Server Manager → Tools → Internet Information Services (IIS) Manager

### Application Pool Oluşturma:
1. Sol panelde **Application Pools**'a sağ tıklayın
2. **Add Application Pool** seçin
3. Ayarlar:
   - **Name**: `TalkyAPI`
   - **.NET CLR Version**: `No Managed Code` (ÖNEMLİ!)
   - **Managed Pipeline Mode**: `Integrated`
   - **Start application pool immediately**: ✓
4. **OK** tıklayın

### Application Pool İleri Ayarlar:
1. **TalkyAPI** pool'una sağ tıklayın → **Advanced Settings**
2. Şu ayarları yapın:
   - **Identity**: `ApplicationPoolIdentity` (varsayılan)
   - **Start Mode**: `AlwaysRunning`
   - **Idle Time-out (minutes)**: `0` (sürekli çalışsın)
3. **OK** tıklayın

### Web Site Oluşturma:
1. Sol panelde **Sites**'a sağ tıklayın
2. **Add Website** seçin
3. Ayarlar:
   - **Site name**: `TalkyAPI`
   - **Application pool**: `TalkyAPI` (dropdown'dan seçin)
   - **Physical path**: `C:\inetpub\wwwroot\TalkyAPI`
   - **Binding**:
     - Type: `http`
     - IP address: `All Unassigned`
     - Port: `5282` (veya istediğiniz port)
     - Host name: (boş bırakın veya domain adınızı yazın)
4. **OK** tıklayın

---

## 🔧 3. Klasör İzinleri

### IIS_IUSRS İzni Verme:
1. `C:\inetpub\wwwroot\TalkyAPI` klasörüne sağ tıklayın
2. **Properties** → **Security** sekmesi
3. **Edit** → **Add**
4. **IIS_IUSRS** yazın → **Check Names** → **OK**
5. İzinler:
   - ✓ Read & Execute
   - ✓ List folder contents
   - ✓ Read
6. **Apply** → **OK**

### Application Pool Identity İzni:
1. Aynı şekilde **Add** tıklayın
2. `IIS AppPool\TalkyAPI` yazın → **Check Names** → **OK**
3. Aynı izinleri verin
4. **Apply** → **OK**

---

## 🗄️ 4. Database Hazırlığı

### SQL Server Connection String Güncelleme:

`C:\inetpub\wwwroot\TalkyAPI\appsettings.Production.json` dosyasını düzenleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=TalkyDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Veya SQL Server kullanıyorsanız:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=TalkyDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  }
}
```

### Migration Uygulama:

Sunucuda PowerShell'de:

```powershell
cd C:\inetpub\wwwroot\TalkyAPI

# Migration'ları uygula
dotnet TalkyAPI.dll --migrate

# Veya manuel:
# dotnet ef database update (eğer ef tools kuruluysa)
```

**Not:** Eğer migration hatası alırsanız, lokal makinenizde database'i export edip sunucuya import edebilirsiniz.

---

## 🔐 5. Environment Variables (Opsiyonel)

### IIS'de Environment Variable Ayarlama:

1. IIS Manager'da **TalkyAPI** site'ına tıklayın
2. **Configuration Editor** açın
3. **Section**: `system.webServer/aspNetCore` seçin
4. **environmentVariables** bölümünü genişletin
5. **Add** tıklayın:
   - Name: `ASPNETCORE_ENVIRONMENT`
   - Value: `Production`
6. **Apply** tıklayın

---

## 🚀 6. Site'ı Başlatma

### Site'ı Start Edin:
1. IIS Manager'da **TalkyAPI** site'ına sağ tıklayın
2. **Manage Website** → **Start**

### Application Pool'u Start Edin:
1. **Application Pools** → **TalkyAPI**'ya sağ tıklayın
2. **Start**

---

## ✅ 7. Test Etme

### Browser'da Test:
```
http://localhost:5282/swagger
http://YOUR_SERVER_IP:5282/swagger
```

### PowerShell'de Test:
```powershell
# Health check
Invoke-WebRequest -Uri "http://localhost:5282/api/auth/login" -Method GET

# Swagger UI
Start-Process "http://localhost:5282/swagger"
```

### SignalR Test:
```
http://localhost:5282/chatHub
```

---

## 🔍 8. Troubleshooting

### Problem: 500.19 - Internal Server Error
**Çözüm:** .NET Core Hosting Bundle kurulu değil
- https://dotnet.microsoft.com/download/dotnet/8.0 adresinden indirin
- Sunucuyu yeniden başlatın

### Problem: 502.5 - Process Failure
**Çözüm:** 
1. Application Pool'un `.NET CLR Version` ayarı `No Managed Code` olmalı
2. `web.config` dosyası doğru mu kontrol edin
3. Event Viewer'da hata loglarına bakın

### Problem: Database Connection Failed
**Çözüm:**
1. Connection string'i kontrol edin
2. SQL Server çalışıyor mu kontrol edin
3. Firewall kurallarını kontrol edin
4. IIS_IUSRS kullanıcısının database'e erişimi var mı kontrol edin

### Problem: CORS Errors
**Çözüm:**
1. `appsettings.Production.json` içinde `AllowedOrigins` kontrol edin
2. Frontend URL'ini ekleyin
3. Site'ı restart edin

### Log Dosyalarını Kontrol:
```powershell
# IIS logs
Get-Content "C:\inetpub\logs\LogFiles\W3SVC*\*.log" -Tail 50

# Application logs (eğer varsa)
Get-Content "C:\inetpub\wwwroot\TalkyAPI\logs\*.log" -Tail 50

# Event Viewer
eventvwr.msc
# Windows Logs → Application
```

---

## 🔒 9. Güvenlik (Production için)

### HTTPS Yapılandırması:
1. SSL Sertifikası alın (Let's Encrypt, Cloudflare, vb.)
2. IIS Manager → **TalkyAPI** site → **Bindings**
3. **Add** → Type: `https`, Port: `443`, SSL Certificate seçin

### Firewall Kuralları:
```powershell
# Port 5282'yi aç
New-NetFirewallRule -DisplayName "TalkyAPI" -Direction Inbound -LocalPort 5282 -Protocol TCP -Action Allow

# HTTPS için port 443
New-NetFirewallRule -DisplayName "TalkyAPI HTTPS" -Direction Inbound -LocalPort 443 -Protocol TCP -Action Allow
```

### web.config Güvenlik:
`C:\inetpub\wwwroot\TalkyAPI\web.config` dosyasını kontrol edin:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
    </handlers>
    <aspNetCore processPath="dotnet" 
                arguments=".\TalkyAPI.dll" 
                stdoutLogEnabled="true" 
                stdoutLogFile=".\logs\stdout" 
                hostingModel="inprocess">
      <environmentVariables>
        <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
      </environmentVariables>
    </aspNetCore>
    <httpProtocol>
      <customHeaders>
        <remove name="X-Powered-By" />
        <add name="X-Content-Type-Options" value="nosniff" />
        <add name="X-Frame-Options" value="DENY" />
      </customHeaders>
    </httpProtocol>
  </system.webServer>
</configuration>
```

---

## 📊 10. Monitoring

### IIS Monitoring:
1. IIS Manager → **TalkyAPI** site
2. **Worker Processes** - Aktif process'leri görün
3. **Failed Request Tracing** - Hata logları

### Performance Monitor:
```powershell
# CPU ve Memory kullanımı
Get-Process -Name "w3wp" | Select-Object CPU, WorkingSet, Id
```

---

## 🎯 Deployment Checklist

- [ ] .NET 8 Hosting Bundle kuruldu
- [ ] Publish klasörü sunucuya kopyalandı
- [ ] Application Pool oluşturuldu (No Managed Code)
- [ ] Web Site oluşturuldu
- [ ] Klasör izinleri verildi (IIS_IUSRS)
- [ ] appsettings.Production.json güncellendi
- [ ] Database migration uygulandı
- [ ] Environment variable ayarlandı (Production)
- [ ] Site başlatıldı
- [ ] Swagger UI test edildi
- [ ] SignalR bağlantısı test edildi
- [ ] HTTPS yapılandırıldı (production için)
- [ ] Firewall kuralları eklendi

---

## 📞 Yardım

Sorun yaşarsanız:
1. Event Viewer loglarını kontrol edin
2. IIS logs klasörünü kontrol edin: `C:\inetpub\logs\LogFiles\`
3. Application logs: `C:\inetpub\wwwroot\TalkyAPI\logs\`

**Başarılar! 🚀**
