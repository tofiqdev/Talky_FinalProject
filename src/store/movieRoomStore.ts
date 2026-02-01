import { create } from 'zustand';
import type { MovieRoom, MovieRoomMessage } from '../types/movieRoom';
import { movieroomsApi } from '../services/apiService';

interface MovieRoomState {
  rooms: MovieRoom[];
  selectedRoom: MovieRoom | null;
  messages: MovieRoomMessage[];
  isLoading: boolean;
  error: string | null;

  // Actions
  loadRooms: () => Promise<void>;
  loadActiveRooms: () => Promise<void>;
  loadRoom: (roomId: number) => Promise<void>;
  loadMessages: (roomId: number) => Promise<void>;
  createRoom: (name: string, description: string, youTubeUrl: string) => Promise<void>;
  joinRoom: (roomId: number) => Promise<void>;
  leaveRoom: (roomId: number) => Promise<void>;
  sendMessage: (roomId: number, content: string) => Promise<void>;
  updatePlayback: (roomId: number, currentTime: number, isPlaying: boolean) => Promise<void>;
  setSelectedRoom: (room: MovieRoom | null) => void;
  addMessage: (message: MovieRoomMessage) => void;
}

export const useMovieRoomStore = create<MovieRoomState>((set, get) => ({
  rooms: [],
  selectedRoom: null,
  messages: [],
  isLoading: false,
  error: null,

  loadRooms: async () => {
    set({ isLoading: true, error: null });
    try {
      const data = await movieroomsApi.get('/movierooms');
      set({ rooms: data, isLoading: false });
    } catch (error: any) {
      set({ error: error.message, isLoading: false });
    }
  },

  loadActiveRooms: async () => {
    set({ isLoading: true, error: null });
    try {
      const data = await movieroomsApi.get('/movierooms/active');
      set({ rooms: data, isLoading: false });
    } catch (error: any) {
      set({ error: error.message, isLoading: false });
    }
  },

  loadRoom: async (roomId: number) => {
    set({ isLoading: true, error: null });
    try {
      const data = await movieroomsApi.get(`/movierooms/${roomId}`);
      set({ selectedRoom: data, isLoading: false });
    } catch (error: any) {
      set({ error: error.message, isLoading: false });
    }
  },

  loadMessages: async (roomId: number) => {
    set({ isLoading: true, error: null });
    try {
      const data = await movieroomsApi.get(`/movierooms/${roomId}/messages`);
      set({ messages: data, isLoading: false });
    } catch (error: any) {
      set({ error: error.message, isLoading: false });
    }
  },

  createRoom: async (name: string, description: string, youTubeUrl: string) => {
    set({ isLoading: true, error: null });
    try {
      await movieroomsApi.post('/movierooms', { name, description, youTubeUrl });
      await get().loadActiveRooms();
      set({ isLoading: false });
    } catch (error: any) {
      set({ error: error.message, isLoading: false });
      throw error;
    }
  },

  joinRoom: async (roomId: number) => {
    try {
      await movieroomsApi.post(`/movierooms/${roomId}/join`, {});
      await get().loadRoom(roomId);
    } catch (error: any) {
      set({ error: error.message });
      throw error;
    }
  },

  leaveRoom: async (roomId: number) => {
    try {
      await movieroomsApi.post(`/movierooms/${roomId}/leave`, {});
      set({ selectedRoom: null, messages: [] });
    } catch (error: any) {
      set({ error: error.message });
      throw error;
    }
  },

  sendMessage: async (roomId: number, content: string) => {
    try {
      await movieroomsApi.post(`/movierooms/${roomId}/messages`, { content });
      await get().loadMessages(roomId);
    } catch (error: any) {
      set({ error: error.message });
      throw error;
    }
  },

  updatePlayback: async (roomId: number, currentTime: number, isPlaying: boolean) => {
    try {
      await movieroomsApi.put(`/movierooms/${roomId}/playback`, { currentTime, isPlaying });
    } catch (error: any) {
      console.error('Failed to update playback:', error);
    }
  },

  setSelectedRoom: (room: MovieRoom | null) => {
    set({ selectedRoom: room, messages: [] });
    if (room) {
      get().loadMessages(room.id);
    }
  },

  addMessage: (message: MovieRoomMessage) => {
    set((state) => ({
      messages: [...state.messages, message]
    }));
  }
}));
