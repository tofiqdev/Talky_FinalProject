import * as signalR from '@microsoft/signalr';
import type { Message } from '../types/message';

class SignalRService {
  private connection: signalR.HubConnection | null = null;

  async connect(token: string) {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5000/chatHub', {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build();

    await this.connection.start();
    console.log('SignalR Connected');
  }

  disconnect() {
    this.connection?.stop();
  }

  onReceiveMessage(callback: (message: Message) => void) {
    this.connection?.on('ReceiveMessage', callback);
  }

  onUserOnline(callback: (userId: string) => void) {
    this.connection?.on('UserOnline', callback);
  }

  onUserOffline(callback: (userId: string) => void) {
    this.connection?.on('UserOffline', callback);
  }

  async sendMessage(receiverId: string, content: string) {
    await this.connection?.invoke('SendMessage', receiverId, content);
  }
}

export const signalrService = new SignalRService();
