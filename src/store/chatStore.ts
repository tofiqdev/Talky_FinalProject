import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { Message } from '../types/message';
import type { User } from '../types/user';
import type { Group } from '../types/group';
import type { Call } from '../types/call';
import { usersApi, messagesApi } from '../services/apiService';
import { signalrService } from '../services/signalrService';

interface ChatState {
  messages: Message[];
  users: User[];
  groups: Group[];
  calls: Call[];
  selectedUser: User | null;
  selectedGroup: Group | null;
  isLoading: boolean;
  error: string | null;
  
  // Actions
  loadUsers: () => Promise<void>;
  loadGroups: () => Promise<void>;
  loadCalls: () => Promise<void>;
  loadMessages: (userId: number) => Promise<void>;
  loadGroupMessages: (groupId: number) => Promise<void>;
  sendMessage: (receiverId: number, content: string) => Promise<void>;
  sendGroupMessage: (groupId: number, content: string) => Promise<void>;
  setSelectedUser: (user: User | null) => void;
  setSelectedGroup: (group: Group | null) => void;
  addMessage: (message: Message) => void;
  updateUserStatus: (userId: number, isOnline: boolean) => void;
  initializeSignalR: () => void;
  cleanup: () => void;
}

export const useChatStore = create<ChatState>()(
  persist(
    (set, get) => ({
      messages: [],
      users: [],
      groups: [],
      calls: [],
      selectedUser: null,
      selectedGroup: null,
      isLoading: false,
      error: null,

  loadUsers: async () => {
    set({ isLoading: true, error: null });
    try {
      const users = await usersApi.getAllUsers();
      set({ users });
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to load users';
      set({ error: errorMessage });
      console.error('Failed to load users:', error);
    } finally {
      set({ isLoading: false });
    }
  },

  loadGroups: async () => {
    set({ isLoading: true, error: null });
    try {
      const API_BASE_URL = import.meta.env.VITE_API_URL || '/api';
      const token = localStorage.getItem('token');
      const response = await fetch(`${API_BASE_URL}/Groups`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'ngrok-skip-browser-warning': 'true'
        }
      });
      
      if (!response.ok) throw new Error('Failed to load groups');
      
      const groups = await response.json();
      set({ groups });
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to load groups';
      set({ error: errorMessage });
      console.error('Failed to load groups:', error);
    } finally {
      set({ isLoading: false });
    }
  },

  loadCalls: async () => {
    set({ isLoading: true, error: null });
    try {
      const API_BASE_URL = import.meta.env.VITE_API_URL || '/api';
      const token = localStorage.getItem('token');
      const response = await fetch(`${API_BASE_URL}/Call`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'ngrok-skip-browser-warning': 'true'
        }
      });
      
      if (!response.ok) throw new Error('Failed to load calls');
      
      const calls = await response.json();
      set({ calls });
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to load calls';
      set({ error: errorMessage });
      console.error('Failed to load calls:', error);
    } finally {
      set({ isLoading: false });
    }
  },

  loadMessages: async (userId: number) => {
    set({ isLoading: true, error: null });
    try {
      const messages = await messagesApi.getMessages(userId);
      set({ messages });
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to load messages';
      set({ error: errorMessage });
      console.error('Failed to load messages:', error);
    } finally {
      set({ isLoading: false });
    }
  },

  loadGroupMessages: async (groupId: number) => {
    set({ isLoading: true, error: null });
    try {
      const API_BASE_URL = import.meta.env.VITE_API_URL || '/api';
      const token = localStorage.getItem('token');
      const response = await fetch(`${API_BASE_URL}/Groups/${groupId}/messages`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'ngrok-skip-browser-warning': 'true'
        }
      });
      
      if (!response.ok) throw new Error('Failed to load group messages');
      
      const groupMessages = await response.json();
      
      // Convert group messages to Message format (with isSystemMessage support)
      const messages = groupMessages.map((gm: any) => ({
        id: gm.id,
        senderId: gm.senderId,
        receiverId: groupId, // Use groupId as receiverId for compatibility
        senderUsername: gm.senderUsername,
        receiverUsername: '', // Not needed for groups
        content: gm.content,
        isSystemMessage: gm.isSystemMessage || false, // Include system message flag
        isRead: true,
        sentAt: gm.sentAt,
        readAt: null
      }));
      
      set({ messages });
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to load group messages';
      set({ error: errorMessage });
      console.error('Failed to load group messages:', error);
    } finally {
      set({ isLoading: false });
    }
  },

  sendMessage: async (receiverId: number, content: string) => {
    const maxRetries = 3;
    let lastError: Error | null = null;

    for (let attempt = 1; attempt <= maxRetries; attempt++) {
      try {
        console.log(`ChatStore: Sending message attempt ${attempt}/${maxRetries} to:`, receiverId, 'content:', content);
        
        // Add a small delay between retries to avoid rapid duplicate requests
        if (attempt > 1) {
          await new Promise(resolve => setTimeout(resolve, 1000 * attempt));
        }

        // First, save message to database via API
        console.log('ChatStore: Calling messagesApi.sendMessage...');
        const message = await messagesApi.sendMessage(receiverId, content);
        console.log('ChatStore: Message saved to database:', message);
        
        // Then send via SignalR for real-time delivery
        if (signalrService.isConnected()) {
          console.log('ChatStore: Sending via SignalR...');
          await signalrService.sendMessage(receiverId, content);
          console.log('ChatStore: Message sent via SignalR');
        } else {
          console.warn('ChatStore: SignalR not connected, message only saved to database');
        }
        
        // Add to local state
        console.log('ChatStore: Adding message to local state');
        get().addMessage(message);
        console.log('ChatStore: Message added to state successfully');
        
        // Success - break out of retry loop
        return;
      } catch (error) {
        lastError = error instanceof Error ? error : new Error('Unknown error');
        console.error(`ChatStore: Attempt ${attempt} failed:`, lastError.message);
        
        // If it's a duplicate key error and not the last attempt, try again
        if (lastError.message.includes('duplicate key') && attempt < maxRetries) {
          console.log(`ChatStore: Retrying due to duplicate key error (attempt ${attempt + 1}/${maxRetries})`);
          continue;
        }
        
        // If it's not a duplicate key error or it's the last attempt, throw the error
        if (attempt === maxRetries) {
          break;
        }
      }
    }

    // If we get here, all retries failed
    const errorMessage = lastError?.message || 'Failed to send message';
    console.error('ChatStore: All retry attempts failed. Final error:', lastError);
    set({ error: errorMessage });
    throw lastError || new Error(errorMessage);
  },

  sendGroupMessage: async (groupId: number, content: string) => {
    try {
      console.log('Sending group message to:', groupId, 'content:', content);
      
      const API_BASE_URL = import.meta.env.VITE_API_URL || '/api';
      const token = localStorage.getItem('token');
      const response = await fetch(`${API_BASE_URL}/Groups/${groupId}/messages`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`,
          'ngrok-skip-browser-warning': 'true'
        },
        body: JSON.stringify(content)
      });
      
      if (!response.ok) {
        const errorText = await response.text();
        let errorMessage = 'Failed to send group message';
        try {
          const errorJson = JSON.parse(errorText);
          errorMessage = errorJson.message || errorMessage;
        } catch {
          errorMessage = errorText || errorMessage;
        }
        throw new Error(errorMessage);
      }
      
      const groupMessage = await response.json();
      
      // Convert to Message format and add to state
      const message = {
        id: groupMessage.id,
        senderId: groupMessage.senderId,
        receiverId: groupId,
        senderUsername: groupMessage.senderUsername,
        receiverUsername: '',
        content: groupMessage.content,
        isSystemMessage: groupMessage.isSystemMessage || false,
        isRead: true,
        sentAt: groupMessage.sentAt,
        readAt: null
      };
      
      get().addMessage(message);
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to send group message';
      set({ error: errorMessage });
      console.error('Failed to send group message:', error);
      throw error;
    }
  },

  setSelectedUser: (user) => {
    set({ selectedUser: user, selectedGroup: null, messages: [] });
    if (user) {
      get().loadMessages(user.id);
    }
  },

  setSelectedGroup: (group) => {
    set({ selectedGroup: group, selectedUser: null, messages: [] });
    if (group) {
      get().loadGroupMessages(group.id);
    }
  },

  addMessage: (message) => {
    console.log('ChatStore: Adding message to state', message);
    set((state) => {
      // Check if message already exists (prevent duplicates)
      const messageExists = state.messages.some(m => m.id === message.id);
      if (messageExists) {
        console.log('ChatStore: Message already exists, skipping');
        return state;
      }
      
      const newMessages = [...state.messages, message];
      console.log('ChatStore: New messages count:', newMessages.length);
      return { messages: newMessages };
    });
  },

  updateUserStatus: (userId, isOnline) => {
    set((state) => ({
      users: state.users.map((u) =>
        u.id === userId ? { ...u, isOnline, lastSeen: new Date().toISOString() } : u
      ),
    }));
  },

  initializeSignalR: () => {
    console.log('ChatStore: Initializing SignalR listeners');
    
    // First, remove any existing listeners to prevent duplicates
    signalrService.offReceiveMessage();
    signalrService.offUserOnline();
    signalrService.offUserOffline();
    
    // Listen for incoming messages
    signalrService.onReceiveMessage((message) => {
      console.log('SignalR: Received message', message);
      const { selectedUser, selectedGroup } = get();
      
      // For direct messages
      if (selectedUser) {
        const isMessageForSelectedChat = 
          (message.senderId === selectedUser.id) || // Message from selected user
          (message.receiverId === selectedUser.id); // Message to selected user
        
        if (isMessageForSelectedChat) {
          console.log('SignalR: Adding direct message to state');
          get().addMessage(message);
        } else {
          console.log('SignalR: Message not for selected user, ignoring');
        }
      }
      
      // For group messages (if receiverId matches selectedGroup)
      if (selectedGroup && message.receiverId === selectedGroup.id) {
        console.log('SignalR: Adding group message to state');
        get().addMessage(message);
      }
    });

    // Listen for user online status
    signalrService.onUserOnline((userId) => {
      console.log('SignalR: User online', userId);
      get().updateUserStatus(userId, true);
    });

    // Listen for user offline status
    signalrService.onUserOffline((userId) => {
      console.log('SignalR: User offline', userId);
      get().updateUserStatus(userId, false);
    });
    
    console.log('ChatStore: SignalR listeners initialized');
  },

  cleanup: () => {
    signalrService.offReceiveMessage();
    signalrService.offUserOnline();
    signalrService.offUserOffline();
  },
}),
    {
      name: 'chat-storage',
      partialize: (state) => ({
        users: state.users,
        groups: state.groups,
      }),
    }
  )
);

