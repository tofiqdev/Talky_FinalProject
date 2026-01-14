# Profil Yönetimi Özellikleri

## Genel Bakış
Kullanıcılar artık profil resimlerini yükleyebilir, username ve email bilgilerini güncelleyebilir ve tüm uygulama genelinde görüntüleyebilirler.

## Backend Değişiklikleri

### 1. Yeni DTO'lar
- `UpdateProfilePictureDto.cs` - Profil resmi güncellemesi için DTO
  - `ProfilePicture` (string): Base64 encoded resim
- `UpdateProfileDto.cs` - Profil bilgileri güncellemesi için DTO
  - `Username` (string): Yeni kullanıcı adı
  - `Email` (string): Yeni email adresi

### 2. Controller Güncellemeleri
- `UsersController.cs`
  - Yeni endpoint: `PUT /api/users/profile-picture` - Profil resmi güncelleme
  - Yeni endpoint: `PUT /api/users/profile` - Username/Email güncelleme
  - Base64 format validasyonu
  - JWT authentication gerekli

### 3. Service Güncellemeleri
- `IUserService.cs` - Yeni metodlar:
  - `UpdateProfilePicture` - Profil resmi güncelleme
  - `UpdateProfile` - Username/Email güncelleme
- `UserService.cs` - Implementasyonlar:
  - Profil resmi güncelleme
  - Username/Email güncelleme
  - Username uniqueness kontrolü
  - Email uniqueness kontrolü

## Frontend Değişiklikleri

### 1. SettingsTab Component
- Profil resmi upload butonu eklendi
- Kamera ikonu ile upload tetikleme
- Resim önizleme
- Base64 encoding ve backend'e gönderim
- Auth store güncelleme
- Loading state gösterimi
- Dosya boyutu validasyonu (max 5MB)
- Dosya tipi validasyonu (sadece resimler)
- **Username/Email güncelleme formu**
- **Kaydet butonu ile profil güncelleme**
- **Uniqueness hata mesajları**
- **Saving state gösterimi**

### 2. Profil Resmi Gösterimi
Profil resimleri şu yerlerde gösteriliyor:
- **SettingsTab**: Profil bölümünde
- **ChatsTab**: Direkt mesaj listesinde
- **PeopleTab**: Kişiler listesinde ve engellenen kullanıcılarda
- **ChatWindow**: Sohbet header'ında

### 3. Avatar Fallback
Profil resmi yoksa gradient avatar gösteriliyor:
- Direkt mesajlar: Cyan-Blue gradient
- Gruplar: Purple-Pink gradient
- Engellenen kullanıcılar: Gray gradient

## Kullanım

### Profil Resmi Yükleme
1. Settings tab'a git
2. Profil bölümünde kamera ikonuna tıkla
3. Resim seç (max 5MB)
4. Otomatik olarak yüklenir ve tüm yerlerde güncellenir

### Username/Email Güncelleme
1. Settings tab'a git
2. "Edit Profile" butonuna tıkla
3. Username ve/veya Email'i düzenle
4. "Save" butonuna tıkla
5. Başarılı olursa tüm yerlerde güncellenir

### API Endpoints

#### Profil Resmi Güncelleme
```http
PUT /api/users/profile-picture
Authorization: Bearer {token}
Content-Type: application/json

{
  "profilePicture": "data:image/jpeg;base64,..."
}
```

#### Profil Bilgileri Güncelleme
```http
PUT /api/users/profile
Authorization: Bearer {token}
Content-Type: application/json

{
  "username": "newusername",
  "email": "newemail@example.com"
}
```

### Response
```json
{
  "id": 1,
  "username": "newusername",
  "email": "newemail@example.com",
  "avatar": "data:image/jpeg;base64,...",
  "bio": null,
  "isOnline": true,
  "lastSeen": "2024-01-14T10:00:00Z",
  "createdAt": "2024-01-01T00:00:00Z"
}
```

### Hata Durumları
```json
{
  "message": "Username is already taken"
}
```
veya
```json
{
  "message": "Email is already taken"
}
```

## Teknik Detaylar

### Resim Formatı
- Base64 encoded string
- Format: `data:image/{type};base64,{data}`
- Desteklenen tipler: JPEG, PNG, GIF, WebP

### Validasyon
- **Profil Resmi:**
  - Dosya boyutu: Max 5MB
  - Dosya tipi: Sadece resimler (`image/*`)
  - Base64 format kontrolü backend'de
- **Username/Email:**
  - Username: Required, max 50 karakter
  - Email: Required, valid email format, max 100 karakter
  - Username uniqueness kontrolü
  - Email uniqueness kontrolü

### Güvenlik
- JWT authentication gerekli
- Sadece kendi profilini güncelleyebilir
- Base64 format validasyonu
- Email format validasyonu
- Duplicate username/email kontrolü

## Test Senaryoları

1. ✅ Profil resmi yükleme
2. ✅ Profil resmini güncelleme
3. ✅ Profil resmini tüm yerlerde görüntüleme
4. ✅ Dosya boyutu validasyonu
5. ✅ Dosya tipi validasyonu
6. ✅ Avatar fallback (resim yoksa)
7. ✅ Loading state gösterimi
8. ✅ Hata durumları
9. ✅ Username güncelleme
10. ✅ Email güncelleme
11. ✅ Username uniqueness kontrolü
12. ✅ Email uniqueness kontrolü
13. ✅ Boş alan validasyonu
14. ✅ Saving state gösterimi

## Gelecek İyileştirmeler

- [ ] Resim kırpma/düzenleme
- [ ] Profil resmi silme
- [ ] Resim önizleme modal'ı
- [ ] Drag & drop upload
- [ ] Çoklu resim formatı desteği
- [ ] Sunucu tarafında resim optimizasyonu
- [ ] CDN entegrasyonu
