import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { MessageDto, UserDto } from 'src/app/dto';
import {
  AccountService,
  MessageService,
  AppUpdateService
} from 'src/app/services';
import { AppStateService } from 'src/app/state/app-state.service';
import { ButtonComponent } from '../shared/button/button.component';
import { LoginButtonsComponent } from './login-buttons/login-buttons.component';
import { ToplistContainerComponent } from '../toplist-container/toplist-container.component';
import { ChatContainerComponent } from '../chat-container/chat-container.component';
import { TranslateModule } from '@ngx-translate/core';
import { AccountMenuComponent } from '../account/account-menu/account-menu.component';
import { GoldButtonComponent } from '../shared/gold-button/gold-button.component';
import { NewMessagesComponent } from '../shared/new-messages/new-messages.component';

@Component({
  selector: 'app-lobby-container',
  standalone: true,
  templateUrl: './lobby-container.component.html',
  styleUrls: ['./lobby-container.component.scss'],
  imports: [
    CommonModule,
    ButtonComponent,
    LoginButtonsComponent,
    ToplistContainerComponent,
    ChatContainerComponent,
    TranslateModule,
    AccountMenuComponent,
    GoldButtonComponent,
    NewMessagesComponent
  ]
})
export class LobbyContainerComponent implements OnInit {
  constructor(
    public router: Router,
    private accountService: AccountService,
    private messageService: MessageService,
    private updateService: AppUpdateService,
    private appState: AppStateService
  ) {
    this.user$ = this.appState.user.observe();
    this.messages$ = this.appState.messages.observe();
    this.hasNewVersion$ = this.appState.newVersion.observe();
    this.isAdmin$ = this.user$.pipe(map((u) => u?.isAdmin));
  }

  user$: Observable<UserDto>;
  messages$: Observable<MessageDto[]>;
  hasNewVersion$: Observable<boolean>;
  isAdmin$: Observable<boolean>;

  toplist = false;
  loginClicked = false;
  v1 = 0;
  v2 = 0;
  showLoginTip = false;

  ngOnInit(): void {
    this.accountService.repair();

    if (this.accountService.isLoggedIn()) {
      setTimeout(() => {
        // Wait some ms or I get that funny state was changed angualar exception.
        this.messageService.loadMessages();
      }, 100);
    }
  }

  loginPassword(): void {
    this.router.navigate(['password']);
  }

  loginGoogle(token: string) {
    this.loginClicked = true;
    this.accountService.signIn(token, 'GOOGLE');
  }

  logout(): void {
    this.accountService.signOut();
    location.href = '/';
    // this.authService.signOut(true);
  }

  playClick(): void {
    if (!this.isLoggedIn()) {
      this.showLoginTip = true;
      return;
    }
    this.router.navigate(['game'], {
      queryParams: { playAi: false, forGold: true }
    });
  }

  inviteFriendClick(): void {
    this.router.navigateByUrl('invite');
  }

  practiceClick(): void {
    this.router.navigate(['game'], {
      queryParams: { playAi: true, forGold: false }
    });
  }

  editClick(): void {
    this.router.navigate(['game'], {
      queryParams: { editing: true }
    });
  }

  tutorialClick(): void {
    this.router.navigate(['game'], {
      queryParams: { playAi: false, forGold: false, tutorial: true }
    });
  }

  startInvitedGame(id: string): void {
    this.router.navigateByUrl('game?gameId=' + id);
  }

  acceptInviteClick(): void {}

  topListBannerClick(): void {
    this.router.navigateByUrl('/toplist');
  }

  isLoggedIn(): boolean {
    return !!this.appState.user.getValue();
  }

  getGold(): void {
    this.accountService.getGold();
  }

  updateApp() {
    this.updateService.update();
  }

  feedbackClick() {
    this.router.navigateByUrl('/feedback');
  }
}
