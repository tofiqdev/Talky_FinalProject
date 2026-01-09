import { useChatStore } from '../../store/chatStore';
import type { User } from '../../types/user';

export default function ChatsTab() {
  const { users, selectedUser, setSelectedUser } = useChatStore();

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
    <>
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
    </>
  );
}
