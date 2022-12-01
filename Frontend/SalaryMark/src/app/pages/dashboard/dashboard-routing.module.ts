import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { CanAccessGuard } from '@/shared/guard/can-access.guard';
import { AuthGuard } from '@/shared/guard/auth.guard';
import { SubModules, Modules } from '@/shared/models/modules';

import { DashboardComponent } from './dashboard.component';

const routes: Routes = [
  {
    path: '',
    component: DashboardComponent,
    canActivate: [AuthGuard, CanAccessGuard],
    data: { module: Modules.dashboard, subModule: SubModules.none }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
