import { create } from 'zustand';
import type { Message } from '../types/message';
import type { User } from '../types/user';
import type { Group } from '../types/group';
import { usersApi, messagesApi } from '../services/apiService';
import { signalrService } from '../services/signalrService';

interface ChatState {
  messages: Message[];
  users: User[];
  groups: Group[];
  selectedUser: User | null;
  selectedGroup: Group | null;
  isLoading: boolean;
  error: string | null;
  
  // Actions
  loadUsers: () => Promise<void>;
  loadGroups: () => Promise<void>;
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

export const useChatStore = create<ChatState>((set, get) => ({
  messages: [],
  users: [],
  groups: [],
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
      const token = localStorage.getItem('token');
      const response = await fetch('/api/groups', {
        headers: {
          'Authorization': `Bearer ${token}`
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
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/groups/${groupId}/messages`, {
        headers: {
          'Authorization': `Bearer ${token}`
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
    try {
      console.log('Sending message to:', receiverId, 'content:', content);
      
      // Send via SignalR for real-time delivery
      if (signalrService.isConnected()) {
        console.log('SignalR connected, sending via SignalR');
        await signalrService.sendMessage(receiverId, content);
        // Note: Backend will send ReceiveMessage event back to both sender and receiver
      } else {
        console.log('SignalR not connected, using REST API fallback');
        // Fallback to REST API if SignalR not connected
        const message = await messagesApi.sendMessage(receiverId, content);
        get().addMessage(message);
      }
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to send message';
      set({ error: errorMessage });
      console.error('Failed to send message:', error);
      throw error;
    }
  },

  sendGroupMessage: async (groupId: number, content: string) => {
    try {
      console.log('Sending group message to:', groupId, 'content:', content);
      
      const token = localStorage.getItem('token');
      const response = await fetch(`/api/groups/${groupId}/messages`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        },
        body: JSON.stringify(content)
      });
      
      if (!response.ok) throw new Error('Failed to send group message');
      
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
      const { selectedUser } = get();
      const currentUserId = message.senderId; // This will be checked against auth user
      
      // Add message if:
      // 1. We have a selected user AND
      // 2. The message is between current user and selected user
      if (selectedUser) {
        const isMessageForSelectedChat = 
          (message.senderId === selectedUser.id) || // Message from selected user
          (message.receiverId === selectedUser.id); // Message to selected user
        
        if (isMessageForSelectedChat) {
          console.log('SignalR: Adding message to state');
          get().addMessage(message);
        } else {
          console.log('SignalR: Message not for selected user, ignoring');
        }
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
}));

