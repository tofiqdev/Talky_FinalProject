import { useState, useEffect, useRef } from 'react';
import { useMovieRoomStore } from '../../store/movieRoomStore';
import { useAuthStore } from '../../store/authStore';
import { movieRoomSignalrService } from '../../services/movieRoomSignalrService';
import YouTube, { type YouTubeProps } from 'react-youtube';
import { renderCEOBadgeUniversal } from '../../utils/userUtils';
import { useChatStore } from '../../store/chatStore';

export default function MovieRoomWindow() {
  const { selectedRoom, messages, loadMessages, sendMessage, joinRoom, leaveRoom } = useMovieRoomStore();
  const { user } = useAuthStore();
  const { users } = useChatStore();
  const [messageInput, setMessageInput] = useState('');
  const [isJoined, setIsJoined] = useState(false);
  const [isSyncing, setIsSyncing] = useState(false);
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const playerRef = useRef<any>(null);
  const isOwner = selectedRoom?.createdById === user?.id;

  // Get user email from users list
  const getUserEmail = (userId: number): string => {
    const user = users.find(u => u.id === userId);
    return user?.email || '';
  };

  useEffect(() => {
    if (selectedRoom) {
      loadMessages(selectedRoom.id);
      // Check if user is already a participant
      const isParticipant = selectedRoom.participants.some(p => p.userId === user?.id);
      setIsJoined(isParticipant);

      // Connect to SignalR and join room
      const token = localStorage.getItem('token');
      if (token && isParticipant) {
        movieRoomSignalrService.connect(token).then(() => {
          movieRoomSignalrService.joinRoom(selectedRoom.id);

          // Listen for playback sync (for ALL users including owner to see sync status)
          movieRoomSignalrService.onPlaybackSync((data) => {
            console.log('📡 Playback sync received:', { 
              data, 
              isOwner, 
              currentUserId: user?.id,
              roomOwnerId: selectedRoom.createdById 
            });
            
            // Don't sync if this is the owner (they control the video)
            if (isOwner) {
              console.log('⏭️ Skipping sync - user is owner');
              return;
            }
            
            setIsSyncing(true);
            
            if (playerRef.current) {
              const currentPlayerTime = playerRef.current.getCurrentTime();
              const timeDiff = Math.abs(currentPlayerTime - data.currentTime);
              
              console.log('🔄 Syncing video:', { 
                currentTime: currentPlayerTime, 
                targetTime: data.currentTime, 
                diff: timeDiff,
                shouldPlay: data.isPlaying
              });
              
              // Always sync time
              if (timeDiff > 1) {
                console.log('⏩ Seeking to:', data.currentTime);
                playerRef.current.seekTo(data.currentTime, true);
              }
              
              // Sync play/pause state
              if (data.isPlaying) {
                console.log('▶️ Playing video');
                playerRef.current.playVideo();
              } else {
                console.log('⏸️ Pausing video');
                playerRef.current.pauseVideo();
              }
            } else {
              console.warn('⚠️ Player ref not available');
            }
            
            setTimeout(() => setIsSyncing(false), 1000);
          });

          // Listen for new messages
          movieRoomSignalrService.onReceiveRoomMessage(() => {
            loadMessages(selectedRoom.id);
          });
        }).catch(err => {
          console.error('Failed to connect to MovieRoom SignalR:', err);
        });
      }
    }

    return () => {
      if (selectedRoom) {
        movieRoomSignalrService.leaveRoom(selectedRoom.id);
        movieRoomSignalrService.removeAllListeners();
      }
    };
  }, [selectedRoom, loadMessages, user, isOwner]);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  const handleJoinRoom = async () => {
    if (!selectedRoom) return;
    try {
      await joinRoom(selectedRoom.id);
      setIsJoined(true);
      // Reload messages after joining
      await loadMessages(selectedRoom.id);
    } catch (error) {
      console.error('Failed to join room:', error);
    }
  };

  const handleLeaveRoom = async () => {
    if (!selectedRoom) return;
    try {
      await leaveRoom(selectedRoom.id);
      setIsJoined(false);
    } catch (error) {
      console.error('Failed to leave room:', error);
    }
  };

  const handleSendMessage = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedRoom || !messageInput.trim()) return;

    try {
      await sendMessage(selectedRoom.id, messageInput);
      setMessageInput('');
    } catch (error) {
      console.error('Failed to send message:', error);
    }
  };

  const onPlayerReady: YouTubeProps['onReady'] = (event) => {
    playerRef.current = event.target;
    if (selectedRoom) {
      event.target.seekTo(selectedRoom.currentTime, true);
      if (selectedRoom.isPlaying) {
        event.target.playVideo();
      }
    }
  };

  const onPlayerStateChange: YouTubeProps['onStateChange'] = async (event) => {
    // Only owner can trigger sync
    if (!selectedRoom || !isOwner || isSyncing) {
      console.log('State change ignored:', { hasRoom: !!selectedRoom, isOwner, isSyncing });
      return;
    }
    
    const state = event.data;
    // -1: unstarted, 0: ended, 1: playing, 2: paused, 3: buffering, 5: video cued
    const isPlaying = state === 1;
    const isPaused = state === 2;
    const isEnded = state === 0;
    
    if (isPlaying || isPaused || isEnded) {
      const currentTime = event.target.getCurrentTime();
      
      console.log('🎬 Owner triggering sync:', { 
        roomId: selectedRoom.id,
        state, 
        currentTime, 
        isPlaying: isPlaying && !isEnded,
        stateNames: { 1: 'playing', 2: 'paused', 0: 'ended' }[state]
      });
      
      try {
        // Sync with backend and broadcast to all participants
        await movieRoomSignalrService.syncPlayback(selectedRoom.id, currentTime, isPlaying && !isEnded);
        console.log('✅ Sync sent successfully');
      } catch (error) {
        console.error('❌ Failed to send sync:', error);
      }
    }
  };

  const opts: YouTubeProps['opts'] = {
    height: '100%',
    width: '100%',
    playerVars: {
      autoplay: 0,
      controls: isOwner ? 1 : 0, // Only owner can control
      modestbranding: 1,
      rel: 0,
      disablekb: !isOwner ? 1 : 0, // Disable keyboard for non-owners
    },
  };

  if (!selectedRoom) {
    return (
      <div className="flex-1 flex items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="text-6xl mb-4">🎬</div>
          <p className="text-gray-500">Bir film odası seçin</p>
        </div>
      </div>
    );
  }

  if (!isJoined) {
    return (
      <div className="flex-1 flex items-center justify-center bg-gray-50">
        <div className="text-center max-w-md p-8">
          <div className="text-6xl mb-4">🎬</div>
          <h2 className="text-2xl font-bold text-gray-900 mb-2">{selectedRoom.name}</h2>
          {selectedRoom.description && (
            <p className="text-gray-600 mb-6">{selectedRoom.description}</p>
          )}
          <div className="flex items-center justify-center gap-4 text-sm text-gray-500 mb-6">
            <span>👤 {selectedRoom.participantCount} kişi</span>
            <span>👑 {selectedRoom.createdByUsername}</span>
          </div>
          <button
            onClick={handleJoinRoom}
            className="px-8 py-3 bg-gradient-to-r from-purple-500 to-pink-500 text-white rounded-lg hover:from-purple-600 hover:to-pink-600 transition-all text-lg font-semibold"
          >
            Odaya Katıl
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="flex-1 flex flex-col bg-gray-50">
      {/* Header */}
      <div className="bg-white border-b border-gray-200 p-4">
        <div className="flex items-center justify-between">
          <div>
            <div className="flex items-center gap-3">
              <h2 className="text-xl font-bold text-gray-900">{selectedRoom.name}</h2>
              {isOwner && (
                <span className="px-3 py-1 bg-gradient-to-r from-purple-500 to-pink-500 text-white text-xs font-semibold rounded-full">
                  👑 Oda Sahibi
                </span>
              )}
              {isSyncing && !isOwner && (
                <span className="px-3 py-1 bg-blue-100 text-blue-600 text-xs font-semibold rounded-full animate-pulse">
                  🔄 Senkronize ediliyor...
                </span>
              )}
            </div>
            <p className="text-sm text-gray-600 mt-1">
              {selectedRoom.participantCount} kişi izliyor
              {!isOwner && ' • Sadece oda sahibi videoyu kontrol edebilir'}
            </p>
          </div>
          <button
            onClick={handleLeaveRoom}
            className="px-4 py-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
          >
            Odadan Ayrıl
          </button>
        </div>
      </div>

      {/* Main Content */}
      <div className="flex-1 flex overflow-hidden">
        {/* Video Player */}
        <div className="flex-1 bg-black flex items-center justify-center relative">
          <div className="w-full h-full">
            <YouTube
              videoId={selectedRoom.youTubeVideoId}
              opts={opts}
              onReady={onPlayerReady}
              onStateChange={onPlayerStateChange}
              className="w-full h-full"
            />
          </div>
          
          {/* Overlay to prevent non-owners from clicking */}
          {!isOwner && (
            <div className="absolute inset-0 cursor-not-allowed" style={{ pointerEvents: 'all' }}>
              <div className="absolute top-4 left-4 bg-black bg-opacity-70 text-white px-4 py-2 rounded-lg text-sm">
                🔒 Sadece oda sahibi videoyu kontrol edebilir
              </div>
            </div>
          )}
        </div>

        {/* Chat Sidebar */}
        <div className="w-96 bg-white border-l border-gray-200 flex flex-col">
          {/* Participants */}
          <div className="p-4 border-b border-gray-200">
            <h3 className="font-semibold text-gray-900 mb-3">Katılımcılar ({selectedRoom.participants.length})</h3>
            <div className="space-y-2 max-h-32 overflow-y-auto">
              {selectedRoom.participants.map((participant) => (
                <div key={participant.id} className="flex items-center gap-2">
                  <div className="relative">
                    {participant.avatar ? (
                      <img
                        src={participant.avatar}
                        alt={participant.username}
                        className="w-8 h-8 rounded-full object-cover"
                      />
                    ) : (
                      <div className="w-8 h-8 rounded-full bg-gradient-to-br from-purple-400 to-pink-400 flex items-center justify-center text-white text-sm font-semibold">
                        {participant.username[0].toUpperCase()}
                      </div>
                    )}
                    {participant.isOnline && (
                      <div className="absolute bottom-0 right-0 w-2.5 h-2.5 bg-green-500 rounded-full border-2 border-white"></div>
                    )}
                  </div>
                  <span className="text-sm text-gray-700 flex items-center">
                    {participant.username}
                    {renderCEOBadgeUniversal(getUserEmail(participant.userId), participant.username, 'text-xs px-1 py-0.5')}
                  </span>
                </div>
              ))}
            </div>
          </div>

          {/* Messages */}
          <div className="flex-1 overflow-y-auto p-4 space-y-3">
            {messages.length === 0 ? (
              <div className="flex items-center justify-center h-full text-gray-400">
                <div className="text-center">
                  <div className="text-4xl mb-2">💬</div>
                  <p className="text-sm">Henüz mesaj yok</p>
                  <p className="text-xs">İlk mesajı sen gönder!</p>
                </div>
              </div>
            ) : (
              messages.map((message) => (
                <div key={message.id} className="flex gap-2 animate-fadeIn">
                  <div className="flex-shrink-0">
                    {message.senderAvatar ? (
                      <img
                        src={message.senderAvatar}
                        alt={message.senderUsername}
                        className="w-8 h-8 rounded-full object-cover"
                      />
                    ) : (
                      <div className="w-8 h-8 rounded-full bg-gradient-to-br from-purple-400 to-pink-400 flex items-center justify-center text-white text-xs font-semibold">
                        {message.senderUsername[0]?.toUpperCase() || '?'}
                      </div>
                    )}
                  </div>
                  <div className="flex-1 min-w-0">
                    <div className="flex items-baseline gap-2 mb-1">
                      <span className="font-semibold text-sm text-gray-900 flex items-center">
                        {message.senderUsername}
                        {renderCEOBadgeUniversal(getUserEmail(message.senderId), message.senderUsername, 'text-xs px-1 py-0.5')}
                      </span>
                      <span className="text-xs text-gray-500">
                        {new Date(message.sentAt).toLocaleTimeString('tr-TR', {
                          hour: '2-digit',
                          minute: '2-digit'
                        })}
                      </span>
                    </div>
                    <p className="text-sm text-gray-700 break-words">{message.content}</p>
                  </div>
                </div>
              ))
            )}
            <div ref={messagesEndRef} />
          </div>

          {/* Message Input */}
          <form onSubmit={handleSendMessage} className="p-4 border-t border-gray-200">
            <div className="flex gap-2">
              <input
                type="text"
                value={messageInput}
                onChange={(e) => setMessageInput(e.target.value)}
                placeholder="Mesaj yaz..."
                className="flex-1 px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent"
              />
              <button
                type="submit"
                disabled={!messageInput.trim()}
                className="px-4 py-2 bg-gradient-to-r from-purple-500 to-pink-500 text-white rounded-lg hover:from-purple-600 hover:to-pink-600 transition-all disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Gönder
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
