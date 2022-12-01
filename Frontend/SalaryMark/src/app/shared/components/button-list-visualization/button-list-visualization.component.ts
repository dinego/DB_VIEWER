import { copyObject } from "@/shared/common/functions";
import { IDisplayListTypes } from "@/shared/interfaces/positions";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { DisplayTypesEnum } from "./common/typeVisualizationsEnum";

@Component({
  selector: "app-button-list-visualization",
  templateUrl: "./button-list-visualization.component.html",
  styleUrls: ["./button-list-visualization.component.scss"],
})
export class ButtonListVisualizationComponent implements OnInit {
  @Input() list: IDisplayListTypes[];
  @Input() selected: IDisplayListTypes;
  @Output() textSelected = new EventEmitter<IDisplayListTypes>();

  public selectedItem: IDisplayListTypes;
  public displayTypes = DisplayTypesEnum;

  constructor() {}

  ngOnInit(): void {
    this.selectedItem = copyObject(this.selected);
  }

  changeText(item: IDisplayListTypes): void {
    this.selectedItem = item;
    this.textSelected.emit(item);
  }
}
