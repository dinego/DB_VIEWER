import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { SalaryTableComponent } from './salary-table.component';
import { CanAccessGuard } from '@/shared/guard/can-access.guard';
import { AuthGuard } from '@/shared/guard/auth.guard';
import { SubModules, Modules } from '@/shared/models/modules';
import routerNames from '@/shared/routerNames';

const routes: Routes = [
  {
    path: '',
    component: SalaryTableComponent,
    canActivate: [AuthGuard, CanAccessGuard],
    data: { module: Modules.tableSalary, subModule: SubModules.none }
  },
  {
    path: routerNames.SALARY_TABLE.SHARE,
    component: SalaryTableComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SalaryTableRoutingModule { }
