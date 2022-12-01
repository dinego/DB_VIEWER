import * as core from "@angular/core";
import { NgxSpinnerService } from "ngx-spinner";
import {
  IModalPositionDetail,
  IParameterGroupItems,
} from "./common/tabs/position-detail-tab/common/positioin-detail";
import titlesTabs from "./common/titles-tabs";
import { TabDirective } from "ngx-bootstrap/tabs";
import { CommonService } from "@/shared/services/common/common.service";
import { PositionDetailsService } from "@/shared/services/position-details/position-details.service";
import { Modules } from "@/shared/models/modules";
import { ICompanyCombo } from "@/shared/interfaces/parameters";
import { ProfilesResponse } from "@/shared/models/salary-table";
import { BsModalRef } from "ngx-bootstrap/modal";
import { Subject } from "rxjs";
import { IPermissions } from "@/shared/models/token";
import { ContractTypeEnum } from "@/shared/interfaces/positions";
import { HourlyBasisEnum } from "@/shared/models/hourly-basis";
import { EventEmitter, Output } from "@angular/core";

@core.Component({
  selector: "app-modal-position-detail",
  templateUrl: "./modal-position-detail.component.html",
  styleUrls: ["./modal-position-detail.component.scss"],
})
export class ModalPositionDetailComponent implements core.OnInit {
  @core.ViewChild("header", { static: false }) header: core.ElementRef;

  public positionId: number;
  public tableId: number;
  public unitId?: number;
  public modalRows = [];
  public titlesTabs = titlesTabs;
  public positionDetail: IModalPositionDetail;
  public selectedTab?: string;
  public isInitialTab = true;
  public title: string;
  public listParameters: IParameterGroupItems[] = [];
  public levelCombo: ICompanyCombo[];
  public profileCombo: ProfilesResponse;
  public permissions: IPermissions;
  public contractTypeId: ContractTypeEnum;
  public hoursTypeId: HourlyBasisEnum;
  public moduleId: number;
  public gsmGlobalLabel: string;

  public onClose: Subject<boolean>;
  public modalRef: BsModalRef;

  @core.Output() hide = new core.EventEmitter<any>();

  @Output() hideModalPositionEmitter = new EventEmitter<boolean>();

  constructor(
    private ngxSpinnerService: NgxSpinnerService,
    private commonService: CommonService,
    private positionDetailsService: PositionDetailsService,
    private bsModalRef: BsModalRef
  ) {
    this.modalRef = bsModalRef;
  }

  async ngOnInit() {
    await this.getPositionDetails();
    await this.getLevelsCombo();
    await this.getAllProfiles();

    this.ngxSpinnerService.hide();
  }

  async getLevelsCombo() {
    this.levelCombo = await this.commonService.getAllLevels().toPromise();
  }

  async getAllProfiles() {
    this.profileCombo = await this.commonService.getAllProfiles().toPromise();
  }

  async getPositionDetails() {
    this.positionDetail = await this.positionDetailsService
      .getPositionDetails(this.positionId, Modules.tableSalary)
      .toPromise();

    const parameterGroups = await this.commonService
      .getAllParameters()
      .toPromise();
    this.positionDetail.parameters.forEach(async (param) => {
      const parameter = parameterGroups.find(
        (pl) => pl.parameterId == param.parameterId
      );
      const objToPopulate: IParameterGroupItems = {
        parameter: param,
        list: parameter ? parameter.parameters : null,
      };
      this.listParameters.push(objToPopulate);
    });
    this.ngxSpinnerService.hide();
  }

  onSelect(data: TabDirective): void {
    this.isInitialTab = false;
    this.selectedTab = data.heading;
  }

  public onCancel(): void {
    this.onClose.next(false);
    this.bsModalRef.hide();
  }

  hideModalPositionByConfirm(event) {
    this.hideModalPositionEmitter.emit(true);
  }

  showModalPosition() {
    this.hideModalPositionEmitter.emit(false);
  }
}
