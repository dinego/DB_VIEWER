import { NgxSpinnerService } from "ngx-spinner";
import { ToastrService } from "ngx-toastr";
import { Injectable } from "@angular/core";
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse,
} from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";
import { catchError } from "rxjs/operators";

/**
 * Prefixes all requests not starting with `http[s]` with `environment.serverUrl`.
 */
@Injectable({
  providedIn: "root",
})
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private toastr: ToastrService,
    private spinner: NgxSpinnerService
  ) {}
  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    return next
      .handle(request)
      .pipe(catchError((error) => this.errorHandler(error)));
  }

  // Customize the default error handler here if needed
  private errorHandler(
    response: HttpErrorResponse
  ): Observable<HttpEvent<any>> {
    // Do something with the error
    if (!environment.production) {
      console.error("Request error", response);
    }
    this.spinner.hide();
    const error =
      response.error && Array.isArray(response.error)
        ? response.error.map((m: { message: any }) => m.message)
        : response.error
        ? response.error.message
        : "Ocorreu um erro inesperado. Se o problema persistir informe o administrador";
    this.toastr.error(error, "Erro");
    if (response.status === 401) {
      setTimeout(() => {
        window.location.href = environment.autenticatorUrl;
      }, 3000);
    }
    throw response;
  }
}
