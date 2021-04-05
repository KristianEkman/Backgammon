import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { PlayedGameListDto } from 'src/app/dto';
import { AdminService } from 'src/app/services/admin.service';
import { AppState } from 'src/app/state/app-state';

@Component({
  selector: 'app-admin-container',
  templateUrl: './admin-container.component.html',
  styleUrls: ['./admin-container.component.scss']
})
export class AdminContainerComponent implements OnInit {
  constructor(private adminSerive: AdminService) {
    this.playedGames$ = AppState.Singleton.playedGames.observe();
    this.adminSerive.loadPlayedGames('1900-01-01');
  }

  playedGames$: Observable<PlayedGameListDto>;
  onLoadAfter(date: string): void {
    this.adminSerive.loadPlayedGames(date);
  }
  ngOnInit(): void {}
}
