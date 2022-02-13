import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { format, parseISO } from 'date-fns';
import { finalize, map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { FeedbackDto, PostFeedbackDto } from '../dto';
import { AppState } from '../state/app-state';
import { Busy } from '../state/busy';

@Injectable({
  providedIn: 'root'
})
export class FeedbackService {
  url: string;
  constructor(private http: HttpClient) {
    this.url = `${environment.apiServiceUrl}/feedback`;
  }

  postFeedBack(text: string) {
    Busy.showNoOverlay();
    const dto: PostFeedbackDto = { text: text };
    this.http
      .post(this.url, dto)
      .pipe(take(1))
      .subscribe(() => {
        const oldState = AppState.Singleton.feedbackList.getValue();
        const user = AppState.Singleton.user.getValue();
        const date = format(new Date(), 'MMMM dd, yyyy');
        const newPost: FeedbackDto = {
          id: 0,
          senderName: user.name,
          text: text,
          sent: date
        };
        const newList = [newPost, ...oldState];
        AppState.Singleton.feedbackList.setValue(newList);
        Busy.hide();
      });
  }

  loadList(): void {
    const skip = AppState.Singleton.feedbackList.getValue().length;
    Busy.showNoOverlay();
    this.http
      .get(`${this.url}?skip=${skip}`)
      .pipe(
        map((data) => data as FeedbackDto[]),
        finalize(() => {
          Busy.hide();
        })
      )
      .subscribe((list) => {
        if (list) {
          const oldState = AppState.Singleton.feedbackList.getValue();
          const newList = [...oldState, ...list];
          AppState.Singleton.feedbackList.setValue(newList);
        }
      });
  }
}
