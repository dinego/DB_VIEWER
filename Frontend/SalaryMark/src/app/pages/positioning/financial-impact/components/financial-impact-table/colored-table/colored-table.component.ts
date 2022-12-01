import { ClickFinancialImpactChartDataInput } from "@/shared/components/charts/financial-impact-chart/financial-impact-chart-input";
import { FinancialImpact } from "@/shared/models/positioning";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { NgxSpinnerService } from "ngx-spinner";

@Component({
  selector: "app-colored-table",
  templateUrl: "./colored-table.component.html",
  styleUrls: ["./colored-table.component.scss"],
})
export class ColoredTableComponent implements OnInit {
  @Input() data: FinancialImpact;
  @Input() colorIndex: number;

  @Output() openModalEmitter =
    new EventEmitter<ClickFinancialImpactChartDataInput>();

  public page = 1;
  public modalRef: BsModalRef;

  constructor() {}

  ngOnInit(): void {}

  openModal(item, title) {
    item.legend = title;

    this.openModalEmitter.emit(item);
  }
}
