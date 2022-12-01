import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  SimpleChanges,
  ViewChild,
  OnChanges,
} from "@angular/core";
import { BsModalRef } from "ngx-bootstrap/modal";
import { TooltipDirective } from "ngx-bootstrap/tooltip";
import { Subject } from "rxjs/internal/Subject";

@Component({
  selector: "app-modal-share-table",
  templateUrl: "./modal-share-table.component.html",
  styleUrls: ["./modal-share-table.component.scss"],
})
export class ModalShareTableComponent implements OnInit, OnChanges {
  @ViewChild("popLink") popLink: TooltipDirective;

  @Input() url: string;
  public cancelButtonLabel: string;
  public saveButtonLabel: string;
  public title: string;
  public email: string;
  @Output() onChangeEmail = new EventEmitter<string>();
  @Output() onSaveEmitter = new EventEmitter();

  public onClose: Subject<boolean>;
  public modalRef: BsModalRef;

  constructor(private _bsModalRef: BsModalRef) {
    this.modalRef = _bsModalRef;
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.url) {
      setTimeout(() => {
        this.popLink.show();
      }, 500);
    }
  }

  ngOnInit(): void {}

  public onCancel(): void {
    this.onClose.next(false);
    this._bsModalRef.hide();
  }

  onSave() {
    this.onSaveEmitter.emit();
  }
}
