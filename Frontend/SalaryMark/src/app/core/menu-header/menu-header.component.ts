import {
  Component,
  EventEmitter,
  HostListener,
  Input,
  OnInit,
  Output,
} from "@angular/core";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { NgxSpinnerService } from "ngx-spinner";

import locales from "@/locales/menu";
import { NotificationService } from "@/shared/services/notification/notification.service";
import { NotificationsList } from "@/shared/models/notification/notifications-list";
import { Notification } from "@/shared/models/notification/notification";
import { NotificationDetailComponent } from "@/pages/home/notification-detail/notification-detail.component";
import { UserService } from "@/shared/services/user/user.service";
import { IUser } from "@/shared/models/token";
import { ContactUs } from "@/shared/models/contact-us";
import { ActivatedRoute, Router } from "@angular/router";
import { TokenService } from "@/shared/services/token/token.service";
import { environment } from "../../../environments/environment";
import routerNames from "@/shared/routerNames";

@Component({
  selector: "app-menu-header",
  templateUrl: "./menu-header.component.html",
  styleUrls: ["./menu-header.component.scss"],
})
export class MenuHeaderComponent implements OnInit {
  public locales = locales;
  public isOpenChangePassword: boolean;
  public isOpenContactUs: boolean;
  public isOpen: boolean;
  public isOpenProfilePhoto: boolean;
  public notificationIsOpen = false;
  public byPassCloseNotifications = true;
  public byPassCloseMenuProfile = true;
  name: string;
  notificationsList: Pick<NotificationsList, Partial<"notifications">> =
    new NotificationsList();
  public modalRef: BsModalRef;
  public photo: Promise<string>;
  public user: IUser;
  public share: boolean;

  public screenWidth: number;

  initials?: string;
  photoString: string = "";

  @Input() showMenuControl: boolean = false;

  @Output() showMenu = new EventEmitter<boolean>();

  constructor(
    private bsModalService: BsModalService,
    private ngxSpinnerService: NgxSpinnerService,
    public notificationService: NotificationService,
    private userService: UserService,
    private route: ActivatedRoute,
    private tokenService: TokenService,
    private router: Router
  ) {}

  @HostListener("window:resize", ["$event"])
  onResize(event) {
    this.getSizesAndSetMobile();
  }

  ngOnInit(): void {
    this.getSizesAndSetMobile();

    this.router.events.subscribe((event: any) => {
      let r = this.route;
      while (r.firstChild) {
        r = r.firstChild;
      }
      r.params.subscribe((param) => {
        this.share = param.secretkey;
      });
    });
    setTimeout(() => {
      if (!this.share) {
        this.getNotifications();
        this.retrieveUser();
        this.photo = this.userService.retrieveUserPhoto();

        this.photoString = "";
        this.photo.then((e) => {
          this.photoString = e;
        });
      }
    }, 10);

    const clickNotificationsListener = (e: any) => {
      if (document.getElementById("notifications")) {
        if (
          !document.getElementById("notifications").contains(e.target) &&
          this.notificationIsOpen &&
          !this.byPassCloseNotifications
        ) {
          document.getElementById("btn-notifications").click();
          this.byPassCloseNotifications = true;
        }

        this.byPassCloseNotifications = false;
      }
    };

    window.addEventListener("click", clickNotificationsListener);

    const clickMenuProfileListener = (e: any) => {
      if (document.getElementById("menu-profile")) {
        if (
          !document.getElementById("menu-profile").contains(e.target) &&
          this.isOpen &&
          !this.byPassCloseMenuProfile
        ) {
          document.getElementById("btn-menu-profile").click();
          this.byPassCloseMenuProfile = true;
        }

        this.byPassCloseMenuProfile = false;
      }
    };

    window.addEventListener("click", clickMenuProfileListener);
  }

  clickMenuProfile() {
    this.isOpen = !this.isOpen;
    this.byPassCloseMenuProfile = true;
  }

  onOpenProfilePhoto() {
    this.isOpen = false;
    this.isOpenProfilePhoto = true;
  }
  onCloseProfilePhoto() {
    this.isOpenProfilePhoto = false;
  }

  onOpenChangePassword() {
    this.isOpen = false;
    this.isOpenChangePassword = true;
  }
  onCloseChangePassword() {
    this.isOpenChangePassword = false;
  }

  onOpenContactUs() {
    this.isOpen = false;
    this.isOpenContactUs = true;
  }
  onCloseContactUs() {
    this.isOpenContactUs = false;
  }

  getSizesAndSetMobile() {
    this.screenWidth = window.innerWidth;
  }

  get firstsNotificationsNotReaded() {
    return this.notificationsList.notifications
      .filter(
        (notification) =>
          !this.notificationService.reads.includes(notification.id)
      )
      .slice(0, 3);
  }
  onOpenNotifications() {
    this.notificationIsOpen = !this.notificationIsOpen;
    this.byPassCloseNotifications = true;
  }
  openDetailNotification(notification: Notification) {
    this.modalRef = this.bsModalService.show(NotificationDetailComponent, {
      class: "modal-dialog-centered",
      initialState: notification,
    });

    notification.isRead = true;
  }
  getNotifications() {
    this.notificationService.getAllNotReaded().subscribe((data) => {
      this.notificationsList.notifications = data.notifications;
      this.ngxSpinnerService.hide();
    });
  }

  retrieveUser() {
    this.userService.changeUser.subscribe((user: IUser) => {
      this.user = user;

      if (user && user.simulated) {
        this.name = `${(this.name = user
          ? this.retrieveFirstName(user.name)
          : "")} - Simulado `;
      } else {
        this.name = user ? this.replaceUserName(user.name) : "";
      }
      this.initials = this.getInitialName(user.name);
    });
  }

  onSavePhoto(formData: FormData) {
    formData ? this.updateProfilePhoto(formData) : this.removeProfilePhoto();
  }

  updateProfilePhoto(photo: FormData) {
    this.userService.updateProfilePhoto(photo).subscribe((res) => {
      this.photo = this.userService.retrieveUserPhoto();
      this.ngxSpinnerService.hide();
    });
  }

  removeProfilePhoto() {
    this.userService.removeProfilePhoto().toPromise();
  }

  saveSupport(params: ContactUs) {
    this.ngxSpinnerService.show();
    this.userService.saveContactUs(params).subscribe(
      (res) => {
        this.ngxSpinnerService.hide();
      },
      (err) => {}
    );
  }
  logout() {
    this.tokenService.logout();
  }
  goToConsultSalary() {
    window.open(environment.siteCsUrl, "_blank");
  }

  goToHome() {
    //this.ngxSpinnerService.show();
    setTimeout(() => {
      this.router.navigate([`/${routerNames.HOME}/`]);
      this.ngxSpinnerService.hide();
    }, 100);
  }
  showCsMenu() {
    return this.tokenService.showCsMenu();
  }

  changeShowMenu() {
    this.showMenuControl = !this.showMenuControl;
    this.showMenu.emit(this.showMenuControl);
  }

  goToZendesk() {
    window.open(environment.zendeskUrl, "_self");
  }

  getInitialName(name: string) {
    return name
      .match(/(^\S\S?|\b\S)?/g)
      .join("")
      .match(/(^\S|\S$)?/g)
      .join("")
      .toUpperCase();
  }

  replaceUserName(name: string) {
    return name.replace(/([a-z]+) .* ([a-z]+)/i, "$1 $2");
  }
  retrieveFirstName(name: string) {
    return name.split(/(\s).+/).join("");
  }

  onRemovePhoto() {
    this.photoString = "";
  }

  onSetUserPhoto(event) {
    this.photoString = event;
  }
}
