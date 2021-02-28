import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { SocialAuthService } from 'angularx-social-login';
import { CookieService } from 'ngx-cookie-service';
import { Observable } from 'rxjs';
import { UserDto } from 'src/app/dto';
import { AccountService } from 'src/app/services';
import { AppState } from 'src/app/state/app-state';
import { Cookies } from 'src/app/utils';

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
    private cookieService: CookieService
  ) {}

  user$: Observable<UserDto> = AppState.Singleton.user.observe();

  ngOnInit(): void {
    this.authService.authState.subscribe((user) => {
      // console.log(user);
      // send user, store secret user id
      const userDto = {
        name: user.name, // todo: What about first name and last name. Should I use them if one is missing?
        email: user.email,
        socialProviderId: user.id,
        socialProvider: user.provider,
        photoUrl: user.photoUrl
      } as UserDto;
      this.accountService.signIn(userDto, user.idToken);
    });

    const sUser = this.cookieService.get(Cookies.loginKey);
    if (sUser) {
      const userDto = JSON.parse(sUser) as UserDto;
      AppState.Singleton.user.setValue(userDto);
    }
  }

  login(provider: string): void {
    this.authService.signIn(provider);
  }

  logout(): void {
    this.accountService.signOut();
  }

  playAsGuest(): void {
    this.router.navigateByUrl('game');
  }
}
