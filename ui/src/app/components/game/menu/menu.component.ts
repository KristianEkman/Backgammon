import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss']
})
export class MenuComponent {
  @Output() flip = new EventEmitter<void>();
  @Output() resign = new EventEmitter<void>();
  open = false;

  openClick(): void {
    console.log('open');
    this.open = true;
  }

  closeClick(): void {
    this.open = false;
  }

  flipClick(): void {
    this.open = false;
    this.flip.emit();
  }

  resignClick(): void {
    this.open = false;
    this.resign.emit();
  }
}
