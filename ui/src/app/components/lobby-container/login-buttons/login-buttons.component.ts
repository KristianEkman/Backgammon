import {
  AfterViewInit,
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

@Component({
  selector: 'app-login-buttons',
  templateUrl: './login-buttons.component.html',
  styleUrls: ['./login-buttons.component.scss']
})
export class LoginButtonsComponent implements AfterViewInit {
  @Output() loggedinGoogle = new EventEmitter<string>();
  @Output() loggedinFacebook = new EventEmitter<string>();
  @Output() loggedinPassword = new EventEmitter<void>();
  @Input() isLoggedIn: boolean | undefined = false;

  show = false;
  facebookLoginResponse: any;

  ngAfterViewInit(): void {
    this.initGoogle();
    this.initFacebook();
  }

  initFacebook() {
    const FB = (window as any).FB;
    FB.getLoginStatus((response: any) => {
      this.facebookLoginResponse = response;
    });
  }

  initGoogle() {
    const google = (window as any).google;
    google.accounts.id.initialize({
      client_id:
        '296204915760-builmppcda4nq2t6gh3rgtiq5o4v6976.apps.googleusercontent.com',
      callback: this.handleCredentialResponse.bind(this)
    });
    const parentDiv = document.getElementById('googlebuttondiv');
    google.accounts.id.renderButton(parentDiv, {
      type: 'standard',
      theme: 'dark',
      size: 'large',
      text: 'sign_in_with',
      shape: 'pill',
      logo_alignement: 'left'
      // width: 100
    });
    // google.accounts.id.prompt(); // Displays the One Tap dialog
  }

  handleCredentialResponse(response: any) {
    this.show = false;
    this.loggedinGoogle.emit(response.credential);
  }

  passwordLoginClick(): void {
    this.show = false;
    this.loggedinPassword.emit();
  }

  facebookLoginClick() {
    this.show = false;
    const FB = (window as any).FB;
    if (
      this.facebookLoginResponse &&
      this.facebookLoginResponse.status === 'connected'
    ) {
      const token = this.facebookLoginResponse.authResponse.accessToken;
      this.loggedinFacebook.emit(token);
      return;
    }

    // calling login when connected will raise an error.
    FB.login((respons: any) => {
      if (
        respons?.status === 'connected' &&
        respons.authResponse?.accessToken
      ) {
        const token = respons.authResponse.accessToken;
        this.loggedinFacebook.emit(token);
      }
    });
  }
}
