import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { map } from 'rxjs/operators';
import { UserDto } from '../dto/userDto';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  url: string;
  constructor(private http: HttpClient) {
    this.url = `${environment.apiServiceUrl}`;
  }

  public SignIn(userDto: UserDto, idToken: string): void {
    const options = {
      headers: { Authorization: idToken }
    };
    this.http
      .post<UserDto>(`${this.url}/signin`, userDto, options)
      .pipe(
        map((data) => {
          return data;
        })
      )
      .subscribe((data) => {
        console.log(data);
      });
  }
}
