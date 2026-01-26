# ngrok ile Localhost'u İnternete Açma

## 🚀 Hızlı Kurulum

### 1. ngrok İndirin
https://ngrok.com/download

### 2. Kayıt Olun (Ücretsiz)
https://dashboard.ngrok.com/signup

### 3. Auth Token Alın
Dashboard'dan auth token'ınızı kopyalayın

### 4. Kurulum

```bash
# ngrok'u indirip extract edin
# PowerShell'de:

# Auth token'ı ayarlayın
.\ngrok.exe config add-authtoken YOUR_AUTH_TOKEN

# Backend'i başlatın (başka bir terminal'de)
cd back/TalkyAPI
dotnet run

# ngrok'u başlatın
.\ngrok.exe http 5282
```

### 5. URL'i Kopyalayın

ngrok size şöyle bir URL verecek:
```
https://abc123.ngrok.io -> http://localhost:5282
```

### 6. Frontend'de API URL'ini Güncelleyin

`.env.production` dosyası oluşturun:

```env
VITE_API_URL=https://abc123.ngrok.io
```

`src/services/apiService.ts` güncelleyin:

```typescript
const API_BASE_URL = import.meta.env.VITE_API_URL || '/api';
```

### 7. Frontend'i Yeniden Build Edin

```bash
npm run build
netlify deploy --prod
```

## ⚠️ Önemli Notlar:

- **Ücretsiz ngrok**: Her başlatmada URL değişir
- **Ücretli ngrok**: Sabit URL alabilirsiniz ($8/ay)
- **Bilgisayarınız açık olmalı**: Backend çalışmaya devam etmeli
- **Sadece test için**: Production için uygun değil

## 🔒 Güvenlik

Backend'de ngrok URL'ini CORS'a ekleyin:

`appsettings.json`:
```json
{
  "Cors": {
    "AllowedOrigins": [
      "https://your-app.netlify.app",
      "https://abc123.ngrok.io"
    ]
  }
}
```

---

## 🎯 Kullanım Senaryosu

1. Backend'i lokal'de çalıştırın: `dotnet run`
2. ngrok'u başlatın: `ngrok http 5282`
3. ngrok URL'ini frontend'e ekleyin
4. Frontend'i Netlify'a deploy edin
5. Test edin!

**Not:** Her ngrok restart'ında URL değişir, frontend'i tekrar deploy etmeniz gerekir.
