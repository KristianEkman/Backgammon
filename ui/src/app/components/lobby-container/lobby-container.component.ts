import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SocialAuthService } from 'angularx-social-login';
import { Observable } from 'rxjs';
import { MessageDto, Toplist, UserDto } from 'src/app/dto';
import { InviteResponseDto } from 'src/app/dto/rest';
import {
  AccountService,
  InviteService,
  MessageService,
  ToplistService
} from 'src/app/services';
import { AppState } from 'src/app/state/app-state';
import { Busy } from 'src/app/state/busy';
import { Keys } from 'src/app/utils';

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
    private inviteService: InviteService,
    private topListService: ToplistService,
    private messageService: MessageService
  ) {
    this.user$ = AppState.Singleton.user.observe();
    this.toplist$ = AppState.Singleton.toplist.observe();
    this.messages$ = AppState.Singleton.messages.observe();
  }

  user$: Observable<UserDto>;
  invite$: Observable<InviteResponseDto> | null = null;
  toplist$: Observable<Toplist>;
  messages$: Observable<MessageDto[]>;

  playInvite = false;
  inviteId = '';
  toplist = false;

  ngOnInit(): void {
    this.authService.authState.subscribe((user) => {
      // send user, store secret user id
      if (user) {
        const userDto = {
          name: user.name, // todo: What about first name and last name. Should I use them if one is missing?
          email: user.email,
          socialProviderId: user.id,
          socialProvider: user.provider,
          photoUrl: user.photoUrl
        } as UserDto;
        // google ande facebook have different names on the token field.
        Busy.show();
        this.accountService.signIn(userDto, user.idToken || user.authToken);
      }
    });

    this.accountService.repair();

    this.inviteId = this.router.parseUrl(this.router.url).queryParams[
      Keys.inviteId
    ];

    if (this.accountService.isLoggedIn()) {
      this.topListService.loadToplist();
      this.messageService.loadMessages();
    }
  }

  login(provider: string): void {
    this.authService.signIn(provider);
  }

  logout(): void {
    this.accountService.signOut();

    this.authService.signOut(true);
  }

  playClick(): void {
    this.router.navigateByUrl('game');
  }

  playFriendClick(): void {
    this.playInvite = true;
    this.invite$ = this.inviteService.createInvite();
  }

  playAiClick(): void {
    this.router.navigate(['game'], { queryParams: { playAi: true } });
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
