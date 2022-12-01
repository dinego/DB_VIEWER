import { NgModule } from '@angular/core';

import { SharedModule } from '@/shared/shared.module';
import { DashboardService } from '@/shared/services/dashboard/dashboard.service';

import { DashboardComponent } from './dashboard.component';
import { DashboardRoutingModule } from './dashboard-routing.module';


@NgModule({
  declarations: [
    DashboardComponent
  ],
  imports: [
    DashboardRoutingModule,
    SharedModule,
  ],
  providers: [
    DashboardService
  ]
})
export class DashboardModule { }
