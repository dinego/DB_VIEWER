import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import locales from "@/locales/salary-table";
import { TypesEditModal } from "./components/commons/radioTypesEnum";
import { Subject } from "rxjs";
import { BsModalRef } from "ngx-bootstrap/modal";
import { IEditSalarialTable } from "@/shared/models/editSalarialTable";
import { ISalaryTableResponse } from "@/shared/interfaces/positions";

@Component({
  selector: "app-modal-edit-values",
  templateUrl: "./modal-edit-values.component.html",
  styleUrls: ["./modal-edit-values.component.scss"],
})
export class ModalEditValuesComponent implements OnInit {
  @Output() hideModalPositionEmitter = new EventEmitter<boolean>();

  public locales = locales;
  public typesEnum = TypesEditModal;
  public itemTypeChecked;
  public tableId: number;
  public projectId: number;
  public data: IEditSalarialTable;
  public page: number;
  public headersForTemplate: string[];
  public salaryTables: ISalaryTableResponse[];
  public canEditGSMMappingTable: boolean;
  public gsmGlobalLabel: string;

  public errorList: any[] = [];

  public onClose: Subject<boolean>;
  public modalRef: BsModalRef;

  constructor(private _bsModalRef: BsModalRef) {
    this.modalRef = _bsModalRef;
  }

  ngOnInit(): void {
    this.onClose = new Subject();
  }

  public onConfirm(): void {
    this.onClose.next(true);
    this._bsModalRef.hide();
  }

  public onCancel(): void {
    this.onClose.next(true);
    this._bsModalRef.hide();
  }

  changeCheckedType(item: any) {
    this.errorList = [];
    this.itemTypeChecked = item;
  }

  updateErrors(event: any[]) {
    this.errorList = event;
  }

  hideModalPositionByConfirm(event) {
    this.hideModalPositionEmitter.emit(true);
  }

  showModalPosition() {
    this.hideModalPositionEmitter.emit(false);
  }
}
