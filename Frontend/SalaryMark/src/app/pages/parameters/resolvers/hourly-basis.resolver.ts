import { CommonService } from '@/shared/services/common/common.service';
import { Injectable } from '@angular/core';
import {
  Resolve,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root',
})
export class HourlyBasisResolver implements Resolve<boolean> {
  constructor(
    private commonService: CommonService,
    private ngxSpinnerService: NgxSpinnerService
  ) {}
  async resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Promise<boolean> {
    const hourlyBasis = await this.commonService.canAccesHourlyBasis();
    this.ngxSpinnerService.hide();
    return hourlyBasis;
  }
}
