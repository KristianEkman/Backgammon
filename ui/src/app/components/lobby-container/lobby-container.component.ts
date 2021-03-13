import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SocialAuthService } from 'angularx-social-login';
import { Observable } from 'rxjs';
import { UserDto } from 'src/app/dto';
import { InviteResponseDto } from 'src/app/dto/rest';
import { AccountService, InviteService } from 'src/app/services';
import { AppState } from 'src/app/state/app-state';
import { Keys } from 'src/app/utils';

@Component({
  selector: 'app-lobby-container',
  templateUrl: './lobby-container.component.html',
  styleUrls: ['./lobby-container.component.scss']
})
export class LobbyContainerComponent implements OnInit {
  constructor(
    private router: Router,
    private authService: SocialAuthService,
    private accountService: AccountService,
    private inviteService: InviteService
  ) {}

  user$: Observable<UserDto> = AppState.Singleton.user.observe();
  invite$: Observable<InviteResponseDto> | null = null;
  playInvite = false;
  inviteId = '';
  toplist = false;

  ngOnInit(): void {
    this.authService.authState.subscribe((user) => {
      // send user, store secret user id
      const userDto = {
        name: user.name, // todo: What about first name and last name. Should I use them if one is missing?
        email: user.email,
        socialProviderId: user.id,
        socialProvider: user.provider,
        photoUrl: user.photoUrl
      } as UserDto;
      // google ande facebook have different names on the token field.
      this.accountService.signIn(userDto, user.idToken || user.authToken);
    });

    this.accountService.repair();

    this.inviteId = this.router.parseUrl(this.router.url).queryParams[
      Keys.inviteId
    ];
  }

  login(provider: string): void {
    AppState.Singleton.busy.setValue(true);
    this.authService.signIn(provider);
  }

  logout(): void {
    this.authService.signOut(true);
    this.accountService.signOut();
  }

  playClick(): void {
    this.router.navigateByUrl('game');
  }

  playFriendClick(): void {
    this.playInvite = true;
    this.invite$ = this.inviteService.createInvite();
  }

  startInvitedGame(id: string): void {
    this.router.navigateByUrl('game?gameId=' + id);
  }

  cancelInvite(): void {
    this.playInvite = false;
  }

  acceptInviteClick(): void {}

  showingTopList(flag: boolean): void {
    this.toplist = flag;
  }
}
