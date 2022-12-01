import { IParameter as IParameter } from "@/shared/interfaces/parameters";
import { IDefault, IUnit } from "@/shared/interfaces/positions";
import { IDefaultLoop } from "@/shared/interfaces/table-position";
import { CommonService } from "@/shared/services/common/common.service";
import {
  ChangeDetectorRef,
  Component,
  EventEmitter,
  OnInit,
  Output,
} from "@angular/core";
import {
  FormBuilder,
  FormGroup,
  ValidationErrors,
  Validators,
} from "@angular/forms";
import { BsModalRef } from "ngx-bootstrap/modal";
import { PositionSm } from "./common/position-sm";

@Component({
  selector: "app-modal-add-position",
  templateUrl: "./modal-add-position.component.html",
  styleUrls: ["./modal-add-position.component.scss"],
})
export class ModalAddPositionComponent implements OnInit {
  private fieldRequired = "Campo(s) obrigatório(os) não preenchido(os)";
  private fieldDuplicates = "Não é permitido campos duplicados";

  public title: string;
  public closeBtnName?: string;
  public errorList: any[] = [];
  public form: FormGroup;
  public positionSm: PositionSm = new PositionSm();
  public errors: any[] = [];
  public listParam: IDefault[] = [];
  public levels: IDefault[] = [];
  public librarySm: IDefault[] = [];
  public salaryTables: IDefaultLoop[] = [];
  public gsms: IDefaultLoop[] = [];
  public units: IDefaultLoop[] = [];

  @Output() saveEvent = new EventEmitter<any>();
  public profiles: IDefault[];

  constructor(
    private commonService: CommonService,
    public bsModalRef: BsModalRef,
    private fb: FormBuilder,
    private readonly changeDetectorRef: ChangeDetectorRef
  ) {}

  async ngOnInit() {
    this.getAllLevelsMock();
    this.getLibrarySmMock();
    this.getListGsmMock();
    this.getAllUnits();
    this.getAllSalaryTable();

    this.mockData();
    this.setMockToLists();

    this.createForm();
    this.addEmpyLinePosition();
  }

  public getListGsmMock() {
    this.gsms = [
      {
        id: "1",
        title: "1",
        index: null,
      },
      {
        id: "2",
        title: "2",
        index: null,
      },
      {
        id: "3",
        title: "3",
        index: null,
      },
      {
        id: "4",
        title: "4",
        index: null,
      },
    ];
  }

  public async getAllSalaryTable() {
    const item = await this.commonService.getAllSalaryTables(null).toPromise();
    if (item.tableSalaryResponses.length > 0) {
      this.salaryTables = item.tableSalaryResponses.map((m) => {
        return {
          id: m.id.toString(),
          title: m.title,
          index: null,
        };
      });
    }
  }

  getLibrarySmMock() {
    this.librarySm = [
      {
        id: "1",
        title: "Teste de Library",
      },
      {
        id: "2",
        title: "Outro teste de Library",
      },
    ];
  }

  getAllLevelsMock() {
    this.levels = [
      {
        id: "1",
        title: "Teste de Level",
      },
      {
        id: "2",
        title: "Outro teste",
      },
    ];
  }

  checkDuplicateTables() {
    let duplicates = [];
    const values = Array.from(this.positionSm.tables);

    values.forEach((el, i) => {
      values.forEach((element, index) => {
        if (i === index) return null;
        if (
          element.gsm === el.gsm &&
          element.posTableId === el.posTableId &&
          element.posUnitId === el.posUnitId
        ) {
          if (!duplicates.includes(el)) duplicates.push(el);
        }
      });
    });

    if (duplicates.length > 0) {
      this.errorList.push({
        message: this.fieldDuplicates,
      });
    }
  }

  setMockToLists() {
    const parametersMaped = this.positionSm.parameters.map((m) => {
      return {
        id: m.parameterId.toString(),
        title: m.parameter.toString(),
      };
    });

    this.listParam = parametersMaped;
  }

  ngAfterViewChecked(): void {
    this.changeDetectorRef.detectChanges();
  }

  public save() {
    this.form.updateValueAndValidity();

    this.errorList = [];
    this.getFormValidationErrors();
    this.checkDuplicateTables();

    if (this.errorList.length > 0) return;

    this.saveEvent.emit(this.form.value);
  }

  mockData() {
    this.positionSm.parameters = [
      {
        parameter: "Area",
        parameterId: 1,
        paramSelectedId: null,
      },
      {
        parameter: "Parâmetro 1",
        parameterId: 2,
        paramSelectedId: null,
      },
    ];
  }

  public async createForm() {
    this.form = this.fb.group({
      positionSm: this.fb.control(
        this.positionSm.positionSm,
        Validators.required
      ),
      positionIdByLibrary: this.fb.control(
        this.positionSm.positionIdByLibrary,
        Validators.required
      ),
      levelId: this.fb.control(this.positionSm.levelId, Validators.required),
      groupId: this.fb.control(this.positionSm.groupId, Validators.required),

      parameters: this.fb.array(
        this.positionSm.parameters.map((m) =>
          this.fb.group({
            parameterId: this.fb.control(m.parameterId),
            parameter: this.fb.control(m.parameter),
            paramSelectedId: this.fb.control(
              m.paramSelectedId,
              Validators.required
            ),
          })
        )
      ),
      tables: this.fb.array(
        this.positionSm.tables.map((m) =>
          this.fb.group({
            posTableId: this.fb.control(m.posTableId, Validators.required),
            posUnitId: this.fb.control(m.posUnitId, Validators.required),
            gsm: this.fb.control(m.gsm, Validators.required),
          })
        )
      ),
    });
  }

  addEmpyLinePosition() {
    this.positionSm.tables.push({
      gsm: null,
      posTableId: null,
      posUnitId: null,
    });

    this.form.controls["tables"]["controls"].push(
      this.fb.group({
        posTableId: this.fb.control(null, Validators.required),
        posUnitId: this.fb.control(null, Validators.required),
        gsm: this.fb.control(null, Validators.required),
      })
    );
  }

  getFormValidationErrors() {
    Object.keys(this.form.controls).forEach((key) => {
      const controlErrors: ValidationErrors = this.form.get(key).errors;
      if (controlErrors != null) {
        Object.keys(controlErrors).forEach((keyError) => {
          const errorAdd = this.errors.find((error) => error.field === key);
          this.errorList.push(errorAdd);
        });
      }
    });

    if (this.errorList.length > 0) {
      this.errorList.push({
        message: this.fieldRequired,
      });
    }
  }

  setParameterGroup(event: IParameter) {
    this.form.controls.parameters["controls"][event.groupName]["controls"][
      "paramSelectedId"
    ].setValue(event.id);
  }

  getLabelParameterSelected(index: number, value: number): string {
    return this.listParam.find((f) => f.id === value.toString())
      ? this.listParam.find((f) => f.id === value.toString()).title
      : "Selecione";
  }

  setProfile(event) {
    this.form.controls.groupId.setValue(event.id);
  }

  setLevels(event) {
    this.form.controls.levelId.setValue(event.id);
  }

  setLibrarySm(event) {
    this.form.controls.positionIdByLibrary.setValue(event.id);
  }

  setLoopValue(event: IDefaultLoop, field: string) {
    this.positionSm.tables[event.index][field] = event.id;

    this.form.patchValue({
      tables: this.positionSm.tables,
    });
  }

  removeLineRable(index) {
    if (index == 0) return;
    this.form.controls.tables["controls"].splice(index, 1);
    this.positionSm.tables.splice(index, 1);

    this.form.updateValueAndValidity();
  }

  async getAllUnits() {
    const units = await this.commonService.getAllUnits().toPromise();
    this.units = units.map((m) => {
      return {
        id: m.unitId.toString(),
        title: m.unit,
        index: null,
      };
    });
  }
}
