import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { finalize, map } from 'rxjs/operators';
import { pipe } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  url: string;
  constructor(private http: HttpClient) {
    this.url = `${environment.apiServiceUrl}`;
  }

  public SignIn(idToken: string): void {
    const options = {
      headers: { Authorization: idToken }
    };

    this.http
      .get(`${this.url}/signin`, options)
      .pipe(
        map((data) => {
          console.log(data);
          return data;
        })
      )
      .subscribe((data) => {
        console.log(data);
      });
  }
}
