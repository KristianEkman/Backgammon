import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { FeedbackDto } from 'src/app/dto';
import { FeedbackService } from 'src/app/services';
import { AppStateService } from 'src/app/state/app-state.service';
import { FeedbackComponent } from './feedback/feedback.component';
import { FeedbackListComponent } from './feedback-list/feedback-list.component';

@Component({
  selector: 'app-feedback-container',
  standalone: true,
  templateUrl: './feedback-container.component.html',
  styleUrls: ['./feedback-container.component.scss'],
  imports: [CommonModule, FeedbackComponent, FeedbackListComponent]
})
export class FeedbackContainerComponent {
  constructor(
    private service: FeedbackService,
    private appState: AppStateService
  ) {
    this.list$ = this.appState.feedbackList.observe();
  }

  viewForm = true;
  viewList = false;
  list$: Observable<FeedbackDto[]>;

  onFeedbackText(text: string) {
    this.service.postFeedBack(text);
    this.viewPostList();
  }

  viewPostList() {
    this.service.loadList();
    this.viewList = true;
    this.viewForm = false;
  }

  viewPostForm() {
    this.viewList = false;
    this.viewForm = true;
  }

  onLoadMore() {
    this.service.loadList();
  }
}
