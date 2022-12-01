import { ClickProposedMovementsChartDataInput } from "@/shared/components/charts/proposed-movements-chart/proposed-movements-chart-input";
import { ProposedMovementsChart } from "@/shared/models/positioning";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";

@Component({
  selector: "app-proposed-movements-table",
  templateUrl: "./proposed-movements-table.component.html",
  styleUrls: ["./proposed-movements-table.component.scss"],
})
export class ProposedMovementsTableComponent implements OnInit {
  @Input() proposedMovementsChart: ProposedMovementsChart;
  @Input() filterTable: string = "";
  @Output() openModalEmitter =
    new EventEmitter<ClickProposedMovementsChartDataInput>();

  constructor() {}

  ngOnInit(): void {}

  openModalClick(click) {
    this.openModalEmitter.emit(click);
  }

  clickNameItem(item) {
    const itemForData = this.proposedMovementsChart.chart.find((f) =>
      f.data.find((find) => find.name === item.name)
    );

    const itemclick: any = itemForData.data.find((f) => f.name === item.name);

    this.openModalClick(itemclick.click);
  }
}
