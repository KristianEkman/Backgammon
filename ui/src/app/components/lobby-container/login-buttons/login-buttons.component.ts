import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';
import {
  FacebookLoginProvider,
  GoogleLoginProvider
} from 'angularx-social-login';

@Component({
  selector: 'app-login-buttons',
  templateUrl: './login-buttons.component.html',
  styleUrls: ['./login-buttons.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginButtonsComponent {
  @Output() onLogin = new EventEmitter<string>();
  @Input() isLoggedIn: boolean | undefined = false;
  show = false;

  facebookLoginClick(): void {
    this.onLogin.emit(FacebookLoginProvider.PROVIDER_ID);
  }

  googleLoginClick(): void {
    this.onLogin.emit(GoogleLoginProvider.PROVIDER_ID);
  }

  passwordLoginClick(): void {
    this.onLogin.emit('PASSWORD');
  }
}
