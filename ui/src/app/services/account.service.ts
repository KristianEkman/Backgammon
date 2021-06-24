import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { finalize, map, take } from 'rxjs/operators';
import { AppState } from '../state/app-state';
import { Keys, Sound } from '../utils';
import { StorageService, LOCAL_STORAGE } from 'ngx-webstorage-service';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { ToplistService } from './toplist.service';
import { Busy } from '../state/busy';
import { SocialAuthService } from 'angularx-social-login';
import { MessageService } from './message.service';
import { TranslateService } from '@ngx-translate/core';
import { Theme } from '../components/account/theme/theme';
import { GoldGiftDto } from '../dto/rest';
import {
  LocalAccountStatus,
  LocalLoginDto,
  NewLocalUserDto,
  UserDto
} from '../dto';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  url: string;
  constructor(
    private http: HttpClient,
    @Inject(LOCAL_STORAGE) private storage: StorageService,
    private router: Router,
    private topListService: ToplistService,
    private authService: SocialAuthService,
    private messageService: MessageService,
    private trans: TranslateService
  ) {
    this.url = `${environment.apiServiceUrl}/account`;
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
        }),
        take(1) // unsubscribes
      )
      .subscribe((userDto: UserDto) => {
        this.trans.use(userDto.preferredLanguage ?? 'en');
        this.storage.set(Keys.loginKey, userDto);
        AppState.Singleton.user.setValue(userDto);
        if (userDto) Theme.change(userDto.theme);
        Busy.hide();
        if (userDto?.createdNew) {
          this.router.navigateByUrl('/edit-user');
        }
        if (userDto) {
          this.topListService.loadToplist();
          this.messageService.loadMessages();
        }
      });
  }

  signOut(): void {
    AppState.Singleton.user.clearValue();
    this.storage.remove(Keys.loginKey);
    this.trans.use('en');
  }

  // If the user account is stored in local storage, it will be restored without contacting social provider
  repair(): void {
    const user = this.storage.get(Keys.loginKey) as UserDto;
    AppState.Singleton.user.setValue(user);
    this.trans.use(user?.preferredLanguage ?? 'en');
    if (user) {
      Theme.change(user.theme);
      this.synchUser();
    }
  }

  saveUser(user: UserDto): Observable<void> {
    return this.http.post(`${this.url}/saveuser`, user).pipe(
      map(() => {
        AppState.Singleton.user.setValue(user);
        this.storage.set(Keys.loginKey, user);
      })
    );
  }

  deleteUser(): void {
    const user = AppState.Singleton.user.getValue();
    this.http.post(`${this.url}/delete`, user).subscribe(() => {
      AppState.Singleton.user.clearValue();
      this.authService.signOut();
      this.storage.set(Keys.loginKey, null);
      this.router.navigateByUrl('/lobby');
    });
  }

  isLoggedIn(): boolean {
    return !!AppState.Singleton.user.getValue();
  }

  getGold(): void {
    this.http
      .get(`${this.url}/requestgold`)
      .pipe(
        map((response) => {
          const dto = response as GoldGiftDto;
          const user = AppState.Singleton.user.getValue();
          if (dto.gold > user.gold) Sound.playCoin();
          AppState.Singleton.user.setValue({
            ...user,
            gold: dto.gold,
            lastFreeGold: dto.lastFreeGold
          });
        }),
        take(1)
      )
      .subscribe();
  }

  synchUser(): void {
    const user = AppState.Singleton.user.getValue();
    Busy.showNoOverlay();
    this.http
      .get(`${this.url}/getuser?userId=${user.id}`)
      .pipe(
        map((response) => {
          const userDto = response as UserDto;
          this.trans.use(userDto.preferredLanguage ?? 'en');
          this.storage.set(Keys.loginKey, userDto);
          AppState.Singleton.user.setValue(userDto);
          if (userDto) Theme.change(userDto.theme);
          Busy.hide();
        }),
        take(1)
      )
      .subscribe();
  }

  newLocalUser(dto: NewLocalUserDto): Observable<LocalAccountStatus> {
    Busy.show();
    return this.http.post(`${this.url}/newlocal`, dto).pipe(
      map((response) => {
        var userDto = response as UserDto;
        this.storage.set(Keys.loginKey, userDto);
        AppState.Singleton.user.setValue(userDto);
        if (userDto) return LocalAccountStatus.success;
        return LocalAccountStatus.nameExists;
      }),
      finalize(() => {
        Busy.hide();
      }),
      take(1)
    );
  }

  localLogin(dto: LocalLoginDto): Observable<LocalAccountStatus> {
    Busy.show();
    return this.http.post(`${this.url}/signinlocal`, dto).pipe(
      map((response) => {
        var userDto = response as UserDto;
        this.storage.set(Keys.loginKey, userDto);
        AppState.Singleton.user.setValue(userDto);
        if (userDto) {
          return LocalAccountStatus.success;
        }
        return LocalAccountStatus.invalidLogin;
      }),
      finalize(() => {
        Busy.hide();
      }),
      take(1)
    );
  }
}
