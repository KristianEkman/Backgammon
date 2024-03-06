import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-home-button',
  standalone: true,
  templateUrl: './home-button.component.html',
  styleUrls: ['./home-button.component.scss'],
  imports: [CommonModule, TranslateModule]
})
export class HomeButtonComponent {
  constructor(public router: Router) {}

  get Visible(): boolean {
    if (this.router.url === '/') return false;

    const parsed = this.router.parseUrl(this.router.url);
    if (parsed.queryParams['tutorial'] === 'true') {
      return true;
    }

    const hiddenOn = ['/lobby', '/game'];
    for (let i = 0; i < hiddenOn.length; i++) {
      const route = hiddenOn[i];
      if (this.router.url.startsWith(route)) {
        return false;
      }
    }
    return true;
  }
}
