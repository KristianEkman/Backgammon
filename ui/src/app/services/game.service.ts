import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { AppState } from 'src/app/state/game-state';
import { finalize, map } from 'rxjs/operators';
import { ActionDto } from '../dto/Actions/actionDto';

@Injectable({
  providedIn: 'root'
})
export class GameService {
  url: string;
  constructor(private http: HttpClient) {
    this.url = `${environment.apiServiceUrl}/game`;
  }

  public NewAiGame(): void {
    this.http.get(`${this.url}/newai`).pipe(
      map((dto: unknown) => {
        console.log({ dto });
      }),
      finalize(() => {
        AppState.Singleton.busy.setValue(false);
      })
    );
  }
}
