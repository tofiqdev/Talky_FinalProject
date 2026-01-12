import { useEffect, useRef, useState } from 'react';
import { useChatStore } from '../../store/chatStore';
import { useAuthStore } from '../../store/authStore';

export default function MessageList() {
  const { messages } = useChatStore();
  const { user } = useAuthStore();
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const messagesContainerRef = useRef<HTMLDivElement>(null);

  // Auto scroll to bottom when new message arrives
  useEffect(() => {
    if (messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ behavior: 'smooth', block: 'end' });
    }
  }, [messages]);

  const isVoiceMessage = (content: string) => {
    return content.startsWith('[VOICE:');
  };

  const parseVoiceMessage = (content: string) => {
    const match = content.match(/\[VOICE:(\d+)s\](.*)/);
    if (match) {
      return {
        duration: parseInt(match[1]),
        audioData: match[2]
      };
    }
    return null;
  };

  const VoiceMessagePlayer = ({ content, isSent }: { content: string; isSent: boolean }) => {
    const [isPlaying, setIsPlaying] = useState(false);
    const audioRef = useRef<HTMLAudioElement>(null);
    const voiceData = parseVoiceMessage(content);

    if (!voiceData) return null;

    const togglePlay = () => {
      if (audioRef.current) {
        if (isPlaying) {
          audioRef.current.pause();
        } else {
          audioRef.current.play();
        }
        setIsPlaying(!isPlaying);
      }
    };

    const handleEnded = () => {
      setIsPlaying(false);
    };

    return (
      <div className={`flex items-center gap-3 px-4 py-3 rounded-3xl ${
        isSent
          ? 'bg-cyan-500 text-white rounded-br-md'
          : 'bg-gray-200 text-gray-900 rounded-bl-md'
      }`}>
        <button
          onClick={togglePlay}
          className={`w-10 h-10 rounded-full flex items-center justify-center transition ${
            isSent ? 'bg-white/20 hover:bg-white/30' : 'bg-gray-300 hover:bg-gray-400'
          }`}
        >
          {isPlaying ? (
            <svg className={`w-5 h-5 ${isSent ? 'text-white' : 'text-gray-700'}`} fill="currentColor" viewBox="0 0 24 24">
              <path d="M6 4h4v16H6V4zm8 0h4v16h-4V4z" />
            </svg>
          ) : (
            <svg className={`w-5 h-5 ${isSent ? 'text-white' : 'text-gray-700'}`} fill="currentColor" viewBox="0 0 24 24">
              <path d="M8 5v14l11-7z" />
            </svg>
          )}
        </button>
        
        <div className="flex-1 flex items-center gap-2">
          <div className="flex-1 h-1 bg-white/20 rounded-full overflow-hidden">
            <div className={`h-full ${isSent ? 'bg-white' : 'bg-cyan-500'} transition-all duration-300`} 
                 style={{ width: isPlaying ? '100%' : '0%' }}></div>
          </div>
          <span className="text-xs font-medium">{voiceData.duration}s</span>
        </div>

        <audio
          ref={audioRef}
          src={voiceData.audioData}
          onEnded={handleEnded}
          className="hidden"
        />
      </div>
    );
  };

  return (
    <div 
      ref={messagesContainerRef}
      className="flex-1 overflow-y-auto px-6 py-4 space-y-4 scroll-smooth"
    >
      {/* Date Divider */}
      <div className="flex items-center justify-center">
        <span className="px-4 py-1 bg-gray-200 text-gray-600 text-xs rounded-full">TODAY</span>
      </div>

      {messages.length === 0 ? (
        <div className="flex items-center justify-center h-full text-gray-500">
          No messages yet. Start the conversation!
        </div>
      ) : (
        messages.map((msg, index) => {
          const isSent = msg.senderId === user?.id;
          return (
            <div 
              key={msg.id} 
              className={`flex ${isSent ? 'justify-end' : 'justify-start'} animate-fadeIn`}
              style={{
                animationDelay: `${index * 0.05}s`,
                animationFillMode: 'both'
              }}
            >
              {!isSent && (
                <div className="w-8 h-8 rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 mr-2 flex-shrink-0 flex items-center justify-center text-white text-sm font-semibold">
                  {msg.senderUsername.charAt(0).toUpperCase()}
                </div>
              )}
              <div className={`max-w-xl ${isSent ? 'ml-12' : 'mr-12'}`}>
                {isVoiceMessage(msg.content) ? (
                  <VoiceMessagePlayer content={msg.content} isSent={isSent} />
                ) : (
                  <div
                    className={`px-5 py-3 rounded-3xl transition-all duration-300 hover:scale-[1.02] ${
                      isSent
                        ? 'bg-cyan-500 text-white rounded-br-md'
                        : 'bg-gray-200 text-gray-900 rounded-bl-md'
                    }`}
                  >
                    <p className="text-sm leading-relaxed">{msg.content}</p>
                  </div>
                )}
                <div className={`flex items-center gap-1 mt-1 px-2 ${isSent ? 'justify-end' : 'justify-start'}`}>
                  <span className="text-xs text-gray-500">
                    {new Date(msg.sentAt).toLocaleTimeString('en-US', {
                      hour: '2-digit',
                      minute: '2-digit',
                    })}
                  </span>
                  {isSent && msg.isRead && (
                    <svg className="w-4 h-4 text-cyan-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                    </svg>
                  )}
                </div>
              </div>
            </div>
          );
        })
      )}
      
      {/* Invisible element to scroll to */}
      <div ref={messagesEndRef} />
    </div>
  );
}
