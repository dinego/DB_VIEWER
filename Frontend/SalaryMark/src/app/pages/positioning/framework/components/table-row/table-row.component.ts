import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import locales from '@/locales/salary-table';

import { ListPositions, Header, CheckItem } from '@/shared/models/salary-table';

declare var $: any;
@Component({
  selector: 'app-table-row',
  templateUrl: './table-row.component.html',
  styleUrls: ['./table-row.component.scss']
})
export class TableRowComponent implements OnInit {


  @Input() headers: Array<Header> = [];
  @Input() index = 0;
  @Input() items: Array<any> = [];
  @Input() isActive = false;
  @Input() isRowSelect: boolean;
  @Input() listPositions: Array<ListPositions>;

  @Output() isCheckedItem = new EventEmitter<CheckItem>();
  @Output() isRowSelectItem = new EventEmitter<CheckItem>();
  Output

  public locales = locales;


  constructor() { }

  ngOnInit(): void { }


  get rowSelect(): string {
    return this.isRowSelect ?
      'row-expand' : this.index % 2 === 0 ?
      'row-light-container' : 'row-dark-container';
  }
  get changeIcon(): string {
    return this.isRowSelect ?
      '../../../../../assets/imgs/svg/arrow-expand-dropdown-circle.svg' :
      '../../../../../assets/imgs/svg/arrow-dropdown-circle.svg';
  }

  getColumnsFilter() {
    return this.items.filter((item, i) => {
      if (this.headers[i] && this.headers[i].visible) {
        if(this.headers[i].isChecked){
          return item;
        }
      }else{
        return item;
      }
    });
  }

  filterItem(value: boolean) {
    this.isCheckedItem.emit({ id: this.index, checked: value });
  }

  getFormatValue(value: string): string {
    return value.split(',')[0];
  }

  clickExpandItem(listPositions: Array<ListPositions>) {
    this.isRowSelectItem.emit({ id: this.index, checked: !this.isRowSelect });
    this.listPositions = listPositions;
  }

  onClickItem(item: any) {
    if(item && item.colPos === 0) {
      $('#frameworkModal').modal('show');
    }
  }
}
