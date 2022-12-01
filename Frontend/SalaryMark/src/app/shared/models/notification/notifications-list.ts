import { Notification } from './notification';

export class NotificationsList {
  notifications: Notification[] = [];
  amount: number;
  amountNotRead: number;
  currentPage: number;
  sizePage: number;

  constructor(notificationsList?: NotificationsList) {
    Object.assign(this, notificationsList);
  }
}
