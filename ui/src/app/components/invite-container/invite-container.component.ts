import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { UserDto } from 'src/app/dto';
import { InviteResponseDto } from 'src/app/dto/rest';
import { InviteService } from 'src/app/services';
import { AppStateService } from 'src/app/state/app-state.service';
import { Keys } from 'src/app/utils';

@Component({
  selector: 'app-invite-container',
  templateUrl: './invite-container.component.html',
  styleUrls: ['./invite-container.component.scss']
})
export class InviteContainerComponent implements OnInit {
  invite$: Observable<InviteResponseDto> | null = null;
  inviteId = '';
  user$: Observable<UserDto>;

  constructor(
    private router: Router,
    private inviteService: InviteService,
    private appState: AppStateService
  ) {
    this.invite$ = this.inviteService.createInvite();
    this.user$ = this.appState.user.observe();
  }

  ngOnInit(): void {
    this.inviteId = this.router.parseUrl(this.router.url).queryParams[
      Keys.inviteId
    ];
  }

  startInvitedGame(id: string): void {
    this.router.navigateByUrl('game?gameId=' + id);
  }

  cancelInvite(): void {
    this.router.navigateByUrl('lobby');
  }
}
