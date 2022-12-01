import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { IDefault, IUnit } from "@/shared/interfaces/positions";
import { IParameter as IParameter } from "@/shared/interfaces/parameters";
import { IDefaultLoop } from "@/shared/interfaces/table-position";

@Component({
  selector: "app-button-list-light",
  templateUrl: "./button-list-light.component.html",
  styleUrls: ["./button-list-light.component.scss"],
})
export class ButtonListLightComponent implements OnInit {
  @Output() textSelected = new EventEmitter<IDefault>();
  @Output() textUnitSelected = new EventEmitter<IUnit>();
  @Output() paramSelected = new EventEmitter<IParameter>();

  @Input() text: string;
  @Input() placeholder: string;
  @Input() list: IDefault[];
  @Input() unitList: IUnit[];
  @Input() showList: boolean;
  @Input() addAllInUnit: boolean;
  @Input() textAllDefautl: string;
  @Input() addAllInList: boolean;
  @Input() toolTipText: string;
  @Input() sorted: boolean;
  @Input() disabled: boolean = false;
  @Input() groupName: any = null;
  @Input() isFullSize: boolean;
  @Input() isDropUnit: boolean;

  public isCollapsed: boolean = false;
  public listFilter: IDefault[] = [];
  public minLengthSearch = 10;
  constructor() {}

  ngOnInit(): void {}

  changeText(item: IDefault): void {
    if (this.groupName !== null) {
      const objSelected = {
        id: item.id,
        param: item.title,
        groupName: this.groupName,
      };

      this.paramSelected.emit(objSelected);
      return;
    }

    this.text = item.title;

    this.textSelected.emit(item);
  }

  changeTextUnit(item: IUnit): void {
    this.text = item.unit;
    this.textUnitSelected.emit(item);
  }

  onSearchChange(event: string) {
    const searchStr = event.toLowerCase();

    if (this.unitList && this.unitList.length > 0) {
      const toDefault: IDefault[] = this.unitList.map((m) => {
        return {
          id: m.unitId.toString(),
          title: m.unit,
        } as IDefault;
      });
      this.listFilter = toDefault.filter((f) =>
        f.title.toLowerCase().includes(searchStr)
      );
      return;
    }

    this.listFilter = this.list.filter((f) =>
      f.title.toLowerCase().includes(searchStr)
    );
  }
}
