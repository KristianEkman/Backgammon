import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { GameDto } from 'src/app/dto/gameDto';
import { SocketsService } from 'src/app/services';
import { AppState } from 'src/app/state/game-state';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.scss']
})
export class GameComponent {
  constructor(private socket: SocketsService) {
    this.gameDto$ = AppState.Singleton.game.changed;
    socket.connect();
  }

  gameDto$: Observable<GameDto>;
}
