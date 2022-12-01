import { Component, OnInit, ChangeDetectionStrategy } from "@angular/core";

import locales from "@/locales/parameters";
import { HourlyBasisService } from "@/shared/services/hourly-basis/hourly-basis.service";
import { FormArray, FormBuilder, FormControl, FormGroup } from "@angular/forms";
import { NgxSpinnerService } from "ngx-spinner";
import { ToastrService } from "ngx-toastr";
import { ChangeDetectorRef } from "@angular/core";
import { HourlyBasis, HourlyBasisToSave } from "@/shared/models/hourly-basis";
import { IPermissions } from "@/shared/models/token";
import { TokenService } from "@/shared/services/token/token.service";

@Component({
  selector: "app-hourly-basis",
  templateUrl: "./hourly-basis.component.html",
  styleUrls: ["./hourly-basis.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HourlyBasisComponent implements OnInit {
  locales = locales;
  hourlyBasis: HourlyBasis[];
  form: FormGroup;
  hourlyBasisToSave: HourlyBasisToSave = { hourlyBasis: [] };
  permissions: IPermissions;
  isCollapsed: boolean = false;
  canEdit: boolean;

  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    private hourlyBasisService: HourlyBasisService,
    private ngxSpinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private fb: FormBuilder,
    private tokenService: TokenService
  ) {}

  async ngOnInit(): Promise<void> {
    this.permissions = this.tokenService.getPermissions();

    this.hourlyBasis = await this.hourlyBasisService
      .getHourlyBasis()
      .toPromise();

    this.form = this.fb.group({
      hourlyBasis: this.fb.array(
        this.hourlyBasis.map((hourlyBase) =>
          this.fb.group({
            id: this.fb.control(hourlyBase.id),
            baseSalary: this.fb.control(hourlyBase.baseSalary),
            display: this.fb.control(hourlyBase.display),
            parameters: this.fb.group({
              selectedValue: this.fb.control(
                hourlyBase.parameters.selectedValue
              ),
              enabled: this.fb.control(hourlyBase.parameters.enabled),
              options: this.fb.control(hourlyBase.parameters.options),
            }),
          })
        )
      ),
    });

    const hourlyBasis = this.form.get("hourlyBasis") as FormArray;

    hourlyBasis.controls.forEach((control) => {
      control.valueChanges.subscribe((hourlyBase: HourlyBasis) => {
        this.addHourlyBaseToSave(hourlyBase);
      });
    });

    this.ngxSpinnerService.hide();
    this.changeDetectorRef.markForCheck();
  }

  addHourlyBaseToSave(hourlyBase: HourlyBasis) {
    const hourlyBaseToSaveMappedToSave = {
      id: hourlyBase.id,
      display: hourlyBase.display,
      selectedValue: hourlyBase.parameters.selectedValue,
    };

    const hourlyBaseToSaveIndex = this.hourlyBasisToSave.hourlyBasis.findIndex(
      (hourlyBaseToSave) => hourlyBaseToSave.id === hourlyBase.id
    );

    if (hourlyBaseToSaveIndex !== -1) {
      this.hourlyBasisToSave.hourlyBasis[hourlyBaseToSaveIndex] =
        hourlyBaseToSaveMappedToSave;
    } else {
      this.hourlyBasisToSave.hourlyBasis.push(hourlyBaseToSaveMappedToSave);
    }
  }

  async sendHourlyBasisToSave(): Promise<void> {
    this.canEdit = false;

    await this.hourlyBasisService
      .saveHourlyBasis(this.hourlyBasisToSave)
      .toPromise();

    this.ngxSpinnerService.hide();
    this.toastrService.success(
      locales.saveSucessMessage,
      locales.saveSucessMessageTitle
    );
  }

  setParameterOptionSelected(hourlyBase: FormGroup, option: number) {
    hourlyBase.get("parameters").get("selectedValue").setValue(option);
  }

  startEditValues() {
    this.canEdit = true;
  }

  cancelEdit() {
    this.canEdit = false;
  }
}
