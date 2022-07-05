import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-feedback',
  templateUrl: './feedback.component.html',
  styleUrls: ['./feedback.component.scss']
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
