import { Routes, RouterModule } from "@angular/router";
import routerNames from "@/shared/routerNames";

const routes: Routes = [
  {
    path: routerNames.HOME,
    loadChildren: () =>
      import("@/pages/home/home.module").then((m) => m.HomeModule),
  },
  {
    path: routerNames.DASHBOARD,
    loadChildren: () =>
      import("@/pages/dashboard/dashboard.module").then(
        (m) => m.DashboardModule
      ),
  },
  {
    path: routerNames.POSITIONS.BASE,
    loadChildren: () =>
      import("@/pages/positions/positions.module").then(
        (m) => m.PositionsModule
      ),
  },
  {
    path: routerNames.SALARY_TABLE.BASE,
    loadChildren: () =>
      import("@/pages/salary-table/salary-table.module").then(
        (m) => m.SalaryTableModule
      ),
  },
  {
    path: routerNames.POSITIONING.BASE,
    loadChildren: () =>
      import("@/pages/positioning/positioning.module").then(
        (m) => m.PositioningModule
      ),
  },
  {
    path: routerNames.PARAMETERS.BASE,
    loadChildren: () =>
      import("@/pages/parameters/parameters.module").then(
        (m) => m.ParametersModule
      ),
  },
  {
    path: routerNames.MY_REPORTS,
    loadChildren: () =>
      import("@/pages/my-reports/my-reports.module").then(
        (m) => m.MyReportsModule
      ),
  },
  {
    path: routerNames.STUDIES_PUBLICATIONS,
    loadChildren: () =>
      import("@/pages/studies-publications/studies-publications.module").then(
        (m) => m.StudiesPublicationsModule
      ),
  },

  // otherwise redirect to home
  { path: "404", redirectTo: routerNames.HOME, pathMatch: "full" },
  { path: "", redirectTo: routerNames.HOME, pathMatch: "full" },
  { path: "**", redirectTo: "" },
];

export const appRoutingModule = RouterModule.forRoot(routes, {
  relativeLinkResolution: "legacy",
});
