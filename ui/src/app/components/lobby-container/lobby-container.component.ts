import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { MessageDto, UserDto } from 'src/app/dto';
import {
  AccountService,
  MessageService,
  AppUpdateService
} from 'src/app/services';
import { AppStateService } from 'src/app/state/app-state.service';

@Component({
  selector: 'app-lobby-container',
  templateUrl: './lobby-container.component.html',
  styleUrls: ['./lobby-container.component.scss']
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

  loginFacebook(token: string): void {
    this.loginClicked = true;
    this.accountService.signIn(token, 'FACEBOOK');
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
