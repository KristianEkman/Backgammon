import { Component, EventEmitter, Input, Output } from '@angular/core';
import { UserDto } from 'src/app/dto';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent {
  @Input() user: UserDto | null = null;
  @Output() logout = new EventEmitter<void>();
  toggle = false;

  logoutClick(): void {
    this.toggle = false;
    this.logout.emit();
  }

  nameClick(): void {
    if (this.user) {
      this.toggle = !this.toggle;
    } else {
      this.toggle = false;
    }
  }

  getName(): string {
    if (!this.user) {
      return 'Guest';
    }
    return this.user.name;
  }
}
