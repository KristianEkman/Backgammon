import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home-button',
  templateUrl: './home-button.component.html',
  styleUrls: ['./home-button.component.scss']
})
export class HomeButtonComponent {
  constructor(public router: Router) {}

  get Visible(): boolean {
    const hiddenOn = ['/', '/lobby', '/game'];
    for (let i = 0; i < hiddenOn.length; i++) {
      const route = hiddenOn[i];
      if (this.router.url.startsWith(route)) {
        return false;
      }
    }
    return true;
  }
}
