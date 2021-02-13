import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { ActionDto } from '../dto/Actions/actionDto';
import { ActionNames } from '../dto/Actions/actionNames';
import { DicesRolledActionDto } from '../dto/Actions/dicesRolledActionDto';
import { GameCreatedActionDto } from '../dto/Actions/gameCreatedActionDto';
import { GameDto } from '../dto/gameDto';
import { AppState } from '../state/app-state';

@Injectable({
  providedIn: 'root'
})
export class SocketsService {
  socket: WebSocket | undefined;
  url = '';
  constructor() {
    // this.socket = new WebSocket('ws://localhost:60109/ws');
  }

  connect(): void {
    this.url = environment.socketServiceUrl;
    this.socket = new WebSocket(this.url);
    this.socket.onmessage = this.onMessage;
    this.socket.onerror = this.onError;
    this.socket.onopen = this.onOpen;
    this.socket.onclose = this.onClose;
  }

  onOpen(event: Event): void {
    console.log('Open', { event });
  }

  onMessage(message: MessageEvent<string>): void {
    const action = JSON.parse(message.data) as ActionDto;
    switch (action.actionName) {
      case ActionNames.gameCreated:
        const dto = JSON.parse(message.data) as GameCreatedActionDto;
        AppState.Singleton.game.setValue(dto.game);
        break;
      case ActionNames.dicesRolled:
        const dicesAction = JSON.parse(message.data) as DicesRolledActionDto;
        AppState.Singleton.dices.setValue(dicesAction.dices);
        const game = AppState.Singleton.game.getValue();
        const cGame = {
          ...game,
          validMoves: dicesAction.validMoves,
          currentPlayer: dicesAction.playerToMove
        };
        AppState.Singleton.game.setValue(cGame);
        break;
      default:
        break;
    }
  }

  onError(event: Event): void {
    console.error('Error', { event });
  }

  sendMessage(message: string): void {
    if (this.socket) {
      this.socket.send(message);
    }
  }

  onClose(event: CloseEvent): void {
    console.log('Close', { event });
  }
}
