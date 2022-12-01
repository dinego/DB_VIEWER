import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { TokenService } from '../services/token/token.service';
import { Modules, SubModules } from '../models/modules';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class CanAccessGuard implements CanActivate {
  constructor(private tokenService: TokenService,
    private router: Router,
    private toastr: ToastrService) { }
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      const module: Modules = next.data['module'];
      const subModule: SubModules = next.data['subModule'];
    if (!this.tokenService.validateModules(module, subModule)) {
      this.toastr.error('Parece que você não tem acesso à essa página e você será redirecionado para a tela de login.', 'Acesso Negado');
      this.tokenService.logout();
      window.location.href= environment.autenticatorUrl;
      return false;
    }
    return true;
  }
}
