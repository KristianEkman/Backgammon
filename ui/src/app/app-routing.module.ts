import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GameContainerComponent } from './components/game/game-container.component';
import { LobbyComponent } from './components/lobby/lobby.component';

const routes: Routes = [
  {
    path: 'game',
    component: GameContainerComponent
  },
  {
    path: '**',
    component: LobbyComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
