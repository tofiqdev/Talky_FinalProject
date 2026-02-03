import * as signalR from '@microsoft/signalr';
import type { Message } from '../types/message';

class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private reconnectAttempts = 0;
  private maxReconnectAttempts = 5;

  async connect(token: string) {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      console.log('SignalR already connected');
      return;
    }

    console.log('SignalR: Starting connection with token:', token ? 'Token exists' : 'No token');

    // Use environment variable for production, localhost for development
    const hubUrl = import.meta.env.VITE_API_URL 
      ? `${import.meta.env.VITE_API_URL.replace('/api', '')}/chatHub`
      : 'http://localhost:5282/chatHub';

    console.log('SignalR: Connecting to:', hubUrl);

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => {
          console.log('SignalR: Providing access token');
          return token;
        },
        headers: {
          'ngrok-skip-browser-warning': 'true', // Skip ngrok warning page
        },
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          if (retryContext.previousRetryCount >= this.maxReconnectAttempts) {
            return null; // Stop reconnecting
          }
          return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000);
        },
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    // Handle reconnection events
    this.connection.onreconnecting((error) => {
      console.log('SignalR reconnecting...', error);
      this.reconnectAttempts++;
    });

    this.connection.onreconnected((connectionId) => {
      console.log('SignalR reconnected:', connectionId);
      this.reconnectAttempts = 0;
    });

    this.connection.onclose((error) => {
      console.log('SignalR connection closed', error);
    });

    try {
      await this.connection.start();
      console.log('SignalR Connected successfully! Connection ID:', this.connection.connectionId);
    } catch (error) {
      console.error('SignalR Connection Error:', error);
      throw error;
    }
  }

  async disconnect() {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
      console.log('SignalR Disconnected');
    }
  }

  isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected;
  }

  // Event listeners
  onReceiveMessage(callback: (message: Message) => void) {
    this.connection?.on('ReceiveMessage', (messageData: any) => {
      console.log('SignalR: Raw message received:', messageData);
      
      // Convert the received data to Message format
      const message: Message = {
        id: messageData.id,
        senderId: messageData.senderId,
        receiverId: messageData.receiverId,
        senderUsername: messageData.senderUsername || '',
        receiverUsername: messageData.receiverUsername || '',
        content: messageData.content,
        sentAt: messageData.sentAt,
        isRead: messageData.isRead || false,
        readAt: messageData.readAt || null,
        isSystemMessage: messageData.isSystemMessage || false
      };
      
      console.log('SignalR: Converted message:', message);
      callback(message);
    });
  }

  onUserOnline(callback: (userId: number) => void) {
    this.connection?.on('UserOnline', callback);
  }

  onUserOffline(callback: (userId: number) => void) {
    this.connection?.on('UserOffline', callback);
  }

  onTypingIndicator(callback: (userId: number, isTyping: boolean) => void) {
    this.connection?.on('TypingIndicator', callback);
  }

  onMessageRead(callback: (messageId: number) => void) {
    this.connection?.on('MessageRead', callback);
  }

  // Actions
  async sendMessage(receiverId: number, content: string) {
    console.log('SignalR: sendMessage called', { receiverId, content, isConnected: this.isConnected() });
    
    if (!this.isConnected()) {
      console.error('SignalR: Cannot send message - not connected');
      throw new Error('SignalR not connected');
    }
    
    try {
      console.log('SignalR: Invoking SendMessage on hub...');
      await this.connection?.invoke('SendMessage', receiverId, content);
      console.log('SignalR: SendMessage invoked successfully');
    } catch (error) {
      console.error('SignalR: SendMessage failed', error);
      throw error;
    }
  }

  async sendTypingIndicator(receiverId: number, isTyping: boolean) {
    if (!this.isConnected()) return;
    await this.connection?.invoke('TypingIndicator', receiverId, isTyping);
  }

  async markAsRead(messageId: number) {
    if (!this.isConnected()) return;
    await this.connection?.invoke('MarkAsRead', messageId);
  }

  // Remove event listeners
  offReceiveMessage() {
    this.connection?.off('ReceiveMessage');
  }

  offUserOnline() {
    this.connection?.off('UserOnline');
  }

  offUserOffline() {
    this.connection?.off('UserOffline');
  }

  offTypingIndicator() {
    this.connection?.off('TypingIndicator');
  }

  offMessageRead() {
    this.connection?.off('MessageRead');
  }
}

export const signalrService = new SignalRService();

