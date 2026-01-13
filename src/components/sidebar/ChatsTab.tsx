import { useState, useEffect } from 'react';
import { useChatStore } from '../../store/chatStore';
import CreateGroupModal from '../group/CreateGroupModal';

export default function ChatsTab() {
  const { users, groups, selectedUser, selectedGroup, setSelectedUser, setSelectedGroup, loadGroups } = useChatStore();
  const [showCreateGroup, setShowCreateGroup] = useState(false);

  useEffect(() => {
    loadGroups();
  }, [loadGroups]);

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
      {/* Create Group Button */}
      <div className="px-5 py-3 border-b border-gray-100">
        <button
          onClick={() => setShowCreateGroup(true)}
          className="w-full py-2 px-4 bg-cyan-500 text-white rounded-lg hover:bg-cyan-600 transition flex items-center justify-center gap-2"
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
          </svg>
          <span className="font-medium">Create Group</span>
        </button>
      </div>

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
        {/* Groups Section */}
        {groups.length > 0 && (
          <div className="mb-2">
            <div className="px-5 py-2 bg-gray-50">
              <h4 className="text-xs font-semibold text-gray-600 uppercase">Groups</h4>
            </div>
            {groups.map((group) => (
              <div
                key={`group-${group.id}`}
                onClick={() => setSelectedGroup(group)}
                className={`px-5 py-3 cursor-pointer hover:bg-gray-50 transition flex items-center gap-3 ${
                  selectedGroup?.id === group.id ? 'bg-gray-50 border-l-4 border-cyan-500' : ''
                }`}
              >
                <div className="relative flex-shrink-0">
                  <div className="w-12 h-12 rounded-full bg-gradient-to-br from-purple-400 to-pink-500 flex items-center justify-center text-white font-semibold">
                    {group.name.charAt(0).toUpperCase()}
                  </div>
                  <div className="absolute bottom-0 right-0 w-5 h-5 bg-gray-700 text-white text-xs rounded-full flex items-center justify-center border-2 border-white">
                    {group.memberCount}
                  </div>
                </div>
                <div className="flex-1 min-w-0">
                  <div className="flex items-center justify-between mb-1">
                    <h3 className="font-semibold text-gray-900 text-sm truncate">{group.name}</h3>
                  </div>
                  <p className="text-xs text-gray-500 truncate">
                    {group.memberCount} members
                  </p>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Direct Messages Section */}
        {users.length > 0 && (
          <div>
            <div className="px-5 py-2 bg-gray-50">
              <h4 className="text-xs font-semibold text-gray-600 uppercase">Direct Messages</h4>
            </div>
            {users.map((user) => (
              <div
                key={`user-${user.id}`}
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
            ))}
          </div>
        )}

        {users.length === 0 && groups.length === 0 && (
          <div className="px-5 py-8 text-center text-gray-500">
            No conversations yet. Start chatting with someone or create a group!
          </div>
        )}
      </div>

      {/* Create Group Modal */}
      <CreateGroupModal
        isOpen={showCreateGroup}
        onClose={() => setShowCreateGroup(false)}
        onSuccess={() => {
          loadGroups();
          console.log('Group created successfully!');
        }}
      />
    </>
  );
}
