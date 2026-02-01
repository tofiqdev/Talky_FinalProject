export interface MovieRoom {
  id: number;
  name: string;
  description?: string;
  youTubeUrl: string;
  youTubeVideoId: string;
  createdById: number;
  createdByUsername: string;
  createdByAvatar?: string;
  isActive: boolean;
  currentTime: number;
  isPlaying: boolean;
  participantCount: number;
  createdAt: string;
  participants: MovieRoomParticipant[];
}

export interface MovieRoomParticipant {
  id: number;
  userId: number;
  username: string;
  avatar?: string;
  isOnline: boolean;
  joinedAt: string;
}

export interface MovieRoomMessage {
  id: number;
  movieRoomId: number;
  senderId: number;
  senderUsername: string;
  senderAvatar?: string;
  content: string;
  sentAt: string;
}

export interface CreateMovieRoomData {
  name: string;
  description?: string;
  youTubeUrl: string;
}
