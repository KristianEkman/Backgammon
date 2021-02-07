/* eslint-disable @typescript-eslint/ban-types */
/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/explicit-module-boundary-types */
import { Inject } from '@angular/core';
import { Observable, Observer, Subject } from 'rxjs';

export class SocketsService {
  ws: any;
  messages!: Subject<any>;
  private connectionSubject!: Subject<MessageEvent>;
  wsUrl = 'ws://localhost:60109/ws';
  constructor() {
    this.initConnection();
  }

  public initConnection() {
    console.log('init connection');
    this.messages = new Subject<any>();

    this.connectionSubject = this.wsCreate(this.wsUrl);

    if (
      typeof this.ws !== 'undefined' &&
      this.ws.readyState !== 0 &&
      this.ws.readyState !== 1
    ) {
      this.connectionSubject = this.wsCreate(this.wsUrl);
    }

    this.ws.onmessage = (e: any) => {
      const response = e.data;
      console.log(response);
      return this.connectionSubject.next(response);
    };

    this.ws.onclose = () => console.log('client disconnected ');
  }

  send(value: string) {
    console.log('send');
    this.ws.send(value);
  }

  closeConnection() {
    this.ws.close(1000, 'disconnect');
    // this.ws = undefined;
  }

  private wsCreate(url: string): Subject<any> {
    this.ws = new WebSocket(url);
    const observable = new Observable((obs: Observer<string>) => {
      this.ws.onmessage = obs.next.bind(obs);
      this.ws.onerror = obs.error.bind(obs);
      this.ws.onclose = obs.complete.bind(obs);
      return this.ws.close.bind(this.ws);
    });

    const observer = {
      next: (data: object) => {
        console.log('next', this.ws.readyState);
        if (this.ws.readyState === WebSocket.OPEN) {
          this.ws.send(JSON.stringify(data));
        }
      }
    };

    return Subject.create(observer, observable);
  }
}
