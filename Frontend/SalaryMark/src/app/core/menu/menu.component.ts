import { NgxSpinnerService } from "ngx-spinner";
import { Component, EventEmitter, OnInit, Output } from "@angular/core";

import locales from "@/locales/menu";
import { TokenService } from "@/shared/services/token/token.service";
import { Modules, SubModules } from "@/shared/models/modules";
import { ActivatedRoute, Router } from "@angular/router";
import { environment } from "src/environments/environment";
import { IPermissions } from "@/shared/models/token";
import { UserParameterAccess } from "@/shared/models/user-parameter";
import { UserService } from "@/shared/services/user/user.service";
import { CommonService } from "@/shared/services/common/common.service";

@Component({
  selector: "app-menu",
  templateUrl: "./menu.component.html",
  styleUrls: ["./menu.component.scss"],
})
export class MenuComponent implements OnInit {
  public isShow: boolean;
  public locales = locales;
  public modules = Modules;
  public route = locales.home;
  public share: boolean;
  subModules = SubModules;
  permissions: IPermissions;

  public showMenuControl: boolean;
  public showRolesMenu: boolean;
  public showPositionamentsMenu: boolean;
  public showParametersMenu: boolean;

  userParameterAccess: UserParameterAccess = new UserParameterAccess();

  public siteCsUrl = environment.siteCsUrl;

  @Output() showMenu = new EventEmitter<boolean>();

  constructor(
    private tokenService: TokenService,
    private NgxSpinnerService: NgxSpinnerService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private userService: UserService,
    private commonService: CommonService
  ) {}

  ngOnInit(): void {
    this.permissions = this.tokenService.getPermissions();
    this.loadPermissions();

    this.router.events.subscribe((event: any) => {
      let r = this.activatedRoute;
      while (r.firstChild) {
        r = r.firstChild;
      }
      r.params.subscribe((param) => {
        this.share = param.secretkey;
      });
    });
    this.NgxSpinnerService.hide();
  }

  private async loadPermissions() {
    this.tokenService.isMenuRequest = true;
    this.userParameterAccess.showUsers = await this.userService.canAccessUser();
    this.userParameterAccess.showPjSettings =
      await this.commonService.hidePjSettings();
    this.userParameterAccess.showHourlyBasis =
      await this.commonService.canAccesHourlyBasis();
    this.tokenService.isMenuRequest = false;
  }

  validateMenu(module: Modules): boolean {
    return this.tokenService.validateMenu(module);
  }

  canAccess(subModule: SubModules) {
    return this.tokenService.validateModules(Modules.parameters, subModule);
  }

  canAccessDisplayConfiguration() {
    return this.tokenService.validateDisplayConfigurationModules();
  }

  canAccessPositioning(subModule: SubModules) {
    return this.tokenService.validateModules(Modules.positioning, subModule);
  }

  IsAdmin(): boolean {
    const user = this.tokenService.getTokenUser();
    return user && user.isAdmin;
  }

  closeMenu() {
    this.showMenu.emit(false);
  }

  showCsMenu() {
    return this.tokenService.showCsMenu();
  }
}
