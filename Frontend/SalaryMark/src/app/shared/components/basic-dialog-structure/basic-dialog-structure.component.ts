import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { IDialogInput } from "@/shared/interfaces/dialog-input";

@Component({
  selector: "app-basic-dialog-structure",
  templateUrl: "./basic-dialog-structure.component.html",
  styleUrls: ["./basic-dialog-structure.component.scss"],
})
export class BasicDialogStructureComponent implements OnInit {
  @Input() input: IDialogInput;
  @Input() disabledSave: boolean = false;
  @Output() save = new EventEmitter();
  @Output() showChanges = new EventEmitter();
  @Input() isShareModal: boolean;
  @Input() IsSendModal: boolean;
  @Input() isAnalyseModal: boolean;

  constructor() {}

  ngOnInit(): void {}

  ngOnChanges() {}

  onSave() {
    this.save.emit();
  }

  onShow() {
    this.showChanges.emit();
  }
}
