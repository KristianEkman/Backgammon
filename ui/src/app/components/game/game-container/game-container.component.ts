import {
  AfterViewInit,
  Component,
  ElementRef,
  HostListener,
  OnDestroy,
  ViewChild
} from '@angular/core';
import { Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { DiceDto, GameDto, GameState, MoveDto, PlayerColor } from 'src/app/dto';
import { AccountService, SocketsService } from 'src/app/services';
import { AppState } from 'src/app/state/app-state';
import { StatusMessage } from 'src/app/dto/local/status-message';
import { Busy } from 'src/app/state/busy';
import { StatusMessageService } from 'src/app/services/status-message.service';

@Component({
  selector: 'app-game',
  templateUrl: './game-container.component.html',
  styleUrls: ['./game-container.component.scss']
})
export class GameContainerComponent implements OnDestroy, AfterViewInit {
  constructor(
    private service: SocketsService,
    private accountService: AccountService,
    private router: Router,
    private statusMessageService: StatusMessageService
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
    this.message$ = AppState.Singleton.statusMessage.observe();
    this.timeLeft$ = AppState.Singleton.moveTimer.observe();

    // if game page is refreshed, restore user from login cookie
    if (!AppState.Singleton.user.getValue()) {
      this.accountService.repair();
    }

    const gameId = this.router.parseUrl(this.router.url).queryParams['gameId'];
    const playAi = this.router.parseUrl(this.router.url).queryParams['playAi'];
    service.connect(gameId, playAi);
  }

  gameDto$: Observable<GameDto>;
  dices$: Observable<DiceDto[]>;
  playerColor$: Observable<PlayerColor>;
  message$: Observable<StatusMessage>;
  timeLeft$: Observable<number>;
  gameSubs: Subscription;
  diceSubs: Subscription;

  width = 450;
  height = 450;
  rollButtonClicked = false;
  diceColor: PlayerColor | null = PlayerColor.neither;
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
    this.diceColor = dto?.currentPlayer;
    this.fireResize();
    this.newVisible = dto?.playState === GameState.ended;
    this.exitVisible = dto?.playState !== GameState.playing;
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  diceChanged(dto: DiceDto[]): void {
    this.setRollButtonVisible();
    this.setDicesVisible();
    this.setSendVisible();
    this.setUndoVisible();
    this.fireResize();
    this.exitVisible =
      AppState.Singleton.game.getValue()?.playState !== GameState.playing;
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
    this.width = Math.min(window.innerWidth, 1024);
    const span = this.messages?.nativeElement as Element;
    const spanWidth = span.getElementsByTagName('span')[0].clientWidth;
    this.messageCenter = this.width / 2 - spanWidth / 2;

    this.height = Math.min(window.innerHeight - 40, this.width * 0.6);

    const buttons = this.boardButtons?.nativeElement as HTMLElement;
    const btnsOffset = 5; //Cheating. Could not get the height.
    if (buttons) {
      buttons.style.top = `${this.height / 2 - btnsOffset}px`;
      buttons.style.right = `${this.width * 0.11}px`;
    }

    const dices = this.dices?.nativeElement as HTMLElement;
    if (dices) {
      // Puts the dices on right side if its my turn.
      if (this.myTurn()) {
        dices.style.left = `${this.width / 2 + 20}px`;
        dices.style.right = '';
      } else {
        dices.style.right = `${this.width / 2 + 20}px`;
        dices.style.left = '';
      }
      dices.style.top = `${this.height / 2 - btnsOffset}px`;
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
  exitVisible = true;

  rollButtonClick(): void {
    this.rollButtonClicked = true;
    this.setRollButtonVisible();
    this.setDicesVisible();
    this.setSendVisible();
    this.fireResize();
    const gme = AppState.Singleton.game.getValue();
    if (!gme.validMoves || gme.validMoves.length === 0) {
      this.statusMessageService.setBlockedMessage();
    }
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

  resignGame(): void {
    this.service.resignGame();
  }

  newGame(): void {
    this.newVisible = false;
    this.service.connect('', false);
  }

  exitGame(): void {
    this.service.exitGame();
    Busy.hide();
    this.router.navigateByUrl('/lobby');
  }
}
