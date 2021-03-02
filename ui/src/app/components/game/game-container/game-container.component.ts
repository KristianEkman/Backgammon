import {
  AfterViewInit,
  Component,
  ElementRef,
  HostListener,
  OnDestroy,
  ViewChild
} from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { DiceDto, GameDto, GameState, MoveDto, PlayerColor } from 'src/app/dto';
import { AccountService, SocketsService } from 'src/app/services';
import { AppState } from 'src/app/state/app-state';

@Component({
  selector: 'app-game',
  templateUrl: './game-container.component.html',
  styleUrls: ['./game-container.component.scss']
})
export class GameContainerComponent implements OnDestroy, AfterViewInit {
  constructor(
    private service: SocketsService,
    private accountService: AccountService
  ) {
    this.gameDto$ = AppState.Singleton.game.observe();
    this.dices$ = AppState.Singleton.dices.observe();
    this.diceSubs = AppState.Singleton.dices
      .observe()
      .subscribe(this.diceChanged.bind(this));
    this.playerColor$ = AppState.Singleton.myColor.observe();
    this.gameSubs = AppState.Singleton.game
      .observe()
      .subscribe(this.gameChanged.bind(this));

    // if game page is refreshed, restore user from login cookie
    if (!AppState.Singleton.user.getValue()) {
      this.accountService.repair();
    }

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
  diceColor: PlayerColor | null = PlayerColor.neither;
  message = '';
  messageCenter = 0;
  flipped = false;

  @ViewChild('dices') dices: ElementRef | undefined;
  @ViewChild('boardButtons') boardButtons: ElementRef | undefined;
  @ViewChild('messages') messages: ElementRef | undefined;

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

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  gameChanged(dto: GameDto): void {
    this.setRollButtonVisible();
    this.setDicesVisible();
    this.setSendVisible();
    this.setUndoVisible();
    this.diceColor = dto.currentPlayer;
    this.fireResize();
    this.newVisible = dto.playState === GameState.ended;
    this.setTextMessage(dto);
  }

  setTextMessage(game: GameDto): void {
    const myColor = AppState.Singleton.myColor.getValue();
    if (!game) {
      this.message = 'Waiting for opponent to connect';
    } else if (game.playState === GameState.ended) {
      // console.log(this.myColor, this.game.winner);
      this.message =
        myColor === game.winner
          ? 'Congrats! You won.'
          : 'Sorry. You lost the game.';
    } else if (myColor === game.currentPlayer) {
      this.message = `Your turn to move.  (${PlayerColor[game.currentPlayer]})`;
    } else {
      this.message = `Waiting for ${PlayerColor[game.currentPlayer]} to move.`;
    }
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  diceChanged(dto: DiceDto[]): void {
    this.setRollButtonVisible();
    this.setDicesVisible();
    this.setSendVisible();
    this.setUndoVisible();
    this.fireResize();
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
    this.width = Math.min(window.innerWidth - 20, 1024);
    const span = this.messages?.nativeElement as Element;
    const spanWidth = span.getElementsByTagName('span')[0].clientWidth;
    this.messageCenter = this.width / 2 - spanWidth / 2;

    this.height = Math.min(window.innerHeight - 20, this.width * 0.6);

    const buttons = this.boardButtons?.nativeElement as HTMLElement;
    if (buttons) {
      buttons.style.top = `${this.height / 2 - buttons.clientHeight / 2}px`;
      buttons.style.right = `${this.width * 0.11}px`;
    }

    const dices = this.dices?.nativeElement as HTMLElement;
    if (dices) {
      // put the dices on right board if its my turn.
      if (this.myTurn()) {
        dices.style.left = `${this.width / 2 + 10}px`;
        dices.style.right = '';
      } else {
        dices.style.right = `${this.width / 2 + 10}px`;
        dices.style.left = '';
      }
      dices.style.top = `${this.height / 2 - dices.clientHeight / 2}px`;
    }
  }

  ngAfterViewInit(): void {
    this.fireResize();
  }

  fireResize(): void {
    setTimeout(() => {
      this.onResize();
    }, 1);
  }

  rollButtonVisible = false;
  sendVisible = false;
  undoVisible = false;
  dicesVisible = false;
  newVisible = false;

  rollButtonClick(): void {
    this.rollButtonClicked = true;
    this.setRollButtonVisible();
    this.setDicesVisible();
    this.setSendVisible();
    this.fireResize();
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

  abortGame(): void {
    this.service.abortGame();
  }

  newGame(): void {
    this.newVisible = false;
    this.service.connect();
  }
}
