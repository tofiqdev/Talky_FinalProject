import { useChatStore } from '../../store/chatStore';

export default function ChatsTab() {
  const { users, selectedUser, setSelectedUser } = useChatStore();

  const getLastSeenText = (lastSeen: string | null | undefined, isOnline: boolean) => {
    if (isOnline) return 'NOW';
    if (!lastSeen) return 'OFFLINE';
    
    const date = new Date(lastSeen);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 60) return `${diffMins}M`;
    if (diffHours < 24) return `${diffHours}H`;
    if (diffDays === 1) return 'YESTERDAY';
    return `${diffDays}D`;
  };

  return (
    <>
      {/* Stories - Placeholder for now */}
      <div className="px-5 py-4 flex gap-4 overflow-x-auto border-b border-gray-100">
        <div className="flex flex-col items-center gap-1 min-w-[60px]">
          <div className="w-14 h-14 rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 p-0.5">
            <div className="w-full h-full rounded-full bg-white flex items-center justify-center">
              <div className="w-12 h-12 rounded-full bg-gray-300"></div>
            </div>
          </div>
          <span className="text-xs text-gray-700">Your Story</span>
        </div>
      </div>

      {/* Chat List */}
      <div className="flex-1 overflow-y-auto">
        {users.length === 0 ? (
          <div className="px-5 py-8 text-center text-gray-500">
            No conversations yet. Start chatting with someone!
          </div>
        ) : (
          users.map((user) => (
            <div
              key={user.id}
              onClick={() => setSelectedUser(user)}
              className={`px-5 py-3 cursor-pointer hover:bg-gray-50 transition flex items-center gap-3 ${
                selectedUser?.id === user.id ? 'bg-gray-50 border-l-4 border-cyan-500' : ''
              }`}
            >
              <div className="relative flex-shrink-0">
                <div className="w-12 h-12 rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 flex items-center justify-center text-white font-semibold">
                  {user.username.charAt(0).toUpperCase()}
                </div>
                {user.isOnline && (
                  <div className="absolute bottom-0 right-0 w-3 h-3 bg-green-500 border-2 border-white rounded-full"></div>
                )}
              </div>
              <div className="flex-1 min-w-0">
                <div className="flex items-center justify-between mb-1">
                  <h3 className="font-semibold text-gray-900 text-sm">{user.username}</h3>
                  <span className="text-xs text-gray-500">
                    {getLastSeenText(user.lastSeen, user.isOnline)}
                  </span>
                </div>
                <div className="flex items-center justify-between">
                  <p className="text-sm truncate text-gray-600">
                    {user.isOnline ? 'Online' : 'Offline'}
                  </p>
                </div>
              </div>
            </div>
          ))
        )}
      </div>
    </>
  );
}
