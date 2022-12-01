import { UserService } from '@/shared/services/user/user.service';
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
export class CanAccessUserResolver implements Resolve<boolean> {
  constructor(
    private userService: UserService,
    private ngxSpinnerService: NgxSpinnerService
  ) {}
  async resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Promise<boolean> {
    const canAccessUser = await this.userService.canAccessUser();
    this.ngxSpinnerService.hide();
    return canAccessUser;
  }
}
