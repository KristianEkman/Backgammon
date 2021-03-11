import { Injectable } from '@angular/core';
import { StatusMessage } from '../dto/local/status-message';
import { GameDto, GameState, PlayerColor } from '../dto';
import { AppState } from '../state/app-state';

@Injectable({
  providedIn: 'root'
})
export class StatusMessageService {
  setTextMessage(game: GameDto): void {
    const myColor = AppState.Singleton.myColor.getValue();
    let message: StatusMessage;
    if (!game) {
      message = StatusMessage.info('Waiting for opponent to connect');
    } else if (game.playState === GameState.ended) {
      // console.log(this.myColor, this.game.winner);
      message = StatusMessage.info(
        myColor === game.winner
          ? 'Congrats! You won.'
          : 'Sorry. You lost the game.'
      );
    } else if (myColor === game.currentPlayer) {
      message = StatusMessage.info(
        `Your turn to move.  (${PlayerColor[game.currentPlayer]})`
      );
    } else {
      message = StatusMessage.info(
        `Waiting for ${PlayerColor[game.currentPlayer]} to move.`
      );
    }
    AppState.Singleton.statusMessage.setValue(message);
  }

  setMyConnectionLost(reason: string): void {
    const statusMessage = StatusMessage.error(
      reason || 'Server connection lost'
    );
    AppState.Singleton.statusMessage.setValue(statusMessage);
  }

  setOpponentConnectionLost(): void {
    const statusMessage = StatusMessage.warning('Opponent connection lost');
    AppState.Singleton.statusMessage.setValue(statusMessage);
  }

  setWaitingForConnect(): void {
    const statusMessage = StatusMessage.info('Waiting for opponent to connect');
    AppState.Singleton.statusMessage.setValue(statusMessage);
  }
}
