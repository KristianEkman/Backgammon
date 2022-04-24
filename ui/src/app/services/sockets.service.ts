import { Injectable, OnDestroy, ɵPlayState } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  CheckerDto,
  DiceDto,
  GameDto,
  MoveDto,
  PlayerColor,
  GameCookieDto,
  GameState
} from '../dto';
import { CookieService } from 'ngx-cookie-service';
import {
  ActionDto,
  ActionNames,
  DicesRolledActionDto,
  GameCreatedActionDto,
  GameEndedActionDto,
  MovesMadeActionDto,
  OpponentMoveActionDto,
  UndoActionDto,
  ConnectionInfoActionDto,
  GameRestoreActionDto,
  DoublingActionDto,
  HintMovesActionDto
} from '../dto/Actions';
import { AppState } from '../state/app-state';
import { Keys, Sound } from '../utils';
import { StatusMessageService } from './status-message.service';
import { MessageLevel } from '../dto/local/status-message';
import { Router, UrlSerializer } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class SocketsService implements OnDestroy {
  socket: WebSocket | undefined;
  url = '';
  userMoves: MoveDto[] = [];
  gameHistory: GameDto[] = [];
  dicesHistory: DiceDto[][] = [];
  connectTime = new Date();
  constructor(
    private cookieService: CookieService,
    private statusMessageService: StatusMessageService,
    private router: Router,
    private serializer: UrlSerializer
  ) {}

  connect(gameId: string, playAi: boolean, forGold: boolean): void {
    if (this.socket) {
      this.socket.close();
    }
    this.url = environment.socketServiceUrl;
    if (environment.production) {
      this.url = origin.replace('https://', 'wss://') + '/ws';
    }
    const user = AppState.Singleton.user.getValue();
    const userId = user ? user.id : '';
    const tree = this.router.createUrlTree([], {
      queryParams: {
        userId: userId,
        gameId: gameId,
        playAi: playAi,
        forGold: forGold
      }
    });
    const url = this.url + this.serializer.serialize(tree);

    this.socket = new WebSocket(url);
    this.socket.onmessage = this.onMessage.bind(this);
    this.socket.onerror = this.onError.bind(this);
    this.socket.onopen = this.onOpen.bind(this);
    this.socket.onclose = this.onClose.bind(this);
  }

  onOpen(event: Event): void {
    console.log('Open', { event });
    const now = new Date();
    const ping = now.getTime() - this.connectTime.getTime();
    this.statusMessageService.setWaitingForConnect();
    AppState.Singleton.myConnection.setValue({ connected: true, pingMs: ping });
    AppState.Singleton.game.clearValue();
    AppState.Singleton.dices.clearValue();
  }

  onError(event: Event): void {
    console.error('Error', { event });
    const cnn = AppState.Singleton.myConnection.getValue();
    AppState.Singleton.myConnection.setValue({ ...cnn, connected: false });
    this.statusMessageService.setMyConnectionLost('');
  }

  onClose(event: CloseEvent): void {
    console.log('Close', { event });
    const cnn = AppState.Singleton.myConnection.getValue();
    AppState.Singleton.myConnection.setValue({ ...cnn, connected: false });
    this.statusMessageService.setMyConnectionLost(event.reason);
  }

  // Messages received from server.
  onMessage(message: MessageEvent<string>): void {
    const action = JSON.parse(message.data) as ActionDto;
    // console.log(message.data);
    const game = AppState.Singleton.game.getValue();
    switch (action.actionName) {
      case ActionNames.gameCreated: {
        const dto = JSON.parse(message.data) as GameCreatedActionDto;
        AppState.Singleton.myColor.setValue(dto.myColor);
        AppState.Singleton.game.setValue(dto.game);

        const cookie: GameCookieDto = { id: dto.game.id, color: dto.myColor };
        this.cookieService.deleteAll(Keys.gameIdKey);
        // console.log('Settings cookie', cookie);
        this.cookieService.set(Keys.gameIdKey, JSON.stringify(cookie), 2);
        this.statusMessageService.setTextMessage(dto.game);
        AppState.Singleton.moveTimer.setValue(dto.game.thinkTime);
        Sound.fadeBlues();
        this.startTimer();
        break;
      }
      case ActionNames.dicesRolled: {
        const dicesAction = JSON.parse(message.data) as DicesRolledActionDto;
        AppState.Singleton.dices.setValue(dicesAction.dices);
        const cGame = {
          ...game,
          validMoves: dicesAction.validMoves,
          currentPlayer: dicesAction.playerToMove,
          playState: GameState.playing
        };
        // console.log(dicesAction.validMoves);
        AppState.Singleton.game.setValue(cGame);
        this.statusMessageService.setTextMessage(cGame);
        AppState.Singleton.moveTimer.setValue(dicesAction.moveTimer);
        AppState.Singleton.opponentDone.setValue(true);
        break;
      }
      case ActionNames.movesMade: {
        // This action is only sent to server.
        break;
      }
      case ActionNames.gameEnded: {
        const endedAction = JSON.parse(message.data) as GameEndedActionDto;
        // console.log('game ended', endedAction.game.winner);
        AppState.Singleton.game.setValue({
          ...endedAction.game,
          playState: GameState.ended
        });
        this.statusMessageService.setGameEnded(
          endedAction.game,
          endedAction.newScore
        );
        AppState.Singleton.moveTimer.setValue(0);
        break;
      }
      case ActionNames.requestedDoubling: {
        // Opponent has requested
        const action = JSON.parse(message.data) as DoublingActionDto;
        AppState.Singleton.moveTimer.setValue(action.moveTimer);

        AppState.Singleton.game.setValue({
          ...game,
          playState: GameState.requestedDoubling,
          currentPlayer: AppState.Singleton.myColor.getValue()
        });
        this.statusMessageService.setDoublingRequested();
        break;
      }
      case ActionNames.acceptedDoubling: {
        const action = JSON.parse(message.data) as DoublingActionDto;
        AppState.Singleton.moveTimer.setValue(action.moveTimer);
        // Opponent has accepted
        AppState.Singleton.game.setValue({
          ...game,
          playState: GameState.playing,
          goldMultiplier: game.goldMultiplier * 2,
          lastDoubler: AppState.Singleton.myColor.getValue(),
          currentPlayer: AppState.Singleton.myColor.getValue(),
          stake: game.stake * 2,
          whitePlayer: {
            ...game.whitePlayer,
            gold: game.whitePlayer.gold - game.stake / 2
          },
          blackPlayer: {
            ...game.blackPlayer,
            gold: game.blackPlayer.gold - game.stake / 2
          }
        });
        Sound.playCoin();
        this.statusMessageService.setDoublingAccepted();
        break;
      }
      case ActionNames.opponentMove: {
        const action = JSON.parse(message.data) as OpponentMoveActionDto;
        this.doMove(action.move);
        break;
      }
      case ActionNames.undoMove: {
        // const action = JSON.parse(message.data) as UndoActionDto;
        this.undoMove();
        break;
      }
      case ActionNames.rolled: {
        // this is just to fire the changed event. The value is not important.
        AppState.Singleton.rolled.setValue(true);
        break;
      }
      case ActionNames.connectionInfo: {
        const action = JSON.parse(message.data) as ConnectionInfoActionDto;
        if (!action.connection.connected) {
          console.log('Opponent disconnected');
          this.statusMessageService.setOpponentConnectionLost();
        }
        const cnn = AppState.Singleton.opponentConnection.getValue();
        AppState.Singleton.opponentConnection.setValue({
          ...cnn,
          connected: action.connection.connected
        });
        break;
      }
      case ActionNames.gameRestore: {
        const dto = JSON.parse(message.data) as GameRestoreActionDto;
        AppState.Singleton.myColor.setValue(dto.color);
        AppState.Singleton.game.setValue(dto.game);
        AppState.Singleton.dices.setValue(dto.dices);
        AppState.Singleton.moveTimer.setValue(dto.game.thinkTime);
        this.statusMessageService.setTextMessage(dto.game);
        this.startTimer();
        break;
      }
      case ActionNames.hintMoves: {
        const dto = JSON.parse(message.data) as HintMovesActionDto;

        dto.moves.forEach((hint) => {
          const clone = [...AppState.Singleton.moveAnimations.getValue()];
          // console.log('pushing next animation');
          clone.push(hint);
          AppState.Singleton.moveAnimations.setValue(clone);
        });
        break;
      }

      default:
        throw new Error(`Action not implemented ${action.actionName}`);
    }
  }

  timerStarted = false;
  timerId: any;

  startTimer(): void {
    if (this.timerStarted) {
      return;
    }
    this.timerStarted = true;
    this.timerId = setInterval(() => {
      let time = AppState.Singleton.moveTimer.getValue();
      time -= 0.25;
      AppState.Singleton.moveTimer.setValue(time);
      if (time > 0 && time < 10) {
        Sound.playTick();
      }
      if (time <= 0) {
        const currentMes = AppState.Singleton.statusMessage.getValue();
        if (
          AppState.Singleton.myTurn() &&
          currentMes.level !== MessageLevel.warning
        ) {
          this.statusMessageService.setMoveNow();
        }
      }
    }, 250);
  }

  doOpponentMove(move: MoveDto): void {
    const game = AppState.Singleton.game.getValue();
    const gameClone = JSON.parse(JSON.stringify(game)) as GameDto;
    const isWhite = move.color === PlayerColor.white;
    const from = isWhite ? 25 - move.from : move.from;
    const to = isWhite ? 25 - move.to : move.to;
    const checker = <CheckerDto>gameClone.points[from].checkers.pop();

    // hitting opponent checker
    const hit = gameClone.points[to].checkers.find(
      (c) => c.color !== move.color
    );
    if (hit) {
      gameClone.points[to].checkers.pop();
      const barIdx = isWhite ? 0 : 25;
      gameClone.points[barIdx].checkers.push(hit);
    }

    gameClone.points[to].checkers.push(checker);

    AppState.Singleton.game.setValue(gameClone);
  }

  doMove(move: MoveDto): void {
    this.userMoves.push({ ...move, nextMoves: [] }); // server does not need to know nextMoves.
    const prevGame = AppState.Singleton.game.getValue();
    this.gameHistory.push(prevGame);

    const gameClone = JSON.parse(JSON.stringify(prevGame)) as GameDto;
    gameClone.validMoves = move.nextMoves;
    const isWhite = move.color === PlayerColor.white;
    const from = isWhite ? 25 - move.from : move.from;
    const to = isWhite ? 25 - move.to : move.to;

    // remove moved checker
    const checker = <CheckerDto>(
      gameClone.points[from].checkers.find((c) => c.color === move.color)
    );
    const index = gameClone.points[from].checkers.indexOf(checker);
    gameClone.points[from].checkers.splice(index, 1);

    if (move.color == PlayerColor.black) {
      gameClone.blackPlayer.pointsLeft -= move.to - move.from;
    } else {
      gameClone.whitePlayer.pointsLeft -= move.to - move.from;
    }
    // hitting opponent checker
    const hit = gameClone.points[to].checkers.find(
      (c) => c.color !== move.color
    );

    if (hit) {
      if (move.to < 25) Sound.playCheckerWood();
      const hitIdx = gameClone.points[to].checkers.indexOf(hit);
      gameClone.points[to].checkers.splice(hitIdx, 1);
      const barIdx = isWhite ? 0 : 25;
      gameClone.points[barIdx].checkers.push(hit);
      if (move.color == PlayerColor.black) {
        gameClone.whitePlayer.pointsLeft += 25 - move.to;
      } else {
        gameClone.blackPlayer.pointsLeft += 25 - move.to;
      }
    }

    //push checker to new point
    gameClone.points[to].checkers.push(checker);
    AppState.Singleton.game.setValue(gameClone);

    const dices = AppState.Singleton.dices.getValue();
    this.dicesHistory.push(dices);

    const diceClone = JSON.parse(JSON.stringify(dices)) as DiceDto[];

    // Find a dice with equal value as the move length
    // or if bearing off equal or larger
    let diceIdx = diceClone.findIndex(
      (d) => !d.used && d.value === move.to - move.from
    );

    if (diceIdx < 0) {
      diceIdx = diceClone.findIndex(
        (d) => !d.used && move.to === 25 && move.to - move.from <= d.value
      );
    }
    const dice = diceClone[diceIdx];
    dice.used = true;
    AppState.Singleton.dices.setValue(diceClone);

    if (move.animate) {
      const clone = [...AppState.Singleton.moveAnimations.getValue()];
      // console.log('pushing next animation');
      clone.push(move);
      AppState.Singleton.moveAnimations.setValue(clone);
    }
  }

  undoMove(): void {
    if (this.gameHistory.length < 1) {
      return;
    }
    const move = this.userMoves.pop();
    if (!move) {
      return;
    }
    const game = this.gameHistory.pop() as GameDto;
    AppState.Singleton.game.setValue(game);

    const dices = this.dicesHistory.pop() as DiceDto[];
    AppState.Singleton.dices.setValue(dices);

    const clone = [...AppState.Singleton.moveAnimations.getValue()];
    // console.log('pushing next animation');
    clone.push({ ...move, from: move.to, to: move.from });
    AppState.Singleton.moveAnimations.setValue(clone);
  }

  sendMessage(message: string): void {
    if (this.socket && this.socket.readyState === this.socket.OPEN) {
      this.socket.send(message);
    }
  }

  sendMoves(): void {
    const myColor = AppState.Singleton.myColor.getValue();
    // Opponent moves are also stored in userMoves but we cant send them back.
    const action: MovesMadeActionDto = {
      actionName: ActionNames.movesMade,
      moves: this.userMoves.filter((m) => m.color === myColor)
    };
    this.sendMessage(JSON.stringify(action));
    this.userMoves = [];
    this.dicesHistory = [];
    this.gameHistory = [];
  }

  shiftMoveAnimationsQueue(): void {
    // console.log('shifting animation queue');
    const clone = [...AppState.Singleton.moveAnimations.getValue()];
    clone.shift();
    AppState.Singleton.moveAnimations.setValue(clone);
  }

  sendMove(move: MoveDto): void {
    // removing next moves to decrease bytes.
    const action: OpponentMoveActionDto = {
      actionName: ActionNames.opponentMove,
      move: { ...move, nextMoves: [], animate: true }
    };
    this.sendMessage(JSON.stringify(action));
  }

  sendUndo(): void {
    const action: UndoActionDto = {
      actionName: ActionNames.undoMove
    };
    this.sendMessage(JSON.stringify(action));
  }

  sendRolled() {
    const action: ActionDto = {
      actionName: ActionNames.rolled
    };
    this.sendMessage(JSON.stringify(action));
  }

  ngOnDestroy(): void {
    this.socket?.close();
    clearTimeout(this.timerId);
    this.timerStarted = false;
  }

  resignGame(): void {
    const action: ActionDto = {
      actionName: ActionNames.resign
    };
    this.sendMessage(JSON.stringify(action));
  }

  exitGame(): void {
    const action: ActionDto = {
      actionName: ActionNames.exitGame
    };
    this.sendMessage(JSON.stringify(action));
    clearTimeout(this.timerId);
    this.timerStarted = false;
  }

  resetGame(): void {
    this.cookieService.deleteAll(Keys.gameIdKey);
    this.userMoves = [];
    this.gameHistory = [];
    this.dicesHistory = [];
    this.connectTime = new Date();
  }

  //This is when this player accepts a doubling.
  acceptDoubling() {
    const action: DoublingActionDto = {
      actionName: ActionNames.acceptedDoubling,
      moveTimer: 0 // Set on the server
    };
    const game = AppState.Singleton.game.getValue();
    AppState.Singleton.game.setValue({
      ...game,
      playState: GameState.playing,
      goldMultiplier: game.goldMultiplier * 2,
      lastDoubler: AppState.Singleton.getOtherPlayer(),
      currentPlayer: AppState.Singleton.getOtherPlayer(),
      whitePlayer: {
        ...game.whitePlayer,
        gold: game.whitePlayer.gold - game.stake / 2
      },
      blackPlayer: {
        ...game.blackPlayer,
        gold: game.blackPlayer.gold - game.stake / 2
      },
      stake: game.stake * 2
    });

    // TODO: The client countdown is currently only a constant on the backend.
    // What is the best design here?
    AppState.Singleton.moveTimer.setValue(40);
    this.sendMessage(JSON.stringify(action));
    this.statusMessageService.setTextMessage(
      AppState.Singleton.game.getValue()
    );
  }

  //This player requests doubling.
  requestDoubling() {
    const game = AppState.Singleton.game.getValue();
    const otherPlyr = AppState.Singleton.getOtherPlayer();
    AppState.Singleton.game.setValue({
      ...game,
      playState: GameState.requestedDoubling,
      currentPlayer: otherPlyr
    });

    const action: DoublingActionDto = {
      actionName: ActionNames.requestedDoubling,
      moveTimer: 0 // set on the server
    };

    // TODO: The client countdown is currently only a constant on the backend.
    // What is the best design here? Where to store the constant? One extra server message for this case?
    AppState.Singleton.moveTimer.setValue(40);
    this.sendMessage(JSON.stringify(action));
    this.statusMessageService.setWaitingForDoubleResponse();
  }

  requestHint(): void {
    const action: ActionDto = {
      actionName: ActionNames.requestHint
    };
    this.sendMessage(JSON.stringify(action));
  }
}
