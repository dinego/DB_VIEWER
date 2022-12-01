import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { IDefault, IUnit } from "@/shared/interfaces/positions";
import { IParameter as IParameter } from "@/shared/interfaces/parameters";
import { IDefaultLoop } from "@/shared/interfaces/table-position";

@Component({
  selector: "app-button-list-loop-light",
  templateUrl: "./button-list-loop-light.component.html",
  styleUrls: ["./button-list-loop-light.component.scss"],
})
export class ButtonListLoopLightComponent implements OnInit {
  @Output() itemSelected = new EventEmitter<IDefaultLoop>();

  @Input() text: string;
  @Input() index: number;
  @Input() placeholder: string;
  @Input() list: IDefaultLoop[];
  @Input() disabled: boolean = false;

  isCollapsed: boolean = false;

  constructor() {}

  ngOnInit(): void {}

  changeText(item: IDefaultLoop): void {
    this.text = item.title;
    item.index = this.index;

    this.itemSelected.emit(item);
  }
}
