# Tech Context

## Kullanılan Teknolojiler

### Frontend ✅
- **React**: 19.2.0 - UI library
- **TypeScript**: ~5.9.3 - Tip güvenliği
- **Vite**: ^7.2.4 - Build tool ve dev server
- **Tailwind CSS**: 3.x - Utility-first CSS framework
- **@microsoft/signalr**: ^8.x - Gerçek zamanlı iletişim client
- **React Router**: ^7.x - Sayfa yönlendirme
- **Zustand**: ^5.x - State management

### Backend ✅ (Production Ready)
- **.NET 8**: C# Web API framework
- **ASP.NET Core**: Web API
- **SignalR**: WebSocket/gerçek zamanlı iletişim ✅ Çalışıyor
- **Entity Framework Core**: 8.0.0 - ORM
- **SQL Server LocalDB**: (localdb)\MSSQLLocalDB - Database
- **JWT Bearer**: 8.0.0 - Authentication ✅ Aktif
- **BCrypt.Net-Next**: 4.0.3 - Password hashing
- **Swagger**: 6.6.2 - API documentation

### Development Tools
- **ESLint**: Frontend code linting
- **PostCSS**: CSS processing
- **Autoprefixer**: Browser compatibility

## Geliştirme Ortamı

### Frontend Kurulum ve Çalıştırma
```bash
npm install
npm run dev        # http://localhost:5174
npm run build      # Production build
npm run preview    # Preview production build
```

### Production Deployment (Netlify)
```bash
# Build
npm run build

# Deploy
netlify deploy --prod --dir=dist

# Production URL
https://talkychat.netlify.app
```

### Backend Kurulum ve Çalıştırma
```bash
cd back/TalkyAPI

# Paketleri yükle
dotnet restore

# Database migration (ilk kez)
dotnet ef migrations add InitialCreate
dotnet ef database update

# Uygulamayı çalıştır
dotnet run
# veya
dotnet watch run  # Hot reload ile
```

**API URL**: http://localhost:5282 (development)
**Production Tunnel**: https://a0f569cfa40e.ngrok-free.app (ngrok)
**Swagger UI**: http://localhost:5282/swagger (development)
**Swagger UI**: https://a0f569cfa40e.ngrok-free.app/swagger (production)
**Database**: TalkyDB @ (localdb)\MSSQLLocalDB

### ngrok Tunnel (Production)
```bash
# ngrok başlat
C:\ngrok\ngrok.exe http 5282

# ngrok URL'i .env.production'a ekle
VITE_API_URL=https://[ngrok-url].ngrok-free.app

# Frontend'i build ve deploy et
npm run build
netlify deploy --prod --dir=dist
```

**Not**: ngrok her yeniden başlatıldığında URL değişir. Yeni URL'i `.env.production` dosyasına yazıp yeniden build/deploy etmek gerekir.

## Proje Yapısı

```
/
├── src/
│   ├── components/
│   │   └── chat/
│   │       ├── UserList.tsx       # Sol sidebar (stories, chat list, nav)
│   │       ├── ChatWindow.tsx     # Sağ panel (header, input)
│   │       └── MessageList.tsx    # Mesaj baloncukları
│   ├── pages/
│   │   ├── LoginPage.tsx          # Giriş sayfası
│   │   ├── RegisterPage.tsx       # Kayıt sayfası
│   │   └── ChatPage.tsx           # Ana chat sayfası
│   ├── services/
│   │   └── signalrService.ts      # SignalR connection manager
│   ├── store/
│   │   ├── authStore.ts           # Auth state (Zustand)
│   │   └── chatStore.ts           # Chat state (Zustand)
│   ├── types/
│   │   ├── user.ts                # User, Auth types
│   │   └── message.ts             # Message types
│   ├── App.tsx                    # Router ve routes
│   ├── main.tsx                   # Entry point
│   └── index.css                  # Tailwind directives
├── public/
├── index.html
├── tailwind.config.js
├── postcss.config.js
├── vite.config.ts
└── package.json
```

## API Endpoints (Backend için planlanan)

### Authentication
- POST /api/auth/register
- POST /api/auth/login
- GET /api/auth/me

### Users
- GET /api/users
- GET /api/users/{id}

### Messages
- GET /api/messages/{userId}
- POST /api/messages

### SignalR Hub
- /chatHub

## SignalR Methods (Planlanan)

### Client → Server
- SendMessage(receiverId, content)
- TypingIndicator(receiverId, isTyping)
- MarkAsRead(messageId)

### Server → Client
- ReceiveMessage(message)
- UserOnline(userId)
- UserOffline(userId)
- TypingIndicator(userId, isTyping)

## Teknik Kısıtlamalar
- Node.js 18+ gerekli
- Modern browser (WebSocket desteği)
- .NET 8 SDK (backend için)

## Bağımlılıklar

### Frontend (package.json)
```json
{
  "dependencies": {
    "react": "^19.2.0",
    "react-dom": "^19.2.0",
    "@microsoft/signalr": "^8.x",
    "react-router-dom": "^7.x",
    "zustand": "^5.x"
  },
  "devDependencies": {
    "tailwindcss": "^3.x",
    "postcss": "^8.x",
    "autoprefixer": "^10.x",
    "typescript": "~5.9.3",
    "vite": "^7.2.4"
  }
}
```

## Önemli Notlar
- Tailwind CSS v4 yerine v3 kullanıldı (PostCSS uyumluluk)
- TypeScript type-only imports kullanılıyor
- SignalR bağlantı hatası gracefully handle ediliyor
- localStorage ile token persistence
- Mock data ile backend olmadan çalışıyor
