import { Injectable } from '@angular/core';
import { StatusMessage } from '../dto/local/status-message';
import { GameDto, NewScoreDto, PlayerColor } from '../dto';
import { AppState } from '../state/app-state';
import { Busy } from '../state/busy';
import { Sound } from '../utils';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root'
})
export class StatusMessageService {
  constructor(private trans: TranslateService) {}
  setTextMessage(game: GameDto): void {
    const myColor = AppState.Singleton.myColor.getValue();
    let message: StatusMessage;
    const currentColor = this.trans.instant(PlayerColor[game.currentPlayer]);
    if (game && myColor === game.currentPlayer) {
      Busy.hide();
      const m = this.trans.instant('statusmessage.yourturn', {
        color: currentColor
      });
      message = StatusMessage.info(m);
    } else {
      Busy.hide();
      const m = this.trans.instant('statusmessage.waitingfor', {
        color: currentColor
      });
      message = StatusMessage.info(m);
    }
    AppState.Singleton.statusMessage.setValue(message);
  }

  setMyConnectionLost(reason: string): void {
    const m = this.trans.instant('statusmessage.noconnection');
    const statusMessage = StatusMessage.error(reason || m);
    AppState.Singleton.statusMessage.setValue(statusMessage);
  }

  setOpponentConnectionLost(): void {
    const m = this.trans.instant('statusmessage.opponentconnectionlost');
    const statusMessage = StatusMessage.warning(m);
    AppState.Singleton.statusMessage.setValue(statusMessage);
  }

  setWaitingForConnect(): void {
    const m = this.trans.instant('statusmessage.waitingoppcnn');
    const statusMessage = StatusMessage.info(m);
    Busy.showNoOverlay();
    AppState.Singleton.statusMessage.setValue(statusMessage);
  }

  setGameEnded(game: GameDto, newScore: NewScoreDto): void {
    console.log('game ended', game, newScore);
    const myColor = AppState.Singleton.myColor.getValue();
    let score = '';
    if (newScore) {
      let increase = newScore.increase.toString();
      if (newScore.increase > 0) increase = `+${newScore.increase}`;
      score = this.trans.instant('statusmessage.newscore', {
        score: newScore.score,
        increase: increase
      });
    }

    const message = StatusMessage.info(
      myColor === game.winner
        ? this.trans.instant('statusmessage.youwon', { score: score })
        : this.trans.instant('statusmessage.youlost', { score: score })
    );
    AppState.Singleton.statusMessage.setValue(message);
    if (myColor === game.winner) {
      Sound.playWinner();
    } else {
      Sound.playLooser();
    }
  }

  setBlockedMessage(): void {
    const text = this.trans.instant('statusmessage.youareblocked');
    const msg = StatusMessage.warning(text);
    AppState.Singleton.statusMessage.setValue(msg);
  }

  setMoveNow(): void {
    const m = this.trans.instant('statusmessage.movenow');
    const message = StatusMessage.warning(m);
    Sound.playWarning();
    AppState.Singleton.statusMessage.setValue(message);
  }
}
