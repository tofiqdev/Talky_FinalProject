# 🚀 ngrok ile Netlify Deployment - Adım Adım

## ✅ Hazırlık (Tamamlandı)
- [x] CORS ayarları güncellendi (tüm origin'lere izin)
- [x] Frontend environment variable desteği eklendi
- [x] SignalR servisi güncellendi

---

## 📋 Adım 1: ngrok Kurulumu

### 1.1 ngrok İndirin
https://ngrok.com/download

Windows için: `ngrok-v3-stable-windows-amd64.zip`

### 1.2 Extract Edin
Zip dosyasını açın ve `ngrok.exe`'yi kolay erişilebilir bir yere koyun:
```
C:\ngrok\ngrok.exe
```

### 1.3 Kayıt Olun (Ücretsiz)
https://dashboard.ngrok.com/signup

### 1.4 Auth Token Alın
1. Dashboard'a girin: https://dashboard.ngrok.com/get-started/your-authtoken
2. Token'ı kopyalayın (örn: `2abc123def456...`)

### 1.5 Auth Token'ı Ayarlayın
PowerShell'de:
```powershell
cd C:\ngrok
.\ngrok.exe config add-authtoken YOUR_AUTH_TOKEN_HERE
```

---

## 🔧 Adım 2: Backend'i Başlatın

### 2.1 Backend'i Çalıştırın
Yeni bir PowerShell penceresi açın:

```powershell
cd C:\Users\OnOff-06122024\Desktop\Talky\back\TalkyAPI
dotnet run
```

Backend şu adreste çalışacak:
```
http://localhost:5282
```

**Bu pencereyi açık bırakın!**

---

## 🌐 Adım 3: ngrok'u Başlatın

### 3.1 ngrok'u Çalıştırın
**Yeni bir PowerShell penceresi** açın:

```powershell
cd C:\ngrok
.\ngrok.exe http 5282
```

### 3.2 ngrok URL'ini Kopyalayın
Ekranda şöyle bir çıktı göreceksiniz:

```
ngrok

Session Status                online
Account                       your-email@example.com
Version                       3.x.x
Region                        United States (us)
Latency                       -
Web Interface                 http://127.0.0.1:4040
Forwarding                    https://abc123def456.ngrok-free.app -> http://localhost:5282

Connections                   ttl     opn     rt1     rt5     p50     p90
                              0       0       0.00    0.00    0.00    0.00
```

**HTTPS URL'ini kopyalayın:**
```
https://abc123def456.ngrok-free.app
```

**Bu pencereyi de açık bırakın!**

### 3.3 ngrok URL'ini Test Edin
Browser'da açın:
```
https://abc123def456.ngrok-free.app/swagger
```

Swagger UI görünüyorsa başarılı! ✅

---

## 🎨 Adım 4: Frontend'i Hazırlayın

### 4.1 .env.production Dosyasını Güncelleyin

`.env.production` dosyasını açın ve ngrok URL'inizi yazın:

```env
VITE_API_URL=https://abc123def456.ngrok-free.app
```

**ÖNEMLİ:** URL'in sonunda `/` olmamalı!

### 4.2 Frontend'i Build Edin

```powershell
# Talky klasöründe
npm run build
```

Build başarılı olursa `dist/` klasörü oluşacak.

---

## 🚀 Adım 5: Netlify'a Deploy

### Seçenek A: Netlify CLI (Önerilen)

```powershell
# Netlify CLI kurulumu (ilk kez)
npm install -g netlify-cli

# Netlify'a login
netlify login

# Deploy
netlify deploy --prod --dir=dist
```

### Seçenek B: Netlify Dashboard (Manuel)

1. https://app.netlify.com/ adresine gidin
2. **Add new site** → **Deploy manually**
3. `dist/` klasörünü sürükle-bırak
4. Deploy tamamlanana kadar bekleyin

### 5.1 Netlify URL'ini Alın

Deploy tamamlandıktan sonra size bir URL verilecek:
```
https://your-app-name.netlify.app
```

---

## ✅ Adım 6: Test Edin

### 6.1 Netlify Site'ını Açın
```
https://your-app-name.netlify.app
```

### 6.2 Kayıt Olun / Giriş Yapın
- Yeni bir hesap oluşturun
- Veya mevcut hesabınızla giriş yapın

### 6.3 Mesajlaşmayı Test Edin
- Başka bir kullanıcıyla mesajlaşın
- Grup oluşturun
- Story paylaşın

### 6.4 Browser Console'u Kontrol Edin
F12 → Console sekmesi

Şu mesajları görmelisiniz:
```
SignalR: Starting connection with token: Token exists
SignalR: Connected successfully
```

---

## 🔍 Troubleshooting

### Problem: "ngrok not found"
**Çözüm:** ngrok.exe'nin tam yolunu kullanın:
```powershell
C:\ngrok\ngrok.exe http 5282
```

### Problem: "Invalid authtoken"
**Çözüm:** Token'ı tekrar ayarlayın:
```powershell
.\ngrok.exe config add-authtoken YOUR_TOKEN
```

### Problem: "ERR_NGROK_3200"
**Çözüm:** Ücretsiz ngrok'ta her 2 saatte bir yeniden başlatmanız gerekir. Veya ücretli plana geçin.

### Problem: CORS Error
**Çözüm:** Backend'i yeniden başlatın:
```powershell
# Backend penceresinde Ctrl+C
dotnet run
```

### Problem: SignalR Connection Failed
**Çözüm:** 
1. ngrok URL'inin doğru olduğundan emin olun
2. `.env.production` dosyasını kontrol edin
3. Frontend'i yeniden build edin: `npm run build`
4. Netlify'a yeniden deploy edin

### Problem: "Visit Site" Uyarısı
ngrok ücretsiz versiyonunda ilk ziyarette bir uyarı sayfası gösterir.
**Çözüm:** "Visit Site" butonuna tıklayın. Veya ücretli plana geçin.

---

## 📊 ngrok Dashboard

ngrok çalışırken şu adresten istekleri izleyebilirsiniz:
```
http://localhost:4040
```

Burada:
- Gelen tüm HTTP istekleri
- Request/Response detayları
- Hata logları

görülebilir.

---

## ⚠️ Önemli Notlar

### Ücretsiz ngrok Limitleri:
- ✅ HTTPS desteği
- ✅ Sınırsız istek
- ❌ URL her başlatmada değişir
- ❌ 2 saatte bir yeniden başlatma gerekir
- ❌ "Visit Site" uyarı sayfası

### Ücretli ngrok ($8/ay):
- ✅ Sabit URL (custom domain)
- ✅ Uyarı sayfası yok
- ✅ Sürekli çalışma
- ✅ Daha fazla bağlantı

---

## 🔄 Her Seferinde Yapılacaklar

ngrok URL'i her değiştiğinde:

1. **ngrok'u başlatın** → Yeni URL alın
2. **`.env.production`'ı güncelleyin** → Yeni URL'i yazın
3. **Frontend'i build edin** → `npm run build`
4. **Netlify'a deploy edin** → `netlify deploy --prod --dir=dist`

---

## 💡 İpuçları

### Sabit URL İstiyorsanız:
```powershell
# Ücretli ngrok ile
.\ngrok.exe http 5282 --domain=your-custom-domain.ngrok.app
```

### Birden Fazla Port:
```powershell
# Backend: 5282
.\ngrok.exe http 5282

# Başka bir servis: 3000
.\ngrok.exe http 3000
```

### Log Dosyası:
```powershell
.\ngrok.exe http 5282 --log=ngrok.log
```

---

## 🎯 Checklist

- [ ] ngrok indirildi ve kuruldu
- [ ] Auth token ayarlandı
- [ ] Backend başlatıldı (localhost:5282)
- [ ] ngrok başlatıldı
- [ ] ngrok URL kopyalandı
- [ ] .env.production güncellendi
- [ ] Frontend build edildi
- [ ] Netlify'a deploy edildi
- [ ] Site test edildi
- [ ] SignalR bağlantısı çalışıyor

---

## 🚀 Başarılar!

Artık Netlify'daki frontend'iniz, lokal backend'inizle iletişim kuruyor!

**Unutmayın:** 
- Backend ve ngrok'u sürekli çalışır durumda tutun
- Bilgisayarınız kapalıysa site çalışmaz
- Production için gerçek bir sunucuya deploy edin
