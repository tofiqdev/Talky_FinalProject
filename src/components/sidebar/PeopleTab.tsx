import { useState, useEffect } from 'react';
import { useChatStore } from '../../store/chatStore';
import { usersApi } from '../../services/apiService';

interface BlockedUser {
  id: number;
  userId: number;
  username: string;
  email: string;
  avatar?: string;
  blockedAt: string;
}

export default function PeopleTab() {
  const { users, setSelectedUser } = useChatStore();
  const [searchTerm, setSearchTerm] = useState('');
  const [searchResults, setSearchResults] = useState<typeof users>([]);
  const [isSearching, setIsSearching] = useState(false);
  const [showBlocked, setShowBlocked] = useState(false);
  const [blockedUsers, setBlockedUsers] = useState<BlockedUser[]>([]);
  const [isLoadingBlocked, setIsLoadingBlocked] = useState(false);

  useEffect(() => {
    if (showBlocked) {
      loadBlockedUsers();
    }
  }, [showBlocked]);

  const loadBlockedUsers = async () => {
    setIsLoadingBlocked(true);
    try {
      const token = localStorage.getItem('token');
      const response = await fetch('/api/blockedusers', {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      
      if (response.ok) {
        const data = await response.json();
        setBlockedUsers(data);
      }
    } catch (error) {
      console.error('Failed to load blocked users:', error);
    } finally {
      setIsLoadingBlocked(false);
    }
  };

  const handleBlockUser = async (userId: number, username: string) => {
    if (!confirm(`Are you sure you want to block ${username}?`)) return;
    
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/blockedusers/${userId}`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      
      if (response.ok) {
        alert(`${username} has been blocked`);
        // Reload users list
        window.location.reload();
      }
    } catch (error) {
      console.error('Failed to block user:', error);
      alert('Failed to block user');
    }
  };

  const handleUnblockUser = async (userId: number, username: string) => {
    if (!confirm(`Are you sure you want to unblock ${username}?`)) return;
    
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/blockedusers/${userId}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      
      if (response.ok) {
        alert(`${username} has been unblocked`);
        loadBlockedUsers();
      }
    } catch (error) {
      console.error('Failed to unblock user:', error);
      alert('Failed to unblock user');
    }
  };

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
    <div className="flex-1 overflow-y-auto flex flex-col">
      {/* Tab Switcher */}
      <div className="flex border-b border-gray-200">
        <button
          onClick={() => setShowBlocked(false)}
          className={`flex-1 py-3 text-sm font-medium transition ${
            !showBlocked 
              ? 'text-cyan-500 border-b-2 border-cyan-500' 
              : 'text-gray-500 hover:text-gray-700'
          }`}
        >
          Contacts
        </button>
        <button
          onClick={() => setShowBlocked(true)}
          className={`flex-1 py-3 text-sm font-medium transition ${
            showBlocked 
              ? 'text-red-500 border-b-2 border-red-500' 
              : 'text-gray-500 hover:text-gray-700'
          }`}
        >
          Blocked
        </button>
      </div>

      {!showBlocked ? (
        // Contacts View
        <>
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
                className="px-5 py-3 hover:bg-gray-50 transition flex items-center gap-3"
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
                <div className="flex gap-2">
                  <button 
                    onClick={() => handleStartChat(user)}
                    className="text-cyan-500 hover:text-cyan-600 transition"
                    title="Start Chat"
                  >
                    <svg className="w-6 h-6" fill="currentColor" viewBox="0 0 24 24">
                      <path d="M20 2H4c-1.1 0-2 .9-2 2v18l4-4h14c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2z"/>
                    </svg>
                  </button>
                  <button 
                    onClick={() => handleBlockUser(user.id, user.username)}
                    className="text-red-500 hover:text-red-600 transition"
                    title="Block User"
                  >
                    <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M18.364 18.364A9 9 0 005.636 5.636m12.728 12.728A9 9 0 015.636 5.636m12.728 12.728L5.636 5.636" />
                    </svg>
                  </button>
                </div>
              </div>
            ))
          )}
        </>
      ) : (
        // Blocked Users View
        <>
          {isLoadingBlocked ? (
            <div className="px-5 py-8 text-center text-gray-500">
              Loading blocked users...
            </div>
          ) : blockedUsers.length === 0 ? (
            <div className="px-5 py-8 text-center text-gray-500">
              No blocked users
            </div>
          ) : (
            blockedUsers.map((user) => (
              <div
                key={user.id}
                className="px-5 py-3 hover:bg-gray-50 transition flex items-center gap-3"
              >
                <div className="relative flex-shrink-0">
                  <div className="w-12 h-12 rounded-full bg-gradient-to-br from-gray-400 to-gray-600 flex items-center justify-center text-white font-semibold">
                    {user.username.charAt(0).toUpperCase()}
                  </div>
                </div>
                <div className="flex-1 min-w-0">
                  <h3 className="font-semibold text-gray-900 text-sm">{user.username}</h3>
                  <p className="text-sm text-red-500">Blocked</p>
                </div>
                <button 
                  onClick={() => handleUnblockUser(user.userId, user.username)}
                  className="px-3 py-1 text-sm bg-green-100 text-green-700 rounded hover:bg-green-200 transition"
                >
                  Unblock
                </button>
              </div>
            ))
          )}
        </>
      )}
    </div>
  );
}
