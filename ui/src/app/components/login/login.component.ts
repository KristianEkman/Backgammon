import { Component, OnInit } from '@angular/core';
import {
  FacebookLoginProvider,
  GoogleLoginProvider,
  SocialAuthService
} from 'angularx-social-login';
import { UserDto } from 'src/app/dto';
import { AccountService } from 'src/app/services';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  constructor(
    private authService: SocialAuthService,
    private accountService: AccountService
  ) {}

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
      this.accountService.SignIn(userDto, user.idToken);
    });
  }

  facebookLogin(): void {
    this.authService.signIn(FacebookLoginProvider.PROVIDER_ID);
  }

  googleLogin(): void {
    this.authService.signIn(GoogleLoginProvider.PROVIDER_ID);
  }
}
