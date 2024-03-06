import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-menu',
  standalone: true,
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss'],
  imports: [CommonModule, TranslateModule]
})
export class MenuComponent {
  @Output() rotate = new EventEmitter<void>();
  @Output() flip = new EventEmitter<void>();
  @Output() resign = new EventEmitter<void>();
  open = false;

  openClick(): void {
    this.open = true;
  }

  closeClick(): void {
    this.open = false;
  }

  flipClick(): void {
    this.open = false;
    this.flip.emit();
  }

  rotateClick(): void {
    this.open = false;
    this.rotate.emit();
  }

  resignClick(): void {
    this.open = false;
    this.resign.emit();
  }
}
