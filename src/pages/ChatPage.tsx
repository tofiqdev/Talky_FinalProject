import { useEffect, useState } from 'react';
import { useChatStore } from '../store/chatStore';
import { useAuthStore } from '../store/authStore';
import { signalrService } from '../services/signalrService';
import Sidebar from '../components/sidebar/Sidebar';
import ChatWindow from '../components/chat/ChatWindow';

export default function ChatPage() {
  const { token } = useAuthStore();
  const { selectedUser, addMessage, updateUserStatus } = useChatStore();
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    if (token) {
      signalrService.connect(token).then(() => {
        setIsConnected(true);

        signalrService.onReceiveMessage((message) => {
          addMessage(message);
        });

        signalrService.onUserOnline((userId) => {
          updateUserStatus(userId, true);
        });

        signalrService.onUserOffline((userId) => {
          updateUserStatus(userId, false);
        });
      }).catch(() => {
        // SignalR bağlantısı başarısız - backend olmadan çalışıyor
        console.log('SignalR connection failed - running in demo mode');
      });
    }

    return () => {
      signalrService.disconnect();
    };
  }, [token]);

  return (
    <div className="h-screen flex bg-white">
      <Sidebar />
      {selectedUser ? (
        <ChatWindow />
      ) : (
        <div className="flex-1 flex items-center justify-center bg-gray-50">
          <div className="text-center">
            <div className="w-32 h-32 mx-auto mb-6 rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 flex items-center justify-center">
              <svg className="w-16 h-16 text-white" fill="currentColor" viewBox="0 0 24 24">
                <path d="M20 2H4c-1.1 0-2 .9-2 2v18l4-4h14c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2z"/>
              </svg>
            </div>
            <h2 className="text-3xl font-bold text-gray-800 mb-2">Talky</h2>
            <p className="text-gray-600">Select a conversation to start messaging</p>
          </div>
        </div>
      )}
    </div>
  );
}
