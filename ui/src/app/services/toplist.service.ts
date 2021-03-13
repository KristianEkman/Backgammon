import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Toplist } from '../dto';

@Injectable({
  providedIn: 'root'
})
export class ToplistService {
  url: string;
  constructor(private httpClient: HttpClient) {
    this.url = `${environment.apiServiceUrl}/toplist`;
  }

  getToplist(): Observable<Toplist> {
    return this.httpClient.get(this.url).pipe(map((x) => x as Toplist));
  }
}
