import { useEffect, useRef, useState } from 'react';
import { useChatStore } from '../../store/chatStore';
import { useAuthStore } from '../../store/authStore';

export default function MessageList() {
  const { messages, selectedGroup } = useChatStore();
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

  const isImageMessage = (content: string) => {
    return content.startsWith('[IMAGE:');
  };

  const isFileMessage = (content: string) => {
    return content.startsWith('[FILE:');
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

  const parseImageMessage = (content: string) => {
    const match = content.match(/\[IMAGE:(.*?)\](.*)/);
    if (match) {
      return {
        filename: match[1],
        imageData: match[2]
      };
    }
    return null;
  };

  const parseFileMessage = (content: string) => {
    const match = content.match(/\[FILE:(.*?)\](.*)/);
    if (match) {
      return {
        filename: match[1],
        fileData: match[2]
      };
    }
    return null;
  };

  // Parse and highlight mentions in message content
  const renderMessageContent = (content: string, isSent: boolean) => {
    // Match @username patterns
    const mentionRegex = /@(\w+)/g;
    const parts = [];
    let lastIndex = 0;
    let match;

    while ((match = mentionRegex.exec(content)) !== null) {
      // Add text before mention
      if (match.index > lastIndex) {
        parts.push(
          <span key={`text-${lastIndex}`}>
            {content.substring(lastIndex, match.index)}
          </span>
        );
      }

      // Check if this mention is the current user
      const mentionedUsername = match[1];
      const isCurrentUserMentioned = mentionedUsername.toLowerCase() === user?.username.toLowerCase();

      // Add mention with styling
      parts.push(
        <span
          key={`mention-${match.index}`}
          className={`font-semibold ${
            isCurrentUserMentioned
              ? isSent
                ? 'text-yellow-200 bg-yellow-500/20 px-1 rounded'
                : 'text-cyan-600 bg-cyan-100 px-1 rounded'
              : isSent
              ? 'text-white/90'
              : 'text-cyan-600'
          }`}
        >
          @{mentionedUsername}
        </span>
      );

      lastIndex = match.index + match[0].length;
    }

    // Add remaining text
    if (lastIndex < content.length) {
      parts.push(
        <span key={`text-${lastIndex}`}>
          {content.substring(lastIndex)}
        </span>
      );
    }

    return parts.length > 0 ? parts : content;
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

  const ImageMessage = ({ content }: { content: string }) => {
    const imageData = parseImageMessage(content);
    if (!imageData) return null;

    return (
      <div className="max-w-sm">
        <img 
          src={imageData.imageData} 
          alt={imageData.filename}
          className="rounded-lg max-w-full h-auto cursor-pointer hover:opacity-90 transition"
          onClick={() => window.open(imageData.imageData, '_blank')}
        />
        <p className="text-xs mt-1 opacity-75">{imageData.filename}</p>
      </div>
    );
  };

  const FileMessage = ({ content, isSent }: { content: string; isSent: boolean }) => {
    const fileData = parseFileMessage(content);
    if (!fileData) return null;

    const handleDownload = () => {
      const link = document.createElement('a');
      link.href = fileData.fileData;
      link.download = fileData.filename;
      link.click();
    };

    return (
      <div 
        onClick={handleDownload}
        className={`flex items-center gap-3 p-3 rounded-lg cursor-pointer hover:opacity-90 transition ${
          isSent ? 'bg-white/10' : 'bg-gray-100'
        }`}
      >
        <div className={`w-10 h-10 rounded-full flex items-center justify-center ${
          isSent ? 'bg-white/20' : 'bg-cyan-100'
        }`}>
          <svg className={`w-5 h-5 ${isSent ? 'text-white' : 'text-cyan-600'}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
          </svg>
        </div>
        <div className="flex-1 min-w-0">
          <p className="text-sm font-medium truncate">{fileData.filename}</p>
          <p className="text-xs opacity-75">Click to download</p>
        </div>
        <svg className={`w-5 h-5 ${isSent ? 'text-white' : 'text-gray-600'}`} fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
        </svg>
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
          const isSystemMessage = 'isSystemMessage' in msg && msg.isSystemMessage;

          // System message (centered, special styling)
          if (isSystemMessage) {
            return (
              <div 
                key={msg.id} 
                className="flex justify-center animate-fadeIn"
                style={{
                  animationDelay: `${index * 0.05}s`,
                  animationFillMode: 'both'
                }}
              >
                <div className="max-w-md px-4 py-2 bg-yellow-50 border border-yellow-200 rounded-lg text-center">
                  <p className="text-sm text-gray-700">
                    {renderMessageContent(msg.content, false)}
                  </p>
                  <span className="text-xs text-gray-500 mt-1 block">
                    {new Date(msg.sentAt).toLocaleTimeString('en-US', {
                      hour: '2-digit',
                      minute: '2-digit',
                    })}
                  </span>
                </div>
              </div>
            );
          }

          // Regular message
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
                  {msg.senderUsername?.charAt(0).toUpperCase() || 'U'}
                </div>
              )}
              <div className={`max-w-xl ${isSent ? 'ml-12' : 'mr-12'}`}>
                {isVoiceMessage(msg.content) ? (
                  <VoiceMessagePlayer content={msg.content} isSent={isSent} />
                ) : isImageMessage(msg.content) ? (
                  <div className={`rounded-2xl overflow-hidden ${isSent ? 'bg-cyan-500' : 'bg-gray-200'} p-2`}>
                    <ImageMessage content={msg.content} />
                  </div>
                ) : isFileMessage(msg.content) ? (
                  <div className={`rounded-2xl ${isSent ? 'bg-cyan-500' : 'bg-gray-200'}`}>
                    <FileMessage content={msg.content} isSent={isSent} />
                  </div>
                ) : (
                  <div
                    className={`px-5 py-3 rounded-3xl transition-all duration-300 hover:scale-[1.02] ${
                      isSent
                        ? 'bg-cyan-500 text-white rounded-br-md'
                        : 'bg-gray-200 text-gray-900 rounded-bl-md'
                    }`}
                  >
                    {!isSent && selectedGroup && (
                      <p className="text-xs font-semibold mb-1 text-gray-600">
                        {msg.senderUsername}
                      </p>
                    )}
                    <p className="text-sm leading-relaxed">
                      {renderMessageContent(msg.content, isSent)}
                    </p>
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
