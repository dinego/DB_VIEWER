import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { Header } from '@/shared/models/salary-table';

@Component({
  selector: 'app-table-header',
  templateUrl: './table-header.component.html',
  styleUrls: ['./table-header.component.scss']
})
export class TableHeaderComponent implements OnInit {

  @Input() items: Array<Header> = [];
  @Input() isActive: boolean;
  @Output() isCheckedAll = new EventEmitter<boolean>();

  constructor() { }

  ngOnInit(): void {
  }

  filterAll(value: boolean) {
    this.isCheckedAll.emit(value);
    this.isActive = value;
  }

  getHeadersFilter() {
    if (this.items) return this.items.filter((item, i) => {
      if (item.visible) {
        if(item.isChecked){
          return item;
        }
      }else{
        return item;
      }
    });
  }

}
