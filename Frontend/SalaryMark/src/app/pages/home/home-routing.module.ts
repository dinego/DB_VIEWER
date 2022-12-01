import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from './home.component';
import { AuthGuard } from '@/shared/guard/auth.guard';
import { Modules, SubModules } from '@/shared/models/modules';
import { HomeRedirectGuard } from '@/shared/guard/home-redirect.guard';


const routes: Routes = [
  {
    path: '',
    component: HomeComponent,
    canActivate: [AuthGuard, HomeRedirectGuard],
    data: { module: Modules.home, subModule: SubModules.none }
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomeRoutingModule { }
