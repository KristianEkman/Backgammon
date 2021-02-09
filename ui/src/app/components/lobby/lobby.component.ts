import { Component, OnInit } from '@angular/core';
import { GameService } from 'src/services';

@Component({
  selector: 'app-lobby',
  templateUrl: './lobby.component.html',
  styleUrls: ['./lobby.component.scss']
})
export class LobbyComponent implements OnInit {
  constructor(private gameService: GameService) {}

  ngOnInit(): void {}

  playAi(): void {
    this.gameService.NewAiGame();
  }
}
