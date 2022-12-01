import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UsersRoutingModule } from './users-routing.module';
import { UsersComponent } from './users.component';
import { SharedModule } from '@/shared/shared.module';
import { UsersListComponent } from './../users/users-list/users-list.component';
import { UserDetailComponent } from './user-detail/user-detail.component';
import { ReactiveFormsModule } from '@angular/forms';
import { SendLinkAccessComponent } from './send-link-access/send-link-access.component';

@NgModule({
  declarations: [
    UsersComponent,
    UsersListComponent,
    UserDetailComponent,
    SendLinkAccessComponent,
  ],
  imports: [
    SharedModule,
    ReactiveFormsModule,
    CommonModule,
    UsersRoutingModule,
  ],
})
export class UsersModule {}
