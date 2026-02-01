import type { User, AuthResponse } from '../types/user';
import type { Message } from '../types/message';

// Use environment variable for production, proxy for development
const API_BASE_URL = import.meta.env.VITE_API_URL || '/api';

// Helper function to get auth token
const getAuthToken = (): string | null => {
  return localStorage.getItem('token');
};

// Helper function to create headers
const createHeaders = (includeAuth: boolean = false): HeadersInit => {
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
    'ngrok-skip-browser-warning': 'true', // Skip ngrok warning page
  };

  if (includeAuth) {
    const token = getAuthToken();
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }
  }

  return headers;
};

// Auth API
export const authApi = {
  register: async (username: string, email: string, password: string): Promise<AuthResponse> => {
    const response = await fetch(`${API_BASE_URL}/auth/register`, {
      method: 'POST',
      headers: createHeaders(),
      body: JSON.stringify({ username, email, password }),
    });

    if (!response.ok) {
      let errorMessage = 'Registration failed';
      try {
        const text = await response.text();
        if (text) {
          try {
            const error = JSON.parse(text);
            errorMessage = error.message || errorMessage;
          } catch {
            errorMessage = text;
          }
        }
      } catch {
        // Ignore parsing errors
      }
      throw new Error(errorMessage);
    }

    return response.json();
  },

  login: async (email: string, password: string): Promise<AuthResponse> => {
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: createHeaders(),
      body: JSON.stringify({ emailOrUsername: email, password }),
    });

    if (!response.ok) {
      let errorMessage = 'Login failed';
      try {
        const text = await response.text();
        if (text) {
          try {
            const error = JSON.parse(text);
            errorMessage = error.message || errorMessage;
          } catch {
            errorMessage = text;
          }
        }
      } catch {
        // Ignore parsing errors
      }
      throw new Error(errorMessage);
    }

    return response.json();
  },

  getCurrentUser: async (): Promise<User> => {
    const response = await fetch(`${API_BASE_URL}/auth/me`, {
      headers: createHeaders(true),
    });

    if (!response.ok) {
      throw new Error('Failed to get current user');
    }

    return response.json();
  },
};

// Users API
export const usersApi = {
  getAllUsers: async (): Promise<User[]> => {
    const response = await fetch(`${API_BASE_URL}/users`, {
      headers: createHeaders(true),
    });

    if (!response.ok) {
      throw new Error('Failed to fetch users');
    }

    return response.json();
  },

  getUserById: async (userId: number): Promise<User> => {
    const response = await fetch(`${API_BASE_URL}/users/${userId}`, {
      headers: createHeaders(true),
    });

    if (!response.ok) {
      throw new Error('Failed to fetch user');
    }

    return response.json();
  },

  getUserByUsername: async (username: string): Promise<User> => {
    const response = await fetch(`${API_BASE_URL}/users/username/${username}`, {
      headers: createHeaders(true),
    });

    if (!response.ok) {
      throw new Error('User not found');
    }

    return response.json();
  },

  searchUsers: async (searchTerm: string): Promise<User[]> => {
    const response = await fetch(`${API_BASE_URL}/users/search?q=${encodeURIComponent(searchTerm)}`, {
      headers: createHeaders(true),
    });

    if (!response.ok) {
      throw new Error('Failed to search users');
    }

    return response.json();
  },

  updateStatus: async (isOnline: boolean): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/users/status`, {
      method: 'PUT',
      headers: createHeaders(true),
      body: JSON.stringify(isOnline),
    });

    if (!response.ok) {
      throw new Error('Failed to update status');
    }
    
    // Don't try to parse JSON if response is empty (204 No Content)
    if (response.status === 204 || response.headers.get('content-length') === '0') {
      return;
    }
  },
};

// Messages API
export const messagesApi = {
  getMessages: async (userId: number): Promise<Message[]> => {
    const response = await fetch(`${API_BASE_URL}/messages/${userId}`, {
      headers: createHeaders(true),
    });

    if (!response.ok) {
      throw new Error('Failed to fetch messages');
    }

    return response.json();
  },

  sendMessage: async (receiverId: number, content: string): Promise<Message> => {
    const response = await fetch(`${API_BASE_URL}/messages`, {
      method: 'POST',
      headers: createHeaders(true),
      body: JSON.stringify({ receiverId, content }),
    });

    if (!response.ok) {
      throw new Error('Failed to send message');
    }

    return response.json();
  },

  markAsRead: async (messageId: number): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}/messages/${messageId}/read`, {
      method: 'PUT',
      headers: createHeaders(true),
    });

    if (!response.ok) {
      throw new Error('Failed to mark message as read');
    }
    
    // Don't try to parse JSON if response is empty (204 No Content)
    if (response.status === 204 || response.headers.get('content-length') === '0') {
      return;
    }
  },
};

// Calls API
export const callsApi = {
  getCalls: async () => {
    const response = await fetch(`${API_BASE_URL}/calls`, {
      headers: createHeaders(true),
    });

    if (!response.ok) {
      throw new Error('Failed to fetch calls');
    }

    return response.json();
  },

  createCall: async (receiverId: number, callType: 'voice' | 'video', status: string, duration?: number) => {
    const response = await fetch(`${API_BASE_URL}/calls`, {
      method: 'POST',
      headers: createHeaders(true),
      body: JSON.stringify({ receiverId, callType, status, duration }),
    });

    if (!response.ok) {
      throw new Error('Failed to create call');
    }

    return response.json();
  },
};
