import {
  FormBuilder,
  FormGroup,
  FormArray,
  FormControl,
  Validators,
} from "@angular/forms";
import {
  Component,
  OnInit,
  ChangeDetectionStrategy,
  ViewChild,
  ElementRef,
  ChangeDetectorRef,
  AfterViewInit,
  OnDestroy,
  EventEmitter,
  Output,
  Input,
  OnChanges,
  SimpleChanges,
} from "@angular/core";

import locales from "@/locales/parameters";
import { LevelService } from "@/shared/services/level/level.service";
import { NgxSpinnerService } from "ngx-spinner";

import { SaveLevels, Contributor, Level } from "./../../../shared/models/level";
import { ToastrService } from "ngx-toastr";
import { IPermissions } from "@/shared/models/token";
import { TokenService } from "@/shared/services/token/token.service";

@Component({
  selector: "app-levels",
  templateUrl: "./levels.component.html",
  styleUrls: ["./levels.component.scss"],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LevelsComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild("barStrategicData") barStrategicData: ElementRef;
  @ViewChild("barTaticData") barTaticData: ElementRef;
  @ViewChild("barOperationalData") barOperationalData: ElementRef;

  locales = locales;
  form: FormGroup;
  savelevels: SaveLevels = { levels: [] };
  level: Level;
  levelReserve: Level;
  makeHeaderBarsSameDataHeight: any;
  permissions: IPermissions;
  isEdit: boolean = false;

  public formControl = new FormControl(false);

  constructor(
    private changeDetectorRef: ChangeDetectorRef,
    private levelService: LevelService,
    private ngxSpinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private fb: FormBuilder,
    private tokenService: TokenService
  ) {}

  ngOnChanges() {
    if (this.form) {
      this.loadForm();
    }
  }
  async ngOnInit(): Promise<void> {
    this.level = await this.levelService.getLevels().toPromise();

    this.levelReserve = Object.assign({}, this.level);

    this.loadForm();

    this.permissions = this.tokenService.getPermissions();

    const strategic = {
      leadershipContributors: this.form.get("strategic").value
        .yourCompanyStructure.leadershipContributors as FormArray,
      individualContributors: this.form.get("strategic").value
        .yourCompanyStructure.individualContributors as FormArray,
    };

    strategic.leadershipContributors.controls.forEach((control) => {
      control.valueChanges.subscribe((contributor: Contributor) => {
        this.addLevelsToSave(contributor);
      });
    });

    strategic.individualContributors.controls.forEach((control) => {
      control.valueChanges.subscribe((contributor: Contributor) => {
        this.addLevelsToSave(contributor);
      });
    });

    const tatic = {
      leadershipContributors: this.form.get("tatic").value.yourCompanyStructure
        .leadershipContributors as FormArray,
      individualContributors: this.form.get("tatic").value.yourCompanyStructure
        .individualContributors as FormArray,
    };

    tatic.leadershipContributors.controls.forEach((control) => {
      control.valueChanges.subscribe((contributor: Contributor) => {
        this.addLevelsToSave(contributor);
      });
    });

    tatic.individualContributors.controls.forEach((control) => {
      control.valueChanges.subscribe((contributor: Contributor) => {
        this.addLevelsToSave(contributor);
      });
    });

    const operational = {
      leadershipContributors: this.form.get("operational").value
        .yourCompanyStructure.leadershipContributors as FormArray,
      individualContributors: this.form.get("operational").value
        .yourCompanyStructure.individualContributors as FormArray,
    };

    operational.leadershipContributors.controls.forEach((control) => {
      control.valueChanges.subscribe((contributor: Contributor) => {
        this.addLevelsToSave(contributor);
      });
    });

    operational.individualContributors.controls.forEach((control) => {
      control.valueChanges.subscribe((contributor: Contributor) => {
        this.addLevelsToSave(contributor);
      });
    });

    this.ngxSpinnerService.hide();
    this.changeDetectorRef.markForCheck();
  }

  loadForm(reload: boolean = false) {
    this.form = this.fb.group({
      strategic: {
        salaryMarkStructure: {
          leadershipContributors: this.fb.array(
            this.level.strategic.salaryMarkStructure.leadershipContributors.map(
              (leadershipContributors) =>
                this.fb.group({
                  id: this.fb.control(leadershipContributors.id),
                  level: this.fb.control(leadershipContributors.level),
                  code: this.fb.control(leadershipContributors.code),
                  active: this.fb.control(leadershipContributors.active),
                  required: this.fb.control(false),
                })
            )
          ),

          individualContributors: this.fb.array(
            this.level.strategic.salaryMarkStructure.individualContributors.map(
              (individualContributor) =>
                this.fb.group({
                  id: this.fb.control(individualContributor.id),
                  level: this.fb.control(individualContributor.level),
                  code: this.fb.control(individualContributor.code),
                  active: this.fb.control(individualContributor.active),
                })
            )
          ),
        },

        yourCompanyStructure: {
          leadershipContributors: this.fb.array(
            this.level.strategic.yourCompanyStructure.leadershipContributors.map(
              (leadershipContributors) =>
                this.fb.group({
                  id: this.fb.control(leadershipContributors.id),
                  level: this.fb.control(
                    leadershipContributors.level,
                    this.fb.control(leadershipContributors.active)
                      ? Validators.required
                      : undefined
                  ),
                  code: this.fb.control(leadershipContributors.code),
                  active: this.fb.control(leadershipContributors.active),
                })
            )
          ),

          individualContributors: this.fb.array(
            this.level.strategic.yourCompanyStructure.individualContributors.map(
              (individualContributor) =>
                this.fb.group({
                  id: this.fb.control(individualContributor.id),
                  level: this.fb.control(individualContributor.level),
                  code: this.fb.control(individualContributor.code),
                  active: this.fb.control(individualContributor.active),
                })
            )
          ),
        },
      },

      tatic: {
        salaryMarkStructure: {
          leadershipContributors: this.fb.array(
            this.level.tatic.salaryMarkStructure.leadershipContributors.map(
              (leadershipContributors) =>
                this.fb.group({
                  id: this.fb.control(leadershipContributors.id),
                  level: this.fb.control(leadershipContributors.level),
                  code: this.fb.control(leadershipContributors.code),
                  active: this.fb.control(leadershipContributors.active),
                })
            )
          ),

          individualContributors: this.fb.array(
            this.level.tatic.salaryMarkStructure.individualContributors.map(
              (individualContributor) =>
                this.fb.group({
                  id: this.fb.control(individualContributor.id),
                  level: this.fb.control(individualContributor.level),
                  code: this.fb.control(individualContributor.code),
                  active: this.fb.control(individualContributor.active),
                })
            )
          ),
        },

        yourCompanyStructure: {
          leadershipContributors: this.fb.array(
            this.level.tatic.yourCompanyStructure.leadershipContributors.map(
              (leadershipContributors) =>
                this.fb.group({
                  id: this.fb.control(leadershipContributors.id),
                  level: this.fb.control(
                    leadershipContributors.level,
                    this.fb.control(leadershipContributors.active)
                      ? Validators.required
                      : undefined
                  ),
                  code: this.fb.control(leadershipContributors.code),
                  active: this.fb.control(leadershipContributors.active),
                })
            )
          ),

          individualContributors: this.fb.array(
            this.level.tatic.yourCompanyStructure.individualContributors.map(
              (individualContributor) =>
                this.fb.group({
                  id: this.fb.control(individualContributor.id),
                  level: this.fb.control(
                    individualContributor.level,
                    this.fb.control(individualContributor.active)
                      ? Validators.required
                      : undefined
                  ),
                  code: this.fb.control(individualContributor.code),
                  active: this.fb.control(individualContributor.active),
                })
            )
          ),
        },
      },

      operational: {
        salaryMarkStructure: {
          leadershipContributors: this.fb.array(
            this.level.operational.salaryMarkStructure.leadershipContributors.map(
              (leadershipContributors) =>
                this.fb.group({
                  id: this.fb.control(leadershipContributors.id),
                  level: this.fb.control(leadershipContributors.level),
                  code: this.fb.control(leadershipContributors.code),
                  active: this.fb.control(leadershipContributors.active),
                })
            )
          ),

          individualContributors: this.fb.array(
            this.level.operational.salaryMarkStructure.individualContributors.map(
              (individualContributor) =>
                this.fb.group({
                  id: this.fb.control(individualContributor.id),
                  level: this.fb.control(individualContributor.level),
                  code: this.fb.control(individualContributor.code),
                  active: this.fb.control(individualContributor.active),
                })
            )
          ),
        },

        yourCompanyStructure: {
          leadershipContributors: this.fb.array(
            this.level.operational.yourCompanyStructure.leadershipContributors.map(
              (leadershipContributors) =>
                this.fb.group({
                  id: this.fb.control(leadershipContributors.id),
                  level: this.fb.control(
                    leadershipContributors.level,
                    this.fb.control(leadershipContributors.active)
                      ? Validators.required
                      : undefined
                  ),
                  code: this.fb.control(leadershipContributors.code),
                  active: this.fb.control(leadershipContributors.active),
                })
            )
          ),

          individualContributors: this.fb.array(
            this.level.operational.yourCompanyStructure.individualContributors.map(
              (individualContributor) =>
                this.fb.group({
                  id: this.fb.control(individualContributor.id),
                  level: this.fb.control(
                    individualContributor.level,
                    this.fb.control(individualContributor.active)
                      ? Validators.required
                      : undefined
                  ),
                  code: this.fb.control(individualContributor.code),
                  active: this.fb.control(individualContributor.active),
                })
            )
          ),
        },
      },
    });
  }

  addLevelsToSave(contributor: Contributor) {
    const contributorMappedToSave = {
      levelId: contributor.id,
      level: contributor.active ? contributor.level : "",
      enabled: contributor.active,
    };

    const contributorIndex = this.savelevels.levels.findIndex(
      (level) => level.levelId === contributor.id
    );

    if (contributorIndex !== -1) {
      this.savelevels.levels[contributorIndex] = contributorMappedToSave;
    } else {
      this.savelevels.levels.push(contributorMappedToSave);
    }
  }

  validator(): boolean {
    let valid = true;

    this.form.controls.strategic.value.yourCompanyStructure.leadershipContributors.value.map(
      (item: any) => {
        if (item.active && item.level == "") {
          valid = false;
        }
      }
    );

    this.form.controls.tatic.value.yourCompanyStructure.leadershipContributors.value.map(
      (item: any) => {
        if (item.active && item.level == "") {
          valid = false;
        }
      }
    );

    this.form.controls.tatic.value.yourCompanyStructure.individualContributors.value.map(
      (item: any) => {
        if (item.active && item.level == "") {
          valid = false;
        }
      }
    );

    this.form.controls.operational.value.yourCompanyStructure.individualContributors.value.map(
      (item: any) => {
        if (item.active && item.level == "") {
          valid = false;
        }
      }
    );

    this.form.controls.operational.value.yourCompanyStructure.leadershipContributors.value.map(
      (item: any) => {
        if (item.active && item.level == "") {
          valid = false;
        }
      }
    );

    return valid;
  }

  async sendLevelsToSave(): Promise<void> {
    if (!this.validator()) {
      this.ngxSpinnerService.hide();
      this.toastrService.error(locales.errorMessageInputs, locales.errorTitle);
      return;
    }

    this.permissions.canEditLevels = true;
    this.isEdit = false;

    this.resetLastObjectLevels();

    await this.levelService.saveLavels(this.savelevels).toPromise();
    this.ngxSpinnerService.hide();
    this.toastrService.success(
      locales.saveNewSucessMessage,
      locales.saveNewSucessMessageTitle
    );
  }

  resetLastObjectLevels() {
    this.levelReserve.operational.salaryMarkStructure.individualContributors =
      this.objectMapLevels(
        this.levelReserve.operational.salaryMarkStructure.individualContributors
      );

    this.levelReserve.operational.salaryMarkStructure.leadershipContributors =
      this.objectMapLevels(
        this.levelReserve.operational.salaryMarkStructure.leadershipContributors
      );

    this.levelReserve.operational.yourCompanyStructure.individualContributors =
      this.objectMapLevels(
        this.levelReserve.operational.yourCompanyStructure
          .individualContributors
      );

    this.levelReserve.operational.yourCompanyStructure.leadershipContributors =
      this.objectMapLevels(
        this.levelReserve.operational.yourCompanyStructure
          .leadershipContributors
      );

    /******************************************************* */
    this.levelReserve.strategic.salaryMarkStructure.individualContributors =
      this.objectMapLevels(
        this.levelReserve.strategic.salaryMarkStructure.individualContributors
      );

    this.levelReserve.strategic.salaryMarkStructure.leadershipContributors =
      this.objectMapLevels(
        this.levelReserve.strategic.salaryMarkStructure.leadershipContributors
      );

    this.levelReserve.strategic.yourCompanyStructure.individualContributors =
      this.objectMapLevels(
        this.levelReserve.strategic.yourCompanyStructure.individualContributors
      );

    this.levelReserve.strategic.yourCompanyStructure.leadershipContributors =
      this.objectMapLevels(
        this.levelReserve.strategic.yourCompanyStructure.leadershipContributors
      );

    /******************************************************* */
    this.levelReserve.tatic.yourCompanyStructure.individualContributors =
      this.objectMapLevels(
        this.levelReserve.tatic.yourCompanyStructure.individualContributors
      );

    this.levelReserve.tatic.yourCompanyStructure.leadershipContributors =
      this.objectMapLevels(
        this.levelReserve.tatic.yourCompanyStructure.leadershipContributors
      );

    this.levelReserve.tatic.salaryMarkStructure.individualContributors =
      this.objectMapLevels(
        this.levelReserve.tatic.salaryMarkStructure.individualContributors
      );

    this.levelReserve.tatic.salaryMarkStructure.leadershipContributors =
      this.objectMapLevels(
        this.levelReserve.tatic.salaryMarkStructure.leadershipContributors
      );
  }

  objectMapLevels(levelArray: any[]): any[] {
    return levelArray.map((m) => {
      const levelObjetctSave = this.savelevels.levels.find((f) => {
        f.levelId === m.id;
      });
      m.active =
        levelObjetctSave && levelObjetctSave.enabled
          ? levelObjetctSave.enabled
          : false;
      m.level =
        levelObjetctSave && levelObjetctSave.level
          ? levelObjetctSave.level
          : "";
    });
  }

  onResize() {
    this.changeDetectorRef.markForCheck();
  }

  ngAfterViewInit(): void {
    this.makeHeaderBarsSameDataHeight = setInterval(() => {
      if (
        this.barStrategicData &&
        this.barStrategicData.nativeElement.offsetHeight &&
        this.barTaticData &&
        this.barTaticData.nativeElement.offsetHeight &&
        this.barOperationalData &&
        this.barOperationalData.nativeElement.offsetHeight
      ) {
        clearInterval(this.makeHeaderBarsSameDataHeight);
      }
      this.changeDetectorRef.markForCheck();
    }, 200);
  }

  ngOnDestroy(): void {
    clearInterval(this.makeHeaderBarsSameDataHeight);
  }

  invertValue(control: FormControl) {
    const newValue = !this.formControl.value;
    this.writeValue(newValue);

    if (!newValue) {
      control.get("level").patchValue("");
    }
  }

  writeValue(value: boolean): void {
    this.formControl.setValue(value);
  }

  startEditValues() {
    this.isEdit = true;
  }

  cancelEdit() {
    this.isEdit = false;
    Object.assign(this.level, this.levelReserve);

    this.form.controls.strategic.value.yourCompanyStructure.leadershipContributors.controls.forEach(
      (element: FormControl, index) => {
        element.patchValue(
          this.level.strategic.yourCompanyStructure.leadershipContributors[
            index
          ]
        );
      }
    );

    this.form.controls.tatic.value.yourCompanyStructure.leadershipContributors.controls.forEach(
      (element: FormControl, index) => {
        element.patchValue(
          this.level.tatic.yourCompanyStructure.leadershipContributors[index]
        );
      }
    );

    this.form.controls.tatic.value.yourCompanyStructure.individualContributors.controls.forEach(
      (element: FormControl, index) => {
        element.patchValue(
          this.level.tatic.yourCompanyStructure.individualContributors[index]
        );
      }
    );

    this.form.controls.operational.value.yourCompanyStructure.leadershipContributors.controls.forEach(
      (element: FormControl, index) => {
        element.patchValue(
          this.level.operational.yourCompanyStructure.leadershipContributors[
            index
          ]
        );
      }
    );

    this.form.controls.operational.value.yourCompanyStructure.individualContributors.controls.forEach(
      (element: FormControl, index) => {
        element.patchValue(
          this.level.operational.yourCompanyStructure.individualContributors[
            index
          ]
        );
      }
    );
  }
}
