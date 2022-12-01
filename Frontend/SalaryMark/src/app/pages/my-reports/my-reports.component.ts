import { Component, OnInit } from "@angular/core";

import locales from "@/locales/my-reports";
import commonLocales from "@/locales/common";
import { OrderTypeEnum } from "@/shared/enum/order-type-enum";
import { IDownloadMyReport, IReports } from "@/shared/interfaces/my-reports";
import { ReportService } from "@/shared/services/reports/report.service";
import { NgxSpinnerService } from "ngx-spinner";
import { CommonService } from "@/shared/services/common/common.service";
import { IUnit } from "@/shared/interfaces/positions";

@Component({
  selector: "app-my-reports",
  templateUrl: "./my-reports.component.html",
  styleUrls: ["./my-reports.component.scss"],
})
export class MyReportsComponent implements OnInit {
  public locales = locales;
  public commonLocales = commonLocales;
  public query: string;
  public orderTypes = this.convertEnumToArray();
  public reports: IReports[];
  public myPage: number = 1;
  public orderType: number = 1;
  public unitList: Array<IUnit> = [];
  public unitIdSelected: number;
  public unitSelected: string;

  constructor(
    private reportService: ReportService,
    private ngxSpinnerService: NgxSpinnerService,
    private commonService: CommonService
  ) {}

  async ngOnInit() {
    await this.getAllUnits();
    await this.getAllReports(this.orderType, this.myPage, this.unitIdSelected);
  }

  changeSelectOrderType(event: any) {
    this.orderType = event.id;
    this.getAllReports(this.orderType, this.myPage, this.unitIdSelected);
  }

  downloadFile(download: IDownloadMyReport) {
    this.reportService.downloadFile(download.id).subscribe((item) => {
      const file = new Blob([item], {
        type: item.type,
      });
      const blob = window.URL.createObjectURL(file);
      var anchor = document.createElement("a");
      anchor.download = download.fileName;
      anchor.href = blob;
      anchor.click();

      //window.open(blob, '_blank');
      this.ngxSpinnerService.hide();
    });
  }

  async getAllReports(
    orderBy: number,
    page: number,
    unitId: number,
    term?: string
  ) {
    const termRequest = term ? term : "";
    this.reportService
      .getReports(orderBy, page, unitId, termRequest)
      .subscribe((items) => {
        this.ngxSpinnerService.hide();
        if (items.length == 0) {
          this.myPage--;
          return;
        }
        return (this.reports = items);
      });
  }

  onChangeSearch(eventQuery: string) {
    this.query = eventQuery;
    if (eventQuery.length >= 3 || eventQuery.length == 0) {
      this.getAllReports(
        this.orderType,
        this.myPage,
        this.unitIdSelected,
        eventQuery
      );
    }
  }

  onScrollDown() {
    this.myPage++;
    this.getAllReports(this.orderType, this.myPage, this.unitIdSelected);
  }

  convertEnumToArray() {
    const arrayObjects = [];
    let i = 1;
    for (const [propertyKey, propertyValue] of Object.entries(OrderTypeEnum)) {
      if (!Number.isNaN(Number(propertyKey))) {
        continue;
      }
      arrayObjects.push({ id: i, title: propertyValue });
      i++;
    }

    return arrayObjects;
  }

  async getAllUnits() {
    const units = await this.commonService.getAllUnits().toPromise();
    if (units.length > 0) {
      this.unitList = units;
      this.unitIdSelected = this.unitList[0].unitId;
      this.unitSelected = this.unitList[0].unit;
    }
  }

  changeUnit(event) {
    this.unitIdSelected = event.unitId;
    this.unitSelected = event.unit;
    this.getAllReports(this.orderType, this.myPage, this.unitIdSelected);
  }
}
