import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { SocketsService } from 'src/services/socket-service.service';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MessageTestComponent } from './components/message-test/message-test.component';

@NgModule({
  declarations: [AppComponent, MessageTestComponent],
  imports: [BrowserModule, AppRoutingModule],
  providers: [SocketsService],
  bootstrap: [AppComponent]
})
export class AppModule {}
