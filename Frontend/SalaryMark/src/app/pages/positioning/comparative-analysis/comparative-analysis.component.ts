import {
  IDisplay,
  IDisplayListTypes,
} from "./../../../shared/interfaces/positions";
import commonLocales from "@/locales/common";
import locales from "@/locales/positioning";
import visualizations from "@/pages/salary-table/common/visualizations";
import {
  ClickComparativeAnalyseChartDataInput,
  ComparativeAnalysisChartInput,
} from "@/shared/components/charts/comparative-analysis-chart/comparative-analysis-chart-input";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import { IDefault, IDisplayTypes } from "@/shared/interfaces/positions";
import { Modules, SubModules } from "@/shared/models/modules";
import {
  Body,
  ComparativeAnalysisTableInput,
  Header,
} from "@/shared/models/positioning";
import { IPermissions } from "@/shared/models/token";
import routerNames from "@/shared/routerNames";
import { CommonService } from "@/shared/services/common/common.service";
import { PositioningService } from "@/shared/services/positioning/positioning.service";
import { TokenService } from "@/shared/services/token/token.service";
import { styleBasedOnValue } from "@/utils/style-based-on-value";
import { Clipboard } from "@angular/cdk/clipboard";
import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnDestroy,
  OnInit,
} from "@angular/core";
import { MediaObserver } from "@angular/flex-layout";
import { FormBuilder, FormGroup } from "@angular/forms";
import { DomSanitizer } from "@angular/platform-browser";
import { ActivatedRoute } from "@angular/router";
import { ColumnMode } from "@swimlane/ngx-datatable";
import * as fs from "file-saver";
import { NgxSpinnerService } from "ngx-spinner";
import { environment } from "src/environments/environment";
import {
  DisplayImagesListEnum,
  DisplayTypesEnum,
} from "@/shared/components/button-list-visualization/common/typeVisualizationsEnum";

enum TableTypes {
  FIXED = 1,
  DYNAMIC = 2,
}

// This lets me use jquery
declare var $: any;

@Component({
  selector: "app-comparative-analysis",
  templateUrl: "./comparative-analysis.component.html",
  styleUrls: ["./comparative-analysis.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ComparativeAnalysisComponent implements OnInit, OnDestroy {
  public selectedVisualization: IDisplayListTypes = {
    id: DisplayTypesEnum.COLUMNS,
    image: DisplayImagesListEnum.COLUMN,
    title: "",
  };
  public visualizations: IDisplayListTypes[] = visualizations;

  locales = locales;
  displayTypes = DisplayTypesEnum;
  permissions: IPermissions;
  inputShareModal: IDialogInput;
  secretKey: string;
  shared = false;
  shareURL: string;
  email: string;
  isChangeDisplayBy = false;
  commonLocales = commonLocales;
  ColumnMode = ColumnMode;
  share: {
    user: string;
    date: Date;
    unit: string;
    unitId: string;
    displayBy: string;
    displayById: number;
    scenario: string;
    scenarioId: number;
    permissions: null;
  };

  tableTypes = TableTypes;
  showLine = true;
  unitSelected: IDefault;
  scenarioSelected: IDefault;
  dynamicColumns: any;
  tableClass = "";

  colorsRangeBasedOnValues = [
    {
      min: 130,
      max: 999,
      color: "#537158",
    },
    {
      min: 121,
      max: 130,
      color: "#668C6C",
    },
    {
      min: 110,
      max: 120.999,
      color: "#81A487",
    },
    {
      min: 103,
      max: 109.999,
      color: "#9FBDA1",
    },
    {
      min: 98,
      max: 102.999,
      color: "#B5CBB7",
    },
    {
      min: 93,
      max: 97.999,
      color: "#D4E1D6",
    },
    {
      min: 89,
      max: 92.999,
      color: "#EBF1EC",
    },
    {
      min: 86,
      max: 88.999,
      color: "#FAEEEC",
    },
    {
      min: 83,
      max: 85.999,
      color: "#F3D9D5",
    },
    {
      min: 78,
      max: 82.999,
      color: "#E2AAA2",
    },
    {
      min: 70,
      max: 77.999,
      color: "#CE7F74",
    },
    {
      min: 0,
      max: 69.999,
      color: "#A4483E",
    },
  ];

  listShowBy: IDefault[] = [];
  units: IDefault[] = [];
  scenarios: IDefault[] = [];

  comparativeAnalysisTableInput: ComparativeAnalysisTableInput;
  comparativeAnalysisChartInput: ComparativeAnalysisChartInput;
  clickComparativeAnalyseChartDataInput: ClickComparativeAnalyseChartDataInput;
  chartHeight = "";
  form: FormGroup;

  setGraph: IDisplayTypes;
  selectedDisplayBy: IDefault;
  selectedVersusDisplayBy: IDefault;

  constructor(
    private commonService: CommonService,
    private ngxSpinnerService: NgxSpinnerService,
    private positioningService: PositioningService,
    private tokenService: TokenService,
    private changeDetectorRef: ChangeDetectorRef,
    private domSanitizer: DomSanitizer,
    private fb: FormBuilder,
    private clipboard: Clipboard,
    private activatedRoute: ActivatedRoute,
    private mediaObserver: MediaObserver
  ) {}

  async ngOnInit(): Promise<void> {
    this.setGraph = this.visualizations.find((e) => {
      return e.id == 2;
    });

    this.selectedVisualization = visualizations ? visualizations[0] : null;

    this.configureScreen();

    this.secretKey = this.activatedRoute.snapshot.params.secretkey;

    if (this.secretKey) {
      this.shared = true;
      await this.loadSharedData();
      return;
    }

    this.inputShareModal = {
      disableFooter: false,
      idModal: "shareModal",
      title: locales.share,
      btnPrimaryTitle: locales.send,
      btnSecondaryTitle: locales.cancel,
    };

    this.permissions = this.tokenService.getPermissions();

    await Promise.all([
      this.positioningService
        .getDisplayBy()
        .toPromise()
        .then((displayBy) => (this.listShowBy = displayBy)),
      this.commonService
        .getMovements()
        .toPromise()
        .then((scenarios) => (this.scenarios = scenarios)),
      this.commonService
        .getAllUnits()
        .toPromise()
        .then(
          (units) =>
            (this.units = units.map((unit) => ({
              id: unit.unitId.toString(),
              title: unit.unit,
            })))
        ),
    ]);

    this.scenarioSelected =
      this.scenarios && this.scenarios.length > 0 ? this.scenarios[0] : null;
    this.selectedDisplayBy =
      this.listShowBy && this.listShowBy.length > 0 ? this.listShowBy[0] : null;
    this.selectedVersusDisplayBy =
      this.listShowBy && this.listShowBy.length >= 1
        ? this.listShowBy[1]
        : null;

    await this.loadData();

    this.ngxSpinnerService.hide();
    this.changeDetectorRef.detectChanges();
  }

  async loadSharedData(): Promise<void> {
    const display = parseInt(this.activatedRoute.snapshot.queryParams.type, 10);

    if (
      display === this.displayTypes.COLUMNS ||
      display === this.displayTypes.BAR
    ) {
      this.comparativeAnalysisChartInput = await this.positioningService
        .getComparativeAnalysisChartShared(this.secretKey)
        .toPromise();

      this.share = this.comparativeAnalysisChartInput.share;
    } else {
      this.comparativeAnalysisTableInput = await this.positioningService
        .getComparativeAnalysisTableShared(this.secretKey)
        .toPromise();

      this.share = this.comparativeAnalysisTableInput.share;
    }

    this.ngxSpinnerService.hide();
    this.changeDetectorRef.detectChanges();
  }

  configureScreen() {
    this.mediaObserver.media$.subscribe((x) => {
      switch (x.mqAlias) {
        case "xl":
          this.tableClass = "ngx-custom-lg-fr";
          break;
        case "md":
          this.tableClass = "ngx-custom-md-ex";
          break;
        default:
          this.tableClass = "ngx-custom";
          break;
      }
    });

    this.mediaObserver.media$.subscribe((x) => {
      switch (x.mqAlias) {
        case "md":
          this.chartHeight = (window.innerHeight * 0.4).toString();
          break;
        case "xl":
          this.chartHeight = (window.innerHeight * 0.6).toString();
          break;
        default:
          this.chartHeight = (window.innerHeight * 0.5).toString();
          break;
      }
    });
  }

  get tablesHeaders(): Header[] {
    return this.comparativeAnalysisTableInput.tables
      .map((table) => table.header.map((header) => header))
      .reduce((acc, val) => acc.concat(val), []);
  }

  get fixedTableHeader(): Header[] {
    return this.comparativeAnalysisTableInput.tables.find(
      (table) => table.type === this.tableTypes.FIXED
    ).header;
  }

  get tableRows() {
    return [
      ...this.comparativeAnalysisTableInput.tables
        .find((table) => table.type === this.tableTypes.FIXED)
        .body.map((body: Body[], fixedTableBodyRowIndex) => {
          return [
            ...body,
            ...this.comparativeAnalysisTableInput.tables
              .find((table) => table.type === this.tableTypes.DYNAMIC)
              .body.filter(
                (dynamicTableBodyRow, dynamicTableBodyRowIndex) =>
                  fixedTableBodyRowIndex === dynamicTableBodyRowIndex
              )
              .reduce((acc, val) => acc.concat(val), []),
          ];
        }),
      [
        ...this.comparativeAnalysisTableInput.tables.find(
          (table) => table.type === this.tableTypes.FIXED
        ).total,
        ...this.comparativeAnalysisTableInput.tables.find(
          (table) => table.type === this.tableTypes.DYNAMIC
        ).total,
      ],
    ];
  }

  showEnabledTableRow() {
    if (this.shared) {
      return this.tableRows;
    }

    const selectedRows = this.form
      .get("items")
      .value.filter((row) => row.show) as [{ name: string; show: boolean }];

    return this.tableRows.filter((row) =>
      selectedRows.map((selected) => selected.name).includes(row[0].name)
    );
  }

  styleButton(value: number) {
    return this.domSanitizer.bypassSecurityTrustStyle(styleBasedOnValue(value));
  }

  check() {
    this.changeDetectorRef.detectChanges();
  }

  ngOnDestroy(): void {}

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

  async getShareKey(): Promise<void> {
    this.shareURL = null;
    this.changeDetectorRef.detectChanges();
    this.commonService
      .getShareKey({
        moduleId: Modules.positioning,
        moduleSubItemId: SubModules.comparativeAnalysis,
        columnsExcluded: [[]],
        parameters: {
          displayBy: this.selectedVisualization.id,
          scenario: this.scenarioSelected.id,
        },
      })
      .subscribe((key) => {
        // tslint:disable-next-line: max-line-length
        this.shareURL =
          `${environment.baseUrl}posicionamento/configuracoes/${routerNames.POSITIONING.SHARE_COMPARATIVE_ANALYSIS}?type=${this.selectedVisualization.id}`.replace(
            ":secretkey",
            key
          );
        this.clipboard.copy(this.shareURL);
        this.changeDetectorRef.detectChanges();
        this.ngxSpinnerService.hide();
      });
  }

  buildFormTable() {
    if (!this.comparativeAnalysisTableInput.tables) {
      this.form = this.fb.group({
        items: this.fb.array([]),
      });

      return;
    }

    this.form = this.fb.group({
      items: this.fb.array([
        ...this.tableRows.map((item) =>
          this.fb.group({
            name: this.fb.control(item[0].name),
            show: this.fb.control(true),
          })
        ),
      ]),
    });

    this.form.get("items").valueChanges.subscribe((item) => {
      setTimeout(() => {
        this.check();
      }, 250);
    });
  }

  buildFormGraphic() {
    if (!this.comparativeAnalysisChartInput.chart) {
      this.form = this.fb.group({
        items: this.fb.array([]),
      });

      return;
    }

    const basis = Array.from(
      new Set(
        this.comparativeAnalysisChartInput.chart.chart
          .reduce((acc, val) => acc.concat(val.data), [])
          .map((data) => data.name)
      )
    );

    this.form = this.fb.group({
      items: this.fb.array(
        basis.map((data) =>
          this.fb.group({
            name: this.fb.control(data),
            show: this.fb.control(true),
          })
        )
      ),
    });
  }

  openListModal(
    clickComparativeAnalyseChartDataInput: ClickComparativeAnalyseChartDataInput
  ) {
    this.clickComparativeAnalyseChartDataInput =
      clickComparativeAnalyseChartDataInput;
    $("#showChartDetailList").modal("show");
  }

  openListModalTable(item: Body, sendCarrerAxis: boolean = false) {
    const header = sendCarrerAxis
      ? this.comparativeAnalysisTableInput.tables
          .find((table) => table.type === this.tableTypes.DYNAMIC)
          .header.find((h) => h.colPos === item.colPos)
      : null;
    const comparativeAnalyseInput: ClickComparativeAnalyseChartDataInput = {
      careerAxis: header ? header.name : "",
      categoryId: item.categoryId,
      displayBy: Number(this.selectedVisualization.id),
      scenario: Number(this.scenarioSelected.id),
      unitId: this.unitSelected ? Number(this.unitSelected.id) : null,
    };
    this.clickComparativeAnalyseChartDataInput = comparativeAnalyseInput;
    $("#showChartDetailList").modal("show");
  }

  async loadData(): Promise<void> {
    if (!this.selectedVisualization) return;

    if (
      this.selectedVisualization.id === DisplayTypesEnum.COLUMNS ||
      this.selectedVisualization.id === DisplayTypesEnum.BAR
    ) {
      await this.loadComparativeAnalysisChart();
      this.buildFormGraphic();
      this.changeDetectorRef.detectChanges();
    } else {
      await this.loadComparativeAnalysisTable();
      this.buildFormTable();
    }
  }

  async loadComparativeAnalysisChart(): Promise<void> {
    this.comparativeAnalysisChartInput = await this.positioningService
      .getComparativeAnalysisChart(
        this.selectedDisplayBy.id.toString(),
        this.scenarioSelected.id.toString(),
        this.unitSelected ? this.unitSelected.id.toString() : ""
      )
      .toPromise();
  }

  async loadComparativeAnalysisTable(): Promise<void> {
    this.comparativeAnalysisTableInput = await this.positioningService
      .getComparativeAnalysisTable(
        this.selectedDisplayBy.id.toString(),
        this.selectedVersusDisplayBy.id.toString(),
        this.scenarioSelected.id.toString(),
        this.unitSelected ? this.unitSelected.id.toString() : ""
      )
      .toPromise();
  }

  showEnabledChartData(
    charts: ComparativeAnalysisChartInput
  ): ComparativeAnalysisChartInput {
    if (this.shared) {
      return charts;
    }

    const selectedStatistics = this.form
      .get("items")
      .value.filter((row) => row.show) as [{ name: string; show: boolean }];

    return {
      ...charts,
      chart: {
        average: charts?.chart?.average,
        chart: charts?.chart?.chart.map((statistic) => {
          return {
            ...statistic,
            data: statistic.data.filter((data) =>
              selectedStatistics
                .map((selectedData) => selectedData.name)
                .includes(data.name)
            ),
          };
        }),
      },
    };
  }

  async getFileSpreadsheet(): Promise<void> {
    const data = await this.positioningService
      .getComparativeAnalysisTableExportedToExcel(
        this.selectedVisualization.id.toString(),
        this.scenarioSelected.id.toString(),
        this.unitSelected ? this.unitSelected.id.toString() : ""
      )
      .toPromise();

    const file = new Blob([data], {
      type: data.type,
    });

    const blob = window.URL.createObjectURL(file);
    fs.saveAs(blob, "Analise_Comparativa.xlsx");
    this.ngxSpinnerService.hide();
  }

  getStylesBasedOnDataValue(data: number) {
    if (!data) {
      return "";
    }
    const colorRange = this.colorsRangeBasedOnValues.find(
      (value) => data >= value.min && data <= value.max
    );
    const text = data <= 102.999 && data >= 83 ? "#595959" : "white";

    return this.domSanitizer.bypassSecurityTrustStyle(
      `color:${!colorRange ? "#595959" : text}; background-color:${
        colorRange ? colorRange.color : "#ddd"
      }`
    );
  }

  async changeVisualization(displayType: IDisplayListTypes): Promise<void> {
    if (this.shared) {
      return;
    }

    this.selectedVisualization = displayType;

    await this.loadData();
    this.ngxSpinnerService.hide();
    this.changeDetectorRef.detectChanges();
  }

  async changeTable(item: IDefault): Promise<void> {
    this.selectedDisplayBy = item;

    if (this.isChangeDisplayBy) {
      await this.loadData();
      this.ngxSpinnerService.hide();
      this.changeDetectorRef.detectChanges();
    } else {
      this.isChangeDisplayBy = true;
    }
  }

  async versusChangeTable(item: IDefault): Promise<void> {
    this.selectedVersusDisplayBy = item;

    if (this.isChangeDisplayBy) {
      await this.loadData();
      this.ngxSpinnerService.hide();
      this.changeDetectorRef.detectChanges();
    } else {
      this.isChangeDisplayBy = true;
    }
  }

  async changeSelectUnit(unit: IDefault): Promise<void> {
    this.unitSelected = unit;
    await this.loadData();
    this.ngxSpinnerService.hide();
    this.changeDetectorRef.detectChanges();
  }

  async changeSelectScenario(scenario: IDefault): Promise<void> {
    this.scenarioSelected = scenario;
    await this.loadData();
    this.ngxSpinnerService.hide();
    this.changeDetectorRef.detectChanges();
  }

  changeUnitLabel(items: IDefault[]) {
    if (!items) {
      return "";
    }
    return items.length > 1
      ? this.locales.all
      : items.length === 1
      ? items[0].title
      : "";
  }

  setWidthColumn(item: Header) {
    const columns = [2, 3, 4];
    if (columns.includes(item.columnId)) return 120;
    return 150;
  }

  changeSelectVisualization(event: IDisplayListTypes) {
    this.selectedVisualization = event;
  }

  getImageExportChart() {
    //TODO Fazer exportação de PDF E PNG
    alert("EXPORTADO - DEV");
  }
}
