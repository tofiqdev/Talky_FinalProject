import * as signalR from '@microsoft/signalr';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5135';

class MovieRoomSignalRService {
  private connection: signalR.HubConnection | null = null;

  async connect(token: string): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_BASE_URL}/movieRoomHub`, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    try {
      await this.connection.start();
      console.log('MovieRoom SignalR Connected');
    } catch (err) {
      console.error('MovieRoom SignalR Connection Error:', err);
      throw err;
    }
  }

  async joinRoom(roomId: number): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      throw new Error('SignalR not connected');
    }

    await this.connection.invoke('JoinMovieRoom', roomId);
    console.log(`Joined movie room: ${roomId}`);
  }

  async leaveRoom(roomId: number): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      return;
    }

    await this.connection.invoke('LeaveMovieRoom', roomId);
    console.log(`Left movie room: ${roomId}`);
  }

  async syncPlayback(roomId: number, currentTime: number, isPlaying: boolean): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      console.error('❌ Cannot sync - SignalR not connected. State:', this.connection?.state);
      return;
    }

    console.log('📤 Sending sync to server:', { roomId, currentTime, isPlaying });
    
    try {
      await this.connection.invoke('SyncPlayback', roomId, currentTime, isPlaying);
      console.log('✅ Sync invoke completed');
    } catch (error) {
      console.error('❌ Sync invoke failed:', error);
      throw error;
    }
  }

  onPlaybackSync(callback: (data: { currentTime: number; isPlaying: boolean; timestamp: string }) => void): void {
    if (!this.connection) return;
    this.connection.on('PlaybackSync', callback);
  }

  onUserJoined(callback: (userId: number) => void): void {
    if (!this.connection) return;
    this.connection.on('UserJoined', callback);
  }

  onUserLeft(callback: (userId: number) => void): void {
    if (!this.connection) return;
    this.connection.on('UserLeft', callback);
  }

  onReceiveRoomMessage(callback: (data: { senderId: number; content: string; sentAt: string }) => void): void {
    if (!this.connection) return;
    this.connection.on('ReceiveRoomMessage', callback);
  }

  removeAllListeners(): void {
    if (!this.connection) return;
    this.connection.off('PlaybackSync');
    this.connection.off('UserJoined');
    this.connection.off('UserLeft');
    this.connection.off('ReceiveRoomMessage');
  }

  async disconnect(): Promise<void> {
    if (this.connection) {
      this.removeAllListeners();
      await this.connection.stop();
      this.connection = null;
      console.log('MovieRoom SignalR Disconnected');
    }
  }

  getConnectionState(): signalR.HubConnectionState | null {
    return this.connection?.state || null;
  }
}

export const movieRoomSignalrService = new MovieRoomSignalRService();
