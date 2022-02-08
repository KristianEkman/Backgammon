import { StatusMessage } from '../dto/local/status-message';
import {
  ConnectionDto,
  GameState,
  MoveDto,
  PlayedGameListDto,
  PlayerColor,
  Toplist,
  UserDto
} from '../dto';
import { DiceDto } from '../dto/diceDto';
import { GameDto } from '../dto/gameDto';
import { StateObject } from './state-object';
import { Busy } from './busy';
import { ErrorState } from './ErrorState';
import { MessageDto } from '../dto/message/messageDto';

export class AppState {
  constructor() {
    this.busy = new StateObject<Busy>();
    this.game = new StateObject<GameDto>();
    this.myColor = new StateObject<PlayerColor>();
    this.myColor.setValue(PlayerColor.neither);
    this.dices = new StateObject<DiceDto[]>();
    this.dices.setValue([]);
    this.moveAnimations = new StateObject<MoveDto[]>();
    this.moveAnimations.setValue([]);
    this.myConnection = new StateObject<ConnectionDto>();
    this.opponentConnection = new StateObject<ConnectionDto>();
    this.user = new StateObject<UserDto>();
    this.statusMessage = new StateObject<StatusMessage>();
    this.moveTimer = new StateObject<number>();
    this.toplist = new StateObject<Toplist>();
    this.errors = new StateObject<ErrorState>();
    this.playedGames = new StateObject<PlayedGameListDto>();
    this.playedGames.setValue({ games: [] });
    this.messages = new StateObject<MessageDto[]>();
    this.messages.setValue([]);
    this.rolled = new StateObject<boolean>();
    this.opponentDone = new StateObject<boolean>();
    this.theme = new StateObject<string>();
    this.theme.setValue('dark');
    this.tutorialStep = new StateObject<number>();
    this.tutorialStep.setValue(0);
  }

  private static _singleton: AppState;
  public static get Singleton(): AppState {
    if (!this._singleton) {
      this._singleton = new AppState();
    }
    return this._singleton;
  }

  busy: StateObject<Busy>;
  game: StateObject<GameDto>;
  myColor: StateObject<PlayerColor>;
  dices: StateObject<DiceDto[]>;
  moveAnimations: StateObject<MoveDto[]>;
  myConnection: StateObject<ConnectionDto>;
  opponentConnection: StateObject<ConnectionDto>;
  user: StateObject<UserDto>;
  statusMessage: StateObject<StatusMessage>;
  moveTimer: StateObject<number>;
  toplist: StateObject<Toplist>;
  errors: StateObject<ErrorState>;
  playedGames: StateObject<PlayedGameListDto>;
  messages: StateObject<MessageDto[]>;
  rolled: StateObject<boolean>;
  opponentDone: StateObject<boolean>;
  theme: StateObject<string>;
  tutorialStep: StateObject<number>;

  myTurn(): boolean {
    const game = this.game.getValue();

    return (
      game &&
      game.playState !== GameState.ended &&
      game.currentPlayer === this.myColor.getValue()
    );
  }

  doublingRequested(): boolean {
    const game = this.game.getValue();
    return game && game.playState === GameState.requestedDoubling;
  }

  getOtherPlayer(): PlayerColor {
    return this.myColor.getValue() === PlayerColor.black
      ? PlayerColor.white
      : PlayerColor.black;
  }
}
