import { useState, useEffect } from 'react';
import { useChatStore } from '../../store/chatStore';
import { useAuthStore } from '../../store/authStore';
import { usersApi } from '../../services/apiService';
import CreateGroupModal from '../group/CreateGroupModal';
import CreateStoryModal from '../story/CreateStoryModal';
import ViewStoryModal from '../story/ViewStoryModal';
import type { Story } from '../../types/story';
import type { User } from '../../types/user';

export default function ChatsTab() {
  const { users, groups, messages, selectedUser, selectedGroup, setSelectedUser, setSelectedGroup, loadGroups } = useChatStore();
  const { user: currentUser } = useAuthStore();
  const [showCreateGroup, setShowCreateGroup] = useState(false);
  const [showCreateStory, setShowCreateStory] = useState(false);
  const [showViewStory, setShowViewStory] = useState(false);
  const [stories, setStories] = useState<Story[]>([]);
  const [selectedStoryIndex, setSelectedStoryIndex] = useState(0);
  const [isLoadingStories, setIsLoadingStories] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<User[]>([]);
  const [isSearching, setIsSearching] = useState(false);

  useEffect(() => {
    loadGroups();
    loadStories();
  }, [loadGroups]);

  const loadStories = async () => {
    setIsLoadingStories(true);
    try {
      const token = localStorage.getItem('token');
      const response = await fetch('/api/stories', {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      
      if (response.ok) {
        const data = await response.json();
        setStories(data);
      }
    } catch (error) {
      console.error('Failed to load stories:', error);
    } finally {
      setIsLoadingStories(false);
    }
  };

  const handleStoryClick = (userId: number) => {
    // Find all stories for this user
    const userStories = stories.filter(s => s.userId === userId);
    if (userStories.length === 0) return;
    
    // Find the index of the first story of this user in the full stories array
    const firstStoryIndex = stories.findIndex(s => s.userId === userId);
    setSelectedStoryIndex(firstStoryIndex);
    setShowViewStory(true);
  };

  const handleStorySuccess = () => {
    loadStories();
  };

  // Search functionality
  const handleSearch = async (query: string) => {
    setSearchQuery(query);
    
    if (query.trim().length < 2) {
      setSearchResults([]);
      setIsSearching(false);
      return;
    }

    setIsSearching(true);
    try {
      const results = await usersApi.searchUsers(query.trim());
      setSearchResults(results);
    } catch (error) {
      console.error('Search failed:', error);
      setSearchResults([]);
    } finally {
      setIsSearching(false);
    }
  };

  const handleSelectSearchResult = (user: User) => {
    // Add user to users list if not already there
    const userExists = users.some(u => u.id === user.id);
    if (!userExists) {
      useChatStore.setState((state) => ({
        users: [...state.users, user]
      }));
    }
    
    setSelectedUser(user);
    setSearchQuery('');
    setSearchResults([]);
  };

  // Group stories by user - only show one avatar per user
  const groupedStories = stories.reduce((acc, story) => {
    const existing = acc.find(item => item.userId === story.userId);
    if (!existing) {
      acc.push({
        userId: story.userId,
        username: story.username,
        storyCount: 1,
        hasUnviewed: !story.hasViewed,
        firstStory: story
      });
    } else {
      existing.storyCount++;
      if (!story.hasViewed) {
        existing.hasUnviewed = true;
      }
    }
    return acc;
  }, [] as Array<{
    userId: number;
    username: string;
    storyCount: number;
    hasUnviewed: boolean;
    firstStory: Story;
  }>);

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

  // Filter users based on messages - only show users we've chatted with
  const usersWithMessages = users.filter(user => {
    return messages.some(msg => 
      msg.senderId === user.id || msg.receiverId === user.id
    );
  });

  // Filter users and groups based on search query
  const filteredUsers = searchQuery.trim().length === 0 
    ? usersWithMessages 
    : usersWithMessages.filter(user => 
        user.username.toLowerCase().includes(searchQuery.toLowerCase())
      );

  const filteredGroups = searchQuery.trim().length === 0
    ? groups
    : groups.filter(group =>
        group.name.toLowerCase().includes(searchQuery.toLowerCase())
      );

  return (
    <>
      {/* Search Bar */}
      <div className="px-5 py-3 border-b border-gray-100">
        <div className="relative">
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => handleSearch(e.target.value)}
            placeholder="Search users or groups..."
            className="w-full py-2 pl-10 pr-4 bg-gray-100 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-500 transition"
          />
          <svg 
            className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" 
            fill="none" 
            stroke="currentColor" 
            viewBox="0 0 24 24"
          >
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          {isSearching && (
            <div className="absolute right-3 top-1/2 -translate-y-1/2">
              <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-cyan-500"></div>
            </div>
          )}
        </div>
      </div>

      {/* Search Results (when searching for new users) */}
      {searchQuery.trim().length >= 2 && searchResults.length > 0 && (
        <div className="border-b border-gray-100">
          <div className="px-5 py-2 bg-cyan-50">
            <h4 className="text-xs font-semibold text-cyan-700 uppercase">Search Results</h4>
          </div>
          {searchResults.map((user) => (
            <div
              key={`search-${user.id}`}
              onClick={() => handleSelectSearchResult(user)}
              className="px-5 py-3 cursor-pointer hover:bg-gray-50 transition flex items-center gap-3"
            >
              <div className="relative flex-shrink-0">
                {user.avatar ? (
                  <img 
                    src={user.avatar} 
                    alt={user.username}
                    className="w-12 h-12 rounded-full object-cover"
                  />
                ) : (
                  <div className="w-12 h-12 rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 flex items-center justify-center text-white font-semibold">
                    {user.username.charAt(0).toUpperCase()}
                  </div>
                )}
                {user.isOnline && (
                  <div className="absolute bottom-0 right-0 w-3 h-3 bg-green-500 border-2 border-white rounded-full"></div>
                )}
              </div>
              <div className="flex-1 min-w-0">
                <h3 className="font-semibold text-gray-900 text-sm">{user.username}</h3>
                <p className="text-xs text-gray-500 truncate">{user.email}</p>
              </div>
            </div>
          ))}
        </div>
      )}

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

      {/* Stories */}
      <div className="px-5 py-4 flex gap-4 overflow-x-auto border-b border-gray-100">
        {/* Add Story Button */}
        <div 
          onClick={() => setShowCreateStory(true)}
          className="flex flex-col items-center gap-1 min-w-[60px] cursor-pointer group"
        >
          <div className="w-14 h-14 rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 p-0.5 group-hover:scale-105 transition">
            <div className="w-full h-full rounded-full bg-white p-0.5 flex items-center justify-center">
              {currentUser?.avatar ? (
                <div className="relative w-full h-full">
                  <img 
                    src={currentUser.avatar} 
                    alt={currentUser.username}
                    className="w-full h-full rounded-full object-cover"
                  />
                  <div className="absolute inset-0 flex items-center justify-center bg-black/30 rounded-full">
                    <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                    </svg>
                  </div>
                </div>
              ) : (
                <div className="w-full h-full rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 flex items-center justify-center">
                  <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                  </svg>
                </div>
              )}
            </div>
          </div>
          <span className="text-xs text-gray-700 font-medium">Add Story</span>
        </div>

        {/* User Stories - Grouped by user */}
        {isLoadingStories ? (
          <div className="flex items-center justify-center py-4">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-cyan-500"></div>
          </div>
        ) : (
          groupedStories.map((userStory) => (
            <div
              key={userStory.userId}
              onClick={() => handleStoryClick(userStory.userId)}
              className="flex flex-col items-center gap-1 min-w-[60px] cursor-pointer group relative"
            >
              <div className={`w-14 h-14 rounded-full p-0.5 group-hover:scale-105 transition ${
                userStory.hasUnviewed 
                  ? 'bg-gradient-to-br from-purple-400 via-pink-500 to-red-500'
                  : 'bg-gray-300'
              }`}>
                <div className="w-full h-full rounded-full bg-white p-0.5">
                  {userStory.firstStory.avatar ? (
                    <img 
                      src={userStory.firstStory.avatar} 
                      alt={userStory.username}
                      className="w-full h-full rounded-full object-cover"
                    />
                  ) : (
                    <div className="w-full h-full rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 flex items-center justify-center text-white font-semibold text-sm">
                      {userStory.username.charAt(0).toUpperCase()}
                    </div>
                  )}
                </div>
              </div>
              {/* Story count badge */}
              {userStory.storyCount > 1 && (
                <div className="absolute top-0 right-0 w-5 h-5 bg-cyan-500 text-white text-xs rounded-full flex items-center justify-center border-2 border-white font-semibold">
                  {userStory.storyCount}
                </div>
              )}
              <span className="text-xs text-gray-700 truncate max-w-[60px]">{userStory.username}</span>
            </div>
          ))
        )}
      </div>

      {/* Chat List */}
      <div className="flex-1 overflow-y-auto">
        {/* Groups Section */}
        {filteredGroups.length > 0 && (
          <div className="mb-2">
            <div className="px-5 py-2 bg-gray-50">
              <h4 className="text-xs font-semibold text-gray-600 uppercase">Groups</h4>
            </div>
            {filteredGroups.map((group) => (
              <div
                key={`group-${group.id}`}
                onClick={() => setSelectedGroup(group)}
                className={`px-5 py-3 cursor-pointer hover:bg-gray-50 transition flex items-center gap-3 ${
                  selectedGroup?.id === group.id ? 'bg-gray-50 border-l-4 border-cyan-500' : ''
                }`}
              >
                <div className="relative flex-shrink-0">
                  {group.avatar ? (
                    <img 
                      src={group.avatar} 
                      alt={group.name}
                      className="w-12 h-12 rounded-full object-cover"
                    />
                  ) : (
                    <div className="w-12 h-12 rounded-full bg-gradient-to-br from-purple-400 to-pink-500 flex items-center justify-center text-white font-semibold">
                      {group.name.charAt(0).toUpperCase()}
                    </div>
                  )}
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
        {filteredUsers.length > 0 && (
          <div>
            <div className="px-5 py-2 bg-gray-50">
              <h4 className="text-xs font-semibold text-gray-600 uppercase">Direct Messages</h4>
            </div>
            {filteredUsers.map((user) => (
              <div
                key={`user-${user.id}`}
                onClick={() => setSelectedUser(user)}
                className={`px-5 py-3 cursor-pointer hover:bg-gray-50 transition flex items-center gap-3 ${
                  selectedUser?.id === user.id ? 'bg-gray-50 border-l-4 border-cyan-500' : ''
                }`}
              >
                <div className="relative flex-shrink-0">
                  {user.avatar ? (
                    <img 
                      src={user.avatar} 
                      alt={user.username}
                      className="w-12 h-12 rounded-full object-cover"
                    />
                  ) : (
                    <div className="w-12 h-12 rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 flex items-center justify-center text-white font-semibold">
                      {user.username.charAt(0).toUpperCase()}
                    </div>
                  )}
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

        {/* No Results Message */}
        {searchQuery.trim().length > 0 && filteredUsers.length === 0 && filteredGroups.length === 0 && searchResults.length === 0 && !isSearching && (
          <div className="px-5 py-8 text-center text-gray-500">
            <svg className="w-12 h-12 mx-auto mb-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
            <p>No results found for "{searchQuery}"</p>
            <p className="text-sm mt-1">Try searching with a different keyword</p>
          </div>
        )}

        {/* Empty State */}
        {searchQuery.trim().length === 0 && users.length === 0 && groups.length === 0 && (
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

      {/* Create Story Modal */}
      <CreateStoryModal
        isOpen={showCreateStory}
        onClose={() => setShowCreateStory(false)}
        onSuccess={handleStorySuccess}
      />

      {/* View Story Modal */}
      <ViewStoryModal
        isOpen={showViewStory}
        stories={stories}
        initialIndex={selectedStoryIndex}
        onClose={() => setShowViewStory(false)}
      />
    </>
  );
}
