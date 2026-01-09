export interface Message {
  id: string;
  senderId: string;
  receiverId: string;
  content: string;
  sentAt: Date;
  isRead: boolean;
}

export interface SendMessageDto {
  receiverId: string;
  content: string;
}
