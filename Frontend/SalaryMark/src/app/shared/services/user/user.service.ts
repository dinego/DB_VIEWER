import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, Observable } from "rxjs";

import {
  GenerateLinkAccessFromUserParameter,
  IUser,
} from "@/shared/models/token";
import { ContactUs } from "@/shared/models/contact-us";

import { TokenService } from "../token/token.service";

import { environment } from "src/environments/environment";
import { AuthService } from "../auth/auth.service";

@Injectable({
  providedIn: "root",
})
export class UserService {
  public user: IUser = null;
  public changeUser = new BehaviorSubject<IUser>(null);
  public isFirstAccess: boolean = false;
  public canAccessUsers?: boolean = null;
  constructor(
    private httpClient: HttpClient,
    private tokenService: TokenService,
    private authService: AuthService
  ) {
    this.setUser();
  }

  setUser() {
    const user: IUser = this.tokenService.getTokenUser();
    this.changeUser.next(user);
  }

  updateUserPassword(
    password: string,
    confirmPassword: string
  ): Observable<any> {
    return this.httpClient.post(environment.api.account.resetPassword, {
      newPassword: password,
      confirmPassword,
    });
  }

  updateProfilePhoto(file: FormData | null): Observable<any> {
    return this.httpClient.put(environment.api.home.uploadPhoto, file);
  }
  removeProfilePhoto(): Observable<any> {
    return this.httpClient.put(environment.api.home.removePhoto, null);
  }

  saveContactUs(params: ContactUs): Observable<any> {
    return this.httpClient.put(environment.api.home.contactUs, {
      attachment: null,
      subject: `${params.attachment} - ${params.subject}`,
      message: params.message,
    });
  }

  generateLinkAccess(
    generateLinkAccessFromUserParameter: GenerateLinkAccessFromUserParameter
  ): Observable<string> {
    return this.httpClient.post<string>(
      environment.api.account.generateLinkAccess,
      generateLinkAccessFromUserParameter
    );
  }

  async retrieveUserPhoto() {
    const userPhoto = await this.httpClient
      .get<string>(environment.api.account.getUserPhoto)
      .toPromise();

    return userPhoto && userPhoto.length > 0
      ? "data:image/png;base64," + userPhoto
      : null;
  }

  convertToImage(dataURI: string): File {
    if (!dataURI) {
      return null;
    }
    const byteString: string = window.atob(dataURI);
    const arrayBuffer: ArrayBuffer = new ArrayBuffer(byteString.length);
    const int8Array: Uint8Array = new Uint8Array(arrayBuffer);
    for (let i = 0; i < byteString.length; i++) {
      int8Array[i] = byteString.charCodeAt(i);
    }
    const blob = new Blob([int8Array], { type: "image/png" });
    const file = new File([blob], Math.random().toString(16).slice(2), {
      type: blob.type,
    });
    return file;
  }

  getFirstAccess(): boolean {
    const user: IUser = this.authService.getUserToken();

    const isFirstAccess = this.isFirstAccess
      ? this.isFirstAccess
      : user && user.isFirstAccess
      ? user.isFirstAccess
      : false;

    return isFirstAccess;
  }

  changeFirstAccess(data?: any) {
    return this.httpClient.put(environment.SetNotFirstAccess, data);
  }

  async canAccessUser() {
    if (this.canAccessUsers == null) {
      this.canAccessUsers = await this.httpClient
        .get<boolean>(environment.api.userParameter.canAccessUsers)
        .toPromise();
    }
    return this.canAccessUsers;
  }
}
