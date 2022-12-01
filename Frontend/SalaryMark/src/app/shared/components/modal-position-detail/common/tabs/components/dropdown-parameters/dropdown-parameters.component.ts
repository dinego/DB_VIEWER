import { ParamTypeEnum } from "@/shared/enum/modal-position-detail/parameter-type-enum";
import { IParameters } from "@/shared/interfaces/parameters";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { ParametersEnum } from "../../position-detail-tab/common/positioin-detail";

@Component({
  selector: "app-dropdown-parameters",
  templateUrl: "./dropdown-parameters.component.html",
  styleUrls: ["./dropdown-parameters.component.scss"],
})
export class DropdownParametersComponent implements OnInit {
  @Input() text: string;
  @Input() list: IParameters[];
  @Input() index: number;
  @Input() innerIndex: number;
  @Input() isLimitedDropdown: boolean;
  @Input() paramTypeById: number;

  @Output() addItemEmitter = new EventEmitter<any>();
  @Output() addParameterEmitter = new EventEmitter<string>();

  public selectedText: string;
  public addNewParameter: string;
  public parametersEnum = ParametersEnum;

  constructor() {}

  ngOnInit(): void {}

  addItem(item) {
    this.selectedText = item.parameter;
    this.addItemEmitter.emit(item);
  }

  addParameter() {
    this.addParameterEmitter.emit(this.addNewParameter);
    setTimeout(() => {
      this.addNewParameter = "";
    }, 300);
  }
}
