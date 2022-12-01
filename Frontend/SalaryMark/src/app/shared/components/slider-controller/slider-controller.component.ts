import { ChangeContext, LabelType, Options } from "@angular-slider/ngx-slider";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import locales from "@/locales/common";

@Component({
  selector: "app-slider-controller",
  templateUrl: "./slider-controller.component.html",
  styleUrls: ["./slider-controller.component.scss"],
})
export class SliderControllerComponent implements OnInit {
  @Input() minValue: number = 1;
  @Input() maxValue: number = 30;
  @Input() complement: string = "";
  @Input() customWidth: number;
  @Input() floor: number;
  @Input() ceil: number;
  @Input() title: string = "";
  @Input() delayTime = 500;
  @Input() restoreOptions: boolean;
  @Input() isMRAuto: boolean;

  @Output() userChange = new EventEmitter<number[]>();

  public copyMinValue: number;
  public copyMaxValue: number;

  //options: Options = optionsSlider(this.complement, this.floor, this.ceil);
  public options: Options;
  public locales = locales;
  constructor() {}

  ngOnInit(): void {
    this.copyValues();
    this.options = {
      floor: this.floor,
      ceil: this.ceil,
      translate: (value: number, label: LabelType): string => {
        switch (label) {
          case LabelType.Low:
            return value + this.complement;
          case LabelType.High:
            return value + this.complement;
          default:
            return value + this.complement;
        }
      },
    };
  }

  restoreValues() {
    this.maxValue = this.copyMaxValue;
    this.minValue = this.copyMinValue;
  }

  copyValues() {
    this.copyMaxValue = this.maxValue;
    this.copyMinValue = this.minValue;
  }

  onUserChange(changeContext: ChangeContext): void {
    this.userChange.emit([changeContext.value, changeContext.highValue]);
  }

  getChangeContextString(changeContext: ChangeContext) {
    return (
      changeContext.value +
      this.complement +
      changeContext.highValue +
      this.complement
    );
  }
}
