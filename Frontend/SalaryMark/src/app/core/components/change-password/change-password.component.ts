import { NgxSpinnerService } from "ngx-spinner";
import { Component, OnInit, ViewChild } from "@angular/core";

import locales from "@/locales/menu";
import { UserService } from "@/shared/services/user/user.service";
import { IDialogInput } from "@/shared/interfaces/dialog-input";
import { NgForm } from "@angular/forms";
import validations from "./Validations";
import { ToastrService } from "ngx-toastr";

const Constants = {
  eyeClose: "fa-eye-slash",
  eyeOpen: "fa-eye",
  inputPassword: "password",
  inputText: "text",
};

@Component({
  selector: "app-change-password",
  templateUrl: "./change-password.component.html",
  styleUrls: ["./change-password.component.scss"],
})
export class ChangePasswordComponent implements OnInit {
  public confirmPassword: string;
  public constants = Constants;
  public eyePassword = Constants.eyeClose;
  public inputType = Constants.inputPassword;
  public input: IDialogInput;
  public isPassword: boolean;
  public isConfirmPassword: boolean;
  public locales = locales;
  public password: string = "";
  public validations = validations;
  @ViewChild("form") ngForm: NgForm;

  constructor(
    private userService: UserService,
    private ngxSpinnerService: NgxSpinnerService,
    private ToastrService: ToastrService
  ) {}

  ngOnInit(): void {
    this.input = {
      disableFooter: false,
      idModal: "showModalChangePassword",
      title: locales.changePassword,
      btnPrimaryTitle: locales.changePassword,
      btnSecondaryTitle: locales.cancel,
    };
  }

  get getPasswordIcon(): string {
    return !this.isPassword ? this.constants.eyeClose : this.constants.eyeOpen;
  }
  get getPasswordInput(): string {
    return !this.isPassword
      ? this.constants.inputPassword
      : this.constants.inputText;
  }
  get getConfirmPasswordIcon(): string {
    return !this.isConfirmPassword
      ? this.constants.eyeClose
      : this.constants.eyeOpen;
  }
  get getConfirmPasswordInput(): string {
    return !this.isConfirmPassword
      ? this.constants.inputPassword
      : this.constants.inputText;
  }
  changePassword() {
    this.isPassword = !this.isPassword;
  }
  changeConfirmPassword() {
    this.isConfirmPassword = !this.isConfirmPassword;
  }

  onSave(e) {
    if (
      this.password === this.confirmPassword &&
      this.validationsIsValid(this.password)
    ) {
      this.userService
        .updateUserPassword(this.password, this.confirmPassword)
        .subscribe(
          (res) => {
            this.ngxSpinnerService.hide();
            this.ToastrService.success(locales.passwordChangedSuccessfully);
          },
          (err) => {}
        );
    } else {
      e.stopPropagation();
      !this.validationsIsValid(this.password) &&
      this.password === this.confirmPassword
        ? this.ToastrService.error(locales.passwordErrorChanged)
        : this.ToastrService.error(locales.PasswordsNotMatch);
    }
  }

  validationsIsValid(password: string) {
    return (
      this.password === this.confirmPassword &&
      password.length >= 8 &&
      /[A-Z]/.test(password) &&
      /[a-z]/.test(password) &&
      /\d/.test(password) &&
      /[ `!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/.test(password)
    );
  }

  validator(password: string, validation: any): any {
    let objReturn = { color: validation.iconColor };
    switch (validation) {
      case this.validations[0]:
        objReturn.color = password.length >= 8 ? "#3CB137" : objReturn.color;
        break;
      case this.validations[1]:
        objReturn.color = /[A-Z]/.test(password) ? "#3CB137" : objReturn.color;
        break;
      case this.validations[2]:
        objReturn.color = /[a-z]/.test(password) ? "#3CB137" : objReturn.color;
        break;
      case this.validations[3]:
        objReturn.color = /\d/.test(password) ? "#3CB137" : objReturn.color;
        break;
      case this.validations[4]:
        objReturn.color = /[ `!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/.test(
          password
        )
          ? "#3CB137"
          : objReturn.color;
        break;
      default:
        break;
    }
    return objReturn;
  }
}
