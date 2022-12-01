import { ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { SharedModule } from "@/shared/shared.module";
import { ParametersRoutingModule } from "./parameters-routing.module";
import { ParametersComponent } from "./parameters.component";
import { LevelsComponent } from "./levels/levels.component";
import { ParametersNavHeaderComponent } from "./parameters-nav-header/parameters-nav-header.component";
import { HourlyBasisComponent } from "./hourly-basis/hourly-basis.component";
import { PJSettingsComponent } from "./pj-settings/pj-settings.component";
import { UsersModule } from "./users/users.module";
import { SalaryStrategyComponent } from "./salary-strategy/salary-strategy.component";
import { TableRowComponent } from "./salary-strategy/components/table-row/table-row.component";
import { TableHeaderComponent } from "./salary-strategy/components/table-header/table-header.component";
import { GlobalLabelsComponent } from "./global-labels/global-labels.component";
import { ParamsService } from "@/shared/services/params/params.service";
import { ConfigureDisplayComponent } from "./configure-display/configure-display/configure-display.component";

@NgModule({
  declarations: [
    ParametersComponent,
    LevelsComponent,
    ParametersNavHeaderComponent,
    HourlyBasisComponent,
    SalaryStrategyComponent,
    TableRowComponent,
    TableHeaderComponent,
    PJSettingsComponent,
    GlobalLabelsComponent,
    ConfigureDisplayComponent,
  ],
  imports: [
    SharedModule,
    CommonModule,
    ParametersRoutingModule,
    ReactiveFormsModule,
    UsersModule,
  ],
  exports: [TableRowComponent, TableHeaderComponent],
  providers: [ParamsService],
})
export class ParametersModule {}
