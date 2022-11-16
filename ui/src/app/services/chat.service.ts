import { Injectable, OnDestroy } from '@angular/core';
import { Router, UrlSerializer } from '@angular/router';
import { environment } from 'src/environments/environment';
import { ChatMessageDto } from '../dto/chat/chatMessageDto';
import { JoinedChatDto } from '../dto/chat/joinedChatDto';
import { LeftChatDto } from '../dto/chat/leftChatDto';
import { AppStateService } from '../state/app-state.service';

@Injectable({
  providedIn: 'root'
})
export class ChatService implements OnDestroy {
  socket: WebSocket | undefined;
  url: string = '';

  constructor(
    private appState: AppStateService,
    private router: Router,
    private serializer: UrlSerializer
  ) {}

  connect() {
    this.cleanup();

    this.url = environment.chatServiceUrl;
    if (environment.production) {
      this.url = origin.replace('https://', 'wss://') + '/chat'; // TODO: is this correct?
    }

    const user = this.appState.user.getValue();
    const userId = user ? user.id : '';
    const tree = this.router.createUrlTree([], {
      queryParams: {
        userId: userId
      }
    });
    const url = this.url + this.serializer.serialize(tree);

    this.socket = new WebSocket(url);
    this.socket.onmessage = this.onMessage.bind(this);
    this.socket.onerror = this.onError.bind(this);
    this.socket.onopen = this.onOpen.bind(this);
    this.socket.onclose = this.onClose.bind(this);
  }

  onOpen(event: Event): void {
    console.log('Open', { event });
    const now = new Date();
    // const ping = now.getTime() - this.connectTime.getTime();

    // this.appState.myConnection.setValue({ connected: true, pingMs: ping });
  }

  onError(event: Event): void {
    console.error('Error', { event });
  }

  onClose(event: CloseEvent): void {
    console.log('Close', { event });
    // this.statusMessageService.setMyConnectionLost(event.reason);
  }

  // Messages received from server.
  onMessage(message: MessageEvent<string>): void {
    const dto = JSON.parse(message.data);

    if (dto.type === 'ChatMessageDto') {
      const msg = this.appState.chatMessages.getValue();
      this.appState.chatMessages.setValue([...msg, dto]);
    } else if (dto.type === 'JoinedChatDto') {
      const users = (dto as JoinedChatDto).users;
      this.appState.chatUsers.setValue(users);
    } else if (dto.type === 'LeftChatDto') {
      const users = (dto as JoinedChatDto).users;
      this.appState.chatUsers.setValue(users);
    }
  }

  // sending to server
  sendMessage(message: string) {
    const utcNow = new Date().toISOString();
    const dto: ChatMessageDto = {
      fromUser: this.appState.user.getValue().name,
      type: 'ChatMessageDto',
      message: message,
      utcDateTime: utcNow
    };

    if (this.socket && this.socket.readyState === this.socket.OPEN) {
      this.socket?.send(JSON.stringify(dto));
    }
  }

  ngOnDestroy(): void {
    this.cleanup();
  }

  private cleanup() {
    if (this.socket && this.socket.readyState !== this.socket.CLOSED) {
      this.socket.close();
    }
  }

  disconnect() {
    var users = [...this.appState.chatUsers.getValue()];
    var index = users.indexOf(this.appState.user.getValue().name);
    users.splice(index, 1);

    const dto: LeftChatDto = {
      users: users,
      type: 'LeftChatDto'
    };

    if (this.socket?.readyState === this.socket?.OPEN) {
      this.socket?.send(JSON.stringify(dto));
    }
  }
}