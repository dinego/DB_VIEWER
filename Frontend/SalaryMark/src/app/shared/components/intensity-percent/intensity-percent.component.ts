import colorsRangeBasedOnValues from "@/shared/components/charts/comparative-analysis-chart-dashboard/colorsRange";
import { Component, Input, OnInit } from "@angular/core";
import { DomSanitizer } from "@angular/platform-browser";

@Component({
  selector: "app-intensity-percent",
  templateUrl: "./intensity-percent.component.html",
  styleUrls: ["./intensity-percent.component.scss"],
})
export class IntensityPercentComponent implements OnInit {
  @Input() percent: number;
  private colorsRangeBasedOnValues = colorsRangeBasedOnValues;
  constructor(private domSanitizer: DomSanitizer) {}

  ngOnInit(): void {}

  getStylesBasedOnDataValue(data: number) {
    if (!data) {
      return "";
    }
    const colorRange = this.colorsRangeBasedOnValues.find(
      (value) => data >= value.min && data <= value.max
    );
    const text = data <= 102.999 && data >= 83 ? "#595959" : "white";

    return this.domSanitizer.bypassSecurityTrustStyle(
      `color:${!colorRange ? "#595959" : text}; background-color:${
        colorRange ? colorRange.color : "#ddd"
      }`
    );
  }
}
