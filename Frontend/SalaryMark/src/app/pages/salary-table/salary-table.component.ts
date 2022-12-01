import commonLocales from "@/locales/common";
import locales from "@/locales/salary-table";
import { TableSalaryColumnEnum } from "@/shared/enum/table-salary-column-enum";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import {
  ContractTypeEnum,
  IDefault,
  IDisplayListTypes,
  IDisplayTypes,
  ISalaryTableResponse,
  IUnit,
} from "@/shared/interfaces/positions";
import { HourlyBasisEnum } from "@/shared/models/hourly-basis";
import { Modules } from "@/shared/models/modules";
import {
  Body,
  Header,
  IShareSalaryTableFilter,
  Table,
  TableSalaryViewEnum,
  UpdateColumns,
  UpdateColumnsRequest,
} from "@/shared/models/salary-table";
import { IShareHeader } from "@/shared/models/share-header";
import { IPermissions } from "@/shared/models/token";
import { CommonService } from "@/shared/services/common/common.service";
import { SalaryTableService } from "@/shared/services/salary-table/salary-table.service";
import { TokenService } from "@/shared/services/token/token.service";
import { Clipboard } from "@angular/cdk/clipboard";
import {
  Component,
  ElementRef,
  HostListener,
  OnInit,
  Renderer2,
  ViewChild,
  ViewEncapsulation,
} from "@angular/core";
import { MediaObserver } from "@angular/flex-layout";
import { ActivatedRoute } from "@angular/router";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { NgxSpinnerService } from "ngx-spinner";
import { environment } from "src/environments/environment";
import { ModalEditValuesComponent } from "./components/modal-edit-values/modal-edit-values.component";
import visualizationTableGraph from "./common/visualizations-table-graph";
import { IEditSalarialTable } from "@/shared/models/editSalarialTable";
import { SalaryChart } from "./common/salary-chart";
import { copyObject, isUndefined } from "@/shared/common/functions";
import { DisplayTypesEnum } from "@/shared/components/button-list-visualization/common/typeVisualizationsEnum";
import { ExportCSVService } from "@/shared/services/export-csv/export-csv.service";
import { ModalPositionDetailComponent } from "@/shared/components/modal-position-detail/modal-position-detail.component";
import html2canvas from "html2canvas";
import { IRangeSalaryGraph } from "@/shared/interfaces/range-salary-table";
import { TableHeaderModalComponent } from "@/shared/components/table-header-modal/table-header-modal.component";
import { ToastrService } from "ngx-toastr";
import { ModalShareTableComponent } from "@/shared/components/modal-share-table/modal-share-table.component";
import {
  ColPosEnum,
  SalaryTableHeaderEnum,
} from "@/shared/enum/modal-position-detail/colPos-enum";

@Component({
  selector: "app-salary-table",
  templateUrl: "./salary-table.component.html",
  styleUrls: ["./salary-table.component.scss"],
  encapsulation: ViewEncapsulation.None,
})
export class SalaryTableComponent implements OnInit {
  @ViewChild("print") print: ElementRef;
  @ViewChild("downloadLink") downloadLink: ElementRef;

  public contractTypeId = ContractTypeEnum.CLT;
  public contractType: string;
  public displayColumns: Array<UpdateColumns> = [];
  public email: string;
  public groupId?: number;
  public group: string;
  public hoursTypeId = HourlyBasisEnum.MonthSalary;
  public hoursType: string;
  public inputModalShow: IDialogInput;
  public isClearFilter: boolean;
  public isModalEdit: boolean;
  public listTables: ISalaryTableResponse[] = [];
  public locales = locales;
  public visualizations: IDisplayListTypes[];
  public selectedVisualization: IDisplayTypes;
  public commonLocales = commonLocales;
  public page = 1;
  public pageSize = 20;
  public period: IDefault[] = [];
  public profiles: IDefault[];
  public selectedUnity: string;
  public shareURL: string;
  public tableId: number;
  public table: string;
  public projectId: number;
  public typePosition: IDefault[] = [];
  public permissions: IPermissions;
  public share: boolean;
  public secretKey: string;
  public shareData: IShareSalaryTableFilter;
  public shareHeader: IShareHeader[];
  public isAsc = true;
  public selected = [];
  public columHeaders: Header[];
  public columHeadersPositions: Header[];
  public hourlyBasisEnum = HourlyBasisEnum;
  public tableSalaryColumnEnum = TableSalaryColumnEnum;
  public rangesGraph: IRangeSalaryGraph;
  public updateColumns = false;
  public unitId?: number;
  public unit: string;
  public units: Array<IUnit> = [];
  public isShowPositions: boolean = false;
  public data = [];
  public dataPosition = [];
  public dataPositionExport = [];
  public showAllGsm = false;
  public tableClass = "";
  public sortClass = "datatable-icon-sort-unset sort-btn";
  public modalRef?: BsModalRef;
  public filterSearch?: string;
  public editValues: IEditSalarialTable;
  public defaultHeader: Header[] = [];
  public salaryChart: SalaryChart;
  public minRange: number;
  public maxRange: number;
  public firstShowPosition: boolean = true;
  public heightChartTable: number;
  public displayTypesEnum = DisplayTypesEnum;
  public sortColumnId?: number;
  public isPrinting: boolean;
  public tableInfo: Table = {
    header: [],
    body: [],
  };
  public tablePositions: Table = {
    header: [],
    body: [],
  };
  public tablePositionsExport: Table = {
    header: [],
    body: [],
  };

  constructor(
    private commonService: CommonService,
    private ngxSpinnerService: NgxSpinnerService,
    private salaryTableService: SalaryTableService,
    private tokenService: TokenService,
    private route: ActivatedRoute,
    private clipboard: Clipboard,
    private mediaObserver: MediaObserver,
    private modalService: BsModalService,
    private salaryTableExportService: ExportCSVService,
    private _toastrService: ToastrService,
    private _renderer: Renderer2
  ) {}

  async ngOnInit() {
    this.heightChartTable = window.innerHeight - 300;
    this.setListVisualizations();
    this.configureScreen();
    this.secretKey = this.route.snapshot.paramMap.get("secretkey");
    if (this.secretKey) {
      await this.getShareData();
      return;
    }
    await this.getAllSalaryTable();
    await this.getAllContractTypes();
    await this.getHourlyBase();
    await this.getRangesGraph();
    this.permissions = this.tokenService.getPermissions();

    this.inputModalShow = {
      disableFooter: false,
      idModal: "showHeaderTableSalaryModal",
      title: locales.showConfiguration,
      btnPrimaryTitle: locales.saveAndShow,
      btnSecondaryTitle: locales.show,
      btnWithoutCancel: true,
      canRenameColumn: this.permissions.canEditGlobalLabels,
      isInfoPosition: true,
      isRightModal: true,
    };
    this.ngxSpinnerService.hide();
  }

  setListVisualizations() {
    this.visualizations = visualizationTableGraph;
    this.selectedVisualization = Object.assign(this.visualizations[0]);
  }

  configureScreen() {
    this.mediaObserver.media$.subscribe((x) => {
      switch (x.mqAlias) {
        case "xl":
          this.tableClass = "ngx-custom-lg";
          break;
        case "md":
          this.tableClass = "ngx-custom-md";
        default:
          this.tableClass = "ngx-custom";
          break;
      }
    });
  }

  resetTables(resetClassSortTable: boolean = true) {
    this.page = 1;
    this.resetClassSortTableHeader(resetClassSortTable);
    this.tableInfo = {
      header: this.updateColumns ? [] : this.tableInfo.header,
      body: [],
    };

    this.selected = [];
    this.data = [];
    this.dataPosition = [];
  }

  resetAllTables() {
    this.page = 1;
    this.tableInfo = { header: [], body: [] };
    this.tablePositions = { header: [], body: [] };
    this.selected = [];
    this.data = [];
    this.dataPosition = [];
  }

  resetClassSortTableHeader(clear: boolean) {
    if (!clear) return;
    switch (this.isShowPositions) {
      case true:
        this.tablePositions.header.forEach((header: Header) => {
          header.isDesc = false;
          header.sortClass = this.sortClass;
        });
        break;
      case false:
        this.tableInfo.header.forEach((header: Header) => {
          header.isDesc = false;
          header.sortClass = this.sortClass;
        });
        break;
    }
    this.isAsc = true;
    this.sortColumnId = null;
  }

  // *********** TRANSFORM DATA VALUES ***********
  transformText(row: any, header: any) {
    return row[`${header.colPos}`].value;
  }

  getRowResult(row: any, header: any) {
    return row[`${header.colPos}`];
  }

  getIsMidPointResult(row: any, header: any) {
    return row[`${header.colPos}`].isMidPoint;
  }

  getTooltip(row: any, header: Header) {
    return row[`${header.colPos}`].subItems.filter((item) => item.tooltip);
  }

  isCLT(row: any, header: Header) {
    return row[`${header.colPos}`].occupantCLT;
  }

  isPJ(row: any, header: Header) {
    return row[`${header.colPos}`].occupantPJ;
  }

  getFormatValue(row: any, header: any): string {
    return row[`${header.colPos}`].value.split(",")[0];
  }
  // *********** END TRANSFORM DATA VALUES ***********

  // *********** METHODS SALARY TABLE ***********
  getSalaryTable(scrolled: boolean = false, sort?: boolean) {
    this.ngxSpinnerService.show();
    this.salaryTableService
      .getSalaryTable(
        this.tableId,
        this.unitId,
        this.contractTypeId,
        this.hoursTypeId,
        this.page,
        this.pageSize,
        this.groupId,
        this.isAsc,
        "",
        this.showAllGsm,
        this.sortColumnId
      )
      .subscribe((res) => {
        if (res.table) {
          this.tableInfo.header =
            res.table.body.length > 0 && this.tableInfo.header.length <= 0
              ? res.table.header
              : this.tableInfo.header;
          this.defaultHeader = copyObject(this.tableInfo.header);

          if (res.table.body.length > 0) {
            if (
              (this.filterSearch && this.filterSearch.length > 0) ||
              !scrolled
            )
              this.data = [];
            const formatResult = res.table.body.map((info) => {
              const tableResult = {};
              info.map((res) => {
                this.tableInfo.header.forEach((item) => {
                  if (item.colPos == res.colPos) {
                    res.isChecked = false;
                    tableResult[`${item.colPos}`] = res;
                  }
                });
              });
              return tableResult;
            });

            const rows =
              this.data && isUndefined(sort)
                ? [...this.data, ...formatResult]
                : formatResult;

            this.data = rows;
          }
        } else if (!scrolled && res.nextPage === 0) {
          this.data = [];
        }

        this.page = res.nextPage;

        this.getHeaderColumns();
        this.ngxSpinnerService.hide();
      });
  }
  getHeaderColumns() {
    this.columHeaders = this.tableInfo.header.filter((item) => {
      if (item.visible) {
        if (item.isChecked) {
          return item;
        }
      } else return item;
    });
  }

  getFileSpreadsheet() {
    this.ngxSpinnerService.show();

    this.salaryTableService
      .getExportExcel(
        this.tableId ? this.tableId.toString() : "",
        this.unitId ? this.unitId.toString() : "",
        this.groupId ? this.groupId.toString() : "",
        this.contractTypeId ? this.contractTypeId.toString() : "",
        this.hoursTypeId.toString(),
        this.isAsc ? this.isAsc.toString() : "",
        this.showAllGsm ? this.showAllGsm.toString() : "",
        this.sortColumnId ? this.sortColumnId.toString() : ""
      )
      .subscribe((item) => {
        this.salaryTableExportService.downloadExcelSalaryTable(
          item.headers,
          item.body,
          item.table,
          item.fileName
        );

        this.ngxSpinnerService.hide();
        this._toastrService.success("Download gerado com sucesso!");
      });
  }
  public onRestoreCols(modalChildren) {
    modalChildren.forEach((mdItem, index) => {
      mdItem.isChecked = this.defaultHeader[index].isChecked;
      mdItem.value = this.defaultHeader[index].nickName;
    });
    this.getHeaderColumns();
  }

  async onSort(item: Header) {
    this.page = 1;
    this.isAsc = !item.isDesc;
    this.sortColumnId = item.columnId;
    const objectManipulate: Table = this.isShowPositions
      ? this.tablePositions
      : this.tableInfo;

    objectManipulate.header.forEach((header: Header) => {
      header.isDesc = header.columnId !== item.columnId ? false : this.isAsc;
      header.sortClass =
        header.columnId !== item.columnId
          ? this.sortClass
          : this.isAsc
          ? "sort-btn datatable-icon-up sort-asc"
          : "sort-btn datatable-icon-down sort-desc";
    });
    this.secretKey
      ? await this.getShareData()
      : this.isShowPositions
      ? this.getSalaryPositionTable(false, true)
      : this.getSalaryTable(false, true);

    if (this.secretKey) {
      switch (this.shareData.viewType) {
        case TableSalaryViewEnum.SalaryTable:
          await this.loadShareSalaryTable(false, true);
          break;
        case TableSalaryViewEnum.PositionSalaryTable:
          await this.loadSharePositionSalaryTable(false, true);
          break;
      }
      return;
    }
  }
  onShowChanges(modalChildren, isEdit: boolean = false) {
    this.displayColumns = [];
    modalChildren.forEach((mdItem, index) => {
      if (
        this.tableInfo.header[index].visible &&
        (this.tableInfo.header[index].isChecked !== mdItem.isChecked ||
          mdItem.value)
      ) {
        this.displayColumns.push({
          columnId: mdItem.columnId,
          name:
            mdItem && mdItem.value
              ? mdItem.value
              : this.tableInfo.header[index].colName,
          isChecked: mdItem.isChecked,
        });
      }
      this.tableInfo.header[index].isChecked = mdItem.isChecked;
      if (mdItem.value) {
        this.tableInfo.header[index].nickName = mdItem.value;
      }
    });
    this.getHeaderColumns();
    this.updateColumns = isEdit;
  }

  async getEditSalarialTable() {
    return await this.salaryTableService
      .getEditSalarialTable(this.tableId, this.projectId)
      .toPromise();
  }

  async openModalEditValues() {
    this.ngxSpinnerService.show();
    this.editValues = await this.getEditSalarialTable();

    // forçando colocar * nos dois primeiros items
    const headersForTemplate: string[] = ["Tabela Salarial Id*"];
    this.columHeaders.forEach((element) => {
      if (element.colPos >= 3 || element.colPos === 0)
        headersForTemplate.push(
          element.colPos == 0 ? element.nickName + "*" : element.nickName
        );
    });

    const initialState = {
      tableId: this.tableId,
      projectId: this.projectId,
      data: this.editValues,
      headersForTemplate: headersForTemplate,
      salaryTables: this.listTables,
      canEditGSMMappingTable: this.permissions.canEditGSMMappingTable,
      gsmGlobalLabel: this.gsmGlobalLabel,
    };

    this.modalRef = this.modalService.show(ModalEditValuesComponent, {
      class: "modal-lg modal-edit positionDecorator",
      initialState,
    });
    this.modalRef.content.onClose.subscribe(() => {
      this.modalRef.hide();
    });

    this.modalRef.content.hideModalPositionEmitter.subscribe((res) => {
      if (res === true) {
        this.hideModalPosition(true);
        return;
      }

      this.hideModalPosition(false);
    });

    this.ngxSpinnerService.hide();
  }

  closeEdit(): void {
    this.modalRef.hide();
  }
  // *********** END METHODS SALARY TABLE ***********

  // *********** METHODS POSITIONS ***********
  getSalaryPositionTable(scrolled: boolean = false, sort?: boolean) {
    this.firstShowPosition = false;
    this.ngxSpinnerService.show();
    this.salaryTableService
      .getSalaryPositionTable(
        this.tableId,
        this.unitId,
        this.contractTypeId,
        this.hoursTypeId,
        this.page,
        this.pageSize,
        this.groupId,
        this.isAsc,
        this.filterSearch,
        this.sortColumnId
      )
      .subscribe((res) => {
        if (res.table) {
          this.tablePositions.header = this.filterSearch
            ? res.table.header
            : res.table.body.length > 0 &&
              this.tablePositions.header.length <= 0
            ? res.table.header
            : this.tablePositions.header;

          this.defaultHeader = copyObject(this.tablePositions.header);

          if (res.table.body.length > 0 || this.filterSearch) {
            if (
              (this.filterSearch && this.filterSearch.length > 0) ||
              !scrolled
            )
              this.dataPosition = [];

            res.table.body.forEach((item) => {
              this.dataPosition.push(item);
            });
          }
        } else if (!scrolled && res.nextPage === 0) {
          this.dataPosition = [];
        }

        this.page = res.nextPage;
        this.getPositionsHeaderColumns();
        this.ngxSpinnerService.hide();
      });
  }
  getPositionsHeaderColumns() {
    this.columHeadersPositions = this.tablePositions.header.filter((item) => {
      if (item.visible) {
        if (item.isChecked) {
          return item;
        }
        return;
      } else return item;
    });
  }
  public onRestoreColsPositions(modalChildren) {
    modalChildren.forEach((mdItem, index) => {
      mdItem.isChecked = this.defaultHeader[index].isChecked;
      mdItem.value = this.defaultHeader[index].nickName;
      this.tablePositions.header[index].nickName = mdItem.value;
      this.tablePositions.header[index].isChecked = mdItem.isChecked;
    });
    this.getPositionsHeaderColumns();
  }
  shiftArray(row: Body[]) {
    const headerColPos = this.columHeadersPositions.map((res) => res.colPos);
    const shifted = row.filter((r) => headerColPos.includes(r.colPos)).slice(1);
    return shifted;
  }
  async getFileSpreadsheetPositions() {
    this.ngxSpinnerService.show();

    await this.getSalaryTablePositionIgnorePagination();

    this._toastrService.success("Download gerado com sucesso!");
  }

  async getSalaryTablePositionIgnorePagination() {
    this.salaryTableService
      .getSalaryPositionTable(
        this.tableId,
        this.unitId,
        this.contractTypeId,
        this.hoursTypeId,
        this.page,
        this.pageSize,
        this.groupId,
        this.isAsc,
        this.filterSearch,
        this.sortColumnId,
        true // ignore paginação
      )
      .subscribe((res) => {
        if (res.table) {
          this.tablePositionsExport.header = res.table.header;

          if (res.table.body.length > 0) {
            res.table.body.forEach((item) => {
              const dataBody: Body[] = [];
              item.forEach((data) => {
                const verifyIsVisible = this.tablePositionsExport.header.find(
                  (f) => data.colPos === f.colPos
                );

                if (verifyIsVisible.isChecked) {
                  dataBody.push(data);
                }
              });
              this.dataPositionExport.push(dataBody);
            });
          }

          const tableTitle = this.listTables.find(
            (f) => parseInt(f.id) === this.tableId
          ).title;

          this.tablePositionsExport.header =
            this.tablePositionsExport.header.filter((f) => f.isChecked);

          this.salaryTableExportService.downloadExcelSalaryTablePositions(
            this.tablePositionsExport.header,
            this.dataPositionExport,
            tableTitle
          );

          // clean for anothoer download
          this.tablePositionsExport.header = [];
          this.tablePositionsExport.body = [];
          this.dataPositionExport = [];
        }

        this.ngxSpinnerService.hide();
      });
  }

  onShowPostionsChanges(modalChildren, isEdit: boolean = false) {
    this.displayColumns = [];
    modalChildren.forEach((mdItem, index) => {
      if (
        this.tablePositions.header[index].visible &&
        (this.tablePositions.header[index].isChecked !== mdItem.isChecked ||
          mdItem.value)
      ) {
        this.displayColumns.push({
          columnId: mdItem.columnId,
          name:
            mdItem && mdItem.value
              ? mdItem.value
              : this.tableInfo.header[index].colName,
          isChecked: mdItem.isChecked,
        });
      }
      this.tablePositions.header[index].isChecked = mdItem.isChecked;
      if (mdItem.value) {
        this.tablePositions.header[index].nickName = mdItem.value;
      }
    });
    this.getPositionsHeaderColumns();
    this.updateColumns = isEdit;
  }

  showOccupantPJIcon() {
    if (this.share && this.shareData) {
      return ContractTypeEnum.PJ === this.shareData.contractTypeId;
    }
    return this.typePosition.some((x) => +x.id === ContractTypeEnum.PJ);
  }
  showOccupantCLTIcon() {
    if (this.share && this.shareData) {
      const contractTypes = [
        ContractTypeEnum.CLT,
        ContractTypeEnum.CLT_Executive,
        ContractTypeEnum.CLT_Flex,
      ];
      return contractTypes.some((ctr) => ctr === this.shareData.contractTypeId);
    }
    return this.typePosition.some(
      (x) =>
        +x.id === ContractTypeEnum.CLT ||
        +x.id === ContractTypeEnum.CLT_Executive ||
        +x.id === ContractTypeEnum.CLT_Flex
    );
  }

  showPositions(event: boolean): void {
    this.isShowPositions = event;
    this.page = 1;

    this.updateColumns = true;
    this.onClearFilter();
    this.resetAllTables();
    if (event) {
      this.filterSearch = "";
      this.getSalaryPositionTable(false);
      return;
    }
    this.getSalaryTable(false);
  }

  async setFilterSearch(event: string) {
    if (event && event.length <= 3) return;
    this.page = 1;
    this.filterSearch = event;
    this.firstShowPosition = true;
    await this.getSalaryPositionTable();
  }
  // *********** END METHODS POSITIONS ***********

  async getSalaryChart() {
    const getSalarialChart = await this.salaryTableService
      .getSalaryGraph(
        this.tableId,
        this.minRange,
        this.maxRange,
        this.contractTypeId,
        this.hoursTypeId,
        this.unitId,
        this.groupId
      )
      .toPromise();

    // long gray bar of graph
    if (
      getSalarialChart &&
      getSalarialChart.chart &&
      getSalarialChart.chart[0]
    ) {
      getSalarialChart.chart[0].data.forEach((data) => {
        data.value = data.value * 10;
      });
    }

    this.salaryChart = getSalarialChart;
    if (this.salaryChart) {
      this.minRange = this.salaryChart.rangeMin;
      this.maxRange = this.salaryChart.rangeMax;
    }
    this.ngxSpinnerService.hide();
  }

  public sliderChange(event) {
    this.minRange = event[0];
    this.maxRange = event[1];
    this.groupId = 0;
    this.group = "Todos";
    this.unitId = 0;
    this.unit = "Todas";
    this.getSalaryChart();
  }

  resetFilters() {
    if (this.profiles) this.changeSelectProfile(this.profiles[0]);

    if (this.units) this.changeUnitSelected(this.units[0]);
  }

  getHeightChart() {
    return 600;
  }

  @HostListener("window:resize", ["$event"])
  onResize(event) {
    this.heightChartTable = window.innerHeight - 300;
  }

  // *********** METHODS DATA SHARE ***********
  async getShareData() {
    this.share = true;
    this.shareData = await this.salaryTableService
      .getShareData(this.secretKey)
      .toPromise();

    if (this.shareData) {
      this.tableId = this.shareData.tableId;
      this.projectId = this.shareData.projectId;
      this.contractTypeId = this.shareData.contractTypeId;
      this.hoursTypeId = this.shareData.hoursTypeId;
      this.groupId = this.shareData.groupId;
      this.showAllGsm = this.shareData.showAllGsm;
      this.permissions = this.shareData.permissions;
      this.shareHeader = this.salaryTableService.prepareShareHeader(
        this.shareData
      );
      this.minRange = this.shareData.rangeInit;
      this.maxRange = this.shareData.rangeFinal;

      switch (this.shareData.viewType) {
        case TableSalaryViewEnum.SalaryTable:
          await this.loadShareSalaryTable();
          break;
        case TableSalaryViewEnum.PositionSalaryTable:
          await this.loadSharePositionSalaryTable();
          break;
        case TableSalaryViewEnum.GraphSalaryTable:
          await this.loadShareGraphSalaryTable();
          break;
      }
    }
  }

  async getRangesGraph() {
    this.rangesGraph = await this.salaryTableService
      .getRangeSalaryGraph(this.tableId)
      .toPromise();

    this.minRange = this.rangesGraph ? this.rangesGraph.defaultRange.min : 1;
    this.maxRange = this.rangesGraph ? this.rangesGraph.defaultRange.max : 50;
  }

  getShareKey(): void {
    this.shareURL = null;

    const viewType =
      this.selectedVisualization.id === DisplayTypesEnum.COLUMNS
        ? TableSalaryViewEnum.GraphSalaryTable
        : this.isShowPositions
        ? TableSalaryViewEnum.PositionSalaryTable
        : TableSalaryViewEnum.SalaryTable;

    this.commonService
      .getShareKey({
        moduleId: Modules.tableSalary,
        moduleSubItemId: null,
        columnsExcluded: [
          this.tableInfo.header.filter((item) => !item.visible),
        ],
        parameters: {
          tableId: this.tableId,
          table: this.table,
          groupId: this.groupId,
          group: this.group,
          contractTypeId: this.contractTypeId,
          contractType: this.contractType,
          hoursTypeId: this.hoursTypeId,
          hoursType: this.hoursType,
          unitId: this.unitId,
          unit: this.unit,
          permissions: this.permissions,
          viewType: viewType,
          rangeInit: this.minRange,
          rangeFinal: this.maxRange,
        },
      })
      .subscribe((key) => {
        this.shareURL = `${environment.baseUrl}compartilhar-tabela-salarial/${key}`;
        this.clipboard.copy(this.shareURL);
        this.ngxSpinnerService.hide();
        this.openModalShare();
      });
  }

  async onSendEmail(): Promise<void> {
    await this.commonService
      .shareLink({
        to: this.email,
        url: this.shareURL,
      })
      .toPromise();

    this.ngxSpinnerService.hide();
  }

  onPutEmail(event) {
    this.email = event;
  }
  // *********** END METHODS DATA SHARE ***********

  // *********** COMMONS METHODS ***********
  onFilterChecked() {
    if (!this.isShowPositions) this.data = this.selected;
    else this.dataPosition = this.selected;
    this.selected = [];
  }

  async showAllRows(value: boolean) {
    this.showAllGsm = value;
    this.resetTables();
    this.onClearFilter();
    await this.getSalaryTable();
  }

  public async onClearFilter() {
    this.isClearFilter = false;
    this.page = 1;
    this.data = [];
    this.selected = [];
    if (this.secretKey) {
      switch (this.shareData.viewType) {
        case TableSalaryViewEnum.SalaryTable:
          await this.loadShareSalaryTable(false, true);
          break;
        case TableSalaryViewEnum.PositionSalaryTable:
          await this.loadSharePositionSalaryTable(false, true);
          break;
      }
      return;
    }
    this.isShowPositions
      ? this.getSalaryPositionTable()
      : this.getSalaryTable();
  }

  clearAllFilters(value: boolean) {
    this.isClearFilter = value;
  }

  changeTable(item: IDefault) {
    this.tableId = +item?.id;
    this.table = item.title;
    this.resetAllTables();
    this.cascateFilterByTable();
    if (this.selectedVisualization.id === DisplayTypesEnum.COLUMNS) {
      this.getSalaryChart();
      return;
    }
    this.isShowPositions
      ? this.getSalaryPositionTable()
      : this.getSalaryTable();
  }
  changeSelectPeriod(item: IDefault) {
    this.hoursTypeId = +item.id;
    this.hoursType = item.title;
    this.resetAllTables();
    if (this.selectedVisualization.id === DisplayTypesEnum.COLUMNS) {
      this.getSalaryChart();
      return;
    }
    this.isShowPositions
      ? this.getSalaryPositionTable(false)
      : this.getSalaryTable();
  }
  changeSelectTypePosition(item: IDefault) {
    this.contractTypeId = +item.id;
    this.contractType = item.title;
    this.resetAllTables();
    if (this.selectedVisualization.id === DisplayTypesEnum.COLUMNS) {
      this.getSalaryChart();
      return;
    }
    this.isShowPositions
      ? this.getSalaryPositionTable()
      : this.getSalaryTable();
  }
  changeSelectProfile(textSelected: IDefault) {
    this.groupId = +textSelected.id;
    this.group = textSelected.title;
    this.resetAllTables();
    this.getAllUnitsByFilter();
    if (this.selectedVisualization.id === DisplayTypesEnum.COLUMNS) {
      this.minRange = this.rangesGraph.defaultRange.min;
      this.maxRange = this.rangesGraph.defaultRange.max;
      this.getSalaryChart();
      return;
    }
    this.isShowPositions
      ? this.getSalaryPositionTable(true)
      : this.getSalaryTable();
  }

  public async onScrollDown() {
    if (this.secretKey) {
      switch (this.shareData.viewType) {
        case TableSalaryViewEnum.SalaryTable:
          await this.loadShareSalaryTable(true);
          break;
        case TableSalaryViewEnum.PositionSalaryTable:
          await this.loadSharePositionSalaryTable(true);
          break;
      }
      return;
    }
    if (this.isShowPositions && this.page > 0) {
      this.getSalaryPositionTable(true);
    } else if (this.page > 0) {
      this.getSalaryTable(true);
    }
  }

  async saveAndShowCols(modalChildren) {
    this.isShowPositions
      ? this.onShowPostionsChanges(modalChildren, true)
      : this.onShowChanges(modalChildren, true);
    const requestDisplayColumns: UpdateColumnsRequest = {
      displayColumns: this.displayColumns,
    };
    await this.salaryTableService
      .updateColumns(requestDisplayColumns)
      .toPromise();
    this.resetTables();
    this.isShowPositions
      ? this.getSalaryPositionTable()
      : this.getSalaryTable();
  }

  changeProfileLabel(items: IDefault[]) {
    if (!items) {
      return ``;
    }
    return items.length > 1 ? this.locales.all : items[0].title;
  }

  isAllChecked(event: any) {
    if (this.isShowPositions) {
      this.dataPosition.forEach((row) => {
        row[0].isChecked = event.currentTarget.checked;
        row[0].activeRow = event.currentTarget.checked ? "active" : "";
      });
      this.dataPosition = [...this.dataPosition];
      this.selected = event.currentTarget.checked ? [...this.dataPosition] : [];
      this.isClearFilter = event.currentTarget.checked;
      return;
    }

    this.data.forEach((row) => {
      row[0].isChecked = event.currentTarget.checked;
      row[0].activeRow = event.currentTarget.checked ? "active" : "";
    });
    this.data = [...this.data];
    this.selected = event.currentTarget.checked ? [...this.data] : [];
    this.isClearFilter = event.currentTarget.checked;
  }

  checkOnePosition(event: any, gsmChecked: any) {
    this.isClearFilter = event.currentTarget.checked;
    this.dataPosition.forEach((rows) => {
      const element = rows[0];

      if (element.gsm === gsmChecked) {
        element.isChecked = event.currentTarget.checked;

        if (event.currentTarget.checked) {
          this.selected.push(rows);
        } else {
          const indexRemove = this.selected.indexOf(rows, 0);
          if (this.selected.indexOf(indexRemove, 0) > -1) {
            this.selected.splice(indexRemove, 1);
          }
        }
      }
    });
  }

  checkOne(event: any, row: Body) {
    this.isClearFilter = event.currentTarget.checked;
    row[0].isChecked = event.currentTarget.checked;
    if (!event.currentTarget.checked) {
      const index = this.selected.indexOf(row, 0);
      if (index > -1) {
        this.selected.splice(index, 1);
        row[0].activeRow = "";
      }
      return;
    }

    this.selected.push(row);
    row[0].activeRow = "active";
  }

  changeSelectVisualization(event: IDisplayListTypes) {
    this.selectedVisualization = event;
    this.page = 1;
    this.resetTables();

    if (event.id === DisplayTypesEnum.COLUMNS) {
      this.getSalaryChart();
      return;
    }

    this.isShowPositions = false;
    this.getSalaryTable(false);
  }

  hoverEnterTr(event) {
    const elementHover = event.path[0].className.split(" ")[0];

    const elements = Array.from(document.getElementsByClassName(elementHover));

    elements.forEach((element) => {
      element.classList.add("hover-tr");
    });
  }

  hoverLeaveTr(event) {
    const elementHover = event.path[0].className.split(" ")[0];
    const elements = Array.from(document.getElementsByClassName(elementHover));

    elements.forEach((element) => {
      element.classList.remove("hover-tr");
    });
  }

  setStyleRow(gsm: number) {
    const ret = gsm % 2 == 0 ? "#0000000d" : "#ffffff";
    return ret;
  }
  // *********** END COMMONS METHODS ***********

  // *********** LOAD FILTERS ***********
  async getAllUnitsByFilter() {
    this.units = await this.commonService
      .getUnitsByFilter(this.tableId, this.groupId)
      .toPromise();
  }

  async changeUnitSelected(item: IUnit) {
    this.resetAllTables();
    this.unitId = +item.unitId;
    this.unit = item.unit;
    await this.cascateFiltersByUnit();
    if (this.selectedVisualization.id === DisplayTypesEnum.COLUMNS) {
      this.getSalaryChart();
      return;
    }
    this.isShowPositions
      ? this.getSalaryPositionTable(true)
      : this.getSalaryTable();
  }

  async getAllSalaryTable() {
    const salaries = await this.commonService.getAllSalaryTables().toPromise();

    this.listTables = salaries.tableSalaryResponses;
    this.tableId =
      salaries.tableSalaryResponses.length > 0
        ? +salaries.tableSalaryResponses[0].id
        : 0;

    this.projectId =
      salaries.tableSalaryResponses.length > 0
        ? salaries.tableSalaryResponses[0].projectId
        : 0;
  }

  async getAllContractTypes() {
    const contracts = await this.commonService.getOccupantList().toPromise();
    if (contracts) {
      this.typePosition = contracts.contractTypesResponse;
      this.contractTypeId = +contracts.contractTypesResponse[0].id;
    }
  }

  async getHourlyBase() {
    const hourlyBases = await this.commonService.getHourlyBasis().toPromise();
    if (hourlyBases) {
      this.period = hourlyBases.hoursBaseResponse;
    }
  }

  async getAllProfiles() {
    const profiles = await this.commonService
      .getAllProfiles(this.tableId, this.unitId)
      .toPromise();
    if (profiles) {
      this.profiles = [{ id: null, title: "Todos" }];
      this.profiles.push(...profiles.profilesResponse);
    }

    this.groupId = null;
  }

  public async cascateFilterByTable() {
    this.groupId = null;
    this.group = "Todos";
    await this.getAllProfiles();
    await this.getAllUnitsByFilter();
  }

  public async cascateFiltersByUnit() {
    await this.getAllProfiles();
    this.groupId = null;
    this.group = "Todos";
  }

  // *********** END FILTERS ***********

  async loadShareSalaryTable(scrolled: boolean = false, sort?: boolean) {
    const result = await this.salaryTableService
      .getSalaryTableShared(
        this.secretKey,
        this.tableId,
        this.unitId,
        this.contractTypeId,
        this.hoursTypeId,
        this.page,
        this.pageSize,
        this.groupId,
        this.isAsc,
        this.showAllGsm,
        this.sortColumnId
      )
      .toPromise();
    if (this.shareData) {
      this.tableInfo.header =
        result.table.body.length > 0
          ? result.table.header
          : this.tableInfo.header;

      if (result.table.body.length > 0) {
        const formatResult = result.table.body.map((info) => {
          const tableResult = {};
          info.map((res) => {
            this.tableInfo.header.forEach((item) => {
              if (item.colPos == res.colPos) {
                res.isChecked = false;
                tableResult[`${item.colPos}`] = res;
              }
            });
          });
          return tableResult;
        });

        const rows =
          this.data && isUndefined(sort)
            ? [...this.data, ...formatResult]
            : formatResult;
        this.data = rows;
      }
    } else if (!scrolled && result.nextPage === 0) {
      this.data = [];
    }
    this.page = result.nextPage;
    this.getHeaderColumns();
    this.ngxSpinnerService.hide();
  }

  async loadSharePositionSalaryTable(
    scrolled: boolean = false,
    sort?: boolean
  ) {
    const result = await this.salaryTableService
      .getShareSalaryPositionTable(
        this.secretKey,
        this.tableId,
        this.unitId,
        this.contractTypeId,
        this.hoursTypeId,
        this.page,
        this.pageSize,
        this.groupId,
        this.isAsc,
        this.filterSearch
      )
      .toPromise();

    this.isShowPositions = true;

    if (result.table) {
      this.tablePositions.header =
        result.table.body.length > 0 && this.tablePositions.header.length <= 0
          ? result.table.header
          : this.tablePositions.header;

      this.defaultHeader = copyObject(this.tablePositions.header);

      if (result.table.body.length > 0) {
        if ((this.filterSearch && this.filterSearch.length > 0) || !scrolled)
          this.dataPosition = [];

        result.table.body.forEach((item) => {
          this.dataPosition.push(item);
        });
      }
    } else if (!scrolled && result.nextPage === 0) {
      this.dataPosition = [];
    }

    this.page = result.nextPage;
    this.getPositionsHeaderColumns();
    this.ngxSpinnerService.hide();
  }
  async loadShareGraphSalaryTable() {
    const result = await this.salaryTableService
      .getShareSalaryGraph(
        this.secretKey,
        this.tableId,
        this.minRange,
        this.maxRange,
        this.contractTypeId,
        this.hoursTypeId,
        this.unitId,
        this.groupId
      )
      .toPromise();

    this.selectedVisualization.id = this.displayTypesEnum.COLUMNS;

    this.salaryChart = result;
    this.ngxSpinnerService.hide();
  }

  get gsmGlobalLabel() {
    return this.columHeaders
      ? this.columHeaders[SalaryTableHeaderEnum.GSM].nickName
      : null;
  }

  openModalPosition(positionRow: any) {
    const initialState = {
      positionId: positionRow.positionId,
      permissions: this.permissions,
      contractTypeId: this.contractTypeId,
      hoursTypeId: this.hoursTypeId,
      tableId: this.tableId,
      unitId: this.unitId,
      moduleId: Modules.tableSalary,
      gsmGlobalLabel: this.gsmGlobalLabel,
    };

    this.modalRef = this.modalService.show(ModalPositionDetailComponent, {
      class: "full-size positionDecorator",
      initialState,
    });

    this.modalRef.content.hideModalPositionEmitter.subscribe((res) => {
      if (res === true) {
        this.hideModalPosition(true);
        return;
      }

      this.hideModalPosition(false);
    });
  }

  hideModalPosition(hide: boolean) {
    const modalDiv = document.getElementsByClassName("positionDecorator");
    this._renderer.addClass(modalDiv[0], hide ? "hide-modal" : "show-modal");
  }

  openModalShare() {
    const initialState = {
      url: this.shareURL,
      title: locales.share,
      saveButtonLabel: locales.send,
      cancelButtonLabel: locales.cancel,
    };

    this.modalRef = this.modalService.show(ModalShareTableComponent, {
      class: "modal-dialog modal-share",
      initialState,
    });

    this.modalRef.content.onSaveEmitter.subscribe((res) => {
      this.email = res;
      this.onSendEmail();
    });
  }

  exportChart() {
    this.ngxSpinnerService.show();
    this.isPrinting = true;
    this.print.nativeElement.removeAttribute("class", "d-none");

    //changeChartSvgFont(this.print, "text", "Proxima Nova");
    html2canvas(this.print.nativeElement).then((canvas) => {
      const canvasUrl = canvas.toDataURL();

      this.downloadLink.nativeElement.href = canvasUrl;
      this.downloadLink.nativeElement.download = "tabela-salarial.png";
      this.downloadLink.nativeElement.click();

      //changeChartSvgFont(this.print, "text", "proxima-nova");

      this._toastrService.success("Download gerado com sucesso!");

      this.ngxSpinnerService.hide();
      this.isPrinting = false;
    });
  }

  getMinimumWidth() {
    const size = this.salaryChart.chart[0].data.length;
    const minWidth = size * 41;

    return minWidth < 1600 ? 1600 : minWidth;
  }

  openModalShow() {
    const initialState = {
      inputModalShowInputModal: this.inputModalShow,
      headerInfoInputModal: this.isShowPositions
        ? this.tablePositions.header
        : this.tableInfo.header,
      viaOriginalClick: true,
    };

    this.modalRef = this.modalService.show(TableHeaderModalComponent, {
      class: "modal-edit-right",
      initialState,
    });

    this.modalRef.content.restoreFilters.subscribe((res) => {
      if (this.isShowPositions) {
        this.onRestoreColsPositions(res);
        return;
      }

      this.onRestoreCols(res);
    });

    this.modalRef.content.save.subscribe((res) => {
      this.saveAndShowCols(res);
    });

    this.modalRef.content.showChanges.subscribe((res) => {
      if (this.isShowPositions) {
        this.onShowPostionsChanges(res);
        return;
      }

      this.onShowChanges(res);
    });
  }
}
