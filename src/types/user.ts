export interface User {
  id: string;
  username: string;
  email: string;
  isOnline: boolean;
  lastSeen?: Date;
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
