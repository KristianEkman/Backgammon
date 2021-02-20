import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { SocketsService } from 'src/app/services/sockets.service';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MessageTestComponent } from './components/message-test/message-test.component';
import { GameBoardComponent } from './components/game-board/game-board.component';
import { LobbyComponent } from './components/lobby/lobby.component';
import { GameContainerComponent } from './components/game-container/game-container.component';
import { AccountService } from 'src/app/services';
import { DicesComponent } from './components/dices/dices.component';
import { BoardButtonsComponent } from './components/board-buttons/board-buttons.component';

@NgModule({
  declarations: [
    AppComponent,
    MessageTestComponent,
    GameBoardComponent,
    GameContainerComponent,
    LobbyComponent,
    DicesComponent,
    BoardButtonsComponent
  ],
  imports: [BrowserModule, AppRoutingModule],
  providers: [SocketsService, AccountService],
  bootstrap: [AppComponent]
})
export class AppModule {}
