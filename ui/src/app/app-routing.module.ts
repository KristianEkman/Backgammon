import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountEditContainerComponent } from './components/account/account-edit-container/account-edit-container.component';
import { AdminContainerComponent } from './components/admin-container/admin-container.component';
import { GameContainerComponent } from './components/game/game-container/game-container.component';
import { InviteContainerComponent } from './components/invite-container/invite-container.component';
import { LobbyContainerComponent } from './components/lobby-container/lobby-container.component';
import { MessageContainerComponent } from './components/message-container/message-container.component';
import { ToplistContainerComponent } from './components/toplist-container/toplist-container.component';
import { LoginGuard } from './guards/login.guard';

const routes: Routes = [
  {
    path: 'lobby',
    component: LobbyContainerComponent
  },
  {
    path: 'game',
    component: GameContainerComponent
  },
  {
    path: 'edit-user',
    component: AccountEditContainerComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'adminpage',
    component: AdminContainerComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'messages',
    component: MessageContainerComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'toplist',
    component: ToplistContainerComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'invite',
    component: InviteContainerComponent,
    canActivate: [LoginGuard]
  },
  {
    path: '**',
    component: LobbyContainerComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
