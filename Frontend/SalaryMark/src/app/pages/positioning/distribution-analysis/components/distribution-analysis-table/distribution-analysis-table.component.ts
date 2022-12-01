import { DistributionAnalysisChart } from "@/shared/models/positioning";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { IRowDistributionAnalisis } from "./common/IRowDistributionAnalisis";

@Component({
  selector: "app-distribution-analysis-table",
  templateUrl: "./distribution-analysis-table.component.html",
  styleUrls: ["./distribution-analysis-table.component.scss"],
})
export class DistributionAnalysisTableComponent implements OnInit {
  @Input() data: DistributionAnalysisChart;
  @Input() filterText: string = "";
  @Output() rowItemsOut = new EventEmitter<IRowDistributionAnalisis[]>();

  public rowCollapse: string[];
  public collapseGroupControl: boolean[] = [];
  public rowItems: IRowDistributionAnalisis[];

  constructor() {}

  ngOnInit(): void {
    this.getHeadersColumns();
    this.getDataRowRefactored();
  }

  getHeadersColumns() {
    this.rowCollapse = this.data.chart.main.map((item) => item.name);
  }

  getDataRowRefactored() {
    this.rowItems = [];

    this.data.chart.drillDown.forEach((drill) => {
      const titleCollapse = drill.itemGrouped;
      const belowValue = this.data.chart.main.find((f) => f.type === 3);
      const filteredData = drill.data.filter((f) => f.type === 2);
      const aboveValue = this.data.chart.main.find((f) => f.type === 1);
      const insideValue = this.data.chart.main.find((f) => f.type === 2);

      const pushData = {
        titleCollapser: titleCollapse,
        insideValue:
          insideValue &&
          insideValue.data.find((f) => f.name === titleCollapse).value
            ? insideValue.data.find((f) => f.name === titleCollapse).value
            : 0,
        aboveValue:
          aboveValue &&
          aboveValue.data.find((f) => f.name === titleCollapse).value
            ? aboveValue.data.find((f) => f.name === titleCollapse).value
            : 0,
        belowValue:
          belowValue &&
          belowValue.data.find((f) => f.name === titleCollapse).value
            ? belowValue.data.find((f) => f.name === titleCollapse).value
            : 0,
        rowsInside: filteredData.map((m) => {
          return {
            title: m.name,
            value: m.value,
          };
        }),
      } as IRowDistributionAnalisis;

      this.rowItems.push(pushData);
    });

    this.rowItemsOut.emit(this.rowItems);
  }
}
