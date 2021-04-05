import { ErrorHandler, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { StorageServiceModule } from 'ngx-webstorage-service';
import {
  SocialLoginModule,
  SocialAuthServiceConfig
} from 'angularx-social-login';
import {
  GoogleLoginProvider,
  FacebookLoginProvider
} from 'angularx-social-login';

import { SocketsService } from 'src/app/services/sockets.service';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GameBoardComponent } from './components/game/game-board/game-board.component';
import { LobbyContainerComponent } from './components/lobby-container/lobby-container.component';
import { GameContainerComponent } from './components/game/game-container/game-container.component';
import { AccountService, AuthInterceptor } from 'src/app/services';
import { DicesComponent } from './components/game/dices/dices.component';
import { BoardButtonsComponent } from './components/game/board-buttons/board-buttons.component';
import { MessagesComponent } from './components/game/messages/messages.component';
import { MenuComponent } from './components/game/menu/menu.component';
import { LoginButtonsComponent } from './components/lobby-container/login-buttons/login-buttons.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { InviteComponent } from './components/lobby-container/invite/invite.component';
import { ToplistContainerComponent } from './components/toplist-container/toplist-container.component';
import { ToplistComponent } from './components/toplist-container/toplist/toplist.component';
import { ToplistBannerComponent } from './components/toplist-container/toplist-banner/toplist-banner.component';
import { AccountMenuComponent } from './components/account/account-menu/account-menu.component';
import { AccountEditContainerComponent } from './components/account/account-edit-container/account-edit-container.component';
import { ReactiveFormsModule } from '@angular/forms';
import { BusyComponent } from './components/shared/busy/busy.component';
import { ErrorHandlerComponent } from './components/shared/error-handler/error-handler.component';
import { GlobalErrorService } from './services';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { AdminContainerComponent } from './components/admin-container/admin-container.component';
import { PlayedGamesComponent } from './components/admin-container/played-games/played-games.component';
@NgModule({
  declarations: [
    AppComponent,
    GameBoardComponent,
    GameContainerComponent,
    LobbyContainerComponent,
    DicesComponent,
    BoardButtonsComponent,
    MessagesComponent,
    MenuComponent,
    LoginButtonsComponent,
    AccountMenuComponent,
    InviteComponent,
    ToplistContainerComponent,
    ToplistComponent,
    ToplistBannerComponent,
    AccountEditContainerComponent,
    BusyComponent,
    ErrorHandlerComponent,
    AdminContainerComponent,
    PlayedGamesComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    SocialLoginModule,
    HttpClientModule,
    StorageServiceModule,
    ReactiveFormsModule,
    ServiceWorkerModule.register('ngsw-worker.js', {
      enabled: environment.production
    })
  ],
  providers: [
    SocketsService,
    AccountService,
    {
      provide: 'SocialAuthServiceConfig',
      useValue: {
        autoLogin: navigator.userAgent.indexOf('Firefox') == -1, // firefox must have false. dont know why
        providers: [
          {
            id: GoogleLoginProvider.PROVIDER_ID,
            provider: new GoogleLoginProvider(
              '296204915760-builmppcda4nq2t6gh3rgtiq5o4v6976.apps.googleusercontent.com'
            )
          },
          {
            id: FacebookLoginProvider.PROVIDER_ID,
            provider: new FacebookLoginProvider('548694169142603')
          }
        ]
      } as SocialAuthServiceConfig
    },
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    { provide: ErrorHandler, useClass: GlobalErrorService }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
