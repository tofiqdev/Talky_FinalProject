# Talky - Real-time Messaging Platform

A modern, production-ready real-time messaging web application built with React and .NET 8, featuring instant messaging, group chats, and voice messages.

## 🚀 Features

- ✅ **Real-time Messaging**: Instant message delivery with SignalR WebSocket
- ✅ **Stories**: Create and view 24-hour stories with images and captions
- ✅ **Group Chats**: Create groups, manage members, role-based permissions (Owner/Admin/Member)
- ✅ **Mute/Unmute System**: Moderators can mute members via UI or chat commands (`@username /mute`)
- ✅ **Voice Messages**: Hold-to-record feature with Opus codec compression
- ✅ **User Management**: Registration, login, JWT authentication
- ✅ **Online Status**: Real-time online/offline indicators
- ✅ **User Search**: Find and connect with users by username
- ✅ **Message History**: Persistent message storage and retrieval
- ✅ **System Messages**: Special notifications for group actions (mute, unmute, etc.)
- ✅ **Multiple Tabs**: 
  - 💬 Chats - Direct messages, group conversations, and stories
  - 📞 Calls - Call history (incoming/outgoing/missed)
  - 👥 People - User search and contacts
  - ⚙️ Settings - Profile and app settings
- ✅ **Modern UI**: Clean, responsive design with Tailwind CSS
- ✅ **Smooth Animations**: Fade-in effects, auto-scroll, hover transitions

## 🛠️ Tech Stack

### Frontend
- **React** 19.2.0 - UI library
- **TypeScript** 5.9.3 - Type safety
- **Vite** 7.2.4 - Build tool and dev server
- **Tailwind CSS** 3.x - Utility-first CSS framework
- **React Router** 7.x - Client-side routing
- **Zustand** 5.x - State management
- **SignalR Client** 10.x - Real-time communication

### Backend
- **.NET 8** - C# Web API framework
- **ASP.NET Core** - Web API
- **SignalR** - WebSocket/real-time communication
- **Entity Framework Core** 8.0.0 - ORM
- **SQL Server LocalDB** - Database
- **JWT Bearer** 8.0.0 - Authentication
- **BCrypt.Net-Next** 4.0.3 - Password hashing
- **Swagger** 6.6.2 - API documentation

## 📦 Installation & Setup

### Prerequisites
- Node.js 18+ (for frontend)
- .NET 8 SDK (for backend)
- SQL Server LocalDB (comes with Visual Studio)

### Frontend Setup
```bash
# Install dependencies
npm install

# Run development server
npm run dev
# Frontend runs on http://localhost:5174

# Build for production
npm run build

# Preview production build
npm run preview
```

### Backend Setup
```bash
# Navigate to backend directory
cd back/TalkyAPI

# Restore packages
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the API
dotnet run
# Backend runs on http://localhost:5282
# Swagger UI: http://localhost:5282/swagger

# Or run with hot reload
dotnet watch run
```

## 📁 Project Structure

```
Talky/
├── src/                          # Frontend (React + TypeScript)
│   ├── components/
│   │   ├── sidebar/             # ChatsTab, CallsTab, PeopleTab, SettingsTab
│   │   ├── chat/                # ChatWindow, MessageList
│   │   └── group/               # CreateGroupModal, GroupDetailsModal
│   ├── pages/                   # LoginPage, RegisterPage, ChatPage
│   ├── services/                # signalrService (WebSocket connection)
│   ├── store/                   # authStore, chatStore (Zustand)
│   ├── types/                   # TypeScript type definitions
│   ├── App.tsx                  # Router and routes
│   └── main.tsx                 # Entry point
│
├── back/TalkyAPI/               # Backend (.NET 8)
│   ├── Controllers/             # Auth, Users, Messages, Groups, Calls
│   ├── Models/                  # User, Message, Group, GroupMember, GroupMessage
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Data/                    # AppDbContext (Entity Framework)
│   ├── Hubs/                    # ChatHub (SignalR)
│   └── Program.cs               # Application entry point
│
├── memory-bank/                 # Project documentation
│   ├── projectbrief.md
│   ├── productContext.md
│   ├── techContext.md
│   ├── activeContext.md
│   ├── progress.md
│   └── systemPatterns.md
│
└── README.md
```

## 🎨 Design

- **Primary Color**: Cyan (#06B6D4) for direct messages
- **Secondary Color**: Purple/Pink gradient for group chats
- **Message Bubbles**: Pill-shaped design
- **Navigation**: Bottom tab navigation (4 tabs)
- **Layout**: Sidebar + Chat window
- **Responsive**: Mobile-first approach with Tailwind CSS
- **Animations**: Smooth transitions, fade-in effects, auto-scroll

## 🔐 Security Features

- JWT Bearer token authentication
- BCrypt password hashing
- CORS configuration
- Authorization middleware
- Secure WebSocket connections
- Role-based access control (Owner/Admin/Member)

## 📡 API Endpoints

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `GET /api/auth/me` - Get current user (JWT required)

### Users
- `GET /api/users` - Get all users (JWT required)
- `GET /api/users/search?q=term` - Search users (JWT required)

### Messages
- `GET /api/messages/{userId}` - Get message history (JWT required)
- `POST /api/messages` - Send message (JWT required)

### Groups
- `GET /api/groups` - Get user's groups (JWT required)
- `POST /api/groups` - Create group (JWT required)
- `GET /api/groups/{id}` - Get group details (JWT required)
- `GET /api/groups/{id}/messages` - Get group messages (JWT required)
- `POST /api/groups/{id}/messages` - Send group message (JWT required)
- `POST /api/groups/{id}/members` - Add member (JWT required)
- `DELETE /api/groups/{id}/members/{memberId}` - Remove member (JWT required)
- `POST /api/groups/{id}/members/{memberId}/promote` - Promote to admin (JWT required)
- `POST /api/groups/{id}/members/{memberId}/demote` - Demote from admin (JWT required)
- `POST /api/groups/{id}/members/{memberId}/mute` - Mute member (JWT required)
- `POST /api/groups/{id}/members/{memberId}/unmute` - Unmute member (JWT required)
- `DELETE /api/groups/{id}` - Delete group (JWT required)
- `POST /api/groups/{id}/leave` - Leave group (JWT required)

### Stories
- `GET /api/stories` - Get all active stories (JWT required)
- `GET /api/stories/{id}` - Get story details (JWT required)
- `POST /api/stories` - Create story (JWT required)
- `POST /api/stories/{id}/view` - Mark story as viewed (JWT required)
- `GET /api/stories/{id}/views` - Get story views (JWT required, owner only)
- `DELETE /api/stories/{id}` - Delete story (JWT required, owner only)

### SignalR Hub
- `/chatHub` - WebSocket endpoint for real-time messaging

## 🎮 Chat Commands

### Group Moderation Commands
Moderators (Owner and Admins) can use these commands in group chats:

**Member Mute/Unmute:**
- `@username /mute` - Mute a member (prevents them from sending messages)
- `@username /unmute` - Unmute a member

**Group-wide Mute/Unmute:**
- `/muteall` - Mute all members (only admins can send messages)
- `/unmuteall` - Unmute all members (everyone can send messages)

**Example:**
```
@john /mute
/muteall
/unmuteall
```

**System Messages:**
- Mute: "Şəhər yatır, Mafiya oyaqdır. @john isə artıq danışmır. By @admin"
- Unmute: "@john artık konuşabilir. Unmuted by @admin"
- Mute All: "🔇 Grup susturuldu. Sadece yöneticiler mesaj gönderebilir. By @admin"
- Unmute All: "🔊 Grup susturması kaldırıldı. Herkes mesaj gönderebilir. By @admin"

**Notes:**
- Only Owner and Admins can use moderation commands
- Group owner cannot be muted
- Muted members see a warning and cannot send messages
- When group is muted for all, only admins can send messages
- Commands are case-insensitive

## 🔜 Optional Enhancements

- [ ] Real-time group messages via SignalR (currently using REST API)
- [ ] Story replies and reactions
- [ ] Typing indicator
- [ ] Message read receipts
- [ ] File/image sharing
- [ ] Emoji picker
- [ ] Voice/video calls
- [ ] Dark mode
- [ ] Push notifications
- [ ] Message reactions

## 📝 Database Schema

### Tables
- **Users** - User accounts and profiles
- **Messages** - Direct messages between users
- **Groups** - Group information
- **GroupMembers** - Group membership and roles
- **GroupMessages** - Messages in groups
- **Stories** - User stories (24-hour expiry)
- **StoryViews** - Story view tracking
- **Calls** - Call history

## 🧪 Testing

The application is fully functional and tested:
- ✅ User registration and login
- ✅ Real-time direct messaging
- ✅ Group creation and management
- ✅ Voice message recording and playback
- ✅ User search and discovery
- ✅ Online/offline status updates
- ✅ Role-based permissions

## 📚 Documentation

Detailed project documentation is available in the `memory-bank/` directory:
- `projectbrief.md` - Project overview and goals
- `productContext.md` - Product features and user flows
- `techContext.md` - Technology stack and setup
- `activeContext.md` - Current development status
- `progress.md` - Completed features and roadmap
- `systemPatterns.md` - Architecture and design patterns

## 📝 License

This project is part of a final project.

## 👨‍💻 Author

Tofiq - [GitHub](https://github.com/tofiqdev)
