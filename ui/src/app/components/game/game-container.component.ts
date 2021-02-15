import { Component, OnDestroy } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { DiceDto, GameDto, MoveDto, PlayerColor } from 'src/app/dto';
import { SocketsService } from 'src/app/services';
import { AppState } from 'src/app/state/app-state';

@Component({
  selector: 'app-game',
  templateUrl: './game-container.component.html',
  styleUrls: ['./game-container.component.scss']
})
export class GameContainerComponent implements OnDestroy {
  constructor(private service: SocketsService) {
    this.gameDto$ = AppState.Singleton.game.changed;
    this.dices$ = AppState.Singleton.dices.changed;
    this.playerColor$ = AppState.Singleton.myColor.changed;
    this.gameSubs = AppState.Singleton.game.changed.subscribe(this.gameChanged);
    service.connect();
  }
  gameDto$: Observable<GameDto>;
  dices$: Observable<DiceDto[]>;
  playerColor$: Observable<PlayerColor>;
  gameSubs: Subscription;

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

  gameChanged(dto: GameDto): void {
    // todo: auto send here.
    this.sendDisabled = dto.validMoves && dto.validMoves.length > 0;
  }

  ngOnDestroy(): void {
    this.gameSubs.unsubscribe();
  }
}
