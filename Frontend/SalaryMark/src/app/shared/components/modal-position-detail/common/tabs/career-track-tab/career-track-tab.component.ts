import { copyObject } from "@/shared/common/functions";
import { ConfirmModalEditPositionComponent } from "@/shared/components/confirm-modal-edit-position/confirm-modal-edit-position.component";
import { ICareerTrackPosition, IParameter } from "@/shared/models/positioning";
import { PositioningService } from "@/shared/services/positioning/positioning.service";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { ToastrService } from "ngx-toastr";

@Component({
  selector: "app-career-track-tab",
  templateUrl: "./career-track-tab.component.html",
  styleUrls: ["./career-track-tab.component.scss"],
})
export class CareerTrackTabComponent implements OnInit {
  @Input() modalRef: BsModalRef;
  @Input() moduleId: number;
  @Output() hideModalPosition = new EventEmitter<boolean>();
  @Output() showModalPosition = new EventEmitter();

  public careerTrack: ICareerTrackPosition;
  public copyCarrerTrack: ICareerTrackPosition;
  public editable: boolean;

  public copyCareerTrackParameters: IParameter[];

  constructor(
    private positioningService: PositioningService,
    private _modalService: BsModalService,
    private _toastrService: ToastrService
  ) {}

  async ngOnInit() {
    await this.getCarrerTrack();
  }

  async getCarrerTrack() {
    this.careerTrack = await this.positioningService
      .getCareerTrack(null)
      .toPromise();

    this.copyCarrerTrack = copyObject(this.careerTrack);
    this.copyCareerTrackParameters = this.copyCarrerTrack.parameters;
  }

  getClassDivisorParameters(count: number): string {
    return `col-sm-${Math.floor(12 / count)}`;
  }

  resetData() {}

  removeInner(objIndexArray: any) {
    this.copyCareerTrackParameters[0].parametersInner[
      objIndexArray.inner
    ].positionsRelated.splice(objIndexArray.remove, 1);

    if (
      this.copyCareerTrackParameters[0].parametersInner[objIndexArray.inner]
        .positionsRelated.length === 0
    ) {
      this.copyCareerTrackParameters[0].parametersInner.splice(
        objIndexArray.inner,
        1
      );
    }
  }

  selectItem(event) {
    this.copyCarrerTrack.parameters[0].parametersInner[
      event.innerIndex
    ].positionsRelated[event.indexModify].positionId = parseInt(event.item.id);
    this.copyCarrerTrack.parameters[0].parametersInner[
      event.innerIndex
    ].positionsRelated[event.indexModify].position = event.item.title;
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

  saveData() {
    this.modalRef.hide();
    this._toastrService.success("Trilha de carreira salva com sucesso");
    setTimeout(() => {
      window.location.reload();
    }, 500);
  }
}
