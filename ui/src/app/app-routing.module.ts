import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GameContainerComponent } from './components/game/game-container/game-container.component';
import { LobbyContainerComponent } from './components/lobby-container/lobby-container.component';
import { LoginButtonsComponent } from './components/login-buttons/login-buttons.component';

const routes: Routes = [
  {
    path: 'login',
    component: LoginButtonsComponent
  },
  {
    path: 'game',
    component: GameContainerComponent
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
