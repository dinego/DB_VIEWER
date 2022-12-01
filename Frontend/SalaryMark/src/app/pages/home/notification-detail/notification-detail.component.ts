import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

import { NgxSpinnerService } from 'ngx-spinner';
import { NotificationService } from '../../../shared/services/notification/notification.service';

@Component({
  templateUrl: './notification-detail.component.html',
  styleUrls: ['./notification-detail.component.scss']
})
export class NotificationDetailComponent implements OnInit {
  title: string;
  description: string;
  create: string;
  id: number;
  isRead: boolean;

  constructor(
    public bsModalRef: BsModalRef,
    private notificationService: NotificationService,
    private ngxSpinnerService: NgxSpinnerService
  ) {}

  async ngOnInit() {
    if (this.isRead || this.notificationService.reads.includes(this.id)) {
      return;
    }

    try {
      this.ngxSpinnerService.show();
      await this.notificationService
        .put({ notificationId: this.id })
        .toPromise();

      this.notificationService.amountNotRead--;
      this.notificationService.reads.push(this.id);
    } catch (err) {
      console.error(err);
    } finally {
      this.ngxSpinnerService.hide();
    }
  }
}
