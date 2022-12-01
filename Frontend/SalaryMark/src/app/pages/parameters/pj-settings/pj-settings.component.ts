import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
} from "@angular/core";

import locales from "@/locales/parameters";
import { ContractTypesService } from "@/shared/services/contract-types/contract-types.service";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { NgxSpinnerService } from "ngx-spinner";
import { ToastrService } from "ngx-toastr";
import {
  ContractTypes,
  ContractTypesResponse,
} from "@/shared/models/contract-types";
import { Data, PJSetting, PJSettingsToSave } from "@/shared/models/pj-setting";
import { PjSettingsService } from "@/shared/services/pj-settings/pj-settings.service";
import { IPermissions } from "@/shared/models/token";
import { TokenService } from "@/shared/services/token/token.service";

@Component({
  selector: "app-pj-settings",
  templateUrl: "./pj-settings.component.html",
  styleUrls: ["./pj-settings.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PJSettingsComponent implements OnInit {
  isCollapsedAll = false;
  locales = locales;
  contractTypes: ContractTypes;
  pjSettings: PJSetting;
  contractTypesSelected: ContractTypesResponse;
  pjSettingsToSave: PJSettingsToSave = { data: [] };
  form: FormGroup;
  makeHeaderBarsSameDataHeight: any;
  permissions: IPermissions;
  canSave: boolean;
  canEdit: boolean;

  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    private contractTypesService: ContractTypesService,
    private pjSettingsService: PjSettingsService,
    private ngxSpinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private fb: FormBuilder,
    private tokenService: TokenService
  ) {}

  async ngOnInit(): Promise<void> {
    this.permissions = this.tokenService.getPermissions();

    this.contractTypes = await this.contractTypesService
      .getContractTypesPjSettings()
      .toPromise();

    this.contractTypesSelected = this.contractTypes.contractTypesResponse[0];

    if (this.contractTypesSelected) {
      this.pjSettings = await this.pjSettingsService
        .getPJSettings(this.contractTypesSelected.id)
        .toPromise();
      this.buildform();
    }

    this.ngxSpinnerService.hide();
    this.changeDetectorRef.markForCheck();
  }

  buildform() {
    this.form = this.fb.group({
      contractTypePercentageTotal: this.fb.control(
        this.pjSettings.contractTypePercentageTotal
      ),
      pjSettingsPercentageTotal: this.fb.control(
        this.pjSettings.pjSettingsPercentageTotal
      ),
      items: this.fb.array(
        this.pjSettings.items
          ? this.pjSettings.items.map((item) =>
              this.fb.group({
                name: this.fb.control(item.name),
                itemTypeId: this.fb.control(item.itemTypeId),
                pjSettingsId: this.fb.control(item.pjSettingsId),
                contractTypePercentage: this.fb.control(
                  item.contractTypePercentage
                ),
                pjSettingsPercentage: this.fb.control(
                  item.pjSettingsPercentage,
                  Validators.required
                ),
                subItems: this.fb.array(
                  item.subItems.map((subItem) =>
                    this.fb.group({
                      name: this.fb.control(subItem.name),
                      contractTypePercentage: this.fb.control(
                        subItem.contractTypePercentage
                      ),
                    })
                  )
                ),
              })
            )
          : []
      ),
    });

    this.pjSettingsToSave = { data: [], contractTypeId: null };
  }

  async getPJSettingsByContractTypeId(
    contractType: ContractTypesResponse
  ): Promise<void> {
    this.pjSettings = await this.pjSettingsService
      .getPJSettings(contractType.id)
      .toPromise();
    this.contractTypesSelected = contractType;
    this.buildform();
    this.ngxSpinnerService.hide();
    this.changeDetectorRef.markForCheck();
  }

  collapseAll() {
    this.isCollapsedAll = !this.isCollapsedAll;
  }

  async sendPJSettingsToSave(): Promise<void> {
    this.canEdit = false;
    if (!this.contractTypesSelected) {
      this.toastrService.error(
        locales.invalidContractTypeMessage,
        locales.errorTitle
      );
      return;
    }
    const data = this.form.getRawValue();
    const dataToSave = data.items.map((res) => {
      const result: Data = {
        percentage: res.pjSettingsPercentage,
        itemTypeId: res.itemTypeId,
        pjSettingsId: res.pjSettingsId,
      };
      return result;
    });
    this.pjSettingsToSave.data = dataToSave;
    this.pjSettingsToSave.contractTypeId = this.contractTypesSelected.id;
    await this.pjSettingsService
      .updatePJSetting(this.pjSettingsToSave)
      .toPromise();

    this.ngxSpinnerService.hide();
    this.toastrService.success(
      locales.saveNewSucessMessage,
      locales.infoMessage
    );
  }

  get totalPJSettingsPercentage() {
    this.canSave = true;
    const data = this.form.getRawValue();
    if (data && data.items) {
      return Math.round(
        data.items
          .map((item) => item.pjSettingsPercentage)
          .reduce((a, b) => a + b, 0)
      );
    }

    return 0;
  }

  startEditValues() {
    this.canEdit = true;
  }

  cancelEdit() {
    this.canEdit = false;
  }
}
