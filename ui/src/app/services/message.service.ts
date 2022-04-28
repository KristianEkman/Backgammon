import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { finalize, map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { MassMailDto } from '../dto/message';
import { MessageDto } from '../dto/message/messageDto';
import { AppState } from '../state/app-state';
import { Busy } from '../state/busy';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  url: string;
  constructor(private http: HttpClient) {
    this.url = `${environment.apiServiceUrl}/message`;
  }

  loadMessages(): void {
    Busy.showNoOverlay();
    this.http
      .get(`${this.url}/users`)
      .pipe(
        map((data) => data as MessageDto[]),
        finalize(() => {
          Busy.hide();
        })
      )
      .subscribe((messages) => {
        AppState.Singleton.messages.setValue(messages);
      });
  }

  deleteMessage(id: number): void {
    this.http.delete(this.url + '/delete?id=' + id).subscribe(() => {
      const messages = AppState.Singleton.messages.getValue();
      const filtered = messages.filter((m) => m.id !== id);
      AppState.Singleton.messages.setValue(filtered);
    });
  }

  addallsharepromptmessages(): void {
    this.http.put(this.url + '/addallsharepromptmessages', {}).subscribe();
  }

  sendMessages(dto: MassMailDto): void {
    Busy.show();
    this.http
      .post(this.url + '/sendToAll', dto)
      .pipe(take(1))
      .subscribe(() => {
        Busy.hide();
      });
  }
}
