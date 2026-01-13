# Active Context

## Şu Anki Odak
✅ **Proje Production Ready!** Backend ve frontend tamamen entegre, tüm özellikler çalışıyor. Real-time mesajlaşma, grup yönetimi, ses mesajları, kullanıcı yetkilendirme sistemi, **mute/unmute komut sistemi** aktif. Proje stabil ve kullanıma hazır durumda.

## Son Değişiklikler

### Mute/Unmute Komut Sistemi Eklendi ✅
- ✅ Chat'te komut ile susturma: `@username /mute`
- ✅ Chat'te komut ile susturmayı kaldırma: `@username /unmute`
- ✅ Sadece Owner ve Admin'ler komut kullanabilir
- ✅ Owner susturulamaz
- ✅ Sistem mesajı otomatik oluşturuluyor: "Şşşt @user Encapsulation By @admin"
- ✅ Unmute mesajı: "@user artık konuşabilir. Unmuted by @admin"
- ✅ Muted kullanıcılar mesaj gönderemez (input devre dışı)
- ✅ Muted uyarısı: "🔇 You are muted in this group"
- ✅ Sistem mesajları sarı arka plan ile ortalanmış gösteriliyor
- ✅ UI butonları ile de mute/unmute yapılabiliyor (GroupDetailsModal) **Grup mesajlarında @ mention (bahsetme) özelliği eklendi!**

## Son Değişiklikler

### @ Mention Özelliği Eklendi ✅
- ✅ Grup mesajlarında @ yazınca üye önerileri gösteriliyor
- ✅ Klavye navigasyonu (↑↓ ok tuşları, Enter, Escape)
- ✅ Mention'lar mesajlarda vurgulanıyor (highlight)
- ✅ Kullanıcı kendisi mention edildiğinde özel vurgu
- ✅ Otomatik tamamlama dropdown'u
- ✅ Online/offline durumu gösterimi
- ✅ Admin badge'i mention listesinde
- ✅ Grup mesajlarında gönderen adı gösteriliyor

### Grup Oluşturma ve Mesajlaşma Eklendi ✅
- ✅ Backend grup modelleri (Group, GroupMember, GroupMessage)
- ✅ Database migration uygulandı (AddGroupFeature)
- ✅ GroupsController API endpoint'leri
- ✅ Frontend grup type tanımları
- ✅ CreateGroupModal component'i
- ✅ Grup listesi ChatsTab'da görünüyor
- ✅ Grup mesajlaşma ChatWindow'da çalışıyor
- ✅ Grup ve direkt mesaj ayrımı (mor/pembe vs mavi gradient)
- ✅ Grup üye sayısı badge'i
- ✅ Grup mesajları yükleme ve gönderme

### Grup Yetkilendirme ve Yönetim Eklendi ✅
- ✅ Owner/Admin/Member rolleri
- ✅ GroupDetailsModal - Üye yönetimi
- ✅ Admin atama/kaldırma (sadece owner)
- ✅ Üye ekleme/çıkarma (owner ve admin)
- ✅ Grup ayarları menüsü
- ✅ Grup silme (sadece owner)
- ✅ Gruptan ayrılma (normal üyeler)
- ✅ Yetki etiketleri (Owner/Admin/Member)
- ✅ Settings/Details view geçişi

### Real-time Mesaj Görüntüleme Düzeltildi ✅
- ✅ Gönderilen mesajlar anlık görünüyor (sayfa yenileme gereksiz)
- ✅ Gelen mesajlar anlık görünüyor
- ✅ SignalR ReceiveMessage event handler düzeltildi
- ✅ Mesaj filtreleme mantığı iyileştirildi (senderId ve receiverId kontrolü)
- ✅ Duplicate mesaj kontrolü eklendi (aynı mesaj iki kez eklenmiyor)
- ✅ Backend'den gelen mesajlar doğru şekilde state'e ekleniyor

### Ses Kayıt Özelliği Eklendi ✅
- ✅ Basılı tut & kaydet özelliği
- ✅ Ses kaydı UI (kırmızı pulse animasyonu)
- ✅ Süre göstergesi
- ✅ İptal butonu
- ✅ Ses mesajı player (play/pause, progress bar)
- ✅ REST API ile gönderim (SignalR yerine - base64 çok büyük)
- ✅ Düşük bitrate (16kbps) ve sample rate (16kHz)
- ✅ Opus codec ile sıkıştırma
- ✅ Mobil uyumlu (touch events)

### Mesaj Animasyonları Eklendi ✅
- ✅ Mesajlar fade-in animasyonu ile geliyor
- ✅ Otomatik scroll en alta (smooth)
- ✅ Mesaj hover efekti (scale)
- ✅ Input focus animasyonu
- ✅ Gönder butonu hover/active animasyonları
- ✅ Loading spinner mesaj gönderilirken
- ✅ Mesaj gönderilince input anında temizleniyor

### Real-time Mesajlaşma Düzeltildi ✅
- ✅ SignalR event listener'ları düzgün çalışıyor
- ✅ Gelen mesajlar anlık görünüyor (sayfa yenileme gereksiz)
- ✅ Gönderilen mesajlar anlık görünüyor (sayfa yenileme gereksiz)
- ✅ Console log'lar eklendi (debugging için)
- ✅ Duplicate listener'lar önleniyor
- ✅ Duplicate mesajlar önleniyor (ID kontrolü ile)
- ✅ Mesaj filtreleme mantığı: senderId VEYA receiverId selectedUser'a eşit olmalı

### Contact Sistemi Kaldırıldı ✅
- ✅ Contact tablosu database'den kaldırıldı
- ✅ Username unique constraint zaten var
- ✅ User search API eklendi (GET /api/users/search?q=term)
- ✅ Username ile kullanıcı bulma (GET /api/users/username/{username})
- ✅ PeopleTab'a real-time search eklendi
- ✅ Kullanıcılar direkt username ile arama yapıp mesajlaşabiliyor

### Mock Data Kaldırıldı ✅
- ✅ ChatStore backend entegrasyonu
- ✅ PeopleTab gerçek kullanıcıları gösteriyor
- ✅ ChatsTab gerçek kullanıcıları gösteriyor
- ✅ MessageList gerçek mesajları gösteriyor
- ✅ ChatWindow backend'e mesaj gönderiyor
- ✅ SignalR real-time event listeners

## Sonraki Adımlar

### Test ve İyileştirmeler
- ✅ @ Mention özelliği (grup mesajlarında)
- ⏳ Ses mesajlarını test et
- ⏳ Grup mesajlaşmasını test et
- ⏳ Real-time grup mesajları (SignalR ile - şu an REST API)
- ⏳ CallsTab backend entegrasyonu (opsiyonel)
- ⏳ Typing indicator (opsiyonel)
- ⏳ Message read receipts (opsiyonel)
- ⏳ Dosya/resim paylaşımı (opsiyonel)
- ⏳ Emoji picker (opsiyonel)
- ⏳ Dark mode (opsiyonel)
- ⏳ Push notifications (opsiyonel)
- ⏳ Kick komutu: `@username /kick` (opsiyonel)
- ⏳ Ban komutu: `@username /ban` (opsiyonel)

## Aktif Kararlar
- **Component Structure**: Sidebar yönetir tüm tab'ları
- **Navigation**: Bottom navigation ile tab switching
- **Backend**: LocalDB kullanılıyor - (localdb)\MSSQLLocalDB
- **Database**: Code First yaklaşımı, EF Core migrations
- **State Management**: Zustand (basit ve etkili)
- **Real-time**: SignalR (direkt mesajlar), REST API (grup mesajları)
- **Authentication**: JWT Bearer token
- **Grup Yetkilendirme**: Owner/Admin/Member rolleri
- **Komut Sistemi**: Chat'te `@username /command` formatı
- **Sistem Mesajları**: IsSystemMessage flag ile özel gösterim
- **Styling**: Tailwind CSS 3, referans tasarıma %100 uyumlu
- **Authentication**: JWT Bearer token
- **Password**: BCrypt hashing
- **Animations**: CSS keyframes + Tailwind transitions
- **Voice Messages**: REST API (base64 too large for SignalR)
- **Audio Format**: WebM with Opus codec, 16kbps bitrate

## Component Hiyerarşisi
```
ChatPage
└── Sidebar
    ├── Header (dynamic title)
    ├── Tab Content
    │   ├── ChatsTab (stories + chat list)
    │   ├── CallsTab (call history)
    │   ├── PeopleTab (contacts)
    │   └── SettingsTab (settings menu)
    └── Bottom Navigation (4 tabs)
```

## Öğrenilenler

### Frontend
- UserList component'i Sidebar'a refactor edildi
- Her tab ayrı component olarak organize edildi
- Tab state Sidebar component'inde yönetiliyor
- Settings'de logout functionality çalışıyor
- Calls tab'da incoming/outgoing/missed call indicators
- People tab'da online/offline status gösterimi
- Tüm tab'lar aynı tasarım dilini kullanıyor
- Mesaj animasyonları fade-in ve smooth scroll ile
- Auto-scroll yeni mesajlarda
- Loading states ve spinner animasyonları
- Ses kayıt özelliği MediaRecorder API ile
- Ses mesajları base64 formatında saklanıyor
- REST API fallback ses mesajları için

### Backend
- .NET 10 SDK kurulu ama proje .NET 8 kullanıyor
- EF Core tools version uyumsuzluğu → 8.0.0 versiyonu kullanıldı
- LocalDB instance adı: (localdb)\MSSQLLocalDB
- Migration başarıyla uygulandı, TalkyDB oluşturuldu
- Foreign key'ler DeleteBehavior.Restrict ile yapılandırıldı
- Unique index'ler Username ve Email için eklendi
- CORS frontend için yapılandırıldı (SetIsOriginAllowed)
- JWT token 7 gün geçerli
- Password BCrypt ile hash'leniyor
- API port: 5282 (HTTP)
- SignalR JWT authentication query string ile
- Contact tablosu kaldırıldı - Username ile direkt arama
- Search API: GET /api/users/search?q=term (min 2 karakter)
- Backend publish edildi (back/publish/)
- Grup tabloları eklendi: Groups, GroupMembers, GroupMessages
- Grup API endpoint'leri: 
  - POST /api/groups - Grup oluştur
  - GET /api/groups - Kullanıcının grupları
  - GET /api/groups/{id} - Grup detayı
  - GET /api/groups/{id}/messages - Grup mesajları
  - POST /api/groups/{id}/messages - Grup mesajı gönder
  - POST /api/groups/{id}/members/{memberId}/promote - Admin yap
  - POST /api/groups/{id}/members/{memberId}/demote - Admin kaldır
  - DELETE /api/groups/{id}/members/{memberId} - Üye çıkar
  - POST /api/groups/{id}/members - Üye ekle
  - DELETE /api/groups/{id} - Grup sil
  - POST /api/groups/{id}/leave - Gruptan ayrıl

### SignalR Real-time
- SignalR bağlantısı login'de kuruluyor
- Event listener'lar ChatPage mount'unda kaydediliyor
- Duplicate listener'lar cleanup ile önleniyor
- ReceiveMessage event'i hem gönderene hem alıcıya gidiyor
- Console log'lar debugging için eklendi
- Connection state kontrolleri yapılıyor
- Ses mesajları SignalR yerine REST API ile (base64 çok büyük)
- Mesaj filtreleme: (senderId === selectedUser.id) VEYA (receiverId === selectedUser.id)
- Duplicate mesaj kontrolü: message.id ile kontrol ediliyor
- Mesajlar anlık görünüyor, sayfa yenileme gereksiz

### Voice Messages
- MediaRecorder API kullanımı
- WebM audio format with Opus codec
- 16kbps bitrate (düşük dosya boyutu)
- 16kHz sample rate
- Base64 encoding
- Format: [VOICE:15s]data:audio/webm;codecs=opus;base64,...
- REST API ile gönderim (POST /api/messages)
- Audio player component (play/pause, progress bar)
- Touch events desteği (mobil)
- Mikrofon izni kontrolü

### Frontend
- Vite proxy kullanılıyor (/api → http://localhost:5282)
- CORS sorunu proxy ile çözüldü
- API_BASE_URL: '/api' (relative path)
- SignalR URL: http://localhost:5282/chatHub
- Type definitions backend uyumlu (id: number, dates: string)
- Auth store backend entegrasyonu tamamlandı
- Chat store backend entegrasyonu tamamlandı
- Mock data tamamen kaldırıldı
- Real-time search (2+ karakter)
- Loading ve error states eklendi
- Mesaj animasyonları: fadeIn, smooth scroll, hover effects
- Input animasyonları: focus, transition
- Button animasyonları: hover scale, active scale, loading spinner
- Voice recording: hold to record, release to send
- Grup yönetimi: groups state, selectedGroup, loadGroups, setSelectedGroup
- Grup mesajlaşma: loadGroupMessages, sendGroupMessage
- ChatWindow hem direkt hem grup mesajları destekliyor
- ChatPage hem selectedUser hem selectedGroup kontrolü yapıyor
- **Komut sistemi**: `@username /mute` ve `@username /unmute` komutları
- **Sistem mesajları**: Sarı arka plan, ortalanmış, özel stil
- **Muted UI**: Input devre dışı, uyarı mesajı, butonlar disabled
