import { create } from 'zustand';
import type { Message } from '../types/message';
import type { User } from '../types/user';

interface ChatState {
  messages: Message[];
  users: User[];
  selectedUser: User | null;
  addMessage: (message: Message) => void;
  setMessages: (messages: Message[]) => void;
  setUsers: (users: User[]) => void;
  setSelectedUser: (user: User | null) => void;
  updateUserStatus: (userId: string, isOnline: boolean) => void;
}

export const useChatStore = create<ChatState>((set) => ({
  messages: [],
  users: [],
  selectedUser: null,
  addMessage: (message) => set((state) => ({ messages: [...state.messages, message] })),
  setMessages: (messages) => set({ messages }),
  setUsers: (users) => set({ users }),
  setSelectedUser: (user) => set({ selectedUser: user }),
  updateUserStatus: (userId, isOnline) =>
    set((state) => ({
      users: state.users.map((u) => (u.id === userId ? { ...u, isOnline } : u)),
    })),
}));
