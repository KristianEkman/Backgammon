import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import {
  ReactiveFormsModule,
  UntypedFormBuilder,
  UntypedFormGroup
} from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { PlayedGameListDto, SummaryDto } from 'src/app/dto';
import { MassMailDto } from 'src/app/dto/message';
import { MessageService } from 'src/app/services';
import { AdminService } from 'src/app/services/admin.service';
import { AppStateService } from 'src/app/state/app-state.service';
import { AdminSummaryComponent } from './admin-summary/admin-summary.component';
import { PlayedGamesComponent } from './played-games/played-games.component';
import { MailingComponent } from './mailing/mailing.component';

@Component({
  selector: 'app-admin-container',
  standalone: true,
  templateUrl: './admin-container.component.html',
  styleUrls: ['./admin-container.component.scss'],
  imports: [
    CommonModule,
    AdminSummaryComponent,
    PlayedGamesComponent,
    MailingComponent,
    ReactiveFormsModule
  ]
})
export class AdminContainerComponent {
  constructor(
    private adminSerivce: AdminService,
    public router: Router,
    private messageService: MessageService,
    private fb: UntypedFormBuilder,
    private appState: AppStateService
  ) {
    this.playedGames$ = this.appState.playedGames.observe();

    this.summary$ = this.adminSerivce.getSummary();
    this.formGroup = this.fb.group({
      view: ['summary']
    });
  }
  allGames = false;
  playedGames$: Observable<PlayedGameListDto>;
  summary$: Observable<SummaryDto>;
  formGroup: UntypedFormGroup;

  loadMore(): void {
    this.adminSerivce.loadPlayedGames();
  }

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
    this.appState.playedGames.setValue({ games: [] });
    this.adminSerivce.loadPlayedGames();
  }
}
