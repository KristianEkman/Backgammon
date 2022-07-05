import { Component, OnInit } from '@angular/core';
import {
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { take } from 'rxjs/operators';
import {
  LocalAccountStatus,
  LocalLoginDto,
  NewLocalUserDto
} from 'src/app/dto';
import { AccountService } from 'src/app/services';

@Component({
  selector: 'app-password-container',
  templateUrl: './password-container.component.html',
  styleUrls: ['./password-container.component.scss']
})
export class PasswordContainerComponent {
  create = false;
  formGroup: UntypedFormGroup;
  submitClicked = false;
  nameExists = false;
  invalidLogin = false;

  constructor(
    private service: AccountService,
    private fb: UntypedFormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private translateService: TranslateService
  ) {
    this.formGroup = this.fb.group({
      name: ['', [Validators.required]],
      password: ['', [Validators.required]],
      repeat_password: ['']
    });

    this.formGroup.get('name')?.valueChanges.subscribe(() => {
      this.nameExists = false;
      this.invalidLogin = false;
    });

    this.formGroup.get('password')?.valueChanges.subscribe(() => {
      this.invalidLogin = false;
    });

    this.route.queryParams.pipe(take(1)).subscribe((q) => {
      this.create = q.create === 'true';
      if (this.create) {
        this.formGroup
          .get('password')
          ?.setValidators([Validators.required, Validators.minLength(8)]);
        this.formGroup.setValidators([this.passwordMatch.bind(this)]);
      }
    });
  }

  submit(): void {
    this.submitClicked = true;
    const me = this;
    if (!this.formGroup.valid) {
      // console.log(this.formGroup);
      return;
    }

    if (!this.formGroup) return;

    if (this.create) {
      const dto: NewLocalUserDto = {
        name: this.formGroup.get('name')?.value,
        passHash: this.hash(this.formGroup.get('password')?.value)
      };
      this.service.newLocalUser(dto).subscribe((status) => {
        if (status === LocalAccountStatus.success) {
          this.router.navigateByUrl('edit-user');
        }
        if (status === LocalAccountStatus.nameExists) {
          me.nameExists = true;
        }
      });
    } else {
      const dto: LocalLoginDto = {
        name: this.formGroup.get('name')?.value,
        passHash: this.hash(this.formGroup.get('password')?.value)
      };
      this.service.localLogin(dto).subscribe((result) => {
        if (result === LocalAccountStatus.success) {
          this.router.navigateByUrl('lobby');
        }
        if (result === LocalAccountStatus.invalidLogin) {
          this.invalidLogin = true;
        }
      });
    }
  }

  hash(value: string): number {
    var hash = 0,
      i,
      chr;

    for (i = 0; i < value.length; i++) {
      chr = value.charCodeAt(i);
      hash = (hash << 5) - hash + chr;
      hash |= 0; // Convert to 32bit integer
    }
    return hash;
  }

  get title(): string {
    if (this.create) {
      return this.translateService.instant('password.newplayer');
    }
    return this.translateService.instant('password.login');
  }

  error(field: string): boolean {
    return !this.formGroup.get(field)?.valid && this.submitClicked;
  }

  get messages(): any {
    return {
      form: {
        nopassmatch: this.translateService.instant('password.passdontmatch')
      },
      name: {
        required: this.translateService.instant('password.namerequired')
      },
      password: {
        required: this.translateService.instant('password.required'),
        minlength: this.translateService.instant('password.format')
      }
    };
  }

  formErrors(): string {
    const control = this.formGroup;

    if (control?.errors && this.submitClicked) {
      const firstError = Object.keys(control?.errors)[0];
      return this.messages['form'][firstError];
    }
    return '';
  }

  labelText(field: string): string {
    const control = this.formGroup.get(field);
    const trans = this.translateService.instant('password.' + field);
    if (!this.error(field)) return trans;

    if (control?.errors) {
      const firstError = Object.keys(control?.errors)[0];
      return this.messages[field][firstError];
    }
    return trans;
  }

  firstToUpper(text: string) {
    return text.charAt(0).toUpperCase() + text.slice(1);
  }

  createClicked(): void {
    this.router.navigate(['create-password'], {
      queryParams: { create: true }
    });
  }

  passwordMatch(): any {
    const p1 = this.formGroup.get('password')?.value;
    const p2 = this.formGroup.get('repeat_password')?.value;
    if (p1 !== p2) {
      return {
        nopassmatch: true
      };
    }
    return null;
  }
}
