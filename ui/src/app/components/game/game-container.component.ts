import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { DiceDto, GameDto, MoveDto } from 'src/app/dto';
import { SocketsService } from 'src/app/services';
import { AppState } from 'src/app/state/app-state';

@Component({
  selector: 'app-game',
  templateUrl: './game-container.component.html',
  styleUrls: ['./game-container.component.scss']
})
export class GameContainerComponent {
  constructor(private service: SocketsService) {
    this.gameDto$ = AppState.Singleton.game.changed;
    this.dices$ = AppState.Singleton.dices.changed;
    service.connect();
  }

  gameDto$: Observable<GameDto>;
  dices$: Observable<DiceDto[]>;

  sendDisabled = true;

  sendMoves(): void {
    this.sendDisabled = true;
    this.service.sendMoves();
  }

  doMove(move: MoveDto): void {
    this.service.doMove(move);
    this.sendDisabled = move.nextMoves && move.nextMoves.length > 0;
  }

  undoMove(): void {
    this.service.undoMove();
    this.sendDisabled = true;
  }
}
