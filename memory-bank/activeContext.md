# Active Context

## Şu Anki Odak
Proje tamamlandı! Backend ve frontend tamamen entegre, real-time mesajlaşma çalışıyor. Test aşamasında.

## Son Değişiklikler

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
- ⏳ İki kullanıcı ile test et
- ⏳ Real-time mesajlaşma test et
- ⏳ Search functionality test et
- ⏳ CallsTab backend entegrasyonu (opsiyonel)
- ⏳ Typing indicator (opsiyonel)
- ⏳ Message read receipts (opsiyonel)

### Frontend İyileştirmeleri (Opsiyonel)
- ⏳ Search functionality (contacts, messages)
- ⏳ Profile edit modal
- ⏳ Settings detail pages
- ⏳ Video/voice call UI
- ⏳ Group chat support
- ⏳ File/image upload
- ⏳ Emoji picker
- ⏳ Dark mode
- ⏳ Notifications

## Aktif Kararlar
- **Component Structure**: Sidebar yönetir tüm tab'ları
- **Navigation**: Bottom navigation ile tab switching
- **Backend**: LocalDB kullanılıyor - (localdb)\MSSQLLocalDB
- **Database**: Code First yaklaşımı, EF Core migrations
- **State Management**: Zustand (basit ve etkili)
- **Styling**: Tailwind CSS 3, referans tasarıma %100 uyumlu
- **Authentication**: JWT Bearer token
- **Password**: BCrypt hashing

## Önemli Desenler ve Tercihler
- Tab-based navigation pattern
- Reusable tab components
- Consistent UI across all tabs
- Mock data her tab için hazır
- Active tab highlighting
- Icon + label navigation
- Modular component structure

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
- Mock data her tab için ayrı hazırlandı
- Settings'de logout functionality çalışıyor
- Calls tab'da incoming/outgoing/missed call indicators
- People tab'da online/offline status gösterimi
- Tüm tab'lar aynı tasarım dilini kullanıyor

### Backend
- .NET 10 SDK kurulu ama proje .NET 8 kullanıyor
- EF Core tools version uyumsuzluğu → 8.0.0 versiyonu kullanıldı
- LocalDB instance adı: (localdb)\MSSQLLocalDB
- Migration başarıyla uygulandı, TalkyDB oluşturuldu
- Foreign key'ler DeleteBehavior.Restrict ile yapılandırıldı
- Unique index'ler Username ve Email için eklendi
- CORS frontend için yapılandırıldı (SetIsOriginAllowed)
- JWT token 7 gün geçerli
- Password BCrypt ile hash'leniyor (work factor: default)
- Swagger'da "Authorize" butonu ile JWT test edilebilir
- ClaimTypes.NameIdentifier user ID için kullanılıyor
- API port: 5282 (HTTP), HTTPS redirect kaldırıldı
- SignalR JWT authentication query string ile (/chatHub?access_token=...)
- Contact tablosu kaldırıldı - Username ile direkt arama
- Search API: GET /api/users/search?q=term (min 2 karakter)

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
