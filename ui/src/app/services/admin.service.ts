import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { finalize, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { PlayedGameListDto, SummaryDto } from '../dto';
import { AppStateService } from '../state/app-state.service';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  url: string;

  constructor(
    private httpClient: HttpClient,
    private appState: AppStateService
  ) {
    this.url = `${environment.apiServiceUrl}/admin`;
  }

  loadPlayedGames(): void {
    this.appState.showBusyNoOverlay();
    const games = this.appState.playedGames.getValue();
    const skip = games?.games?.length ?? 0;
    this.httpClient
      .get(`${this.url}/allgames?skip=${skip}`)
      .pipe(
        map((data) => data as PlayedGameListDto),
        finalize(() => {
          this.appState.hideBusy();
        })
      )
      .subscribe((list) => {
        if (list.games) {
          const oldState = this.appState.playedGames.getValue();
          const newList = [...oldState.games, ...list.games];
          this.appState.playedGames.setValue({
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
        this.appState.hideBusy();
      })
    );
  }
}
