import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { TokenService } from '../services/token/token.service';
import { Modules, SubModules } from '../models/modules';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import routerNames from '@/shared/routerNames';
@Injectable({
  providedIn: 'root'
})
export class HomeRedirectGuard implements CanActivate {
  constructor(private tokenService: TokenService,
    private toastr: ToastrService) { }
  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      const module: Modules = next.data['module'];
      const subModule: SubModules = next.data['subModule'];
    if (!this.tokenService.validateModules(module, subModule)) {
      const nextUrlAccess = this.getNextModule();
      if(!nextUrlAccess){
        this.toastr.error('Parece que você não tem acesso à essa página e você será redirecionado para a tela de login.', 'Acesso Negado');
        this.tokenService.logout();
        window.location.href= environment.autenticatorUrl;
        return false;
      }
      window.location.href= nextUrlAccess;
      return false;
    }
    return true;
  }

  getNextModule(): string{
    if(this.tokenService.tokenPayload &&
       this.tokenService.tokenPayload.products &&
       this.tokenService.tokenPayload.products.modules){
      const module: Modules = +this.tokenService.tokenPayload.products.modules.find(x=> x.id > 0);
      switch(module){
        case Modules.dashboard:
          return `${environment.baseUrl}${routerNames.DASHBOARD}`;
        case Modules.positions:
          return `${environment.baseUrl}${routerNames.POSITIONS}`;
        case Modules.home:
          return `${environment.baseUrl}${routerNames.HOME}`;
        case Modules.myReports:
          return `${environment.baseUrl}${routerNames.MY_REPORTS}`;
        case Modules.parameters:
          return `${environment.baseUrl}${routerNames.PARAMETERS}`;
        case Modules.positioning:
          return `${environment.baseUrl}${routerNames.POSITIONING}`;
      }
    }
    return '';
  }
}
