import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { format } from 'date-fns';
import { finalize, map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { FeedbackDto, PostFeedbackDto } from '../dto';
import { AppStateService } from '../state/app-state.service';

@Injectable({
  providedIn: 'root'
})
export class FeedbackService {
  url: string;
  constructor(
    private http: HttpClient,
    private appState: AppStateService
  ) {
    this.url = `${environment.apiServiceUrl}/feedback`;
  }

  postFeedBack(text: string) {
    this.appState.showBusyNoOverlay();
    const dto: PostFeedbackDto = { text: text };
    this.http
      .post(this.url, dto)
      .pipe(take(1))
      .subscribe(() => {
        const oldState = this.appState.feedbackList.getValue();
        const user = this.appState.user.getValue();
        const date = format(new Date(), 'MMMM dd, yyyy');
        const newPost: FeedbackDto = {
          id: 0,
          senderName: user.name,
          text: text,
          sent: date
        };
        const newList = [newPost, ...oldState];
        this.appState.feedbackList.setValue(newList);
        this.appState.hideBusy();
      });
  }

  loadList(): void {
    const skip = this.appState.feedbackList.getValue().length;
    this.appState.showBusyNoOverlay();

    this.http
      .get(`${this.url}?skip=${skip}`)
      .pipe(
        map((data) => data as FeedbackDto[]),
        finalize(() => {
          this.appState.hideBusy();
        })
      )
      .subscribe((list) => {
        if (list) {
          const oldState = this.appState.feedbackList.getValue();
          const newList = [...oldState, ...list];
          this.appState.feedbackList.setValue(newList);
        }
      });
  }
}
