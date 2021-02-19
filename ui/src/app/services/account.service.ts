import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { finalize, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  url: string;
  constructor(private http: HttpClient) {
    this.url = `${environment.apiServiceUrl}/game`;
  }

  public Login(): void {
    this.http.get(`${this.url}/login`).pipe(
      map((dto: unknown) => {
        console.log({ dto });
      }),
      finalize(() => {})
    );
  }
}
