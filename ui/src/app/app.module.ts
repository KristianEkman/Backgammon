import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { SocketsService } from 'src/app/services/sockets.service';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { GameBoardComponent } from './components/game-board/game-board.component';
import { LobbyComponent } from './components/lobby/lobby.component';
import { GameContainerComponent } from './components/game-container/game-container.component';
import { AccountService } from 'src/app/services';
import { DicesComponent } from './components/dices/dices.component';
import { BoardButtonsComponent } from './components/board-buttons/board-buttons.component';
import { MessagesComponent } from './components/messages/messages.component';
import { MenuComponent } from './components/menu/menu.component';

@NgModule({
  declarations: [
    AppComponent,
    GameBoardComponent,
    GameContainerComponent,
    LobbyComponent,
    DicesComponent,
    BoardButtonsComponent,
    MessagesComponent,
    MenuComponent
  ],
  imports: [BrowserModule, AppRoutingModule, BrowserAnimationsModule],
  providers: [SocketsService, AccountService],
  bootstrap: [AppComponent]
})
export class AppModule {}
