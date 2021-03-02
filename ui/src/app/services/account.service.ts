import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { map } from 'rxjs/operators';
import { UserDto } from '../dto/userDto';
import { AppState } from '../state/app-state';
import { CookieService } from 'ngx-cookie-service';
import { Cookies } from '../utils';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  url: string;
  constructor(private http: HttpClient, private cookies: CookieService) {
    this.url = `${environment.apiServiceUrl}`;
  }

  public signIn(userDto: UserDto, idToken: string): void {
    const options = {
      headers: { Authorization: idToken }
    };
    // Gets or creates the user in backgammon database.
    this.http
      .post<UserDto>(`${this.url}/signin`, userDto, options)
      .pipe(
        map((data) => {
          return data;
        })
      )
      .subscribe((data: UserDto) => {
        this.cookies.set(Cookies.loginKey, JSON.stringify(data), 14);
        AppState.Singleton.user.setValue(data);
        AppState.Singleton.busy.setValue(false);
      });
  }

  signOut(): void {
    AppState.Singleton.user.clearValue();
    this.cookies.delete(Cookies.loginKey);
  }

  // If the user account is stored as cookie, it will be restored without contacting social provider
  repair(): void {
    const sUser = this.cookies.get(Cookies.loginKey);
    if (sUser) {
      const userDto = JSON.parse(sUser) as UserDto;
      AppState.Singleton.user.setValue(userDto);
    }
  }
}
