import locales from "@/locales/parameters";
import routerNames from "@/shared/routerNames";
import { TokenService } from "@/shared/services/token/token.service";
import { Modules, SubModules } from "@/shared/models/modules";
import { NgxSpinnerService } from "ngx-spinner";
import { IPermissions } from "@/shared/models/token";
import { UserParameterAccess } from "@/shared/models/user-parameter";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";

@Component({
  selector: "app-parameters",
  templateUrl: "./parameters.component.html",
  styleUrls: ["./parameters.component.scss"],
})
export class ParametersComponent implements OnInit {
  routerNames = routerNames;
  locales = locales;
  subModules = SubModules;
  permissions: IPermissions;
  userParameterAccess: UserParameterAccess = new UserParameterAccess();

  constructor(
    private tokenService: TokenService,
    private ngxSpinnerService: NgxSpinnerService,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit() {
    this.permissions = this.tokenService.getPermissions();

    this.userParameterAccess.showPjSettings =
      this.activatedRoute.snapshot.data.showPjSettings;
    this.userParameterAccess.showHourlyBasis =
      this.activatedRoute.snapshot.data.showHourlyBasis;
    this.userParameterAccess.showUsers =
      this.activatedRoute.snapshot.data.showUsers;
    this.setStorage();
    this.ngxSpinnerService.hide();
  }

  canAccess(subModule: SubModules) {
    return this.tokenService.validateModules(Modules.parameters, subModule);
  }

  canAccessDisplayConfiguration() {
    return this.tokenService.validateDisplayConfigurationModules();
  }

  setStorage() {
    localStorage.removeItem("user-parameter-access");
    localStorage.setItem(
      "user-parameter-access",
      JSON.stringify(this.userParameterAccess)
    );
  }
}
