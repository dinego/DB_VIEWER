import { ContractTypeEnum } from '@/shared/interfaces/positions';
import { PjAccess } from '@/shared/models/pj-setting';
import { CommonService } from '@/shared/services/common/common.service';
import { Injectable } from '@angular/core';
import {
  Resolve,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';
import { isClass } from 'highcharts';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root',
})
export class PjSettingsResolver implements Resolve<boolean> {
  constructor(
    private commonService: CommonService,
    private ngxSpinnerService: NgxSpinnerService
  ) {}
  async resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Promise<boolean> {
    const hide = await this.commonService.hidePjSettings();
    this.ngxSpinnerService.hide();
    return hide;
  }
}
