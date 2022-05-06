import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { finalize, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { PlayedGameListDto, SummaryDto } from '../dto';
import { AppState } from '../state/app-state';
import { Busy } from '../state/busy';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  url: string;

  constructor(private httpClient: HttpClient) {
    this.url = `${environment.apiServiceUrl}/admin`;
  }

  loadPlayedGames(): void {
    Busy.showNoOverlay();
    const games = AppState.Singleton.playedGames.getValue();
    const skip = games?.games?.length ?? 0;
    this.httpClient
      .get(`${this.url}/allgames?skip=${skip}`)
      .pipe(
        map((data) => data as PlayedGameListDto),
        finalize(() => {
          Busy.hide();
        })
      )
      .subscribe((list) => {
        if (list.games) {
          const oldState = AppState.Singleton.playedGames.getValue();
          const newList = [...oldState.games, ...list.games];
          AppState.Singleton.playedGames.setValue({
            ...oldState,
            games: newList
          });
        }
      });
  }

  getSummary(): Observable<SummaryDto> {
    return this.httpClient.get(`${this.url}/summary`).pipe(
      map((data) => data as SummaryDto),
      finalize(() => {
        Busy.hide();
      })
    );
  }
}
