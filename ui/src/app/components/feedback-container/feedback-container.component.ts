import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { FeedbackDto } from 'src/app/dto';
import { FeedbackService } from 'src/app/services';
import { AppState } from 'src/app/state/app-state';

@Component({
  selector: 'app-feedback-container',
  templateUrl: './feedback-container.component.html',
  styleUrls: ['./feedback-container.component.scss']
})
export class FeedbackContainerComponent {
  constructor(private service: FeedbackService) {
    this.list$ = AppState.Singleton.feedbackList.observe();
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