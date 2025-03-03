import { Injectable, Logger } from '@nestjs/common';
import { RawData, Server, WebSocket } from 'ws';
import { WebSocketServer } from '@nestjs/websockets';

@Injectable()
export class SocketService {
  @WebSocketServer() server: Server;
  private clients = new Map<string, WebSocket>();
  private logger = new Logger('SocketService');

  // Called when a new client connects.
  handleConnection(client: WebSocket, clientId: string): void {
    // Notify all other clients about the new connection.
    this.clients.forEach((ws) => {
      if (ws.readyState === WebSocket.OPEN) {
        const message = { type: 'newConnection', clientId };
        ws.send(JSON.stringify(message));
      }
    });

    // Store the client.
    this.clients.set(clientId, client);

    // Send confirmation to the connecting client.
    client.send(JSON.stringify({ type: 'connection', clientId }));

    // Listen for messages from this client.
    client.on('message', (payload: RawData) => {
      this.handleMessage(payload);
    });

    // Optionally, handle errors on the client.
    client.on('error', (err) => {
      this.logger.error(`Error on client ${clientId}: ${err.message}`);
    });
  }

  handleMessage(payload: RawData): void {
    // Convert the raw data to a string, then parse JSON.
    let messageObj: { type: string; data: any; clientId: string };
    try {
      messageObj = JSON.parse(payload.toString());
    } catch (error) {
      this.logger.error('Failed to parse message payload', error);
      return;
    }

    const { clientId, data, type } = messageObj;

    switch (type) {
      case 'message':
        this.logger.log(`Received message from ${clientId}: ${data}`);
        // Broadcast message to all clients except the sender.
        this.clients.forEach((ws, id) => {
          if (ws.readyState === WebSocket.OPEN && id !== clientId) {
            ws.send(JSON.stringify({ type: 'message', clientId, data }));
          }
        });
        break;
      default:
        // Send error back to the originating client.
        const sender = this.clients.get(clientId);
        const errorMessage = { type: 'error', message: 'Invalid event' };
        if (sender && sender.readyState === WebSocket.OPEN) {
          sender.send(JSON.stringify(errorMessage));
        }
        break;
    }
  }

  handleDisconnect(client: WebSocket) {
    const clientId = Array.from(this.clients.entries()).find(
      ([, value]) => value === client,
    )?.[0];

    this.clients.forEach((client) => {
      const message = {
        type: 'disconnection',
        clientId,
      };
      if (client.readyState === WebSocket.OPEN) {
        client.send(JSON.stringify(message));
      }
    });

    this.logger.log(`Client ${clientId} disconnected`);

    this.clients.delete(clientId);
  }
}
