import { useState, useRef, useEffect } from 'react';
import { useChatStore } from '../../store/chatStore';
import { useAuthStore } from '../../store/authStore';
import MessageList from './MessageList';
import GroupDetailsModal from '../group/GroupDetailsModal';
import messageSendSound from '../../assets/message_send_sound.mp3';
import EmojiPicker, { type EmojiClickData } from 'emoji-picker-react';

export default function ChatWindow() {
  const { selectedUser, selectedGroup, sendMessage, sendGroupMessage, loadGroups, loadGroupMessages } = useChatStore();
  const { user } = useAuthStore();
  
  // Debug log
  console.log('ChatWindow render:', { selectedUser, selectedGroup });
  
  const [message, setMessage] = useState('');
  const [isSending, setIsSending] = useState(false);
  const [isRecording, setIsRecording] = useState(false);
  const [recordingTime, setRecordingTime] = useState(0);
  const [showGroupDetails, setShowGroupDetails] = useState(false);
  const [showMentionSuggestions, setShowMentionSuggestions] = useState(false);
  const [showEmojiPicker, setShowEmojiPicker] = useState(false);
  const [mentionSearch, setMentionSearch] = useState('');
  const [mentionStartPos, setMentionStartPos] = useState(0);
  const [selectedMentionIndex, setSelectedMentionIndex] = useState(0);
  const [isContact, setIsContact] = useState(true); // Assume contact initially
  const [addingContact, setAddingContact] = useState(false);
  const mediaRecorderRef = useRef<MediaRecorder | null>(null);
  const audioChunksRef = useRef<Blob[]>([]);
  const recordingIntervalRef = useRef<ReturnType<typeof setInterval> | null>(null);
  const inputRef = useRef<HTMLInputElement>(null);
  const audioRef = useRef<HTMLAudioElement | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  // Check if user is contact
  useEffect(() => {
    if (selectedUser && !selectedGroup) {
      checkIsContact(selectedUser.id);
    }
  }, [selectedUser, selectedGroup]);

  const checkIsContact = async (userId: number) => {
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/contacts/check/${userId}`, {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      
      if (response.ok) {
        const isContactResult = await response.json();
        setIsContact(isContactResult);
      }
    } catch (error) {
      console.error('Failed to check contact status:', error);
    }
  };

  const handleAddToContacts = async () => {
    if (!selectedUser) return;
    
    setAddingContact(true);
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/contacts/${selectedUser.id}`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });

      if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Failed to add contact');
      }

      setIsContact(true);
      alert('Contact added successfully!');
    } catch (error) {
      console.error('Failed to add contact:', error);
      alert(error instanceof Error ? error.message : 'Failed to add contact');
    } finally {
      setAddingContact(false);
    }
  };

  // Initialize audio
  if (!audioRef.current) {
    audioRef.current = new Audio(messageSendSound);
    audioRef.current.volume = 0.5; // 50% volume
  }

  const playMessageSound = () => {
    try {
      audioRef.current?.play().catch(err => {
        console.log('Could not play sound:', err);
      });
    } catch (error) {
      console.log('Sound play error:', error);
    }
  };

  const handleEmojiClick = (emojiData: EmojiClickData) => {
    setMessage(prev => prev + emojiData.emoji);
    setShowEmojiPicker(false);
    inputRef.current?.focus();
  };

  const handleFileSelect = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    // Check file size (max 5MB for better performance)
    if (file.size > 5 * 1024 * 1024) {
      alert('File size must be less than 5MB');
      return;
    }

    try {
      setIsSending(true);
      
      let base64: string;
      
      // If it's an image, compress it
      if (file.type.startsWith('image/')) {
        base64 = await compressImage(file);
      } else {
        // For non-images, just convert to base64
        base64 = await new Promise<string>((resolve, reject) => {
          const reader = new FileReader();
          reader.onloadend = () => resolve(reader.result as string);
          reader.onerror = reject;
          reader.readAsDataURL(file);
        });
      }

      // Determine file type
      const isImage = file.type.startsWith('image/');
      const fileMessage = isImage 
        ? `[IMAGE:${file.name}]${base64}`
        : `[FILE:${file.name}]${base64}`;

      // Send as message
      if (selectedGroup) {
        await sendGroupMessage(selectedGroup.id, fileMessage);
      } else if (selectedUser) {
        await sendMessage(selectedUser.id, fileMessage);
      }

      playMessageSound();
      
      // Reset file input
      if (fileInputRef.current) {
        fileInputRef.current.value = '';
      }
    } catch (error) {
      console.error('Failed to send file:', error);
      alert('Failed to send file: ' + (error instanceof Error ? error.message : 'Unknown error'));
    } finally {
      setIsSending(false);
    }
  };

  const compressImage = (file: File): Promise<string> => {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = (e) => {
        const img = new Image();
        img.onload = () => {
          const canvas = document.createElement('canvas');
          let width = img.width;
          let height = img.height;
          
          // Max dimensions
          const maxWidth = 1024;
          const maxHeight = 1024;
          
          // Calculate new dimensions
          if (width > height) {
            if (width > maxWidth) {
              height = (height * maxWidth) / width;
              width = maxWidth;
            }
          } else {
            if (height > maxHeight) {
              width = (width * maxHeight) / height;
              height = maxHeight;
            }
          }
          
          canvas.width = width;
          canvas.height = height;
          
          const ctx = canvas.getContext('2d');
          ctx?.drawImage(img, 0, 0, width, height);
          
          // Convert to base64 with compression (0.7 quality)
          const compressedBase64 = canvas.toDataURL('image/jpeg', 0.7);
          resolve(compressedBase64);
        };
        img.onerror = reject;
        img.src = e.target?.result as string;
      };
      reader.onerror = reject;
      reader.readAsDataURL(file);
    });
  };

  const isGroup = !!selectedGroup;
  const chatTarget = selectedUser || selectedGroup;

  // Check if current user is muted in group
  const currentMember = selectedGroup?.members.find(m => m.userId === user?.id);
  const isMuted = currentMember?.isMuted || false;
  
  // Check if group is muted for all (only admins can speak)
  const isOwner = selectedGroup?.createdById === user?.id;
  const isAdmin = currentMember?.isAdmin || false;
  const isMutedForAll = selectedGroup?.isMutedForAll || false;
  const canSpeak = !isMuted && (!isMutedForAll || isOwner || isAdmin);
  
  // Get mute warning message
  const getMuteWarning = () => {
    if (isMuted) return "🔇 You are muted in this group";
    if (isMutedForAll && !isOwner && !isAdmin) return "🔇 Group is muted. Only admins can send messages";
    return "";
  };
  
  const muteWarning = getMuteWarning();

  // Get filtered members for mention suggestions
  const getMentionSuggestions = () => {
    if (!selectedGroup) return [];
    
    const search = mentionSearch.toLowerCase();
    return selectedGroup.members
      .filter(member => member.username.toLowerCase().includes(search))
      .slice(0, 5); // Show max 5 suggestions
  };

  const mentionSuggestions = getMentionSuggestions();

  // Handle input change with mention detection
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    const cursorPos = e.target.selectionStart || 0;
    
    setMessage(value);

    // Check if user is typing @ mention
    if (isGroup && value.includes('@')) {
      const textBeforeCursor = value.substring(0, cursorPos);
      const lastAtIndex = textBeforeCursor.lastIndexOf('@');
      
      if (lastAtIndex !== -1) {
        const textAfterAt = textBeforeCursor.substring(lastAtIndex + 1);
        
        // Check if there's a space after @ (which would end the mention)
        if (!textAfterAt.includes(' ')) {
          setMentionSearch(textAfterAt);
          setMentionStartPos(lastAtIndex);
          setShowMentionSuggestions(true);
          setSelectedMentionIndex(0);
          return;
        }
      }
    }
    
    setShowMentionSuggestions(false);
  };

  // Handle mention selection
  const selectMention = (username: string) => {
    const beforeMention = message.substring(0, mentionStartPos);
    const afterMention = message.substring(mentionStartPos + mentionSearch.length + 1);
    const newMessage = `${beforeMention}@${username} ${afterMention}`;
    
    setMessage(newMessage);
    setShowMentionSuggestions(false);
    setMentionSearch('');
    
    // Focus back on input
    inputRef.current?.focus();
  };

  // Handle keyboard navigation for mentions
  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (!showMentionSuggestions) return;

    if (e.key === 'ArrowDown') {
      e.preventDefault();
      setSelectedMentionIndex(prev => 
        prev < mentionSuggestions.length - 1 ? prev + 1 : prev
      );
    } else if (e.key === 'ArrowUp') {
      e.preventDefault();
      setSelectedMentionIndex(prev => prev > 0 ? prev - 1 : 0);
    } else if (e.key === 'Enter' && mentionSuggestions.length > 0) {
      e.preventDefault();
      selectMention(mentionSuggestions[selectedMentionIndex].username);
    } else if (e.key === 'Escape') {
      setShowMentionSuggestions(false);
    }
  };

  const handleSend = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!message.trim() || (!selectedUser && !selectedGroup) || isSending) return;

    const messageToSend = message;
    setIsSending(true);
    
    // Clear input immediately with animation
    setMessage('');
    
    try {
      // Send message (backend will handle commands)
      if (selectedGroup) {
        await sendGroupMessage(selectedGroup.id, messageToSend);
        
        // If it's a command, reload group data
        if (messageToSend.startsWith('/') || messageToSend.startsWith('@')) {
          await loadGroups();
          await loadGroupMessages(selectedGroup.id);
        }
        
        playMessageSound();
      } else if (selectedUser) {
        await sendMessage(selectedUser.id, messageToSend);
        playMessageSound();
      }
    } catch (error) {
      console.error('Failed to send message:', error);
      // Restore message on error
      setMessage(messageToSend);
      alert(error instanceof Error ? error.message : 'Failed to send message');
    } finally {
      setIsSending(false);
    }
  };

  const startRecording = async () => {
    try {
      console.log('Requesting microphone access...');
      const stream = await navigator.mediaDevices.getUserMedia({ 
        audio: {
          echoCancellation: true,
          noiseSuppression: true,
          sampleRate: 16000 // Lower sample rate for smaller file size
        } 
      });
      console.log('Microphone access granted');
      
      // Use lower bitrate for smaller file size
      const options = { 
        mimeType: 'audio/webm;codecs=opus',
        audioBitsPerSecond: 16000 // Lower bitrate
      };
      
      const mediaRecorder = new MediaRecorder(stream, options);
      mediaRecorderRef.current = mediaRecorder;
      audioChunksRef.current = [];

      mediaRecorder.ondataavailable = (event) => {
        console.log('Data available, size:', event.data.size);
        if (event.data.size > 0) {
          audioChunksRef.current.push(event.data);
        }
      };

      mediaRecorder.onstop = async () => {
        console.log('Recording stopped, chunks:', audioChunksRef.current.length);
        const audioBlob = new Blob(audioChunksRef.current, { type: 'audio/webm;codecs=opus' });
        console.log('Audio blob created, size:', audioBlob.size);
        await sendVoiceMessage(audioBlob);
      };

      mediaRecorder.start();
      console.log('Recording started');
      setIsRecording(true);
      setRecordingTime(0);

      // Start timer
      recordingIntervalRef.current = setInterval(() => {
        setRecordingTime(prev => prev + 1);
      }, 1000);

    } catch (error) {
      console.error('Failed to start recording:', error);
      alert('Mikrofon erişimi reddedildi. Lütfen tarayıcı ayarlarından mikrofon iznini kontrol edin.');
    }
  };

  const stopRecording = () => {
    console.log('Stopping recording...');
    if (mediaRecorderRef.current && isRecording) {
      const recorder = mediaRecorderRef.current;
      recorder.stop();
      setIsRecording(false);
      
      if (recordingIntervalRef.current) {
        clearInterval(recordingIntervalRef.current);
        recordingIntervalRef.current = null;
      }

      // Stop all tracks after a short delay to ensure data is captured
      setTimeout(() => {
        if (recorder.stream) {
          recorder.stream.getTracks().forEach(track => {
            console.log('Stopping track:', track.kind);
            track.stop();
          });
        }
      }, 100);
    }
  };

  const cancelRecording = () => {
    if (mediaRecorderRef.current && isRecording) {
      mediaRecorderRef.current.stop();
      setIsRecording(false);
      audioChunksRef.current = [];
      
      if (recordingIntervalRef.current) {
        clearInterval(recordingIntervalRef.current);
        recordingIntervalRef.current = null;
      }

      // Stop all tracks
      if (mediaRecorderRef.current.stream) {
        mediaRecorderRef.current.stream.getTracks().forEach(track => track.stop());
      }
    }
  };

  const sendVoiceMessage = async (audioBlob: Blob) => {
    if (!selectedUser) return;

    console.log('Sending voice message, blob size:', audioBlob.size, 'duration:', recordingTime);
    setIsSending(true);

    try {
      // For voice messages, use REST API instead of SignalR (base64 is too large)
      const base64Audio = await new Promise<string>((resolve, reject) => {
        const reader = new FileReader();
        reader.onloadend = () => {
          if (reader.result) {
            resolve(reader.result as string);
          } else {
            reject(new Error('Failed to read audio'));
          }
        };
        reader.onerror = reject;
        reader.readAsDataURL(audioBlob);
      });

      console.log('Audio converted to base64, length:', base64Audio.length);
      
      // Send as special voice message format via REST API
      const voiceMessage = `[VOICE:${recordingTime}s]${base64Audio}`;
      console.log('Sending voice message via REST API...');
      
      // Import messagesApi
      const { messagesApi } = await import('../../services/apiService');
      const message = await messagesApi.sendMessage(selectedUser.id, voiceMessage);
      
      // Add to local state
      const { addMessage } = useChatStore.getState();
      addMessage(message);
      
      console.log('Voice message sent successfully');
    } catch (error) {
      console.error('Failed to send voice message:', error);
      alert('Ses mesajı gönderilemedi. Lütfen tekrar deneyin.');
    } finally {
      setIsSending(false);
    }
  };

  const formatTime = (seconds: number) => {
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  };

  if (!chatTarget) return null;

  return (
    <div className="flex-1 flex flex-col bg-gray-50">
      {/* Header */}
      <div className="bg-white px-6 py-4 flex items-center justify-between border-b border-gray-100">
        <div className="flex items-center gap-3">
          <div className="relative">
            {!isGroup && selectedUser?.avatar ? (
              <img 
                src={selectedUser.avatar} 
                alt={selectedUser.username}
                className="w-10 h-10 rounded-full object-cover"
              />
            ) : isGroup && selectedGroup?.avatar ? (
              <img 
                src={selectedGroup.avatar} 
                alt={selectedGroup.name}
                className="w-10 h-10 rounded-full object-cover"
              />
            ) : (
              <div className={`w-10 h-10 rounded-full ${isGroup ? 'bg-gradient-to-br from-purple-400 to-pink-500' : 'bg-gradient-to-br from-cyan-400 to-blue-500'} flex items-center justify-center text-white font-semibold`}>
                {isGroup 
                  ? selectedGroup?.name.charAt(0).toUpperCase() 
                  : selectedUser?.username.charAt(0).toUpperCase()
                }
              </div>
            )}
            {!isGroup && selectedUser?.isOnline && (
              <div className="absolute bottom-0 right-0 w-3 h-3 bg-green-500 border-2 border-white rounded-full"></div>
            )}
            {isGroup && selectedGroup && (
              <div className="absolute bottom-0 right-0 w-5 h-5 bg-gray-700 text-white text-xs rounded-full flex items-center justify-center border-2 border-white">
                {selectedGroup.memberCount}
              </div>
            )}
          </div>
          <div>
            <h3 className="font-semibold text-gray-900">
              {isGroup ? selectedGroup?.name : selectedUser?.username}
            </h3>
            <p className={`text-xs ${!isGroup && selectedUser?.isOnline ? 'text-green-500' : 'text-gray-500'}`}>
              {isGroup ? `${selectedGroup?.memberCount} members` : (selectedUser?.isOnline ? 'Online' : 'Offline')}
            </p>
          </div>
        </div>
        <div className="flex items-center gap-2">
          {!isGroup && (
            <>
              <button className="w-10 h-10 rounded-full hover:bg-gray-100 flex items-center justify-center text-cyan-500">
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z" />
                </svg>
              </button>
              <button className="w-10 h-10 rounded-full hover:bg-gray-100 flex items-center justify-center text-cyan-500">
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                </svg>
              </button>
            </>
          )}
          {isGroup && selectedGroup && (
            <button 
              onClick={() => setShowGroupDetails(true)}
              className="w-10 h-10 rounded-full hover:bg-gray-100 flex items-center justify-center text-purple-500"
              title="Group Details"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </button>
          )}
          <button className="w-10 h-10 rounded-full hover:bg-gray-100 flex items-center justify-center text-gray-600">
            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 5v.01M12 12v.01M12 19v.01M12 6a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2zm0 7a1 1 0 110-2 1 1 0 010 2z" />
            </svg>
          </button>
        </div>
      </div>

      {/* Add to Contacts Banner */}
      {!isGroup && selectedUser && !isContact && (
        <div className="bg-yellow-50 border-b border-yellow-200 px-6 py-3 flex items-center justify-between">
          <div className="flex items-center gap-2">
            <svg className="w-5 h-5 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
            </svg>
            <span className="text-sm text-yellow-800">
              <strong>{selectedUser.username}</strong> is not in your contacts
            </span>
          </div>
          <button
            onClick={handleAddToContacts}
            disabled={addingContact}
            className="px-4 py-1.5 bg-cyan-500 text-white text-sm rounded-lg hover:bg-cyan-600 transition disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {addingContact ? 'Adding...' : 'Add to Contacts'}
          </button>
        </div>
      )}

      <MessageList />

      {/* Group Details Modal */}
      {isGroup && selectedGroup && (
        <GroupDetailsModal
          group={selectedGroup}
          isOpen={showGroupDetails}
          onClose={() => setShowGroupDetails(false)}
          onUpdate={() => {
            loadGroups();
            setShowGroupDetails(false);
          }}
          onDelete={() => {
            loadGroups();
            setShowGroupDetails(false);
            // Clear selection since group is deleted/left
            window.location.reload();
          }}
        />
      )}

      {/* Input */}
      <div className="bg-white px-6 py-4 border-t border-gray-100 relative">
        {/* Muted Warning */}
        {isMuted && (
          <div className="mb-3 px-4 py-2 bg-red-50 border border-red-200 rounded-lg text-center">
            <p className="text-sm text-red-600 font-medium">
              🔇 You are muted in this group and cannot send messages
            </p>
          </div>
        )}

        {/* Mention Suggestions Dropdown */}
        {showMentionSuggestions && mentionSuggestions.length > 0 && !isMuted && (
          <div className="absolute bottom-full left-6 right-6 mb-2 bg-white rounded-lg shadow-lg border border-gray-200 max-h-60 overflow-y-auto">
            {mentionSuggestions.map((member, index) => (
              <button
                key={member.userId}
                onClick={() => selectMention(member.username)}
                className={`w-full px-4 py-2 flex items-center gap-3 hover:bg-gray-50 transition ${
                  index === selectedMentionIndex ? 'bg-cyan-50' : ''
                }`}
              >
                <div className="relative flex-shrink-0">
                  <div className="w-8 h-8 rounded-full bg-gradient-to-br from-cyan-400 to-blue-500 flex items-center justify-center text-white text-sm font-semibold">
                    {member.username.charAt(0).toUpperCase()}
                  </div>
                  {member.isOnline && (
                    <div className="absolute bottom-0 right-0 w-2 h-2 bg-green-500 border border-white rounded-full"></div>
                  )}
                </div>
                <div className="flex-1 text-left">
                  <p className="text-sm font-medium text-gray-900">@{member.username}</p>
                  {member.isAdmin && (
                    <p className="text-xs text-cyan-600">Admin</p>
                  )}
                </div>
              </button>
            ))}
          </div>
        )}

        {isRecording ? (
          // Recording UI
          <div className="flex items-center gap-3">
            <button 
              onClick={cancelRecording}
              className="text-red-500 hover:text-red-600 transition"
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
            
            <div className="flex-1 flex items-center gap-3 px-4 py-2.5 bg-red-50 rounded-full">
              <div className="w-3 h-3 bg-red-500 rounded-full animate-pulse"></div>
              <span className="text-sm text-red-600 font-medium">Kaydediliyor...</span>
              <span className="text-sm text-red-600 ml-auto">{formatTime(recordingTime)}</span>
            </div>

            <button
              onClick={stopRecording}
              className="w-10 h-10 rounded-full bg-cyan-500 hover:bg-cyan-600 flex items-center justify-center text-white transition-all duration-200 hover:scale-110 active:scale-95"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
              </svg>
            </button>
          </div>
        ) : (
          // Normal input UI
          <form onSubmit={handleSend} className="flex items-center gap-3 relative">
            {/* Hidden file input */}
            <input
              ref={fileInputRef}
              type="file"
              accept="image/*,application/pdf,.doc,.docx,.txt"
              onChange={handleFileSelect}
              className="hidden"
            />
            
            {/* File upload button */}
            <button 
              type="button" 
              onClick={() => fileInputRef.current?.click()}
              disabled={!canSpeak}
              className="text-gray-400 hover:text-cyan-500 transition disabled:opacity-50 disabled:cursor-not-allowed"
              title="Send file or image"
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15.172 7l-6.586 6.586a2 2 0 102.828 2.828l6.414-6.586a4 4 0 00-5.656-5.656l-6.415 6.585a6 6 0 108.486 8.486L20.5 13" />
              </svg>
            </button>

            {/* Emoji picker */}
            <div className="relative">
              <button 
                type="button"
                onClick={() => setShowEmojiPicker(!showEmojiPicker)}
                disabled={!canSpeak}
                className="text-gray-400 hover:text-cyan-500 transition disabled:opacity-50 disabled:cursor-not-allowed"
                title="Add emoji"
              >
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M14.828 14.828a4 4 0 01-5.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
              </button>
              
              {showEmojiPicker && (
                <div className="absolute bottom-12 left-0 z-50">
                  <EmojiPicker onEmojiClick={handleEmojiClick} />
                </div>
              )}
            </div>
            <input
              ref={inputRef}
              type="text"
              value={message}
              onChange={handleInputChange}
              onKeyDown={handleKeyDown}
              placeholder={muteWarning || (isGroup ? "Message... (Type @ to mention, @user /mute, /muteall, /unmuteall)" : "Message...")}
              disabled={isSending || !canSpeak}
              className="flex-1 px-4 py-2.5 bg-gray-100 rounded-full focus:outline-none focus:ring-2 focus:ring-cyan-500 text-sm disabled:opacity-50 transition-all duration-200 focus:bg-white"
            />
            <button 
              type="button" 
              onMouseDown={startRecording}
              onMouseUp={stopRecording}
              onMouseLeave={cancelRecording}
              onTouchStart={startRecording}
              onTouchEnd={stopRecording}
              disabled={!canSpeak}
              className="text-gray-400 hover:text-cyan-500 transition active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed"
              title={muteWarning || "Basılı tutarak ses kaydı yapın"}
            >
              <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 11a7 7 0 01-7 7m0 0a7 7 0 01-7-7m7 7v4m0 0H8m4 0h4m-4-8a3 3 0 01-3-3V5a3 3 0 116 0v6a3 3 0 01-3 3z" />
              </svg>
            </button>
            <button
              type="submit"
              disabled={isSending || !message.trim() || !canSpeak}
              className="w-10 h-10 rounded-full bg-cyan-500 hover:bg-cyan-600 flex items-center justify-center text-white transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed hover:scale-110 active:scale-95"
            >
              {isSending ? (
                <svg className="w-5 h-5 animate-spin" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
              ) : (
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                </svg>
              )}
            </button>
          </form>
        )}
      </div>
    </div>
  );
}
