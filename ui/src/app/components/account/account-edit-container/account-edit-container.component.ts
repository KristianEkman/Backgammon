import { ChangeDetectorRef, Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { UserDto } from 'src/app/dto';
import { AccountService } from 'src/app/services';
import { AppState } from 'src/app/state/app-state';
import { Busy } from 'src/app/state/busy';
import { Theme } from '../theme/theme';
import { Language } from '../../../utils';

@Component({
  selector: 'app-account-edit-container',
  templateUrl: './account-edit-container.component.html',
  styleUrls: ['./account-edit-container.component.scss']
})
export class AccountEditContainerComponent {
  constructor(
    private service: AccountService,
    private formBuidler: FormBuilder,
    private router: Router,
    private translateService: TranslateService,
    private changeDetector: ChangeDetectorRef
  ) {
    this.user = AppState.Singleton.user.getValue();

    this.formGroup = this.formBuidler.group({
      name: [this.user.name, [Validators.required, Validators.maxLength(100)]],
      emailNotification: [this.user.emailNotification],
      preferredLanguage: [this.user.preferredLanguage],
      theme: [this.user.theme],
      showPhoto: [this.user.showPhoto]
    });

    this.formGroup.get('theme')?.valueChanges.subscribe((theme) => {
      Theme.change(theme);
    });

    this.formGroup.get('preferredLanguage')?.valueChanges.subscribe((lang) => {
      this.translateService.use(lang ?? 'en');
    });

    AppState.Singleton.user.observe().subscribe((userDto) => {
      this.user = userDto;
      this.changeDetector.detectChanges();
      if (userDto) {
        this.formGroup.patchValue({
          name: userDto.name,
          emailNotification: userDto.emailNotification,
          preferredLanguage: userDto.preferredLanguage,
          theme: userDto.theme,
          showPhoto: userDto.showPhoto
        });
      }
    });
  }

  formGroup: FormGroup;
  user: UserDto | null = null;
  confirm = false;
  Language = Language;

  submit(): void {
    if (this.formGroup.valid) {
      const user = { ...this.user, ...this.formGroup.value };
      Busy.show();
      this.service.saveUser(user).subscribe(() => {
        Busy.hide();
        this.router.navigateByUrl('/lobby');
      });
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

  nameMissing(): boolean {
    return this.formGroup.get('name')?.errors?.required;
  }
}
