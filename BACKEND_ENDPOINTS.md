# Backend API Endpoints - Tam Liste

## ✅ Auth Endpoints
- POST `/api/auth/register` - Kullanıcı kaydı
- POST `/api/auth/login` - Kullanıcı girişi
- GET `/api/auth/me` - Mevcut kullanıcı bilgisi

## ✅ Users Endpoints
- GET `/api/users` - Tüm kullanıcılar (contacts)
- GET `/api/users/search?q=term` - Kullanıcı arama
- GET `/api/users/username/{username}` - Username ile kullanıcı bulma
- GET `/api/users/{id}` - ID ile kullanıcı bulma
- PUT `/api/users/status` - Online/offline durumu güncelleme
- PUT `/api/users/profile-picture` - Profil resmi yükleme
- PUT `/api/users/profile` - Profil güncelleme (username, email)

## ✅ Messages Endpoints
- GET `/api/messages/{userId}` - Kullanıcı ile mesaj geçmişi
- POST `/api/messages` - Mesaj gönderme
- PUT `/api/messages/{messageId}/read` - Mesajı okundu olarak işaretle

## ✅ Groups Endpoints
- GET `/api/groups` - Kullanıcının grupları
- GET `/api/groups/{id}` - Grup detayı
- POST `/api/groups` - Grup oluşturma
- DELETE `/api/groups/{id}` - Grup silme (owner only)
- POST `/api/groups/{id}/leave` - Gruptan ayrılma
- GET `/api/groups/{id}/messages` - Grup mesajları
- POST `/api/groups/{id}/messages` - Grup mesajı gönderme
- POST `/api/groups/{id}/members` - Üye ekleme
- DELETE `/api/groups/{id}/members/{memberId}` - Üye çıkarma
- POST `/api/groups/{id}/members/{memberId}/promote` - Admin yapma
- POST `/api/groups/{id}/members/{memberId}/demote` - Admin kaldırma
- POST `/api/groups/{id}/members/{memberId}/mute` - Üye susturma
- POST `/api/groups/{id}/members/{memberId}/unmute` - Üye susturmasını kaldırma
- POST `/api/groups/{id}/mute-all` - Tüm grubu susturma
- POST `/api/groups/{id}/unmute-all` - Tüm grubun susturmasını kaldırma
- PUT `/api/groups/{id}/avatar` - Grup profil resmi yükleme

## ✅ Stories Endpoints
- GET `/api/stories` - Tüm aktif story'ler
- GET `/api/stories/{id}` - Story detayı
- POST `/api/stories` - Story oluşturma
- DELETE `/api/stories/{id}` - Story silme
- POST `/api/stories/{id}/view` - Story görüntüleme kaydı
- GET `/api/stories/{id}/views` - Story görüntüleyenler

## ✅ Calls Endpoints
- GET `/api/calls` - Arama geçmişi
- POST `/api/calls` - Arama kaydı oluşturma

## ✅ Contacts Endpoints
- GET `/api/contacts` - Kişiler listesi
- POST `/api/contacts/{contactUserId}` - Kişi ekleme
- DELETE `/api/contacts/{contactUserId}` - Kişi silme
- GET `/api/contacts/check/{contactUserId}` - Kişi kontrolü

## ✅ BlockedUsers Endpoints
- GET `/api/blockedusers` - Engellenen kullanıcılar
- POST `/api/blockedusers/{userId}` - Kullanıcıyı engelleme
- DELETE `/api/blockedusers/{userId}` - Engeli kaldırma
- GET `/api/blockedusers/check/{userId}` - Engel durumu kontrolü

## 🔄 SignalR Hub
- `/chatHub` - Real-time mesajlaşma hub'ı
  - SendMessage(receiverId, content)
  - ReceiveMessage(message)
  - UserOnline(userId)
  - UserOffline(userId)

---

## Frontend Kullanım Durumu

### ✅ Kullanılan Endpoint'ler
- Auth: register, login, getCurrentUser
- Users: getAllUsers, searchUsers, getUserByUsername, getUserById, updateStatus, updateProfilePicture, updateProfile
- Messages: getMessages, sendMessage, markAsRead
- Groups: Tüm endpoint'ler kullanılıyor
- Stories: Tüm endpoint'ler kullanılıyor
- Calls: getCalls, createCall
- Contacts: Tüm endpoint'ler kullanılıyor
- BlockedUsers: Tüm endpoint'ler kullanılıyor
- SignalR: Real-time mesajlaşma aktif

### ✅ Sonuç
**TÜM BACKEND ENDPOINT'LERİ FRONTEND'DE KULLANILIYOR!**

Proje tam entegre durumda, eksik endpoint yok.
