# ⚡ Hızlı Başlangıç - Backend Deploy (5 Dakika)

## 🎯 En Kolay Yöntem: ngrok

### 1️⃣ ngrok İndir (2 dakika)

1. **İndir:** https://ngrok.com/download
2. **Aç:** ZIP dosyasını aç
3. **Kopyala:** `ngrok.exe`'yi bu proje klasörüne kopyala

### 2️⃣ ngrok Hesabı (1 dakika)

1. **Kayıt Ol:** https://dashboard.ngrok.com/signup
2. **Token Al:** Dashboard'da "Your Authtoken" bölümünden kopyala
3. **Ayarla:** Terminal'de çalıştır:
   ```cmd
   ngrok config add-authtoken SENIN_TOKEN_BURAYA
   ```

### 3️⃣ Başlat (1 dakika)

**Otomatik (Önerilen):**
```cmd
start-ngrok.bat
```

**Manuel:**
```cmd
# Terminal 1: Backend
cd BackNtier\Talky_API
dotnet run

# Terminal 2: ngrok
ngrok http 5135
```

### 4️⃣ URL'i Kopyala

ngrok çıktısında göreceksin:
```
Forwarding    https://abc123.ngrok-free.app -> http://localhost:5135
```

**Bu URL'i kopyala:** `https://abc123.ngrok-free.app`

### 5️⃣ Frontend'i Güncelle (1 dakika)

1. **Dosya Oluştur:** `.env.production`
   ```env
   VITE_API_URL=https://abc123.ngrok-free.app
   ```

2. **Build Et:**
   ```cmd
   npm run build
   ```

3. **Deploy Et:**
   ```cmd
   netlify deploy --prod --dir=dist
   ```

## ✅ Tamamlandı!

- **Backend:** https://abc123.ngrok-free.app
- **Swagger:** https://abc123.ngrok-free.app/swagger
- **Frontend:** Netlify URL'i

---

## 🎮 Test Et

1. **Swagger'ı Aç:** https://abc123.ngrok-free.app/swagger
2. **Register Endpoint'ini Test Et:**
   - POST `/api/auth/register`
   - Body:
     ```json
     {
       "username": "test",
       "email": "test@test.com",
       "password": "Test123!"
     }
     ```
3. **200 OK Dönerse Başarılı!** ✅

---

## ⚠️ Önemli Notlar

### ngrok Ücretsiz Sınırlamalar:
- ✅ Sınırsız kullanım
- ⚠️ Her restart'ta URL değişir
- ⚠️ Bilgisayar açık olmalı
- ⚠️ 2 saat sonra otomatik kapanır (yeniden başlat)

### URL Değişirse:
1. Yeni ngrok URL'ini kopyala
2. `.env.production` dosyasını güncelle
3. Frontend'i yeniden build et: `npm run build`
4. Yeniden deploy et: `netlify deploy --prod --dir=dist`

---

## 🆘 Sorun mu Var?

### "ngrok: command not found"
- `ngrok.exe`'yi proje klasörüne kopyaladın mı?
- Veya tam yol ile çalıştır: `C:\path\to\ngrok.exe http 5135`

### Backend başlamıyor
```cmd
# Port kullanımda mı?
netstat -ano | findstr :5135

# Process'i kapat
taskkill /PID PROCESS_ID /F
```

### CORS hatası
- ngrok URL'ini `.env.production`'a doğru yazdın mı?
- Frontend'i yeniden build ettin mi?

---

## 🚀 Alternatif: Railway (Kalıcı URL)

Eğer her seferinde URL değiştirmek istemiyorsan:

1. **Railway'e Git:** https://railway.app
2. **GitHub ile Giriş Yap**
3. **New Project → Deploy from GitHub**
4. **Repo'nu Seç**
5. **Settings:**
   - Root Directory: `BackNtier/Talky_API`
   - Build Command: `dotnet publish -c Release -o out`
   - Start Command: `dotnet out/Talky_API.dll`
6. **Environment Variables:**
   ```
   ASPNETCORE_ENVIRONMENT=Production
   ASPNETCORE_URLS=http://0.0.0.0:$PORT
   ```

Railway otomatik URL verir ve kalıcıdır! 🎉

---

## 📞 Yardım Lazım?

1. **Console'u Aç:** F12
2. **Network Tab'ına Bak:** API istekleri görünüyor mu?
3. **Backend Log'larına Bak:** Terminal'de hata var mı?
4. **ngrok Dashboard:** http://localhost:4040 (ngrok çalışırken)

İyi şanslar! 🚀
