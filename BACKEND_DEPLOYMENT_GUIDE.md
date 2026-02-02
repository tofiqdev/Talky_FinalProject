# 🚀 Backend Deployment Rehberi

## Seçenek 1: ngrok ile Deploy (ÖNERİLEN - En Kolay)

ngrok, local backend'inizi internete açar. Ücretsiz ve 5 dakikada hazır!

### Adım 1: ngrok İndir ve Kur

1. **ngrok İndir:**
   - Git: https://ngrok.com/download
   - Windows için ZIP dosyasını indir
   - ZIP'i aç ve `ngrok.exe`'yi masaüstüne kopyala

2. **ngrok Hesabı Oluştur (Ücretsiz):**
   - Git: https://dashboard.ngrok.com/signup
   - Email ile kayıt ol
   - Dashboard'a gir

3. **Auth Token Al:**
   - Dashboard'da "Your Authtoken" bölümünü bul
   - Token'ı kopyala (örn: `2abc...xyz`)

4. **Token'ı Ayarla:**
   ```cmd
   ngrok config add-authtoken SENIN_TOKEN_BURAYA
   ```

### Adım 2: Backend'i Başlat

1. **Terminal Aç** (bu projede)
2. **Backend'i Çalıştır:**
   ```cmd
   cd BackNtier\Talky_API
   dotnet run
   ```
3. **Çalıştığını Kontrol Et:**
   - Görmeli: `Now listening on: http://localhost:5135`

### Adım 3: ngrok ile İnternete Aç

1. **Yeni Terminal Aç**
2. **ngrok Başlat:**
   ```cmd
   ngrok http 5135
   ```

3. **URL'i Kopyala:**
   ```
   Forwarding    https://abc123.ngrok-free.app -> http://localhost:5135
   ```
   - `https://abc123.ngrok-free.app` bu senin backend URL'in!

### Adım 4: Frontend'i Güncelle

1. **`.env.production` Dosyası Oluştur:**
   ```env
   VITE_API_URL=https://abc123.ngrok-free.app
   ```

2. **Frontend'i Build Et:**
   ```cmd
   npm run build
   ```

3. **Netlify'a Deploy Et:**
   ```cmd
   npm install -g netlify-cli
   netlify deploy --prod --dir=dist
   ```

### ✅ Tamamlandı!

- Backend: `https://abc123.ngrok-free.app`
- Frontend: Netlify URL'i
- Swagger: `https://abc123.ngrok-free.app/swagger`

---

## Seçenek 2: Azure App Service (Profesyonel)

### Ön Gereksinimler:
- Azure hesabı (ücretsiz $200 kredi)
- Azure CLI kurulu

### Adım 1: Azure'a Giriş

```cmd
az login
```

### Adım 2: Resource Group Oluştur

```cmd
az group create --name TalkyResourceGroup --location westeurope
```

### Adım 3: App Service Plan Oluştur

```cmd
az appservice plan create --name TalkyPlan --resource-group TalkyResourceGroup --sku F1 --is-linux
```

### Adım 4: Web App Oluştur

```cmd
az webapp create --name talky-backend-api --resource-group TalkyResourceGroup --plan TalkyPlan --runtime "DOTNET|8.0"
```

### Adım 5: Database Connection String Ayarla

```cmd
az webapp config connection-string set --name talky-backend-api --resource-group TalkyResourceGroup --connection-string-type SQLAzure --settings DefaultConnection="Server=tcp:YOUR_SERVER.database.windows.net,1433;Database=TalkyDB;User ID=YOUR_USER;Password=YOUR_PASSWORD;Encrypt=True;"
```

### Adım 6: Deploy Et

```cmd
cd BackNtier\Talky_API
dotnet publish -c Release -o ./publish
cd publish
az webapp deployment source config-zip --resource-group TalkyResourceGroup --name talky-backend-api --src publish.zip
```

### ✅ Tamamlandı!

Backend URL: `https://talky-backend-api.azurewebsites.net`

---

## Seçenek 3: Railway (Kolay + Ücretsiz)

### Adım 1: Railway Hesabı

1. Git: https://railway.app
2. GitHub ile giriş yap

### Adım 2: Yeni Proje

1. "New Project" tıkla
2. "Deploy from GitHub repo" seç
3. Repo'nu seç

### Adım 3: Ayarlar

1. **Root Directory:** `BackNtier/Talky_API`
2. **Build Command:** `dotnet publish -c Release -o out`
3. **Start Command:** `dotnet out/Talky_API.dll`

### Adım 4: Environment Variables

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
```

### ✅ Tamamlandı!

Railway otomatik URL verir: `https://talky-backend.up.railway.app`

---

## 🎯 Hangi Seçeneği Seçmeliyim?

| Özellik | ngrok | Azure | Railway |
|---------|-------|-------|---------|
| **Kurulum** | ⭐⭐⭐⭐⭐ Çok Kolay | ⭐⭐ Orta | ⭐⭐⭐⭐ Kolay |
| **Ücretsiz** | ✅ Evet | ✅ $200 kredi | ✅ 500 saat/ay |
| **Hız** | ⭐⭐⭐⭐⭐ Çok Hızlı | ⭐⭐⭐⭐ Hızlı | ⭐⭐⭐⭐ Hızlı |
| **Kalıcı URL** | ❌ Her restart'ta değişir | ✅ Kalıcı | ✅ Kalıcı |
| **Database** | ❌ Local | ✅ Azure SQL | ✅ PostgreSQL |
| **Önerilen** | Test için | Production | Hobby projeler |

---

## 📝 Önemli Notlar

### ngrok Kullanıyorsan:
- ⚠️ Her ngrok restart'ında URL değişir
- ⚠️ Frontend'i her seferinde yeniden deploy etmen gerekir
- ✅ Test ve demo için mükemmel
- ✅ Bilgisayarın açık olmalı

### Azure/Railway Kullanıyorsan:
- ✅ URL kalıcı
- ✅ 7/24 çalışır
- ✅ Production-ready
- ⚠️ Database migration gerekir

---

## 🆘 Sorun Giderme

### ngrok "command not found"
```cmd
# ngrok.exe'nin bulunduğu klasöre git
cd C:\Users\KULLANICI_ADIN\Desktop
ngrok http 5135
```

### Backend başlamıyor
```cmd
# Port kullanımda mı kontrol et
netstat -ano | findstr :5135

# Eğer kullanımdaysa, process'i kapat
taskkill /PID PROCESS_ID /F
```

### CORS hatası
`Program.cs`'de CORS ayarlarını kontrol et:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.SetIsOriginAllowed(_ => true)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials());
});
```

---

## 🎉 Başarılı Deploy Sonrası

1. **Swagger Test Et:**
   - `https://BACKEND_URL/swagger`
   - Auth endpoint'lerini test et

2. **Frontend'i Güncelle:**
   - `.env.production` dosyasını oluştur
   - Backend URL'ini ekle
   - Yeniden build et

3. **Test Et:**
   - Login yap
   - Mesaj gönder
   - Film odası oluştur

---

## 📞 Yardım

Sorun yaşarsan:
1. Console log'larına bak (F12)
2. Backend log'larına bak
3. ngrok dashboard'a bak: http://localhost:4040

İyi şanslar! 🚀
