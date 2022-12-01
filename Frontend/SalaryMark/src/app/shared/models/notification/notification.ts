export class Notification {
  id: number;
  title: string;
  description: string;
  create: string;
  isRead: boolean;
  isOpened: boolean;

  constructor(notifications?: Notification) {
    Object.assign(this, notifications);
  }
}
