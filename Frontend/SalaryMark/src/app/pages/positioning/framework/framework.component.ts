import {
  Component,
  OnInit,
  ViewChild,
  ElementRef,
  ChangeDetectorRef,
} from "@angular/core";
import { Clipboard } from "@angular/cdk/clipboard";
import * as fs from "file-saver";

import locales from "@/locales/positioning";
import commonLocales from "@/locales/common";
import {
  IDefault,
  IUnit,
  CheckItem,
  Table,
  IShareFrameworkFilter,
  IDialogFramework,
  IHeaderFrameworkPosition,
} from "@/shared/models/framework";
import { CommonService } from "@/shared/services/common/common.service";
import { NgxSpinnerService } from "ngx-spinner";
import { FrameworkService } from "@/shared/services/framework/framework.service";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import { TokenService } from "@/shared/services/token/token.service";
import { IPermissions } from "@/shared/models/token";
import { Header } from "@/shared/models/position-table";
import {
  UpdateColumns,
  IUpdateDisplayColumnsListRequest,
  ContractTypeEnum,
  IDisplay,
  IDisplayTypes,
  IDisplayListTypes,
} from "@/shared/interfaces/positions";
import { Modules, SubModules } from "@/shared/models/modules";
import { environment } from "src/environments/environment";
import { ActivatedRoute } from "@angular/router";
import { IShareHeader } from "@/shared/models/share-header";
import { DomSanitizer } from "@angular/platform-browser";
import { styleBasedOnValue } from "@/utils/style-based-on-value";
import {
  ColumnMode,
  DatatableComponent,
  SelectionType,
} from "@swimlane/ngx-datatable";
import { HourlyBasisEnum } from "@/shared/models/hourly-basis";
import { MovimentTypeEnum } from "@/shared/enum/moviment-type-enum";
import { FrameworkColumnsMainEnum } from "@/shared/enum/framework-columns-main-enum";
import { FrameworkFullInfoEnum } from "@/shared/enum/framework-full-info-enum";
import { MediaObserver } from "@angular/flex-layout";
import { isUndefined } from "@/shared/common/functions";
import visualizations from "./common/visualizations";
import { DisplayTypesEnum } from "@/shared/components/button-list-visualization/common/typeVisualizationsEnum";
declare var $: any;
@Component({
  selector: "app-framework",
  templateUrl: "./framework.component.html",
  styleUrls: ["./framework.component.scss"],
})
export class FrameworkComponent implements OnInit {
  @ViewChild("scrollOne") scrollOne: ElementRef;
  @ViewChild("scrollTwo") scrollTwo: ElementRef;
  @ViewChild("firstTable") firstTable: DatatableComponent;
  @ViewChild("secondTable") secondTable: DatatableComponent;

  public locales = locales;
  public commonLocales = commonLocales;
  public isClearFilter = false;
  public contracts: IDefault[];
  public monthlyList: IDefault[];
  public units: IUnit[];
  public selectedContract: IDefault;
  public selectedMonthly: IDefault;
  public selectedUnit: IUnit;
  public query: string;
  public termQuery = "";
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
  public checkInputs: Array<CheckItem> = [];
  public checkInputAll: boolean;
  public inputModalShow: IDialogInput;
  public inputShareModal: IDialogInput;
  public inputModalFramework: IDialogInput;
  public permissions: IPermissions;
  public modalHeader: Array<any>;
  public editCols = false;
  public displayColumns: Array<UpdateColumns> = [];
  public shareURL: string;
  public email: string;
  public secretKey: string;
  public share = false;
  public shareData: IShareFrameworkFilter;
  public shareHeader: IShareHeader[];
  public dialogFrameworkData: IDialogFramework;
  public dataResult = [];
  public columHeaders = [];
  public ColumnMode = ColumnMode;
  public SelectionType = SelectionType;
  public pageLimit = 20;
  public headerHeight = 50;
  public rowHeight = 46;
  public isLoading: boolean = false;
  public selected = [];
  public isAsc = true;
  public sortColumnId?: number;
  public haveSecondTable = false;
  public hourlyBasisEnum = HourlyBasisEnum;
  public hoursType = HourlyBasisEnum.MonthSalary;
  public movementsList: IDisplay[];
  public frameworkColumnsEnum = FrameworkColumnsMainEnum;
  public firstBlockPositionInfo: IHeaderFrameworkPosition[];
  public secondBlockPositionInfo: IHeaderFrameworkPosition[];
  public thirdBlockPositionInfo: IHeaderFrameworkPosition[];
  public updateColumns = false;
  public columnsFiltered: number[] = [];
  public tableClass = "";
  public listProfiles: IDefault[] = [];
  public visualizations: IDisplayListTypes[] = visualizations;
  public selectedVisualization: IDisplayListTypes;
  public displayTypeEnum = DisplayTypesEnum;
  public percentArrays: any[] = [];

  constructor(
    private frameworkService: FrameworkService,
    private commonService: CommonService,
    private ngxSpinnerService: NgxSpinnerService,
    private tokenService: TokenService,
    private router: ActivatedRoute,
    private clipboard: Clipboard,
    private domSanitizer: DomSanitizer,
    private el: ElementRef,
    private mediaObserver: MediaObserver
  ) {}

  setMockProfiles() {
    this.listProfiles = [
      {
        id: null,
        title: "Todos os Campos",
      },
      {
        id: "1",
        title: "Nome do Cargo",
      },
      {
        id: "2",
        title: "Setor",
      },
      {
        id: "3",
        title: "Nome Pesquisa",
      },
      {
        id: "4",
        title: "Categoria",
      },
    ];
  }

  async ngOnInit() {
    this.setMockProfiles();
    this.setInitVisualization();
    this.configureScreen();
    this.secretKey = this.router.snapshot.paramMap.get("secretkey");
    this.permissions = this.tokenService.getPermissions();
    this.isShowModeratedMovementTable =
      this.permissions && this.permissions.canFilterMM;
    this.isShowIdealMovementTable =
      this.permissions &&
      !this.permissions.canFilterMM &&
      this.permissions.canFilterMI
        ? true
        : false;

    this.inputModalShow = {
      disableFooter: false,
      idModal: "showModal",
      title: locales.showConfiguration,
      btnPrimaryTitle: locales.saveAndShow,
      btnSecondaryTitle: locales.show,
      btnWithoutCancel: true,
      canRenameColumn: this.permissions.canRenameColumns,
      isInfoPosition: true,
    };

    this.inputShareModal = {
      disableFooter: false,
      idModal: "shareModal",
      title: locales.share,
      btnPrimaryTitle: locales.send,
      btnSecondaryTitle: locales.cancel,
    };

    this.inputModalFramework = {
      disableFooter: true,
      idModal: "frameworkModal",
      title: locales.framework,
      isInfoPosition: true,
    };

    if (this.secretKey) {
      await this.getShareData();
      return;
    }
    await this.getAllMonthlyList();
    await this.getAllContractTypes();
    const resultUnits = await this.commonService.getAllUnits().toPromise();
    this.getAllUnits(resultUnits);
    await this.getMovements();
    //await this.getFramework();
  }

  setInitVisualization() {
    this.selectedVisualization = this.visualizations.find(
      (f) => f.id == this.displayTypeEnum.VALUES
    );
  }

  configureScreen() {
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

  styleButton(value: number) {
    return this.domSanitizer.bypassSecurityTrustStyle(styleBasedOnValue(value));
  }

  getFileSpreadsheet() {
    const columns = this.displayColumns
      ? this.displayColumns.map((x) => x.columnId)
      : null;
    this.frameworkService
      .getAllFrameworkExcel(
        this.isShowModeratedMovementTable,
        this.isShowIdealMovementTable,
        this.selectedContract.id,
        this.selectedMonthly.id,
        this.termQuery,
        this.selectedUnit ? this.selectedUnit.unitId : "",
        this.sortColumnId,
        this.isAsc,
        columns
      )
      .subscribe((item) => {
        const file = new Blob([item], {
          type: item.type,
        });
        const blob = window.URL.createObjectURL(file);
        fs.saveAs(blob, "Enquadramento.xlsx");
        this.ngxSpinnerService.hide();
      });
  }

  public changeHeaderShowCols(header: Array<Header>): void {
    this.modalHeader = header;
  }

  async changeSelectMonthly(item: IDefault) {
    this.selectedMonthly = item;
    this.hoursType = +item.id;
    this.resetTableInfo();
    await this.getFramework();
  }

  async changeSelectContract(item: IDefault) {
    this.selectedContract = item;
    this.resetTableInfo();
    await this.getFramework();
  }

  public async changeSelectUnit(item: IUnit) {
    this.selectedUnit = item;
    this.resetTableInfo();
    await this.getFramework();
  }

  public filterAll(value: boolean) {
    this.checkInputs.forEach((item) => (item.checked = value));
  }

  public filterItem(item: CheckItem) {
    this.checkInputs[item.id].checked = item.checked;
    this.showFilter();
  }

  public showFilter() {
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

  public async getAllMonthlyList() {
    const result = await this.commonService.getHourlyBasis().toPromise();
    if (result.hoursBaseResponse.length > 0) {
      this.monthlyList = result.hoursBaseResponse;
      this.selectedMonthly = this.monthlyList[0];
    }
  }

  public async getAllContractTypes() {
    const result = await this.commonService.getOccupantList().toPromise();
    if (result.contractTypesResponse.length > 0) {
      this.contracts = result.contractTypesResponse;
      this.selectedContract = this.contracts[0];
    }
  }

  public async getAllUnits(result) {
    if (result.length > 0) {
      this.units = result;
    }
  }

  public getShareKey(): void {
    const scenarioLabel =
      this.isShowModeratedMovementTable && this.movementsList
        ? this.movementsList.find(
            (x) => +x.id === MovimentTypeEnum.moderatedMovement
          ).name
        : this.movementsList.find(
            (x) => +x.id === MovimentTypeEnum.idealMovement
          ).name;
    this.commonService
      .getShareKey({
        moduleId: Modules.positioning,
        moduleSubItemId: SubModules.framework,
        columnsExcluded: [
          this.movementTable.header.filter((item) => (item.visible = false)),
        ],
        parameters: {
          isMM: this.isShowModeratedMovementTable,
          isMI: this.isShowIdealMovementTable,
          contractType: this.selectedContract.id,
          hoursType: this.selectedMonthly.id,
          unitId: null,
          permissions: this.permissions,
          scenarioLabel: scenarioLabel,
        },
      })
      .subscribe((key) => {
        this.shareURL = `${environment.baseUrl}posicionamento/configuracoes/compartilhar-enquadramento/${key}`;
        this.clipboard.copy(this.shareURL);
        this.ngxSpinnerService.hide();
      });
  }

  async getShareData(scrolled: boolean = false, sort?: boolean) {
    this.isLoading = !sort ? true : false;
    this.share = true;
    const result = await this.frameworkService
      .getPositionsShared(
        this.secretKey,
        this.myPage,
        this.selectedContract ? this.selectedContract.id : "",
        this.selectedMonthly ? this.selectedMonthly.id : "",
        this.selectedUnit ? this.selectedUnit.unitId : "",
        this.sortColumnId,
        this.isAsc
      )
      .toPromise();

    if (result.framework.tables) {
      const tableItems = this.isShowModeratedMovementTable
        ? result.framework.tables.filter((t) => t.displayType === 1)[0]
        : result.framework.tables.filter((t) => t.displayType === 2)[0];

      this.dataResult = this.termQuery ? [] : this.dataResult;
      this.movementTable =
        this.movementTable &&
        this.movementTable.header &&
        this.movementTable.header.length > 0
          ? this.movementTable
          : tableItems;

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
    if (result.share) {
      //TODO:: PEDIR PARA O VALDEMAR COLOCAR O NOME DO TIPO DE MOVIMENTO FILTRADO
      this.shareData = result.share;
      this.selectedContract = {
        id: result.share.contractTypeId.toString(),
        title: result.share.contractType,
      };
      this.selectedMonthly = {
        id: result.share.hoursTypeId.toString(),
        title: result.share.hoursType,
      };
      this.selectedUnit = {
        unitId: result.share.unitId,
        unit: result.share.unit,
      };

      this.shareHeader = this.prepareShareHeader(result.share);
    }
    this.getHeaderColumns();
    this.myPage = result.nextPage;
    this.isLoading = false;
    this.ngxSpinnerService.hide();
  }

  prepareShareHeader(shareData: IShareFrameworkFilter): IShareHeader[] {
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
    ];
  }

  public async getFramework(scrolled: boolean = false, sort?: boolean) {
    this.isLoading = !sort ? true : false;
    const result = await this.frameworkService
      .getAllFrameworks(
        this.isShowModeratedMovementTable,
        this.isShowIdealMovementTable,
        this.myPage,
        this.selectedContract.id,
        this.selectedMonthly.id,
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

  public async onChangeSearch(eventQuery: string) {
    if (eventQuery.length >= 3 || eventQuery.length == 0) {
      this.query = eventQuery;
      this.termQuery = this.query;
      this.resetTableInfo();
      await this.getFramework();
    }
  }

  onLineClick(tableType: number) {
    this.dialogFrameworkData = null;
    if (this.secretKey) {
      this.frameworkService
        .getFullInfoFrameworkForShare(this.secretKey, tableType)
        .subscribe((item) => {
          this.dialogFrameworkData = item;
          $("#frameworkModal").modal("show");
          this.ngxSpinnerService.hide();
        });
    } else {
      this.frameworkService
        .getFullInfoFramework(
          tableType,
          this.isShowModeratedMovementTable,
          this.isShowIdealMovementTable,
          this.selectedContract.id,
          this.selectedMonthly.id
        )
        .subscribe((item) => {
          this.dialogFrameworkData = item;

          $("#frameworkModal").modal("show");
          this.ngxSpinnerService.hide();
        });
    }
  }

  public async onClearFilter() {
    this.checkInputAll = false;
    this.isClearFilter = false;
    this.selected = [];
    this.filterAll(false);
    this.movementTable = null;
    this.resetTableInfo();
    if (this.secretKey) {
      await this.getShareData(true);
    } else {
      await this.getFramework(true);
    }
  }

  public onFilterChecked() {
    this.dataResult = this.selected;
    this.selected = [];
  }

  onSelect({ selected }) {
    this.selected.splice(0, this.selected.length);
    this.selected.push(...selected);
    this.isClearFilter = this.selected.length > 0;
  }

  async onSort(event: any) {
    this.isAsc = event.newValue !== "desc";
    this.sortColumnId = event.column.prop;
    this.resetTableInfo();
    if (this.secretKey) {
      await this.getShareData(false, true);
    } else {
      await this.getFramework(false, true);
    }
  }

  public async onRestoreCols(modalChildren) {
    this.editCols = false;
    modalChildren.forEach((mdItem, index) => {
      mdItem.isChecked = true;
      mdItem.value = this.movementTable.header[index].colName;
      this.movementTable.header[index].isChecked = true;
      this.movementTable.header[index].nickName = mdItem.value;
    });
  }

  public onShowChanges(modalChildren, isEdit: boolean = false) {
    modalChildren.forEach((mdItem, index) => {
      if (
        this.movementTable.header[index].visible &&
        (this.movementTable.header[index].isChecked !== mdItem.isChecked ||
          mdItem.value)
      ) {
        this.displayColumns.push({
          columnId: mdItem.columnId,
          name:
            mdItem && mdItem.value
              ? mdItem.value
              : this.movementTable.header[index].colName,
          isChecked: mdItem.isChecked,
        });
      }

      this.movementTable.header[index].isChecked = mdItem.isChecked;
      if (mdItem.value) {
        this.movementTable.header[index].nickName = mdItem.value;
      }
    });
    this.getHeaderColumns();
    this.updateColumns = isEdit;
  }

  public onSendEmail() {
    this.commonService
      .shareLink({
        to: this.email,
        url: this.shareURL,
      })
      .subscribe((_response) => this.ngxSpinnerService.hide());
  }

  public onPutEmail(event) {
    this.email = event;
  }

  public async saveAndShowCols(modalChildren) {
    this.onShowChanges(modalChildren, true);
    if (!this.displayColumns || this.displayColumns.length <= 0) return;
    const request: IUpdateDisplayColumnsListRequest = {
      displayColumns: this.displayColumns,
    };
    await this.frameworkService.setHeaderChecked(request).toPromise();
    await this.getFramework();
  }

  async onScroll(offsetY: number) {
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
        this.myPage = Math.max(pageSize, this.pageLimit);
      }
      if (this.secretKey && this.myPage > 0) {
        await this.getShareData(true);
      } else if (this.myPage > 0) {
        await this.getFramework(true);
      }
    }
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
  get contractTypeCLT() {
    const clt = this.contracts.find(
      (x) => x.id === ContractTypeEnum.CLT.toString()
    );
    if (clt) {
      return clt;
    }
    const contract: IDefault = this.contracts
      ? this.contracts[0]
      : { id: "", title: "" };
    return contract;
  }
  changeUnitLabel(items: IUnit[]) {
    if (!items) return ``;
    return items.length > 1 ? this.locales.all : items[0].unit;
  }
  async getMovements() {
    const movements = await this.commonService.getMovements().toPromise();
    if (movements) {
      this.movementsList = movements.map((res) => {
        const value: IDisplay = {
          id: res.id,
          name: res.title,
        };
        return value;
      });
    }
  }
  async changeMovements(item: IDefault) {
    this.isShowModeratedMovementTable =
      +item.id === MovimentTypeEnum.moderatedMovement;
    this.isShowIdealMovementTable = +item.id === MovimentTypeEnum.idealMovement;
    this.resetTableInfo();
    if (this.secretKey) {
      await this.getShareData(true);
    } else {
      await this.getFramework();
    }
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

  getFirstBlockPositionInfo() {
    const headers = [
      FrameworkFullInfoEnum.EmployeeId,
      FrameworkFullInfoEnum.UnitPlace,
      FrameworkFullInfoEnum.HourlyBasis,
    ];
    return this.dialogFrameworkData && this.dialogFrameworkData.headerPosition
      ? this.dialogFrameworkData.headerPosition
          .filter((x) => headers.includes(x.propertyId) && x.visible)
          .map((res) => {
            switch (res.propertyId) {
              case FrameworkFullInfoEnum.EmployeeId:
                res.colPos = 0;
                break;
              case FrameworkFullInfoEnum.UnitPlace:
                res.colPos = 1;
                break;
              case FrameworkFullInfoEnum.HourlyBasis:
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
      FrameworkFullInfoEnum.Employee,
      FrameworkFullInfoEnum.Profile,
      FrameworkFullInfoEnum.Salary,
    ];
    return this.dialogFrameworkData && this.dialogFrameworkData.headerPosition
      ? this.dialogFrameworkData.headerPosition
          .filter((x) => headers.includes(x.propertyId) && x.visible)
          .map((res) => {
            switch (res.propertyId) {
              case FrameworkFullInfoEnum.Employee:
                res.colPos = 0;
                break;
              case FrameworkFullInfoEnum.Profile:
                res.colPos = 1;
                break;
              case FrameworkFullInfoEnum.Salary:
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
      FrameworkFullInfoEnum.CurrentPosition,
      FrameworkFullInfoEnum.Level,
    ];
    return this.dialogFrameworkData && this.dialogFrameworkData.headerPosition
      ? this.dialogFrameworkData.headerPosition
          .filter((x) => headers.includes(x.propertyId) && x.visible)
          .map((res) => {
            switch (res.propertyId) {
              case FrameworkFullInfoEnum.CurrentPosition:
                res.colPos = 0;
                break;
              case FrameworkFullInfoEnum.Level:
                res.colPos = 1;
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

  formatPositionValue(item) {
    return item.value.split(",")[0];
  }

  searchFilter(event) {}

  changeSelectVisualization(event: IDisplayListTypes) {
    this.selectedVisualization = event;
  }
}
