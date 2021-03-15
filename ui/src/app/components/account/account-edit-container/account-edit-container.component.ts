import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserDto } from 'src/app/dto';
import { AccountService } from 'src/app/services';
import { AppState } from 'src/app/state/app-state';

@Component({
  selector: 'app-account-edit-container',
  templateUrl: './account-edit-container.component.html',
  styleUrls: ['./account-edit-container.component.scss']
})
export class AccountEditContainerComponent {
  constructor(
    private service: AccountService,
    private formBuidler: FormBuilder,
    private router: Router
  ) {
    this.formGroup = this.formBuidler.group({
      name: ['', [Validators.required, Validators.maxLength(100)]]
    });

    AppState.Singleton.user.observe().subscribe((userDto) => {
      this.user = userDto;
      if (userDto) {
        this.formGroup.patchValue({
          name: userDto.name
        });
      }
    });
  }

  formGroup: FormGroup;
  user: UserDto | null = null;
  confirm = false;

  submit(): void {
    if (this.formGroup.valid) {
      const user = { ...this.user, ...this.formGroup.value };
      this.service.saveUser(user);
      this.router.navigateByUrl('/lobby');
    }
  }

  cancel(): void {
    this.router.navigateByUrl('/lobby');
  }

  deleteUser(): void {
    this.confirm = true;
  }

  doDeletion(): void {
    this.service.deleteUser();
  }
}
