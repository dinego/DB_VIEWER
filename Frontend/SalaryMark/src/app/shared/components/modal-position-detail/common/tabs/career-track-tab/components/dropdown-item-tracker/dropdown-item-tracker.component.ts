import { IDefault } from "@/shared/interfaces/positions";
import { ICareerTrackPosition, IPosition } from "@/shared/models/positioning";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";

@Component({
  selector: "app-dropdown-item-tracker",
  templateUrl: "./dropdown-item-tracker.component.html",
  styleUrls: ["./dropdown-item-tracker.component.scss"],
})
export class DropdownItemTrackerComponent implements OnInit {
  @Input() position: IPosition;
  @Input() editable: boolean;
  @Input() isLast: boolean;
  @Input() innerIndex: number;
  @Input() indexRemove: number;

  @Output() removeEmitter = new EventEmitter<any>();
  @Output() selectItemEmitter = new EventEmitter<any>();

  public selected: IDefault = {
    id: null,
    title: null,
  };

  constructor() {}

  ngOnInit(): void {
    this.selected.id = this.position.positionId.toString();
    this.selected.title = this.position.position;
  }

  selectItem(item: any) {
    this.selected = item;

    const objSender = {
      item,
      innerIndex: this.innerIndex,
      indexModify: this.indexRemove,
    };

    this.selectItemEmitter.emit(objSender);
  }

  removeItemClick() {
    const pos = { inner: this.innerIndex, remove: this.indexRemove };
    this.removeEmitter.emit(pos);
  }
}
