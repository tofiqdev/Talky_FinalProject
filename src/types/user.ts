export interface User {
  id: number;
  username: string;
  email: string;
  avatar?: string | null;
  bio?: string | null;
  isOnline: boolean;
  lastSeen?: string | null;
  createdAt: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  username: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

