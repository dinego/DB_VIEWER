import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";

import locales from "@/locales/positions";
import { TokenService } from "@/shared/services/token/token.service";
import { Modules, SubModules } from "@/shared/models/modules";
import routerNames from "@/shared/routerNames";

@Component({
  selector: "app-positions",
  templateUrl: "./positions.component.html",
  styleUrls: ["./positions.component.scss"],
})
export class PositionsComponent implements OnInit {
  public locales = locales;
  subModules = SubModules;
  public routerNames = routerNames;

  share: boolean;

  constructor(private route: Router, private tokenService: TokenService) {}

  ngOnInit(): void {
    if (this.route.url.split("/")[4]) {
      this.share = true;
    }
  }

  navigation(patch: string) {
    this.route.navigate([this.route.url, patch]);
  }
  canAccess(subModule: SubModules) {
    const res = this.tokenService.validateModules(Modules.positions, subModule);
    return res;
  }
}
