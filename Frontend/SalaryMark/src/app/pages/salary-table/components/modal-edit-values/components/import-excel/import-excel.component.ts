import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import locales from "@/locales/salary-table";
import {
  FormBuilder,
  FormGroup,
  ValidationErrors,
  Validators,
} from "@angular/forms";
import errors from "./common/errors";
import { BsModalRef } from "ngx-bootstrap/modal";
import { ExportCSVService } from "@/shared/services/export-csv/export-csv.service";
import { IEditSalarialTable } from "@/shared/models/editSalarialTable";
import { ISalaryTableResponse } from "@/shared/interfaces/positions";

@Component({
  selector: "app-import-excel",
  templateUrl: "./import-excel.component.html",
  styleUrls: ["./import-excel.component.scss"],
})
export class ImportExcelComponent implements OnInit {
  public formUpdate: FormGroup;

  @Output() eventErrors = new EventEmitter<any[]>();
  @Input() bsModalRef: BsModalRef;
  @Input() headersForTemplate: string[];
  @Input() data: IEditSalarialTable;
  @Input() salaryTables: ISalaryTableResponse[];
  @Input() tableId: string;

  public locales = locales;
  public errors = errors;
  public errorList: any[] = [];
  public importFile: File | null = null;
  constructor(
    private fb: FormBuilder,
    private salaryTableExportService: ExportCSVService
  ) {}

  ngOnInit(): void {
    this.createFormUpdate();
  }

  createFormUpdate() {
    const salaryTableName = this.salaryTables.find(
      (f) => f.id === this.tableId
    ).title;

    this.formUpdate = this.fb.group({
      tableName: [
        salaryTableName,
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
    });
  }
  updateData() {
    this.getFormValidationErrors();
    this.eventErrors.emit(this.errorList);
  }

  getFormValidationErrors() {
    this.errorList = [];
    Object.keys(this.formUpdate.controls).forEach((key) => {
      const controlErrors: ValidationErrors = this.formUpdate.get(key).errors;
      if (controlErrors != null) {
        Object.keys(controlErrors).forEach((keyError) => {
          const errorAdd = this.errors.find((error) => error.field === key);
          this.errorList.push(errorAdd);
        });
      }
    });
  }

  async downloadTemplate() {
    const salaryTable = this.salaryTables.find(
      (f) => f.id === this.tableId
    ).title;

    await this.salaryTableExportService.downloadExcelTemplate(
      this.headersForTemplate,
      this.data,
      this.salaryTables,
      salaryTable
    );
  }

  onImportUpdate(event: any) {
    if (event) {
      this.importFile = event[0];

      // TODO service para criar blob e upload
    }
  }

  removeFileImport() {
    this.importFile = null;
  }

  closeModal(e) {
    this.bsModalRef.hide();
    e.preventDefault();
  }
}
