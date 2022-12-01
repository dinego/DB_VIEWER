import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { CanAccessGuard } from "@/shared/guard/can-access.guard";
import { AuthGuard } from "@/shared/guard/auth.guard";
import { SubModules, Modules } from "@/shared/models/modules";
import routerNames from "@/shared/routerNames";
import { StudiesPublicationsComponent } from "./studies-publications.component";

const routes: Routes = [
  {
    path: "",
    component: StudiesPublicationsComponent,
    canActivate: [AuthGuard, CanAccessGuard],
    data: { module: Modules.studiesPublications, subModule: SubModules.none },
  },
  {
    path: routerNames.STUDIES_PUBLICATIONS,
    component: StudiesPublicationsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class StudiesPublicationRoutingModule {}
