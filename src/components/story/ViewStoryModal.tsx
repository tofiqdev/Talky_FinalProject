import { useState, useEffect } from 'react';
import type { Story, StoryView } from '../../types/story';
import { API_BASE_URL } from '../../services/apiService';

interface ViewStoryModalProps {
  isOpen: boolean;
  stories: Story[];
  initialIndex: number;
  onClose: () => void;
}

export default function ViewStoryModal({ isOpen, stories, initialIndex, onClose }: ViewStoryModalProps) {
  const [currentIndex, setCurrentIndex] = useState(initialIndex);
  const [progress, setProgress] = useState(0);
  const [showViews, setShowViews] = useState(false);
  const [views, setViews] = useState<StoryView[]>([]);
  const [isLoadingViews, setIsLoadingViews] = useState(false);

  const currentStory = stories[currentIndex];
  const STORY_DURATION = 5000; // 5 seconds
  
  // Get all stories from the current user
  const currentUserStories = stories.filter(s => s.userId === currentStory?.userId);
  const currentUserStoryIndex = currentUserStories.findIndex(s => s.id === currentStory?.id);
  const hasPrevious = currentUserStoryIndex > 0;
  const hasNext = currentUserStoryIndex < currentUserStories.length - 1;

  useEffect(() => {
    if (!isOpen || !currentStory) return;

    // Mark story as viewed
    markAsViewed(currentStory.id);

    // Progress animation
    setProgress(0);
    const interval = setInterval(() => {
      setProgress((prev) => {
        if (prev >= 100) {
          handleNext();
          return 0;
        }
        return prev + (100 / (STORY_DURATION / 100));
      });
    }, 100);

    return () => clearInterval(interval);
  }, [currentIndex, isOpen]);

  const markAsViewed = async (storyId: number) => {
    try {
      const token = localStorage.getItem('token');
      await fetch(`${API_BASE_URL}/stories/${storyId}/view`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
    } catch (error) {
      console.error('Failed to mark story as viewed:', error);
    }
  };

  const loadViews = async (storyId: number) => {
    setIsLoadingViews(true);
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`${API_BASE_URL}/stories/${storyId}/views`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      
      if (response.ok) {
        const data = await response.json();
        setViews(data);
      }
    } catch (error) {
      console.error('Failed to load views:', error);
    } finally {
      setIsLoadingViews(false);
    }
  };

  const handleNext = () => {
    // Check if there are more stories from the same user
    const currentUserId = currentStory.userId;
    const nextIndex = currentIndex + 1;
    
    if (nextIndex < stories.length && stories[nextIndex].userId === currentUserId) {
      // Next story is from the same user
      setCurrentIndex(nextIndex);
      setProgress(0);
    } else {
      // No more stories from this user, close modal
      onClose();
    }
  };

  const handlePrevious = () => {
    // Check if there are previous stories from the same user
    const currentUserId = currentStory.userId;
    const prevIndex = currentIndex - 1;
    
    if (prevIndex >= 0 && stories[prevIndex].userId === currentUserId) {
      // Previous story is from the same user
      setCurrentIndex(prevIndex);
      setProgress(0);
    }
  };

  const handleViewsClick = () => {
    if (!showViews) {
      loadViews(currentStory.id);
    }
    setShowViews(!showViews);
  };

  const getTimeAgo = (date: string) => {
    const now = new Date();
    const storyDate = new Date(date);
    const diffMs = now.getTime() - storyDate.getTime();
    const diffHours = Math.floor(diffMs / 3600000);
    
    if (diffHours < 1) return 'Just now';
    if (diffHours === 1) return '1h ago';
    return `${diffHours}h ago`;
  };

  if (!isOpen || !currentStory) return null;

  return (
    <div className="fixed inset-0 bg-black z-50 flex items-center justify-center">
      {/* Progress bars - only for current user's stories */}
      <div className="absolute top-0 left-0 right-0 flex gap-1 p-2 z-10">
        {currentUserStories.map((story, index) => (
          <div key={story.id} className="flex-1 h-1 bg-gray-600 rounded-full overflow-hidden">
            <div
              className="h-full bg-white transition-all duration-100"
              style={{
                width: index < currentUserStoryIndex ? '100%' : index === currentUserStoryIndex ? `${progress}%` : '0%'
              }}
            />
          </div>
        ))}
      </div>

      {/* Header */}
      <div className="absolute top-4 left-0 right-0 px-4 flex items-center justify-between z-10">
        <div className="flex items-center gap-3">
          {currentStory.avatar ? (
            <img 
              src={currentStory.avatar} 
              alt={currentStory.username}
              className="w-10 h-10 rounded-full object-cover border-2 border-white"
            />
          ) : (
            <div className="w-10 h-10 rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 flex items-center justify-center text-white font-semibold border-2 border-white">
              {currentStory.username.charAt(0).toUpperCase()}
            </div>
          )}
          <div>
            <p className="text-white font-semibold">{currentStory.username}</p>
            <p className="text-white text-xs opacity-80">{getTimeAgo(currentStory.createdAt)}</p>
          </div>
        </div>
        {/* Only show close button when views panel is closed */}
        {!showViews && (
          <button
            onClick={onClose}
            className="text-white hover:text-gray-300 transition"
          >
            <svg className="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        )}
      </div>

      {/* Story Image */}
      <img
        src={currentStory.imageUrl}
        alt="Story"
        className="max-w-full max-h-full object-contain"
      />

      {/* Caption */}
      {currentStory.caption && (
        <div className="absolute bottom-20 left-0 right-0 px-6">
          <p className="text-white text-center text-lg">{currentStory.caption}</p>
        </div>
      )}

      {/* View count (only for own stories) */}
      <div className="absolute bottom-6 left-0 right-0 px-6 flex justify-center">
        <button
          onClick={handleViewsClick}
          className="flex items-center gap-2 bg-black bg-opacity-50 px-4 py-2 rounded-full text-white hover:bg-opacity-70 transition"
        >
          <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
          </svg>
          <span>{currentStory.viewCount} views</span>
        </button>
      </div>

      {/* Navigation */}
      <button
        onClick={handlePrevious}
        className="absolute left-4 top-1/2 -translate-y-1/2 text-white hover:text-gray-300 transition disabled:opacity-30 disabled:cursor-not-allowed"
        disabled={!hasPrevious}
      >
        <svg className="w-10 h-10" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
        </svg>
      </button>
      <button
        onClick={handleNext}
        className="absolute right-4 top-1/2 -translate-y-1/2 text-white hover:text-gray-300 transition disabled:opacity-30 disabled:cursor-not-allowed"
        disabled={!hasNext}
      >
        <svg className="w-10 h-10" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
        </svg>
      </button>

      {/* Views Panel */}
      {showViews && (
        <div className="absolute right-0 top-0 bottom-0 w-80 bg-white shadow-xl overflow-y-auto z-20">
          <div className="p-4 border-b border-gray-200 flex items-center justify-between">
            <h3 className="font-semibold text-gray-900">Views</h3>
            <button
              onClick={() => setShowViews(false)}
              className="text-gray-400 hover:text-gray-600 transition"
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>
          
          {isLoadingViews ? (
            <div className="p-8 text-center text-gray-500">Loading...</div>
          ) : views.length === 0 ? (
            <div className="p-8 text-center text-gray-500">No views yet</div>
          ) : (
            <div className="divide-y divide-gray-100">
              {views.map((view) => (
                <div key={view.id} className="p-4 flex items-center gap-3">
                  {view.avatar ? (
                    <img 
                      src={view.avatar} 
                      alt={view.username}
                      className="w-10 h-10 rounded-full object-cover"
                    />
                  ) : (
                    <div className="w-10 h-10 rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 flex items-center justify-center text-white font-semibold">
                      {view.username.charAt(0).toUpperCase()}
                    </div>
                  )}
                  <div className="flex-1">
                    <p className="font-medium text-gray-900">{view.username}</p>
                    <p className="text-xs text-gray-500">{getTimeAgo(view.viewedAt)}</p>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  );
}
