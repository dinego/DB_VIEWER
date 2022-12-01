import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import locales from '@/locales/parameters';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { NgxSpinnerService } from 'ngx-spinner';
import { GlobalLabelService } from '@/shared/services/global-label/global-label.service';
import { IGlobalLabel, IGlobalLabelToSave } from '@/shared/models/global-label';
import { IPermissions } from '@/shared/models/token';
import { TokenService } from '@/shared/services/token/token.service';
import { GlobalLabelEnum } from '@/shared/enum/global-label-enum';

@Component({
  selector: 'app-global-labels',
  templateUrl: './global-labels.component.html',
  styleUrls: ['./global-labels.component.scss']
})
export class GlobalLabelsComponent implements OnInit {

  locales = locales;
  form: FormGroup;
  globalLabels: IGlobalLabel[] = [];
  permissions: IPermissions;
  globalLabelEnum = GlobalLabelEnum;

  constructor(private changeDetectorRef: ChangeDetectorRef,
    private ngxSpinnerService: NgxSpinnerService,
    private toastrService: ToastrService,
    private fb: FormBuilder,
    private globalLabelService: GlobalLabelService,
    private tokenService: TokenService
  ) { }

  async ngOnInit() {
    this.permissions = this.tokenService.getPermissions();
    this.globalLabels = await this.globalLabelService.get().toPromise();
    this.createForm();
    this.changeDetectorRef.markForCheck();
  }

  createForm() {
    this.form = this.fb.group({
      globalLabels: this.fb.array(
        this.globalLabels.map(
          (m) =>
            this.fb.group({
              id: this.fb.control(m.id),
              isChecked: this.fb.control(m.isChecked),
              name: this.fb.control(m.name),
              alias: this.fb.control(m.alias, Validators.required)
            })
        )
      )
      // this.globalLabels
      //   ? this.fb.array(
      //       this.globalLabels.map(m => {
      //         this.fb.group({
      //           id: this.fb.control(m.id),
      //           isChecked: this.fb.control(m.isChecked),
      //           name: this.fb.control(m.name),
      //           alias: this.fb.control(m.alias, Validators.required)
      //         });
      //       })
      //     )
      //   : []
    });
    this.ngxSpinnerService.hide();
  }

  async save() {
    const dataToSave: IGlobalLabelToSave = this.form.getRawValue();
    await this.globalLabelService
      .update(dataToSave)
      .toPromise();
    this.ngxSpinnerService.hide();
    this.toastrService.success(
      locales.saveSucessMessage,
      locales.saveSucessMessageTitle
    );
  }
}
