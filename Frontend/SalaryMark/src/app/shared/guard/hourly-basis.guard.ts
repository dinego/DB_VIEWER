import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot } from '@angular/router';
import { CommonService } from '../services/common/common.service';

@Injectable({
  providedIn: 'root'
})
export class HourlyBasisGuard implements CanActivate {
  constructor(private commonService: CommonService) { }
  async canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Promise<boolean> {
    return await this.commonService.canAccesHourlyBasis();
  }
}
