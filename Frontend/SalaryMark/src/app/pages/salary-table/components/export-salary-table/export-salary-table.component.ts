import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import locales from "@/locales/common";
import { DisplayTypesEnum } from "@/shared/components/button-list-visualization/common/typeVisualizationsEnum";

@Component({
  selector: "app-export-salary-table",
  templateUrl: "./export-salary-table.component.html",
  styleUrls: ["./export-salary-table.component.scss"],
})
export class ExportSalaryTableComponent implements OnInit {
  public locales = locales;
  public displayTypesEnum = DisplayTypesEnum;

  @Input() selectedVisualization;

  @Output() exportChart = new EventEmitter<void>();
  @Output() exportCSV = new EventEmitter<void>();

  constructor() {}

  ngOnInit(): void {}
}
