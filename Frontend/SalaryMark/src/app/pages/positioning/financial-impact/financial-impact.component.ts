import {
  Component,
  OnInit,
  ChangeDetectorRef,
  ChangeDetectionStrategy,
} from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Clipboard } from "@angular/cdk/clipboard";
import { NgxSpinnerService } from "ngx-spinner";

import locales from "@/locales/positioning";
import commonLocales from "@/locales/common";

import { CommonService } from "@/shared/services/common/common.service";
import { Modules, SubModules } from "@/shared/models/modules";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import { PositioningService } from "@/shared/services/positioning/positioning.service";
import {
  Categories,
  CheckShowItem,
  FinancialImpactChart,
  SharedFinancialImpact,
} from "@/shared/models/positioning";
import {
  IDefault,
  IDisplayListTypes,
  IUnit,
} from "@/shared/interfaces/positions";
import { copyObject } from "@/shared/common/functions";
import { ClickFinancialImpactChartDataInput } from "@/shared/components/charts/financial-impact-chart/financial-impact-chart-input";
import { IShareHeader } from "@/shared/models/share-header";
import { TokenService } from "@/shared/services/token/token.service";
import { IPermissions } from "@/shared/models/token";

import { environment } from "src/environments/environment";
import { FullInfoFinancialImpactEnum } from "@/shared/enum/full-info-positioning-enum";
import { MediaObserver } from "@angular/flex-layout";
import displayTypesList from "./common/displayList";
import { DisplayTypesEnum } from "@/shared/components/button-list-visualization/common/typeVisualizationsEnum";
import { ExportCSVService } from "@/shared/services/export-csv/export-csv.service";
import { IExportData } from "@/shared/services/export-csv/common/IExportData";

declare var $: any;

@Component({
  selector: "app-financial-impact",
  templateUrl: "./financial-impact.component.html",
  styleUrls: ["./financial-impact.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FinancialImpactComponent implements OnInit {
  public checkedShow: Array<CheckShowItem> = [];
  public commonLocales = commonLocales;
  public displayBy: number;
  public displayByList: Array<IDefault>;
  public email: string;

  public financialImpactChart: FinancialImpactChart;
  public financialImpactCopyChart: FinancialImpactChart;

  public inputShareModal: IDialogInput;
  public inputCheckedBoxShowModal: IDialogInput;
  public isChangeDisplayBy = false;
  public locales = locales;
  public movementsList: Array<IDefault>;
  public query: string;
  public scenario: number;
  public secretKey: string;
  public shareURL: string;
  public units: IUnit[];
  public unitId: number;
  public termQuery = "";
  public share = false;
  public shareHeader: IShareHeader[];
  public permissions: IPermissions;
  public listDisplayTypes: IDisplayListTypes[] = [];

  public fullInfoPositionEnum = FullInfoFinancialImpactEnum;
  public chartHeight = "";
  public clickFinancialImpactChartDataInput: ClickFinancialImpactChartDataInput;
  public listTitle: string;
  public selectedVisualizationType: IDisplayListTypes;
  public displayTypesEnum = DisplayTypesEnum;

  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    private commonService: CommonService,
    private ngxSpinnerService: NgxSpinnerService,
    private positioningService: PositioningService,
    private route: ActivatedRoute,
    private tokenService: TokenService,
    private clipboard: Clipboard,
    private mediaObserver: MediaObserver,
    private exportCSVService: ExportCSVService
  ) {}

  async ngOnInit() {
    this.configureScreen();
    this.setListDisplays();
    this.secretKey = this.route.snapshot.paramMap.get("secretkey");
    this.permissions = this.tokenService.getPermissions();

    this.inputCheckedBoxShowModal = {
      disableFooter: true,
      idModal: "checkedShowModal",
      title: locales.showConfiguration,
      isInfoPosition: true,
      isRightModal: true,
    };
    this.inputShareModal = {
      disableFooter: false,
      idModal: "shareModal",
      title: locales.share,
      btnPrimaryTitle: locales.send,
      btnSecondaryTitle: locales.cancel,
    };

    if (this.secretKey) {
      await this.getShareData();
      return;
    }

    await this.getDisplayByPositioning();
    await this.getAllUnits();
    await this.getMovements();
    await this.getFinancialImpact(this.displayBy, this.scenario, this.unitId);
  }

  configureScreen() {
    this.mediaObserver.media$.subscribe((x) => {
      switch (x.mqAlias) {
        case "xl":
          this.chartHeight = "550";
          break;
        default:
          this.chartHeight = "420";
          break;
      }
    });
  }

  setListDisplays() {
    this.listDisplayTypes = displayTypesList;
    this.selectedVisualizationType = this.listDisplayTypes.find(
      (dis) => dis.id === DisplayTypesEnum.BAR
    );
  }

  async changeMovements(item: IDefault) {
    this.scenario = +item.id;
    await this.getFinancialImpact(this.displayBy, this.scenario, this.unitId);
  }
  async changeUnits(item: IUnit) {
    this.unitId = +item.unitId;
    await this.getFinancialImpact(this.displayBy, this.scenario, this.unitId);
  }
  async changeDisplayBy(item: IDefault) {
    if (this.isChangeDisplayBy) {
      this.displayBy = +item.id;
      this.checkedShow = [];
      await this.getFinancialImpact(this.displayBy, this.scenario, this.unitId);
    } else {
      this.isChangeDisplayBy = true;
    }
  }

  changeCheckedGraph(checkedShow: Array<CheckShowItem>) {
    this.checkedShow = [...checkedShow];
    const aux = copyObject(this.financialImpactChart);
    const aux2 = copyObject(this.financialImpactChart);
    aux.chart.forEach((item, i) => {
      aux2.chart[i].data = [
        ...item.data.filter((item2) => {
          if (
            checkedShow.find((ele) => ele.name === item2.name && ele.checked)
          ) {
            return item2;
          }
        }),
      ];
    });
    this.financialImpactCopyChart.chart = [...aux2.chart];
    this.changeDetectorRef.detectChanges();
  }

  async getDisplayByPositioning() {
    const res = await this.positioningService.getDisplayBy().toPromise();
    if (res) {
      this.displayByList = res;
      this.displayBy = Number(res[0].id);
    }
  }

  async getAllUnits() {
    const units = await this.commonService.getAllUnits().toPromise();
    if (units.length > 0) {
      this.units = units;
      this.unitId = null;
    }
  }

  async getMovements() {
    const movements = await this.commonService.getMovements().toPromise();
    if (movements) {
      this.movementsList = movements;
      this.scenario = Number(movements[0].id);
    }
  }

  async getShareData() {
    const share = await this.positioningService
      .getShareFinancialImpact(this.secretKey)
      .toPromise();
    if (share) {
      this.share = true;
      this.financialImpactChart = share;
      this.financialImpactChart.categories.forEach((item: Categories) => {
        this.checkedShow.push({
          ...item,
          checked: true,
        });
      });
      this.financialImpactCopyChart = copyObject(this.financialImpactChart);
      this.shareHeader = this.prepareShareHeader(
        this.financialImpactChart.share
      );
      this.ngxSpinnerService.hide();
      this.changeDetectorRef.markForCheck();
    }
  }

  getShareKey(): void {
    this.commonService
      .getShareKey({
        moduleId: Modules.positioning,
        moduleSubItemId: SubModules.financialImpact,
        columnsExcluded: [{}],
        parameters: {
          unitId: this.unitId,
          scenario: this.scenario,
          displayBy: this.displayBy,
        },
      })
      .subscribe((key) => {
        this.shareURL = `${environment.baseUrl}posicionamento/configuracoes/compartilhar-impacto-financeiro/${key}`;
        this.clipboard.copy(this.shareURL);
        this.ngxSpinnerService.hide();
        this.changeDetectorRef.markForCheck();
      });
  }

  onSendEmail() {
    this.commonService
      .shareLink({
        to: this.email,
        url: this.shareURL,
      })
      .subscribe((res) => this.ngxSpinnerService.hide());
  }

  onPutEmail(event) {
    this.email = event;
  }

  prepareShareHeader({
    user,
    date,
    scenario,
    unit,
  }: SharedFinancialImpact): IShareHeader[] {
    const options: any = { year: "numeric", month: "numeric", day: "numeric" };
    const formattedDate = new Date(date).toLocaleDateString("pt-br", options);
    return [
      {
        label: "Nome",
        value: user,
        type: "string",
      },
      {
        label: "Data",
        value: formattedDate,
        type: "string",
      },
      {
        label: "Cenário",
        value: scenario,
        type: "string",
      },
      {
        label: "Unidade",
        value: unit,
        type: "string",
      },
    ];
  }

  changeUnitLabel(items: IUnit[]) {
    if (!items) return ``;
    return items.length > 1 ? this.locales.all : items[0].unit;
  }

  openListModal(
    clickFinancialImpactChartDataInput: ClickFinancialImpactChartDataInput
  ) {
    this.clickFinancialImpactChartDataInput =
      clickFinancialImpactChartDataInput;

    this.listTitle = this.clickFinancialImpactChartDataInput
      ? `${this.locales.financialImpactTitleTable} - ${this.clickFinancialImpactChartDataInput.legend}`
      : "";

    $("#tableShowModal").modal("show");
  }

  async getFinancialImpact(
    displayBy?: number,
    scenario?: number,
    unitId?: number
  ) {
    this.ngxSpinnerService.show();
    const res = await this.positioningService
      .getFinancialImpact(displayBy, scenario, unitId)
      .toPromise();

    if (res) {
      this.financialImpactChart = res;
      if (this.checkedShow.length === 0) {
        this.financialImpactChart.categories.forEach((item: Categories) => {
          this.checkedShow.push({
            ...item,
            checked: true,
          });
        });
      }
      this.financialImpactCopyChart = copyObject(this.financialImpactChart);
      this.ngxSpinnerService.hide();
      this.changeDetectorRef.markForCheck();
    }
  }

  changeVisualization(event) {
    this.selectedVisualizationType = event;
  }

  exportCSV() {
    const dataBody = this.dataChartToBodyExportCSV();
    const headers = this.dataChartToHeaderExportCSV();

    const scenario = this.scenario
      ? this.movementsList.find((f) => parseInt(f.id) === this.scenario).title
      : this.movementsList[0].title;
    const unit = this.unitId
      ? this.units.find((f) => f.unitId === this.unitId).unit
      : this.units[0].unit;

    const bojToExportCSV: IExportData = {
      bodyData: dataBody,
      columns: headers,
      scenario: scenario,
      unit: unit,
      tableTitle: this.locales.financialImpactTitleTable,
    };

    this.exportCSVService.downloadExcelFinancialImpact(bojToExportCSV);
  }

  exportGraph() {}

  dataChartToBodyExportCSV() {
    const dataBody: any[][] = [];

    this.financialImpactCopyChart.categories.forEach((f, indexCat) => {
      const innerData: any[] = [];

      this.financialImpactCopyChart.chart.forEach((chart, index) => {
        if (index === 0) innerData.push(chart.data[indexCat].name);

        innerData.push(chart.data[indexCat].func);
        innerData.push(chart.data[indexCat].y);
        innerData.push(chart.data[indexCat].percentage);
      });

      dataBody.push(innerData);
    });

    return dataBody;
  }

  dataChartToHeaderExportCSV() {
    const headers: string[] = [];
    const getDisplayBy: string = this.displayByList.find(
      (f) => parseInt(f.id) === this.displayBy
    ).title;
    headers.push(getDisplayBy);

    this.financialImpactCopyChart.chart.forEach((chart) => {
      headers.push(chart.name);
    });

    return headers;
  }
}
