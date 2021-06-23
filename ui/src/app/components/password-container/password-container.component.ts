import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
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
  formGroup: FormGroup;
  submitClicked = false;
  emailExists = false;
  invalidLogin = false;

  constructor(
    private service: AccountService,
    private fb: FormBuilder,
    route: ActivatedRoute,
    private router: Router
  ) {
    this.formGroup = this.fb.group({
      name: [''],
      email: ['', [Validators.required]],
      password: ['', [Validators.required]],
      password2: ['']
    });

    this.formGroup.get('email')?.valueChanges.subscribe(() => {
      this.emailExists = false;
      this.invalidLogin = false;
    });

    this.formGroup.get('password')?.valueChanges.subscribe(() => {
      this.invalidLogin = false;
    });

    route.queryParams.pipe(take(1)).subscribe((q) => {
      this.create = q.create === 'true';
      if (this.create) {
        this.formGroup.get('name')?.setValidators(Validators.required);
        this.formGroup
          .get('password')
          ?.setValidators([Validators.required, Validators.minLength(8)]);
        this.formGroup
          .get('password2')
          ?.setValidators([this.passwordMatch.bind(this)]);
      }
    });
  }

  submit(): void {
    this.submitClicked = true;
    const me = this;
    if (!this.formGroup.valid) return;

    if (!this.formGroup) return;

    if (this.create) {
      const dto: NewLocalUserDto = {
        name: this.formGroup.get('name')?.value,
        email: this.formGroup.get('email')?.value,
        passHash: this.hash(this.formGroup.get('password')?.value)
      };
      this.service.newLocalUser(dto).subscribe((status) => {
        if (status === LocalAccountStatus.success) {
          this.router.navigateByUrl('edit-user');
        }
        if (status === LocalAccountStatus.emailExists) {
          me.emailExists = true;
        }
      });
    } else {
      const dto: LocalLoginDto = {
        email: this.formGroup.get('email')?.value,
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
      return 'New Player';
    }
    return 'Login';
  }

  error(field: string): boolean {
    return !this.formGroup.get(field)?.valid && this.submitClicked;
  }

  labelText(field: string): string {
    const control = this.formGroup.get(field);
    if (!this.error(field)) return this.firstToUpper(field);

    const messages: any = {
      name: {
        required: 'Name is required'
      },
      email: {
        required: 'Email is required'
      },
      password: {
        required: 'Password is required',
        minlength: 'Password must have atleast 8 characters'
      },
      password2: {
        nopassmatch: 'Passwords don´t match'
      }
    };

    if (control?.errors) {
      const firstError = Object.keys(control?.errors)[0];
      return messages[field][firstError];
    }
    return field;
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
    const p2 = this.formGroup.get('password2')?.value;
    if (p1 !== p2) {
      return {
        nopassmatch: true
      };
    }
    return null;
  }
}
