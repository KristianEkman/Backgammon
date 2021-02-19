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

  width = 450;
  height = 450;
  rollButtonClicked = false;

  sendMoves(): void {
    this.service.sendMoves();
    this.rollButtonClicked = false;
  }

  doMove(move: MoveDto): void {
    this.service.doMove(move);
    this.service.sendMove(move);
  }

  undoMove(): void {
    this.service.undoMove();
    this.service.sendUndo();
  }

  myTurn(): boolean {
    return AppState.Singleton.myTurn();
  }

  gameChanged(dto: GameDto): void {
    this.setRollButtonVisible();
    this.setDicesVisible();
    this.setSendVisible();
    this.setUndoVisible();
  }

  diceChanged(dto: DiceDto[]): void {
    this.setRollButtonVisible();
    this.setDicesVisible();
    this.setSendVisible();
    this.setUndoVisible();
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

  rollButtonVisible = false;
  sendVisible = false;
  undoVisible = false;
  dicesVisible = false;

  rollButtonClick(): void {
    this.rollButtonClicked = true;
    this.setRollButtonVisible();
    this.setDicesVisible();
    this.setSendVisible();
  }

  setRollButtonVisible(): void {
    if (!this.myTurn()) {
      this.rollButtonVisible = false;
      return;
    }

    this.rollButtonVisible = !this.rollButtonClicked;
  }

  setSendVisible(): void {
    if (!this.myTurn() || !this.rollButtonClicked) {
      this.sendVisible = false;
      return;
    }

    const game = AppState.Singleton.game.getValue();
    this.sendVisible = !game || game.validMoves.length == 0;
  }

  setUndoVisible(): void {
    if (!this.myTurn()) {
      this.undoVisible = false;
      return;
    }
    const dices = AppState.Singleton.dices.getValue();
    this.undoVisible = dices && dices.filter((d) => d.used).length > 0;
  }

  setDicesVisible(): void {
    if (!this.myTurn()) {
      this.dicesVisible = true;
      return;
    }
    this.dicesVisible = !this.rollButtonVisible;
  }
}
