# Active Context

## Şu Anki Odak
Proje tamamlandı! Backend ve frontend tamamen entegre, real-time mesajlaşma çalışıyor. Ses kayıt özelliği eklendi.

## Son Değişiklikler

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
- ✅ Gelen mesajlar anlık görünüyor
- ✅ Gönderilen mesajlar anlık görünüyor
- ✅ Console log'lar eklendi (debugging için)
- ✅ Duplicate listener'lar önleniyor

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
- ⏳ Ses mesajlarını test et
- ⏳ CallsTab backend entegrasyonu (opsiyonel)
- ⏳ Typing indicator (opsiyonel)
- ⏳ Message read receipts (opsiyonel)

### Frontend İyileştirmeleri (Opsiyonel)
- ⏳ Profile edit modal
- ⏳ Settings detail pages
- ⏳ Video/voice call UI
- ⏳ Group chat support
- ⏳ File/image upload
- ⏳ Emoji picker
- ⏳ Dark mode
- ⏳ Push notifications

## Aktif Kararlar
- **Component Structure**: Sidebar yönetir tüm tab'ları
- **Navigation**: Bottom navigation ile tab switching
- **Backend**: LocalDB kullanılıyor - (localdb)\MSSQLLocalDB
- **Database**: Code First yaklaşımı, EF Core migrations
- **State Management**: Zustand (basit ve etkili)
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

### SignalR Real-time
- SignalR bağlantısı login'de kuruluyor
- Event listener'lar ChatPage mount'unda kaydediliyor
- Duplicate listener'lar cleanup ile önleniyor
- ReceiveMessage event'i hem gönderene hem alıcıya gidiyor
- Console log'lar debugging için eklendi
- Connection state kontrolleri yapılıyor
- Ses mesajları SignalR yerine REST API ile (base64 çok büyük)

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
