import { UserParameterService } from "@/shared/services/user-parameter/user-parameter.service";
import { Injectable } from "@angular/core";
import {
  Resolve,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  ActivatedRoute,
  Router,
} from "@angular/router";
import { NgxSpinnerService } from "ngx-spinner";
import { ToastrService } from "ngx-toastr";
import { Observable, of } from "rxjs";
import { catchError, finalize } from "rxjs/operators";
import locales from "./locales/page-not-found";

@Injectable({
  providedIn: "root",
})
export class UserDetailResolverService implements Resolve<any> {
  constructor(
    private userParameterService: UserParameterService,
    private router: Router,
    private ngxSpinnerService: NgxSpinnerService,
    private toastrService: ToastrService
  ) {}

  async executeError(): Promise<Observable<any>> {
    this.router.navigate(["/"]);
    this.toastrService.remove(this.toastrService.currentlyActive);
    this.toastrService.error(locales.pageNotFound, locales.error);

    return of();
  }

  resolve(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<any> | Observable<never> {
    return this.userParameterService.getUserInformation(route.params.id).pipe(
      catchError(() => this.executeError()),
      finalize(() => {
        this.ngxSpinnerService.hide();
      })
    );
  }
}
