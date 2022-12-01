import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  Input,
  OnChanges,
  SimpleChanges,
  ChangeDetectorRef,
} from "@angular/core";
import { NgxSpinnerService } from "ngx-spinner";

import { IDialogInput } from "@/shared/interfaces/dialog-input";
import locales from "@/locales/positioning";
import {
  ClickComparativeAnalyseChartDataInput,
  FullInfoComparativeAnalysis,
  CheckItem,
} from "@/shared/components/charts/comparative-analysis-chart/comparative-analysis-chart-input";

import { PositioningService } from "@/shared/services/positioning/positioning.service";
import { ColumnMode, SelectionType } from "@swimlane/ngx-datatable";
import commonLocales from "@/locales/common";
import { DomSanitizer } from "@angular/platform-browser";
import { styleBasedOnValue } from "@/utils/style-based-on-value";
import { FullInfoPositioningEnum } from "@/shared/enum/full-info-positioning-enum";
import { ColsFinancialImpact } from "@/shared/models/positioning";
import { isUndefined } from "@/shared/common/functions";
import { ExportCSVService as ExportCSVService } from "@/shared/services/export-csv/export-csv.service";

@Component({
  selector: "app-chart-detail-list",
  templateUrl: "./chart-detail-list.component.html",
  styleUrls: ["./chart-detail-list.component.scss"],
  changeDetection: ChangeDetectionStrategy.Default,
})
export class ChartDetailListComponent implements OnInit, OnChanges {
  @Input()
  clickComparativeAnalyseChartDataInput: ClickComparativeAnalyseChartDataInput;
  @Input() secretKey: string;

  public locales = locales;
  public inputModalShow: IDialogInput;

  public myPage = 1;
  public lastPage = 0;
  public fullInfoComparativeAnalysis: FullInfoComparativeAnalysis = {
    category: "",
    nextPage: 0,
    scenario: "",
    table: { body: [], header: [] },
  };
  public checkInputAll: boolean;
  public checkInputs: Array<CheckItem> = [];
  public isClearFilter = false;
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
  public commonLocales = commonLocales;
  public fullInfoPositionEnum = FullInfoPositioningEnum;
  public query: string;

  constructor(
    private ngxSpinnerService: NgxSpinnerService,
    private ref: ChangeDetectorRef,
    private positioningService: PositioningService,
    private domSanitizer: DomSanitizer,
    private exportSVGService: ExportCSVService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (this.clickComparativeAnalyseChartDataInput) {
      this.resetTableInfo();
      this.getAllSalaryStrategyTable();
    }
  }

  ngOnInit(): void {
    this.inputModalShow = {
      disableFooter: false,
      idModal: "showChartDetailList",
      title: locales.comparativeAnalysis,
      btnPrimaryTitle: "",
      btnSecondaryTitle: "",
      fullSize: true,
    };
  }

  private getAllSalaryStrategyTable(scrolled: boolean = false, sort?: boolean) {
    this.isLoading = true;
    if (this.secretKey) {
      this.positioningService
        .getFullInfoComparativeAnalysisShare(
          this.myPage.toString(),
          "20",
          this.clickComparativeAnalyseChartDataInput?.categoryId,
          this.secretKey,
          this.clickComparativeAnalyseChartDataInput.careerAxis,
          this.sortColumnId,
          this.isAsc
        )
        .subscribe((fullInfoComparativeAnalysis) => {
          this.prepareDataResult(fullInfoComparativeAnalysis, scrolled, sort);
        });
    } else {
      this.positioningService
        .getFullInfoComparativeAnalysis(
          this.myPage.toString(),
          "20",
          this.clickComparativeAnalyseChartDataInput?.categoryId,
          this.clickComparativeAnalyseChartDataInput.careerAxis,
          this.clickComparativeAnalyseChartDataInput.displayBy?.toString(),
          this.clickComparativeAnalyseChartDataInput.scenario?.toString(),
          this.clickComparativeAnalyseChartDataInput.unitId?.toString(),
          this.sortColumnId,
          this.isAsc
        )
        .subscribe((fullInfoComparativeAnalysis) => {
          this.prepareDataResult(fullInfoComparativeAnalysis, scrolled, sort);
        });
    }
  }

  prepareDataResult(
    fullInfoComparativeAnalysis: FullInfoComparativeAnalysis,
    scrolled: boolean,
    sort?: boolean
  ) {
    if (fullInfoComparativeAnalysis && fullInfoComparativeAnalysis.table) {
      this.fullInfoComparativeAnalysis.table.header =
        fullInfoComparativeAnalysis.table.header
          ? fullInfoComparativeAnalysis.table.header
          : this.fullInfoComparativeAnalysis.table.header;

      if (
        fullInfoComparativeAnalysis.table.body &&
        fullInfoComparativeAnalysis.table.body.length > 0
      ) {
        const formatResult = fullInfoComparativeAnalysis.table.body.map(
          (info) => {
            let tableResult = {};
            info.map((res) => {
              this.fullInfoComparativeAnalysis.table.header.forEach((item) => {
                if (item.colPos == res.colPos) {
                  tableResult[`${item.colPos}`] = res;
                }
              });
            });
            return tableResult;
          }
        );
        const rows =
          this.dataResult && isUndefined(sort)
            ? [...this.dataResult, ...formatResult]
            : formatResult;
        this.dataResult = rows;
      }
    } else if (!scrolled && fullInfoComparativeAnalysis.nextPage === 0) {
      this.dataResult = [];
    }
    this.fullInfoComparativeAnalysis.category =
      fullInfoComparativeAnalysis && fullInfoComparativeAnalysis.category
        ? fullInfoComparativeAnalysis.category
        : this.fullInfoComparativeAnalysis.category;
    this.fullInfoComparativeAnalysis.scenario =
      fullInfoComparativeAnalysis && fullInfoComparativeAnalysis.scenario
        ? fullInfoComparativeAnalysis.scenario
        : this.fullInfoComparativeAnalysis.scenario;
    this.myPage = fullInfoComparativeAnalysis
      ? fullInfoComparativeAnalysis.nextPage
      : 0;
    this.ref.detectChanges();
    this.ref.markForCheck();
    this.isLoading = false;
    this.ngxSpinnerService.hide();
  }

  onClearFilter() {
    this.isClearFilter = false;
    this.resetTableInfo();
    this.getAllSalaryStrategyTable();
  }

  onFilterChecked() {
    this.dataResult = this.selected;
    this.ref.markForCheck();
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

  getFormatValue(row: any, header: any): string {
    return row[`${header.colPos}`].value.split(",")[0];
  }

  getReplaceValue(row: any, header: any): string {
    return row[`${header.colPos}`].value.replace(",", ".");
  }

  onSelect({ selected }) {
    this.selected.splice(0, this.selected.length);
    this.selected.push(...selected);
    this.isClearFilter = this.selected.length > 0;
  }

  onSort(event: any) {
    this.isAsc = event.newValue !== "desc";
    this.isLoading = true;
    this.myPage = 1;
    this.sortColumnId = event.column.prop;
    this.getAllSalaryStrategyTable(false, true);
  }

  resetTableInfo() {
    this.myPage = 1;
    this.fullInfoComparativeAnalysis = {
      category: "",
      nextPage: 0,
      scenario: "",
      table: { body: [], header: [] },
    };
    this.dataResult = [];
  }

  styleButton(value: number) {
    return this.domSanitizer.bypassSecurityTrustStyle(styleBasedOnValue(value));
  }

  setColumnWidth(column: ColsFinancialImpact) {
    return column.colPos >= 3 &&
      column.columnId != this.fullInfoPositionEnum.CompareMidPoint &&
      column.columnId != this.fullInfoPositionEnum.Employee
      ? 100
      : column.columnId === this.fullInfoPositionEnum.CompareMidPoint ||
        column.columnId === this.fullInfoPositionEnum.Employee
      ? 180
      : 300;
  }

  onChangeSearch(event) {
    this.query = event;
  }

  async exportTable() {
    this.ngxSpinnerService.show();

    const objectToMapCSV = {
      columns: this.fullInfoComparativeAnalysis.table.header,
      bodyData: this.dataResult,
      tableTitle: locales.comparativeAnalysis,
      scenario: this.fullInfoComparativeAnalysis.scenario,
    };

    await this.exportSVGService.downloadExcelComparativeAnalyseDetail(
      objectToMapCSV
    );

    this.ngxSpinnerService.hide();
  }
}
