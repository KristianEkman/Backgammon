import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AboutContainerComponent } from './components/about-container/about-container.component';
import { AccountEditContainerComponent } from './components/account/account-edit-container/account-edit-container.component';
import { AdminContainerComponent } from './components/admin-container/admin-container.component';
import { FeedbackContainerComponent } from './components/feedback-container/feedback-container.component';
import { GameContainerComponent } from './components/game/game-container/game-container.component';
import { InviteContainerComponent } from './components/invite-container/invite-container.component';
import { LobbyContainerComponent } from './components/lobby-container/lobby-container.component';
import { MessageContainerComponent } from './components/message-container/message-container.component';
import { PasswordContainerComponent } from './components/password-container/password-container.component';
import { PrivacyPolicyContainerComponent } from './components/privacy-policy-container/privacy-policy-container.component';
import { ToLittleGoldComponent } from './components/to-little-gold/to-little-gold.component';
import { ToplistContainerComponent } from './components/toplist-container/toplist-container.component';
import { UnsubscribeContainerComponent } from './components/unsubscribe-container/unsubscribe-container.component';
import { GoldGuard, LoginGuard } from './guards';

export const routes: Routes = [
  {
    path: 'lobby',
    component: LobbyContainerComponent
  },
  {
    path: 'game',
    component: GameContainerComponent,
    canActivate: [GoldGuard]
  },
  {
    path: 'tolittlegold',
    component: ToLittleGoldComponent
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
    path: 'about',
    component: AboutContainerComponent
  },
  {
    path: 'password',
    component: PasswordContainerComponent
    // todo, only enabled when not logged in.
  },
  {
    path: 'create-password',
    component: PasswordContainerComponent
    // todo, only enabled when not logged in.
  },
  {
    path: 'privacy-policy',
    component: PrivacyPolicyContainerComponent
  },
  {
    path: 'feedback',
    component: FeedbackContainerComponent,
    canActivate: [LoginGuard]
  },
  {
    path: 'unsubscribe',
    component: UnsubscribeContainerComponent
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
