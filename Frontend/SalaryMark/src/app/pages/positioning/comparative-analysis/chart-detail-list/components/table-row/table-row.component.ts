import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import locales from '@/locales/salary-table';

import {
  ListPositions,
  Header,
  CheckItem,
} from '@/shared/components/charts/comparative-analysis-chart/comparative-analysis-chart-input';

@Component({
  selector: 'app-comparative-analysis-table-row',
  templateUrl: './table-row.component.html',
  styleUrls: ['./table-row.component.scss'],
})
export class ComparativeAnalysisTableRowComponent implements OnInit {
  @Input() headers: Array<Header> = [];
  @Input() index = 0;
  @Input() items: Array<any> = [];
  @Input() isActive = false;
  @Input() isRowSelect: boolean;
  @Input() listPositions: Array<ListPositions>;

  @Output() isCheckedItem = new EventEmitter<CheckItem>();
  @Output() isRowSelectItem = new EventEmitter<CheckItem>();

  public locales = locales;

  constructor() {}

  ngOnInit(): void {}

  get rowSelect(): string {
    return this.isRowSelect
      ? 'row-expand'
      : this.index % 2 === 0
      ? 'row-light-container'
      : 'row-dark-container';
  }
  get changeIcon(): string {
    return this.isRowSelect
      ? 'assets/imgs/svg/arrow-expand-dropdown-circle.svg'
      : 'assets/imgs/svg/arrow-dropdown-circle.svg';
  }

  getColumnsFilter() {
    this.items = this.items.filter((item, i) => {
      return item;
    });
    return this.items;
  }

  filterItem(value: boolean) {
    this.isCheckedItem.emit({ id: this.index, checked: value });
  }

  clickExpandItem(listPositions: Array<ListPositions>) {
    this.isRowSelectItem.emit({ id: this.index, checked: !this.isRowSelect });
    this.listPositions = listPositions;
  }
}
