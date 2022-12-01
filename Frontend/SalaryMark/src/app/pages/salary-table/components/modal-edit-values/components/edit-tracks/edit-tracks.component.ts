import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import errors from "./common/errors";
import locales from "@/locales/salary-table";
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  Validators,
} from "@angular/forms";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { NgxSpinnerService } from "ngx-spinner";
import { SalaryTableService } from "@/shared/services/salary-table/salary-table.service";
import { HourlyBasisEnum } from "@/shared/models/hourly-basis";
import { TableSalaryColumnEnum } from "@/shared/enum/table-salary-column-enum";
import {
  IEditSalarialTable,
  ITableValues,
} from "@/shared/models/editSalarialTable";
import { DecimalPipe } from "@angular/common";
import { ToastrService } from "ngx-toastr";
import { IDefault } from "@/shared/interfaces/positions";
import { PercentHeaderEnum } from "./common/tracks-enum";
import { ConfirmModalEditPositionComponent } from "@/shared/components/confirm-modal-edit-position/confirm-modal-edit-position.component";

@Component({
  selector: "app-edit-tracks",
  templateUrl: "./edit-tracks.component.html",
  styleUrls: ["./edit-tracks.component.scss"],
})
export class EditTracksComponent implements OnInit {
  @Output() hideModalPosition = new EventEmitter<boolean>();
  @Output() showModalPosition = new EventEmitter();
  @Output() eventErrors = new EventEmitter<any[]>();
  @Input() bsModalRef: BsModalRef;
  @Input() tableId?: number;
  @Input() isAsc?: boolean;
  @Input() hoursType?: HourlyBasisEnum;
  @Input() data: IEditSalarialTable;
  @Input() projectId: number;

  public tableSalaryColumnEnum = TableSalaryColumnEnum;
  public hourlyBasisEnum = HourlyBasisEnum;
  public locales = locales;
  public formEdit: FormGroup;
  public errors = errors;
  public errorList: any[] = [];
  public gsmList: IDefault[] = [];
  public valuesItems: FormArray;

  constructor(
    private _fb: FormBuilder,
    private _ngxSpinnerService: NgxSpinnerService,
    private _salaryTableService: SalaryTableService,
    private _toastService: ToastrService,
    private _decimalPipe: DecimalPipe,
    private _modalService: BsModalService,
    private _toastrService: ToastrService
  ) {}

  async ngOnInit(): Promise<void> {
    await this.insertBlankHeaderData();
    await this.createFormUpdate();
    await this.createArrayGsm();
  }

  async insertBlankHeaderData() {
    this.data.headers.splice(0, 0, {
      colName: null,
      colPos: 0,
      isMidPoint: false,
    });
  }

  async createFormUpdate() {
    this.formEdit = this._fb.group({
      tableName: [
        this.data.salaryTableValues.salarialTableName,
        Validators.compose([
          Validators.required,
          Validators.minLength(1),
          Validators.maxLength(255),
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
      valuesRow: this.setValuesRows(),
    });
  }

  setValuesRows() {
    var rows = new FormArray([]);
    this.data.rangeEdit = this.data.rangeEdit.sort(); // reordenar

    this.data.salaryTableValues.salaryTableValues.forEach((values) => {
      var row = new FormArray([]);
      this.data.rangeEdit.forEach((range) => {
        var valueRow = new FormGroup({
          item: new FormControl(
            this.setValueItem(this.getValueRange(range, values)),
            Validators.compose([
              Validators.required,
              Validators.minLength(1),
              Validators.maxLength(10),
              Validators.pattern(/^(\d+(?:[\.\,]\d{3})|\-?)$/),
            ])
          ),
        });
        row.push(valueRow);
      });
      rows.push(row);
    });

    return rows;
  }
  setValueItem(value?: any): any {
    if (!value) return "-";

    return this._decimalPipe.transform(Math.round(value), "1.0-3");
  }

  getRows() {
    return this.formEdit.get("valuesRow")["controls"];
  }

  getRow(form) {
    return form["controls"];
  }

  editData() {
    if (this.errorList.length == 0) {
      this._ngxSpinnerService.show();

      this.data.salaryTableValues.salarialTableName =
        this.formEdit.controls["tableName"]["value"];

      var arrayFormValues = this.formEdit.controls["valuesRow"]["controls"];

      var rangeEdit = this.data.rangeEdit;

      if (arrayFormValues.length > 0) {
        this.data.salaryTableValues.tableUpdate.justify =
          this.formEdit.controls["justify"]["value"];

        let indexX = 0;
        arrayFormValues.forEach((element) => {
          let indexY = 0;
          rangeEdit.forEach((range) => {
            var objRet = this.data.salaryTableValues.salaryTableValues[indexX];
            this.data.salaryTableValues.salaryTableValues[indexX] =
              this.setValueByRange(
                range,
                objRet,
                element["controls"][indexY]["value"]
              );
            indexY++;
          });
          indexX++;
        });

        const objSendUpdate = {
          projectId: this.projectId,
          tableId: this.tableId,
          salaryTable: {
            salaryTableName: this.data.salaryTableValues.salarialTableName,
            gsmInitial: this.data.salaryTableValues.tableUpdate.gsmInitial,
            gsmFinal: this.data.salaryTableValues.tableUpdate.gsmFinal,
            justify: this.data.salaryTableValues.tableUpdate.justify,
            multiply: this.data.salaryTableValues.tableUpdate.multiply,
            typeMultiply: this.data.salaryTableValues.tableUpdate.typeMultiply,
            salaryTableValues: this.data.salaryTableValues.salaryTableValues,
          },
        };

        this._salaryTableService.updateTableValues(objSendUpdate).subscribe(
          (res) => {
            this._ngxSpinnerService.hide();
            this._toastService.success(locales.updateTableSucessfully);

            this.bsModalRef.hide();
            setTimeout(() => {
              window.location.reload();
            }, 500);
          },
          (err) => {}
        );
      }
    }
  }

  getFormValidationErrors() {
    this.errorList = [];
    Object.keys(this.formEdit.controls).forEach((key) => {
      const controlErrors: ValidationErrors = this.formEdit.get(key).errors;
      if (controlErrors != null) {
        Object.keys(controlErrors).forEach((keyError) => {
          const errorAdd = this.errors.find((error) => error.field === key);
          this.errorList.push(errorAdd);
        });
      }
    });
  }

  closeModal(e) {
    this.bsModalRef.hide();
  }

  getValueRange(range: number, values: ITableValues): any {
    switch (range) {
      case PercentHeaderEnum.Minus6:
        return values.minor6;
      case PercentHeaderEnum.Minus5:
        return values.minor5;
      case PercentHeaderEnum.Minus4:
        return values.minor4;
      case PercentHeaderEnum.Minus3:
        return values.minor3;
      case PercentHeaderEnum.Minus2:
        return values.minor2;
      case PercentHeaderEnum.Minus1:
        return values.minor1;
      case PercentHeaderEnum.Mid:
        return values.mid;
      case PercentHeaderEnum.Plus1:
        return values.plus1;
      case PercentHeaderEnum.Plus2:
        return values.plus2;
      case PercentHeaderEnum.Plus3:
        return values.plus3;
      case PercentHeaderEnum.Plus4:
        return values.plus4;
      case PercentHeaderEnum.Plus5:
        return values.plus5;
      case PercentHeaderEnum.Plus6:
        return values.plus6;
      default:
        break;
    }
  }

  setValueByRange(range: number, objRet: ITableValues, value: any): any {
    value = value.item == "-" ? null : value.item;

    if (value) {
      value = value.split(".").join("");
      value = parseFloat(value);
    }

    switch (range) {
      case PercentHeaderEnum.Minus6:
        objRet.minor6 = value;
        break;
      case PercentHeaderEnum.Minus5:
        objRet.minor5 = value;
        break;
      case PercentHeaderEnum.Minus4:
        objRet.minor4 = value;
        break;
      case PercentHeaderEnum.Minus3:
        objRet.minor3 = value;
        break;
      case PercentHeaderEnum.Minus2:
        objRet.minor2 = value;
        break;
      case PercentHeaderEnum.Minus1:
        objRet.minor1 = value;
        break;
      case PercentHeaderEnum.Mid:
        objRet.mid = value;
        break;
      case PercentHeaderEnum.Plus1:
        objRet.plus1 = value;
        break;
      case PercentHeaderEnum.Plus2:
        objRet.plus2 = value;
        break;
      case PercentHeaderEnum.Plus3:
        objRet.plus3 = value;
        break;
      case PercentHeaderEnum.Plus4:
        objRet.plus4 = value;
        break;
      case PercentHeaderEnum.Plus5:
        objRet.plus5 = value;
        break;
      case PercentHeaderEnum.Plus6:
        objRet.plus6 = value;
        break;
      default:
        break;
    }

    return objRet;
  }

  async createArrayGsm() {
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
        this.editData();
      });

      this.bsModalRef.content.onCancelEmitter.subscribe((res) => {
        this.showModalPosition.emit();
      });
    }
  }
}
