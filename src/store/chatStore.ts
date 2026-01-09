import { create } from 'zustand';
import type { Message } from '../types/message';
import type { User } from '../types/user';
import { usersApi, messagesApi } from '../services/apiService';
import { signalrService } from '../services/signalrService';

interface ChatState {
  messages: Message[];
  users: User[];
  selectedUser: User | null;
  isLoading: boolean;
  error: string | null;
  
  // Actions
  loadUsers: () => Promise<void>;
  loadMessages: (userId: number) => Promise<void>;
  sendMessage: (receiverId: number, content: string) => Promise<void>;
  setSelectedUser: (user: User | null) => void;
  addMessage: (message: Message) => void;
  updateUserStatus: (userId: number, isOnline: boolean) => void;
  initializeSignalR: () => void;
  cleanup: () => void;
}

export const useChatStore = create<ChatState>((set, get) => ({
  messages: [],
  users: [],
  selectedUser: null,
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

  sendMessage: async (receiverId: number, content: string) => {
    try {
      // Send via SignalR for real-time delivery
      if (signalrService.isConnected()) {
        await signalrService.sendMessage(receiverId, content);
      } else {
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

  setSelectedUser: (user) => {
    set({ selectedUser: user, messages: [] });
    if (user) {
      get().loadMessages(user.id);
    }
  },

  addMessage: (message) => {
    set((state) => ({
      messages: [...state.messages, message],
    }));
  },

  updateUserStatus: (userId, isOnline) => {
    set((state) => ({
      users: state.users.map((u) =>
        u.id === userId ? { ...u, isOnline, lastSeen: new Date().toISOString() } : u
      ),
    }));
  },

  initializeSignalR: () => {
    // Listen for incoming messages
    signalrService.onReceiveMessage((message) => {
      get().addMessage(message);
    });

    // Listen for user online status
    signalrService.onUserOnline((userId) => {
      get().updateUserStatus(userId, true);
    });

    // Listen for user offline status
    signalrService.onUserOffline((userId) => {
      get().updateUserStatus(userId, false);
    });
  },

  cleanup: () => {
    signalrService.offReceiveMessage();
    signalrService.offUserOnline();
    signalrService.offUserOffline();
  },
}));

