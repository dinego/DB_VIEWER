import {
  Component,
  ElementRef,
  OnInit,
  ViewEncapsulation,
} from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NgxSpinnerService } from "ngx-spinner";
import { Clipboard } from "@angular/cdk/clipboard";
import * as fs from "file-saver";

import { Table, Header, RowBody } from "@/shared/models/position-table";
import { PositionListService } from "@/shared/services/position-table/position-table.service";
import {
  IDefault,
  IUpdateDisplayColumnsListRequest,
  IUnit,
  UpdateColumns,
  ContractTypeEnum,
  IPositionsResponse,
  ISharePositionsFilter,
  IDialogPosition,
  HeaderPositionEnum,
  HeaderApplyStyleTypeEnum,
} from "@/shared/interfaces/positions";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import routerNames from "@/shared/routerNames";
import { CommonService } from "@/shared/services/common/common.service";

import locales from "@/locales/positions";
import commonLocales from "@/locales/common";
import { Observable } from "rxjs";
import { Modules, SubModules } from "@/shared/models/modules";
import { IPermissions } from "@/shared/models/token";
import { TokenService } from "@/shared/services/token/token.service";
import { HourlyBasisEnum } from "@/shared/models/hourly-basis";
import { IShareHeader } from "@/shared/models/share-header";
import { environment } from "src/environments/environment";
import { ColumnMode, SelectionType } from "@swimlane/ngx-datatable";
import { MapTableService } from "@/shared/services/map-table/map-table.service";
import { PositionProjectColumnsEnum } from "@/shared/enum/position-project-columns-enum";
import { MediaObserver } from "@angular/flex-layout";
import { isUndefined } from "@/shared/common/functions";
import { BsModalRef, BsModalService, ModalOptions } from "ngx-bootstrap/modal";
import { ModalAddPositionComponent } from "@/shared/components/modal-add-position/modal-add-position.component";
import { ModalPositionDetailComponent } from "@/shared/components/modal-position-detail/modal-position-detail.component";
declare var $: any;
@Component({
  selector: "app-list",
  templateUrl: "./list.component.html",
  styleUrls: ["./list.component.scss"],
  encapsulation: ViewEncapsulation.None,
})
export class ListComponent implements OnInit {
  public assetsPath = "../../assets/imgs/svg/";
  public commonLocales = commonLocales;
  public page = 1;
  public editCols = false;
  public inputModalHelp: IDialogInput;
  public inputModalShow: IDialogInput;
  public inputShareModal: IDialogInput;
  public inputModalPositions: IDialogInput;
  public dialogPositionData: IDialogPosition;
  public isClearFilter = false;
  public listTables: IDefault[];
  public locales = locales;
  public modalHeader: Array<any>;
  public period: IDefault[] = null;
  public query: string;
  public selectedPeriod: IDefault;
  public selectedTypePosition: IDefault;
  public selectedUnit: IUnit;
  public shareURL: string;
  public tableInfo: Table = { header: [], body: [] };
  public typePosition: IDefault[] = null;
  public filteredOptions: { [key: string]: Observable<string[]> };
  public email: string;
  public units: IUnit[];
  public showJustWithOccupants = false;
  public selectedTable: IDefault;
  public displayColumns: Array<UpdateColumns> = [];
  public termQuery = "";
  public permissions: IPermissions;
  public share: boolean;
  public shareHeader: IShareHeader[];
  public shareData: IPositionsResponse;
  public secretKey: string;
  public dataResult;
  public sortColumnId?: number;
  public selected = [];
  ColumnMode = ColumnMode;
  SelectionType = SelectionType;
  public headerHeight = 50;
  public rowHeight = 46;
  public pageLimit = 20;
  public isLoading: boolean;
  public isShowColName: boolean;
  public isAsc = true;
  public subModules = SubModules;
  public hourlyBasisEnum = HourlyBasisEnum;
  public hoursType = HourlyBasisEnum.MonthSalary;
  public listColumnEnum = PositionProjectColumnsEnum;
  public updateColumns = false;
  public cellClass = "header-column-cell";
  public modalRows = [];
  public columns = [
    { prop: "Unidade" },
    { prop: "Unidade" },
    { prop: "Unidade" },
  ];
  public columnsFiltered: number[] = [];
  public tableClass = "";
  public routerNames = routerNames;
  public bsModalRef?: BsModalRef;
  public profiles: IDefault[] = [];
  public selectedProfile: IDefault;

  public listProfiles: IDefault[] = [];

  constructor(
    private commonService: CommonService,
    private positionListService: PositionListService,
    private route: Router,
    private ngxSpinnerService: NgxSpinnerService,
    private tokenService: TokenService,
    private activeRouter: ActivatedRoute,
    private router: Router,
    private clipboard: Clipboard,
    private el: ElementRef,
    private mediaObserver: MediaObserver,
    private modalService: BsModalService
  ) {}

  async ngOnInit(): Promise<void> {
    if (this.router.url.split("/")[4]) {
      this.share = true;
    }

    this.configureScreen();
    this.inputModalPositions = {
      disableFooter: true,
      idModal: "listPositionModal",
      title: locales.positionsTitle,
      isInfoPosition: true,
    };
    this.secretKey = this.activeRouter.snapshot.paramMap.get("secretkey");
    if (this.secretKey) {
      this.getShareData();
      return;
    }
    await this.getTableList();
    await this.getAllUnits();
    this.getAllContractTypes();
    this.getAllHoursBase();
    this.getAllProfiles();
    this.permissions = this.tokenService.getPermissions();

    this.inputModalHelp = {
      disableFooter: true,
      idModal: "helpModal",
      title: locales.help,
    };

    this.inputModalShow = {
      disableFooter: false,
      idModal: "showModal",
      title: locales.showConfiguration,
      btnPrimaryTitle: locales.saveAndShow,
      btnSecondaryTitle: locales.show,
      canRenameColumn: this.permissions.canRenameColumns,
      btnWithoutCancel: true,
      isInfoPosition: true,
    };

    this.inputShareModal = {
      disableFooter: false,
      idModal: "shareModal",
      title: locales.share,
      btnPrimaryTitle: locales.send,
      btnSecondaryTitle: locales.cancel,
    };
  }

  public async getAllProfiles() {
    this.profiles.push({
      id: null,
      title: "Todos",
    });

    const item = await this.commonService
      .getAllProfiles(null, null)
      .toPromise();
    if (item.profilesResponse.length > 0) {
      this.profiles.push(...item.profilesResponse);
    }

    this.selectedProfile = this.profiles ? this.profiles[0] : null;
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

  getShareData(scrolled: boolean = false, sort?: boolean) {
    this.isLoading = !sort ? true : false;
    this.share = true;
    this.positionListService
      .getPositionsShared(
        this.secretKey,
        this.showJustWithOccupants,
        this.page,
        this.sortColumnId,
        this.isAsc
      )
      .subscribe((shareData) => {
        this.shareData = shareData;
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
                this.dataResult && !sort
                  ? [...this.dataResult, ...formatResult]
                  : formatResult;
              this.dataResult = rows;
            }
          } else if (!scrolled && shareData.nextPage === 0) {
            this.dataResult = [];
          }
          if (this.shareData.share) {
            this.selectedTypePosition = {
              id: this.shareData.share.contractTypeId.toString(),
              title: "",
            };
            this.showJustWithOccupants = this.shareData.share.isWithOccupants;
            this.selectedPeriod = {
              id: this.shareData.share.hoursTypeId.toString(),
              title: "",
            };
            this.selectedTable = {
              id: this.shareData.share.tableId.toString(),
              title: "",
            };
            this.selectedUnit = {
              unitId: this.shareData.share.unitId,
              unit: "",
            };
            this.permissions = this.shareData.share.permissions;
            this.shareHeader = this.prepareShareHeader(this.shareData.share);
          }
          this.ngxSpinnerService.hide();
          this.isLoading = false;
          this.page = this.shareData.nextPage;
        }
      });
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
        label: "Mostrar Como",
        value: shareData.hoursType,
        type: "string",
      },
      {
        label: "Tipo de Contrato",
        value: shareData.contractType,
        type: "string",
      },
      {
        label: "Unidade",
        value: shareData.unit,
        type: "string",
      },
      {
        label: "Perfil",
        value: shareData.group,
        type: "string",
      },
    ];
  }

  public getAllPositions(
    scrolled: boolean = false,
    sort?: boolean,
    filterSearch?: any
  ) {
    this.isLoading = !sort ? true : false;
    this.ngxSpinnerService.show();
    const selectedColumSearch =
      filterSearch && filterSearch.selected ? filterSearch.selected.id : null;
    const searchInColumn =
      filterSearch && filterSearch.value ? filterSearch.value : null;

    const typeContract = this.selectedTypePosition
      ? this.selectedTypePosition.id
      : ContractTypeEnum.CLT;
    const hourlyBasis = this.selectedPeriod
      ? this.selectedPeriod.id
      : HourlyBasisEnum.MonthSalary;
    this.hoursType = +hourlyBasis;
    this.positionListService
      .getPositionListTable(
        this.selectedTable.id,
        this.showJustWithOccupants,
        typeContract.toString(),
        hourlyBasis.toString(),
        this.termQuery,
        this.selectedUnit ? this.selectedUnit.unitId : null,
        this.selectedProfile ? parseInt(this.selectedProfile.id) : null,
        this.sortColumnId,
        this.page,
        this.pageLimit,
        this.isAsc,
        this.columnsFiltered,
        selectedColumSearch,
        searchInColumn
      )
      .subscribe((res) => {
        if (res.table) {
          this.tableInfo.header = [
            ...(res.table.body.length > 0 && this.tableInfo.header.length <= 0
              ? res.table.header
              : this.tableInfo.header),
          ];

          this.setDropSearchItems(this.tableInfo.header);

          if (res.table.body.length > 0) {
            this.dataResult = this.termQuery ? [] : this.dataResult;
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
          }
        } else if (!scrolled && res.nextPage === 0) {
          this.dataResult = [];
        }
        this.page = res.nextPage;
        this.ngxSpinnerService.hide();
        this.isLoading = false;
      });
  }

  setDropSearchItems(items: Header[]) {
    const itemsFilter = items.filter((f) => f.visible);
    this.listProfiles = [
      {
        id: "",
        title: "Todos os campos",
      },
    ];
    const itemsPush = itemsFilter.map((m) => {
      return {
        id: m.columnId.toString(),
        title: m.colName,
      } as IDefault;
    });

    this.listProfiles.push(...itemsPush);
  }

  transformText(row: any, header: any) {
    return row[`${header.colPos}`].value;
  }

  getRowResult(row: any, header: any) {
    return row[`${header.colPos}`];
  }

  getTooltip(row: any, header: Header) {
    return row[`${header.colPos}`].tooltips;
  }

  isCLT(row: any, header: Header) {
    return row[`${header.colPos}`].occupantCLT && this.showOccupantCLTIcon();
  }

  isPJ(row: any, header: Header) {
    return row[`${header.colPos}`].occupantPJ && this.showOccupantPJIcon();
  }

  getFormatValue(row: any, header: any): string {
    return row[`${header.colPos}`].value.split(",")[0];
  }

  onSelect({ selected }) {
    this.selected.splice(0, this.selected.length);
    this.selected.push(...selected);
    this.isClearFilter = this.selected.length > 0;
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
    this.isLoading = true;
    this.page = 1;
    this.sortColumnId = event.column.prop;
    if (this.secretKey) {
      this.getShareData(false, true);
    } else {
      this.getAllPositions(false, true);
    }
  }

  resetTableInfo() {
    this.page = 1;
    this.columnsFiltered =
      this.tableInfo && this.tableInfo.header
        ? this.tableInfo.header
            .filter((x) => x.visible && !x.isChecked)
            .map((x) => x.columnId)
        : null;
    this.tableInfo = {
      header: this.updateColumns ? [] : this.tableInfo.header,
      body: [],
    };
    this.dataResult = [];
  }

  public changeHeaderShowCols(header: Array<Header>): void {
    this.modalHeader = header;
  }

  public changeSelectPeriod(item: IDefault): void {
    this.selectedPeriod = item;
    this.resetTableInfo();
    this.getAllPositions();
  }

  public changeSelectTypePosition(item: IDefault): void {
    this.selectedTypePosition = item;
    this.resetTableInfo();
    this.getAllPositions();
  }

  public changeSelectUnit(item: IUnit): void {
    this.selectedUnit = item;
    this.resetTableInfo();
    this.getAllPositions();
  }

  public changeSelectProfile(item: IDefault): void {
    this.selectedProfile = item;
    this.resetTableInfo();
    this.getAllPositions();
  }

  public getAllHoursBase(): void {
    this.commonService.getHourlyBasis().subscribe((item) => {
      if (item.hoursBaseResponse.length > 0) {
        this.period = item.hoursBaseResponse;
        this.selectedPeriod = this.period[0];
      }
    });
  }

  public getAllContractTypes() {
    this.commonService.getOccupantList().subscribe((item) => {
      if (item.contractTypesResponse.length > 0) {
        this.typePosition = item.contractTypesResponse;
        this.selectedTypePosition = this.typePosition[0];
      }
    });
  }

  public async getAllUnits() {
    this.units = await this.commonService
      .getUnitsByFilter(+this.selectedTable.id)
      .toPromise();
  }

  getFileSpreadsheet() {
    const typeContract = this.selectedTypePosition
      ? this.selectedTypePosition.id
      : ContractTypeEnum.CLT;
    const hourlyBasis = this.selectedPeriod
      ? this.selectedPeriod.id
      : HourlyBasisEnum.MonthSalary;
    this.hoursType = +hourlyBasis;
    this.positionListService
      .getExportExcel(
        this.selectedTable.id,
        this.showJustWithOccupants,
        typeContract.toString(),
        hourlyBasis.toString(),
        this.termQuery,
        this.selectedUnit ? this.selectedUnit.unitId : null,
        this.sortColumnId,
        this.isAsc,
        this.columnsFiltered
      )
      .subscribe((item) => {
        const file = new Blob([item], {
          type: item.type,
        });
        const blob = window.URL.createObjectURL(file);
        fs.saveAs(blob, `${locales.list}.xlsx`);
        this.ngxSpinnerService.hide();
      });
  }

  public getShareKey(): void {
    this.commonService
      .getShareKey({
        moduleId: Modules.positions,
        moduleSubItemId: SubModules.positionList,
        columnsExcluded: [
          this.tableInfo.header.filter((item) => (item.visible = false)),
        ],
        parameters: {
          tableId: this.selectedTable.id,
          showJustWithOccupants: this.showJustWithOccupants,
          contractType: this.selectedTypePosition.id,
          hoursType: this.selectedPeriod.id,
          unitId: this.selectedUnit ? this.selectedUnit.unitId : null,
          permissions: this.permissions,
        },
      })
      .subscribe((key) => {
        this.shareURL = `${environment.baseUrl}${routerNames.POSITIONS.BASE}/${routerNames.POSITIONS.SETTINGS}/compartilhar-arquitetura-cargos/${key}`;
        this.clipboard.copy(this.shareURL);
        this.ngxSpinnerService.hide();
      });
  }

  public async getTableList() {
    const tables = await this.commonService.getAllSalaryTables().toPromise();
    if (tables) {
      this.listTables = tables.tableSalaryResponses;
      this.selectedTable = this.listTables[0];
    }
  }

  public navigation(patch: string): void {
    this.route.navigate([`/${routerNames.POSITIONS.BASE}/`, patch]);
  }

  public onChangeSearch(eventQuery: string) {
    if (eventQuery.length >= 3 || eventQuery.length == 0) {
      this.query = eventQuery;
      this.termQuery = this.query;
      this.resetTableInfo();
      this.getAllPositions();
    }
  }

  public onClearFilter(): void {
    this.isClearFilter = false;
    this.selected = [];
    this.resetTableInfo();
    if (this.secretKey) {
      this.getShareData();
    } else {
      this.getAllPositions();
    }
  }

  public onEditCols(): void {
    this.editCols = true;
  }

  public onFilterChecked(): void {
    this.dataResult = this.selected;
  }

  public onOccupantsSwitch(isChecked: boolean) {
    this.showJustWithOccupants = isChecked;
    this.resetTableInfo();
    if (this.secretKey) {
      this.getShareData();
    } else {
      this.getAllPositions();
    }
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
    this.positionListService.setHeaderChecked(request).subscribe(
      (res) => {
        this.getAllPositions();
      },
      (err) => {}
    );
  }

  public onSendEmail() {
    this.commonService
      .shareLink({
        to: this.email,
        url: this.shareURL,
      })
      .subscribe((_response) => this.ngxSpinnerService.hide());
  }

  onScroll(offsetY: number) {
    // total height of all rows in the viewport
    const viewHeight =
      this.el.nativeElement.getBoundingClientRect().height - this.headerHeight;

    // check if we scrolled to the end of the viewport
    if (
      !this.isLoading &&
      offsetY + viewHeight >= this.dataResult.length * this.rowHeight
    ) {
      // check if we haven't fetched any results yet
      if (this.dataResult.length === 0) {
        // calculate the number of rows that fit within viewport
        const pageSize = Math.ceil(viewHeight / this.rowHeight);

        // change the limit to pageSize such that we fill the first page entirely
        // (otherwise, we won't be able to scroll past it)
        this.page = Math.max(pageSize, this.pageLimit);
      }
      if (this.secretKey && this.page > 0) {
        this.getShareData(true);
      } else if (this.page > 0) {
        this.getAllPositions(true);
      }
    }
  }

  public async tableSelectedEvent(item: IDefault) {
    this.selectedUnit = null;
    this.selectedTable = item;
    this.resetTableInfo();
    this.getAllPositions();
    await this.getAllUnits();
  }

  public trackItem(_index: number, item: RowBody) {
    return item.colPos;
  }

  public trackItemModal(_index: number, item: Header) {
    return item.colPos;
  }

  async onLineClick(row, title) {
    this.modalDetailPosition(row[0].cmCode, title);
  }

  modalDetailPosition(cmCode: number, title: string) {
    const initialState = {
      cmCode,
      profiles: this.profiles,
      title: title,
    };

    this.modalService.show(ModalPositionDetailComponent, {
      class: "full-size",
      initialState: initialState,
    });
  }

  canAccess(subModule: SubModules) {
    return this.tokenService.validateModules(Modules.positions, subModule);
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

  setColumnWidth(header: Header) {
    if (
      header.columnId === this.listColumnEnum.GSM ||
      header.columnId === this.listColumnEnum.HourBase ||
      header.colName.includes("%")
    )
      return 100;
    else if (
      header.columnId === this.listColumnEnum.TechnicalAdjustment ||
      header.columnId === this.listColumnEnum.Smcode
    )
      return 180;

    return 280;
  }

  setClassHeader(header: Header) {
    if (
      header.columnId === this.listColumnEnum.GSM ||
      header.columnId === this.listColumnEnum.HourBase ||
      header.columnId === this.listColumnEnum.TechnicalAdjustment ||
      header.colName.includes("%")
    )
      return this.cellClass;
    return "";
  }

  openAddPosition() {
    const initialState = { profiles: this.profiles };
    this.bsModalRef = this.modalService.show(ModalAddPositionComponent, {
      class: "modal-xl",
      initialState,
    });
    this.bsModalRef.content.saveEvent.subscribe(($e) => {});

    this.ngxSpinnerService.hide();
  }

  containsParameters(item) {
    return (
      item.colName.includes(HeaderApplyStyleTypeEnum.Area) ||
      item.colName.includes(HeaderApplyStyleTypeEnum.ParameterOne) ||
      item.colName.includes(HeaderApplyStyleTypeEnum.ParameterTwo) ||
      item.colName.includes(HeaderApplyStyleTypeEnum.ParameterThree)
    );
  }

  searchFilter(event) {
    this.query = event.value;
    this.getAllPositions(false, null, event);
  }
}
