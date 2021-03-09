import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SocialAuthService } from 'angularx-social-login';
import { Observable } from 'rxjs';
import { UserDto } from 'src/app/dto';
import { AccountService } from 'src/app/services';
import { AppState } from 'src/app/state/app-state';

@Component({
  selector: 'app-lobby-container',
  templateUrl: './lobby-container.component.html',
  styleUrls: ['./lobby-container.component.scss']
})
export class LobbyContainerComponent implements OnInit {
  constructor(
    private router: Router,
    private authService: SocialAuthService,
    private accountService: AccountService
  ) {}

  user$: Observable<UserDto> = AppState.Singleton.user.observe();

  ngOnInit(): void {
    this.authService.authState.subscribe((user) => {
      console.log(user);
      // send user, store secret user id
      // const userDto = {
      //   name: user.name, // todo: What about first name and last name. Should I use them if one is missing?
      //   email: user.email,
      //   socialProviderId: user.id,
      //   socialProvider: user.provider,
      //   photoUrl: user.photoUrl
      // } as UserDto;
      // this.accountService.signIn(userDto, user.idToken);
    });

    this.accountService.repair();
  }

  login(provider: string): void {
    AppState.Singleton.busy.setValue(true);
    this.authService.signIn(provider);
  }

  logout(): void {
    this.accountService.signOut();
  }

  playRandom(): void {
    this.router.navigateByUrl('game');
  }
}
