import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import routerNames from "@/shared/routerNames";

import { PositionsComponent } from "./positions.component";
import { ListComponent } from "./list/list.component";
import { MapComponent } from "./map/map.component";
import { AuthGuard } from "@/shared/guard/auth.guard";
import { Modules, SubModules } from "@/shared/models/modules";
import { CanAccessGuard } from "@/shared/guard/can-access.guard";

const routes: Routes = [
  {
    path: "",
    component: PositionsComponent,
    canActivate: [AuthGuard, CanAccessGuard],
    data: { module: Modules.positions, subModule: SubModules.none },
    children: [
      {
        path: routerNames.POSITIONS.LIST,
        component: ListComponent,
        canActivate: [AuthGuard, CanAccessGuard],
        data: { module: Modules.positions, subModule: SubModules.positionList },
      },
      {
        path: routerNames.POSITIONS.MAP,
        component: MapComponent,
        canActivate: [AuthGuard, CanAccessGuard],
        data: { module: Modules.positions, subModule: SubModules.positionMap },
      },
      {
        path: routerNames.POSITIONS.SHARE_LIST,
        component: ListComponent,
      },
      {
        path: routerNames.POSITIONS.SHARE_MAP,
        component: MapComponent,
      },
    ],
  },
  { path: "", redirectTo: "", pathMatch: "full" },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PositionsRoutingModule {}
