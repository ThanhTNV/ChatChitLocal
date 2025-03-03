import {
  OnGatewayConnection,
  OnGatewayDisconnect,
  WebSocketGateway,
} from '@nestjs/websockets';
import { SocketService } from './socket.service';
import { WebSocket } from 'ws';
import { IncomingMessage } from 'http';
import { Logger } from "@nestjs/common";

@WebSocketGateway(8080, {
  cors: {
    origin: true,
    methods: ['GET', 'POST'],
  },
})
export class SocketGateway implements OnGatewayConnection, OnGatewayDisconnect {
  private readonly logger = new Logger(SocketGateway.name);
  constructor(private readonly socketService: SocketService) {}

  handleConnection(client: WebSocket, ...args: any[]): any {
    const req = args[0] as IncomingMessage;
    const clientId = req.headers['sec-websocket-key'] as string;

    this.logger.log('New connection from client ' + clientId);
    this.socketService.handleConnection(client, clientId);
  }

  handleDisconnect(client: WebSocket): any {
    this.socketService.handleDisconnect(client);
  }
}
