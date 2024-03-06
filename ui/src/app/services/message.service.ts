import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { finalize, map, take } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { MassMailDto } from '../dto/message';
import { MessageDto } from '../dto/message/messageDto';
import { AppStateService } from '../state/app-state.service';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  url: string;
  constructor(
    private http: HttpClient,
    private appState: AppStateService
  ) {
    this.url = `${environment.apiServiceUrl}/message`;
  }

  loadMessages(): void {
    this.appState.showBusyNoOverlay();
    this.http
      .get(`${this.url}/users`)
      .pipe(
        map((data) => data as MessageDto[]),
        finalize(() => {
          this.appState.hideBusy();
        })
      )
      .subscribe((messages) => {
        this.appState.messages.setValue(messages);
      });
  }

  deleteMessage(id: number): void {
    this.http.delete(this.url + '/delete?id=' + id).subscribe(() => {
      const messages = this.appState.messages.getValue();
      const filtered = messages.filter((m) => m.id !== id);
      this.appState.messages.setValue(filtered);
    });
  }

  addallsharepromptmessages(): void {
    this.http.put(this.url + '/addallsharepromptmessages', {}).subscribe();
  }

  sendMessages(dto: MassMailDto): void {
    this.appState.showBusy();
    this.http
      .post(this.url + '/sendToAll', dto)
      .pipe(take(1))
      .subscribe(() => {
        this.appState.hideBusy();
      });
  }

  emailUnsubscribe(id: string): Observable<unknown> {
    return this.http.get(this.url + `/unsubscribe?id=${id}`);
  }
}
