import { ClickFinancialImpactChartDataInput } from "@/shared/components/charts/financial-impact-chart/financial-impact-chart-input";
import { FinancialImpactChart } from "@/shared/models/positioning";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";

@Component({
  selector: "app-financial-impact-table",
  templateUrl: "./financial-impact-table.component.html",
  styleUrls: ["./financial-impact-table.component.scss"],
})
export class FinancialImpactTableComponent implements OnInit {
  @Input() financialImpactData: FinancialImpactChart;

  @Output() openModalEmitter =
    new EventEmitter<ClickFinancialImpactChartDataInput>();

  constructor() {}

  ngOnInit(): void {}
}
