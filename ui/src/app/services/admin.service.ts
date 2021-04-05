import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { finalize, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { PlayedGameListDto } from '../dto';
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

  loadPlayedGames(afterDate: string): void {
    Busy.showNoOverlay();
    this.httpClient
      .get(`${this.url}?afterDate=${afterDate}`)
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
}
