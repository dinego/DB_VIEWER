import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  OnChanges,
  HostListener,
} from "@angular/core";
import { IDefault, IUnit } from "@/shared/interfaces/positions";

@Component({
  selector: "app-button-list",
  templateUrl: "./button-list.component.html",
  styleUrls: ["./button-list.component.scss"],
})
export class ButtonListComponent implements OnInit, OnChanges {
  @Input() toolTipText: string;
  @Input() text: string;
  @Input() list: IDefault[];
  @Input() unitList: IUnit[];
  @Input() sorted: boolean;
  @Input() addAllInUnit: boolean;
  @Input() addAllInList: boolean;
  @Output() textSelected = new EventEmitter<IDefault>();
  @Output() textUnitSelected = new EventEmitter<IUnit>();
  @Input() textAllDefautl: string;
  @Input() showList: boolean;
  @Input() isLargeButton: boolean;

  public isOpened: boolean;

  constructor() {}

  ngOnInit(): void {}

  ngOnChanges(): void {
    this.addDefaultItemList();
    this.addDefaultItemUnit();
  }

  changeText(item: IDefault): void {
    this.text = item.title;
    this.textSelected.emit(item);
  }

  changeTextUnit(item: IUnit): void {
    this.text = item.unit;
    this.textUnitSelected.emit(item);
  }

  get sortedItemsList() {
    return this.list.sort((a, b) => {
      if (parseInt(a.id, 10) > parseInt(b.id, 10)) {
        return 1;
      }

      return -1;
    });
  }

  get sortedUnitList() {
    return this.unitList.sort((a, b) => {
      if (
        parseInt(a.unitId.toString(), 10) > parseInt(b.unitId.toString(), 10)
      ) {
        return 1;
      }

      return -1;
    });
  }

  addDefaultItemList() {
    if (this.list && this.addAllInList && this.list.length > 1) {
      const item: IDefault = {
        id: "0",
        title: this.textAllDefautl ? this.textAllDefautl : "Todos",
      };
      this.list.unshift(item);
    }
  }

  addDefaultItemUnit() {
    if (this.unitList && this.addAllInUnit && this.unitList.length > 1) {
      const item: IUnit = {
        unitId: 0,
        unit: this.textAllDefautl ? this.textAllDefautl : "Todos",
      };
      this.unitList.unshift(item);
    }
  }

  changeOpen() {
    this.isOpened = !this.isOpened;
  }

  @HostListener("click", ["$event.target"])
  onClick(btn) {}
}
