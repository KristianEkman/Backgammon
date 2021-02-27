import { Component, OnInit } from '@angular/core';
import {
  FacebookLoginProvider,
  GoogleLoginProvider,
  SocialAuthService
} from 'angularx-social-login';
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
      console.log(user);
      // send user, store secret user id
      this.accountService.SignIn(user.idToken);
    });
  }

  facebookLogin(): void {
    this.authService.signIn(FacebookLoginProvider.PROVIDER_ID);
  }

  googleLogin(): void {
    this.authService.signIn(GoogleLoginProvider.PROVIDER_ID);
  }
}
