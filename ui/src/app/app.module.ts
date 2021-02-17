import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { SocketsService } from 'src/app/services/sockets.service';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MessageTestComponent } from './components/message-test/message-test.component';
import { GameBoardComponent } from './components/game-board/game-board.component';
import { LobbyComponent } from './components/lobby/lobby.component';
import { GameContainerComponent } from './components/game-container/game-container.component';
import { GameService } from 'src/app/services';
import { DicesComponent } from './components/dices/dices.component';

@NgModule({
  declarations: [
    AppComponent,
    MessageTestComponent,
    GameBoardComponent,
    GameContainerComponent,
    LobbyComponent,
    DicesComponent
  ],
  imports: [BrowserModule, AppRoutingModule],
  providers: [SocketsService, GameService],
  bootstrap: [AppComponent]
})
export class AppModule {}
