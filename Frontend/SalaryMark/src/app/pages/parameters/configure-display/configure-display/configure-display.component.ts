import { Item } from "./../../../../shared/models/pj-setting";
import { GlobalLabels } from "./../../../../shared/models/preferences-display";
import { ParamsService } from "@/shared/services/params/params.service";
import { ToastrService } from "ngx-toastr";
import { Component, OnInit } from "@angular/core";
import locales from "@/locales/parameters";
import routerNames from "@/shared/routerNames";
import { TokenService } from "@/shared/services/token/token.service";
import { Modules, SubModules } from "@/shared/models/modules";
import { ActivatedRoute } from "@angular/router";
import { IPermissions } from "@/shared/models/token";
import { Area, UserParameterAccess } from "@/shared/models/user-parameter";
import {
  DisplayItem,
  DisplaySettins,
  DisplayType,
  DisplayTypesConfigurations,
  DisplayTypeSettings,
} from "@/shared/models/display-settings";
import { PreferenceDisplay } from "@/shared/models/preferences-display";
import { IDefault } from "@/shared/interfaces/positions";
import { NgxSpinnerService } from "ngx-spinner";
import {
  DisplayOptionsEnum,
  DisplaySettingsEnum,
} from "@/shared/enum/display-settings-enum";
import { GlobalLabelEnum } from "@/shared/enum/global-label-enum";

@Component({
  selector: "app-configure-display",
  templateUrl: "./configure-display.component.html",
  styleUrls: ["./configure-display.component.scss"],
})
export class ConfigureDisplayComponent implements OnInit {
  routerNames = routerNames;
  locales = locales;
  subModules = SubModules;
  permissions: IPermissions;
  userParameterAccess: UserParameterAccess = new UserParameterAccess();
  canEdit: boolean;

  displayEnum = DisplaySettingsEnum;

  public displayTypesConfigurations: DisplayTypesConfigurations;
  public globalLabels: GlobalLabels;
  public displaySettings: DisplaySettins;
  public preferencesDisplays: PreferenceDisplay;
  public parameterList: any;
  public preferenceTitle: string;
  displaySettingsEnum: DisplaySettingsEnum;

  public validateCheck: any;
  public copyForResetData: DisplayTypesConfigurations = {
    displayConfiguration: {
      displayTypes: [],
    },
    preference: {
      globalLabels: [],
    },
  };

  constructor(
    private tokenService: TokenService,
    private activatedRoute: ActivatedRoute,
    private toastrService: ToastrService,
    private paramsService: ParamsService,
    private ngxSpinnerService: NgxSpinnerService
  ) {}

  async ngOnInit() {
    await this.loadData();
    await this.loadPermissions();
    await this.setStorage();

    await this.saveCopyData();

    this.ngxSpinnerService.hide();
  }

  async loadPermissions() {
    this.permissions = this.tokenService.getPermissions();
    this.userParameterAccess.showPjSettings =
      this.activatedRoute.snapshot.data.showPjSettings;
    this.userParameterAccess.showHourlyBasis =
      this.activatedRoute.snapshot.data.showHourlyBasis;
    this.userParameterAccess.showUsers =
      this.activatedRoute.snapshot.data.showUsers;
  }

  async loadData() {
    this.displayTypesConfigurations = await this.paramsService
      .getDisplayConfiguration()
      .toPromise();

    this.displaySettings = this.displayTypesConfigurations.displayConfiguration;
    this.preferencesDisplays = this.displayTypesConfigurations.preference;
    this.loadDataDropdown();
    this.displaySettingsEnum =
      this.displaySettings && this.displaySettings.displayTypes.length > 0
        ? DisplaySettingsEnum.display
        : DisplaySettingsEnum.preference;
  }

  loadDataDropdown() {
    if (
      !this.preferencesDisplays ||
      this.preferencesDisplays.globalLabels.length <= 0
    )
      return;
    const ignoreItems = [
      GlobalLabelEnum.POSITION_SALARY_MARK,
      GlobalLabelEnum.GSM,
      GlobalLabelEnum.LEVEL,
    ];
    this.parameterList = this.preferencesDisplays.globalLabels
      .filter((gl) => !ignoreItems.includes(gl.id))
      .map((item: GlobalLabels) => {
        return {
          id: item.id,
          title: item.alias,
          isDefault: item.isDefault,
        };
      });
    this.preferenceTitle = this.parameterList.find((pl) => pl.isDefault).title;
  }

  canAccess(subModule: SubModules) {
    return this.tokenService.validateModules(Modules.parameters, subModule);
  }

  async setStorage() {
    localStorage.removeItem("user-parameter-access");
    localStorage.setItem(
      "user-parameter-access",
      JSON.stringify(this.userParameterAccess)
    );
  }

  async sendLevelsToSave(): Promise<void> {
    this.ngxSpinnerService.show();

    switch (this.displaySettingsEnum) {
      case DisplaySettingsEnum.display:
        await this.saveDisplaySettings();
        break;

      case DisplaySettingsEnum.preference:
        await this.savePreferences();
        break;
    }

    this.saveCopyData();
    this.ngxSpinnerService.hide();
  }

  startEditValues() {
    this.canEdit = true;
  }

  cancelEdit() {
    this.canEdit = false;

    this.displayTypesConfigurations = Object.assign(
      this.displayTypesConfigurations,
      this.copyForResetData
    );
  }

  changeItemSelect(selected: IDefault) {
    this.preferencesDisplays.globalLabels.forEach((element) => {
      selected && element.id === +selected.id
        ? (element.isDefault = true)
        : (element.isDefault = false);
    });
  }

  changeItemSelectDisplay(event: any, item: DisplayItem, indexDisplay: number) {
    this.displaySettings.displayTypes[indexDisplay].subItems.find(
      (f) => f.id === item.id
    ).isChecked = event.currentTarget.checked;
  }

  saveCopyData() {
    this.copyForResetData = Object.assign(
      this.copyForResetData,
      this.displayTypesConfigurations
    );
  }

  async saveDisplaySettings() {
    this.validateCheck = this.displaySettings.displayTypes
      .filter(
        (dt) =>
          dt.subItems.filter((sb) => !sb.isChecked).length ==
            dt.subItems.length &&
          (dt.id == DisplayOptionsEnum.contractType ||
            dt.id == DisplayOptionsEnum.scenarios)
      )
      .map((res) => {
        return res.name;
      });

    if (this.validateCheck.length > 0) {
      this.toastrService.error(
        "Sessão " + this.validateCheck + ": " + locales.itemRequired,
        locales.errorTitle
      );
      this.canEdit = true;

      return;
    }
    const displaySettings: DisplayTypeSettings[] =
      this.displayTypesConfigurations.displayConfiguration.displayTypes
        .filter((f) => f.subItems.some((sb) => !sb.isChecked))
        .map(
          (res: DisplayType) =>
            <DisplayTypeSettings>{
              id: res.id,
              subItems: res.subItems
                .filter((sb) => !sb.isChecked)
                .map((sub) => sub.id),
            }
        );

    await this.paramsService.updateDisplaySettings(displaySettings).toPromise();
    this.canEdit = false;

    this.toastrService.success(
      locales.saveNewSucessMessage,
      locales.saveNewSucessMessageTitle
    );
  }

  async savePreferences() {
    await this.paramsService
      .updateGlobalLabels(
        this.displayTypesConfigurations.preference.globalLabels
      )
      .toPromise();

    this.canEdit = false;

    this.toastrService.success(
      locales.saveNewSucessMessage,
      locales.saveNewSucessMessageTitle
    );
  }

  loadGlobalLabels() {
    return this.preferencesDisplays.globalLabels.filter((gl) => !gl.disabled);
  }
}
