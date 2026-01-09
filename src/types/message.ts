export interface Message {
  id: number;
  senderId: number;
  receiverId: number;
  senderUsername: string;
  receiverUsername: string;
  content: string;
  sentAt: string;
  isRead: boolean;
  readAt?: string | null;
}

export interface SendMessageDto {
  receiverId: number;
  content: string;
}

