import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { NotificationsList } from "../../models/notification/notifications-list";
import { Notification } from "../../models/notification/notification";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";

@Injectable({
  providedIn: "root",
})
export class NotificationService {
  public amountNotRead = 0;
  public reads: number[] = [];

  constructor(private httpClient: HttpClient) {}

  getAll(data: any): Observable<NotificationsList> {
    const params = new HttpParams({
      fromObject: data,
    });

    return this.httpClient.get<NotificationsList>(
      environment.api.home.getNotifications,
      {
        params,
      }
    );
  }

  getAllNotReaded(): Observable<
    Pick<NotificationsList, Partial<"notifications">>
  > {
    return this.httpClient
      .get<Pick<NotificationsList, Partial<"notifications">>>(
        environment.api.home.getNotificationsNotReaded
      )
      .pipe(
        tap((data) => {
          this.amountNotRead = data.notifications.length;
        })
      );
  }

  delete(notification: Notification) {
    const params = new HttpParams({
      fromObject: {
        notificationId: notification.id.toString(),
      },
    });

    return this.httpClient.delete(
      `${environment.api.home.removeNotificationById}`,
      {
        params,
      }
    );
  }

  put(data: any) {
    return this.httpClient.put(environment.api.home.setReadNotification, data);
  }
}
