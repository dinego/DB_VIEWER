import { SalarialChartComponent } from "@/shared/components/charts/salarial-chart/salarial-chart.component";
import * as core from "@angular/core";
import { Component, OnInit } from "@angular/core";
import { SalaryChart } from "../../common/salary-chart";

@Component({
  selector: "app-salary-table-export-png",
  templateUrl: "./salary-table-export-png.component.html",
  styleUrls: ["./salary-table-export-png.component.scss"],
})
export class SalaryTableExportPngComponent implements OnInit {
  @core.Input() salarialTable: string;
  @core.Input() unit: string;
  @core.Input() profile: string;
  @core.Input() chartHeight: number;
  @core.Input() chart: SalaryChart = null;

  constructor() {}

  ngOnInit(): void {}
}
