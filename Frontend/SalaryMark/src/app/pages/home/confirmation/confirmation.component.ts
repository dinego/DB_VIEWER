import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Subject } from 'rxjs';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  templateUrl: './confirmation.component.html',
  styleUrls: ['./confirmation.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ConfirmationComponent implements OnInit {
  result: Subject<boolean> = new Subject<boolean>();

  title: string;
  closeBtnName: string;
  confirmBtnName: string;
  content: string;

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit() {
    this.result = new Subject();
  }

  confirm(): void {
    this.result.next(true);
    this.bsModalRef.hide();
  }

  decline(): void {
    this.result.next(false);
    this.bsModalRef.hide();
  }
}
