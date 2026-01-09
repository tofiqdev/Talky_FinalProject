import { useState } from 'react';
import { useChatStore } from '../../store/chatStore';
import { usersApi } from '../../services/apiService';

export default function PeopleTab() {
  const { users, setSelectedUser } = useChatStore();
  const [searchTerm, setSearchTerm] = useState('');
  const [searchResults, setSearchResults] = useState<typeof users>([]);
  const [isSearching, setIsSearching] = useState(false);

  const handleSearch = async (term: string) => {
    setSearchTerm(term);
    
    if (term.trim().length < 2) {
      setSearchResults([]);
      setIsSearching(false);
      return;
    }

    setIsSearching(true);
    try {
      const results = await usersApi.searchUsers(term);
      setSearchResults(results);
    } catch (error) {
      console.error('Search failed:', error);
      setSearchResults([]);
    } finally {
      setIsSearching(false);
    }
  };

  const displayUsers = searchTerm.trim().length >= 2 ? searchResults : users;

  const getLastSeenText = (lastSeen: string | null | undefined, isOnline: boolean) => {
    if (isOnline) return 'Online';
    if (!lastSeen) return 'Offline';
    
    const date = new Date(lastSeen);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 60) return `Last seen ${diffMins}m ago`;
    if (diffHours < 24) return `Last seen ${diffHours}h ago`;
    if (diffDays === 1) return 'Last seen yesterday';
    if (diffDays < 7) return `Last seen ${diffDays} days ago`;
    return `Last seen ${Math.floor(diffDays / 7)} weeks ago`;
  };

  const handleStartChat = (user: typeof users[0]) => {
    setSelectedUser(user);
    setSearchTerm('');
    setSearchResults([]);
  };

  return (
    <div className="flex-1 overflow-y-auto">
      <div className="px-5 py-3 border-b border-gray-100">
        <input
          type="text"
          placeholder="Search by username..."
          value={searchTerm}
          onChange={(e) => handleSearch(e.target.value)}
          className="w-full px-4 py-2 bg-gray-100 rounded-full text-sm focus:outline-none focus:ring-2 focus:ring-cyan-500"
        />
      </div>
      {isSearching ? (
        <div className="px-5 py-8 text-center text-gray-500">
          Searching...
        </div>
      ) : displayUsers.length === 0 ? (
        <div className="px-5 py-8 text-center text-gray-500">
          {searchTerm.trim().length >= 2 ? 'No users found' : 'No contacts yet'}
        </div>
      ) : (
        displayUsers.map((user) => (
          <div
            key={user.id}
            className="px-5 py-3 cursor-pointer hover:bg-gray-50 transition flex items-center gap-3"
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
              <h3 className="font-semibold text-gray-900 text-sm">{user.username}</h3>
              <p className={`text-sm ${user.isOnline ? 'text-green-500' : 'text-gray-500'}`}>
                {getLastSeenText(user.lastSeen, user.isOnline)}
              </p>
            </div>
            <button 
              onClick={() => handleStartChat(user)}
              className="text-cyan-500 hover:text-cyan-600"
            >
              <svg className="w-6 h-6" fill="currentColor" viewBox="0 0 24 24">
                <path d="M20 2H4c-1.1 0-2 .9-2 2v18l4-4h14c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2z"/>
              </svg>
            </button>
          </div>
        ))
      )}
    </div>
  );
}
