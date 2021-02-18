import {
  AfterViewInit,
  Component,
  HostListener,
  OnDestroy
} from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { DiceDto, GameDto, MoveDto, PlayerColor } from 'src/app/dto';
import { SocketsService } from 'src/app/services';
import { AppState } from 'src/app/state/app-state';

@Component({
  selector: 'app-game',
  templateUrl: './game-container.component.html',
  styleUrls: ['./game-container.component.scss']
})
export class GameContainerComponent implements OnDestroy, AfterViewInit {
  constructor(private service: SocketsService) {
    this.gameDto$ = AppState.Singleton.game.observe();
    this.dices$ = AppState.Singleton.dices.observe();
    this.diceSubs = AppState.Singleton.dices
      .observe()
      .subscribe(this.diceChanged.bind(this));
    this.playerColor$ = AppState.Singleton.myColor.observe();
    this.gameSubs = AppState.Singleton.game
      .observe()
      .subscribe(this.gameChanged.bind(this));

    service.connect();
  }

  gameDto$: Observable<GameDto>;
  dices$: Observable<DiceDto[]>;
  playerColor$: Observable<PlayerColor>;
  gameSubs: Subscription;
  diceSubs: Subscription;

  sendHidden = true;
  undoVisible = false;
  width = 450;
  height = 450;

  sendMoves(): void {
    this.sendHidden = true;
    this.service.sendMoves();
  }

  doMove(move: MoveDto): void {
    this.service.doMove(move);
    this.sendHidden = move.nextMoves && move.nextMoves.length > 0;
  }

  undoMove(): void {
    this.service.undoMove();
    this.sendHidden = true;
  }

  gameChanged(dto: GameDto): void {
    // todo: auto send here.
    this.sendHidden =
      (dto.validMoves && dto.validMoves.length > 0) ||
      AppState.Singleton.myColor.getValue() !== dto.currentPlayer;
  }

  diceChanged(dto: DiceDto[]): void {
    this.undoVisible = dto.filter((d) => d.used).length > 0;
  }

  ngOnDestroy(): void {
    this.gameSubs.unsubscribe();
    this.diceSubs.unsubscribe();
  }

  moveAnimFinished(): void {
    this.service.shiftMoveAnimationsQueue();
  }

  @HostListener('window:resize', ['$event'])
  onResize(): void {
    this.width = Math.min(window.innerWidth, 800);
    this.height = Math.min(window.innerHeight, this.width * 0.6);
  }

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.onResize();
    }, 0);
  }
}
