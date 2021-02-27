import { Component, EventEmitter, Output } from '@angular/core';
import {
  FacebookLoginProvider,
  GoogleLoginProvider
} from 'angularx-social-login';

@Component({
  selector: 'app-login-buttons',
  templateUrl: './login-buttons.component.html',
  styleUrls: ['./login-buttons.component.scss']
})
export class LoginButtonsComponent {
  @Output() onLogin = new EventEmitter<string>();

  facebookLoginClick(): void {
    this.onLogin.emit(FacebookLoginProvider.PROVIDER_ID);
  }

  googleLoginClick(): void {
    this.onLogin.emit(GoogleLoginProvider.PROVIDER_ID);
    // this.authService.signIn(GoogleLoginProvider.PROVIDER_ID);
  }
}
