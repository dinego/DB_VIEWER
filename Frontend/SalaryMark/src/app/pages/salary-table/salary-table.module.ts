import { NgModule } from "@angular/core";
import { SharedModule } from "@/shared/shared.module";

import { SalaryTableService } from "@/shared/services/salary-table/salary-table.service";

import { SalaryTableComponent } from "./salary-table.component";
import { SalaryTableRoutingModule } from "./salary-table-routing.module";
import { TableRowComponent } from "./components/table-row/table-row.component";
import { TableHeaderComponent } from "./components/table-header/table-header.component";
import { ModalEditValuesComponent } from "./components/modal-edit-values/modal-edit-values.component";
import { ApplyUpdateComponent } from "./components/modal-edit-values/components/apply-update/apply-update.component";
import { EditTracksComponent } from "./components/modal-edit-values/components/edit-tracks/edit-tracks.component";
import { ImportExcelComponent } from "./components/modal-edit-values/components/import-excel/import-excel.component";
import { ReactiveFormsModule } from "@angular/forms";
import { DecimalPipe } from "@angular/common";

import { NgxMaskModule, IConfig } from "ngx-mask";
import { ExportCSVService } from "@/shared/services/export-csv/export-csv.service";
import { SalaryTableExportPngComponent } from './components/salary-table-export-png/salary-table-export-png.component';
import { ExportSalaryTableComponent } from './components/export-salary-table/export-salary-table.component';

const maskConfig: Partial<IConfig> = {
  validation: false,
};

@NgModule({
  declarations: [
    SalaryTableComponent,
    TableHeaderComponent,
    TableRowComponent,
    ModalEditValuesComponent,
    ApplyUpdateComponent,
    EditTracksComponent,
    ImportExcelComponent,
    SalaryTableExportPngComponent,
    ExportSalaryTableComponent,
  ],
  imports: [
    SalaryTableRoutingModule,
    ReactiveFormsModule,
    SharedModule,
    NgxMaskModule.forRoot(maskConfig),
  ],
  providers: [SalaryTableService, DecimalPipe, ExportCSVService],
})
export class SalaryTableModule {}
