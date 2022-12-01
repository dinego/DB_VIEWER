import { Component, OnInit } from '@angular/core';

import locales from '@/locales/positioning';
import routerNames from '@/shared/routerNames';
import { TokenService } from '@/shared/services/token/token.service';
import { Modules, SubModules } from '@/shared/models/modules';
import { Router } from '@angular/router';

@Component({
  selector: 'app-positioning-nav-header',
  templateUrl: './positioning-nav-header.component.html',
  styleUrls: ['./positioning-nav-header.component.scss']
})
export class PositioningNavHeaderComponent implements OnInit {

  public locales = locales;
  public routerNames = routerNames;
  public share: boolean;
  subModules = SubModules;

  constructor(private router: Router,
    private tokenService: TokenService) { }

  ngOnInit(): void {
    if(this.router.url.split("/")[4]) {
      this.share = true;
    }
  }

  canAccess(subModule: SubModules){
    return this.tokenService.validateModules(Modules.positioning, subModule);
  }
}
