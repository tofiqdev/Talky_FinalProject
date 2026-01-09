import { create } from 'zustand';
import type { User } from '../types/user';
import { authApi } from '../services/apiService';
import { signalrService } from '../services/signalrService';

interface AuthState {
  user: User | null;
  token: string | null;
  isLoading: boolean;
  error: string | null;
  setAuth: (user: User, token: string) => void;
  login: (email: string, password: string) => Promise<void>;
  register: (username: string, email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  initAuth: () => Promise<void>;
}

export const useAuthStore = create<AuthState>((set, get) => ({
  user: null,
  token: localStorage.getItem('token'),
  isLoading: false,
  error: null,

  setAuth: (user, token) => {
    localStorage.setItem('token', token);
    set({ user, token, error: null });

    // Connect to SignalR
    signalrService.connect(token).catch((error) => {
      console.error('Failed to connect to SignalR:', error);
    });
  },

  login: async (email, password) => {
    set({ isLoading: true, error: null });
    try {
      const response = await authApi.login(email, password);
      get().setAuth(response.user, response.token);
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Login failed';
      set({ error: errorMessage });
      throw error;
    } finally {
      set({ isLoading: false });
    }
  },

  register: async (username, email, password) => {
    set({ isLoading: true, error: null });
    try {
      const response = await authApi.register(username, email, password);
      get().setAuth(response.user, response.token);
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Registration failed';
      set({ error: errorMessage });
      throw error;
    } finally {
      set({ isLoading: false });
    }
  },

  logout: async () => {
    await signalrService.disconnect();
    localStorage.removeItem('token');
    set({ user: null, token: null, error: null });
  },

  initAuth: async () => {
    const token = localStorage.getItem('token');
    if (!token) return;

    set({ isLoading: true });
    try {
      const user = await authApi.getCurrentUser();
      set({ user, token });
      await signalrService.connect(token);
    } catch (error) {
      console.error('Failed to initialize auth:', error);
      localStorage.removeItem('token');
      set({ user: null, token: null });
    } finally {
      set({ isLoading: false });
    }
  },
}));

