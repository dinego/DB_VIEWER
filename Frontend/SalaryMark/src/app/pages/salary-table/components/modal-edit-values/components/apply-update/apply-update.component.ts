import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";

import { TypesValuesGsm } from "../commons/radioTypesEnum";
import {
  FormBuilder,
  FormGroup,
  ValidationErrors,
  Validators,
} from "@angular/forms";
import { UpdateValidation } from "./common/validations";
import errors from "./common/errors";
import locales from "@/locales/salary-table";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { IEditSalarialTable } from "@/shared/models/editSalarialTable";
import { IDefault } from "@/shared/interfaces/positions";
import { SalaryTableService } from "@/shared/services/salary-table/salary-table.service";
import { NgxSpinnerService } from "ngx-spinner";
import { ToastrService } from "ngx-toastr";
import { ConfirmModalEditPositionComponent } from "@/shared/components/confirm-modal-edit-position/confirm-modal-edit-position.component";
import { IErrorFieldMessage } from "@/shared/interfaces/error-message";

@Component({
  selector: "app-apply-update",
  templateUrl: "./apply-update.component.html",
  styleUrls: ["./apply-update.component.scss"],
})
export class ApplyUpdateComponent implements OnInit {
  public locales = locales;
  public typeGsm = TypesValuesGsm;
  public itemGsmChecked: number;
  public formUpdate: FormGroup;
  public gsmInitial: number;
  public gsmFinal: number;
  public gsmList: IDefault[] = [];
  public maskGsmValue: any = "00*.00";
  public suffix: string = "%";
  public prefix = "";

  @Output() eventErrors = new EventEmitter<any[]>();
  @Input() bsModalRef: BsModalRef;
  @Input() data: IEditSalarialTable;
  @Input() tableId: number;
  @Input() projectId: number;
  @Input() gsmGlobalLabel: string;

  @Output() hideModalPosition = new EventEmitter<boolean>();
  @Output() showModalPosition = new EventEmitter();

  public errors = errors;
  public errorList: any[] = [];
  constructor(
    private fb: FormBuilder,
    private salarialTableService: SalaryTableService,
    private ngxSpinner: NgxSpinnerService,
    private toastrService: ToastrService,
    private _modalService: BsModalService
  ) {}

  ngOnInit(): void {
    this.createFormUpdate();
    this.createArrayGsm();
    this.setRadioInit(true);
  }

  setRadioInit(isInit: boolean) {
    this.itemGsmChecked =
      this.data?.salaryTableValues?.tableUpdate?.typeMultiply;

    if (this.itemGsmChecked > 0)
      this.changeCheckedType(this.itemGsmChecked, isInit);
  }

  createArrayGsm() {
    if (
      this.data &&
      this.data.salaryTableValues &&
      this.data.salaryTableValues.gsmFinal &&
      this.data.salaryTableValues.gsmInitial
    )
      for (
        var i = this.data.salaryTableValues.gsmInitial;
        i <= this.data.salaryTableValues.gsmFinal;
        i++
      ) {
        const objPush: IDefault = { id: i.toString(), title: i.toString() };
        this.gsmList.push({ ...objPush });
      }
  }

  createFormUpdate() {
    this.gsmInitial =
      this.data &&
      this.data.salaryTableValues &&
      this.data.salaryTableValues.tableUpdate &&
      this.data.salaryTableValues.tableUpdate.gsmInitial
        ? this.data.salaryTableValues.tableUpdate.gsmInitial
        : 0;

    this.gsmFinal =
      this.data &&
      this.data.salaryTableValues &&
      this.data.salaryTableValues.tableUpdate &&
      this.data.salaryTableValues.tableUpdate.gsmInitial
        ? this.data.salaryTableValues.tableUpdate.gsmFinal
        : 0;

    const tableName =
      this.data &&
      this.data.salaryTableValues &&
      this.data.salaryTableValues.salarialTableName
        ? this.data.salaryTableValues.salarialTableName
        : "";

    const multiplyValue =
      this.data &&
      this.data.salaryTableValues &&
      this.data.salaryTableValues.tableUpdate
        ? this.data.salaryTableValues.tableUpdate.multiply
        : 1;

    this.formUpdate = this.fb.group(
      {
        gsmInitial: [
          this.gsmInitial,
          Validators.compose([Validators.required]),
        ],
        gsmFinal: [this.gsmFinal, Validators.compose([Validators.required])],
        tableName: [
          tableName,
          Validators.compose([
            Validators.required,
            Validators.minLength(1),
            Validators.maxLength(255),
          ]),
        ],
        gsmValue: [
          multiplyValue,
          Validators.compose([
            Validators.required,
            Validators.minLength(1),
            Validators.maxLength(40),
          ]),
        ],
        justify: [
          "",
          Validators.compose([
            Validators.required,
            Validators.minLength(1),
            Validators.maxLength(255),
          ]),
        ],
      },
      {
        validator: UpdateValidation.CompareGsms,
      }
    );
  }

  changeCheckedType(item: number, isInit?: boolean) {
    this.itemGsmChecked = item;

    const value: number =
      this.formUpdate.controls["gsmValue"].value > 0
        ? this.formUpdate.controls["gsmValue"].value
        : 0;

    let newValue: string = parseFloat(value.toString()).toFixed(2);

    newValue = newValue.replace(".", ",");

    if (item == this.typeGsm.PERCENT) {
      if (!isInit) this.formUpdate.controls["gsmValue"].setValue("0,00");

      this.formUpdate.controls["gsmValue"].setValue(newValue);
      this.maskGsmValue = "0*,00";
      this.suffix = "%";
      this.prefix = "";
    } else {
      if (!isInit) this.formUpdate.controls["gsmValue"].setValue("0,00");

      this.formUpdate.controls["gsmValue"].setValue(newValue);
      this.maskGsmValue = "0*,00";
      this.suffix = "";
      this.prefix = "R$ ";
    }
  }

  updateData() {
    if (this.errorList.length == 0) {
      this.ngxSpinner.show();

      const updateData = {
        projectId: this.projectId,
        tableId: this.tableId,
        salaryTableName: this.formUpdate.value.tableName,
        gsmInitial: this.formUpdate.value.gsmInitial,
        gsmFinal: this.formUpdate.value.gsmFinal,
        justify: this.formUpdate.value.justify,
        multiply: parseInt(this.formUpdate.value.gsmValue),
        typeMultiply: this.itemGsmChecked,
      };

      this.salarialTableService.updateTableInfo(updateData).subscribe(
        (res) => {
          this.ngxSpinner.hide();
          this.toastrService.success(locales.updateTableSucessfully);
          setTimeout(() => {
            window.location.reload();
          }, 500);
        },
        (err) => {}
      );
    }
  }

  getFormValidationErrors() {
    this.errorList = [];
    Object.keys(this.formUpdate.controls).forEach((key) => {
      const controlErrors: ValidationErrors = this.formUpdate.get(key).errors;
      if (controlErrors != null) {
        Object.keys(controlErrors).forEach((keyError) => {
          const errorAdd = this.errors.find((error) => error.field === key);
          this.errorList.push(this.changeBraboToGlobalLabel(errorAdd));
        });
      }
    });
  }

  closeModal(e) {
    this.bsModalRef.hide();
  }

  initialSelected(event: IDefault) {
    this.gsmInitial = parseInt(event.title);
    this.formUpdate.controls.gsmInitial.setValue(parseInt(event.title));
  }
  finalSelected(event: IDefault) {
    this.gsmFinal = parseInt(event.title);
    this.formUpdate.controls.gsmFinal.setValue(parseInt(event.title));
  }

  openModalConfirm() {
    this.getFormValidationErrors();

    this.eventErrors.emit(this.errorList);

    if (this.errorList.length == 0) {
      this.hideModalPosition.emit(true);

      this.bsModalRef = this._modalService.show(
        ConfirmModalEditPositionComponent,
        {
          class: "modal-dialog modal-dialog-centered",
        }
      );

      this.bsModalRef.content.onSaveEmitter.subscribe((res) => {
        this.updateData();
      });

      this.bsModalRef.content.onCancelEmitter.subscribe((res) => {
        this.showModalPosition.emit();
      });
    }
  }

  changeBraboToGlobalLabel(errorAdd: IErrorFieldMessage): IErrorFieldMessage {
    if (errorAdd.message.includes("@CHANGEOFBRABO"))
      errorAdd.message = errorAdd.message.replace(
        "@CHANGEOFBRABO",
        this.gsmGlobalLabel
      );

    if (errorAdd.message.includes("@CHANGEOFBRABO"))
      errorAdd.message = errorAdd.message.replace(
        "@CHANGEOFBRABO",
        this.gsmGlobalLabel
      );

    return errorAdd;
  }
}
