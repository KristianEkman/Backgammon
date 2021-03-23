import {
  AfterViewInit,
  Component,
  Input,
  OnChanges,
  SimpleChanges
} from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AppState } from 'src/app/state/app-state';
import { ErrorState } from 'src/app/state/ErrorState';

@Component({
  selector: 'app-error-handler',
  templateUrl: './error-handler.component.html',
  styleUrls: ['./error-handler.component.scss']
  // changeDetection: ChangeDetectionStrategy.OnPush
})
export class ErrorHandlerComponent implements AfterViewInit, OnChanges {
  textVisible = false;
  @Input() errors: ErrorState | null = new ErrorState('');
  formGroup: FormGroup;

  constructor(private fb: FormBuilder) {
    this.showErrors.bind(this);
    this.formGroup = fb.group({
      errors: ['']
    });
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  ngOnChanges(changes: SimpleChanges): void {
    // console.log(changes);
    this.setTextAreaValue();
  }

  ngAfterViewInit(): void {
    // setInterval(() => {
    //   console.log(this);
    //   throw new Error('interval');
    // }, 3000);
  }

  showErrors(): void {
    this.textVisible = true;
    this.setTextAreaValue();
  }

  setTextAreaValue(): void {
    this.formGroup.patchValue({ errors: this.errors?.message });
  }

  sendErrors(): void {
    this.clearErrors();
  }

  clearErrors(): void {
    this.textVisible = false;
    AppState.Singleton.errors.clearValue();
  }
}
