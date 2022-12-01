import { Component, OnInit } from '@angular/core';

import locales from '@/locales/positioning';
import routerNames from '@/shared/routerNames';
import { Modules, SubModules } from '@/shared/models/modules';
import { TokenService } from '@/shared/services/token/token.service';

@Component({
  selector: 'app-positioning',
  templateUrl: './positioning.component.html',
  styleUrls: ['./positioning.component.scss']
})
export class PositioningComponent implements OnInit {

  public locales = locales;
  public routerNames = routerNames;
  subModules = SubModules;

  constructor(private tokenService: TokenService) { }

  ngOnInit(): void {
  }

  canAccess(subModule: SubModules) {
    const res = this.tokenService.validateModules(Modules.positioning, subModule);
    return res;
  }

}
