import { ConfigureDisplayComponent } from "./configure-display/configure-display/configure-display.component";
import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";
import routerNames from "@/shared/routerNames";
import { AuthGuard } from "@/shared/guard/auth.guard";
import { CanAccessGuard } from "@/shared/guard/can-access.guard";
import { Modules, SubModules } from "@/shared/models/modules";
import { LevelsComponent } from "./levels/levels.component";
import { PJSettingsComponent } from "./pj-settings/pj-settings.component";
import { ParametersComponent } from "./parameters.component";
import { HourlyBasisComponent } from "./hourly-basis/hourly-basis.component";
import { SalaryStrategyComponent } from "./salary-strategy/salary-strategy.component";
import { GlobalLabelsComponent } from "./global-labels/global-labels.component";
import { ContractTypeGuard } from "@/shared/guard/contract-type.guard";
import { PjSettingsResolver } from "./resolvers/pj-settings.resolver";
import { HourlyBasisGuard } from "@/shared/guard/hourly-basis.guard";
import { HourlyBasisResolver } from "./resolvers/hourly-basis.resolver";
import { CanAccessUserResolver } from "./resolvers/can-access-user.resolver";

const routes: Routes = [
  {
    path: "",
    component: ParametersComponent,
    resolve: {
      showPjSettings: PjSettingsResolver,
      showHourlyBasis: HourlyBasisResolver,
      showUsers: CanAccessUserResolver,
    },
    canActivate: [AuthGuard, CanAccessGuard],
    data: { module: Modules.parameters, subModule: SubModules.none },

    children: [
      {
        path: routerNames.PARAMETERS.CONFIGURE_DISPLAY,
        component: ConfigureDisplayComponent,
        canActivate: [AuthGuard, CanAccessGuard],
        data: { module: Modules.parameters, subModule: SubModules.levels },
        // aqui nesse data falta o subModule de configure-display
      },
      {
        path: routerNames.PARAMETERS.LEVELS,
        component: LevelsComponent,
        canActivate: [AuthGuard, CanAccessGuard],
        data: { module: Modules.parameters, subModule: SubModules.levels },
      },

      {
        path: routerNames.PARAMETERS.HOURLY_BASIS,
        component: HourlyBasisComponent,
        canActivate: [AuthGuard, CanAccessGuard, HourlyBasisGuard],
        data: { module: Modules.parameters, subModule: SubModules.hourlyBasis },
      },

      {
        path: routerNames.PARAMETERS.CORPORATE_SETTINGS,
        component: PJSettingsComponent,
        canActivate: [AuthGuard, CanAccessGuard, ContractTypeGuard],
        data: {
          module: Modules.parameters,
          subModule: SubModules.corporateSettings,
        },
      },
      {
        path: routerNames.PARAMETERS.SALARY_STRATEGY,
        component: SalaryStrategyComponent,
        canActivate: [AuthGuard, CanAccessGuard],
        data: {
          module: Modules.parameters,
          subModule: SubModules.salaryStrategy,
        },
      },
      {
        path: routerNames.PARAMETERS.GLOBAL_LABELS,
        component: GlobalLabelsComponent,
        canActivate: [AuthGuard, CanAccessGuard],
        data: {
          module: Modules.parameters,
          subModule: SubModules.globalLabels,
        },
      },
      {
        path: routerNames.PARAMETERS.USERS,
        loadChildren: () => {
          return import("@/pages/parameters/users/users.module").then(
            (m) => m.UsersModule
          );
        },
        canActivate: [AuthGuard, CanAccessGuard],
        data: { module: Modules.parameters, subModule: SubModules.users },
      },
    ],
  },
  { path: "", redirectTo: "", pathMatch: "full" },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ParametersRoutingModule {}
