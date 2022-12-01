import { NgxSpinnerService } from "ngx-spinner";
import { Injectable } from "@angular/core";
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
} from "@angular/common/http";
import { Observable } from "rxjs";
import { TokenService } from "../services/token/token.service";

@Injectable({
  providedIn: "root",
})
export class HttpApiInterceptor implements HttpInterceptor {
  constructor(
    private spinner: NgxSpinnerService,
    private tokenService: TokenService
  ) {}
  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    if (!this.tokenService.isMenuRequest) this.spinner.show();
    let token = this.tokenService.getToken();
    if (token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      });
    }
    return next.handle(request);
  }
}
