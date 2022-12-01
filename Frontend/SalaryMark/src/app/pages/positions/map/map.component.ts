import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Clipboard } from "@angular/cdk/clipboard";
import * as fs from "file-saver";

import {
  IDefault,
  IDialogPosition,
  IPositionsResponse,
  IUnit,
  IDisplay,
  IUpdateDisplayColumnsListRequest,
  UpdateColumns,
  DisplayMapPositionEnum,
  ISharePositionsFilter,
  HeaderPositionEnum,
  ContractTypeEnum,
  ISalaryTableResponse,
} from "@/shared/interfaces/positions";
import { MapTableService } from "@/shared/services/map-table/map-table.service";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import { Table, RowBody, Header, Body } from "@/shared/models/map-table";

import locales from "@/locales/positions";
import commonLocales from "@/locales/common";
import routerNames from "@/shared/routerNames";
import { CheckItem, PositionBody } from "@/shared/models/position-table";
import { CommonService } from "@/shared/services/common/common.service";
import { NgxSpinnerService } from "ngx-spinner";
import { SubModules, Modules } from "@/shared/models/modules";
import { IPermissions } from "@/shared/models/token";
import { TokenService } from "@/shared/services/token/token.service";
import { IShareHeader } from "@/shared/models/share-header";
import { environment } from "src/environments/environment";
import { ColumnMode, SelectionType } from "@swimlane/ngx-datatable";
import { MediaObserver } from "@angular/flex-layout";
import { copyObject, isUndefined } from "@/shared/common/functions";
import { DndDropEvent } from "ngx-drag-drop";
declare var $: any;
@Component({
  selector: "app-map",
  templateUrl: "./map.component.html",
  styleUrls: ["./map.component.scss"],
})
export class MapComponent implements OnInit {
  public locales = locales;
  public commonLocales = commonLocales;
  public tableInfo: Table = { header: [], body: [] };
  public isActive: boolean;
  public isClearFilter: boolean;
  public inputModalHelpMap: IDialogInput;
  public inputModalShowMap: IDialogInput;
  public inputShareModalMap: IDialogInput;
  public inputModalPositions: IDialogInput;
  public selectedProfile: IDefault;
  public selectedSalaryTable: ISalaryTableResponse;
  public selectedUnit: IUnit;
  public selectedDisplay: IDisplay;
  public listDisplay: IDisplay[];
  public query: string;
  public filterActive = false;
  public editCols = false;
  public dialogPositionData: IDialogPosition;
  public linkModalShare: string;
  public checkInputAll: boolean;
  public checkInputs: Array<CheckItem> = [];
  public modalHeader: Array<any>;
  public salaryTables: ISalaryTableResponse[];
  public units: IUnit[];
  public email: string;
  public shareURL: string;
  public showJustWithOccupants: boolean = false;
  public hideJustEmpty: boolean = false;
  public profiles: IDefault[];
  public displayColumns: Array<UpdateColumns> = [];
  public termQuery = "";
  public permissions: IPermissions;
  public page = 1;
  public share: boolean;
  public shareHeader: IShareHeader[];
  public shareData: IPositionsResponse;
  public secretKey: string;
  public subModules = SubModules;
  public dataResult;
  public copyDataResult;
  public sortColumnId?: number;
  public selected = [];
  ColumnMode = ColumnMode;
  SelectionType = SelectionType;
  public headerHeight = 50;
  public rowHeight = "auto";
  public pageLimit = 10;
  public isLoading: boolean;
  public isShowColName: boolean;
  public isAsc = true;
  public typePosition: IDefault[] = null;
  public updateColumns = false;
  public columnsFiltered: string[] = [];
  public tableClass = "";
  public editable: boolean;
  public rowsTable: any[] = [];

  constructor(
    private commonService: CommonService,
    private mapTableService: MapTableService,
    private route: Router,
    private ngxSpinnerService: NgxSpinnerService,
    private tokenService: TokenService,
    private router: ActivatedRoute,
    private clipboard: Clipboard,
    private mediaObserver: MediaObserver
  ) {}

  async ngOnInit() {
    this.configureScreen();
    this.inputModalPositions = {
      disableFooter: true,
      idModal: "positionModalMap",
      title: locales.positionsTitle,
      isInfoPosition: true,
    };
    this.secretKey = this.router.snapshot.paramMap.get("secretkey");
    if (this.secretKey) {
      await this.getShareData(false);
      return;
    }
    await this.getAllUnits();
    await this.getAllSalaryTable();
    await this.getAllProfiles();
    this.getDisplayBy();
    this.getAllContractTypes();
    this.permissions = this.tokenService.getPermissions();

    this.inputModalHelpMap = {
      disableFooter: true,
      idModal: "helpModal",
      title: locales.help,
    };

    this.inputModalShowMap = {
      disableFooter: false,
      idModal: "showModalMap",
      title: locales.showConfiguration,
      btnPrimaryTitle: locales.saveAndShow,
      btnSecondaryTitle: locales.show,
      canRenameColumn: this.permissions.canRenameColumns,
      btnWithoutCancel: true,
      isInfoPosition: true,
    };

    this.inputShareModalMap = {
      disableFooter: false,
      idModal: "shareModal",
      title: locales.share,
      btnPrimaryTitle: locales.send,
      btnSecondaryTitle: locales.cancel,
    };
  }

  configureScreen() {
    this.mediaObserver.media$.subscribe((x) => {
      switch (x.mqAlias) {
        case "xl":
          this.tableClass = "ngx-custom-lg-ex";
          break;
        case "md":
          this.tableClass = "ngx-custom-md-ex";
        default:
          this.tableClass = "ngx-custom";
          break;
      }
    });
  }

  public getAllContractTypes() {
    this.commonService.getOccupantList().subscribe((item) => {
      if (item.contractTypesResponse.length > 0) {
        this.typePosition = item.contractTypesResponse;
      }
    });
  }

  changeSelectProfile(item: IDefault) {
    this.selectedProfile = item;
    this.resetTableInfo();
    this.getAllMapPositions(1, false);
  }
  async changeSelectSalaryTable(item: ISalaryTableResponse) {
    this.selectedSalaryTable = item;
    this.resetTableInfo();
    await this.cascateFilterByTable();
    this.getAllMapPositions(1, false);
  }

  displaySelectedEvent(item: IDisplay) {
    this.selectedDisplay = item;
    this.resetTableInfo();
    this.getAllMapPositions(1, false);
  }

  showFilter() {
    const hasItem = this.checkInputs.filter(
      (item: CheckItem) => item.checked
    ).length;
    if (hasItem > 0) {
      this.isClearFilter = true;
      this.checkInputAll = hasItem !== this.checkInputs.length ? false : true;
    } else {
      this.checkInputAll = false;
      this.isClearFilter = false;
    }
  }

  public async getAllProfiles() {
    const tableId =
      this.selectedSalaryTable && this.selectedSalaryTable.id !== "0"
        ? this.selectedSalaryTable.id
        : "";
    const unitId = this.selectedUnit ? this.selectedUnit.unitId : null;
    const item = await this.commonService
      .getAllProfiles(+tableId, unitId)
      .toPromise();
    if (item.profilesResponse.length > 0) {
      this.profiles = item.profilesResponse;
    }
  }

  public async getAllSalaryTable() {
    const item = await this.commonService
      .getAllSalaryTables(
        this.selectedUnit ? this.selectedUnit.unitId.toString() : ""
      )
      .toPromise();
    if (item.tableSalaryResponses.length > 0) {
      this.salaryTables = item.tableSalaryResponses;
    }
  }

  public async getAllUnits() {
    this.units = await this.commonService.getAllUnits().toPromise();
  }

  public getAllMapPositions(page?: number, sort?: boolean) {
    this.isLoading = !sort ? true : false;
    this.ngxSpinnerService.show();
    const display = this.selectedDisplay
      ? this.selectedDisplay.id
      : DisplayMapPositionEnum.AxisCarreira.toString();
    this.mapTableService
      .getMapPositionListTable(
        display,
        this.termQuery,
        this.selectedSalaryTable && +this.selectedSalaryTable.id > 0
          ? this.selectedSalaryTable.id
          : "",
        this.selectedUnit && this.selectedUnit.unitId > 0
          ? this.selectedUnit.unitId.toString()
          : "",
        this.selectedProfile && +this.selectedProfile.id > 0
          ? this.selectedProfile.id
          : "",
        this.hideJustEmpty,
        this.showJustWithOccupants,
        this.pageLimit,
        page,
        this.isAsc,
        this.columnsFiltered
      )
      .subscribe((res) => {
        if (res.table) {
          this.tableInfo.header =
            res.table.body.length > 0 && this.tableInfo.header.length <= 0
              ? res.table.header
              : this.tableInfo.header;
          localStorage.setItem(
            "map-headers",
            JSON.stringify(this.tableInfo.header)
          );
          if (res.table.body.length > 0) {
            const formatResult = res.table.body.map((info) => {
              let tableResult = {};
              info.map((res) => {
                this.tableInfo.header.forEach((item) => {
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

            this.copyDataResult = copyObject(this.dataResult);

            this.convertBodyToDragData();
          }
        } else {
          this.dataResult = [];
        }
        this.page = res.nextPage;
        this.ngxSpinnerService.hide();
        this.isLoading = false;
      });
  }

  getFileSpreadsheet() {
    this.ngxSpinnerService.show();
    const display = this.selectedDisplay
      ? this.selectedDisplay.id
      : DisplayMapPositionEnum.AxisCarreira.toString();
    this.mapTableService
      .getExportExcel(
        display,
        this.termQuery,
        this.selectedSalaryTable && +this.selectedSalaryTable.id > 0
          ? this.selectedSalaryTable.id
          : "",
        this.selectedUnit && this.selectedUnit.unitId > 0
          ? this.selectedUnit.unitId.toString()
          : "",
        this.selectedProfile && +this.selectedProfile.id > 0
          ? this.selectedProfile.id
          : "",
        this.hideJustEmpty,
        this.showJustWithOccupants,
        this.isAsc,
        this.columnsFiltered
      )
      .subscribe((item) => {
        const file = new Blob([item], {
          type: item.type,
        });
        const blob = window.URL.createObjectURL(file);
        fs.saveAs(blob, "MapaCargos.xlsx");
        this.ngxSpinnerService.hide();
      });
  }

  public getShareKey(): void {
    this.commonService
      .getShareKey({
        moduleId: Modules.positions,
        moduleSubItemId: SubModules.positionMap,
        columnsExcluded: [
          this.tableInfo.header.filter((item) => (item.visible = false)),
        ],
        parameters: {
          showJustWithOccupants: this.showJustWithOccupants,
          removeRowsEmpty: this.hideJustEmpty,
          tableId:
            this.selectedSalaryTable && +this.selectedSalaryTable.id > 0
              ? this.selectedSalaryTable.id
              : "",
          groupId:
            this.selectedProfile && +this.selectedProfile.id > 0
              ? this.selectedProfile.id
              : "",
          unitId:
            this.selectedUnit && this.selectedUnit.unitId > 0
              ? this.selectedUnit.unitId.toString()
              : "",
          displayBy: this.selectedDisplay.id,
          permissions: this.permissions,
          isAsc: this.isAsc.toString(),
        },
      })
      .subscribe((key) => {
        this.shareURL = `${environment.baseUrl}${routerNames.POSITIONS.BASE}/${routerNames.POSITIONS.SETTINGS}/compartilhar-mapa-cargos/${key}`;
        this.clipboard.copy(this.shareURL);
        this.ngxSpinnerService.hide();
      });
  }

  public getDisplayBy(): void {
    this.commonService.getDisplayBy().subscribe((item) => {
      this.listDisplay = item;
      this.selectedDisplay = item[0];
    });
  }

  navigation(patch: string) {
    this.route.navigate([`/${routerNames.POSITIONS.BASE}/`, patch]);
  }

  async onOccupantsSwitch(isChecked: boolean) {
    this.showJustWithOccupants = isChecked;
    this.resetTableInfo();
    if (this.secretKey) {
      await this.getShareData(false);
    } else {
      this.getAllMapPositions(1, false);
    }
  }

  async onHideEmptySwitch(isChecked: boolean) {
    this.hideJustEmpty = isChecked;
    this.resetTableInfo();
    if (this.secretKey) {
      await this.getShareData(false);
    } else {
      this.getAllMapPositions(1, false);
    }
  }

  public onChangeSearch(eventQuery: string) {
    if (eventQuery.length >= 3 || eventQuery.length == 0) {
      this.query = eventQuery;
      this.termQuery = this.query;
      this.resetTableInfo();
      this.getAllMapPositions(1, false);
    }
  }

  public async cascateFiltersByUnit() {
    await this.getAllSalaryTable();
    if (
      this.selectedSalaryTable &&
      this.selectedSalaryTable.id !== "0" &&
      !this.profiles.some((x) => x.id === this.selectedSalaryTable.id)
    ) {
      this.selectedSalaryTable = null;
    } else if (this.salaryTables && this.salaryTables.length === 1) {
      this.selectedSalaryTable = this.salaryTables[0];
    }
    this.cascateFilterByTable();
  }

  public async cascateFilterByTable() {
    await this.getAllProfiles();
    if (
      (this.selectedSalaryTable && this.selectedSalaryTable.id === "0") ||
      (this.selectedProfile &&
        this.selectedProfile.id !== "0" &&
        !this.profiles.some((x) => x.id === this.selectedProfile.id))
    ) {
      this.selectedProfile = null;
    } else if (this.profiles && this.profiles.length === 1) {
      this.selectedProfile = this.profiles[0];
    }
  }

  public async changeSelectUnit(item: IUnit) {
    this.selectedUnit = item;
    this.resetTableInfo();
    await this.cascateFiltersByUnit();
    this.getAllMapPositions(1, false);
  }

  onClearFilter() {
    this.checkInputAll = false;
    this.isClearFilter = false;
    this.getAllMapPositions(1, false);
  }

  onEditCols() {
    this.editCols = true;
  }

  public onFilterChecked(): void {
    this.dataResult = this.selected;
  }

  changeHeaderShowCols(header: Array<Header>) {
    this.modalHeader = header;
  }

  public onPutEmail(event) {
    this.email = event;
  }

  public onRestoreCols(modalChildren) {
    this.editCols = false;
    modalChildren.forEach((mdItem, index) => {
      mdItem.isChecked = true;
      mdItem.value = this.tableInfo.header[index].colName;
      this.tableInfo.header[index].nickName = mdItem.value;
    });
  }

  public onShowChanges(modalChildren, isEdit: boolean = false) {
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
    this.updateColumns = isEdit;
  }

  public saveAndShowCols(modalChildren) {
    this.onShowChanges(modalChildren, true);
    if (!this.displayColumns || this.displayColumns.length <= 0) return;
    const request: IUpdateDisplayColumnsListRequest = {
      displayColumns: this.displayColumns,
    };
    this.mapTableService.setHeaderMapChecked(request).subscribe(
      (res) => {
        this.getAllMapPositions();
      },
      (err) => {}
    );
  }
  onLineClick(positionSMId: number) {
    this.dialogPositionData = null;
    this.mapTableService.getFullInfoPosition(positionSMId).subscribe((item) => {
      this.dialogPositionData = item;
      $("#positionModalMap").modal("show");
      this.ngxSpinnerService.hide();
    });
  }

  onGenerateLink() {
    //this.linkModalShare = this.mapTableService.getLinkShare();
  }

  public onSendEmail() {
    this.commonService
      .shareLink({
        to: this.email,
        url: this.shareURL,
      })
      .subscribe((_response) => this.ngxSpinnerService.hide());
  }

  async onScrollDown(event) {
    const nextPage = event.offset + 1;
    if (!this.isLoading && nextPage === this.page) {
      if (this.secretKey) {
        await this.getShareData(false);
      } else {
        this.getAllMapPositions(this.page, false);
      }
    }
  }

  public trackItem(index: number, item: RowBody) {
    return item.colPos;
  }

  public trackItemModal(index: number, item: Header) {
    return item.colPos;
  }

  resetTableInfo() {
    this.page = 1;
    this.columnsFiltered =
      this.tableInfo && this.tableInfo.header
        ? this.tableInfo.header
            .filter((x) => x.visible && !x.isChecked)
            .map((x) => x.columnId.toString())
        : null;
    this.tableInfo = {
      header: this.updateColumns ? [] : this.tableInfo.header,
      body: [],
    };
    this.dataResult = [];
  }

  async getShareData(sort?: boolean) {
    this.share = true;
    this.isLoading = !sort ? true : false;
    this.shareData = await this.mapTableService
      .getMapPositionsShared(
        this.secretKey,
        this.hideJustEmpty,
        this.showJustWithOccupants,
        this.page,
        this.sortColumnId,
        this.isAsc
      )
      .toPromise();
    if (this.shareData) {
      if (this.shareData.table) {
        this.tableInfo.header =
          this.shareData.table.body.length > 0
            ? this.shareData.table.header
            : this.tableInfo.header;

        if (this.shareData.table.body.length > 0) {
          const formatResult = this.shareData.table.body.map((info) => {
            let tableResult = {};
            info.map((res) => {
              this.tableInfo.header.forEach((item) => {
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
      } else {
        this.dataResult = [];
      }

      if (this.shareData.share) {
        this.termQuery,
          (this.selectedSalaryTable = {
            id: this.shareData.share.tableId?.toString(),
            title: "",
            projectId: 0,
          }),
          (this.selectedProfile = {
            id: this.shareData.share.groupId?.toString(),
            title: "",
          });
        this.hideJustEmpty = false;
        this.showJustWithOccupants = this.shareData.share.isWithOccupants;
        this.selectedUnit = { unitId: this.shareData.share.unitId, unit: "" };
        this.selectedDisplay = {
          id: this.shareData.share.displayById
            ? this.shareData.share.displayById?.toString()
            : DisplayMapPositionEnum.AxisCarreira.toString(),
          name: "",
        };
        this.permissions = this.shareData.share.permissions;
        this.shareHeader = this.prepareShareHeader(this.shareData.share);
      }
      this.page = this.shareData.nextPage;
      this.isLoading = false;
      this.ngxSpinnerService.hide();
    }
  }

  prepareShareHeader(shareData: ISharePositionsFilter): IShareHeader[] {
    const userArray = shareData.user.split(" ");
    return [
      {
        label: "Nome",
        value: userArray.length > 0 ? userArray[0] : "-",
        type: "string",
      },
      {
        label: "Data",
        value: shareData.date,
        type: "date",
      },
      {
        label: "Perfil",
        value: shareData.group,
        type: "string",
      },
      {
        label: "Tabela",
        value: shareData.table,
        type: "string",
      },
      {
        label: "Unidade",
        value: shareData.unit,
        type: "string",
      },
    ];
  }
  canAccess(subModule: SubModules) {
    return this.tokenService.validateModules(Modules.positions, subModule);
  }

  getHeaderColumns() {
    return this.tableInfo.header.filter((item) => {
      if (item.visible) {
        if (item.isChecked) {
          return item;
        }
      } else {
        return item;
      }
    });
  }

  onSort(event: any) {
    this.isAsc = event.newValue !== "desc";
    this.page = 1;
    this.sortColumnId = event.column.prop;
    if (this.secretKey) {
      this.getShareData(true);
    } else {
      this.getAllMapPositions(this.page, true);
    }
  }

  transformGSMText(row: any, header: any) {
    return row[`${header.colPos}`] &&
      row[`${header.colPos}`].positions.length > 0
      ? row[`${header.colPos}`].positions[0].value
      : "";
  }

  getRowResult(row: any, header: any) {
    return row[`${header.colPos}`];
  }

  getPositions(row: any, header: any) {
    return row[`${header.colPos}`] &&
      row[`${header.colPos}`].positions.length > 0
      ? row[`${header.colPos}`].positions
      : [];
  }

  onSelect({ selected }) {
    this.selected.splice(0, this.selected.length);
    this.selected.push(...selected);
    this.isClearFilter = this.selected.length > 0;
  }
  getRowHeight(row) {
    const values = [2];
    let height = 0;
    let headers: Header[] = JSON.parse(localStorage.getItem("map-headers"));
    headers.forEach((header) => {
      let count = 0;
      if (
        height <= 0 &&
        row[header.colPos] &&
        row[header.colPos].type !== "Int32" &&
        row[header.colPos].positions.length >= 1
      ) {
        const headersToCompare = headers.filter(
          (x) => x.colPos != header.colPos && x.colPos != 0
        );
        headersToCompare.forEach((headerCompare) => {
          if (
            row[headerCompare.colPos] &&
            row[headerCompare.colPos].type !== "Int32" &&
            row[header.colPos].positions.length >
              row[headerCompare.colPos].positions.length
          ) {
            count++;
          }
        });
        height = Math.floor(row[header.colPos].positions.length * 38);
      }
    });

    return height <= 0 ? 58 : height;
  }

  getFirstBlockPositionInfo() {
    const headers = [
      HeaderPositionEnum.localId,
      HeaderPositionEnum.gsm,
      HeaderPositionEnum.hourBase,
    ];
    return this.dialogPositionData && this.dialogPositionData.headerPosition
      ? this.dialogPositionData.headerPosition
          .filter((x) => headers.includes(x.propertyId) && x.visible)
          .map((res) => {
            switch (res.propertyId) {
              case HeaderPositionEnum.localId:
                res.colPos = 0;
                break;
              case HeaderPositionEnum.gsm:
                res.colPos = 1;
                break;
              case HeaderPositionEnum.hourBase:
                res.colPos = 2;
                break;
            }
            return res;
          })
          .sort((obj1, obj2) => {
            if (obj1.colPos > obj2.colPos) {
              return 1;
            }
            if (obj1.colPos < obj2.colPos) {
              return -1;
            }
            return 0;
          })
      : [];
  }
  getSecondBlockPositionInfo() {
    const headers = [
      HeaderPositionEnum.positionSalaryMark,
      HeaderPositionEnum.level,
      HeaderPositionEnum.profile,
    ];
    return this.dialogPositionData && this.dialogPositionData.headerPosition
      ? this.dialogPositionData.headerPosition
          .filter((x) => headers.includes(x.propertyId) && x.visible)
          .map((res) => {
            switch (res.propertyId) {
              case HeaderPositionEnum.positionSalaryMark:
                res.colPos = 0;
                break;
              case HeaderPositionEnum.level:
                res.colPos = 1;
                break;
              case HeaderPositionEnum.profile:
                res.colPos = 2;
                break;
            }
            return res;
          })
          .sort((obj1, obj2) => {
            if (obj1.colPos > obj2.colPos) {
              return 1;
            }
            if (obj1.colPos < obj2.colPos) {
              return -1;
            }
            return 0;
          })
      : [];
  }
  getThirdBlockPositionInfo() {
    const headers = [
      HeaderPositionEnum.smCode,
      HeaderPositionEnum.axisCareer,
      HeaderPositionEnum.parameter02,
    ];
    return this.dialogPositionData && this.dialogPositionData.headerPosition
      ? this.dialogPositionData.headerPosition
          .filter((x) => headers.includes(x.propertyId) && x.visible)
          .map((res) => {
            switch (res.propertyId) {
              case HeaderPositionEnum.smCode:
                res.colPos = 0;
                break;
              case HeaderPositionEnum.axisCareer:
                res.colPos = 1;
                break;
              case HeaderPositionEnum.parameter02:
                res.colPos = 2;
                break;
            }
            return res;
          })
          .sort((obj1, obj2) => {
            if (obj1.colPos > obj2.colPos) {
              return 1;
            }
            if (obj1.colPos < obj2.colPos) {
              return -1;
            }
            return 0;
          })
      : [];
  }
  changeUnitLabel(items: IUnit[]) {
    if (!items) return ``;
    return items.length > 1 ? this.locales.allA : items[0].unit;
  }
  showOccupantPJIcon() {
    if (this.share && this.shareData && this.shareData.share) {
      return ContractTypeEnum.PJ === this.shareData.share.contractTypeId;
    }
    return (
      this.typePosition &&
      this.typePosition.some((x) => +x.id === ContractTypeEnum.PJ)
    );
  }
  showOccupantCLTIcon() {
    if (this.share && this.shareData && this.shareData.share) {
      const contractTypes = [
        ContractTypeEnum.CLT,
        ContractTypeEnum.CLT_Executive,
        ContractTypeEnum.CLT_Flex,
      ];
      return contractTypes.some(
        (ctr) => ctr === this.shareData.share.contractTypeId
      );
    }
    return (
      this.typePosition &&
      this.typePosition.some(
        (x) =>
          +x.id === ContractTypeEnum.CLT ||
          +x.id === ContractTypeEnum.CLT_Executive ||
          +x.id === ContractTypeEnum.CLT_Flex
      )
    );
  }
  isCLT(position: any) {
    return (
      position &&
      position.tooltips &&
      position.tooltips.some((x) => x.occupantCLT)
    );
  }
  isPJ(position: any) {
    return (
      position &&
      position.tooltips &&
      position.tooltips.some((x) => x.occupantPJ)
    );
  }

  enableDrags() {
    this.editable = !this.editable;

    this.setDragPropertyObjects();
  }

  setDragPropertyObjects() {
    const asArray = Object.values(this.copyDataResult);
    asArray.forEach((element) => {
      const innerObj = Object.values(element);

      innerObj.forEach((inner) => {
        if (
          inner.type === "String" &&
          Object.values(inner.positions).length > 0
        ) {
          inner;
        }
      });
    });
  }

  async convertBodyToDragData() {
    const arrayInit = Object.values(this.dataResult);

    arrayInit.forEach((f) => {
      const arrayInner = Object.values(f);

      let row: any = [];

      arrayInner.forEach((inner) => {
        const objPush = {
          colPos: inner.colPos,
          data: inner.positions,
          type: inner.type,
          effectAllowed: "all",
          disable: false,
          handle: false,
        };

        row.push(objPush);
      });

      this.rowsTable.push(row);
    });
  }

  restoreDrags() {
    this.dataResult = this.copyDataResult;
    this.convertBodyToDragData();
  }

  saveDrag(event) {
    this.rowsTable = event;
    this.editable = false;
  }

  cancelDrag() {
    this.restoreDrags();
    this.editable = false;
  }
}
