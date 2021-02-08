import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { SocketsService } from 'src/services/sockets.service';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MessageTestComponent } from './components/message-test/message-test.component';
import { GameBoardComponent } from './components/game-board/game-board.component';

@NgModule({
  declarations: [AppComponent, MessageTestComponent, GameBoardComponent],
  imports: [BrowserModule, AppRoutingModule],
  providers: [SocketsService],
  bootstrap: [AppComponent]
})
export class AppModule {}
