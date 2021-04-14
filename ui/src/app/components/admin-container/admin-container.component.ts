import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { addDays, format } from 'date-fns';
import { Observable } from 'rxjs';
import { PlayedGameListDto, SummaryDto } from 'src/app/dto';
import { MessageService } from 'src/app/services';
import { AdminService } from 'src/app/services/admin.service';
import { AppState } from 'src/app/state/app-state';

@Component({
  selector: 'app-admin-container',
  templateUrl: './admin-container.component.html',
  styleUrls: ['./admin-container.component.scss']
})
export class AdminContainerComponent implements OnInit {
  constructor(
    private adminSerivce: AdminService,
    public router: Router,
    private messageService: MessageService
  ) {
    this.playedGames$ = AppState.Singleton.playedGames.observe();
    const tomorrow = addDays(new Date(), 1);

    this.adminSerivce.loadPlayedGames(format(tomorrow, 'yyyy-MM-dd'));
    this.summary$ = this.adminSerivce.getSummary();
  }
  allGames = false;
  playedGames$: Observable<PlayedGameListDto>;
  summary$: Observable<SummaryDto>;

  onLoadAfter(date: string): void {
    this.adminSerivce.loadPlayedGames(date);
  }
  ngOnInit(): void {}

  addSharePrompts(): void {
    this.messageService.addallsharepromptmessages();
  }
}
