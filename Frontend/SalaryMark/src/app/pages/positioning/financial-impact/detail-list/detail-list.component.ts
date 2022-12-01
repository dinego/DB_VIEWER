import commonLocales from "@/locales/common";
import locales from "@/locales/positioning";
import { ClickFinancialImpactChartDataInput } from "@/shared/components/charts/financial-impact-chart/financial-impact-chart-input";
import { FullInfoFinancialImpactEnum } from "@/shared/enum/full-info-positioning-enum";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import {
  ColsFinancialImpact,
  FinancialImpactTable,
} from "@/shared/models/positioning";
import { FrameworkService } from "@/shared/services/framework/framework.service";
import { PositioningService } from "@/shared/services/positioning/positioning.service";
import {
  ChangeDetectorRef,
  Component,
  Input,
  OnInit,
  SimpleChanges,
} from "@angular/core";
import { ColumnMode, SelectionType } from "@swimlane/ngx-datatable";
import { NgxSpinnerService } from "ngx-spinner";
import * as fs from "file-saver";
import { ExportCSVService } from "@/shared/services/export-csv/export-csv.service";

@Component({
  selector: "app-detail-list",
  templateUrl: "./detail-list.component.html",
  styleUrls: ["./detail-list.component.scss"],
})
export class DetailListComponent implements OnInit {
  public financialImpactTable: FinancialImpactTable = {
    category: "",
    scenario: "",
    table: { header: [], body: [] },
  };
  public isClearFilter = false;
  public dataResult;
  public sortColumnId?: number;
  public selected = [];
  ColumnMode = ColumnMode;
  SelectionType = SelectionType;
  public headerHeight = 52;
  public rowHeight = 50;
  public pageLimit = 20;
  public isLoading: boolean;
  public isShowColName: boolean;
  public inputModalShow: IDialogInput;
  public commonLocales = commonLocales;
  public fullInfoPositionEnum = FullInfoFinancialImpactEnum;
  public locales = locales;
  public share = false;
  public isAsc = true;
  public query: string;
  public termQuery: string = "";

  public isMMExport: boolean = true;
  public isMIExport: boolean = false;
  public contractTypeExport: string = "1";
  public hourTypeExport: string = "0";
  public unitExport?: number = null;
  public sortColumnIdExport?: number = null;
  public isAscExport: boolean = true;
  public columnsExport: number[] = [];

  @Input() secretKey: string;
  @Input() title: string = "";
  @Input()
  clickFinancialImpactChartDataInput: ClickFinancialImpactChartDataInput;

  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    private ngxSpinnerService: NgxSpinnerService,
    private positioningService: PositioningService,
    private exportSVGService: ExportCSVService
  ) {}

  ngOnInit(): void {
    this.setInputModalShow();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.clickFinancialImpactChartDataInput) {
      this.resetTableInfo();
      this.getFullInfoPositioningFinancialImpact();
    }

    if (this.inputModalShow) {
      this.setInputModalShow();
    }
  }

  setInputModalShow() {
    this.inputModalShow = {
      disableFooter: true,
      fullSize: true,
      idModal: "tableShowModal",
      title: this.title,
    };
  }

  public getFullInfoPositionsShare() {
    this.ngxSpinnerService.show();
    this.positioningService
      .getFullInfoPositioningFinancialImpactShare(
        this.secretKey,
        this.clickFinancialImpactChartDataInput.serieId,
        this.clickFinancialImpactChartDataInput.categoryId,
        this.sortColumnId,
        this.isAsc
      )
      .subscribe((result) => {
        if (result.table) {
          this.financialImpactTable = result;
          this.financialImpactTable.table.header =
            result.table.body.length > 0
              ? result.table.header
              : this.financialImpactTable.table.header;

          if (result.table.body.length > 0) {
            const formatResult = result.table.body.map((info) => {
              let tableResult = {};
              info.map((res) => {
                this.financialImpactTable.table.header.forEach((item) => {
                  if (item.colPos == res.colPos) {
                    tableResult[`${item.colPos}`] = res;
                  }
                });
              });
              return tableResult;
            });
            this.dataResult = formatResult;
          }
        } else {
          this.dataResult = [];
        }
        this.financialImpactTable.category = result.category
          ? result.category
          : this.financialImpactTable.category;
        this.financialImpactTable.scenario = result.scenario
          ? result.scenario
          : this.financialImpactTable.scenario;
        this.isLoading = false;
        this.ngxSpinnerService.hide();
        this.changeDetectorRef.detectChanges();
        this.changeDetectorRef.markForCheck();
      });
  }

  public onClearFilter() {
    this.isClearFilter = false;
    if (this.secretKey) {
      this.getFullInfoPositionsShare();
    } else {
      this.getFullInfoPositions();
    }
  }

  public onFilterChecked(): void {
    this.dataResult = this.selected;
    this.changeDetectorRef.detectChanges();
    this.changeDetectorRef.markForCheck();
  }
  public getFullInfoPositions() {
    this.positioningService
      .getFullInfoPositioningFinancialImpact(
        this.clickFinancialImpactChartDataInput.displayBy,
        this.clickFinancialImpactChartDataInput.serieId,
        this.clickFinancialImpactChartDataInput.scenario,
        this.clickFinancialImpactChartDataInput.unitId,
        this.clickFinancialImpactChartDataInput.categoryId,
        this.sortColumnId,
        this.isAsc
      )
      .subscribe((result) => {
        if (result.table && result.table.header && result.table.body) {
          this.financialImpactTable.table.header =
            result.table.body.length > 0
              ? result.table.header
              : this.financialImpactTable.table.header;

          if (result.table.body.length > 0) {
            const formatResult = result.table.body.map((info) => {
              let tableResult = {};
              info.map((res) => {
                this.financialImpactTable.table.header.forEach((item) => {
                  if (item.colPos == res.colPos) {
                    tableResult[`${item.colPos}`] = res;
                  }
                });
              });
              return tableResult;
            });
            this.dataResult = formatResult;
          }
        } else {
          this.dataResult = [];
        }
        this.financialImpactTable.category = result.category;
        this.financialImpactTable.scenario = result.scenario;
        this.ngxSpinnerService.hide();
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
        this.changeDetectorRef.markForCheck();
      });
  }

  public onSort(event: any) {
    this.isAsc = event.newValue !== "desc";
    this.isLoading = true;
    this.sortColumnId = event.column.prop;
    if (this.secretKey) {
      this.getFullInfoPositionsShare();
    } else {
      this.getFullInfoPositions();
    }
  }

  onSelect({ selected }) {
    this.selected.splice(0, this.selected.length);
    this.selected.push(...selected);
    this.isClearFilter = this.selected.length > 0;
    this.changeDetectorRef.detectChanges();
    this.changeDetectorRef.markForCheck();
  }

  getFormatValue(row: any, header: any): string {
    return row[`${header.colPos}`].value.split(",")[0];
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

  setColumnWidth(column: ColsFinancialImpact) {
    const columns = [
      FullInfoFinancialImpactEnum.Employee,
      FullInfoFinancialImpactEnum.HoursBase,
    ];
    return columns.includes(column.columnId)
      ? 180
      : column.colPos >= 3 &&
        column.columnId != this.fullInfoPositionEnum.FinancialImpact
      ? 100
      : column.columnId === this.fullInfoPositionEnum.FinancialImpact
      ? 180
      : 300;
  }

  resetTableInfo() {
    this.financialImpactTable = {
      category: "",
      scenario: "",
      table: { header: [], body: [] },
    };
    this.dataResult = [];
  }

  public getFullInfoPositioningFinancialImpact() {
    this.isLoading = true;
    if (this.secretKey) {
      this.getFullInfoPositionsShare();
    } else {
      this.getFullInfoPositions();
    }
  }

  public async onChangeSearch(eventQuery: string) {
    if (eventQuery.length >= 3 || eventQuery.length == 0) {
      this.query = eventQuery;
      this.termQuery = this.query;
      this.resetTableInfo();
      this.getFullInfoPositioningFinancialImpact();
    }
  }

  async getFileSpreadsheet() {
    this.ngxSpinnerService.show();

    const objectToMapCSV = {
      columns: this.financialImpactTable.table.header,
      bodyData: this.dataResult,
      tableTitle:
        locales.financialImpactTitleTable +
        " - " +
        this.financialImpactTable.category,
      scenario: this.financialImpactTable.scenario,
    };

    await this.exportSVGService.downloadExcelFinancialImpactModal(
      objectToMapCSV
    );

    this.ngxSpinnerService.hide();
  }
}
