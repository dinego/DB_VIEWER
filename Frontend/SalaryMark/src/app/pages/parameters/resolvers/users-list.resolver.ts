import { UserParameterService } from '@/shared/services/user-parameter/user-parameter.service';
import { Injectable } from '@angular/core';
import {
  Resolve,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class UserListResolverService implements Resolve<any> {
  constructor(
    private userParameterService: UserParameterService,
    private ngxSpinnerService: NgxSpinnerService
  ) {}
  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Observable<never> {
    return this.userParameterService.getUserParameters(1).pipe(
      finalize(() => {
        this.ngxSpinnerService.hide();
      })
    );
  }
}
