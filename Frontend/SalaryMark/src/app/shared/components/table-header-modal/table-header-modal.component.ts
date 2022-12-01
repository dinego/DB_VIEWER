import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  ViewChildren,
  QueryList,
  OnChanges,
} from "@angular/core";

import locales from "@/locales/common";
import { Header } from "@/shared/models/salary-table";
import { ViewChildShowModalComponent } from "../view-child-show-modal/view-child-show-modal.component";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import { BsModalRef } from "ngx-bootstrap/modal";
import { Subject } from "rxjs";

@Component({
  selector: "app-table-header-modal",
  templateUrl: "./table-header-modal.component.html",
  styleUrls: ["./table-header-modal.component.scss"],
})
export class TableHeaderModalComponent implements OnInit, OnChanges {
  @ViewChildren(ViewChildShowModalComponent)
  public modalChildren: QueryList<ViewChildShowModalComponent>;
  @Input() inputModalShow: IDialogInput;
  @Input() headerInfo: Array<Header> = [];

  public viaOriginalClick: boolean;
  public inputModalShowInputModal: IDialogInput;
  public headerInfoInputModal: Array<Header> = [];

  @Output() changeHeader = new EventEmitter<
    QueryList<ViewChildShowModalComponent>
  >();
  @Output() save = new EventEmitter();
  @Output() showChanges = new EventEmitter<
    QueryList<ViewChildShowModalComponent>
  >();
  @Output() restoreFilters = new EventEmitter<
    QueryList<ViewChildShowModalComponent>
  >();

  public isModalEdit: boolean = false;
  public locales = locales;
  public nameCol: string;
  public firstBlock: Array<Header> = [];
  public secondBlock: Array<Header> = [];

  public onClose: Subject<boolean>;
  public modalRef: BsModalRef;

  constructor(private _bsModalRef: BsModalRef) {
    this.modalRef = _bsModalRef;
  }

  async ngOnInit() {
    await this.initConfigComponent();
    this.onClose = new Subject();
  }

  async initConfigComponent() {
    if (this.viaOriginalClick) {
      this.inputModalShow = this.inputModalShowInputModal;
      this.headerInfo = this.headerInfoInputModal;

      await this.calculateBlock();
    }
  }

  public onConfirm(): void {
    this.onClose.next(true);
    this._bsModalRef.hide();
  }

  public onCancel(): void {
    this.onClose.next(true);
    this._bsModalRef.hide();
  }

  ngOnChanges() {
    if (this.headerInfo && this.headerInfo.length > 0) {
      this.calculateBlock();
    }
  }

  changeEditCols() {
    this.isModalEdit = !this.isModalEdit;
  }

  saveAndShowCols() {
    this.isModalEdit = false;
    this.save.emit(this.modalChildren);
    this.changeHeader.emit(this.modalChildren);
  }

  editAndShowCols() {
    this.isModalEdit = false;
    this.showChanges.emit(this.modalChildren);
  }

  onValueChanged(value: string, index: number) {}

  public trackItemModal(index: number, item: Header) {
    return item.colPos;
  }
  canEdit() {
    return (
      this.inputModalShow &&
      this.inputModalShow.canRenameColumn &&
      this.headerInfo &&
      this.headerInfo.some((x) => x.editable)
    );
  }
  showLine() {
    return (
      this.headerInfo && this.headerInfo.filter((x) => x.visible).length > 6
    );
  }

  async calculateBlock() {
    const chunk = (arr, size) =>
      arr.reduce(
        (acc, e, i) => (
          i % size ? acc[acc.length - 1].push(e) : acc.push([e]), acc
        ),
        []
      );
    const size = this.headerInfo.length / 2;
    const blocks = chunk(this.headerInfo, Math.ceil(size));
    this.firstBlock = blocks ? blocks[0] : [];
    this.secondBlock = blocks && blocks.length === 2 ? blocks[1] : [];
  }

  onSave() {
    this.isModalEdit = false;
    this.save.emit(this.modalChildren);
    this.modalRef.hide();
  }

  onShow() {
    this.isModalEdit = false;
    this.showChanges.emit(this.modalChildren);
    this.modalRef.hide();
  }
}
