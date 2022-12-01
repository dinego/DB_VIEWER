import { Component, OnInit, Output, EventEmitter, Input } from "@angular/core";

import locales from "@/locales/common";

@Component({
  selector: "app-clear-filter",
  templateUrl: "./clear-filter.component.html",
  styleUrls: ["./clear-filter.component.scss"],
})
export class ClearFilterComponent implements OnInit {
  @Output() clearFilters = new EventEmitter<boolean>();
  @Output() filters = new EventEmitter<void>();
  @Input() label: string;
  @Input() disabled: boolean;

  public locales = locales;

  constructor() {}

  ngOnInit(): void {}
}
