import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { IParameterPositionDetail } from "../modal-position-detail/common/tabs/position-detail-tab/common/positioin-detail";

@Component({
  selector: "app-item-button-remove",
  templateUrl: "./item-button-remove.component.html",
  styleUrls: ["./item-button-remove.component.scss"],
})
export class ItemButtonRemoveComponent implements OnInit {
  @Input() parameter: string;
  @Input() enabled: boolean;
  @Output() removeClick = new EventEmitter<void>();

  constructor() {}

  ngOnInit(): void {}

  clickRemove() {
    this.removeClick.emit();
  }
}
