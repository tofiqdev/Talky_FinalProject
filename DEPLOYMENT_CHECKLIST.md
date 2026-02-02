# ✅ Deployment Checklist

## 📋 Deployment Öncesi Kontrol Listesi

### Backend Hazırlık

- [ ] **Database Migration Tamamlandı mı?**
  ```cmd
  cd BackNtier\02.DAL\DAL
  dotnet ef database update --startup-project ../../Talky_API
  ```

- [ ] **Backend Build Başarılı mı?**
  ```cmd
  cd BackNtier\Talky_API
  dotnet build
  ```
  - ✅ 0 error olmalı

- [ ] **Backend Çalışıyor mu?**
  ```cmd
  dotnet run
  ```
  - ✅ `Now listening on: http://localhost:5135` görünmeli
  - ✅ Swagger açılmalı: http://localhost:5135/swagger

- [ ] **API Endpoint'leri Test Edildi mi?**
  - [ ] POST `/api/auth/register` - Kullanıcı kaydı
  - [ ] POST `/api/auth/login` - Giriş
  - [ ] GET `/api/users` - Kullanıcı listesi (JWT ile)
  - [ ] GET `/api/movierooms/active` - Film odaları

### Frontend Hazırlık

- [ ] **Environment Variables Ayarlandı mı?**
  - [ ] `.env.production` dosyası oluşturuldu
  - [ ] `VITE_API_URL` backend URL'i ile güncellendi

- [ ] **Frontend Build Başarılı mı?**
  ```cmd
  npm run build
  ```
  - ✅ `dist` klasörü oluşmalı
  - ✅ 0 error olmalı

- [ ] **Build Preview Test Edildi mi?**
  ```cmd
  npm run preview
  ```
  - ✅ http://localhost:4173 açılmalı

### Deployment

#### ngrok Kullanıyorsan:

- [ ] **ngrok Kuruldu mu?**
  - [ ] ngrok.exe indirildi
  - [ ] Auth token ayarlandı
  - [ ] `ngrok http 5135` çalışıyor

- [ ] **Backend Çalışıyor mu?**
  - [ ] Terminal 1: `dotnet run` çalışıyor
  - [ ] Terminal 2: `ngrok http 5135` çalışıyor

- [ ] **URL Kopyalandı mı?**
  - [ ] ngrok URL'i kopyalandı (örn: https://abc123.ngrok-free.app)
  - [ ] `.env.production` güncellendi
  - [ ] Frontend yeniden build edildi

#### Railway Kullanıyorsan:

- [ ] **Railway Projesi Oluşturuldu mu?**
  - [ ] GitHub repo bağlandı
  - [ ] Build ayarları yapıldı
  - [ ] Environment variables eklendi

- [ ] **Deploy Başarılı mı?**
  - [ ] Railway dashboard'da "Active" görünüyor
  - [ ] Logs'da hata yok
  - [ ] URL açılıyor

### Test

- [ ] **Backend Erişilebilir mi?**
  - [ ] Swagger açılıyor: `BACKEND_URL/swagger`
  - [ ] Health check çalışıyor

- [ ] **Frontend Backend'e Bağlanıyor mu?**
  - [ ] Login çalışıyor
  - [ ] Mesaj gönderme çalışıyor
  - [ ] Film odası oluşturma çalışıyor

- [ ] **SignalR Çalışıyor mu?**
  - [ ] Real-time mesajlaşma çalışıyor
  - [ ] Film senkronizasyonu çalışıyor
  - [ ] Console'da "SignalR Connected" görünüyor

- [ ] **CORS Çalışıyor mu?**
  - [ ] Console'da CORS hatası yok
  - [ ] API istekleri başarılı

### Production Ayarları

- [ ] **Güvenlik**
  - [ ] JWT secret key güçlü
  - [ ] Database şifresi güçlü
  - [ ] HTTPS kullanılıyor

- [ ] **Performance**
  - [ ] Database connection pool ayarlandı
  - [ ] SignalR reconnect ayarlandı
  - [ ] API rate limiting var mı? (opsiyonel)

- [ ] **Monitoring**
  - [ ] Backend log'ları izleniyor
  - [ ] Frontend error tracking var mı? (opsiyonel)
  - [ ] Uptime monitoring var mı? (opsiyonel)

### Son Kontroller

- [ ] **Tüm Özellikler Çalışıyor mu?**
  - [ ] ✅ Kullanıcı kaydı ve girişi
  - [ ] ✅ Mesajlaşma (direkt ve grup)
  - [ ] ✅ Film odası oluşturma
  - [ ] ✅ Film senkronizasyonu
  - [ ] ✅ Story oluşturma ve görüntüleme
  - [ ] ✅ Kullanıcı engelleme
  - [ ] ✅ Profil resmi yükleme

- [ ] **Mobil Uyumlu mu?**
  - [ ] Telefonda test edildi
  - [ ] Responsive tasarım çalışıyor

- [ ] **Farklı Tarayıcılarda Test Edildi mi?**
  - [ ] Chrome
  - [ ] Firefox
  - [ ] Safari (Mac varsa)
  - [ ] Edge

---

## 🎉 Deployment Tamamlandı!

### Paylaş:

- **Frontend URL:** ___________________________
- **Backend URL:** ___________________________
- **Swagger URL:** ___________________________

### Notlar:

- ngrok kullanıyorsan: Her restart'ta URL değişir
- Railway kullanıyorsan: URL kalıcıdır
- Database: LocalDB kullanıyorsan, bilgisayar açık olmalı

---

## 🆘 Sorun Çözümleri

### Backend'e erişilemiyor
1. Backend çalışıyor mu? (`dotnet run`)
2. ngrok çalışıyor mu? (`ngrok http 5135`)
3. Firewall engelliyor mu?

### CORS hatası
1. `.env.production` doğru mu?
2. Frontend yeniden build edildi mi?
3. `Program.cs`'de CORS ayarları var mı?

### SignalR bağlanmıyor
1. WebSocket destekleniyor mu?
2. JWT token doğru mu?
3. Console'da hata var mı?

### Database hatası
1. Migration uygulandı mı?
2. Connection string doğru mu?
3. LocalDB çalışıyor mu?

---

## 📞 Yardım

Sorun devam ediyorsa:
1. Console log'larını kontrol et (F12)
2. Backend log'larını kontrol et
3. ngrok dashboard'u kontrol et: http://localhost:4040

İyi şanslar! 🚀
