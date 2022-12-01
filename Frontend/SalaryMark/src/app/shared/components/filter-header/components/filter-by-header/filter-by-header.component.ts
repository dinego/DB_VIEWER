import { IDefault, IDisplay } from "@/shared/interfaces/positions";
import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";

@Component({
  selector: "app-filter-by-header",
  templateUrl: "./filter-by-header.component.html",
  styleUrls: ["./filter-by-header.component.scss"],
})
export class FilterByHeaderComponent implements OnInit {
  @Input() share: any;
  @Input() shareFilterTitle: any;
  @Input() selectedItem: IDisplay;
  @Input() selectedTable: IDefault;
  @Input() showLabel = true;
  @Input() listDisplay: IDisplay[] = [];
  @Input() listTables: IDefault[] = [];
  @Output() changeTableEmitter = new EventEmitter<IDefault>();
  @Output() changeDisplayEmitter = new EventEmitter<IDisplay>();

  public selectedLocal: IDefault;

  constructor() {}

  ngOnInit(): void {}

  changeTable(item: IDefault) {
    this.selectedLocal = item;
    this.changeTableEmitter.emit(item);
  }
  changeDisplay(item: IDisplay) {
    this.selectedLocal = { id: item.id, title: item.name } as IDefault;
    this.changeDisplayEmitter.emit(item);
  }
}
