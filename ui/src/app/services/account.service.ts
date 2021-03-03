import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { map } from 'rxjs/operators';
import { UserDto } from '../dto/userDto';
import { AppState } from '../state/app-state';
import { Keys } from '../utils';
import { StorageService, LOCAL_STORAGE } from 'ngx-webstorage-service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  url: string;
  constructor(
    private http: HttpClient,
    @Inject(LOCAL_STORAGE) private storage: StorageService
  ) {
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
        this.storage.set(Keys.loginKey, data);
        AppState.Singleton.user.setValue(data);
        AppState.Singleton.busy.setValue(false);
      });
  }

  signOut(): void {
    AppState.Singleton.user.clearValue();
    this.storage.remove(Keys.loginKey);
  }

  // If the user account is stored in local storage, it will be restored without contacting social provider
  repair(): void {
    const user = this.storage.get(Keys.loginKey) as UserDto;
    AppState.Singleton.user.setValue(user);
  }
}
