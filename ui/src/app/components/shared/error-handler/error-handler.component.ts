import {
  AfterViewInit,
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges
} from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { ErrorReportDto } from 'src/app/dto';
import { AppStateService } from 'src/app/state/app-state.service';
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
  @Output() save = new EventEmitter<ErrorReportDto>();
  formGroup: UntypedFormGroup;

  constructor(
    private fb: UntypedFormBuilder,
    private appState: AppStateService
  ) {
    this.showErrors.bind(this);
    this.formGroup = this.fb.group({
      errors: ['']
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    // console.log(changes);
    this.setTextAreaValue();
  }

  ngAfterViewInit(): void {
    // setInterval(() => {
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
    const dto: ErrorReportDto = {
      error: this.formGroup.get('errors')?.value,
      reproduce: ''
    };

    this.save.emit(dto);
  }

  clearErrors(): void {
    this.textVisible = false;
    this.appState.errors.clearValue();
  }
}
