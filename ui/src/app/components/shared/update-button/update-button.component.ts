import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-update-button',
  standalone: true,
  templateUrl: './update-button.component.html',
  styleUrls: ['./update-button.component.scss'],
  imports: [TranslateModule]
})
export class UpdateButtonComponent {
  @Input() updateAvailable: boolean | null = false;
  @Output() update = new EventEmitter<void>();

  updateClick() {
    this.update.emit();
  }
}
