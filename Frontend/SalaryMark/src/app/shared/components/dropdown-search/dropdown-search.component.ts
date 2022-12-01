import locales from "@/locales/common";
import { IDefault } from "@/shared/interfaces/positions";

import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";

@Component({
  selector: "app-dropdown-search",
  templateUrl: "./dropdown-search.component.html",
  styleUrls: ["./dropdown-search.component.scss"],
})
export class DropdownSearchComponent implements OnInit {
  @Input() isFullSize: boolean;
  @Input() list: IDefault[] = [];
  @Output() searchEmitter = new EventEmitter<any>();

  public locales = locales;
  public isCollapsed: boolean = false;
  public filterSelected: IDefault = null;

  constructor() {}

  ngOnInit(): void {
    this.filterSelected =
      this.list && this.list.length > 0 ? this.list[0] : null;
  }

  searchEmit(value: string) {
    this.searchEmitter.emit({ value: value, selected: this.filterSelected });
  }

  selectFilter(selected: IDefault) {
    this.filterSelected = selected;
  }
}
