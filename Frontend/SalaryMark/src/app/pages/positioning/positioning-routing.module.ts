import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { Modules, SubModules } from '@/shared/models/modules';
import { AuthGuard } from '@/shared/guard/auth.guard';
import { CanAccessGuard } from '@/shared/guard/can-access.guard';
import routerNames from '@/shared/routerNames';

import { PositioningComponent } from './positioning.component';
import { DistributionAnalysisComponent } from './distribution-analysis/distribution-analysis.component';
import { FinancialImpactComponent } from './financial-impact/financial-impact.component';
import { PositioningNavHeaderComponent } from './positioning-nav-header/positioning-nav-header.component';
import { ComparativeAnalysisComponent } from './comparative-analysis/comparative-analysis.component';
import { FrameworkComponent } from './framework/framework.component';
import { ProposedMovementsComponent } from './proposed-movements/proposed-movements.component';

const routes: Routes = [
  {
    path: '',
    component: PositioningComponent,
    canActivate: [AuthGuard, CanAccessGuard],
    data: { module: Modules.positioning, subModule: SubModules.none },
  },
  {
    path: routerNames.POSITIONING.SETTINGS,
    component: PositioningNavHeaderComponent,
    children: [
      {
        path: routerNames.POSITIONING.COMPARATIVE_ANALYSIS,
        component: ComparativeAnalysisComponent,
        canActivate: [AuthGuard, CanAccessGuard],
        data: { module: Modules.positioning, subModule: SubModules.comparativeAnalysis },
      },
      {
        path: routerNames.POSITIONING.SHARE_COMPARATIVE_ANALYSIS,
        component: ComparativeAnalysisComponent,
      },
      {
        path: routerNames.POSITIONING.FRAMEWORK,
        component: FrameworkComponent,
        canActivate: [AuthGuard, CanAccessGuard],
        data: { module: Modules.positioning, subModule: SubModules.framework },
      },
      {
        path: routerNames.POSITIONING.SHARE_FRAMEWORK,
        component: FrameworkComponent,
      },
      {
        path: routerNames.POSITIONING.FINANCIAL_IMPACT,
        component: FinancialImpactComponent,
        canActivate: [AuthGuard, CanAccessGuard],
        data: {
          module: Modules.positioning,
          subModule: SubModules.financialImpact,
        },
      },
      {
        path: routerNames.POSITIONING.SHARE_FINANCIAL_IMPACT,
        component: FinancialImpactComponent,
      },
      {
        path: routerNames.POSITIONING.DISTRIBUTION_ANALYSIS,
        component: DistributionAnalysisComponent,
        canActivate: [AuthGuard, CanAccessGuard],
        data: {
          module: Modules.positioning,
          subModule: SubModules.distributionAnalysis,
        },
      },
      {
        path: routerNames.POSITIONING.SHARE_DISTRIBUTION_ANALYSIS,
        component: DistributionAnalysisComponent,
      },
      {
        path: routerNames.POSITIONING.PROPOSED_MOVEMENTS,
        component: ProposedMovementsComponent,
        canActivate: [AuthGuard, CanAccessGuard],
        data: { module: Modules.positioning, subModule: SubModules.movement },
      },
      {
        path: routerNames.POSITIONING.SHARE_PROPOSED_MOVEMENTS,
        component: ProposedMovementsComponent,
      },
    ],
  },
  { path: '', redirectTo: '', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PositioningRoutingModule {}
