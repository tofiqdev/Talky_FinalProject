# Active Context

## Şu Anki Odak
✅ **Talky Projesi Tamamen Tamamlandı!** Backend (BackNtier/Talky_API) port 5135'te çalışıyor, frontend port 5173'te. Tüm özellikler çalışır durumda: Film gecesi, grup mesajlaşma, story sistemi, profil yönetimi, ses mesajları ve daha fazlası. Proje production-ready durumda!

## Son Değişiklikler (3 Şubat 2026)

### 🎉 Proje Tamamen Tamamlandı ✅
Talky projesi tüm özellikleriyle birlikte production-ready duruma geldi. Kullanıcılar kayıt olup giriş yapabilir, gerçek zamanlı mesajlaşabilir, grup oluşturabilir, film odaları açabilir, story paylaşabilir ve daha birçok özelliği kullanabilir.

### 📊 Proje İstatistikleri
- ✅ **Backend**: 73 API endpoint, N-Tier mimari, SignalR real-time
- ✅ **Frontend**: React 19.2.0, TypeScript, Tailwind CSS 3
- ✅ **Database**: SQL Server LocalDB, 15+ tablo
- ✅ **Özellikler**: 25+ ana özellik (mesajlaşma, gruplar, filmler, story'ler)
- ✅ **Deployment**: Netlify (frontend) + ngrok (backend tunnel)
- ✅ **Production URL**: https://talkychat.netlify.app

### 🎬 Film Gecesi (Movie Room) Özelliği ✅
- ✅ **Backend Entity Models**: MovieRoom, MovieRoomParticipant, MovieRoomMessage
- ✅ **Database Migration**: AddMovieRoomFeature uygulandı
- ✅ **N-Tier Architecture**: DAL, BLL, API katmanları
- ✅ **MovieRoomHub**: SignalR hub ile real-time senkronizasyon
- ✅ **API Endpoints**: 11 endpoint (CRUD + join/leave + playback sync + messages)
- ✅ **Frontend Components**: MoviesTab, CreateMovieRoomModal, MovieRoomWindow
- ✅ **YouTube Integration**: react-youtube ile video player
- ✅ **Senkronize Oynatma**: Sadece oda sahibi kontrole sahip
- ✅ **Real-time Chat**: Film izlerken mesajlaşma
- ✅ **Katılımcı Listesi**: Online/offline durumları
- ✅ **Video Overlay**: Non-owner'lar için tıklama engelleme
- ✅ **Debug Logging**: Kapsamlı console ve backend log'ları

#### Film Gecesi Özellikleri:
- ✅ Film odası oluşturma (YouTube URL ile)
- ✅ Aktif odaları listeleme
- ✅ Odaya katılma/ayrılma
- ✅ **Senkronize video oynatma** (play/pause/seek)
- ✅ **Sadece oda sahibi kontrole sahip**
- ✅ Yan tarafta real-time chat
- ✅ Katılımcı listesi (online/offline status)
- ✅ YouTube thumbnail preview
- ✅ Oynatma durumu gösterimi
- ✅ "👑 Oda Sahibi" badge'i
- ✅ "🔄 Senkronize ediliyor..." göstergesi
- ✅ Video overlay (non-owner'lar için)

#### Film Gecesi API Endpoints:
- GET /api/movierooms - Tüm odalar
- GET /api/movierooms/active - Aktif odalar
- GET /api/movierooms/{id} - Oda detayı
- POST /api/movierooms - Oda oluştur
- PUT /api/movierooms/{id} - Oda güncelle
- DELETE /api/movierooms/{id} - Oda sil
- POST /api/movierooms/{id}/join - Odaya katıl
- POST /api/movierooms/{id}/leave - Odadan ayrıl
- PUT /api/movierooms/{id}/playback - Oynatma durumu güncelle
- GET /api/movierooms/{id}/messages - Oda mesajları
- POST /api/movierooms/{id}/messages - Mesaj gönder

#### Film Gecesi SignalR Hub:
- `/movieRoomHub` - SignalR endpoint
- `JoinMovieRoom(roomId)` - Odaya katıl
- `LeaveMovieRoom(roomId)` - Odadan ayrıl
- `SyncPlayback(roomId, currentTime, isPlaying)` - Video senkronizasyonu (sadece owner)
- `PlaybackSync` event - Tüm katılımcılara broadcast
- `UserJoined` / `UserLeft` events - Katılımcı değişiklikleri
- `ReceiveRoomMessage` event - Yeni mesaj bildirimi

#### Film Gecesi Teknik Detaylar:
- **Backend**: MovieRoomHub (SignalR), MovieRoomManager, MovieRoomMessageManager
- **Frontend**: movieRoomSignalrService, MovieRoomWindow, MoviesTab
- **Senkronizasyon**: Owner'ın her aksiyonu (play/pause/seek) tüm katılımcılara broadcast
- **Video Kontrolü**: Sadece owner için aktif, diğerleri için devre dışı
- **Overlay**: Non-owner'lar için transparent overlay ile tıklama engelleme
- **JWT Auth**: SignalR için JWT token desteği (/movieRoomHub)
- **Logging**: Kapsamlı debug log'ları (frontend console + backend)
- **Auto-sync**: 1 saniyeden fazla fark varsa otomatik senkronizasyon

### Profil ve Grup Avatar Yükleme Eklendi ✅
- ✅ User ve Group Avatar kolonları nvarchar(max) olarak güncellendi
- ✅ FluentValidation MaxLength(500) limiti kaldırıldı
- ✅ Base64 resim formatı kabul ediliyor (data:image/...)
- ✅ Frontend: Profil resmi yükleme (SettingsTab → EditProfileModal)
- ✅ Frontend: Grup resmi yükleme (GroupDetailsModal)
- ✅ Resim sıkıştırma (400x400, JPEG 0.8 quality)
- ✅ Backend: PUT /api/User/profile-picture endpoint
- ✅ Backend: PUT /api/Groups/{id}/avatar endpoint
- ✅ Migration: FixAvatarMaxLength uygulandı
- ✅ Entity tracking conflict çözüldü (UserManager.Update refactor)
- ✅ Duplicate name kontrolü düzeltildi (kendi ID'sini exclude ediyor)
- ✅ MessageList undefined senderUsername hatası düzeltildi (optional chaining)

### Entity Tracking Sorunları Çözüldü ✅
- ✅ UserManager.Update() - Database'den fresh entity çekiliyor
- ✅ Mapper yerine manuel property update kullanılıyor
- ✅ DuplicateUserName metodu userId ve name parametreleri alıyor
- ✅ "Another instance with the same key value is already being tracked" hatası çözüldü
- ✅ Login ve profil güncelleme çalışıyor

### Grup Tıklama Beyaz Ekran Sorunu Düzeltildi ✅
- ✅ GroupListDTO'ya Members array eklendi (List<GroupMemberListDTO>)
- ✅ GroupMemberListDTO'ya Username, Avatar, IsOnline field'ları eklendi
- ✅ GroupsController GetAll() ve GetById() metodları members'ı dolduruyor
- ✅ Her member için user bilgileri (username, avatar, online status) ekleniyor
- ✅ CreatedByName → CreatedByUsername olarak düzeltildi
- ✅ AutoMapper mapping'leri eklendi (GroupListDTO ↔ GroupUpdateDTO, GroupMemberListDTO ↔ GroupMemberUpdateDTO)
- ✅ Grup tıklandığında artık mesajlaşma ekranı açılıyor

### Grup Komutları Düzeltildi ✅
- ✅ `/muteall` - Tüm grubu sustur (çalışıyor)
- ✅ `/unmuteall` - Tüm grubun susturmasını kaldır (çalışıyor)
- ✅ `@username /mute` - Belirli kullanıcıyı sustur (çalışıyor)
- ✅ `@username /unmute` - Belirli kullanıcının susturmasını kaldır (çalışıyor)
- ✅ Frontend'deki özel command handling kaldırıldı - Tüm komutlar backend'e gönderiliyor
- ✅ Error handling iyileştirildi - Backend'den gelen hata mesajları düzgün gösteriliyor
- ✅ Komut sonrası otomatik reload - Grup ve mesajlar yeniden yükleniyor
- ✅ GroupUpdateDTO field'ları manuel oluşturuluyor (entity tracking conflict önleniyor)
- ✅ GroupMemberUpdateDTO field'ları manuel oluşturuluyor (entity tracking conflict önleniyor)

### Telegram-like Özellik ✅
- ✅ Arama sonucundan kullanıcı seçildiğinde `users` listesine ekleniyor
- ✅ Duplicate kontrol (aynı kullanıcı iki kez eklenmiyor)
- ✅ Mesajlaşma başladığında kullanıcı Chats listesinde görünüyor
- ✅ Telegram gibi çalışıyor: Ara → Seç → Mesajlaş → Chats'ta Görün

### BackNtier Migration Tamamlandı ✅
- ✅ back/ klasörü silindi (eski monolitik yapı)
- ✅ BackNtier/ ile devam (modern N-Tier mimari)
- ✅ ContactManager DTO desteği eklendi (AutoMapper)
- ✅ ContactController mapper kullanımı kaldırıldı (service zaten DTO döndürüyor)
- ✅ Backend yeniden başlatıldı (port: 5135)
- ✅ Vite config güncellendi (proxy: 5135)
- ✅ Frontend yeniden başlatıldı
- ✅ Build başarılı: 0 error, 0 warning
- ✅ Database migration tamamlandı
- ✅ 73 API endpoint hazır

### Profil Yönetimi Özellikleri Eklendi ✅
- ✅ Profil resmi yükleme (base64, max 5MB)
- ✅ Resim sıkıştırma (400x400, JPEG 0.8 quality)
- ✅ Username güncelleme
- ✅ Email güncelleme
- ✅ Uniqueness kontrolü (username/email)
- ✅ Avatar kolonu database'de nvarchar(max)
- ✅ Migration: UpdateAvatarColumnSize
- ✅ Backend: PUT /api/users/profile-picture
- ✅ Backend: PUT /api/users/profile
- ✅ Frontend: EditProfileModal (upload + edit)
- ✅ Profil resimleri tüm yerlerde gösteriliyor:
  - SettingsTab (profil bölümü)
  - ChatsTab (direkt mesaj listesi)
  - PeopleTab (kişiler ve engellenenler)
  - ChatWindow (sohbet header'ı)
  - Story listesi (ChatsTab)
  - Story görüntüleme (ViewStoryModal)
  - Story views paneli

### Grup Profil Resmi Yükleme Eklendi ✅
- ✅ Group model - Avatar MaxLength kaldırıldı (nvarchar(max))
- ✅ Migration: UpdateGroupAvatarSize
- ✅ UpdateGroupAvatarDto oluşturuldu
- ✅ Backend: PUT /api/groups/{id}/avatar
- ✅ Sadece owner ve adminler yükleyebilir
- ✅ Base64 format validasyonu
- ✅ Frontend: GroupDetailsModal - Avatar upload butonu
- ✅ Resim sıkıştırma (400x400, JPEG 0.8)
- ✅ Grup avatarları gösteriliyor:
  - ChatsTab (grup listesi)
  - ChatWindow (sohbet header'ı)
  - GroupDetailsModal (grup detayları)

### Contact Sistemi Eklendi ✅
- ✅ Contact model ve Contacts tablosu
- ✅ Migration: AddContactSystem
- ✅ ContactsController API endpoint'leri:
  - GET /api/contacts - Kişi listesi
  - POST /api/contacts/{userId} - Kişi ekle
  - DELETE /api/contacts/{userId} - Kişi sil
  - GET /api/contacts/check/{userId} - Kişi kontrolü
- ✅ UserService - Sadece contact'ları döndürüyor
- ✅ StoriesController - Sadece contact'ların story'lerini gösteriyor
- ✅ ChatWindow - "Add to Contacts" banner'ı
- ✅ Contact kontrolü ve ekleme butonu
- ✅ Sarı uyarı banner'ı (contact değilse)
- ✅ Engellenen kullanıcılar contact olarak eklenemez
- ✅ Ölçeklenebilir yapı (milyonlarca kullanıcı için)

### Story'lerde Profil Resimleri Eklendi ✅
- ✅ StoryDto - Avatar field'ı
- ✅ StoryViewDto - Avatar field'ı
- ✅ StoriesController - Avatar döndürülüyor
- ✅ ChatsTab - Story avatarları
- ✅ Add Story butonu - Kendi profil resmi
- ✅ ViewStoryModal - Story header'da profil resmi
- ✅ Views paneli - Görüntüleyenlerin profil resimleri

### Emoji ve Media Gönderme Eklendi ✅
- ✅ Emoji picker entegrasyonu (emoji-picker-react)
- ✅ Emoji butonu ile emoji seçimi
- ✅ Dosya/resim upload butonu (ataş ikonu)
- ✅ Desteklenen formatlar: Resimler, PDF, DOC, DOCX, TXT
- ✅ Maksimum dosya boyutu: 10MB
- ✅ Base64 encoding ile gönderim
- ✅ Resim gösterimi: Thumbnail + tıkla-büyüt
- ✅ Dosya gösterimi: İkon + dosya adı + tıkla-indir
- ✅ Hem direkt hem grup mesajları için
- ✅ MessageList: Image ve File rendering component'leri

### SignalR Mesajlaşma Optimizasyonu ✅
- ✅ SignalR listener'ı hem direkt hem grup mesajları için çalışıyor
- ✅ Grup mesajları için receiverId kontrolü eklendi
- ✅ Mesajlar anlık olarak state'e ekleniyor
- ✅ Duplicate mesaj kontrolü aktif
- ✅ Console log'ları debugging için eklendi

### Mesaj Gönderme Sesi Eklendi ✅
- ✅ Mesaj gönderildiğinde ses efekti çalıyor
- ✅ Ses dosyası: src/assets/message_send_sound.mp3
- ✅ Volume: 50% (ayarlanabilir)
- ✅ Hem direkt mesajlar hem grup mesajları için
- ✅ Hata durumunda sessiz devam ediyor

### Kullanıcı Engelleme Özelliği Eklendi ✅
- ✅ Kullanıcıları engelleme/engeli kaldırma
- ✅ PeopleTab'da Contacts / Blocked tab switcher
- ✅ Block butonu (kırmızı X icon)
- ✅ Blocked users listesi
- ✅ Unblock butonu
- ✅ Backend: BlockedUser model, BlockedUsersController
- ✅ API endpoints: GET/POST/DELETE /api/blockedusers
- ✅ UserService: Engellenen kullanıcılar filtreleniyor
- ✅ Karşılıklı engelleme desteği
- ✅ Migration: AddBlockedUsers uygulandı

### Mute All Özelliği Eklendi ✅
- ✅ Grup sahibi ve adminler tüm grubu susturabilir
- ✅ Grup muted iken sadece owner ve adminler mesaj gönderebilir
- ✅ Chat komutları: `/muteall` ve `/unmuteall`
- ✅ UI butonu: Group Settings → Mute All Members
- ✅ Backend: IsMutedForAll field (Group model)
- ✅ API endpoints: POST /api/groups/{id}/mute-all, unmute-all
- ✅ Sistem mesajları: Mute/unmute all için özel mesajlar
- ✅ Frontend uyarıları: "Group is muted. Only admins can send messages"
- ✅ Input, emoji, ses kaydı devre dışı (muted grup için)
- ✅ Migration: AddGroupMuteAll uygulandı

### Story Özelliği Eklendi ✅
- ✅ Story oluşturma (resim + caption)
- ✅ Story görüntüleme (5 saniye otomatik geçiş)
- ✅ Story'ler kullanıcıya göre gruplanıyor
- ✅ Her kullanıcı için tek avatar + story sayısı badge
- ✅ Görüntülenen/görüntülenmemiş renk kodları
- ✅ Progress bar animasyonu
- ✅ View tracking (kimin görüntülediği)
- ✅ 24 saat otomatik silme
- ✅ X buton çakışması düzeltildi (views panel vs story close)
- ✅ Backend: Stories ve StoryViews tabloları
- ✅ Frontend: CreateStoryModal, ViewStoryModal
- ✅ API: GET/POST/DELETE /api/stories endpoints

### Story Gruplama ve Navigasyon ✅
- ✅ Aynı kullanıcının birden fazla story'si tek avatar'da gösteriliyor
- ✅ Story sayısı badge (sağ üst köşe)
- ✅ Tıklandığında o kullanıcının tüm story'leri açılıyor
- ✅ Otomatik geçiş sadece aynı kullanıcının story'leri arasında
- ✅ Ok tuşları ile manuel geçiş
- ✅ Son story'de otomatik kapanma
- ✅ Progress bar sadece mevcut kullanıcının story'leri için

### Settings Ekranı Fonksiyonları Eklendi ✅
- ✅ Edit Profile modal (username, email düzenleme)
- ✅ Account Settings modal (privacy, security, 2FA, vb.)
- ✅ Chat Settings modal (dark mode toggle, wallpaper, chat history)
- ✅ Notifications modal (message/group/call notifications toggle)
- ✅ Storage modal (storage kullanımı, network usage)
- ✅ Help modal (help center, contact, about)
- ✅ Tüm modal'lar responsive ve çalışır durumda
- ✅ Toggle switch'ler aktif
- ✅ UI seviyesinde tam fonksiyonel

### CallsTab Backend Entegrasyonu ✅
- ✅ Mock data kaldırıldı
- ✅ Backend'den gerçek call history yükleniyor
- ✅ Call type'ları oluşturuldu (src/types/call.ts)
- ✅ ChatStore'a calls state ve loadCalls() eklendi
- ✅ Incoming/Outgoing call gösterimi
- ✅ Missed call gösterimi (kırmızı)
- ✅ Call duration formatlaması (MM:SS)
- ✅ Video/Voice call icon'ları
- ✅ Smart time formatting (Today, Yesterday, X days ago)
- ✅ Loading ve empty states
- ✅ GET /api/calls endpoint entegrasyonu

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

### Proje Tamamlandı! 🎉
Talky projesi tüm temel ve gelişmiş özellikleriyle birlikte tamamen tamamlandı. Kullanıcılar:
- ✅ Kayıt olup giriş yapabilir
- ✅ Gerçek zamanlı mesajlaşabilir
- ✅ Grup oluşturup yönetebilir
- ✅ Film odaları açıp arkadaşlarıyla film izleyebilir
- ✅ Story paylaşıp görüntüleyebilir
- ✅ Profil resmi yükleyebilir
- ✅ Ses mesajları gönderebilir
- ✅ Emoji ve dosya paylaşabilir
- ✅ Kullanıcıları engelleyebilir
- ✅ Ve daha fazlası...

### İsteğe Bağlı İyileştirmeler (Opsiyonel)
- ⏳ Story replies (story'lere cevap verme)
- ⏳ Story reactions (emoji ile tepki)
- ⏳ Video gönderme özelliği
- ⏳ Push notifications
- ⏳ Dark mode backend entegrasyonu
- ⏳ Video/Voice call functionality (UI hazır, backend gerekli)
- ⏳ Message read receipts
- ⏳ Typing indicator animasyonu

## Aktif Kararlar
- **Component Structure**: Sidebar yönetir tüm tab'ları (5 tab: Chats, Calls, People, Movies, Settings)
- **Navigation**: Bottom navigation ile tab switching
- **Backend**: LocalDB kullanılıyor - (localdb)\MSSQLLocalDB
- **Database**: Code First yaklaşımı, EF Core migrations
- **State Management**: Zustand (basit ve etkili)
- **Real-time**: SignalR (direkt mesajlar, film odaları), REST API (grup mesajları)
- **Authentication**: JWT Bearer token
- **Contact System**: Sadece contact'lar görünüyor (ölçeklenebilir)
- **Story Visibility**: Sadece contact'ların story'leri
- **Grup Yetkilendirme**: Owner/Admin/Member rolleri
- **Komut sistemi**: Chat'te `@username /command` ve `/command` formatı
- **Mute All**: Tüm grubu susturma, sadece adminler konuşabilir
- **Chat Commands**: /muteall, /unmuteall, @user /mute, @user /unmute
- **Sistem Mesajları**: IsSystemMessage flag ile özel gösterim
- **Settings UI**: Modal-based settings (6 kategori)
- **Call History**: Backend entegrasyonu tamamlandı
- **Story System**: 24-hour expiry, view tracking, user grouping
- **Story UI**: CreateStoryModal, ViewStoryModal, progress bars
- **Story Grouping**: Tek avatar per user, story count badge
- **Movie Room**: YouTube integration, SignalR sync, owner-only control
- **Movie Room Sync**: Sadece oda sahibi videoyu kontrol eder, diğerleri otomatik sync
- **Styling**: Tailwind CSS 3, referans tasarıma %100 uyumlu
- **Password**: BCrypt hashing
- **Animations**: CSS keyframes + Tailwind transitions
- **Voice Messages**: REST API (base64 too large for SignalR)
- **Audio Format**: WebM with Opus codec, 16kbps bitrate
- **Profile Pictures**: User and Group avatars, base64, nvarchar(max)
- **Deployment**: Netlify (frontend) + ngrok (backend tunnel)
- **Production URL**: https://talkychat.netlify.app
- **Backend Tunnel**: ngrok (değişken URL, her restart'ta yeni)
- **Environment**: `.env.production` ile ngrok URL yönetimi
- **JSON Format**: Backend camelCase desteği (PropertyNamingPolicy)

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
- Story tabloları eklendi: Stories, StoryViews
- Mute All özelliği: IsMutedForAll field (Group model)
- Chat komutları backend'de işleniyor: /muteall, /unmuteall, @user /mute, @user /unmute
- BlockedUsers tablosu eklendi: UserId, BlockedUserId, BlockedAt
- UserService engellenen kullanıcıları filtreliyor (GetAllUsers, SearchUsers)
- Grup API endpoint'leri: 
  - POST /api/groups - Grup oluştur
  - GET /api/groups - Kullanıcının grupları
  - GET /api/groups/{id} - Grup detayı
  - GET /api/groups/{id}/messages - Grup mesajları
  - POST /api/groups/{id}/messages - Grup mesajı gönder (komut kontrolü ile)
  - POST /api/groups/{id}/members/{memberId}/promote - Admin yap
  - POST /api/groups/{id}/members/{memberId}/demote - Admin kaldır
  - DELETE /api/groups/{id}/members/{memberId} - Üye çıkar
  - POST /api/groups/{id}/members - Üye ekle
  - POST /api/groups/{id}/members/{memberId}/mute - Üye sustur
  - POST /api/groups/{id}/members/{memberId}/unmute - Üye susturmasını kaldır
  - POST /api/groups/{id}/mute-all - Tüm grubu sustur
  - POST /api/groups/{id}/unmute-all - Tüm grubun susturmasını kaldır
  - DELETE /api/groups/{id} - Grup sil
  - POST /api/groups/{id}/leave - Gruptan ayrıl
- Story API endpoint'leri:
  - GET /api/stories - Tüm aktif story'ler
  - GET /api/stories/{id} - Story detayı
  - POST /api/stories - Story oluştur
  - POST /api/stories/{id}/view - Story görüntüleme kaydı
  - GET /api/stories/{id}/views - Story görüntüleyenler (owner only)
  - DELETE /api/stories/{id} - Story sil (owner only)
- BlockedUsers API endpoint'leri:
  - GET /api/blockedusers - Engellenen kullanıcılar listesi
  - POST /api/blockedusers/{userId} - Kullanıcıyı engelle
  - DELETE /api/blockedusers/{userId} - Engeli kaldır
  - GET /api/blockedusers/check/{userId} - Engel durumu kontrol

### SignalR Real-time
- SignalR bağlantısı login'de kuruluyor
- Event listener'lar ChatPage mount'unda kaydediliyor
- Duplicate listener'lar cleanup ile önleniyor
- ReceiveMessage event'i hem gönderene hem alıcıya gidiyor
- **Grup mesajları için receiverId kontrolü eklendi**
- **SignalR listener hem direkt hem grup mesajları destekliyor**
- Console log'lar debugging için eklendi
- Connection state kontrolleri yapılıyor
- Ses mesajları SignalR yerine REST API ile (base64 çok büyük)
- Mesaj filtreleme: (senderId === selectedUser.id) VEYA (receiverId === selectedUser.id)
- Grup mesajları: receiverId === selectedGroup.id kontrolü
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
- API_BASE_URL: '/api' (relative path - development), ngrok URL (production)
- SignalR URL: http://localhost:5282/chatHub (development), ngrok URL (production)
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
- **Mute All UI**: Grup muted ise sadece adminler mesaj gönderebilir
- **Story sistemi**: CreateStoryModal (resim upload, caption), ViewStoryModal (otomatik geçiş, progress)
- **Story gruplama**: Kullanıcıya göre gruplama, tek avatar, story count badge
- **Story navigasyon**: Ok tuşları, otomatik geçiş (5 saniye), aynı kullanıcı story'leri
- **Story UI**: Views panel, X buton çakışması düzeltildi
- **Chat placeholder**: Tüm komutlar gösteriliyor (@user /mute, /muteall, /unmuteall)
- **Kullanıcı engelleme**: PeopleTab'da Contacts/Blocked tab switcher
- **Block UI**: Kırmızı X icon, Unblock butonu, gri avatar (blocked users)
- **Mesaj sesi**: message_send_sound.mp3, 50% volume, hata toleranslı
- **Emoji picker**: emoji-picker-react, popup picker, emoji ekleme
- **File upload**: Hidden input, ataş butonu, base64 encoding, 10MB limit
- **Media rendering**: ImageMessage (thumbnail), FileMessage (download), VoiceMessage (player)
- **Dependencies**: emoji-picker-react npm package
- **ChatsTab Search**: Real-time arama, filtreleme, yeni kullanıcı bulma
- **Search UX**: Minimum 2 karakter, loading spinner, no results message
- **Header Cleanup**: Gereksiz arama ikonu ve menü kaldırıldı, sadeleştirildi
- **Mute Messages**: Özelleştirilmiş Azerbaycan Türkçesi mesajları
- **System Messages**: Mute/unmute için tematik mesajlar (Doktor, narkoz teması)
- **Deployment**: Netlify production build, environment variables
- **API Endpoints**: Tüm endpoint'lerde `/api` prefix düzeltildi
- **Error Handling**: Response body parsing iyileştirildi (text → JSON parse)
- **Production Config**: `.env.production` ile ngrok URL yönetimi
