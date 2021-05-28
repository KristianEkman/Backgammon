import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SocialAuthService, SocialUser } from 'angularx-social-login';
import { Observable } from 'rxjs';
import { MessageDto, UserDto } from 'src/app/dto';
import { AccountService, MessageService } from 'src/app/services';
import { AppState } from 'src/app/state/app-state';
import { Busy } from 'src/app/state/busy';

@Component({
  selector: 'app-lobby-container',
  templateUrl: './lobby-container.component.html',
  styleUrls: ['./lobby-container.component.scss']
})
export class LobbyContainerComponent implements OnInit {
  constructor(
    public router: Router,
    private authService: SocialAuthService,
    private accountService: AccountService,
    private messageService: MessageService
  ) {
    this.user$ = AppState.Singleton.user.observe();
    this.messages$ = AppState.Singleton.messages.observe();
  }

  user$: Observable<UserDto>;
  messages$: Observable<MessageDto[]>;

  toplist = false;
  loginClicked = false;
  playAiAsGuestMessage = false;
  v1 = 0;
  v2 = 0;

  ngOnInit(): void {
    this.authService.authState.subscribe((user: SocialUser) => {
      if (!this.loginClicked) {
        // This gets fired sometimes for unknow reasons.
        // Makes sure it only does things when I want to.
        return;
      }
      this.loginClicked = false;
      // send user, store secret user id
      if (user) {
        const userDto = {
          name: user.name, // todo: What about first name and last name. Should I use them if one is missing?
          email: user.email,
          socialProviderId: user.id,
          socialProvider: user.provider,
          photoUrl: user.photoUrl
        } as UserDto;
        // google and facebook have different names on the token field.
        Busy.show();
        this.accountService.signIn(userDto, user.idToken || user.authToken);
      }
    });
    this.accountService.repair();

    if (this.accountService.isLoggedIn()) {
      setTimeout(() => {
        // Wait some ms or I get that funny state was changed angualar exception.
        this.messageService.loadMessages();
      }, 100);
    }
  }

  login(provider: string): void {
    this.loginClicked = true;
    this.authService.signIn(provider);
  }

  logout(): void {
    this.accountService.signOut();
    this.authService.signOut(true);
  }

  playClick(): void {
    this.router.navigateByUrl('game');
  }

  inviteFriendClick(): void {
    this.router.navigateByUrl('invite');
  }

  playAiClick(): void {
    if (this.playAiAsGuestMessage || this.isLoggedIn()) {
      this.router.navigate(['game'], { queryParams: { playAi: true } });
      return;
    }
    this.playAiAsGuestMessage = true;

    // The user has to click button twice.
  }

  startInvitedGame(id: string): void {
    this.router.navigateByUrl('game?gameId=' + id);
  }

  acceptInviteClick(): void {}

  topListBannerClick(): void {
    this.router.navigateByUrl('/toplist');
  }

  isLoggedIn(): boolean {
    return !!AppState.Singleton.user.getValue();
  }

  getGold(): void {
    this.accountService.getGold();
  }
}
