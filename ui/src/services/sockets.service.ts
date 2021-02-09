import { Injectable } from '@angular/core';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SocketsService {
  socket: WebSocket;
  url = '';
  constructor() {
    // this.socket = new WebSocket('ws://localhost:60109/ws');
    this.url = environment.socketServiceUrl;
    this.socket = new WebSocket(this.url);
    this.socket.onmessage = this.onMessage;
    this.socket.onerror = this.onError;
    this.socket.onopen = this.onOpen;
    this.socket.onclose = this.onClose;
  }

  onOpen(event: Event): void {
    console.log(event);
  }

  onMessage(message: MessageEvent<unknown>): void {
    console.log('Message: ' + message.data);
  }

  onError(event: Event): void {
    console.error('error in my socket', event);
  }

  sendMessage(message: string): void {
    this.socket.send(message);
  }

  onClose(event: CloseEvent): void {
    console.log(event);
  }
}
