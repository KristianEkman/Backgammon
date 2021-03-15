import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountEditContainerComponent } from './components/account/account-edit-container/account-edit-container.component';
import { GameContainerComponent } from './components/game/game-container/game-container.component';
import { LobbyContainerComponent } from './components/lobby-container/lobby-container.component';

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
    component: AccountEditContainerComponent
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
