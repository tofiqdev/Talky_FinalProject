# Talky - Real-time Messaging Platform

A modern, real-time messaging web application built with React and designed for seamless communication.

## 🚀 Features

- **Real-time Messaging**: Instant message delivery with SignalR (ready for backend integration)
- **Multiple Tabs**: 
  - 💬 Chats - Message conversations with stories
  - 📞 Calls - Call history (incoming/outgoing/missed)
  - 👥 People - Contact list with online/offline status
  - ⚙️ Settings - Profile and app settings
- **Modern UI**: Clean, responsive design with Tailwind CSS
- **Authentication**: Login/Register pages (mock data, ready for backend)
- **State Management**: Zustand for efficient state handling
- **TypeScript**: Full type safety

## 🛠️ Tech Stack

### Frontend
- **React** 19.2.0
- **TypeScript** 5.9.3
- **Vite** 7.2.4
- **Tailwind CSS** 3.x
- **React Router** 7.x
- **Zustand** 5.x
- **SignalR Client** 8.x

### Backend (Planned)
- **.NET 8** C# Web API
- **SignalR** for real-time communication
- **Entity Framework Core**
- **SQL Server / PostgreSQL**
- **JWT Authentication**

## 📦 Installation

```bash
# Install dependencies
npm install

# Run development server
npm run dev

# Build for production
npm run build
```

## 🌐 Development

The app runs on `http://localhost:5174`

## 📁 Project Structure

```
src/
├── components/
│   ├── chat/          # Chat window and message components
│   └── sidebar/       # Sidebar with tabs (Chats, Calls, People, Settings)
├── pages/             # Login, Register, Chat pages
├── services/          # SignalR service
├── store/             # Zustand stores (auth, chat)
├── types/             # TypeScript type definitions
└── App.tsx            # Main app with routing
```

## 🎨 Design

- **Primary Color**: Cyan (#06B6D4)
- **Message Bubbles**: Pill-shaped design
- **Navigation**: Bottom tab navigation
- **Responsive**: Mobile-first approach

## 🔜 Roadmap

- [ ] Backend API integration (.NET 8)
- [ ] Real SignalR connection
- [ ] Database implementation
- [ ] File/image sharing
- [ ] Voice/video calls
- [ ] Group chats
- [ ] Dark mode
- [ ] Push notifications

## 📝 License

This project is part of a final project.

## 👨‍💻 Author

Tofiq - [GitHub](https://github.com/tofiqdev)
