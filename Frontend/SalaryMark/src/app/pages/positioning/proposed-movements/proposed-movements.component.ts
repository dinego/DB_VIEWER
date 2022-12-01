import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnInit,
} from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Clipboard } from "@angular/cdk/clipboard";
import { NgxSpinnerService } from "ngx-spinner";

import locales from "@/locales/positioning";
import commonLocales from "@/locales/common";

import { ClickProposedMovementsChartDataInput } from "@/shared/components/charts/proposed-movements-chart/proposed-movements-chart-input";
import { CommonService } from "@/shared/services/common/common.service";
import { Modules, SubModules } from "@/shared/models/modules";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import { PositioningService } from "@/shared/services/positioning/positioning.service";
import {
  Categories,
  CheckShowItem,
  ColsFinancialImpact,
  ProposedMovementsChart,
  ProposedMovementsTable,
  SharedProposedMovement,
} from "@/shared/models/positioning";
import {
  IDefault,
  IDisplayListTypes,
  IUnit,
} from "@/shared/interfaces/positions";
import { copyObject, isUndefined } from "@/shared/common/functions";
import { environment } from "src/environments/environment";
import { IShareHeader } from "@/shared/models/share-header";
import { IPermissions } from "@/shared/models/token";
import { TokenService } from "@/shared/services/token/token.service";
import { ColumnMode, SelectionType } from "@swimlane/ngx-datatable";
import { FullInfoProposedMovementEnum } from "@/shared/enum/full-info-positioning-enum";
import { MediaObserver } from "@angular/flex-layout";
import displayTypesList from "./common/visualizations";
import { DisplayTypesEnum } from "@/shared/components/button-list-visualization/common/typeVisualizationsEnum";
import { ExportCSVService } from "@/shared/services/export-csv/export-csv.service";
import { IExportData } from "@/shared/services/export-csv/common/IExportData";

declare var $: any;

@Component({
  selector: "app-proposed-movements",
  templateUrl: "./proposed-movements.component.html",
  styleUrls: ["./proposed-movements.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProposedMovementsComponent implements OnInit {
  public checkedShow: Array<CheckShowItem> = [];
  public commonLocales = commonLocales;
  public displayBy: number;
  public displayByList: Array<IDefault>;
  public email: string;
  public filterTable: ClickProposedMovementsChartDataInput;
  public proposedMovementsChart: ProposedMovementsChart;
  public proposedMovementsCopyChart: ProposedMovementsChart;
  public proposedMovementsTable: ProposedMovementsTable = {
    category: "",
    scenario: "",
    nextPage: 1,
    table: { header: [], body: [] },
  };
  public inputTableModal: IDialogInput;
  public inputShareModal: IDialogInput;
  public inputCheckedBoxShowModal: IDialogInput;
  public isChangeDisplayBy = false;
  public locales = locales;
  public movementsList: Array<IDefault>;
  public page = 1;
  public lastPage = 0;
  public query: string;
  public scenario: number;
  public shareURL: string;
  public units: IUnit[];
  public unitId: number;
  public termQuery = "";
  public secretKey: string;
  public share: boolean = false;
  public shareHeader: IShareHeader[];
  public permissions: IPermissions;
  public dataResult;
  public sortColumnId?: number;
  public selected = [];
  public ColumnMode = ColumnMode;
  public SelectionType = SelectionType;
  public headerHeight = 50;
  public rowHeight = 46;
  public pageLimit = 20;
  public isLoading: boolean;
  public isShowColName: boolean;
  public isAsc = true;
  public isClearFilter = false;
  public fullInfoPositionEnum = FullInfoProposedMovementEnum;
  public chartHeight = "";
  public listVisualization: IDisplayListTypes[] = displayTypesList;
  public selectedVisualization: IDisplayListTypes;
  public displayTypesEnum = DisplayTypesEnum;
  public isTableInPage: boolean;

  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    private commonService: CommonService,
    private ngxSpinnerService: NgxSpinnerService,
    private positioningService: PositioningService,
    private route: ActivatedRoute,
    private tokenService: TokenService,
    private clipboard: Clipboard,
    private el: ElementRef,
    private mediaObserver: MediaObserver,
    private exportCSVService: ExportCSVService
  ) {}

  async ngOnInit() {
    this.configureScreen();
    this.setFirstSelectedVisualization();
    this.secretKey = this.route.snapshot.paramMap.get("secretkey");
    this.permissions = this.tokenService.getPermissions();
    this.configureTableModal();
    if (this.secretKey) {
      await this.getShareData();
      return;
    }

    await this.getDisplayByPositioning();
    await this.getAllUnits();
    await this.getMovements();
    await this.getProposedMovements(this.displayBy, this.scenario, this.unitId);
  }

  setFirstSelectedVisualization() {
    this.selectedVisualization = this.listVisualization[0];
  }

  configureScreen() {
    this.mediaObserver.media$.subscribe((x) => {
      switch (x.mqAlias) {
        case "xl":
          this.chartHeight = "590";
          break;
        default:
          this.chartHeight = "410";
          break;
      }
    });
  }

  get getProposedMovementsChart(): ProposedMovementsChart {
    return this.proposedMovementsCopyChart
      ? this.proposedMovementsCopyChart
      : null;
  }

  configureTableModal() {
    this.inputTableModal = {
      disableFooter: true,
      fullSize: true,
      idModal: "tableShowModal",
      title: locales.proposedMovements,
    };
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
  }

  changeMovements(item: IDefault) {
    this.scenario = +item.id;
    this.getProposedMovements(this.displayBy, this.scenario, this.unitId);
  }
  changeUnits(item: IUnit) {
    this.unitId = +item.unitId;
    this.getProposedMovements(this.displayBy, this.scenario, this.unitId);
  }
  changeDisplayBy(item: IDefault) {
    if (this.isChangeDisplayBy) {
      this.displayBy = +item.id;
      this.checkedShow = [];
      this.getProposedMovements(this.displayBy, this.scenario, this.unitId);
    } else {
      this.isChangeDisplayBy = true;
    }
  }

  changeCheckedGraph(checkedShow: Array<CheckShowItem>) {
    this.checkedShow = [...checkedShow];
    const aux = copyObject(this.proposedMovementsChart);
    const aux2 = copyObject(this.proposedMovementsChart);
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
    this.proposedMovementsCopyChart.chart = [...aux2.chart];
  }

  public onScrollDown(offsetY: number) {
    // total height of all rows in the viewport
    const viewHeight =
      this.el.nativeElement.getBoundingClientRect().height - this.headerHeight;

    // check if we scrolled to the end of the viewport
    if (
      !this.isLoading &&
      offsetY + viewHeight >= this.dataResult.length * this.rowHeight
    ) {
      if (this.lastPage === this.page) {
        return;
      }
      this.lastPage = this.page;
      if (this.secretKey && this.page > 0) {
        this.getFullInfoProposedMovementsShare(true);
      } else if (this.page > 0) {
        this.getFullInfoProposedMovements(true);
      }
    }
  }

  async getDisplayByPositioning() {
    const res = await this.positioningService.getDisplayBy().toPromise();
    if (res) {
      this.displayByList = res;
      this.displayBy =
        this.displayByList && this.displayByList.length > 0
          ? +this.displayByList[0].id
          : 0;
    }
  }

  async getAllUnits() {
    const units = await this.commonService.getAllUnits().toPromise();
    if (units && units.length > 0) {
      this.units = units;
      this.unitId = null;
    }
  }

  async getMovements() {
    const res = await this.commonService.getMovements().toPromise();
    if (res) {
      this.movementsList = res;
      this.scenario =
        this.movementsList && this.movementsList.length > 0
          ? +this.movementsList[0].id
          : 0;
    }
  }

  getProposedMovementsTable(items) {
    this.resetTableInfo();
    this.filterTable = items;
    this.setTitle();
    if (this.secretKey) {
      this.getFullInfoProposedMovementsShare(items);
    } else {
      this.getFullInfoProposedMovements(items);
    }
  }

  async getProposedMovements(
    displayBy?: number,
    scenario?: number,
    unitId?: number
  ) {
    const res = await this.positioningService
      .getProposedMovements(displayBy, scenario, unitId)
      .toPromise();
    if (res) {
      this.proposedMovementsChart = res;
      if (this.checkedShow.length === 0) {
        this.proposedMovementsChart.categories.forEach((item: Categories) => {
          this.checkedShow.push({
            ...item,
            checked: true,
          });
        });
      }
      this.proposedMovementsCopyChart = copyObject(this.proposedMovementsChart);
      this.ngxSpinnerService.hide();
      this.changeDetectorRef.markForCheck();
    }
  }

  async getShareData() {
    const share = await this.positioningService
      .getShareProposedMovements(this.secretKey)
      .toPromise();
    if (share) {
      this.share = true;
      this.proposedMovementsChart = share;
      this.proposedMovementsChart.categories.forEach((item: Categories) => {
        this.checkedShow.push({
          ...item,
          checked: true,
        });
      });
      this.proposedMovementsCopyChart = copyObject(this.proposedMovementsChart);
      this.shareHeader = this.prepareShareHeader(
        this.proposedMovementsChart.share
      );
      this.ngxSpinnerService.hide();
      this.changeDetectorRef.markForCheck();
    }
  }

  getShareKey(): void {
    this.commonService
      .getShareKey({
        moduleId: Modules.positioning,
        moduleSubItemId: SubModules.movement,
        columnsExcluded: [{}],
        parameters: {
          unitId: this.unitId,
          scenario: this.scenario,
          displayBy: this.displayBy,
        },
      })
      .subscribe((key) => {
        this.shareURL = `${environment.baseUrl}posicionamento/configuracoes/compartilhar-movimentos-propostos/${key}`;

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

  getFileSpreadsheet() {
    // TO DO call endpoint
  }

  prepareShareHeader({
    user,
    date,
    scenario,
    unit,
  }: SharedProposedMovement): IShareHeader[] {
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

  public getFullInfoProposedMovementsShare(
    scrolled: boolean = false,
    sort?: boolean
  ) {
    this.positioningService
      .getFullInfoProposedMovementsShare(
        this.page,
        this.pageLimit,
        this.filterTable.categoryId,
        this.secretKey,
        this.sortColumnId,
        this.isAsc,
        this.filterTable.serieId
      )
      .subscribe((res) => {
        if (res.table && res.table.header && res.table.body) {
          this.proposedMovementsTable.table.header =
            res.table.body.length > 0
              ? res.table.header
              : this.proposedMovementsTable.table.header;

          if (res.table.body.length > 0) {
            const formatResult = res.table.body.map((info) => {
              let tableResult = {};
              info.map((res) => {
                this.proposedMovementsTable.table.header.forEach((item) => {
                  if (item.colPos == res.colPos) {
                    tableResult[`${item.colPos}`] = res;
                  }
                });
              });
              return tableResult;
            });
            const rows =
              this.dataResult && isUndefined(sort)
                ? [...this.dataResult, ...formatResult]
                : formatResult;
            this.dataResult = rows;
          }
        } else if (!scrolled && res.nextPage === 0) {
          this.dataResult = [];
        }
        this.proposedMovementsTable.category = res.category;
        this.proposedMovementsTable.scenario = res.scenario;
        this.page = res.nextPage;
        this.ngxSpinnerService.hide();
        this.isLoading = false;
        $("#tableShowModal").modal("show");
        this.changeDetectorRef.detectChanges();
        this.changeDetectorRef.markForCheck();
      });
  }
  public getFullInfoProposedMovements(
    scrolled: boolean = false,
    sort?: boolean
  ) {
    this.positioningService
      .getFullInfoProposedMovements(
        this.filterTable.displayBy,
        this.page,
        this.pageLimit,
        this.filterTable.scenario,
        this.filterTable.unitId,
        this.filterTable.categoryId,
        this.sortColumnId,
        this.isAsc,
        this.filterTable.serieId
      )
      .subscribe((res) => {
        if (res.table && res.table.header && res.table.body) {
          this.proposedMovementsTable.table.header =
            res.table.body.length > 0
              ? res.table.header
              : this.proposedMovementsTable.table.header;

          if (res.table.body.length > 0) {
            const formatResult = res.table.body.map((info) => {
              let tableResult = {};
              info.map((res) => {
                this.proposedMovementsTable.table.header.forEach((item) => {
                  if (item.colPos == res.colPos) {
                    tableResult[`${item.colPos}`] = res;
                  }
                });
              });
              return tableResult;
            });
            const rows =
              this.dataResult && isUndefined(sort)
                ? [...this.dataResult, ...formatResult]
                : formatResult;
            this.dataResult = rows;
          } else if (!scrolled && res.nextPage === 0) {
            this.dataResult = [];
          }
        }
        this.proposedMovementsTable.category = res.category;
        this.proposedMovementsTable.scenario = res.scenario;
        this.page = res.nextPage;
        this.isLoading = false;
        if (!this.isTableInPage) $("#tableShowModal").modal("show");
        $("#spinner").hide();
        this.changeDetectorRef.detectChanges();
        this.changeDetectorRef.markForCheck();
      });
  }

  public onSort(event: any) {
    this.isAsc = event.newValue !== "desc";
    this.isLoading = true;
    this.sortColumnId = event.column.prop;
    if (this.secretKey) {
      this.getFullInfoProposedMovementsShare(false, true);
    } else {
      this.getFullInfoProposedMovements(false, true);
    }
  }

  getFormatValue(row: any, header: any): string {
    return row[`${header.colPos}`].value.split(",")[0];
  }

  onSelect({ selected }) {
    this.selected.splice(0, this.selected.length);
    this.selected.push(...selected);
    this.isClearFilter = this.selected.length > 0;
    this.changeDetectorRef.detectChanges();
    this.changeDetectorRef.markForCheck();
  }

  getReplaceValue(row: any, header: any): string {
    return row[`${header.colPos}`].value.replace(",", ".");
  }

  transformText(row: any, header: any) {
    return row[`${header.colPos}`].value;
  }

  transformTextToNumber(row: any, header: any) {
    return +row[`${header.colPos}`].value;
  }

  getRowResult(row: any, header: any) {
    return row[`${header.colPos}`];
  }

  public onClearFilter() {
    this.isClearFilter = false;
    if (this.secretKey) {
      this.getFullInfoProposedMovementsShare();
    } else {
      this.getFullInfoProposedMovements();
    }
  }

  public onFilterChecked(): void {
    this.dataResult = this.selected;
    this.changeDetectorRef.detectChanges();
    this.changeDetectorRef.markForCheck();
  }

  changeUnitLabel(items: IUnit[]) {
    if (!items) return ``;
    return items.length > 1 ? this.locales.all : items[0].unit;
  }
  setTitle() {
    this.inputTableModal.title = `${this.locales.proposedMovements} - ${
      this.filterTable ? this.filterTable.legend : ""
    }`;
  }

  resetTableInfo() {
    this.page = 1;
    this.proposedMovementsTable = {
      category: "",
      nextPage: 0,
      scenario: "",
      table: { body: [], header: [] },
    };
    this.inputTableModal.title = "";
    this.dataResult = [];
  }
  setColumnWidth(column: ColsFinancialImpact) {
    const columns = [
      FullInfoProposedMovementEnum.Employee,
      FullInfoProposedMovementEnum.HoursBase,
    ];
    return columns.includes(column.columnId)
      ? 180
      : column.colPos >= 3 &&
        column.columnId != this.fullInfoPositionEnum.ProposedMovementLabel
      ? 100
      : 300;
  }

  displayTypeSelect(event) {
    this.isTableInPage = false;
    this.selectedVisualization = event;
  }

  onChangeSearch(event) {
    this.query = event;
  }

  openModalTable(event) {
    this.filterTable = event;
    this.ngxSpinnerService.show();
    this.getFullInfoProposedMovements();
  }

  exportCSV() {
    const headers = this.graphDataToHeaders(this.proposedMovementsChart);
    const dataBody = this.graphDataToTableBody(this.proposedMovementsChart);
    const scenario = this.scenario
      ? this.movementsList.find((f) => parseInt(f.id) === this.scenario).title
      : this.movementsList[0].title;
    const unit = this.unitId
      ? this.units.find((f) => f.unitId === this.unitId).unit
      : this.units[0].unit;

    const objToExportCSV: IExportData = {
      bodyData: dataBody,
      columns: headers,
      scenario: scenario,
      tableTitle: locales.proposedMovements,
      unit: unit,
    };

    this.exportCSVService.downloadExcelProposedMovements(objToExportCSV);
  }

  exportCSVModal() {
    const headers = this.proposedMovementsTable.table.header;
    const dataBody = this.dataResult;
    const scenario = this.scenario
      ? this.movementsList.find((f) => parseInt(f.id) === this.scenario).title
      : this.movementsList[0].title;
    const unit = this.unitId
      ? this.units.find((f) => f.unitId === this.unitId).unit
      : this.units[0].unit;

    const objToExportCSV: IExportData = {
      bodyData: dataBody,
      columns: headers,
      scenario: scenario,
      tableTitle: locales.proposedMovements,
      unit: unit,
    };

    this.exportCSVService.downloadExcelProposedMovementsModal(objToExportCSV);
  }

  graphDataToHeaders(proposedMovementsChart: ProposedMovementsChart): string[] {
    const headers: string[] = [];
    headers.push(
      this.displayByList.find((f) => parseInt(f.id) === this.displayBy).title
    );

    proposedMovementsChart.chart.forEach((chart) => {
      headers.push(chart.name);
    });

    return headers;
  }

  graphDataToTableBody(
    proposedMovementsChart: ProposedMovementsChart
  ): any[][] {
    const dataBody: any[][] = [];

    proposedMovementsChart.categories.forEach((cats, index) => {
      const dataInner: string[] = [];

      dataInner.push(cats.name);
      proposedMovementsChart.chart.forEach((chart, indexChart) => {
        dataInner.push(chart.data[index].value.toString());
      });

      dataBody.push(dataInner);
    });

    return dataBody;
  }

  exportGraph() {}
}
