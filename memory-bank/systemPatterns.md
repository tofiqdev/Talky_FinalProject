# System Patterns

## Mimari Yapı

### Genel Mimari
```
Client (React) ←→ SignalR Hub ←→ Backend API ←→ Database
                      ↓
                 JWT Auth
```

### Frontend Klasör Yapısı
```
src/
├── components/
│   ├── chat/
│   │   ├── ChatWindow.tsx      # Sağ panel (header, input)
│   │   └── MessageList.tsx     # Mesaj baloncukları
│   └── sidebar/
│       ├── Sidebar.tsx         # Ana sidebar container
│       ├── ChatsTab.tsx        # Stories + chat listesi
│       ├── CallsTab.tsx        # Arama geçmişi
│       ├── PeopleTab.tsx       # Kişiler listesi
│       └── SettingsTab.tsx     # Ayarlar menüsü
├── pages/
│   ├── LoginPage.tsx           # Giriş sayfası
│   ├── RegisterPage.tsx        # Kayıt sayfası
│   └── ChatPage.tsx            # Ana chat sayfası
├── services/
│   └── signalrService.ts       # SignalR connection manager
├── store/
│   ├── authStore.ts            # Auth state (Zustand)
│   └── chatStore.ts            # Chat state (Zustand)
├── types/
│   ├── user.ts                 # User, Auth types
│   └── message.ts              # Message types
├── App.tsx                     # Router ve routes
├── main.tsx                    # Entry point
└── index.css                   # Tailwind directives
```

### Backend Klasör Yapısı (Planlanan)
```
backend/
├── Controllers/
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── MessagesController.cs
│   └── CallsController.cs
├── Hubs/
│   └── ChatHub.cs
├── Models/
│   ├── User.cs
│   ├── Message.cs
│   ├── Conversation.cs
│   └── Call.cs
├── Data/
│   └── AppDbContext.cs
├── Services/
│   ├── IAuthService.cs
│   ├── AuthService.cs
│   ├── IMessageService.cs
│   └── MessageService.cs
├── DTOs/
│   ├── LoginDto.cs
│   ├── RegisterDto.cs
│   └── MessageDto.cs
└── Program.cs
```

## Tasarım Desenleri

### Frontend Patterns
- **Tab-based Navigation**: Sidebar component yönetir
- **Component Composition**: Küçük, yeniden kullanılabilir component'ler
- **State Management**: Zustand stores (authStore, chatStore)
- **Service Layer**: SignalR service singleton
- **Mock Data Pattern**: Backend olmadan çalışma

### Backend Patterns (Planlanan)
- **Repository Pattern**: Data access layer
- **Service Layer**: Business logic
- **DTO Pattern**: Data transfer objects
- **Dependency Injection**: .NET Core DI container
- **Hub Pattern**: SignalR için

## Kritik Uygulama Yolları

### Tab Navigation Flow
1. User clicks bottom navigation button
2. Sidebar updates activeTab state
3. Conditional rendering shows correct tab component
4. Tab component renders its content
5. Active tab highlighted with cyan color

### Authentication Flow
1. Kullanıcı login formunu doldurur
2. Mock user ve token oluşturulur (backend için: POST /api/auth/login)
3. authStore.setAuth() çağrılır
4. Token localStorage'a kaydedilir
5. Navigate to /chat
6. Token her API isteğinde header'da gönderilir (backend için)

### Real-time Messaging Flow (Backend ile)
1. Kullanıcı giriş yapar
2. SignalR connection başlatılır (JWT ile)
3. Hub'a bağlanır
4. Mesaj gönderilir → Hub.SendMessage()
5. Hub mesajı database'e kaydeder
6. Hub alıcıya mesajı iletir
7. Frontend mesajı UI'da gösterir

### Message History Flow (Backend ile)
1. Kullanıcı bir sohbet açar
2. GET /api/messages/{userId}
3. Backend database'den mesajları getirir
4. Frontend mesajları listeler

## Component İlişkileri

### ChatPage Component Tree
```
ChatPage
├── Sidebar
│   ├── Header (dynamic)
│   ├── ChatsTab
│   │   ├── Stories
│   │   └── Chat List
│   ├── CallsTab
│   │   └── Call History
│   ├── PeopleTab
│   │   ├── Search Bar
│   │   └── Contact List
│   ├── SettingsTab
│   │   ├── Profile Section
│   │   └── Settings Menu
│   └── Bottom Navigation
└── ChatWindow (conditional)
    ├── Header
    ├── MessageList
    └── Input Area
```

## SignalR Hub Methods (Planlanan)

### Client → Server
- SendMessage(receiverId, content)
- TypingIndicator(receiverId, isTyping)
- MarkAsRead(messageId)
- StartCall(receiverId, callType)
- EndCall(callId)

### Server → Client
- ReceiveMessage(message)
- UserOnline(userId)
- UserOffline(userId)
- TypingIndicator(userId, isTyping)
- IncomingCall(callerId, callType)
- CallEnded(callId)

## Database Schema (Planlanan)

### Users Table
- Id (PK)
- Username
- Email
- PasswordHash
- CreatedAt
- LastSeen
- IsOnline

### Messages Table
- Id (PK)
- SenderId (FK)
- ReceiverId (FK)
- Content
- SentAt
- IsRead

### Calls Table
- Id (PK)
- CallerId (FK)
- ReceiverId (FK)
- CallType (voice/video)
- StartedAt
- EndedAt
- Duration
- Status (missed/completed)

### Conversations Table (opsiyonel)
- Id (PK)
- User1Id (FK)
- User2Id (FK)
- LastMessageAt

## UI Patterns

### Tab Switching
- Active tab: cyan color (#06B6D4)
- Inactive tabs: gray color
- Smooth transitions
- Icon + label design

### List Items
- Hover effect: bg-gray-50
- Avatar + name + status/message
- Time/date on right
- Unread badge (cyan circle)
- Online indicator (green dot)

### Message Bubbles
- Sent: cyan background, rounded-br-md
- Received: gray background, rounded-bl-md
- Max width constraint
- Timestamp below
- Check mark for sent messages
