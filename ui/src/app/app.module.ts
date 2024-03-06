// import { ErrorHandler, NgModule } from '@angular/core';
// import { BrowserModule } from '@angular/platform-browser';
// import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
// import { StorageServiceModule } from 'ngx-webstorage-service';

// import { GameService } from 'src/app/services/game.service';

// import { AppRoutingModule } from './app.routes';
// import { AccountService, AuthInterceptor } from 'src/app/services';
// import {
//   HttpClient,
//   HttpClientModule,
//   HTTP_INTERCEPTORS
// } from '@angular/common/http';
// import { ReactiveFormsModule } from '@angular/forms';
// import { GlobalErrorService } from './services';
// import { ServiceWorkerModule } from '@angular/service-worker';
// import { environment } from '../environments/environment';
// import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
// import { TranslateHttpLoader } from '@ngx-translate/http-loader';

// export function HttpLoaderFactory(http: HttpClient): TranslateHttpLoader {
//   return new TranslateHttpLoader(http);
// }

// @NgModule({
//   declarations: [],
//   imports: [
//     ServiceWorkerModule.register('ngsw-worker.js', {
//       enabled: environment.production
//     }),
//     TranslateModule.forRoot({
//       loader: {
//         provide: TranslateLoader,
//         useFactory: HttpLoaderFactory,
//         deps: [HttpClient]
//       }
//     })
//   ],
//   providers: [
//     GameService,
//     AccountService,
//     { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
//     { provide: ErrorHandler, useClass: GlobalErrorService }
//   ],
//   bootstrap: []
// })
// export class AppModule {}
