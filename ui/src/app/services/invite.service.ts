import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { InviteResponseDto } from '../dto/rest';

@Injectable({
  providedIn: 'root'
})
/**
 * General non websockets rest client.
 */
export class InviteService {
  url: string;
  constructor(private httpClient: HttpClient) {
    this.url = `${environment.apiServiceUrl}/invite`;
  }

  createInvite(): Observable<InviteResponseDto> {
    return this.httpClient
      .get(`${this.url}/create`)
      .pipe(map((dto) => dto as InviteResponseDto));
  }
}
