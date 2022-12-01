import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { MyReportsComponent } from './my-reports.component';
import { CanAccessGuard } from '@/shared/guard/can-access.guard';
import { AuthGuard } from '@/shared/guard/auth.guard';
import { SubModules, Modules } from '@/shared/models/modules';

const routes: Routes = [
  {
    path: '',
    component: MyReportsComponent,
    canActivate: [AuthGuard, CanAccessGuard],
    data: { module: Modules.myReports, subModule: SubModules.none }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MyReportsRoutingModule { }
