import { Component, EventEmitter, Input, Output } from '@angular/core';
import { UserDto } from 'src/app/dto';

@Component({
  selector: 'app-account-menu',
  templateUrl: './account-menu.component.html',
  styleUrls: ['./account-menu.component.scss']
})
export class AccountMenuComponent {
  @Input() user: UserDto | null = null;
  @Output() logout = new EventEmitter<void>();
  @Output() onEditAccount = new EventEmitter<void>();
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

  get Name(): string {
    if (!this.user) {
      return 'Guest';
    }
    return this.user.name;
  }

  editAccount(): void {
    this.onEditAccount.emit();
  }
}
