import { IShareHeader } from '@/shared/models/share-header';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-share-header',
  templateUrl: './share-header.component.html',
  styleUrls: ['./share-header.component.scss']
})
export class ShareHeaderComponent implements OnInit {

  @Input() headers: IShareHeader [];
  constructor() { }

  ngOnInit(): void {
  }

}
