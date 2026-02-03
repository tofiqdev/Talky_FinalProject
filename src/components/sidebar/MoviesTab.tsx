import { useState, useEffect } from 'react';
import { useMovieRoomStore } from '../../store/movieRoomStore';
import CreateMovieRoomModal from '../movie/CreateMovieRoomModal.tsx';
import { renderCEOBadgeUniversal } from '../../utils/userUtils';
import { useChatStore } from '../../store/chatStore';

export default function MoviesTab() {
  const { rooms, loadActiveRooms, setSelectedRoom } = useMovieRoomStore();
  const { users } = useChatStore();
  const [showCreateModal, setShowCreateModal] = useState(false);

  useEffect(() => {
    loadActiveRooms();
  }, [loadActiveRooms]);

  // Get user email from users list
  const getUserEmail = (userId: number): string => {
    const user = users.find(u => u.id === userId);
    return user?.email || '';
  };

  return (
    <div className="flex-1 overflow-y-auto">
      {/* Header */}
      <div className="p-4 border-b border-gray-200">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-xl font-semibold text-gray-900">Film Gecesi 🎬</h2>
          <button
            onClick={() => setShowCreateModal(true)}
            className="px-4 py-2 bg-gradient-to-r from-purple-500 to-pink-500 text-white rounded-lg hover:from-purple-600 hover:to-pink-600 transition-all"
          >
            + Oda Oluştur
          </button>
        </div>
        <p className="text-sm text-gray-600">
          Arkadaşlarınla YouTube'dan film izle ve sohbet et!
        </p>
      </div>

      {/* Movie Rooms List */}
      <div className="p-4 space-y-3">
        {rooms.length === 0 ? (
          <div className="text-center py-12">
            <div className="text-6xl mb-4">🎬</div>
            <p className="text-gray-500 mb-4">Henüz aktif film odası yok</p>
            <button
              onClick={() => setShowCreateModal(true)}
              className="px-6 py-3 bg-gradient-to-r from-purple-500 to-pink-500 text-white rounded-lg hover:from-purple-600 hover:to-pink-600 transition-all"
            >
              İlk Odayı Oluştur
            </button>
          </div>
        ) : (
          rooms.map((room) => (
            <div
              key={room.id}
              onClick={() => setSelectedRoom(room)}
              className="p-4 bg-white rounded-lg border border-gray-200 hover:border-purple-500 hover:shadow-md transition-all cursor-pointer"
            >
              <div className="flex items-start gap-3">
                {/* YouTube Thumbnail */}
                <div className="w-24 h-16 bg-gray-200 rounded-lg overflow-hidden flex-shrink-0">
                  <img
                    src={`https://img.youtube.com/vi/${room.youTubeVideoId}/mqdefault.jpg`}
                    alt={room.name}
                    className="w-full h-full object-cover"
                  />
                </div>

                {/* Room Info */}
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2 mb-1">
                    <h3 className="font-semibold text-gray-900 truncate">
                      {room.name}
                    </h3>
                    {room.isPlaying && (
                      <span className="text-xs px-2 py-0.5 bg-red-100 text-red-600 rounded-full">
                        ▶ Oynatılıyor
                      </span>
                    )}
                  </div>
                  
                  {room.description && (
                    <p className="text-sm text-gray-600 truncate mb-2">
                      {room.description}
                    </p>
                  )}

                  <div className="flex items-center gap-4 text-xs text-gray-500">
                    <span className="flex items-center gap-1">
                      <span>👤</span>
                      {room.participantCount} kişi
                    </span>
                    <span className="flex items-center gap-1">
                      <span>👑</span>
                      <span className="flex items-center">
                        {room.createdByUsername}
                        {renderCEOBadgeUniversal(getUserEmail(room.createdById), room.createdByUsername, 'text-xs px-1 py-0.5 ml-1')}
                      </span>
                    </span>
                  </div>
                </div>
              </div>
            </div>
          ))
        )}
      </div>

      {/* Create Room Modal */}
      {showCreateModal && (
        <CreateMovieRoomModal onClose={() => setShowCreateModal(false)} />
      )}
    </div>
  );
}
