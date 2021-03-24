import { Injectable } from '@angular/core';
import { StatusMessage } from '../dto/local/status-message';
import { GameDto, NewScoreDto, PlayerColor } from '../dto';
import { AppState } from '../state/app-state';
import { Busy } from '../state/busy';

@Injectable({
  providedIn: 'root'
})
export class StatusMessageService {
  setTextMessage(game: GameDto): void {
    const myColor = AppState.Singleton.myColor.getValue();
    let message: StatusMessage;
    if (game && myColor === game.currentPlayer) {
      Busy.hide();
      message = StatusMessage.info(
        `Your turn to move.  (${PlayerColor[game.currentPlayer]})`
      );
    } else {
      Busy.hide();
      message = StatusMessage.info(
        `Waiting for ${PlayerColor[game.currentPlayer]} to move.`
      );
    }
    AppState.Singleton.statusMessage.setValue(message);
  }

  setMyConnectionLost(reason: string): void {
    const statusMessage = StatusMessage.error(reason || 'No server connection');
    AppState.Singleton.statusMessage.setValue(statusMessage);
  }

  setOpponentConnectionLost(): void {
    const statusMessage = StatusMessage.warning('Opponent connection lost');
    AppState.Singleton.statusMessage.setValue(statusMessage);
  }

  setWaitingForConnect(): void {
    const statusMessage = StatusMessage.info('Waiting for opponent to connect');
    Busy.showNoOverlay();
    AppState.Singleton.statusMessage.setValue(statusMessage);
  }

  setGameEnded(game: GameDto, newScore: NewScoreDto): void {
    // console.log(this.myColor, this.game.winner);
    const myColor = AppState.Singleton.myColor.getValue();
    let message = StatusMessage.info('Game ended.');
    if (newScore) {
      const score = `New score ${newScore.score} (${newScore.increase})`;

      message = StatusMessage.info(
        myColor === game.winner
          ? `Congrats! You won. ${score}`
          : `Sorry. You lost the game. ${score}`
      );
      AppState.Singleton.statusMessage.setValue(message);
    }
  }

  setBlockedMessage(): void {
    const text = 'You are blocked. Click "Done"';
    const msg = StatusMessage.warning(text);
    AppState.Singleton.statusMessage.setValue(msg);
  }
}
