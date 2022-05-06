import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { addDays, format } from 'date-fns';
import { Observable } from 'rxjs';
import { MessageType, PlayedGameListDto, SummaryDto } from 'src/app/dto';
import { MassMailDto } from 'src/app/dto/message';
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
    private messageService: MessageService,
    private fb: FormBuilder
  ) {
    this.playedGames$ = AppState.Singleton.playedGames.observe();

    this.summary$ = this.adminSerivce.getSummary();
    this.formGroup = this.fb.group({
      view: ['summary']
    });
  }
  allGames = false;
  playedGames$: Observable<PlayedGameListDto>;
  summary$: Observable<SummaryDto>;
  formGroup: FormGroup;

  loadMore(): void {
    this.adminSerivce.loadPlayedGames();
  }
  ngOnInit(): void {}

  addSharePrompts(): void {
    this.messageService.addallsharepromptmessages();
  }

  sendMessages(type: MassMailDto): void {
    this.messageService.sendMessages(type);
  }

  get view(): string {
    return this.formGroup?.get('view')?.value;
  }

  loadGames(): void {
    this.adminSerivce.loadPlayedGames();
  }

  reload() {
    AppState.Singleton.playedGames.setValue({ games: [] });
    this.adminSerivce.loadPlayedGames();
  }
}
