import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

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
import { AccountService } from 'src/app/services';
import { DicesComponent } from './components/game/dices/dices.component';
import { BoardButtonsComponent } from './components/game/board-buttons/board-buttons.component';
import { MessagesComponent } from './components/game/messages/messages.component';
import { MenuComponent } from './components/game/menu/menu.component';
import { LoginButtonsComponent } from './components/login-buttons/login-buttons.component';
import { HttpClientModule } from '@angular/common/http';

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
    LoginButtonsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    SocialLoginModule,
    HttpClientModule
  ],
  providers: [
    SocketsService,
    AccountService,
    {
      provide: 'SocialAuthServiceConfig',
      useValue: {
        autoLogin: false,
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
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
