import { Component } from '@angular/core';
// import { SocketsService } from 'src/services/socket-service.service';

@Component({
  selector: 'app-message-test',
  templateUrl: './message-test.component.html',
  styleUrls: ['./message-test.component.scss']
})
export class MessageTestComponent {
  socket: WebSocket;

  constructor() {
    // this.socket = new WebSocket('ws://localhost:60109/ws');
    this.socket = new WebSocket('wss://localhost:44394/ws');
    this.socket.onmessage = this.onMessage;
    this.socket.onerror = this.onError;
    this.socket.onopen = this.onOpen;
    this.socket.onclose = this.onClose;
  }

  onOpen(event: Event): void {
    console.log(event);
  }

  onMessage(message: MessageEvent<unknown>): void {
    console.log('Message: ' + message);
  }

  onError(event: Event): void {
    console.error('error in my socket', event);
  }

  sendMessage(): void {
    this.socket.send('Hello from client!');
  }

  onClose(event: CloseEvent): void {
    console.log(event);
  }
}
