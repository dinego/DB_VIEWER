import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MyReportsComponent } from '../my-reports/my-reports.component'
import { MyReportsRoutingModule } from './my-reports-routing.module';
import { SharedModule } from '@/shared/shared.module';
import { ReportService } from '@/shared/services/reports/report.service';


@NgModule({
  declarations: [
    MyReportsComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    MyReportsRoutingModule,
  ],
  providers: [
    ReportService
  ]
})
export class MyReportsModule { }
