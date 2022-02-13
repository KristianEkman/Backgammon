import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FeedbackDto } from 'src/app/dto';

@Component({
  selector: 'app-feedback-list',
  templateUrl: './feedback-list.component.html',
  styleUrls: ['./feedback-list.component.scss']
})
export class FeedbackListComponent {
  constructor() {}

  @Input() list: FeedbackDto[] | null = [];
  @Output() viewForm = new EventEmitter<void>();
  @Output() loadMore = new EventEmitter<void>();

  viewPostForm() {
    this.viewForm.emit();
  }

  onScroll(event: Event): void {
    const div = event.target as HTMLDivElement;
    if (div.scrollTop + div.clientHeight + 1 >= div.scrollHeight) {
      this.loadMore.emit();
    }
  }
}
