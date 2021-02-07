import { Component } from '@angular/core';
import { SocketsService } from 'src/services/sockets.service';
// import { SocketsService } from 'src/services/socket-service.service';

@Component({
  selector: 'app-message-test',
  templateUrl: './message-test.component.html',
  styleUrls: ['./message-test.component.scss']
})
export class MessageTestComponent {
  constructor(private socket: SocketsService) {}

  sendMessage(): void {
    this.socket.sendMessage('Hello from client!');
  }
}
