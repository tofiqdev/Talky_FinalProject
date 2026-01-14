export interface Group {
  id: number;
  name: string;
  description?: string;
  avatar?: string;
  createdById: number;
  createdByUsername: string;
  createdAt: string;
  isMutedForAll: boolean;
  memberCount: number;
  members: GroupMember[];
}

export interface GroupMember {
  id: number;
  userId: number;
  username: string;
  avatar?: string;
  isAdmin: boolean;
  isMuted: boolean;
  isOnline: boolean;
  joinedAt: string;
}

export interface GroupMessage {
  id: number;
  groupId: number;
  senderId: number;
  senderUsername: string;
  senderAvatar?: string;
  content: string;
  isSystemMessage: boolean;
  sentAt: string;
}
