import {
  IAccessToken,
  IAuthenticatorToken,
} from "../../models/authenticatorToken";
import { IToken, ProductTypeEnum } from "../../models/token";
import { Injectable } from "@angular/core";
import * as jwt_decode from "jwt-decode";
import { environment } from "src/environments/environment";
import { Modules, SubModules } from "@/shared/models/modules";
import { CookieService } from "ngx-cookie-service";
import { isUndefined } from "@/shared/common/functions";

@Injectable({
  providedIn: "root",
})
export class TokenService {
  public authenticatorToken: IAuthenticatorToken;
  public tokenPayload: IToken;
  public isMenuRequest: boolean;

  constructor(private cookieService: CookieService) {}

  decodePayloadJWT() {
    try {
      const jwtToken = jwt_decode(this.getToken());
      if (jwtToken && jwtToken.products) {
        jwtToken.products = JSON.parse(jwtToken.products);
      }
      if (jwtToken && jwtToken.user) {
        jwtToken.user = JSON.parse(jwtToken.user);
      }
      this.tokenPayload = jwtToken;
    } catch (Error) {
      this.tokenPayload = null;
    }
  }

  getToken(): string {
    if (this.authenticatorToken) {
      return this.authenticatorToken.token;
    }
    this.authenticatorToken = this.cookieService.check("access-token-sm")
      ? JSON.parse(this.cookieService.get("access-token-sm"))
      : null;
    return this.authenticatorToken?.token;
  }
  setTokenStorage(result: IAccessToken) {
    this.cookieService.set(
      "access-token-sm",
      JSON.stringify(result.accessTokenSM)
    );
    this.authenticatorToken = result.accessTokenSM;
  }

  retrieveAuthenticatorToken() {
    this.fakeLogin();
    if (!this.authenticatorToken) {
      this.authenticatorToken = this.cookieService.check("access-token-sm")
        ? JSON.parse(this.cookieService.get("access-token-sm"))
        : null;
    }
  }

  isAuthenticated(): boolean {
    this.retrieveAuthenticatorToken();
    const date = new Date();
    if (this.authenticatorToken) {
      const expiresAt = new Date(this.authenticatorToken.expiresAt);
      return (
        this.authenticatorToken.isAuthenticated &&
        this.formatDate(expiresAt) >= this.formatDate(date)
      );
    }
    return false;
  }

  formatDate(date: Date) {
    const newDate = new Date(date.toLocaleDateString("en-US"));
    return newDate;
  }

  getTokenUser() {
    this.decodePayloadJWT();
    if (this.tokenPayload) {
      return this.tokenPayload.user;
    }
    return null;
  }
  getFirstAccess(): boolean {
    this.decodePayloadJWT();
    if (this.tokenPayload && this.tokenPayload.user) {
      return this.tokenPayload.user.isFirstAccess;
    }
    return false;
  }
  fakeLogin() {
    if (!environment.production) {
      this.authenticatorToken = {
        token: environment.fakeToken,
        expiresAt: new Date(),
        isAuthenticated: true,
      };
      this.cookieService.deleteAll();
      this.cookieService.set(
        "access-token-sm",
        JSON.stringify(this.authenticatorToken)
      );
      this.cookieService.set(
        "products",
        JSON.stringify([ProductTypeEnum.CS, ProductTypeEnum.SM])
      );
      this.decodePayloadJWT();
    }
  }

  validateMenu(module: Modules): boolean {
    const payload = this.tokenPayload;
    if (payload) {
      const menuModules = payload.products.modules.find((m) => m.id === module);
      return menuModules ? true : false;
    }
    return false;
  }

  validateModules(module: Modules, subModule: SubModules): boolean {
    const payload = this.tokenPayload;
    if (payload) {
      const userModules = payload.products.modules.find((m) => m.id === module);
      if (userModules && subModule !== SubModules.none) {
        return userModules.subItems.includes(subModule);
      }
      return !isUndefined(userModules);
    }
    return false;
  }

  validateDisplayConfigurationModules(): boolean {
    const payload = this.tokenPayload;
    if (payload) {
      return payload.products.modules.some(
        (m) =>
          m.id === Modules.parameters &&
          m.subItems.filter(
            (sb) =>
              sb == SubModules.displayConfiguration ||
              sb == SubModules.globalLabels
          ).length > 0
      );
    }
    return false;
  }

  getPermissions() {
    this.decodePayloadJWT();
    if (this.tokenPayload && this.tokenPayload.products) {
      return this.tokenPayload.products.permissions;
    }
    return null;
  }

  logout() {
    window.location.href = environment.autenticatorUrl;
  }

  showCsMenu() {
    if (this.cookieService.check("products")) {
      const products: ProductTypeEnum[] = JSON.parse(
        this.cookieService.get("products")
      );
      return products.some((product) => product == ProductTypeEnum.CS);
    }
    return false;
  }
}
