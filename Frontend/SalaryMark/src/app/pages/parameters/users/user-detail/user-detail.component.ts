import { UserDetailResolverService } from "./../../resolvers/user-detail.resolver";
import { ContentEnum } from "./../../../../shared/enum/display-settings-enum";
import {
  Area,
  UserPermissions,
} from "./../../../../shared/models/user-parameter";
import { ActivatedRoute } from "@angular/router";
import { Component, OnInit, ChangeDetectionStrategy } from "@angular/core";
import locales from "@/locales/parameters";
import { FormGroup, FormBuilder, FormControl } from "@angular/forms";
import {
  ChangeStatusUserToSave,
  UserParameterDetail,
} from "@/shared/models/user-parameter";
import { UserParameterService } from "@/shared/services/user-parameter/user-parameter.service";
import { NgxSpinnerService } from "ngx-spinner";
import { ToastrService } from "ngx-toastr";
import { IPermissions } from "@/shared/models/token";
import { TokenService } from "@/shared/services/token/token.service";

@Component({
  selector: "app-user-detail",
  templateUrl: "./user-detail.component.html",
  styleUrls: ["./user-detail.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserDetailComponent implements OnInit {
  locales = locales;
  userParameterDetail: UserParameterDetail;
  permissions: IPermissions;
  allIsChecked: boolean;
  form: FormGroup;
  photo: string;
  objeto: any;
  canEdit: boolean;
  userParameterDetailReserve: UserParameterDetail = {
    active: false,
    email: null,
    id: 0,
    lastAccess: null,
    name: null,
    photo: null,
    sector: null,
    userPermissions: null,
  };

  constructor(
    private fb: FormBuilder,
    private userParameterService: UserParameterService,
    private ngxSpinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private activatedRoute: ActivatedRoute,
    private tokenService: TokenService
  ) {}

  ngOnInit(): void {
    this.permissions = this.tokenService.getPermissions();
    this.userParameterDetail =
      this.activatedRoute.snapshot.data.userParameterDetail;
    this.userParameterDetailReserve = this.userParameterDetail;

    this.form = this.fb.group({
      id: this.fb.control(this.userParameterDetail.id),
      name: this.fb.control(this.userParameterDetail.name),
      email: this.fb.control(this.userParameterDetail.email),
      sector: this.fb.control(this.userParameterDetail.sector),
      active: this.fb.control(this.userParameterDetail.active),
      photo: this.fb.control(this.userParameterDetail.photo),
      lastAccess: this.fb.control(this.userParameterDetail.lastAccess),
      userPermissions: this.fb.group({
        levels: this.fb.array(
          this.userParameterDetail.userPermissions.levels.map((level) =>
            this.fb.group({
              id: this.fb.control(level.id),
              name: this.fb.control(level.name),
              isChecked: this.fb.control(level.isChecked),
            })
          )
        ),

        areas: this.fb.array(
          this.userParameterDetail.userPermissions.areas.map((area) =>
            this.fb.group({
              id: this.fb.control(area.id),
              name: this.fb.control(area.name),
              isChecked: this.fb.control(area.isChecked),
            })
          )
        ),
        sections: this.fb.array(
          this.userParameterDetail.userPermissions.sections.map((section) =>
            this.fb.group({
              id: this.fb.control(section.id),
              name: this.fb.control(section.name),
              allIsChecked: this.fb.control(
                section.subItems.filter((x) => x.isChecked).length ===
                  section.subItems.length
              ),
              isChecked: this.fb.control(section.isChecked),
              subItems: this.fb.array(
                section.subItems.map((subItem) =>
                  this.fb.group({
                    id: this.fb.control(subItem.id),
                    name: this.fb.control(subItem.name),
                    isChecked: this.fb.control(subItem.isChecked),
                  })
                )
              ),
            })
          )
        ),
        permission: this.fb.array(
          this.userParameterDetail.userPermissions.permission.map(
            (permission) =>
              this.fb.group({
                id: this.fb.control(permission.id),
                name: this.fb.control(permission.name),
                allIsChecked: this.fb.control(
                  permission.subItems.filter((x) => x.isChecked).length ===
                    permission.subItems.length
                ),
                isChecked: this.fb.control(permission.isChecked),
                subItems: this.fb.array(
                  permission.subItems.map((subItem) =>
                    this.fb.group({
                      id: this.fb.control(subItem.id),
                      name: this.fb.control(subItem.name),
                      isChecked: this.fb.control(subItem.isChecked),
                    })
                  )
                ),
              })
          )
        ),
        contents: this.fb.array(
          this.userParameterDetail.userPermissions.contents.map((content) =>
            this.fb.group({
              id: this.fb.control(content.id),
              name: this.fb.control(content.name),
              isChecked: this.fb.control(content.isChecked),
              allIsChecked: this.fb.control(
                content.subItems.filter((x) => x.isChecked).length ===
                  content.subItems.length
              ),

              subItems: this.fb.array(
                content.subItems.map((subItem) =>
                  this.fb.group({
                    id: this.fb.control(subItem.id),
                    name: this.fb.control(subItem.name),
                    isChecked: this.fb.control(subItem.isChecked),
                  })
                )
              ),
            })
          )
        ),
      }),
    });

    this.photo =
      this.userParameterDetail && this.userParameterDetail.photo
        ? `data:image/png;base64,${this.userParameterDetail.photo}`
        : "";
  }

  sendUserInformationToSave() {
    const userParameter = this.form.getRawValue();
    const validateCheck = userParameter.userPermissions.contents
      .filter(
        (dt) =>
          dt.subItems.filter((sb) => !sb.isChecked).length ==
            dt.subItems.length &&
          (dt.id == ContentEnum.salaryTable ||
            dt.id == ContentEnum.scenarios ||
            dt.id == ContentEnum.unity ||
            dt.id == ContentEnum.hierarchicalGroup)
      )
      .map((res) => {
        return res.name;
      });

    if (validateCheck.length > 0) {
      this.toastrService.error(
        "Sessão " + validateCheck + ": " + locales.itemRequired,
        locales.errorTitle
      );
      this.canEdit = true;

      return;
    }

    this.userParameterService
      .saveParameterDetailToSave(this.form.value)
      .subscribe((s) => {
        this.ngxSpinnerService.hide();
        this.toastrService.success(
          locales.saveNewSucessMessage,
          locales.infoMessage
        );

        this.userParameterDetail = this.form.getRawValue();
        this.userParameterDetailReserve = this.form.getRawValue();
        this.setPermissionsAfterSaveCancel();
        this.canEdit = false;
      });
  }

  async changeStatusUser(userParameter: UserParameterDetail): Promise<void> {
    const changeStatusUserToSave: ChangeStatusUserToSave = {
      userId: userParameter.id,
      active: userParameter.active,
    };

    await this.userParameterService
      .changeStatusUser(changeStatusUserToSave)
      .toPromise();

    this.ngxSpinnerService.hide();
    this.toastrService.success(
      locales.saveNewSucessMessage,
      locales.saveNewSucessMessageTitle
    );
  }

  onSelectedChildren(event: any, formGroup: FormGroup) {
    const total = formGroup.value.subItems.length;
    const subItemisChecked = formGroup.value.subItems.filter(
      (sb) => sb.isChecked
    ).length;

    formGroup.value.allIsChecked = total === subItemisChecked;
  }
  onSelectedOneChildren(event: any, formGroup: FormGroup) {
    const total = formGroup.value.subItems.length;
    const subItemisChecked = formGroup.value.subItems.filter(
      (sb) => sb.isChecked
    ).length;

    subItemisChecked > 0
      ? (formGroup.value.isChecked = true)
      : (formGroup.value.isChecked = total === subItemisChecked);
  }

  onSelectedAll(checked: any, formGroup: FormGroup) {
    formGroup.value.allIsChecked = checked;
    formGroup.value.subItems.forEach((element) => {
      element.isChecked = checked;
    });
  }
  onSelectedAllItems(checked: any, formGroup: FormGroup) {
    formGroup.value.isChecked = checked;
    formGroup.value.subItems.forEach((element) => {
      element.isChecked = checked;
    });
  }

  generateId(): string {
    return "c-" + Math.random().toString(16).slice(2);
  }

  startEditValues() {
    this.canEdit = true;
  }

  cancelEdit() {
    this.canEdit = false;

    Object.assign(this.userParameterDetail, this.userParameterDetailReserve);

    this.setPermissionsAfterSaveCancel();
  }

  setPermissionsAfterSaveCancel() {
    this.form
      .get("userPermissions")
      .get("levels")
      ["controls"].forEach((element: FormControl, index) => {
        element.patchValue(
          this.userParameterDetail.userPermissions.levels[index]
        );
      });

    this.form
      .get("userPermissions")
      .get("areas")
      ["controls"].forEach((element: FormControl, index) => {
        element.patchValue(
          this.userParameterDetail.userPermissions.areas[index]
        );
      });

    this.form
      .get("userPermissions")
      .get("sections")
      ["controls"].forEach((element: FormControl, index) => {
        element.patchValue(
          this.userParameterDetail.userPermissions.sections[index]
        );
      });
    this.form
      .get("userPermissions")
      .get("permission")
      ["controls"].forEach((element: FormControl, index) => {
        element.patchValue(
          this.userParameterDetail.userPermissions.permission[index]
        );
      });
    this.form
      .get("userPermissions")
      .get("contents")
      ["controls"].forEach((element: FormControl, index) => {
        element.patchValue(
          this.userParameterDetail.userPermissions.contents[index]
        );
      });
  }

  hiddenCheckboxAllOnContent(content: Area) {
    return content.id == ContentEnum.person || content.subItems.length <= 1;
  }
}
