export interface Call {
  id: number;
  callerId: number;
  receiverId: number;
  callerUsername: string;
  receiverUsername: string;
  callType: 'voice' | 'video';
  status: 'missed' | 'completed' | 'rejected';
  startedAt: string;
  endedAt?: string | null;
  duration?: number | null; // in seconds
}

export interface CreateCallDto {
  receiverId: number;
  callType: 'voice' | 'video';
  status: 'missed' | 'completed' | 'rejected';
  duration?: number;
}
