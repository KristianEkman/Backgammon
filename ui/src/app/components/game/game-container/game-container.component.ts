import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  HostListener,
  OnDestroy,
  ViewChild
} from '@angular/core';
import { Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import {
  DiceDto,
  GameDto,
  GameState,
  MoveDto,
  PlayerColor,
  UserDto
} from 'src/app/dto';
import {
  AccountService,
  EditorService,
  GameService,
  SoundService,
  TutorialService
} from 'src/app/services';
import { AppStateService } from 'src/app/state/app-state.service';
import { StatusMessage } from 'src/app/dto/local/status-message';
import { StatusMessageService } from 'src/app/services/status-message.service';
import { map } from 'rxjs/operators';
import { CommonModule } from '@angular/common';
import { MessagesComponent } from '../messages/messages.component';
import { ButtonComponent } from '../../shared/button/button.component';
import { MenuComponent } from '../menu/menu.component';
import { PlayerComponent } from '../player/player.component';
import { GameBoardComponent } from '../game-board/game-board.component';
import { DicesComponent } from '../dices/dices.component';
import { BoardButtonsComponent } from '../board-buttons/board-buttons.component';
import { PlayAiQuestionComponent } from '../play-ai-question/play-ai-question.component';
import { TutorialMessageComponent } from '../tutorial-message/tutorial-message.component';
import { InputCopyComponent } from '../../shared/input-copy/input-copy.component';

@Component({
  selector: 'app-game',
  standalone: true,
  templateUrl: './game-container.component.html',
  styleUrls: ['./game-container.component.scss'],
  imports: [
    CommonModule,
    MessagesComponent,
    ButtonComponent,
    MenuComponent,
    PlayerComponent,
    GameBoardComponent,
    DicesComponent,
    BoardButtonsComponent,
    PlayAiQuestionComponent,
    TutorialMessageComponent,
    InputCopyComponent
  ]
})
export class GameContainerComponent implements OnDestroy, AfterViewInit {
  constructor(
    private service: GameService,
    private accountService: AccountService,
    private tutorialService: TutorialService,
    private router: Router,
    private statusMessageService: StatusMessageService,
    private changeDetector: ChangeDetectorRef,
    private sound: SoundService,
    private appState: AppStateService,
    private editService: EditorService
  ) {
    this.gameDto$ = this.appState.game.observe();
    this.dices$ = this.appState.dices.observe();
    this.diceSubs = this.appState.dices
      .observe()
      .subscribe(this.diceChanged.bind(this));
    this.playerColor$ = this.appState.myColor.observe();
    this.playerColor$.subscribe(this.gotPlayerColor.bind(this));
    this.gameSubs = this.appState.game
      .observe()
      .subscribe(this.gameChanged.bind(this));
    this.rolledSubs = this.appState.rolled
      .observe()
      .subscribe(this.opponentRolled.bind(this));

    this.oponnetDoneSubs = this.appState.opponentDone
      .observe()
      .subscribe(this.oponnentDone.bind(this));
    this.message$ = this.appState.statusMessage.observe();
    this.timeLeft$ = this.appState.moveTimer.observe();
    this.appState.moveTimer.observe().subscribe(this.timeTick.bind(this));

    this.user$ = this.appState.user.observe();
    this.tutorialStep$ = this.appState.tutorialStep.observe();
    this.gameString$ = this.appState.gameString.observe();

    this.user$.subscribe((user) => {
      if (user) this.introMuted = user.muteIntro;
    });

    // if game page is refreshed, restore user from login cookie
    if (!this.appState.user.getValue()) {
      this.accountService.repair();
    }

    const parsed = this.router.parseUrl(this.router.url);
    const gameId = parsed.queryParams['gameId'];
    const playAi = parsed.queryParams['playAi'];
    const forGold = parsed.queryParams['forGold'];
    const tutorial = parsed.queryParams['tutorial'];
    const editing = parsed.queryParams['editing'];

    this.playAiFlag = playAi === 'true';
    this.forGodlFlag = forGold === 'true';
    this.lokalStake = 0;
    this.tutorial = tutorial === 'true';
    this.editing = editing === 'true';

    if (tutorial) {
      // Waiting for everything else before starting makes Input data update components.
      setTimeout(() => {
        this.tutorialService.start();
      }, 1);
    } else if (!this.editing) {
      service.connect(gameId, playAi, forGold);
    }

    if (this.editing) {
      this.exitVisible = true;
      this.newVisible = false;
      this.sendVisible = false;
      this.dicesVisible = false;
      this.editService.setStartPosition();
    }
    // For some reason i could not use an observable for theme. Maybe i'll figure out why someday
    // service.connect might need to be in a setTimeout callback.
    this.themeName = this.appState.user.getValue()?.theme ?? 'dark';
  }

  gameDto$: Observable<GameDto>;
  dices$: Observable<DiceDto[]>;
  playerColor$: Observable<PlayerColor>;
  message$: Observable<StatusMessage>;
  timeLeft$: Observable<number>;
  user$: Observable<UserDto>;
  tutorialStep$: Observable<number>;
  gameString$: Observable<string>;
  themeName: string;

  gameSubs: Subscription;
  diceSubs: Subscription;
  rolledSubs: Subscription;
  oponnetDoneSubs: Subscription;

  started = false;
  width = 450;
  height = 450;
  rollButtonClicked = false;
  diceColor: PlayerColor | null = PlayerColor.neither;
  messageCenter = 0;
  rotated = false;
  flipped = false;
  playAiFlag = false;
  forGodlFlag = false;
  PlayerColor = PlayerColor;
  lokalStake = 0;
  animatingStake = false;
  playAiQuestion = false;
  tutorial = false;
  editing = false;
  dicesDto: DiceDto[] | undefined;
  nextDoublingFactor = 1;
  introMuted = this.appState.user.getValue()?.muteIntro ?? false;

  @ViewChild('dices') dices: ElementRef | undefined;
  @ViewChild('boardButtons') boardButtons: ElementRef | undefined;
  @ViewChild('messages') messages: ElementRef | undefined;

  gotPlayerColor() {
    if (this.appState.myColor.getValue() == PlayerColor.white) {
      this.flipped = true;
    }
  }

  sendMoves(): void {
    this.service.sendMoves();
    this.rollButtonClicked = false;
    this.dicesVisible = false;
  }

  doMove(move: MoveDto): void {
    if (!move.animate) this.sound.playChecker();
    this.service.doMove(move);
    this.service.sendMove(move);
  }

  doEditMove(move: MoveDto): void {
    this.editService.doMove(move);
    this.editService.updateGameString();
  }

  undoMove(): void {
    this.service.undoMove();
    this.service.sendUndo();
  }

  myTurn(): boolean {
    return this.appState.myTurn();
  }

  doublingRequested(): boolean {
    return this.appState.doublingRequested();
  }

  oponnentDone(): void {
    this.dicesVisible = false;
  }

  startedHandle: any;
  gameChanged(dto: GameDto): void {
    if (this.editing) {
      this.fireResize();
      return;
    }

    if (!this.started && dto) {
      clearTimeout(this.startedHandle);
      this.started = true;
      this.playAiQuestion = false;
      if (dto.isGoldGame) this.sound.playCoin();
    }
    // console.log(dto?.id);
    this.setRollButtonVisible();
    this.setSendVisible();
    this.setUndoVisible();
    this.setDoublingVisible(dto);
    this.diceColor = dto?.currentPlayer;
    this.fireResize();
    this.newVisible = dto?.playState === GameState.ended;
    this.exitVisible =
      dto?.playState !== GameState.playing &&
      dto?.playState !== GameState.requestedDoubling;
    this.nextDoublingFactor = dto?.goldMultiplier * 2;

    this.animateStake(dto);
  }

  animateStake(dto: GameDto) {
    if (dto && dto.isGoldGame && dto.stake !== this.lokalStake) {
      this.animatingStake = true;
      const step = Math.ceil((dto.stake - this.lokalStake) / 10);
      setTimeout(() => {
        const handle = setInterval(() => {
          this.lokalStake += step;
          this.changeDetector.detectChanges();

          if (
            (step > 0 && this.lokalStake >= dto.stake) ||
            (step < 0 && this.lokalStake <= dto.stake)
          ) {
            clearInterval(handle);
            this.lokalStake = dto.stake;
            this.animatingStake = false;
          }
        }, 100);
      }, 100); // Give time to show everything
    }
  }

  setDoublingVisible(gameDto: GameDto) {
    if (!gameDto) return;
    this.acceptDoublingVisible =
      gameDto.isGoldGame &&
      gameDto.playState === GameState.requestedDoubling &&
      this.myTurn();
    // Visible if it is a gold-game and if it is my turn to double.
    const turn = this.appState.myColor.getValue() !== gameDto.lastDoubler;
    const rightType = gameDto.isGoldGame;
    this.requestDoublingVisible =
      turn &&
      rightType &&
      this.myTurn() &&
      this.rollButtonVisible &&
      gameDto.isGoldGame &&
      this.hasFundsForDoubling(gameDto);
  }

  hasFundsForDoubling(gameDto: GameDto): boolean {
    return (
      gameDto.blackPlayer.gold >= gameDto.stake / 2 &&
      gameDto.whitePlayer.gold >= gameDto.stake / 2
    );
  }

  diceChanged(dto: DiceDto[]): void {
    this.dicesDto = dto;
    this.setRollButtonVisible();
    this.setSendVisible();
    this.setUndoVisible();
    this.fireResize();
    const game = this.appState.game.getValue();
    this.exitVisible =
      game?.playState !== GameState.playing &&
      game?.playState !== GameState.requestedDoubling;
  }

  ngOnDestroy(): void {
    this.gameSubs.unsubscribe();
    this.diceSubs.unsubscribe();
    this.rolledSubs.unsubscribe();
    this.oponnetDoneSubs.unsubscribe();
    clearTimeout(this.startedHandle);
    this.appState.game.clearValue();
    this.appState.myColor.clearValue();
    this.appState.dices.clearValue();
    this.appState.messages.clearValue();
    this.appState.moveTimer.clearValue();
    this.started = false;
    this.service.exitGame();
    this.sound.fadeIntro();
  }

  moveAnimFinished(): void {
    this.sound.playChecker();
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
    this.playAiQuestion = false;
    this.lokalStake = 0;

    if (!this.playAiFlag && !this.editing) this.waitForOpponent();
    this.fireResize();
  }

  private waitForOpponent() {
    this.sound.playPianoIntro();
    this.startedHandle = setTimeout(() => {
      if (!this.started) {
        this.playAiQuestion = true;
      }
    }, 11000);
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
  acceptDoublingVisible = false;
  requestDoublingVisible = false;
  requestHintVisible = false;

  rollButtonClick(): void {
    this.service.sendRolled();
    this.rollButtonClicked = true;
    this.setRollButtonVisible();
    this.dicesVisible = true;

    this.sound.playDice();

    this.setSendVisible();
    this.fireResize();
    this.requestDoublingVisible = false;
    const gme = this.appState.game.getValue();
    if (!gme.validMoves || gme.validMoves.length === 0) {
      this.statusMessageService.setBlockedMessage();
    }
    this.changeDetector.detectChanges();
  }

  opponentRolled(): void {
    this.dicesVisible = true;
    this.sound.playDice();
  }

  setRollButtonVisible(): void {
    if (!this.myTurn() || this.doublingRequested()) {
      this.rollButtonVisible = false;
      return;
    }

    this.rollButtonVisible = !this.rollButtonClicked;
  }

  setSendVisible(): void {
    if (!this.myTurn() || !this.rollButtonClicked || this.doublingRequested()) {
      this.sendVisible = false;
      return;
    }

    const game = this.appState.game.getValue();
    this.sendVisible = !game || game.validMoves.length == 0;
  }

  setUndoVisible(): void {
    if (!this.myTurn() || this.doublingRequested()) {
      this.undoVisible = false;
      return;
    }

    const dices = this.appState.dices.getValue();
    this.undoVisible = dices && dices.filter((d) => d.used).length > 0;
  }

  resignGame(): void {
    this.service.resignGame();
  }

  newGame(): void {
    this.newVisible = false;
    this.started = false;
    this.rollButtonClicked = false;

    this.service.resetGame();
    this.service.connect('', this.playAiFlag, this.forGodlFlag);
    this.waitForOpponent();
  }

  exitGame(): void {
    clearTimeout(this.startedHandle);
    this.service.exitGame();
    this.appState.hideBusy();
    this.router.navigateByUrl('/lobby');
  }

  requestDoubling(): void {
    this.requestDoublingVisible = false;
    this.service.requestDoubling();
  }

  requestHint(): void {
    this.requestHintVisible = false;
    this.service.requestHint();
  }

  acceptDoubling(): void {
    this.acceptDoublingVisible = false;
    this.service.acceptDoubling();
  }

  getDoubling(color: PlayerColor): Observable<number> {
    return this.gameDto$.pipe(
      map((game) => {
        return game?.lastDoubler === color ? game?.goldMultiplier : 0;
      })
    );
  }

  async playAi() {
    this.playAiQuestion = false;
    this.service.exitGame();

    while (this.appState.myConnection.getValue().connected) {
      await this.delay(500);
    }

    this.service.connect('', true, this.forGodlFlag);
  }

  delay(ms: number) {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }

  keepWaiting(): void {
    this.sound.playBlues();
    this.playAiQuestion = false;
  }

  @HostListener('window:beforeunload', ['$event'])
  unloadHandler() {
    return !this.started;
  }

  nextTutorialMessage() {
    this.tutorialService.nextStep();
  }

  previousTutorialMessage() {
    this.tutorialService.previousStep();
  }

  onFlipped(): void {
    this.flipped = !this.flipped;
    // both flipped and rotated is not supported
    if (this.flipped) {
      this.rotated = false;
    }
  }

  onRotated(): void {
    this.rotated = !this.rotated;
    if (this.rotated) {
      this.flipped = false;
    }
  }

  timeTick(time: number) {
    if (time < 30 && this.myTurn()) {
      const game = this.appState.game.getValue();
      if (
        game &&
        !game.isGoldGame &&
        game.playState === GameState.playing &&
        !this.rollButtonVisible &&
        !this.undoVisible
      ) {
        this.requestHintVisible = true;
        return;
      }
    }
    this.requestHintVisible = false;
  }

  toggleMuted() {
    this.accountService.toggleIntro();
  }

  get introPlaying() {
    return this.sound.introPlaying;
  }
}
