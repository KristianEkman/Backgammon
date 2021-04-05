import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { PlayedGameListDto, SummaryDto } from 'src/app/dto';
import { AdminService } from 'src/app/services/admin.service';
import { AppState } from 'src/app/state/app-state';

@Component({
  selector: 'app-admin-container',
  templateUrl: './admin-container.component.html',
  styleUrls: ['./admin-container.component.scss']
})
export class AdminContainerComponent implements OnInit {
  constructor(private adminSerive: AdminService, public router: Router) {
    this.playedGames$ = AppState.Singleton.playedGames.observe();
    this.adminSerive.loadPlayedGames('1900-01-01');
    this.summary$ = this.adminSerive.getSummary();
  }
  allGames = false;
  playedGames$: Observable<PlayedGameListDto>;
  summary$: Observable<SummaryDto>;

  onLoadAfter(date: string): void {
    this.adminSerive.loadPlayedGames(date);
  }
  ngOnInit(): void {}
}
