import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { TokenService } from '../services/token/token.service';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private tokenService: TokenService,
    private toastr: ToastrService) { }
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    if (!this.tokenService.isAuthenticated()) {
      this.tokenService.logout();
      this.toastr.error('Parece que seu login expirou e você será redirecionado para a tela de login.', 'Acesso Negado');
      setTimeout(() => {
        window.location.href= environment.autenticatorUrl;
      }, 3000);
      return false;
    }
    return true;
  }
}
