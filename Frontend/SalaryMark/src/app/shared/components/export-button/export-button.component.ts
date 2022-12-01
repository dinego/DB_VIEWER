import { Component, OnInit, Output, EventEmitter } from '@angular/core';

import locales from '@/locales/common';

@Component({
  selector: 'app-export-button',
  templateUrl: './export-button.component.html',
  styleUrls: ['./export-button.component.scss']
})
export class ExportButtonComponent implements OnInit {
  @Output() export = new EventEmitter();
  public locales = locales;

  constructor() { }

  ngOnInit(): void {
  }
}
