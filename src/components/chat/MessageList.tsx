import { useChatStore } from '../../store/chatStore';
import { useAuthStore } from '../../store/authStore';
import type { Message } from '../../types/message';

export default function MessageList() {
  const { messages, selectedUser } = useChatStore();
  const { user } = useAuthStore();

  // Mock messages
  const mockMessages: Message[] = [
    {
      id: '1',
      senderId: selectedUser?.id || '',
      receiverId: user?.id || '',
      content: 'Hey! Did you see the new design specs for the messenger app?',
      sentAt: new Date('2024-01-09T10:42:00'),
      isRead: true,
    },
    {
      id: '2',
      senderId: selectedUser?.id || '',
      receiverId: user?.id || '',
      content: 'The pill-shaped bubbles look so clean. I think they really hit that "instant" feel we were going for. 💬',
      sentAt: new Date('2024-01-09T10:42:30'),
      isRead: true,
    },
    {
      id: '3',
      senderId: user?.id || '',
      receiverId: selectedUser?.id || '',
      content: 'Just looking at them now. The speed is incredible! 🚀',
      sentAt: new Date('2024-01-09T10:43:00'),
      isRead: true,
    },
    {
      id: '4',
      senderId: selectedUser?.id || '',
      receiverId: user?.id || '',
      content: 'Exactly! We\'re planning to use a really vibrant electric blue for the sent states. What do you think?',
      sentAt: new Date('2024-01-09T10:45:00'),
      isRead: true,
    },
    {
      id: '5',
      senderId: user?.id || '',
      receiverId: selectedUser?.id || '',
      content: 'Love it. Matches the "high-speed" vibe perfectly. Can we test it on mobile first?',
      sentAt: new Date('2024-01-09T10:48:00'),
      isRead: true,
    },
  ];

  const displayMessages = messages.length > 0 ? messages : mockMessages;

  const filteredMessages = displayMessages.filter(
    (msg) =>
      (msg.senderId === user?.id && msg.receiverId === selectedUser?.id) ||
      (msg.senderId === selectedUser?.id && msg.receiverId === user?.id)
  );

  return (
    <div className="flex-1 overflow-y-auto px-6 py-4 space-y-4">
      {/* Date Divider */}
      <div className="flex items-center justify-center">
        <span className="px-4 py-1 bg-gray-200 text-gray-600 text-xs rounded-full">TODAY</span>
      </div>

      {filteredMessages.map((msg) => {
        const isSent = msg.senderId === user?.id;
        return (
          <div key={msg.id} className={`flex ${isSent ? 'justify-end' : 'justify-start'}`}>
            {!isSent && (
              <div className="w-8 h-8 rounded-full bg-gray-300 mr-2 flex-shrink-0"></div>
            )}
            <div className={`max-w-xl ${isSent ? 'ml-12' : 'mr-12'}`}>
              <div
                className={`px-5 py-3 rounded-3xl ${
                  isSent
                    ? 'bg-cyan-500 text-white rounded-br-md'
                    : 'bg-gray-200 text-gray-900 rounded-bl-md'
                }`}
              >
                <p className="text-sm leading-relaxed">{msg.content}</p>
              </div>
              <div className={`flex items-center gap-1 mt-1 px-2 ${isSent ? 'justify-end' : 'justify-start'}`}>
                <span className="text-xs text-gray-500">
                  {new Date(msg.sentAt).toLocaleTimeString('en-US', {
                    hour: '2-digit',
                    minute: '2-digit',
                  })}
                </span>
                {isSent && (
                  <svg className="w-4 h-4 text-cyan-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                  </svg>
                )}
              </div>
            </div>
          </div>
        );
      })}
    </div>
  );
}
