import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import {
  ReactiveFormsModule,
  UntypedFormBuilder,
  UntypedFormGroup,
  Validators
} from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-feedback',
  standalone: true,
  templateUrl: './feedback.component.html',
  styleUrls: ['./feedback.component.scss'],
  imports: [CommonModule, ReactiveFormsModule, TranslateModule]
})
export class FeedbackComponent {
  constructor(private formBuilder: UntypedFormBuilder) {
    this.formGroup = this.formBuilder.group({
      text: ['', [Validators.required, Validators.maxLength(200)]]
    });
  }

  @Output() feedbackText = new EventEmitter<string>();
  @Output() viewPosts = new EventEmitter<void>();

  formGroup: UntypedFormGroup;

  submit() {
    if (this.formGroup.valid) {
      this.feedbackText.emit(this.formGroup.get('text')?.value as string);
    }
  }

  get textLength(): number {
    return this.formGroup.get('text')?.value?.length ?? 0;
  }

  viewPostList() {
    this.viewPosts.emit();
  }
}
