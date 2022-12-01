import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { UsersListComponent } from './users-list/users-list.component';
import { UsersComponent } from './users.component';
import { UserDetailComponent } from './user-detail/user-detail.component';
import { UserDetailResolverService } from '../resolvers/user-detail.resolver';
import { UserListResolverService } from '../resolvers/users-list.resolver';

const routes: Routes = [
  {
    path: '',
    component: UsersComponent,
    children: [
      {
        path: '',
        component: UsersListComponent,
        resolve: { usersParameter: UserListResolverService },
      },
      {
        path: ':id',
        component: UserDetailComponent,
        resolve: { userParameterDetail: UserDetailResolverService },
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class UsersRoutingModule {}
