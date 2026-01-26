# 🚀 Netlify'a Deploy - Son Adımlar

## ✅ Tamamlananlar:
- [x] Backend çalışıyor (localhost:5282)
- [x] ngrok çalışıyor (https://644780f539c9.ngrok-free.app)
- [x] Frontend build edildi (dist/ klasörü hazır)
- [x] Netlify CLI kuruldu

---

## 📋 Şimdi Yapılacaklar:

### 1. Netlify'a Login

PowerShell'de:
```powershell
netlify login
```

Bu komut browser'ınızı açacak. Netlify hesabınızla giriş yapın.

### 2. Deploy

```powershell
netlify deploy --prod --dir=dist
```

### 3. Site URL'ini Alın

Deploy tamamlandıktan sonra size bir URL verilecek:
```
https://your-app-name.netlify.app
```

---

## 🎯 Test

1. Netlify URL'ini browser'da açın
2. Kayıt olun / Giriş yapın
3. Mesajlaşmayı test edin

---

## ⚠️ Önemli Notlar:

### ngrok URL Her Değiştiğinde:

1. `.env.production` dosyasını güncelleyin
2. `npm run build` çalıştırın
3. `netlify deploy --prod --dir=dist` ile yeniden deploy edin

### Backend ve ngrok Sürekli Çalışmalı:

- Backend: `dotnet run` (back/TalkyAPI)
- ngrok: `C:\ngrok\ngrok.exe http 5282`

Her ikisi de çalışmıyorsa site çalışmaz!

---

## 🔍 Troubleshooting

### "Visit Site" Uyarısı (ngrok)
İlk ziyarette ngrok bir uyarı sayfası gösterir.
**Çözüm:** "Visit Site" butonuna tıklayın.

### CORS Hatası
**Çözüm:** Backend'i yeniden başlatın.

### SignalR Bağlanamıyor
**Çözüm:** 
1. ngrok URL'inin doğru olduğundan emin olun
2. Frontend'i yeniden build edin
3. Netlify'a yeniden deploy edin

---

## 🎉 Başarılar!

Artık Netlify'daki frontend'iniz, lokal backend'inizle iletişim kuruyor!
