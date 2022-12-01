import { NgModule } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";

import { PositioningComponent } from "./positioning.component";
import { PositioningRoutingModule } from "./positioning-routing.module";

import { SharedModule } from "@/shared/shared.module";
import { CommonService } from "@/shared/services/common/common.service";
import { PositioningService } from "@/shared/services/positioning/positioning.service";

import { PositioningNavHeaderComponent } from "./positioning-nav-header/positioning-nav-header.component";
import { ComparativeAnalysisComponent } from "./comparative-analysis/comparative-analysis.component";
import { FinancialImpactComponent } from "./financial-impact/financial-impact.component";
import { DistributionAnalysisComponent } from "./distribution-analysis/distribution-analysis.component";
import { ProposedMovementsComponent } from "./proposed-movements/proposed-movements.component";
import { ShowAsComponent } from "./show-as/show-as.component";
import { ChartDetailListComponent } from "./comparative-analysis/chart-detail-list/chart-detail-list.component";
import { ComparativeAnalysisTableRowComponent } from "./comparative-analysis/chart-detail-list/components/table-row/table-row.component";
import { ComparativeAnalysisTableHeaderComponent } from "./comparative-analysis/chart-detail-list/components/table-header/table-header.component";
import { FrameworkComponent } from "./framework/framework.component";
import { TableHeaderComponent } from "./framework/components/table-header/table-header.component";
import { TableRowComponent } from "./framework/components/table-row/table-row.component";
import { DetailListComponent } from "./financial-impact/detail-list/detail-list.component";
import { FinancialImpactTableComponent } from "./financial-impact/components/financial-impact-table/financial-impact-table.component";
import { ColoredTableComponent } from "./financial-impact/components/financial-impact-table/colored-table/colored-table.component";
import { CategoriesTableComponent } from "./financial-impact/components/financial-impact-table/categories-table/categories-table.component";
import { DistributionAnalysisTableComponent } from "./distribution-analysis/components/distribution-analysis-table/distribution-analysis-table.component";
import { CollapseModule } from "ngx-bootstrap/collapse";
import { ProposedMovementsTableComponent } from "./proposed-movements/components/proposed-movements-table/proposed-movements-table.component";

@NgModule({
  declarations: [
    DistributionAnalysisComponent,
    FinancialImpactComponent,
    PositioningComponent,
    PositioningNavHeaderComponent,
    FrameworkComponent,
    ComparativeAnalysisComponent,
    ProposedMovementsComponent,
    ShowAsComponent,
    ChartDetailListComponent,
    TableHeaderComponent,
    TableRowComponent,
    ComparativeAnalysisTableRowComponent,
    ComparativeAnalysisTableHeaderComponent,
    DetailListComponent,
    FinancialImpactTableComponent,
    ColoredTableComponent,
    CategoriesTableComponent,
    DistributionAnalysisTableComponent,
    ProposedMovementsTableComponent,
  ],
  imports: [
    PositioningRoutingModule,
    SharedModule,
    ReactiveFormsModule,
    CollapseModule.forRoot(),
  ],
  exports: [TableHeaderComponent, TableRowComponent],
  providers: [CommonService, PositioningService],
})
export class PositioningModule {}
