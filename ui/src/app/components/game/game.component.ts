import { Component } from '@angular/core';
import { SocketsService } from 'src/app/services';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.scss']
})
export class GameComponent {
  constructor(private socket: SocketsService) {}
}
