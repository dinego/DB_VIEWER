import {
  Component,
  OnInit,
  Input,
  Output,
  EventEmitter,
  OnChanges,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
} from "@angular/core";
import locales from "@/locales/positions";
import commonLocales from "@/locales/common";
import { IDefault, IDisplay } from "@/shared/interfaces/positions";

@Component({
  selector: "app-filter-header",
  templateUrl: "./filter-header.component.html",
  styleUrls: ["./filter-header.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FilterHeaderComponent implements OnInit, OnChanges {
  @Input() isSearch: boolean;
  @Input() listTables: IDefault[] = [];
  @Input() listDisplay: IDisplay[] = [];
  @Input() selectedTable: string;
  @Output() expandAllRows = new EventEmitter();
  @Output() textUnitySelected = new EventEmitter<IDefault>();
  @Output() tableSelected = new EventEmitter<IDefault>();
  @Output() displaySelected = new EventEmitter<IDisplay>();

  @Output() versusTableSelected = new EventEmitter<IDefault>();

  @Input() share: boolean;
  @Input() shareFilterTitle: string;
  @Input() shareFilter: string;
  @Input() isSpacedBetween = true;
  @Input() showVersusCompare: boolean;

  public locales = locales;
  public commonLocales = commonLocales;
  public selectedItem: string;
  public versusSelectedItem: string;
  public versusSelectedTable: string;

  constructor(private changeDetectorRef: ChangeDetectorRef) {}

  ngOnInit(): void {
    if (this.listTables && this.listTables.length > 0) {
      this.tableSelected.emit(this.listTables[0]);
    }

    if (this.listDisplay && this.listDisplay.length > 0) {
      this.displaySelected.emit(this.listDisplay[0]);
    }
  }

  ngOnChanges(changes): void {
    if (changes.listTables && changes.listTables.currentValue) {
      this.listTables = changes.listTables.currentValue;
      if (this.listTables && this.listTables.length > 0) {
        this.tableSelected.emit(this.listTables[0]);
      }
      this.changeDetectorRef.detectChanges();
    }
    if (changes.listDisplay && changes.listDisplay.currentValue) {
      if (this.listDisplay) {
        this.displaySelected.emit(this.listDisplay[0]);
      }
      this.changeDetectorRef.detectChanges();
    }
  }

  changeTable(item: IDefault) {
    this.selectedItem = item.title;
    this.tableSelected.emit(item);
  }

  changeDisplay(item: IDisplay) {
    this.selectedItem = item.name;
    this.displaySelected.emit(item);
  }

  versusChangeTable(item: IDefault) {
    this.versusSelectedTable = item.title;
    this.versusTableSelected.emit(item);
  }
}
