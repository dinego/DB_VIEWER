import { ModalPositionDetailComponent } from "@/shared/components/modal-position-detail/modal-position-detail.component";
import { IPermissions } from "@/shared/models/token";
import { Component, Input, OnInit } from "@angular/core";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { Position } from "../salarial-chart-input";

@Component({
  selector: "app-tooltip",
  templateUrl: "./tooltip.component.html",
  styleUrls: ["./tooltip.component.scss"],
})
export class TooltipComponent implements OnInit {
  @Input() positions: Position[];
  @Input() componentScope: any;
  @Input() maxValue: number;
  @Input() showOccupantCLT: boolean;
  @Input() showOccupantPJ: boolean;
  @Input() midPoint: string;
  @Input() gsm: string;
  @Input() permissions: IPermissions;
  @Input() gsmGlobalLabel: string;

  public modalRef?: BsModalRef;

  constructor(private _modalService: BsModalService) {}

  ngOnInit(): void {
    this.midPoint = parseInt(this.midPoint).toFixed(3).toString();
  }
  ngDoCheck() {
    setTimeout(() => {
      this.positions.forEach((position) => {
        if (document.getElementById(`link-position-${position.id}`))
          document
            .getElementById(`link-position-${position.id}`)
            .addEventListener("click", this.callModalPosition.bind(this));
      });
    }, 5);
  }

  callModalPosition(event: any) {
    const getIdClick: string = event.target.id;
    const sliptClick = getIdClick.split("-");

    const positionId: number = parseInt(sliptClick[2]);

    const initialState = {
      positionId: positionId,
      permissions: this.permissions,
    };

    this.modalRef = this._modalService.show(ModalPositionDetailComponent, {
      class: "full-size",
      initialState,
    });

    setTimeout(() => {
      let modal: HTMLElement = document.getElementsByClassName(
        "modal-content"
      )[0] as HTMLElement;

      modal.click();
    }, 500);
  }
}
