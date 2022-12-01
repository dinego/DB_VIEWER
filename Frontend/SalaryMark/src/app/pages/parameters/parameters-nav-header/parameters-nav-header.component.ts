import { Component, OnInit, ChangeDetectionStrategy } from "@angular/core";

import locales from "@/locales/parameters";
import routerNames from "@/shared/routerNames";
import { TokenService } from "@/shared/services/token/token.service";
import { Modules, SubModules } from "@/shared/models/modules";
import { NgxSpinnerService } from "ngx-spinner";
import { IPermissions } from "@/shared/models/token";
import { UserParameterAccess } from "@/shared/models/user-parameter";

@Component({
  selector: "app-parameters-nav-header",
  templateUrl: "./parameters-nav-header.component.html",
  styleUrls: ["./parameters-nav-header.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ParametersNavHeaderComponent implements OnInit {
  routerNames = routerNames;
  locales = locales;
  subModules = SubModules;
  userParameterAccess: UserParameterAccess;
  permissions: IPermissions;

  constructor() {}

  ngOnInit() {}
}
