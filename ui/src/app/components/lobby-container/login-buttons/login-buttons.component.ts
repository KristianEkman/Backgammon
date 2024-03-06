import {
  AfterViewInit,
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';
import { ButtonComponent } from '../../shared/button/button.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login-buttons',
  standalone: true,
  templateUrl: './login-buttons.component.html',
  styleUrls: ['./login-buttons.component.scss'],
  imports: [ButtonComponent, CommonModule]
})
export class LoginButtonsComponent implements AfterViewInit {
  @Output() loggedinGoogle = new EventEmitter<string>();
  @Output() loggedinPassword = new EventEmitter<void>();
  @Input() isLoggedIn: boolean | undefined = false;

  show = false;

  ngAfterViewInit(): void {
    this.initGoogle();
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
}
