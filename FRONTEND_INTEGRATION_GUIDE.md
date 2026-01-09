# Frontend-Backend Entegrasyon Rehberi

## ✅ Tamamlanan İşlemler

### 1. API Service Oluşturuldu
- `src/services/apiService.ts`
- Auth API (register, login, getCurrentUser)
- Users API (getAllUsers, getUserById, updateStatus)
- Messages API (getMessages, sendMessage, markAsRead)
- Calls API (getCalls, createCall)

### 2. SignalR Service Güncellendi
- Backend URL: `http://localhost:5282/chatHub`
- JWT authentication ile bağlantı
- Automatic reconnection
- Event listeners (ReceiveMessage, UserOnline, UserOffline, TypingIndicator, MessageRead)
- Actions (sendMessage, sendTypingIndicator, markAsRead)

### 3. Auth Store Güncellendi
- Backend API entegrasyonu
- Login/Register fonksiyonları
- SignalR otomatik bağlantı
- Error handling
- Loading states

### 4. Login & Register Pages
- Backend API çağrıları
- Error gösterimi
- Loading states
- Form validation

### 5. Type Definitions
- User type (id: number, backend uyumlu)
- Message type (backend uyumlu)
- AuthResponse type

## 🧪 Test Adımları

### 1. Backend Çalışıyor mu Kontrol Et
```bash
cd back/TalkyAPI
dotnet run
```
API: http://localhost:5282
Swagger: http://localhost:5282/swagger

### 2. Frontend Çalıştır
```bash
npm run dev
```
Frontend: http://localhost:5174

### 3. Test Senaryosu

#### A. Kullanıcı Kaydı
1. http://localhost:5174/register adresine git
2. Kullanıcı bilgilerini gir:
   - Username: testuser
   - Email: test@example.com
   - Password: 123456
3. "Kayıt Ol" butonuna tıkla
4. Başarılı olursa otomatik olarak /chat'e yönlendirileceksin

#### B. Kullanıcı Girişi
1. http://localhost:5174 adresine git
2. Giriş bilgilerini gir:
   - Email: test@example.com
   - Password: 123456
3. "Giriş Yap" butonuna tıkla
4. Başarılı olursa /chat'e yönlendirileceksin

#### C. SignalR Bağlantısı
1. Browser console'u aç (F12)
2. "SignalR Connected" mesajını gör
3. Network tab'ında WebSocket bağlantısını kontrol et

### 4. Swagger ile Test

#### Kullanıcı Oluştur
```
POST /api/auth/register
{
  "username": "user1",
  "email": "user1@example.com",
  "password": "123456"
}
```

#### Giriş Yap
```
POST /api/auth/login
{
  "email": "user1@example.com",
  "password": "123456"
}
```

Token'ı kopyala ve "Authorize" butonuna yapıştır.

#### Kullanıcıları Listele
```
GET /api/users
```

#### Mesaj Gönder
```
POST /api/messages
{
  "receiverId": 2,
  "content": "Hello!"
}
```

## 🔧 Kalan İşler

### ChatStore Güncelleme
- Mock data'yı kaldır
- Backend API'yi kullan
- SignalR event'lerini dinle

### ChatWindow Component
- Mesaj gönderme backend'e bağla
- Mesaj geçmişini backend'den çek
- Real-time mesaj alma

### Sidebar Components
- Kullanıcı listesini backend'den çek
- Online/offline durumları güncelle
- Arama geçmişini backend'den çek

## 🐛 Troubleshooting

### CORS Hatası
Backend'de CORS yapılandırması var:
```csharp
policy.WithOrigins("http://localhost:5174", "http://localhost:3000")
```

### SignalR Bağlantı Hatası
- Token'ın geçerli olduğundan emin ol
- Backend'in çalıştığından emin ol
- Console'da hata mesajlarını kontrol et

### 401 Unauthorized
- Token'ın localStorage'da olduğundan emin ol
- Token'ın expire olmadığından emin ol
- Login/Register işleminin başarılı olduğundan emin ol

## 📝 Notlar

- Backend API: http://localhost:5282
- Frontend: http://localhost:5174
- SignalR Hub: ws://localhost:5282/chatHub
- Token 7 gün geçerli
- Password minimum 6 karakter
- Username minimum 3 karakter
