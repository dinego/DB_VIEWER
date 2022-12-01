import { NgxSpinnerService } from "ngx-spinner";

import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
} from "@angular/core";
import { ColumnMode, SelectionType } from "@swimlane/ngx-datatable";

import { SalaryStrategyService } from "@/shared/services/salary-strategy/salary-strategy.service";
import {
  ISalaryStrategyPayload,
  CheckItem,
  IHeader,
} from "@/shared/models/salary-strategy";

import locales from "@/locales/parameters";
import commonLocales from "@/locales/common";
import { SalaryStrategyColumnEnum } from "@/shared/enum/salary-strategy-column-enum";
import { isUndefined } from "@/shared/common/functions";
import { IAlteredCel, ISaveSalaryStrategy } from "./common/body-data";
import { ToastrService } from "ngx-toastr";
import { ISalaryTableResponse, IDefault } from "@/shared/interfaces/positions";
import { CommonService } from "@/shared/services/common/common.service";
import { IPermissions } from "@/shared/models/token";
import { TokenService } from "@/shared/services/token/token.service";

@Component({
  selector: "app-salary-strategy",
  templateUrl: "./salary-strategy.component.html",
  styleUrls: ["./salary-strategy.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SalaryStrategyComponent implements OnInit {
  @ViewChild("myTable") myTable;

  public locales = locales;
  public salaryStrategyList: ISalaryStrategyPayload = {
    body: [],
    header: [],
    nextPage: null,
  };
  public checkInputAll: boolean;
  public checkInputs: Array<CheckItem> = [];
  public isClearFilter: boolean = false;
  public data = [];
  public pageLimit = 20;
  public headerHeight = 50;
  public rowHeight = 46;
  public isLoading: boolean;
  public ColumnMode = ColumnMode;
  public SelectionType = SelectionType;
  public isAsc = true;
  public sortColumnId?: number;
  public commonLocales = commonLocales;
  public selected = [];
  public columHeaders: Array<IHeader>;
  public salaryStrategyColumnEnum = SalaryStrategyColumnEnum;
  public editable: boolean;
  public alteredCel: IAlteredCel[] = [];
  public listTables: ISalaryTableResponse[] = [];
  public tableId: number;
  public projectId: number;
  public tableName: string;
  public isInit = true;
  permissions: IPermissions;

  constructor(
    private salaryStrategyService: SalaryStrategyService,
    private ngxSpinnerService: NgxSpinnerService,
    private ref: ChangeDetectorRef,
    private el: ElementRef,
    private toastrService: ToastrService,
    private commonService: CommonService,
    private tokenService: TokenService
  ) {}

  async ngOnInit() {
    await this.getAllSalaryTable();
    this.getAllSalaryStrategyTable();
    this.permissions = this.tokenService.getPermissions();
  }

  transformText(row: any, header: any) {
    return row[`${header.colPos}`].value;
  }

  getRowResult(row: any, header: any) {
    return row[`${header.colPos}`];
  }

  getFormatValue(row: any, header: any): string {
    return row[`${header.colPos}`].value.split(",")[0];
  }

  resetTableInfo() {
    this.salaryStrategyList = {
      header: [],
      body: [],
      nextPage: null,
    };
    this.data = [];
  }

  private getAllSalaryStrategyTable(sort?: boolean) {
    this.isLoading = !sort ? true : false;
    this.salaryStrategyService
      .getAllSalaryStrategy(this.tableId, this.sortColumnId, this.isAsc)
      .subscribe((res) => {
        if (res.body) {
          this.salaryStrategyList.header =
            res.body.length > 0 ? res.header : this.salaryStrategyList.header;
          if (res.body.length > 0) {
            const formatResult = res.body.map((info) => {
              let tableResult = {};
              info.map((res) => {
                this.salaryStrategyList.header.forEach((item) => {
                  if (item.colPos == res.colPos) {
                    tableResult[`${item.colPos}`] = res;
                  }
                });
              });
              return tableResult;
            });
            this.data = this.convertDataToArray(formatResult);
          }
        } else {
          this.data = [];
        }
        this.getHeaderColumns();
        this.ngxSpinnerService.hide();
        this.isLoading = false;
        this.ref.detectChanges();
      });
  }

  convertDataToArray(rows: any[]): any[] {
    rows.forEach((f, index) => {
      rows[index] = Object.values(rows[index]);
    });

    return rows;
  }

  getHeaderColumns() {
    this.columHeaders = this.salaryStrategyList.header.filter((item) => {
      if (item.visible) {
        if (item.isChecked) {
          return item;
        }
      } else {
        return item;
      }
    });
  }

  onSelect({ selected }) {
    this.selected.splice(0, this.selected.length);
    this.selected.push(...selected);
    this.isClearFilter = this.selected.length > 0;
  }

  onSort(event: any) {
    this.isAsc = event.newValue !== "desc";
    this.isLoading = true;
    this.sortColumnId = event.column.prop;
    this.getAllSalaryStrategyTable(true);
  }

  onClearFilter() {
    this.checkInputAll = false;
    this.isClearFilter = false;
    this.selected = [];
    this.resetTableInfo();
    this.getAllSalaryStrategyTable();
  }

  public onFilterChecked(): void {
    this.data = this.selected;
  }

  public changeItemRowCol(event, rowIndex, col): void {
    const saveRow = Object.values(this.data[rowIndex]);
    const saveCel: any = saveRow.find((f: any) => f.colPos === col);

    const verifyHasItemInMemory: IAlteredCel = this.alteredCel.find((f) => {
      if (f.colPos === col && f.rowPos === rowIndex) return f;
    });

    if (!verifyHasItemInMemory) {
      const insertDataAltered: IAlteredCel = {
        colPos: col,
        rowPos: rowIndex,
        type: saveCel.type,
        value: event,
        trackId: saveCel.trackId,
        groupId: saveCel.groupId,
      };

      this.alteredCel.push(insertDataAltered);
    } else {
      verifyHasItemInMemory.value = event;
    }
  }
  cancelEditableSalaryStrategy() {
    this.alteredCel = [];
    this.editable = false;
  }

  saveSalaryStrategy() {
    if (this.alteredCel) {
      this.ngxSpinnerService.show();
      const dataForUpdate = this.data;
      this.alteredCel.forEach((data) => {
        const alterData: any = Object.values(dataForUpdate[data.rowPos]);

        alterData.find((f: any) => {
          if (f.colPos === data.colPos) return f;
        }).value = data.value;

        if (alterData) this.data[data.rowPos] = Object.assign(alterData);
      });

      this.data = Object.assign(this.data);

      this.updateData();
    }

    this.editable = false;
    this.alteredCel = [];
  }

  updateData() {
    const salaryStrategy: ISaveSalaryStrategy = {
      salaryStrategy: this.alteredCel,
      tableId: this.tableId,
      table: this.tableName,
    };
    this.salaryStrategyService.updateRows(salaryStrategy).subscribe((s) => {
      this.ngxSpinnerService.hide();
      this.toastrService.success(
        locales.saveNewSucessMessage,
        locales.saveNewSucessMessageTitle
      );
      this.listTables.find((t) => t.id == this.tableId.toString()).title =
        this.tableName;
    });
  }

  async getAllSalaryTable() {
    const salaries = await this.commonService.getAllSalaryTables().toPromise();

    this.listTables = salaries.tableSalaryResponses;
    this.tableId = this.listTables.length > 0 ? +this.listTables[0].id : 0;
    this.tableName = this.listTables.length > 0 ? this.listTables[0].title : "";
  }

  changeTable(item: IDefault) {
    if (this.isInit) {
      this.isInit = false;
      return;
    }
    this.tableId = +item?.id;
    this.getAllSalaryStrategyTable();
  }
}
