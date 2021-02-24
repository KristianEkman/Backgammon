import { Component } from '@angular/core';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent {
  open = false;

  openClick(): void {
    console.log('open');
    this.open = true;
  }

  closeClick(): void {
    this.open = false;
  }
}
