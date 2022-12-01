import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { Subject } from "rxjs/internal/Subject";

@Component({
  selector: "app-confirm-modal-edit-position",
  templateUrl: "./confirm-modal-edit-position.component.html",
  styleUrls: ["./confirm-modal-edit-position.component.scss"],
})
export class ConfirmModalEditPositionComponent implements OnInit {
  public onClose: Subject<boolean>;
  public modalRef: BsModalRef;

  @Output() onSaveEmitter = new EventEmitter<boolean>();
  @Output() onCancelEmitter = new EventEmitter<boolean>();

  constructor(private _bsModalRef: BsModalRef) {
    this.modalRef = _bsModalRef;
  }

  ngOnInit(): void {}

  public onCancel(): void {
    this.onCancelEmitter.emit(true);
    this.modalRef.hide();
    this.onClose?.next(false);
  }

  onSave() {
    this.onSaveEmitter.emit(true);
    this.modalRef.hide();
  }
}
