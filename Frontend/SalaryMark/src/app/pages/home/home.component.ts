import {
  Component,
  OnInit,
  OnDestroy,
  ChangeDetectorRef,
  HostListener,
  ViewChild,
  TemplateRef,
  ElementRef,
} from "@angular/core";
import { ConfirmationComponent } from "./confirmation/confirmation.component";
import { Subscription } from "rxjs";
import { NotificationsList } from "../../shared/models/notification/notifications-list";
import { Notification } from "../../shared/models/notification/notification";
import { NotificationDetailComponent } from "./notification-detail/notification-detail.component";
import { NgxSpinnerService } from "ngx-spinner";
import { BsModalService, BsModalRef } from "ngx-bootstrap/modal";
import { NotificationService } from "../../shared/services/notification/notification.service";
import { YoutubeService } from "../../shared/services/youtube/youtube.service";
import { environment } from "src/environments/environment";
import { VideoItem } from "../../shared/models/youtube/videos/item";
import { VideoDetailComponent } from "./video-detail/video-detail.component";
import { IUser } from "@/shared/models/token";
import { UserService } from "@/shared/services/user/user.service";

import SwiperCore, {
  Navigation,
  Pagination,
  Scrollbar,
  A11y,
  SwiperOptions,
} from "swiper";

import breakpoints from "./breakpoints";
import { SwiperComponent } from "swiper/angular";

SwiperCore.use([Navigation, Pagination, Scrollbar, A11y]);

@Component({
  selector: "app-home",
  templateUrl: "./home.component.html",
  styleUrls: ["./home.component.scss"],
})
export class HomeComponent implements OnInit, OnDestroy {
  @ViewChild("swiperRef", { static: false }) sliderRef?: SwiperComponent;
  @ViewChild("defineWidthMsg", { static: false })
  defineWidthMsg?: ElementRef;
  @ViewChild("slideItemRef") slideItem: TemplateRef<Element>;

  public notificationsList: NotificationsList = new NotificationsList();
  public call: Subscription;
  private params = {
    page: 1,
    pageSize: 20,
  };
  public allowRequestOnScroll = true;
  public videos: VideoItem[] = [];
  public videosPodcasts: VideoItem[] = [];
  public collapsedNotifications: Notification[] = [];
  public modalRef: BsModalRef;
  public user: IUser;
  public isMobileView: boolean;

  public itemsToSlide: any[];
  public swiperIndex = 0;

  public swiperConfig: SwiperOptions = breakpoints;

  constructor(
    private notificationService: NotificationService,
    private userService: UserService,
    private youtubeService: YoutubeService,
    private bsModalService: BsModalService,
    private ngxSpinnerService: NgxSpinnerService,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.retrieveUser();
    this.getNotifications();
    this.getYoutubeVideos();
  }

  @HostListener("window:resize", ["$event"])
  onResize(event) {
    this.isMobileView = window.innerWidth > 768 ? false : true;
    this.changeDetectorRef.detectChanges();
    this.changeDetectorRef.markForCheck();
    this.sliderRef.swiperRef.update();

    this.setSizeWidthMessage();
  }

  setSizeWidthMessage() {
    setTimeout(() => {
      document
        .getElementsByClassName("mySwiper")[0]
        .setAttribute(
          "style",
          `width: ${this.defineWidthMsg.nativeElement.offsetWidth}px; padding-right: 1px`
        );
    }, 100);
  }

  ngAfterViewChecked() {
    this.changeDetectorRef.detectChanges();
    this.changeDetectorRef.markForCheck();
    this.sliderRef.swiperRef.update();
  }
  ngAfterViewInit() {
    this.setSizeWidthMessage();
  }

  onSwiper(event) {
    this.swiperIndex = event.activeIndex;
  }

  onSlideChange(event) {
    this.swiperIndex = event.activeIndex;
    this.changeDetectorRef.detectChanges();
    this.changeDetectorRef.markForCheck();
  }

  gotToSlide(index: number) {
    this.sliderRef.swiperRef.slideTo(index);
  }

  retrieveUser() {
    this.userService.changeUser.subscribe((user: IUser) => {
      this.user = user;
    });
  }

  trackByIndex(index: number) {
    return index;
  }

  getYoutubeVideos() {
    this.youtubeService
      .getVideosFromPlaylist({
        playlistId: environment.youtube.playlistIdWebinar,
        part: "snippet",
        maxResults: 8,
      })
      .subscribe((data) => {
        this.videos = data.items.filter((y) => y.snippet.thumbnails.default);
      });

    this.youtubeService
      .getVideosFromPlaylist({
        playlistId: environment.youtube.playlistIdPodcasts,
        part: "snippet",
        maxResults: 8,
      })
      .subscribe((data) => {
        this.videosPodcasts = data.items.filter(
          (y) => y.snippet.thumbnails.default
        );
      });
  }

  public getNotifications() {
    this.ngxSpinnerService.show();
    this.allowRequestOnScroll = false;
    this.cancelRequest();

    const obs = this.notificationService.getAll(this.params);

    this.call = obs.subscribe((data) => {
      data.notifications = [
        ...this.notificationsList.notifications,
        ...data.notifications,
      ];

      this.notificationsList = data;
      this.params.page++;
      this.allowRequestOnScroll =
        this.notificationsList.notifications.length < data.amount;

      this.ngxSpinnerService.hide();
    });

    return obs;
  }

  openVideoDetail(videoItem: VideoItem) {
    this.modalRef = this.bsModalService.show(VideoDetailComponent, {
      class: "modal-video-sm",
      initialState: { videoItem },
    });
  }

  public onScrollDown() {
    if (!this.allowRequestOnScroll) {
      return;
    }

    this.getNotifications();
  }

  public cancelRequest() {
    if (this.call) {
      this.call.unsubscribe();
    }
  }

  ngOnDestroy() {
    this.cancelRequest();
  }

  resetPagination() {
    this.params = {
      page: 1,
      pageSize: 20,
    };
  }

  openDetailNotification(notification: Notification) {
    this.modalRef = this.bsModalService.show(NotificationDetailComponent, {
      class: "modal-dialog-centered",
      initialState: notification,
    });

    notification.isRead = true;
  }

  isNotificationsNotReaded(notification: Notification) {
    return (
      !notification.isRead &&
      !this.notificationService.reads.includes(notification.id)
    );
  }

  deleteNotification(notification: Notification) {
    const initialState = {
      title: "Você deseja realmente excluir essa mensagem ?",
      closeBtnName: "Cancelar",
      confirmBtnName: "Confirmar",
    };

    this.modalRef = this.bsModalService.show(ConfirmationComponent, {
      class: "modal-dialog-centered",
      initialState,
    });

    this.modalRef.content.result.subscribe(async (result: boolean) => {
      if (result) {
        await this.notificationService.delete(notification).toPromise();
        this.resetPagination();
        this.notificationsList = new NotificationsList();
        this.getNotifications();
      }
    });
  }

  replaceUserName(name: string) {
    return name.replace(/([a-z]+) .* ([a-z]+)/i, "$1").split(" ")[0];
  }

  handleOverflowOfContent(text: string, maxLength: number) {
    text = text.replace("\n", "").replace("<br />", "");
    return text.length > maxLength
      ? `${text.substring(0, maxLength)}...`
      : text;
  }

  handleNotificationCollapsedState(
    notification: Notification,
    isOpen: boolean
  ) {
    if (isOpen) {
      notification.description = notification.isOpened
        ? notification.description
        : this.applyLink(notification.description);

      notification.isOpened = true;
      this.collapsedNotifications.push(notification);
      this.markNotificationAsRead(notification);
      this.getNotifications();
    } else {
      this.collapsedNotifications = this.collapsedNotifications.filter(
        (notificationCollapsed) => notificationCollapsed.id !== notification.id
      );
    }
  }

  applyLink(content: string): string {
    const replaceMail: RegExp =
      /(([a-zA-Z0-9\-\_\.])+@[a-zA-Z\_]+?(\.[a-zA-Z]{2,6})+)/gim;
    const replaceWww: RegExp = /(^|[^\/])(www\.[\S]+(\b|$))/gim;
    const replaceHttpContent: RegExp =
      /(\b(https?|ftp):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/gim;

    let replacedText: string = "";

    replacedText = content.replace(
      replaceHttpContent,
      '<a href="$1" target="_blank">$1</a>'
    );

    replacedText = replacedText.replace(
      replaceWww,
      '$1<a href="http://$2" target="_blank">$2</a>'
    );

    replacedText = replacedText.replace(
      replaceMail,
      '<a href="mailto:$1">$1</a>'
    );

    replacedText = replacedText.replace(/\n/g, "<br />");

    return replacedText;
  }

  async markNotificationAsRead(notification: Notification) {
    if (
      notification.isRead ||
      this.notificationService.reads.includes(notification.id)
    ) {
      return;
    }

    try {
      this.ngxSpinnerService.show();
      await this.notificationService
        .put({ notificationId: notification.id })
        .toPromise();

      this.notificationService.amountNotRead--;
      this.notificationService.reads.push(notification.id);
    } catch (err) {
      console.error(err);
    } finally {
      this.ngxSpinnerService.hide();
    }

    notification.isRead = true;
  }

  isNotificationCollapsed(notification: Notification): boolean {
    return !!this.collapsedNotifications.find(
      (notificationCollapsed) => notificationCollapsed.id === notification.id
    );
  }

  getTotalNotReaded() {
    return this.notificationsList.notifications.filter((n) => !n.isRead).length;
  }
}
