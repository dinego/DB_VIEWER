import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { Header } from "@/shared/models/map-table";

@Component({
  selector: "app-view-child-show-modal",
  templateUrl: "./view-child-show-modal.component.html",
  styleUrls: ["./view-child-show-modal.component.scss"],
})
export class ViewChildShowModalComponent implements OnInit {
  @Input() editCols: boolean;
  @Input() item: Header;
  @Input() columnId: number;
  @Output() onValueChange = new EventEmitter<string>();

  public isChecked: boolean;
  public value: string;

  constructor() {}

  ngOnInit(): void {
    this.isChecked = this.item.isChecked;
    this.value = !this.item
      ? ""
      : this.item.nickName
      ? this.item.nickName
      : this.item.colName;
  }
}
