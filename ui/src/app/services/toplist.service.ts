import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Toplist } from '../dto';
import { AppState } from '../state/app-state';

@Injectable({
  providedIn: 'root'
})
export class ToplistService {
  url: string;
  constructor(private httpClient: HttpClient) {
    this.url = `${environment.apiServiceUrl}/toplist`;
  }

  loadToplist(): void {
    this.httpClient
      .get(this.url)
      .pipe(map((data) => data as Toplist))
      .subscribe((toplist) => {
        if (!toplist.results.find((t) => t.you)) {
          toplist.results.push(toplist.you); // add you last if not on top 10.
        }
        AppState.Singleton.toplist.setValue(toplist);
      });
  }
}
