import { Injectable } from "@angular/core";
import { environment } from "src/environments/environment";
import { IToken, ProductTypeEnum } from "../../view-models/user";
import { IAuthenticatorToken } from "../../view-models/authenticatorToken";
import { CookieService } from "ngx-cookie-service";
import * as jwt_decode from "jwt-decode";

@Injectable()
export class AuthService {
  public authenticatorToken: IAuthenticatorToken;
  public tokenPayload: IToken;

  constructor(private cookieService: CookieService) {}

  logout(redirect?: boolean) {
    const redirectUrl = redirect
      ? environment.autenticatorUrl + "?redirectUrl=" + window.location.href
      : environment.autenticatorUrl;
    this.cookieService.deleteAll(redirectUrl);
    window.location.href = redirectUrl;
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
    this.authenticatorToken = this.cookieService.check("access-token-cs")
      ? JSON.parse(this.cookieService.get("access-token-cs"))
      : null;
    return this.authenticatorToken ? this.authenticatorToken.token : null;
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
        "access-token-cs",
        JSON.stringify(this.authenticatorToken)
      );
      this.decodePayloadJWT();
    }
  }

  formatDate(date: Date) {
    const newDate = new Date(date.toLocaleDateString("en-US"));
    return newDate;
  }

  getUserToken() {
    this.decodePayloadJWT();
    if (this.tokenPayload) {
      return this.tokenPayload.user;
    }
    return null;
  }
  getProductToken() {
    this.decodePayloadJWT();
    if (this.tokenPayload) {
      return this.tokenPayload.products;
    }
    return null;
  }
  retrieveAuthenticatorToken() {
    this.fakeLogin();
    if (!this.authenticatorToken) {
      this.authenticatorToken = this.cookieService.check("access-token-cs")
        ? JSON.parse(this.cookieService.get("access-token-cs"))
        : null;
    }
  }

  canAccessProduct(productType: ProductTypeEnum) {
    const products: ProductTypeEnum[] = this.cookieService.check("products")
      ? JSON.parse(this.cookieService.get("products"))
      : [];
    return products && products.some((product) => product === productType);
  }
}
