import { useChatStore } from '../../store/chatStore';
import { useAuthStore } from '../../store/authStore';
import type { User } from '../../types/user';

export default function UserList() {
  const { users, selectedUser, setSelectedUser } = useChatStore();
  const { user: currentUser, logout } = useAuthStore();

  // Mock users
  const mockUsers: User[] = [
    { id: '2', username: 'Jordan Smith', email: 'jordan@test.com', isOnline: true },
    { id: '3', username: 'Alex Rivier', email: 'alex@test.com', isOnline: true },
    { id: '4', username: 'Chris Evans', email: 'chris@test.com', isOnline: false },
    { id: '5', username: 'Taylor Reed', email: 'taylor@test.com', isOnline: false },
    { id: '6', username: 'Maria Garcia', email: 'maria@test.com', isOnline: false },
  ];

  const displayUsers = users.length > 0 ? users : mockUsers;

  // Story users
  const storyUsers = [
    { id: 's1', name: 'Your Story', hasStory: true },
    { id: 's2', name: 'Alex', hasStory: false },
    { id: 's3', name: 'Jordan', hasStory: false },
    { id: 's4', name: 'Sarah', hasStory: false },
  ];

  return (
    <div className="w-[370px] bg-white flex flex-col h-screen">
      {/* Header */}
      <div className="px-5 py-4 flex items-center justify-between border-b border-gray-100">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-full bg-cyan-500 flex items-center justify-center">
            <svg className="w-6 h-6 text-white" fill="currentColor" viewBox="0 0 24 24">
              <path d="M20 2H4c-1.1 0-2 .9-2 2v18l4-4h14c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2z"/>
            </svg>
          </div>
          <h1 className="text-2xl font-bold text-gray-900">Messages</h1>
        </div>
        <div className="flex items-center gap-2">
          <button className="w-9 h-9 rounded-full hover:bg-gray-100 flex items-center justify-center">
            <svg className="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
          </button>
          <button className="w-9 h-9 rounded-full hover:bg-gray-100 flex items-center justify-center">
            <svg className="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 5v.01M12 12v.01M12 19v.01M12 6a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2z" />
            </svg>
          </button>
        </div>
      </div>

      {/* Stories */}
      <div className="px-5 py-4 flex gap-4 overflow-x-auto border-b border-gray-100">
        {storyUsers.map((story) => (
          <div key={story.id} className="flex flex-col items-center gap-1 min-w-[60px]">
            <div className={`w-14 h-14 rounded-full flex items-center justify-center ${story.hasStory ? 'bg-gradient-to-br from-cyan-400 to-blue-500 p-0.5' : 'bg-gray-200'}`}>
              <div className="w-full h-full rounded-full bg-white flex items-center justify-center">
                <div className="w-12 h-12 rounded-full bg-gray-300"></div>
              </div>
            </div>
            <span className="text-xs text-gray-700">{story.name}</span>
          </div>
        ))}
      </div>

      {/* Chat List */}
      <div className="flex-1 overflow-y-auto">
        {displayUsers.map((user, index) => {
          const messages = [
            'Are you coming to the party...',
            'Sounds good! See you then. ✅',
            "I've sent the document via email.",
            'Typing...',
            'Happy Birthday! Hope you hav...'
          ];
          const times = ['NOW', '2H', '4H', '6d', 'YESTERDAY'];
          const unreadCounts = [1, 0, 0, 0, 0];
          
          return (
            <div
              key={user.id}
              onClick={() => setSelectedUser(user)}
              className={`px-5 py-3 cursor-pointer hover:bg-gray-50 transition flex items-center gap-3 ${
                selectedUser?.id === user.id ? 'bg-gray-50 border-l-4 border-cyan-500' : ''
              }`}
            >
              <div className="relative flex-shrink-0">
                <div className="w-12 h-12 rounded-full bg-gray-300"></div>
                {user.isOnline && (
                  <div className="absolute bottom-0 right-0 w-3 h-3 bg-green-500 border-2 border-white rounded-full"></div>
                )}
              </div>
              <div className="flex-1 min-w-0">
                <div className="flex items-center justify-between mb-1">
                  <h3 className="font-semibold text-gray-900 text-sm">{user.username}</h3>
                  <span className="text-xs text-gray-500">{times[index]}</span>
                </div>
                <div className="flex items-center justify-between">
                  <p className={`text-sm truncate ${index === 3 ? 'text-cyan-500' : 'text-gray-600'}`}>
                    {index === 1 && <span className="mr-1">✓</span>}
                    {messages[index]}
                  </p>
                  {unreadCounts[index] > 0 && (
                    <span className="ml-2 w-5 h-5 rounded-full bg-cyan-500 text-white text-xs flex items-center justify-center flex-shrink-0">
                      {unreadCounts[index]}
                    </span>
                  )}
                </div>
              </div>
            </div>
          );
        })}
      </div>

      {/* Bottom Navigation */}
      <div className="border-t border-gray-100 px-5 py-3 flex justify-around">
        <button className="flex flex-col items-center gap-1 text-cyan-500">
          <svg className="w-6 h-6" fill="currentColor" viewBox="0 0 24 24">
            <path d="M20 2H4c-1.1 0-2 .9-2 2v18l4-4h14c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2z"/>
          </svg>
          <span className="text-xs font-medium">CHATS</span>
        </button>
        <button className="flex flex-col items-center gap-1 text-gray-400">
          <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
          </svg>
          <span className="text-xs">CALLS</span>
        </button>
        <button className="flex flex-col items-center gap-1 text-gray-400">
          <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
          </svg>
          <span className="text-xs">PEOPLE</span>
        </button>
        <button className="flex flex-col items-center gap-1 text-gray-400">
          <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
          </svg>
          <span className="text-xs">SETTINGS</span>
        </button>
      </div>
    </div>
  );
}
