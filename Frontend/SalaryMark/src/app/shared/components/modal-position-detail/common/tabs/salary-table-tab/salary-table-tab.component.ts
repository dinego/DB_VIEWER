import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { NgxSpinnerService } from "ngx-spinner";
import { CommonService } from "@/shared/services/common/common.service";
import {
  ContractTypeEnum,
  IDefault,
  IUnit,
} from "@/shared/interfaces/positions";
import { HourlyBasisEnum } from "@/shared/models/hourly-basis";
import { copyObject, isUndefined } from "@/shared/common/functions";
import { MediaObserver } from "@angular/flex-layout";
import { PositionDetailsService } from "@/shared/services/position-details/position-details.service";
import {
  Header,
  Table,
  Body,
} from "@/shared/interfaces/salary-table-position-details";
import { TableSalaryPositionDetailColumnEnum } from "@/shared/enum/table-salary-position-detail-column-enum";
import { Modules } from "@/shared/models/modules";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { ConfirmModalEditPositionComponent } from "@/shared/components/confirm-modal-edit-position/confirm-modal-edit-position.component";
import { ToastrService } from "ngx-toastr";
import { RowBody } from "@/shared/models/map-table";
import { ColPosEnum } from "@/shared/enum/modal-position-detail/colPos-enum";
import {
  SalaryTableMapping,
  SalaryTableMappingRequest,
} from "@/shared/models/modal-position-detail/request-update-salary-table";

@Component({
  selector: "app-salary-table-tab",
  templateUrl: "./salary-table-tab.component.html",
  styleUrls: ["./salary-table-tab.component.scss"],
})
export class SalaryTableTabComponent implements OnInit {
  @Input() hoursTypeId: HourlyBasisEnum;
  @Input() contractTypeId: ContractTypeEnum;
  @Input() positionId: number;
  @Input() tableId: number;
  @Input() unitId?: number;
  @Input() modalRef: BsModalRef;
  @Input() moduleId: number;

  @Output() hideModalPosition = new EventEmitter<boolean>();
  @Output() showModalPosition = new EventEmitter();

  public allSalaryTables: IDefault[];
  public allUnits: IUnit[] = [];
  public salaryTable: Table = {
    header: [],
    body: [],
  };
  public page = 1;
  public pageSize = 20;
  public isAsc = true;
  public columHeaders: Header[];
  public hourlyBasisEnum = HourlyBasisEnum;

  public data = [];
  public dataCopy = [];
  public sortClass = "datatable-icon-sort-unset sort-btn";
  public tableSalaryColumnEnum = TableSalaryPositionDetailColumnEnum;
  public tableClass = "";
  public selectedUnit: IUnit;
  public selectedTable: IDefault;
  public editable: boolean;
  public gsmList: IDefault[] = [];
  public sortColumnId: number;

  constructor(
    private positionDetailsService: PositionDetailsService,
    private commonService: CommonService,
    private ngxSpinnerService: NgxSpinnerService,
    private mediaObserver: MediaObserver,
    private _modalService: BsModalService,
    private _toastrService: ToastrService
  ) {}

  async ngOnInit() {
    this.ngxSpinnerService.show();
    await this.configureScreen();
    await this.getAllSalaryTables();
    await this.getAllUnits();
    await this.getSalaryTable();
  }

  async getAllSalaryTables() {
    const tables = await this.commonService.getAllSalaryTables().toPromise();

    this.allSalaryTables = tables.tableSalaryResponses.map((res) => {
      return {
        id: res.id,
        title: res.title,
      } as IDefault;
    });
    this.selectedTable = this.allSalaryTables[0];
    this.tableId = this.tableId ? this.tableId : +this.allSalaryTables[0].id;
  }

  async getAllUnits() {
    const allUnits = await this.commonService
      .getUnitsByFilter(this.tableId)
      .toPromise();
    this.allUnits.push(...allUnits);
    this.unitId = this.unitId ? this.unitId : null;
    this.selectedUnit = this.unitId
      ? this.allUnits.find((u) => u.unitId == this.unitId)
      : allUnits[0];
  }

  changeUnitSelected(unit: IUnit) {
    this.selectedUnit = unit;
    this.getSalaryTable();
  }

  async changeTable(table: IDefault) {
    this.selectedTable = table;
    this.tableId = table ? +table.id : this.tableId;
    await this.getAllUnits();
    this.getSalaryTable();
  }

  async configureScreen() {
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

  async getSalaryTable(scrolled: boolean = false, sort?: boolean) {
    const unitId = this.selectedUnit ? this.selectedUnit.unitId : null;
    this.positionDetailsService
      .getSalaryTable(
        Modules.tableSalary,
        this.positionId,
        this.tableId,
        this.contractTypeId,
        this.hoursTypeId,
        this.page,
        this.pageSize,
        unitId,
        this.isAsc,
        this.sortColumnId
      )
      .subscribe((res) => {
        if (res.table) {
          this.salaryTable.header =
            res.table.body.length > 0 && this.salaryTable.header.length <= 0
              ? res.table.header
              : this.salaryTable.header;

          if (res.table.body.length > 0) {
            const formatResult = res.table.body.map((info) => {
              const tableResult = {};
              info.map((res) => {
                this.salaryTable.header.forEach((item) => {
                  if (item.colPos == res.colPos) {
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
        this.resetData();
        this.ngxSpinnerService.hide();
      });
  }

  getHeaderColumns() {
    this.columHeaders = this.salaryTable.header;
  }

  onSort(item: Header) {
    this.page = 1;
    this.sortColumnId = item.columnId;
    this.isAsc = !item.isDesc;
    item.isDesc = this.isAsc;
    item.sortClass = this.isAsc
      ? "sort-btn datatable-icon-up sort-asc"
      : "sort-btn datatable-icon-down sort-desc";
    this.salaryTable.header.forEach((header) => {
      if (header.columnId !== item.columnId) {
        header.isDesc = false;
        header.sortClass = this.sortClass;
      }
    });

    this.getSalaryTable(false, true);
  }

  public onScrollDown() {
    this.getSalaryTable(true);
  }

  transformText(row: any, header: any) {
    return row[`${header.colPos}`].value;
  }

  getRowResult(row: any, header: any) {
    return row[`${header.colPos}`];
  }

  getIsMidPointResult(row: any, header: any) {
    return row[`${header.colPos}`].isMidPoint;
  }

  getFormatValue(row: any, header: any): string {
    return row[`${header.colPos}`].value.split(",")[0];
  }

  deleteRow(row, index) {
    const itemDeleteOrRemove: any[] = this.dataCopy.find(
      (items) => items[0].value === row[0].value
    );

    // remove line if new (non create yet)
    if (
      itemDeleteOrRemove &&
      itemDeleteOrRemove[0] &&
      itemDeleteOrRemove[0].created
    ) {
      const indexToRemove = this.dataCopy.indexOf(itemDeleteOrRemove);
      this.dataCopy.splice(indexToRemove, 1);
      return;
    }

    // set remove for old lines
    Object.values(this.dataCopy[index]).forEach((element: any) => {
      element.deleted = true;
      element.created = false;
    });
  }

  resetData() {
    this.editable = false;
    const copy = JSON.parse(JSON.stringify(this.data));
    this.dataCopy = copy;
  }

  async setEditable(editable: boolean) {
    this.ngxSpinnerService.show();
    this.editable = editable;

    if (editable) await this.getGsmList();
    this.ngxSpinnerService.hide();
  }

  async getGsmList() {
    this.gsmList = await this.commonService
      .getGsmList(this.tableId, this.unitId)
      .toPromise();
  }

  setGsmInRow(event: IDefault, row: number, colPos: number) {
    const setData: any = Object.values(this.dataCopy[row]);
    setData.find((f: any) => f.colPos === colPos).value = event.title;

    this.tryGetData(row);
  }
  setTableInRow(event: IDefault, row: number, colPos: number) {
    const setData: any = Object.values(this.dataCopy[row]);
    setData.find((f: any) => f.colPos === colPos).value = event.title;

    this.tryGetData(row);
  }
  setUnitInRow(event: IUnit, row: number, colPos: number) {
    const setData: any = Object.values(this.dataCopy[row]);
    setData.find((f: any) => f.colPos === colPos).value = event.unit;
  }

  tryGetData(row: number) {
    const dataVerify: any[] = Object.values(this.dataCopy[row]);
    if (
      dataVerify[ColPosEnum.GSM].created &&
      dataVerify[ColPosEnum.GSM].value &&
      dataVerify[ColPosEnum.Unit].value &&
      dataVerify[ColPosEnum.SalaryTable].value
    ) {
      this.ngxSpinnerService.show();

      const salaryTable = this.allSalaryTables.find(
        (f) => f.title == dataVerify[ColPosEnum.SalaryTable].value
      );

      const unit = this.allUnits.find(
        (f) => f.unit == dataVerify[ColPosEnum.Unit].value
      );

      const gsm = dataVerify[ColPosEnum.GSM].value;

      this.positionDetailsService
        .getSalaryTableByGsm(
          parseInt(salaryTable.id),
          unit.unitId,
          gsm,
          this.contractTypeId,
          this.hoursTypeId
        )
        .subscribe((res) => {
          dataVerify.forEach((element, index) => {
            if (index > 2 && res[index] && res[index].value) {
              element.value = res[index].value;
            }
          });

          this.ngxSpinnerService.hide();
        });
    }
  }

  saveData() {
    this.modalRef.hide();

    const salaryTableMapping: SalaryTableMapping[] = this.getDataToSave();
    const request: SalaryTableMappingRequest = {
      salaryTableMappings: salaryTableMapping,
      moduleId: this.moduleId,
    };

    this.positionDetailsService
      .updateSalaryTableMapping(request)
      .subscribe((res) => {
        this._toastrService.success(res.message);
        this.ngxSpinnerService.hide();
        setTimeout(() => {
          window.location.reload();
        }, 500);
      });
  }

  getDataToSave(): SalaryTableMapping[] {
    const returData: SalaryTableMapping[] = [];
    this.dataCopy.forEach((item) => {
      const dataToPush: SalaryTableMapping = {
        created: item[ColPosEnum.GSM].created
          ? item[ColPosEnum.GSM].created
          : false,
        deleted: item[ColPosEnum.GSM].deleted
          ? item[ColPosEnum.GSM].deleted
          : false,
        gsm: parseInt(item[ColPosEnum.GSM].value),
        tableId: parseInt(
          this.allSalaryTables.find(
            (f) => f.title === item[ColPosEnum.SalaryTable].value
          ).id
        ),
        unitId: this.allUnits.find(
          (unit) => unit.unit === item[ColPosEnum.Unit].value
        ).unitId,
      };

      returData.push(dataToPush);
    });

    return returData;
  }

  openModalConfirm() {
    this.hideModalPosition.emit(true);

    this.modalRef = this._modalService.show(ConfirmModalEditPositionComponent, {
      class: "modal-dialog modal-dialog-centered",
    });

    this.modalRef.content.onSaveEmitter.subscribe((res) => {
      this.saveData();
    });

    this.modalRef.content.onCancelEmitter.subscribe((res) => {
      this.showModalPosition.emit();
    });
  }

  addNewRowTable() {
    const dataMirror = copyObject(this.dataCopy[0]);
    this.eraseData(Object.values(dataMirror));
    this.pushNewLine(dataMirror);
  }

  eraseData(dataMirror: any[]) {
    dataMirror.forEach((f) => {
      f.value = "";
      f.created = true;
      f.deleted = false;
    });
  }

  pushNewLine(dataMirror: any) {
    this.dataCopy.push(dataMirror);
  }
}
