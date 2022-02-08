import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { GameDto, GameState, PlayerColor } from '../dto';
import { AppState } from '../state/app-state';

@Injectable({
  providedIn: 'root'
})
export class TutorialService {
  constructor(private router: Router) {}

  nextStep() {
    var step = AppState.Singleton.tutorialStep.getValue();
    step++;
    if (step == 10) {
      this.router.navigate(['/']);
      return;
    }
    AppState.Singleton.tutorialStep.setValue(step);

    this.setBoard(step);
  }

  previousStep() {
    var step = AppState.Singleton.tutorialStep.getValue();
    step--;
    AppState.Singleton.tutorialStep.setValue(step);
    this.setBoard(step);
  }

  private setBoard(step: number) {
    setTimeout(() => {
      switch (step) {
        case 1:
        case 2:
        case 3:
          this.startPosition();
          break;
        case 4:
        case 5:
          this.bearingOffExample();
          break;
        case 6: {
          this.blockedExample();
          break;
        }
        case 7: {
          this.lonelyCheckerExample();
          break;
        }
        case 8: {
          this.barExample();
          break;
        }
        default:
          break;
      }
    }, 1);
  }

  private emptyGame(): GameDto {
    const game: GameDto = {
      blackPlayer: {
        name: 'You',
        elo: 0,
        gold: 0,
        photoUrl: '',
        playerColor: PlayerColor.black,
        pointsLeft: 0
      },
      currentPlayer: PlayerColor.black,
      goldMultiplier: 0,
      id: '',
      isGoldGame: false,
      playState: GameState.playing,
      points: [],
      stake: 0,
      thinkTime: 1000,
      validMoves: [],
      whitePlayer: {
        name: 'Aina',
        elo: 0,
        gold: 0,
        photoUrl: '',
        playerColor: PlayerColor.black,
        pointsLeft: 0
      },
      winner: PlayerColor.neither,
      lastDoubler: undefined
    };
    for (let i = 0; i < 26; i++) {
      game.points.push({
        blackNumber: i,
        checkers: [],
        whiteNumber: 26 - i
      });
    }
    return game;
  }

  addChecker(game: GameDto, color: PlayerColor, point: number, count: number) {
    for (var i = 0; i < count; i++)
      game.points[point].checkers.push({ color: color });
  }

  startPosition() {
    const game: GameDto = this.emptyGame();
    this.addChecker(game, PlayerColor.black, 1, 2);
    this.addChecker(game, PlayerColor.black, 12, 5);
    this.addChecker(game, PlayerColor.black, 17, 3);
    this.addChecker(game, PlayerColor.black, 19, 5);
    this.addChecker(game, PlayerColor.white, 6, 5);
    this.addChecker(game, PlayerColor.white, 8, 3);
    this.addChecker(game, PlayerColor.white, 13, 5);
    this.addChecker(game, PlayerColor.white, 24, 2);

    AppState.Singleton.myColor.setValue(PlayerColor.black);
    AppState.Singleton.game.setValue(game);
  }

  start() {
    this.startPosition();
    AppState.Singleton.tutorialStep.setValue(1);
  }

  bearingOffExample() {
    const game: GameDto = this.emptyGame();
    for (var i = 0; i < 2; i++)
      game.points[19].checkers.push({ color: PlayerColor.black });
    for (var i = 0; i < 5; i++)
      game.points[20].checkers.push({ color: PlayerColor.black });
    for (var i = 0; i < 3; i++)
      game.points[21].checkers.push({ color: PlayerColor.black });
    for (var i = 0; i < 3; i++)
      game.points[22].checkers.push({ color: PlayerColor.black });
    for (var i = 0; i < 2; i++)
      game.points[25].checkers.push({ color: PlayerColor.black });

    for (var i = 0; i < 5; i++)
      game.points[6].checkers.push({ color: PlayerColor.white });
    for (var i = 0; i < 3; i++)
      game.points[8].checkers.push({ color: PlayerColor.white });
    for (var i = 0; i < 5; i++)
      game.points[13].checkers.push({ color: PlayerColor.white });
    for (var i = 0; i < 2; i++)
      game.points[2].checkers.push({ color: PlayerColor.white });
    AppState.Singleton.myColor.setValue(PlayerColor.black);
    AppState.Singleton.game.setValue(game);
  }

  lonelyCheckerExample() {
    const game: GameDto = this.emptyGame();
    this.addChecker(game, PlayerColor.black, 1, 2);
    this.addChecker(game, PlayerColor.black, 25, 13);

    this.addChecker(game, PlayerColor.white, 0, 14);
    this.addChecker(game, PlayerColor.white, 5, 1);

    AppState.Singleton.myColor.setValue(PlayerColor.black);
    AppState.Singleton.game.setValue(game);
  }

  barExample() {
    const game: GameDto = this.emptyGame();
    this.addChecker(game, PlayerColor.black, 1, 1);
    this.addChecker(game, PlayerColor.black, 5, 1);

    this.addChecker(game, PlayerColor.black, 25, 13);

    this.addChecker(game, PlayerColor.white, 0, 14);
    this.addChecker(game, PlayerColor.white, 25, 1);

    AppState.Singleton.myColor.setValue(PlayerColor.black);
    AppState.Singleton.game.setValue(game);
  }

  blockedExample() {
    const game: GameDto = this.emptyGame();
    this.addChecker(game, PlayerColor.black, 1, 2);
    this.addChecker(game, PlayerColor.black, 25, 13);

    this.addChecker(game, PlayerColor.white, 2, 2);
    this.addChecker(game, PlayerColor.white, 3, 2);
    this.addChecker(game, PlayerColor.white, 4, 2);
    this.addChecker(game, PlayerColor.white, 5, 2);
    this.addChecker(game, PlayerColor.white, 6, 2);
    this.addChecker(game, PlayerColor.white, 7, 5);

    AppState.Singleton.myColor.setValue(PlayerColor.black);
    AppState.Singleton.game.setValue(game);
  }
}
