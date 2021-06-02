import { MemberDetailedResolver } from './_resolvers/member-detailed.resolver';
import { PreventUsavedChangesGuard } from './_guards/prevent-usaved-changes.guard';
import { ServerErrorComponent } from './errors/server-error/server-error.component';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { AuthGuard } from './_guards/auth.guard';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { HomeComponent } from './home/home.component';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MemberEditComponent } from './members/member-edit/member-edit.component';

const routes: Routes = [
  {path: '', component: HomeComponent},
  {path: '',
    runGuardsAndResolvers: 'always',
    canActivate:[AuthGuard],
    children:[
      //{path: 'members', component: MemberListComponent, canActivate: [AuthGuard]},
      {path: 'members', component: MemberListComponent},
      //{path: 'members/:id', component: MemberDetailComponent},
      //{path: 'members/:username', component: MemberDetailComponent},
      {path: 'members/:username', component: MemberDetailComponent, resolve:{member: MemberDetailedResolver}},
      {path: 'member/edit', component: MemberEditComponent, canDeactivate: [PreventUsavedChangesGuard]},
      {path: 'lists', component: ListsComponent},
      {path: 'messages', component: MessagesComponent}
    ]
  },
  {path: 'errors', component: TestErrorsComponent},
  {path: 'not-found', component: NotFoundComponent},
  {path: 'server-error', component: ServerErrorComponent},
  //{path: '**', component: HomeComponent, pathMatch: 'full'} //**=wild card rout. If rout not correct redirect to Home or Error page
  {path: '**', component: NotFoundComponent, pathMatch: 'full'} 
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
