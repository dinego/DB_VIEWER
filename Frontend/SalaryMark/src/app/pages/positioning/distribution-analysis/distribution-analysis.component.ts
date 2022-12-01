import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
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
  DistributionAnalysisChart,
  SharedDistributionAnalysis,
} from "@/shared/models/positioning";
import {
  IDefault,
  IDisplayListTypes,
  IUnit,
} from "@/shared/interfaces/positions";
import { copyObject } from "@/shared/common/functions";
import { environment } from "src/environments/environment";
import { IShareHeader } from "@/shared/models/share-header";
import { IPermissions } from "@/shared/models/token";
import { TokenService } from "@/shared/services/token/token.service";
import { MediaObserver } from "@angular/flex-layout";
import displayTypesList from "./common/visualizations";
import { DisplayTypesEnum } from "@/shared/components/button-list-visualization/common/typeVisualizationsEnum";
import { IRowDistributionAnalisis } from "./components/distribution-analysis-table/common/IRowDistributionAnalisis";
import { transformAll } from "@angular/compiler/src/render3/r3_ast";
import { IRowExportDistributionAnalysis } from "./components/distribution-analysis-table/common/IRowExportDistributionAnalysis";
import { ExportCSVService } from "@/shared/services/export-csv/export-csv.service";
import { IExportData } from "@/shared/services/export-csv/common/IExportData";

@Component({
  selector: "app-distribution-analysis",
  templateUrl: "./distribution-analysis.component.html",
  styleUrls: ["./distribution-analysis.component.scss"],
})
export class DistributionAnalysisComponent implements OnInit {
  public checkedShow: Array<CheckShowItem> = [];
  public commonLocales = commonLocales;
  public displayBy: number;
  public displayByList: Array<IDefault>;
  public email: string;
  public distributionAnalysisChart: DistributionAnalysisChart;
  public distributionAnalysisCopyChart: DistributionAnalysisChart;
  public inputShareModal: IDialogInput;
  public inputCheckedBoxShowModal: IDialogInput;
  public isChangeDisplayBy = false;
  public locales = locales;
  public movementsList: Array<IDefault>;
  public query: string;
  public scenario: number;
  public shareURL: string;
  public units: IUnit[];
  public unitId: number;
  public termQuery = "";
  public secretKey: string;
  public share = false;
  public shareHeader: IShareHeader[];
  public permissions: IPermissions;
  chartHeight = "";
  public inputModalShow: IDialogInput;
  public listVisualization = displayTypesList;
  public selectedVisualization: IDisplayListTypes;
  public displayTypesEnum = DisplayTypesEnum;
  public rowItemsOut: IRowDistributionAnalisis[];
  public rowsExportCSVTable: IRowExportDistributionAnalysis[][];

  constructor(
    private commonService: CommonService,
    private ngxSpinnerService: NgxSpinnerService,
    private positioningService: PositioningService,
    private route: ActivatedRoute,
    private tokenService: TokenService,
    private clipboard: Clipboard,
    private changeDetectorRef: ChangeDetectorRef,
    private mediaObserver: MediaObserver,
    private exportService: ExportCSVService
  ) {}

  async ngOnInit() {
    this.configureScreen();
    this.setSelectedDisplay();
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
    } else {
      await this.getDisplayByPositioning();
      await this.getAllUnits();
      await this.getMovements();
      await this.getDistributionAnalysis(
        this.displayBy,
        this.scenario,
        this.unitId
      );
    }
  }

  getSelectedTable(): string {
    const objRef = this.displayByList.map((m) => {
      return {
        id: m.id.toString(),
        title: m.title,
      };
    });

    return objRef.find((f) => f.id === this.displayBy.toString()).title;
  }

  setSelectedDisplay() {
    this.selectedVisualization = this.listVisualization[0];
  }

  configureScreen() {
    this.mediaObserver.media$.subscribe((x) => {
      switch (x.mqAlias) {
        case "xl":
          this.chartHeight = "620";
          break;
        default:
          this.chartHeight = "440";
          break;
      }
    });
  }
  get getDistributionAnalysisChart(): DistributionAnalysisChart {
    return this.distributionAnalysisCopyChart
      ? this.distributionAnalysisCopyChart
      : null;
  }

  changeMovements(item: IDefault) {
    this.scenario = +item.id;
    this.getDistributionAnalysis(this.displayBy, this.scenario, this.unitId);
  }
  changeUnits(item: IUnit) {
    this.unitId = +item.unitId;
    this.getDistributionAnalysis(this.displayBy, this.scenario, this.unitId);
  }
  changeDisplayBy(item: IDefault) {
    if (this.isChangeDisplayBy) {
      this.displayBy = +item.id;
      this.checkedShow = [];
      this.getDistributionAnalysis(this.displayBy, this.scenario, this.unitId);
    } else {
      this.isChangeDisplayBy = true;
    }
  }

  changeCheckedGraph(checkedShow: Array<CheckShowItem>) {
    this.checkedShow = [...checkedShow];
    const aux = copyObject(this.distributionAnalysisChart);
    const aux2 = copyObject(this.distributionAnalysisChart);
    aux.chart.main.forEach((item, i) => {
      aux2.chart.main[i].data = [
        ...item.data.filter((item2) => {
          if (
            checkedShow.find((ele) => ele.name === item2.name && ele.checked)
          ) {
            return item2;
          }
        }),
      ];
    });
    this.distributionAnalysisCopyChart.chart = aux2.chart;
    this.changeDetectorRef.detectChanges();
  }

  async getDisplayByPositioning() {
    const res = await this.positioningService.getDisplayBy().toPromise();
    this.displayByList = res;
    this.displayBy = this.displayByList
      ? parseInt(this.displayByList[0].id)
      : 0;
  }

  async getAllUnits() {
    const units = await this.commonService.getAllUnits().toPromise();
    if (units.length > 0) {
      this.units = units;
      this.unitId = null;
    }
  }

  async getMovements() {
    const res = await this.commonService.getMovements().toPromise();
    this.movementsList = res;
    this.scenario = this.movementsList ? +this.movementsList[0].id : 0;
  }

  async getDistributionAnalysis(
    displayBy?: number,
    scenario?: number,
    unitId?: number
  ) {
    const res = await this.positioningService
      .getDistributionAnalysis(displayBy, scenario, unitId)
      .toPromise();
    this.distributionAnalysisChart = res;
    this.distributionAnalysisChart.categories.forEach((item: Categories) => {
      this.checkedShow.push({
        ...item,
        checked: true,
      });
    });
    this.distributionAnalysisCopyChart = copyObject(
      this.distributionAnalysisChart
    );

    this.ngxSpinnerService.hide();
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
        this.shareURL = `${environment.baseUrl}posicionamento/configuracoes/compartilhar-analise-distribuicao/${key}`;
        this.clipboard.copy(this.shareURL);
        this.ngxSpinnerService.hide();
      });
  }

  async getShareData() {
    const res = await this.positioningService
      .getShareDistributionAnalysis(this.secretKey)
      .toPromise();
    this.share = true;
    this.distributionAnalysisChart = res;
    this.distributionAnalysisChart.categories.forEach((item: Categories) => {
      this.checkedShow.push({
        ...item,
        checked: true,
      });
    });
    this.distributionAnalysisCopyChart = copyObject(
      this.distributionAnalysisChart
    );
    this.shareHeader = this.prepareShareHeader(
      this.distributionAnalysisChart.share
    );
    this.ngxSpinnerService.hide();
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

  getFileSpreadsheet() {
    // TO DO call endpoint
  }

  prepareShareHeader({
    user,
    date,
    scenario,
    unit,
  }: SharedDistributionAnalysis): IShareHeader[] {
    const options: Intl.DateTimeFormatOptions = {
      year: "numeric",
      month: "numeric",
      day: "numeric",
    };
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

  changeVisualization(event) {
    this.selectedVisualization = event;
  }

  rowItemsMixed(event) {
    this.rowItemsOut = event;
    this.rowsExportCSVTable = this.transformMixedInRowCSV();
  }

  transformMixedInRowCSV(): IRowExportDistributionAnalysis[][] {
    let rows: IRowExportDistributionAnalysis[][] = [];

    this.rowItemsOut.forEach((f, index) => {
      const firstItem = { value: f.titleCollapser, bold: true };
      const secondItem = { value: f.belowValue, bold: false };
      const thirdItem = { value: f.insideValue, bold: false };
      const fourthItem = { value: f.aboveValue, bold: false };

      let rowInside: any[] = [];

      rowInside.push(firstItem);
      rowInside.push(secondItem);
      rowInside.push(thirdItem);
      rowInside.push(fourthItem);
      rows.push(rowInside);

      f.rowsInside.forEach((ins) => {
        let rowInsideEach: any[] = [];
        const insideOne = { value: ins.title, bold: false };
        const insideTwo = { value: "", bold: false };
        const insideThree = { value: ins.value, bold: false };
        const insideFour = { value: "", bold: false };

        rowInsideEach.push(insideOne);
        rowInsideEach.push(insideTwo);
        rowInsideEach.push(insideThree);
        rowInsideEach.push(insideFour);

        rows.push(rowInsideEach);
      });
    });

    return rows;
  }

  exportCSV() {
    this.ngxSpinnerService.show();

    const scenario = this.movementsList.find((f) => {
      return +f.id === this.scenario;
    });

    const objToExportTable: IExportData = {
      bodyData: this.rowsExportCSVTable,
      columns: this.getColumnsGraphToExport(),
      scenario: scenario.title,
      tableTitle: "Análise Distribuição",
      unit:
        this.changeUnitLabel(this.units) === "Todas"
          ? "Todas as Unidades"
          : this.changeUnitLabel(this.units),
    };

    this.exportService.downloadExcelComparativeAnalyseDistribution(
      objToExportTable
    );
  }
  getColumnsGraphToExport(): string[] {
    const displayBy: string = this.displayByList.find(
      (f) => parseInt(f.id) === this.displayBy
    ).title;

    const dataExportHeader: string[] = [];
    dataExportHeader.push(displayBy);
    this.distributionAnalysisChart.chart.main.forEach((main) => {
      dataExportHeader.push(main.name);
    });

    return dataExportHeader;
  }

  exportImage() {}
}
