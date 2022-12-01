import { isUndefined } from "@/shared/common/functions";
import { DisplayTypesEnum } from "@/shared/components/button-list-visualization/common/typeVisualizationsEnum";
import { FrameworkColumnsMainEnum } from "@/shared/enum/framework-columns-main-enum";
import { MovimentTypeEnum } from "@/shared/enum/moviment-type-enum";
import {
  IDefault,
  IDisplay,
  IDisplayListTypes,
  IUnit,
} from "@/shared/interfaces/positions";
import { Table } from "@/shared/models/framework";
import { HourlyBasisEnum } from "@/shared/models/hourly-basis";
import { Header } from "@/shared/models/map-table";
import { CommonService } from "@/shared/services/common/common.service";
import { FrameworkService } from "@/shared/services/framework/framework.service";
import { styleBasedOnValue } from "@/utils/style-based-on-value";
import { Component, ElementRef, OnInit } from "@angular/core";
import { MediaObserver } from "@angular/flex-layout";
import { DomSanitizer } from "@angular/platform-browser";
import { ColumnMode, SelectionType } from "@swimlane/ngx-datatable";
import { NgxSpinnerService } from "ngx-spinner";
import displayTypesList from "./common/visualizations";

@Component({
  selector: "app-framework-tab",
  templateUrl: "./framework-tab.component.html",
  styleUrls: ["./framework-tab.component.scss"],
})
export class FrameworkTabComponent implements OnInit {
  public displayTypesList = displayTypesList;
  public displaySelected = displayTypesList[0];
  public frameworkColumnsEnum = FrameworkColumnsMainEnum;
  public headerHeight = 54;
  public rowHeight = 50;
  public query: string;
  public dataResult = [];
  public columHeaders = [];
  public ColumnMode = ColumnMode;
  public SelectionType = SelectionType;
  public isLoading: boolean;
  public isShowModeratedMovementTable = false;
  public isShowIdealMovementTable = false;
  public myPage = 1;
  public movementTable: Table = {
    header: [],
    body: [],
    displayName: ``,
    displayType: 0,
  };
  public minBar: number;
  public maxBar: number;
  public selectedContractId = "1";
  public selectedMonthlyId = "1";
  public termQuery = "";
  public selectedUnit = null;
  public sortColumnId;
  public isAsc = true;
  public columnsFiltered: number[] = [];
  public percentArrays: any[] = [];
  public hourlyBasisEnum = HourlyBasisEnum;
  public hoursType = HourlyBasisEnum.MonthSalary;
  public pageLimit = 20;
  public updateColumns = false;
  public tableClass = "";
  public displayTypeEnum = DisplayTypesEnum;
  public movementsList: IDefault[];
  public movementSelected: IDefault;
  public unitList: IUnit[];
  public unitSelected: IUnit;

  constructor(
    private frameworkService: FrameworkService,
    private ngxSpinnerService: NgxSpinnerService,
    private domSanitizer: DomSanitizer,
    private el: ElementRef,
    private mediaObserver: MediaObserver,
    private commonService: CommonService
  ) {}

  async ngOnInit() {
    this.ngxSpinnerService.show();

    await this.configureScreen();
    await this.getAllUnits();
    await this.getMovements();
    await this.setMovementBySelected();
    await this.getFramework();

    this.ngxSpinnerService.hide();
  }

  async getAllUnits() {
    const resultUnits = await this.commonService.getAllUnits().toPromise();
    this.getAllUnitsByResult(resultUnits);
  }

  public async getAllUnitsByResult(result) {
    if (result.length > 0) {
      this.unitList = result;
      this.unitSelected = this.unitList[0];
    }
  }

  public setUnitSelected(item: IUnit) {
    this.selectedUnit = item;
  }

  async getMovements() {
    this.movementsList = await this.commonService.getMovements().toPromise();
    this.movementSelected = this.movementsList[0];
  }

  public async setMovementSelected(item: IDefault) {
    this.movementSelected = item;
    await this.setMovementBySelected();
  }

  async setMovementBySelected() {
    this.isShowModeratedMovementTable =
      parseInt(this.movementSelected.id) === MovimentTypeEnum.moderatedMovement;
    this.isShowIdealMovementTable =
      parseInt(this.movementSelected.id) === MovimentTypeEnum.idealMovement;
  }

  setColumnWidth(header: Header) {
    if (this.frameworkColumnsEnum.CompareMidPoint === header.columnId) {
      return 200;
    } else if (
      header.columnId === this.frameworkColumnsEnum.GSM ||
      header.columnId === this.frameworkColumnsEnum.Salary ||
      header.columnId === this.frameworkColumnsEnum.HourlyBasis ||
      header.columnId === this.frameworkColumnsEnum.CompareMidPoint ||
      header.colName.includes("%")
    )
      return 100;

    return 280;
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

  getRowItems(tableBody: any, tableHeader) {
    const formatResult = tableBody.map((info) => {
      let tableResult = {};
      info.map((res) => {
        tableHeader.forEach((item) => {
          if (item.colPos == res.colPos) {
            tableResult[`${item.colPos}`] = res;
          }
        });
      });
      return tableResult;
    });

    return formatResult;
  }

  public getHeaderColumns() {
    this.columHeaders = this.movementTable.header.filter((item) => {
      if (item.visible) {
        if (item.isChecked) {
          return item;
        }
      } else {
        return item;
      }
    });
  }

  public async getFramework(scrolled: boolean = false, sort?: boolean) {
    this.isLoading = !sort ? true : false;
    const result = await this.frameworkService
      .getAllFrameworks(
        this.isShowModeratedMovementTable,
        this.isShowIdealMovementTable,
        this.myPage,
        this.selectedContractId,
        this.selectedMonthlyId,
        this.termQuery,
        this.selectedUnit ? this.selectedUnit.unitId : "",
        this.sortColumnId,
        this.isAsc,
        this.columnsFiltered
      )
      .toPromise();

    if (result.framework.tables) {
      const tableItems = this.isShowModeratedMovementTable
        ? result.framework.tables.filter((t) => t.displayType === 1)[0]
        : result.framework.tables.filter((t) => t.displayType === 2)[0];

      this.dataResult = this.termQuery ? [] : this.dataResult;
      this.movementTable.header =
        tableItems.body.length > 0 && this.movementTable.header.length <= 0
          ? tableItems.header
          : this.movementTable.header;

      this.movementTable.body = tableItems.body;
      this.movementTable.displayName = tableItems.displayName;
      this.movementTable.displayType = tableItems.displayType;

      this.movementTable.body.forEach((mov, index) => {
        const percentArray = mov.filter((f) => f.colPos === 19);
        this.percentArrays.push(...percentArray);
      });

      this.minBar = Math.min.apply(
        Math,
        this.percentArrays.map(function (o) {
          return o.value;
        })
      );

      this.maxBar = Math.max.apply(
        Math,
        this.percentArrays.map(function (o) {
          return o.value;
        })
      );

      if (tableItems) {
        const formatResult = this.getRowItems(
          tableItems.body,
          tableItems.header
        );
        const rows =
          this.dataResult && isUndefined(sort)
            ? [...this.dataResult, ...formatResult]
            : formatResult;
        this.dataResult = [...rows];
      }
    } else if (!scrolled && result.nextPage === 0) {
      this.dataResult = [];
    }
    this.myPage = result.nextPage;
    this.getHeaderColumns();
    this.ngxSpinnerService.hide();
    this.isLoading = false;
  }

  styleButton(value: number) {
    return this.domSanitizer.bypassSecurityTrustStyle(styleBasedOnValue(value));
  }

  setClassBody(header: Header) {
    if (header.colPos === this.frameworkColumnsEnum.PercentagemArea)
      return "datatable-body-cell border-left-white";

    return "datatable-body-cell";
  }

  setClassHeader(header: Header) {
    if (header.colPos === this.frameworkColumnsEnum.PercentagemArea)
      return "header-column-cell border-left-white";

    if (
      header.columnId === this.frameworkColumnsEnum.GSM ||
      header.columnId === this.frameworkColumnsEnum.Salary ||
      header.columnId === this.frameworkColumnsEnum.HourlyBasis ||
      header.columnId === this.frameworkColumnsEnum.CompareMidPoint ||
      header.colName.includes("%")
    )
      return "header-column-cell";
    return "";
  }

  async onScroll(offsetY: number) {
    const viewHeight =
      this.el.nativeElement.getBoundingClientRect().height - this.headerHeight;
    if (
      !this.isLoading &&
      offsetY + viewHeight >= this.dataResult.length * this.rowHeight
    ) {
      if (this.dataResult.length === 0) {
        const pageSize = Math.ceil(viewHeight / this.rowHeight);
        this.myPage = Math.max(pageSize, this.pageLimit);
      }
      await this.getFramework(true);
    }
  }

  async onSort(event: any) {
    this.isAsc = event.newValue !== "desc";
    this.sortColumnId = event.column.prop;
    this.resetTableInfo();

    await this.getFramework(false, true);
  }

  resetTableInfo() {
    this.myPage = 1;
    this.columnsFiltered =
      this.movementTable && this.movementTable.header
        ? this.movementTable.header
            .filter((x) => x.visible && !x.isChecked)
            .map((res) => res.columnId)
        : null;
    this.movementTable = {
      displayName: "",
      displayType: undefined,
      header: this.updateColumns ? [] : this.movementTable.header,
      body: [],
    };
    this.dataResult = [];
  }

  async configureScreen() {
    this.mediaObserver.media$.subscribe((x) => {
      switch (x.mqAlias) {
        case "xl":
          this.tableClass = "ngx-custom-lg-fr";
          break;
        case "md":
          this.tableClass = "ngx-custom-md-ex";
        default:
          this.tableClass = "ngx-custom";
          break;
      }
    });
  }

  changeSelectVisualization(event: IDisplayListTypes) {
    this.displaySelected = event;
  }

  onChangeSearch(event) {
    this.query = event;
    this.termQuery = event;
  }
}
