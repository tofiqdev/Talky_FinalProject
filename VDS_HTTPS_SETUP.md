# VDS'te HTTPS ile Backend Çalıştırma

## Sorun
Netlify (HTTPS) → VDS Backend (HTTP) bağlantısı tarayıcı tarafından engelleniyor.

## Çözüm: ngrok ile HTTPS

### Adım 1: VDS'te ngrok İndir

1. Git: https://ngrok.com/download
2. Windows 64-bit ZIP'i indir
3. ZIP'i aç ve `ngrok.exe`'yi `C:\Talky\` klasörüne kopyala

### Adım 2: ngrok Hesabı Oluştur

1. Git: https://dashboard.ngrok.com/signup
2. Ücretsiz hesap oluştur
3. Dashboard'dan "Your Authtoken" kopyala

### Adım 3: VDS'te ngrok Ayarla

CMD açın:
```cmd
cd C:\Talky
ngrok config add-authtoken SIZIN_TOKEN_BURAYA
```

### Adım 4: Backend ve ngrok'u Başlat

**Terminal 1 - Backend:**
```cmd
cd C:\Talky\Backend
start.bat
```

**Terminal 2 - ngrok:**
```cmd
cd C:\Talky
ngrok http 5000
```

### Adım 5: HTTPS URL'i Kopyala

ngrok çıktısında göreceksiniz:
```
Forwarding    https://abc123.ngrok-free.app -> http://localhost:5000
```

Bu HTTPS URL'i kopyalayın: `https://abc123.ngrok-free.app`

### Adım 6: Frontend'i Güncelle

Kendi bilgisayarınızda `.env.production` dosyasını güncelleyin:
```env
VITE_API_URL=https://abc123.ngrok-free.app
```

### Adım 7: Yeniden Deploy

```cmd
npm run build
netlify deploy --prod --dir=dist
```

## ✅ Tamamlandı!

Artık:
- Frontend: https://talkychat.netlify.app (HTTPS)
- Backend: https://abc123.ngrok-free.app (HTTPS)
- Her ikisi de HTTPS, tarayıcı engellemez!

## ⚠️ Önemli Notlar

1. **ngrok URL'i değişir:** Her ngrok restart'ında URL değişir, frontend'i yeniden deploy etmeniz gerekir
2. **Ücretsiz limit:** ngrok free plan'da bazı limitler var
3. **Kalıcı çözüm:** Production için SSL sertifikası kurun veya Cloudflare kullanın

## 🔄 Alternatif: Cloudflare Tunnel (Kalıcı URL)

Kalıcı URL istiyorsanız Cloudflare Tunnel kullanın:

1. Cloudflare hesabı oluştur (ücretsiz)
2. Domain ekle (varsa) veya Cloudflare'in verdiği subdomain kullan
3. Tunnel oluştur ve VDS'e bağla

Detaylar: https://developers.cloudflare.com/cloudflare-one/connections/connect-apps/
