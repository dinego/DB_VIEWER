import { copyObject } from "@/shared/common/functions";
import { ConfirmModalEditPositionComponent } from "@/shared/components/confirm-modal-edit-position/confirm-modal-edit-position.component";
import { IParameters } from "@/shared/interfaces/parameters";
import {
  ParamRequest,
  PositionDetailRequest,
} from "@/shared/interfaces/position-detail";
import { IDefault } from "@/shared/interfaces/positions";
import { Modules } from "@/shared/models/modules";
import { ProfilesResponse } from "@/shared/models/salary-table";
import { CommonService } from "@/shared/services/common/common.service";
import { PositionDetailsService } from "@/shared/services/position-details/position-details.service";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { NgxSpinnerService } from "ngx-spinner";
import { ToastrService } from "ngx-toastr";
import { locales } from "./common/locales";
import {
  IModalPositionDetail,
  IParameter,
  IParameterGroupItems,
  IProjetParameter,
  ParametersEnum,
} from "./common/positioin-detail";

@Component({
  selector: "app-position-detail-tab",
  templateUrl: "./position-detail-tab.component.html",
  styleUrls: ["./position-detail-tab.component.scss"],
})
export class PositionDetailTabComponent implements OnInit {
  @Input() positionDetail: IModalPositionDetail;
  @Input() listParameters: IParameterGroupItems[];
  @Input() levelCombo: IProjetParameter[];
  @Input() profileCombo: ProfilesResponse;
  @Input() canEditListPosition: boolean;
  @Input() moduleId: number;
  @Input() modalRef: BsModalRef;
  @Input() gsmGlobalLabel: string;

  @Output() hideModalPosition = new EventEmitter<boolean>();
  @Output() showModalPosition = new EventEmitter();

  public levelSelected: IProjetParameter;
  public profileSelected: IDefault;
  public editable: boolean;
  public locales = locales;
  public parametersCopy: IParameterGroupItems[];

  constructor(
    private _positionDetailsService: PositionDetailsService,
    private _ngxSpinnerService: NgxSpinnerService,
    private _toastrService: ToastrService,
    private _commonService: CommonService,
    private _modalService: BsModalService
  ) {}

  async ngOnInit() {
    this.parametersCopy = copyObject(this.listParameters);
    this.setSelectedLevel(this.positionDetail.header.levelId);
    this.setSelectedProfile(this.positionDetail.header.groupId);
  }

  setSelectedLevel(levelId: number) {
    this.levelSelected = this.levelCombo.find((el) => el.id === levelId);
    this.positionDetail.header.levelId = levelId;
  }

  setSelectedProfile(groupId: number) {
    this.profileSelected = this.profileCombo.profilesResponse.find(
      (el) => parseInt(el.id) === groupId
    );
    this.positionDetail.header.groupId = groupId;
  }

  selectedLevel(event: IDefault) {
    this.setSelectedLevel(parseInt(event.id));
  }

  selectedProfile(event: IDefault) {
    this.setSelectedProfile(parseInt(event.id));
  }

  removeClick(list: IProjetParameter[], removeId: number, title: string) {
    const searchItem = list.find((f) => f.id === removeId);
    const itemSplice = list.indexOf(searchItem);
    list.splice(itemSplice, 1);

    this.listParameters = this.listParameters.map(
      (res) =>
        ({
          list: res.list.filter((l) => l.parentParameterId != removeId),
          parameter: {
            parameterId: res.parameter.parameterId,
            title: res.parameter.title,
            projetParameters:
              res.parameter &&
              res.parameter.projetParameters &&
              res.parameter.projetParameters.filter(
                (pp) => pp.parentParameterId != removeId
              ),
            newProjectParameters: res.parameter?.newProjectParameters?.filter(
              (f) => f != title
            ),
          },
        } as IParameterGroupItems)
    );
  }

  async addItemToListParam(
    event: IProjetParameter,
    parameterId: number,
    list: IProjetParameter[]
  ) {
    if (!list.some((l) => l.id == event.id)) list.push(event);
    if (parameterId == ParametersEnum.Area) {
      this.getCareerAxis();
    }
  }

  verifyExistsParameter(event, paramterId: number, _list: any[]) {
    const param = this.listParameters.find((f) => {
      return f.parameter.parameterId === paramterId;
    });

    const includes = param.list.find((f) => f.title === event);

    return {
      includes: includes !== null && includes !== undefined,
      parameter: param,
    };
  }

  addParameterToListParam(event, paramterId: number, list: any[]) {
    const verifyParameter = this.verifyExistsParameter(event, paramterId, list);

    if (verifyParameter.includes) {
      this._toastrService.error("Parametro já existente na lista.");
      return;
    }

    if (!verifyParameter.includes) {
      verifyParameter.parameter.list.push({
        title: event,
        id: 0,
      });

      if (!verifyParameter.parameter.parameter.newProjectParameters) {
        verifyParameter.parameter.parameter.newProjectParameters = [];
      }

      verifyParameter.parameter.parameter.newProjectParameters.push(event);
      verifyParameter.parameter.parameter.projetParameters.push({
        id: 0,
        title: event,
      });
    }

    return;
  }

  getCountItems(items): number {
    return items.length;
  }

  postData() {
    const objToSend = this.getPositionDetailRequest();

    this._ngxSpinnerService.show();
    this._positionDetailsService
      .putPositionDetails(objToSend)
      .subscribe((ret) => {
        if (ret) {
          this._toastrService.success(ret.message);
          this._ngxSpinnerService.hide();
          this.editable = false;

          setTimeout(() => {
            window.location.reload();
          }, 500);
        }
      });
  }

  getPositionDetailRequest() {
    const paramsRequest: ParamRequest[] = [];

    this.positionDetail.parameters.forEach((param) => {
      param.projetParameters = param.projetParameters.filter((f) => f.id !== 0);

      const paramToPush: ParamRequest = {
        parameterId: param.parameterId,
        projectParameters: param.projetParameters.map((m) => m.id),
        newProjectParameters: param.newProjectParameters,
      };
      paramsRequest.push(paramToPush);
    });

    const objToSend: PositionDetailRequest = {
      positionId: this.positionDetail.header.positionId,
      position: this.positionDetail.header.position,
      smCode: this.positionDetail.header.smCode,
      levelId: this.positionDetail.header.levelId,
      groupId: this.positionDetail.header.groupId,
      moduleId: Modules.tableSalary,
      parameters: paramsRequest,
    };

    return objToSend;
  }

  async getCareerAxis() {
    const careerAxisParameters = this.listParameters.find(
      (p) => p.parameter && p.parameter.parameterId == ParametersEnum.Area
    );
    const areasId =
      careerAxisParameters &&
      careerAxisParameters.parameter &&
      careerAxisParameters.parameter.projetParameters
        ? careerAxisParameters.parameter.projetParameters.map((pp) => pp.id)
        : [];
    if (areasId.length <= 0) return;
    const careerAxis = await this._commonService
      .getAllCareerAxis(areasId)
      .toPromise();
    let careerAxisParameter = this.listParameters.find(
      (lp) =>
        lp.parameter && lp.parameter.parameterId == ParametersEnum.CareerAxis
    );

    careerAxisParameter.list = careerAxis.filter(
      (ca) =>
        !careerAxisParameter.parameter.projetParameters.some(
          (pp) => pp.id == ca.id
        )
    );
    this._ngxSpinnerService.hide();
  }

  async edit() {
    this.editable = !this.editable;
    await this.getCareerAxis();
  }

  async cancel() {
    this.editable = !this.editable;
    this.listParameters = copyObject(this.parametersCopy);
  }

  openModalConfirm() {
    this.hideModalPosition.emit(true);

    this.modalRef = this._modalService.show(ConfirmModalEditPositionComponent, {
      class: "modal-dialog modal-dialog-centered",
    });

    this.modalRef.content.onSaveEmitter.subscribe((_res) => {
      this.postData();
    });

    this.modalRef.content.onCancelEmitter.subscribe((_res) => {
      this.showModalPosition.emit();
    });
  }
}
