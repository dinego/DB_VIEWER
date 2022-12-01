import { PercentHeaderEnum } from "@/pages/salary-table/components/modal-edit-values/components/edit-tracks/common/tracks-enum";
import { copyObject } from "@/shared/common/functions";
import { ISalaryTableResponse } from "@/shared/interfaces/positions";
import { IEditSalarialTable } from "@/shared/models/editSalarialTable";
import { Header } from "@/shared/models/salary-table";
import {
  fillBasedOnValue,
  fillFinancialImpactBasedOnValue,
} from "@/utils/style-based-on-value";
import { Injectable } from "@angular/core";
import * as Excel from "exceljs";
import { NgxSpinnerService } from "ngx-spinner";
import { AppComponent } from "src/app/app.component";
import columnsTemplate from "./common/common-export-template";
import columnsTemplateAnalyse from "./common/common-export-template-analyse";
import columnsTemplateProposedMovements from "./common/common-export-template-analyse-distribution";
import columnsTemplateDistributionAnalyse from "./common/common-export-template-distribution-analyse";
import columnsTemplateFinancialImpact from "./common/common-export-template-financial-impact";
import columnsTemplatePositions from "./common/common-export-template-positions";
import columnsProposedMovements from "./common/common-export-template-proposed-movements";
import columnsTemplateSalaryTable from "./common/common-export-template-salary-table";
import {
  IBodyExport,
  IHeaderExport,
} from "./common/content-export-salary-table";
import { DashedRowPosition } from "./common/dashed-row-position";
import { IDataSalaryTable } from "./common/export-data-salary-table";
import {
  convertUrlToBase64,
  emptyRowBySize,
  generateHeader,
  generateHeaderImpact,
  getNumberByPercent,
} from "./common/functions-aux";
import {
  validatorCelIsNumeric,
  getBgColorByCol,
  getBgColorByColFinancialImpact,
  getBgColorBySubColFinancialImpact,
  constainsColumnForBold,
  getBgColorByColProposedMovements,
} from "./common/functions-auxs";
import { IExportData } from "./common/IExportData";
import dataTables from "./common/static-data-tables";

@Injectable()
export class ExportCSVService extends AppComponent {
  public isViewError: boolean = false;
  private titleColumns: string[];
  private sizeFilters: number;
  private staticDataTable = dataTables;
  private startRowConcret = 6;

  constructor(private _spinnerService: NgxSpinnerService) {
    super();
  }

  private async getImage(): Promise<any> {
    const IMAGE_PATH = "assets/imgs/svg/sm-logo-excel.png";
    return await convertUrlToBase64(IMAGE_PATH);
  }

  public async downloadExcelTemplate(
    columns: string[],
    data: IEditSalarialTable,
    salaryTables: ISalaryTableResponse[],
    tableNameSelected: string
  ) {
    let filterValue: string = "";
    const rowsByTable = this.setDataToRows(data, salaryTables, columns.length);

    const logo: any = await this.getImage();
    const workbook = new Excel.Workbook();
    const salarialTableRow = workbook.addWorksheet("Tabela Salarial", {
      pageSetup: {
        paperSize: 9,
        orientation: "landscape",
      },
      views: [{ state: "frozen", ySplit: 5, showGridLines: false }],
    });

    this.titleColumns = columns;
    this.sizeFilters = this.titleColumns.length;

    //inserindo imagem
    const imageId2 = workbook.addImage({
      base64: logo,
      extension: "png",
    });

    salarialTableRow.addImage(imageId2, {
      tl: { col: 0.2, row: 0.3 },
      ext: { width: 234, height: 30 },
    });

    salarialTableRow.columns = columnsTemplate;

    const SalarialTableRowContent = this.getListTemplateHeaders(
      this.staticDataTable.descriptionTemplate + tableNameSelected,
      this.addNumbersInRow4(),
      this.getDynamicColumns(),
      "* campos obrigatórios"
    );

    salarialTableRow.autoFilter =
      "A5:" + String.fromCharCode(96 + this.sizeFilters) + "5";

    SalarialTableRowContent.map((row: any) => {
      salarialTableRow.addRow(row);
    });

    (await rowsByTable).map((row: any[]) => {
      const rowFormated = Object.assign(row);
      salarialTableRow.addRow(rowFormated);
    });

    filterValue = filterValue.substring(0, 0) + '"';

    salaryTables.map((item) => {
      if (item.title === tableNameSelected)
        filterValue += `${item.id}-${item.title},`;
    });

    filterValue = filterValue +=
      filterValue.substring(filterValue.length, filterValue.length) + '"';

    const sizeInternal = this.sizeFilters;

    //interando linhas e determinando estilizacao
    salarialTableRow.eachRow(function (row, rowNumber) {
      row.height = rowNumber == 1 ? 34 : 24;

      //interando cada coluna da linha
      row.eachCell((cell, colNumber) => {
        cell.font = {
          size: rowNumber === 3 || rowNumber === 4 ? 8 : 12,
          name: "Calibri",
          color: {
            argb:
              rowNumber === 3 || rowNumber === 4
                ? "626262"
                : rowNumber === 5
                ? "ffffff"
                : "1f1f1f",
          },
          bold: rowNumber == 5 && colNumber <= sizeInternal,
        };

        cell.fill = {
          type: "pattern",
          pattern: "solid",
          fgColor: {
            argb:
              rowNumber == 5 && colNumber <= sizeInternal ? "7f7f7f" : "ffffff",
          },
        };

        cell.border = {
          top: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          left: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          bottom: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          right: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
        };

        cell.alignment = {
          vertical: "middle",
          horizontal: colNumber <= 2 ? "left" : "center",
          indent: 1,
        };

        cell.dataValidation =
          rowNumber > 5 && colNumber == 1
            ? {
                type: "list",
                allowBlank: true,
                formulae: [filterValue],
              }
            : undefined;

        if (colNumber > 2 && rowNumber > 5) cell.numFmt = "#,##0.00";

        if (rowNumber == 5 && colNumber > 2 && colNumber <= sizeInternal) {
          cell.value = parseInt(cell.value.toString()) / 100;
          cell.numFmt = "0%";
        }
      });
      row.commit();
    });

    //adiciono aspas duplas no inicio da string
    filterValue = filterValue.substring(0, 0) + '"';

    //adicioino as aspas duplas no final da string
    filterValue = filterValue +=
      filterValue.substring(filterValue.length, filterValue.length) + '"';
    this.makeDowload(workbook, "tabela_salarial_modelo");
  }

  public async downloadExcelSalaryTablePositions(
    columns: Header[],
    dataSalaryTablePositions: any[],
    tableTitle: string
  ) {
    this._spinnerService.show();

    const rowsByTable = this.setRowsToStringArray(dataSalaryTablePositions);
    this.sizeFilters = columns.length;

    const logo: any = await this.getImage();
    const workbook = new Excel.Workbook();

    //congelando paineis. Na declaracao do sheet, poderá passar parametros como cor, estilos tb
    const salarialTableRow = workbook.addWorksheet("Tabela Salarial", {
      pageSetup: {
        paperSize: 9,
        orientation: "landscape",
      },
      views: [{ state: "frozen", ySplit: 5, showGridLines: false }],
    });

    //inserindo imagem
    const imageId2 = workbook.addImage({
      base64: logo,
      extension: "png",
    });

    salarialTableRow.addImage(imageId2, {
      tl: { col: 0.2, row: 0.3 },
      ext: { width: 234, height: 30 },
    });

    //definindo colunas a serem populadas com suas dimensoes
    //header podera passar o nome do header como "nome", "email",etc
    // key define a chave para popular com interaçao
    salarialTableRow.columns = columnsTemplatePositions;

    //array gerado para popular as primeiras linhas, a quantidade de itens serao as colunas
    //caso queira que o valor da coluna da esquerda sobreponha a coluna da direita , a coluna da direita deve receber null
    const SalarialTableRowContent = this.getListTemplateHeaders(
      this.staticDataTable.descriptionSalaryTablePosition,
      emptyRowBySize(20),
      this.getDinamicColumnsPositions(columns),
      tableTitle
    );

    //adicionando filtros fixos (cinzas)
    salarialTableRow.autoFilter =
      "A5:" + String.fromCharCode(96 + this.sizeFilters) + "5";

    SalarialTableRowContent.map((row: any) => {
      salarialTableRow.addRow(row);
    });

    rowsByTable.map((row: any[]) => {
      salarialTableRow.addRow(row);
    });

    this.mergeCells(salarialTableRow, dataSalaryTablePositions);

    //fix para funcionar o tamanho de colunas dinâmicas
    const sizeInternal = columns.length;

    const backgroundRowsByGsm = this.getGsmGackground(dataSalaryTablePositions);

    //interando linhas e determinando estilizacao
    salarialTableRow.eachRow(function (row, rowNumber) {
      row.height = rowNumber == 1 ? 34 : 22;

      //interando cada coluna da linha
      row.eachCell((cell, colNumber) => {
        cell.font = {
          size: rowNumber === 3 || rowNumber === 4 ? 8 : 12,
          name: "Calibri",
          color: {
            argb:
              rowNumber === 3 || rowNumber === 4
                ? "626262"
                : rowNumber === 5
                ? "ffffff"
                : "1f1f1f",
          },
          bold: rowNumber == 5 && colNumber <= sizeInternal,
        };

        cell.fill = {
          type: "pattern",
          pattern: "solid",
          fgColor: {
            argb:
              rowNumber == 5 && colNumber <= sizeInternal ? "7f7f7f" : "ffffff",
          },
        };

        cell.border = {
          top: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          left: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          bottom: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          right: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
        };

        let colContainsValueNumber = validatorCelIsNumeric(cell);

        cell.alignment = {
          vertical: "middle",
          horizontal: !colContainsValueNumber
            ? cell.value.toString().toString() === "-"
              ? "center"
              : "left"
            : "center",
          indent: 1,
        };

        if (colNumber > 2 && colContainsValueNumber && rowNumber > 5) {
          cell.value = parseInt(cell.value.toString());
          cell.numFmt = "#,###";
        }

        if (
          rowNumber == 5 &&
          colContainsValueNumber &&
          colNumber <= sizeInternal
        ) {
          cell.value = parseInt(cell.value.toString()) / 100;
          cell.numFmt = "0%";
        }

        if (rowNumber > 5 && colNumber <= sizeInternal) {
          const gsmLocal =
            dataSalaryTablePositions &&
            dataSalaryTablePositions[rowNumber - 6] &&
            dataSalaryTablePositions[rowNumber - 6][0] &&
            dataSalaryTablePositions[rowNumber - 6][0].value
              ? parseInt(dataSalaryTablePositions[rowNumber - 6][0].value)
              : undefined;

          if (gsmLocal) {
            const backgroundByGsm = backgroundRowsByGsm.find((el) => {
              if (el.gsm === gsmLocal) return el;
            });
            cell.fill = {
              type: "pattern",
              pattern: "solid",
              fgColor: {
                argb: backgroundByGsm
                  ? backgroundByGsm.backgroundColor
                  : "FFFFFF",
              },
            };
          }
        }
      });

      row.commit();
    });

    this.makeDowload(workbook, "tabela_salarial" + tableTitle);
  }

  public async downloadExcelSalaryTable(
    columns: IHeaderExport[],
    dataSalaryTable: IBodyExport[],
    tableTitle: string,
    fileName: string
  ) {
    this._spinnerService.show();

    const rowsByTable = this.setRowsToStringArraySalaryTable(dataSalaryTable);
    this.sizeFilters = columns.length;

    const logo: any = await this.getImage();
    const workbook = new Excel.Workbook();

    //congelando paineis. Na declaracao do sheet, poderá passar parametros como cor, estilos tb
    const salarialTableRow = workbook.addWorksheet("Tabela Salarial", {
      pageSetup: {
        paperSize: 9,
        orientation: "landscape",
      },
      views: [{ state: "frozen", ySplit: 5, showGridLines: false }],
    });

    //inserindo imagem
    const imageId2 = workbook.addImage({
      base64: logo,
      extension: "png",
    });

    salarialTableRow.addImage(imageId2, {
      tl: { col: 0.2, row: 0.3 },
      ext: { width: 234, height: 30 },
    });

    //definindo colunas a serem populadas com suas dimensoes
    //header podera passar o nome do header como "nome", "email",etc
    // key define a chave para popular com interaçao
    salarialTableRow.columns = columnsTemplateSalaryTable;

    this.titleColumns = columns.map((m) => m.value);

    //array gerado para popular as primeiras linhas, a quantidade de itens serao as colunas
    //caso queira que o valor da coluna da esquerda sobreponha a coluna da direita , a coluna da direita deve receber null
    const SalarialTableRowContent = this.getListTemplateHeaders(
      this.staticDataTable.descriptionSalaryTablePosition,
      emptyRowBySize(20),
      this.getDynamicColumns(),
      tableTitle
    );

    //adicionando filtros fixos (cinzas)
    salarialTableRow.autoFilter =
      "A5:" + String.fromCharCode(96 + this.sizeFilters) + "5";

    SalarialTableRowContent.map((row: any) => {
      salarialTableRow.addRow(row);
    });

    rowsByTable.map((row: any[]) => {
      salarialTableRow.addRow(row);
    });

    //fix para funcionar o tamanho de colunas dinâmicas
    const sizeInternal = columns.length;

    //interando linhas e determinando estilizacao
    salarialTableRow.eachRow(function (row, rowNumber) {
      row.height = rowNumber == 1 ? 34 : 22;

      //interando cada coluna da linha
      row.eachCell((cell, colNumber) => {
        cell.font = {
          size: rowNumber === 3 || rowNumber === 4 ? 8 : 12,
          name: "Calibri",
          color: {
            argb:
              rowNumber === 3 || rowNumber === 4
                ? "626262"
                : rowNumber === 5
                ? "ffffff"
                : "1f1f1f",
          },
          bold: rowNumber == 5 && colNumber <= sizeInternal,
        };

        cell.fill = {
          type: "pattern",
          pattern: "solid",
          fgColor: {
            argb:
              rowNumber == 5 && colNumber <= sizeInternal ? "7f7f7f" : "ffffff",
          },
        };

        cell.border = {
          top: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          left: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          bottom: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          right: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
        };

        let colContainsValueNumber = validatorCelIsNumeric(cell);

        cell.alignment = {
          vertical: "middle",
          horizontal: !colContainsValueNumber
            ? cell.value.toString().toString() === "-"
              ? "center"
              : "left"
            : "center",
          indent: 1,
        };

        if (colNumber > 2 && colContainsValueNumber && rowNumber > 5) {
          const value = cell.value.toString().replace(",", "");

          cell.value = parseInt(value);
          cell.numFmt = "#,###";
        }

        if (
          rowNumber == 5 &&
          colContainsValueNumber &&
          colNumber <= sizeInternal
        ) {
          cell.value = parseInt(cell.value.toString()) / 100;
          cell.numFmt = "0%";
        }

        if (rowNumber > 5 && colNumber <= sizeInternal) {
          cell.fill = {
            type: "pattern",
            pattern: "solid",
            fgColor: {
              argb: rowNumber % 2 === 0 ? "FFFFFF" : "ededed",
            },
          };
        }
      });

      row.commit();
    });

    this.makeDowload(workbook, fileName);
  }

  mergeCells(
    salarialTableRow: Excel.Worksheet,
    dataSalaryTablePositions: any[]
  ) {
    let gsmRet: number[] = [];

    dataSalaryTablePositions.forEach((element) => {
      gsmRet.push(parseInt(element[0].value));
    });

    const filtered = [...new Set(gsmRet)];

    filtered.forEach((gsm) => {
      var indices = [];
      var idx = gsmRet.indexOf(gsm);

      while (idx != -1) {
        indices.push(idx);
        idx = gsmRet.indexOf(gsm, idx + 1);
      }

      const startIndex = indices[0] + this.startRowConcret;
      const lastIndex = gsmRet.lastIndexOf(gsm) + this.startRowConcret;
      try {
        salarialTableRow.mergeCells(`A${startIndex}:A${lastIndex}`);
      } catch {}
    });
  }

  getGsmGackground(dataSalaryTablePositions: any[]): DashedRowPosition[] {
    let gsmRet: number[] = [];

    dataSalaryTablePositions.forEach((element) => {
      gsmRet.push(parseInt(element[0].value));
    });

    gsmRet = [...new Set(gsmRet)];

    // header lines
    gsmRet.push(0);
    gsmRet.push(0);
    gsmRet.push(0);
    gsmRet.push(0);
    gsmRet.push(0);

    const dashedRowPosition = gsmRet.map((m, index) => {
      return {
        gsm: m,
        backgroundColor: index % 2 === 0 ? "FFFFFF" : "ededed",
      } as DashedRowPosition;
    });

    return dashedRowPosition;
  }

  setRowsToStringArray(dataTable: any[]): any[] {
    let rootArray: any[][] = [];

    dataTable.forEach((f) => {
      let arrayInner: any[] = [];

      Object.assign(f).forEach((element) => {
        arrayInner.push(
          isNaN(element.value) ? element.value : parseInt(element.value)
        );
      });

      rootArray.push(arrayInner);
    });

    return rootArray;
  }

  setRowsToStringArraySalaryTable(dataTable: any[]): any[] {
    let rootArray: any[][] = [];

    dataTable.forEach((f) => {
      let arrayInner: IDataSalaryTable[] = [];

      Object.assign(f).forEach((element) => {
        arrayInner.push(
          isNaN(element.value) ? element.value : parseInt(element.value)
        );
      });

      rootArray.push(arrayInner);
    });

    return rootArray;
  }

  setRowsToStringArrayAnalyse(dataTable: any[]): any[] {
    let rootArray: any[][] = [];

    dataTable.forEach((f) => {
      let arrayInner: any[] = [];

      Object.values(f).forEach((element: any) => {
        element.value = element.value === "" ? "-" : element.value;
        arrayInner.push(
          isNaN(element.value) ? element.value : parseInt(element.value)
        );
      });

      rootArray.push(arrayInner);
    });

    return rootArray;
  }

  setRowsToStringArrayAnalyseDistribution(dataTable: any[]): any[] {
    let rootArray: any[][] = [];

    dataTable.forEach((f) => {
      let arrayInner: any[] = [];

      Object.values(f).forEach((element: any) => {
        element.value = element.value === "" ? "-" : element.value;
        arrayInner.push(
          isNaN(element.value) ? element.value : parseInt(element.value)
        );
        arrayInner.push("");
      });

      rootArray.push(arrayInner);
    });

    return rootArray;
  }

  private async setDataToRows(
    data: IEditSalarialTable,
    salaryTables: ISalaryTableResponse[],
    columnsQuantity: number
  ): Promise<string[][]> {
    let rootArray = [];

    data.salaryTableValues.salaryTableValues.forEach((f) => {
      let arrayInner: any[] = [];

      arrayInner.push(`${salaryTables[0].id}-${salaryTables[0].title}`);

      arrayInner.push(f.gsm);
      if (data.rangeEdit.includes(PercentHeaderEnum.Minus6))
        arrayInner.push(f.minor6 ? this.objectRichText(f.minor6) : "");
      if (data.rangeEdit.includes(PercentHeaderEnum.Minus5))
        arrayInner.push(f.minor5 ? this.objectRichText(f.minor5) : "");
      if (data.rangeEdit.includes(PercentHeaderEnum.Minus4))
        arrayInner.push(f.minor4 ? this.objectRichText(f.minor4) : "");
      if (data.rangeEdit.includes(PercentHeaderEnum.Minus3))
        arrayInner.push(f.minor3 ? this.objectRichText(f.minor3) : "");
      if (data.rangeEdit.includes(PercentHeaderEnum.Minus2))
        arrayInner.push(f.minor2 ? this.objectRichText(f.minor2) : "");
      if (data.rangeEdit.includes(PercentHeaderEnum.Minus1))
        arrayInner.push(f.minor1 ? this.objectRichText(f.minor1) : "");
      if (data.rangeEdit.includes(PercentHeaderEnum.Mid))
        arrayInner.push(f.mid ? this.objectRichText(f.mid) : "");
      if (data.rangeEdit.includes(PercentHeaderEnum.Plus1))
        arrayInner.push(f.plus1 ? this.objectRichText(f.plus1) : "");
      if (data.rangeEdit.includes(PercentHeaderEnum.Plus2))
        arrayInner.push(f.plus2 ? this.objectRichText(f.plus2) : "");
      if (data.rangeEdit.includes(PercentHeaderEnum.Plus3))
        arrayInner.push(f.plus3 ? this.objectRichText(f.plus3) : "");
      if (data.rangeEdit.includes(PercentHeaderEnum.Plus4))
        arrayInner.push(f.plus4 ? this.objectRichText(f.plus4) : "");
      if (data.rangeEdit.includes(PercentHeaderEnum.Plus5))
        arrayInner.push(f.plus5 ? this.objectRichText(f.plus5) : "");
      if (data.rangeEdit.includes(PercentHeaderEnum.Plus6))
        arrayInner.push(f.plus6 ? this.objectRichText(f.plus6) : "");

      rootArray.push(arrayInner);
    });

    rootArray.push(...(await this.addWhiteLines(columnsQuantity)));

    return rootArray;
  }

  async addWhiteLines(length: number): Promise<any[]> {
    let rootArray = [];

    for (let index = 0; index < 1000; index++) {
      let arrayInner: any[] = [];
      for (let indexInner = 1; indexInner <= length; indexInner++) {
        arrayInner.push("");
      }
      rootArray.push(arrayInner);
    }

    return rootArray;
  }

  private getDinamicColumnsPositions(columnsWithHeader: Header[]): string[] {
    const columns = columnsWithHeader.map((m) => {
      return m.nickName.includes("%")
        ? parseInt(m.nickName.replace("%", ""))
        : m.nickName;
    });

    const columnNamesPopulate: string[] = Object.assign(columns);
    for (let index = columnNamesPopulate.length; index <= 20; index++) {
      columnNamesPopulate.push("");
    }

    return columnNamesPopulate;
  }

  private getDynamicColumns(): string[] {
    const columns = this.titleColumns.map((m) => {
      return m.includes("%") ? parseInt(m.replace("%", "")) : m;
    });

    const columnNamesPopulate: string[] = Object.assign(columns);
    for (let index = columnNamesPopulate.length; index <= 20; index++) {
      columnNamesPopulate.push("");
    }

    return columnNamesPopulate;
  }

  private getListTemplateHeaders(
    dinamicTitle: string,
    line4: string[],
    line5: string[],
    tableTitleFields: string
  ): any[] {
    return generateHeader(
      dinamicTitle,
      Object.assign(line4),
      Object.assign(line5),
      tableTitleFields
    );
  }

  private getListTemplateFinancialImpactHeaders(
    dinamicTitle: string,
    line4: string[],
    line5: string[],
    tableTitleFields: string
  ): any[] {
    return generateHeaderImpact(
      dinamicTitle,
      Object.assign(line4),
      Object.assign(line5),
      tableTitleFields
    );
  }

  private addNumbersInRow4(): string[] {
    let numbers: string[] = [];

    this.titleColumns.forEach((f: string) => {
      numbers.push("");
      //numbers.push(getNumberByPercent(f));
    });

    return numbers;
  }

  private objectRichText(track: number): any {
    return parseInt(track.toString());
  }

  public async downloadExcelComparativeAnalyseDetail(
    objAnalyseDetail: IExportData
  ) {
    this._spinnerService.show();

    const clone = copyObject(objAnalyseDetail.bodyData);

    const rowsByTable = this.setRowsToStringArrayAnalyse(clone);

    this.sizeFilters = objAnalyseDetail.columns.length;

    const logo: any = await this.getImage();
    const workbook = new Excel.Workbook();

    //congelando paineis. Na declaracao do sheet, poderá passar parametros como cor, estilos tb
    const tableRow = workbook.addWorksheet(objAnalyseDetail.tableTitle, {
      pageSetup: {
        paperSize: 9,
        orientation: "landscape",
      },
      views: [{ state: "frozen", xSplit: 1, showGridLines: false }],
    });

    //inserindo imagem
    const imageId2 = workbook.addImage({
      base64: logo,
      extension: "png",
    });

    tableRow.addImage(imageId2, {
      tl: { col: 0.2, row: 0.3 },
      ext: { width: 234, height: 30 },
    });

    tableRow.columns = columnsTemplateAnalyse;

    const tableRowContent = this.getListTemplateHeaders(
      `${this.staticDataTable.exportPipe} ${objAnalyseDetail.tableTitle}`,
      emptyRowBySize(20),
      this.getDinamicColumnsAnalyse(objAnalyseDetail.columns),
      objAnalyseDetail.scenario
    );

    //adicionando filtros fixos (cinzas)
    tableRow.autoFilter =
      "A5:" + String.fromCharCode(96 + this.sizeFilters) + "5";

    tableRowContent.map((row: any) => {
      tableRow.addRow(row);
    });

    rowsByTable.map((row: any[]) => {
      tableRow.addRow(row);
    });

    //fix para funcionar o tamanho de colunas dinâmicas
    const sizeInternal = objAnalyseDetail.columns.length;

    //interando linhas e determinando estilizacao
    tableRow.eachRow(function (row, rowNumber) {
      row.height = rowNumber == 1 ? 34 : 22;

      //interando cada coluna da linha
      row.eachCell((cell, colNumber) => {
        cell.font = {
          size: rowNumber === 3 || rowNumber === 4 ? 8 : 12,
          name: "Calibri",
          color: {
            argb:
              rowNumber === 3 || rowNumber === 4
                ? "626262"
                : rowNumber === 5
                ? "ffffff"
                : "1f1f1f",
          },
          bold: rowNumber == 5 && colNumber <= sizeInternal,
        };

        cell.fill = {
          type: "pattern",
          pattern: "solid",
          fgColor: {
            argb:
              rowNumber == 5 && colNumber <= sizeInternal ? "7f7f7f" : "ffffff",
          },
        };

        cell.border = {
          top: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          left: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          bottom: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          right: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
        };

        const colContainsValueNumber = validatorCelIsNumeric(cell);

        cell.alignment = {
          vertical: "middle",
          horizontal: !colContainsValueNumber ? "left" : "center",
          indent: 1,
        };

        if (colNumber > 2 && colContainsValueNumber && rowNumber > 5) {
          cell.value = parseInt(cell.value.toString());
          cell.numFmt = "#,###";
        }

        if (
          rowNumber == 5 &&
          colContainsValueNumber &&
          colNumber <= sizeInternal
        ) {
          cell.value = parseInt(cell.value.toString()) / 100;
          cell.numFmt = "0%";
        }

        if (colNumber === sizeInternal && rowNumber > 5) {
          const colorData = fillBasedOnValue(
            parseInt(cell.value.toString().toString())
          );

          cell.font = {
            color: {
              argb: colorData.color,
            },
          };

          cell.fill = {
            type: "pattern",
            pattern: "solid",
            fgColor: {
              argb: colorData.backgroundColor,
            },
          };

          cell.value = parseInt(cell.value.toString().toString()) / 100;
          cell.numFmt = "0%";
        }
      });

      row.commit();
    });

    this.makeDowload(
      workbook,
      `${objAnalyseDetail.tableTitle} - ${objAnalyseDetail.scenario}`
    );
  }

  private getDinamicColumnsAnalyse(columnsWithHeader: any[]): string[] {
    const columns = columnsWithHeader.map((m) => {
      return m.value.includes("%")
        ? parseInt(m.value.replace("%", ""))
        : m.value;
    });

    const columnNamesPopulate: string[] = Object.assign(columns);
    for (let index = columnNamesPopulate.length; index <= 20; index++) {
      columnNamesPopulate.push("");
    }

    return columnNamesPopulate;
  }

  public async downloadExcelComparativeAnalyseDistribution(
    objAnalyseDetail: IExportData
  ) {
    this._spinnerService.show();

    const clone = copyObject(objAnalyseDetail.bodyData);

    const rowsByTable = this.setRowsToStringArrayAnalyseDistribution(clone);

    this.sizeFilters = objAnalyseDetail.columns.length;

    const logo: any = await this.getImage();
    const workbook = new Excel.Workbook();

    //congelando paineis. Na declaracao do sheet, poderá passar parametros como cor, estilos tb
    const tableRow = workbook.addWorksheet(objAnalyseDetail.tableTitle, {
      pageSetup: {
        paperSize: 9,
        orientation: "landscape",
      },
      views: [{ state: "frozen", ySplit: 5, showGridLines: false }],
    });

    //inserindo imagem
    const logoSM = workbook.addImage({
      base64: logo,
      extension: "png",
    });

    tableRow.addImage(logoSM, {
      tl: { col: 0.2, row: 0.3 },
      ext: { width: 234, height: 30 },
    });

    tableRow.columns = columnsTemplateDistributionAnalyse;
    objAnalyseDetail.columns = this.setColumnsDistribution(
      objAnalyseDetail.columns
    );

    const tableRowContent = this.getListTemplateHeaders(
      `${this.staticDataTable.exportPipe} ${objAnalyseDetail.tableTitle} - ${objAnalyseDetail.unit}`,
      emptyRowBySize(20),
      objAnalyseDetail.columns,
      objAnalyseDetail.scenario
    );

    tableRowContent.map((row: any) => {
      tableRow.addRow(row);
    });

    rowsByTable.map((row: any[]) => {
      tableRow.addRow(row);
    });

    //fix para funcionar o tamanho de colunas dinâmicas
    const sizeInternal = objAnalyseDetail.columns.length;

    //interando linhas e determinando estilizacao
    tableRow.eachRow(function (row, rowNumber) {
      row.height =
        rowNumber > 1 && rowNumber <= 4
          ? 18
          : rowNumber == 1
          ? 34
          : rowNumber === 5
          ? 35
          : 32;

      //interando cada coluna da linha
      row.eachCell((cell, colNumber) => {
        cell.font = {
          size:
            rowNumber === 3 || rowNumber === 4 ? 8 : rowNumber === 5 ? 12 : 10,
          name: "Calibri",
          color: {
            argb:
              rowNumber === 3 || rowNumber === 4
                ? "626262"
                : rowNumber === 5
                ? "ffffff"
                : "1f1f1f",
          },
          bold: rowNumber == 5 && colNumber <= sizeInternal,
        };

        cell.fill = {
          type: "pattern",
          pattern: "solid",
          fgColor: {
            argb:
              rowNumber == 5 && colNumber <= sizeInternal
                ? getBgColorByCol(colNumber)
                : "ffffff",
          },
        };

        cell.border = {
          top: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 || colNumber % 2 === 0
                  ? "FFFFFF"
                  : "bfbfbf",
            },
          },
          left: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 || colNumber % 2 === 0
                  ? "FFFFFF"
                  : "bfbfbf",
            },
          },
          bottom: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 || colNumber % 2 === 0
                  ? "FFFFFF"
                  : "bfbfbf",
            },
          },
          right: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 || colNumber % 2 === 0
                  ? "FFFFFF"
                  : "bfbfbf",
            },
          },
        };

        const colContainsValueNumber = validatorCelIsNumeric(cell);

        cell.alignment = {
          vertical: "middle",
          horizontal: rowNumber > 5 && colNumber !== 1 ? "center" : "left",
          indent: 1,
        };

        if (colNumber > 2 && colContainsValueNumber && rowNumber > 5) {
          cell.value = parseInt(cell.value.toString());
          cell.numFmt = "#,###";
        }

        if (
          rowNumber == 5 &&
          colContainsValueNumber &&
          colNumber <= sizeInternal
        ) {
          cell.value = parseInt(cell.value.toString()) / 100;
          cell.numFmt = "0%";
        }

        if (
          rowNumber > 5 &&
          colNumber <= sizeInternal &&
          colNumber % 2 !== 0 &&
          objAnalyseDetail.bodyData[rowNumber - 6] &&
          objAnalyseDetail.bodyData[rowNumber - 6][0] &&
          objAnalyseDetail.bodyData[rowNumber - 6][0].bold
        ) {
          cell.font = {
            bold: true,
            size: 12,
          };

          cell.fill = {
            type: "pattern",
            pattern: "solid",
            fgColor: {
              argb: "FDECE3",
            },
          };
        }

        const testNaN = isNaN(parseInt(cell.value.toString().toString()));
        if (colNumber === 5 && !testNaN) {
          cell.value = parseInt(cell.value.toString().toString()) / 100;
          cell.numFmt = "0%";
        }
      });

      row.commit();
    });

    this.makeDowload(
      workbook,
      `${objAnalyseDetail.tableTitle} - ${objAnalyseDetail.scenario}`
    );
  }

  public async downloadExcelFinancialImpactModal(
    objAnalyseDetail: IExportData
  ) {
    this._spinnerService.show();

    const clone = copyObject(objAnalyseDetail.bodyData);

    const rowsByTable = this.setRowsToStringArrayAnalyse(clone);

    this.sizeFilters = objAnalyseDetail.columns.length;

    const logo: any = await this.getImage();
    const workbook = new Excel.Workbook();

    //congelando paineis. Na declaracao do sheet, poderá passar parametros como cor, estilos tb
    const tableRow = workbook.addWorksheet(objAnalyseDetail.tableTitle, {
      pageSetup: {
        paperSize: 9,
        orientation: "landscape",
      },
      views: [{ state: "frozen", xSplit: 1, showGridLines: false }],
    });

    //inserindo imagem
    const imageId2 = workbook.addImage({
      base64: logo,
      extension: "png",
    });

    tableRow.addImage(imageId2, {
      tl: { col: 0.2, row: 0.3 },
      ext: { width: 234, height: 30 },
    });

    tableRow.columns = columnsTemplateAnalyse;

    const tableRowContent = this.getListTemplateHeaders(
      `${this.staticDataTable.exportPipe} ${objAnalyseDetail.tableTitle}`,
      emptyRowBySize(20),
      this.getDinamicColumnsAnalyse(objAnalyseDetail.columns),
      objAnalyseDetail.scenario
    );

    //adicionando filtros fixos (cinzas)
    tableRow.autoFilter =
      "A5:" + String.fromCharCode(96 + this.sizeFilters) + "5";

    tableRowContent.map((row: any) => {
      tableRow.addRow(row);
    });

    rowsByTable.map((row: any[]) => {
      tableRow.addRow(row);
    });

    //fix para funcionar o tamanho de colunas dinâmicas
    const sizeInternal = objAnalyseDetail.columns.length;

    //interando linhas e determinando estilizacao
    tableRow.eachRow(function (row, rowNumber) {
      row.height = rowNumber == 1 ? 34 : 22;

      //interando cada coluna da linha
      row.eachCell((cell, colNumber) => {
        cell.font = {
          size: rowNumber === 3 || rowNumber === 4 ? 8 : 12,
          name: "Calibri",
          color: {
            argb:
              rowNumber === 3 || rowNumber === 4
                ? "626262"
                : rowNumber === 5
                ? "ffffff"
                : "1f1f1f",
          },
          bold: rowNumber == 5 && colNumber <= sizeInternal,
        };

        cell.fill = {
          type: "pattern",
          pattern: "solid",
          fgColor: {
            argb:
              rowNumber == 5 && colNumber <= sizeInternal ? "7f7f7f" : "ffffff",
          },
        };

        cell.border = {
          top: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          left: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          bottom: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          right: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
        };

        const colContainsValueNumber = validatorCelIsNumeric(cell);

        cell.alignment = {
          vertical: "middle",
          horizontal: !colContainsValueNumber ? "left" : "center",
          indent: 1,
        };

        if (colNumber > 2 && colContainsValueNumber && rowNumber > 5) {
          cell.value = parseInt(cell.value.toString());
          cell.numFmt = "#,###";
        }

        if (
          rowNumber == 5 &&
          colContainsValueNumber &&
          colNumber <= sizeInternal
        ) {
          cell.value = parseInt(cell.value.toString()) / 100;
          cell.numFmt = "0%";
        }

        if (colNumber === sizeInternal && rowNumber > 5) {
          const colorData: string = fillFinancialImpactBasedOnValue(
            parseInt(cell.value.valueOf().toString())
          );

          cell.font = {
            color: {
              argb: "1f1f1f",
            },
            size: 12,
          };

          cell.fill = {
            type: "pattern",
            pattern: "solid",
            fgColor: {
              argb: colorData,
            },
          };

          cell.value = parseInt(cell.value.toString());
          cell.numFmt = "#,###";
        }
      });

      row.commit();
    });

    this.makeDowload(
      workbook,
      `${objAnalyseDetail.tableTitle} - ${objAnalyseDetail.scenario}`
    );
  }

  public async downloadExcelProposedMovementsModal(
    objAnalyseDetail: IExportData
  ) {
    this._spinnerService.show();

    const clone = copyObject(objAnalyseDetail.bodyData);

    const rowsByTable = this.setRowsToStringArrayAnalyse(clone);

    this.sizeFilters = objAnalyseDetail.columns.length;

    const logo: any = await this.getImage();
    const workbook = new Excel.Workbook();

    //congelando paineis. Na declaracao do sheet, poderá passar parametros como cor, estilos tb
    const tableRow = workbook.addWorksheet(objAnalyseDetail.tableTitle, {
      pageSetup: {
        paperSize: 9,
        orientation: "landscape",
      },
      views: [{ state: "frozen", xSplit: 1, showGridLines: false }],
    });

    //inserindo imagem
    const imageId2 = workbook.addImage({
      base64: logo,
      extension: "png",
    });

    tableRow.addImage(imageId2, {
      tl: { col: 0.2, row: 0.3 },
      ext: { width: 234, height: 30 },
    });

    tableRow.columns = columnsProposedMovements;

    const tableRowContent = this.getListTemplateHeaders(
      `${this.staticDataTable.exportPipe} ${objAnalyseDetail.tableTitle}`,
      emptyRowBySize(20),
      this.getDinamicColumnsAnalyse(objAnalyseDetail.columns),
      objAnalyseDetail.scenario
    );

    //adicionando filtros fixos (cinzas)
    tableRow.autoFilter =
      "A5:" + String.fromCharCode(96 + this.sizeFilters) + "5";

    tableRowContent.map((row: any) => {
      tableRow.addRow(row);
    });

    rowsByTable.map((row: any[]) => {
      tableRow.addRow(row);
    });

    //fix para funcionar o tamanho de colunas dinâmicas
    const sizeInternal = objAnalyseDetail.columns.length;

    //interando linhas e determinando estilizacao
    tableRow.eachRow(function (row, rowNumber) {
      row.height = rowNumber == 1 ? 34 : 22;

      //interando cada coluna da linha
      row.eachCell((cell, colNumber) => {
        cell.font = {
          size: rowNumber === 3 || rowNumber === 4 ? 8 : 12,
          name: "Calibri",
          color: {
            argb:
              rowNumber === 3 || rowNumber === 4
                ? "626262"
                : rowNumber === 5
                ? "ffffff"
                : "1f1f1f",
          },
          bold: rowNumber == 5 && colNumber <= sizeInternal,
        };

        cell.fill = {
          type: "pattern",
          pattern: "solid",
          fgColor: {
            argb:
              rowNumber == 5 && colNumber <= sizeInternal
                ? "7f7f7f"
                : rowNumber > 5
                ? rowNumber % 2 === 0
                  ? "EDEDED"
                  : "ffffff"
                : "FFFFFF",
          },
        };

        cell.border = {
          top: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          left: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          bottom: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
          right: {
            style: "thin",
            color: {
              argb:
                colNumber > sizeInternal || rowNumber < 5 ? "FFFFFF" : "bfbfbf",
            },
          },
        };

        const colContainsValueNumber = validatorCelIsNumeric(cell);

        cell.alignment = {
          vertical: "middle",
          horizontal: !colContainsValueNumber ? "left" : "center",
          indent: 1,
        };

        if (colNumber > 2 && colContainsValueNumber && rowNumber > 5) {
          cell.value = parseInt(cell.value.toString());
          cell.numFmt = "#,###";
        }

        if (
          rowNumber == 5 &&
          colContainsValueNumber &&
          colNumber <= sizeInternal
        ) {
          cell.value = parseInt(cell.value.toString()) / 100;
          cell.numFmt = "0%";
        }

        if (colNumber === sizeInternal && rowNumber > 5) {
          cell.font = {
            color: {
              argb: "1f1f1f",
            },
            size: 12,
          };
        }
      });

      tableRow.columns.forEach(function (column, i) {
        if (i < 8) return;

        var maxLength = 0;
        column["eachCell"]({ includeEmpty: true }, function (cell) {
          var columnLength = cell.value ? cell.value.toString().length : 12;
          if (columnLength > maxLength) {
            maxLength = columnLength;
          }
        });
        column.width = maxLength < 10 ? 12 : maxLength;
      });

      row.commit();
    });

    this.makeDowload(
      workbook,
      `${objAnalyseDetail.tableTitle} - ${objAnalyseDetail.scenario}`
    );
  }

  setColumnsDistribution(columns: string[]): string[] {
    const columnsRet: string[] = [];
    columns.forEach((col, index) => {
      columnsRet.push(col);
      if (index < columns.length - 1) columnsRet.push("");
    });

    return columnsRet;
  }

  public async downloadExcelFinancialImpact(objFinancialImpact: IExportData) {
    this._spinnerService.show();

    const clone = copyObject(objFinancialImpact.bodyData);

    const rowsByTable = this.setDataBodyWithSpaces(clone);

    const logo: any = await this.getImage();
    const workbook = new Excel.Workbook();

    //congelando paineis. Na declaracao do sheet, poderá passar parametros como cor, estilos tb
    const tableRow = workbook.addWorksheet(objFinancialImpact.tableTitle, {
      pageSetup: {
        paperSize: 9,
        orientation: "landscape",
      },
      views: [{ showGridLines: false }],
    });

    //inserindo imagem
    const logoSM = workbook.addImage({
      base64: logo,
      extension: "png",
    });

    tableRow.addImage(logoSM, {
      tl: { col: 0.2, row: 0.3 },
      ext: { width: 234, height: 30 },
    });

    tableRow.columns = columnsTemplateFinancialImpact;
    objFinancialImpact.columns = this.setColumnsFinancialImpact(
      objFinancialImpact.columns
    );

    const tableRowContent = this.getListTemplateFinancialImpactHeaders(
      `${this.staticDataTable.exportPipe} ${objFinancialImpact.tableTitle} - ${objFinancialImpact.unit}`,
      emptyRowBySize(20),
      objFinancialImpact.columns,
      objFinancialImpact.scenario
    );

    const subHeaderSize = tableRowContent.length;
    const headerSize = tableRowContent.length + 1;

    tableRowContent.map((row: any) => {
      tableRow.addRow(row);
    });

    rowsByTable.map((row: any[]) => {
      tableRow.addRow(row);
    });

    //fix para funcionar o tamanho de colunas dinâmicas
    const sizeInternal = objFinancialImpact.columns.length;

    //interando linhas e determinando estilizacao
    tableRow.eachRow(function (row, rowNumber) {
      row.height =
        rowNumber > 1 && rowNumber <= 4
          ? 18
          : rowNumber == 1
          ? 34
          : rowNumber === subHeaderSize
          ? 35
          : rowNumber === headerSize
          ? 30
          : 32;

      //interando cada coluna da linha
      row.eachCell((cell, colNumber) => {
        cell.font = {
          size:
            rowNumber === 3 || rowNumber === 4
              ? 8
              : rowNumber === subHeaderSize
              ? 12
              : 11,
          name: "Calibri",
          color: {
            argb:
              rowNumber === 3 || rowNumber === 4
                ? "626262"
                : (rowNumber === subHeaderSize || rowNumber === headerSize) &&
                  colNumber > 2
                ? "FFFFFF"
                : rowsByTable.length + headerSize === rowNumber &&
                  cell.value.toString()
                ? "FFFFFF"
                : "1f1f1f",
          },
          bold:
            (constainsColumnForBold(colNumber) && rowNumber > headerSize) ||
            (rowNumber === subHeaderSize && colNumber <= sizeInternal) ||
            rowNumber === rowsByTable.length + headerSize,
        };

        cell.fill = {
          type: "pattern",
          pattern: "solid",
          fgColor: {
            argb:
              rowNumber === subHeaderSize && cell.value.toString()
                ? getBgColorByColFinancialImpact(colNumber)
                : rowNumber === headerSize && cell.value.toString()
                ? getBgColorBySubColFinancialImpact(colNumber)
                : rowsByTable.length + headerSize === rowNumber &&
                  cell.value.toString()
                ? "595959"
                : rowNumber % 2 === 0 &&
                  rowNumber > headerSize &&
                  cell.value.toString()
                ? "EDEDED"
                : "ffffff",
          },
        };

        cell.border = {
          top: {
            style: "thin",
            color: {
              argb:
                cell.value.toString() && rowNumber > headerSize
                  ? "bfbfbf"
                  : rowNumber === headerSize && cell.value.toString()
                  ? getBgColorBySubColFinancialImpact(colNumber)
                  : rowNumber === subHeaderSize && cell.value.toString()
                  ? getBgColorByColFinancialImpact(colNumber)
                  : "FFFFFF",
            },
          },
          left: {
            style: "thin",
            color: {
              argb:
                cell.value.toString() && rowNumber > headerSize
                  ? "bfbfbf"
                  : rowNumber === headerSize && cell.value.toString()
                  ? getBgColorBySubColFinancialImpact(colNumber)
                  : rowNumber === subHeaderSize && cell.value.toString()
                  ? getBgColorByColFinancialImpact(colNumber)
                  : "FFFFFF",
            },
          },
          bottom: {
            style: "thin",
            color: {
              argb:
                cell.value.toString() && rowNumber > headerSize
                  ? "bfbfbf"
                  : rowNumber === headerSize && cell.value.toString()
                  ? getBgColorBySubColFinancialImpact(colNumber)
                  : rowNumber === subHeaderSize && cell.value.toString()
                  ? getBgColorByColFinancialImpact(colNumber)
                  : "FFFFFF",
            },
          },
          right: {
            style: "thin",
            color: {
              argb:
                cell.value.toString() && rowNumber > headerSize
                  ? "bfbfbf"
                  : rowNumber === headerSize && cell.value.toString()
                  ? getBgColorBySubColFinancialImpact(colNumber)
                  : rowNumber === subHeaderSize && cell.value.toString()
                  ? getBgColorByColFinancialImpact(colNumber)
                  : "FFFFFF",
            },
          },
        };

        const colContainsValueNumber = validatorCelIsNumeric(cell);

        cell.alignment = {
          vertical:
            rowNumber === subHeaderSize && colNumber === 1 ? "top" : "middle",
          horizontal:
            rowNumber > subHeaderSize && colNumber !== 1 ? "center" : "left",
          indent: colNumber === 1 ? 1 : undefined,
        };

        if (
          colContainsValueNumber &&
          cell.value.toString() !== "0" &&
          rowNumber > headerSize &&
          (colNumber === 4 ||
            colNumber === 8 ||
            colNumber === 12 ||
            colNumber === 16)
        ) {
          cell.value = parseInt(cell.value.toString());
          cell.numFmt = "#,###";
        }

        if (
          rowNumber > headerSize &&
          colContainsValueNumber &&
          (colNumber === 5 ||
            colNumber === 9 ||
            colNumber === 13 ||
            colNumber === 17)
        ) {
          cell.value = parseFloat(cell.value.toString()) / 100;
          cell.numFmt = "0.0%";
        }
      });
      row.commit();
    });

    tableRow.mergeCells("A5:A6");
    tableRow.mergeCells("C5:E5");
    tableRow.mergeCells("G5:I5");
    tableRow.mergeCells("K5:M5");
    tableRow.mergeCells("O5:Q5");
    tableRow.getCell("C5").alignment = {
      wrapText: true,
      vertical: "middle",
      horizontal: "center",
    };
    tableRow.getCell("G5").alignment = {
      wrapText: true,
      vertical: "middle",
      horizontal: "center",
    };
    tableRow.getCell("K5").alignment = {
      wrapText: true,
      vertical: "middle",
      horizontal: "center",
    };
    tableRow.getCell("O5").alignment = {
      wrapText: true,
      vertical: "middle",
      horizontal: "center",
    };

    this.makeDowload(
      workbook,
      `${objFinancialImpact.tableTitle} - ${objFinancialImpact.scenario}`
    );
  }

  setDataBodyWithSpaces(clone: any[][]) {
    const bodyData: any[][] = [];

    clone.forEach((f) => {
      const arrayForInside: any[] = [];
      f.forEach((arr, index) => {
        arrayForInside.push(arr);
        if (index === 0) arrayForInside.push("");

        if (index % 3 === 0 && index > 2) arrayForInside.push("");
      });

      bodyData.push(arrayForInside);
    });

    return bodyData;
  }

  setDataBodyWithOneSpace(clone: any[][]) {
    const bodyData: any[][] = [];

    clone.forEach((f) => {
      const arrayForInside: any[] = [];
      f.forEach((arr, index) => {
        arrayForInside.push(arr);
        if (index < clone[index].length - 1) arrayForInside.push("");
      });

      bodyData.push(arrayForInside);
    });

    return bodyData;
  }

  public async downloadExcelProposedMovements(
    objProposedMovements: IExportData
  ) {
    this._spinnerService.show();

    const clone = copyObject(objProposedMovements.bodyData);

    const rowsByTable = this.setDataBodyWithOneSpace(clone);

    const logo: any = await this.getImage();
    const workbook = new Excel.Workbook();

    //congelando paineis. Na declaracao do sheet, poderá passar parametros como cor, estilos tb
    const tableRow = workbook.addWorksheet(objProposedMovements.tableTitle, {
      pageSetup: {
        paperSize: 9,
        orientation: "landscape",
      },
      views: [{ showGridLines: false }],
    });

    //inserindo imagem
    const logoSM = workbook.addImage({
      base64: logo,
      extension: "png",
    });

    tableRow.addImage(logoSM, {
      tl: { col: 0.2, row: 0.3 },
      ext: { width: 234, height: 30 },
    });

    tableRow.columns = columnsTemplateProposedMovements;
    objProposedMovements.columns = this.setColumnsProposedMovements(
      objProposedMovements.columns
    );

    const tableRowContent = this.getListTemplateHeaders(
      `${this.staticDataTable.exportPipe} ${objProposedMovements.tableTitle} - ${objProposedMovements.unit}`,
      emptyRowBySize(20),
      objProposedMovements.columns,
      objProposedMovements.scenario
    );

    tableRowContent.map((row: any) => {
      tableRow.addRow(row);
    });

    const headerSize = tableRowContent.length + 1;

    rowsByTable.map((row: any[]) => {
      tableRow.addRow(row);
    });

    //fix para funcionar o tamanho de colunas dinâmicas
    const sizeInternal = objProposedMovements.columns.length;

    //interando linhas e determinando estilizacao
    tableRow.eachRow(function (row, rowNumber) {
      row.height =
        rowNumber > 1 && rowNumber <= 4
          ? 18
          : rowNumber == 1
          ? 34
          : rowNumber === headerSize
          ? 35
          : 32;

      //interando cada coluna da linha
      row.eachCell((cell, colNumber) => {
        cell.font = {
          size:
            rowNumber === 3 || rowNumber === 4 ? 8 : rowNumber === 5 ? 12 : 11,
          name: "Calibri",
          color: {
            argb:
              rowNumber === 3 || rowNumber === 4
                ? "626262"
                : (rowNumber === headerSize || rowNumber === headerSize) &&
                  colNumber > 2
                ? "FFFFFF"
                : rowsByTable.length + headerSize === rowNumber &&
                  cell.value.toString()
                ? "FFFFFF"
                : "1f1f1f",
          },
          bold:
            (constainsColumnForBold(colNumber) && rowNumber >= 6) ||
            (rowNumber == headerSize && colNumber <= sizeInternal) ||
            rowNumber === rowsByTable.length + headerSize,
        };

        cell.fill = {
          type: "pattern",
          pattern: "solid",
          fgColor: {
            argb:
              rowNumber === 5 && cell.value.toString()
                ? getBgColorByColProposedMovements(colNumber)
                : rowsByTable.length + headerSize === rowNumber &&
                  cell.value.toString()
                ? "595959"
                : rowNumber % 2 === 0 && rowNumber >= 6 && cell.value.toString()
                ? "EDEDED"
                : "ffffff",
          },
        };

        cell.border = {
          top: {
            style: "thin",
            color: {
              argb:
                cell.value.toString() && rowNumber >= 6
                  ? "bfbfbf"
                  : rowNumber === headerSize && cell.value.toString()
                  ? getBgColorByColProposedMovements(colNumber)
                  : "FFFFFF",
            },
          },
          left: {
            style: "thin",
            color: {
              argb:
                cell.value.toString() && rowNumber >= 6
                  ? "bfbfbf"
                  : rowNumber === headerSize && cell.value.toString()
                  ? getBgColorByColProposedMovements(colNumber)
                  : "FFFFFF",
            },
          },
          bottom: {
            style: "thin",
            color: {
              argb:
                cell.value.toString() && rowNumber >= 6
                  ? "bfbfbf"
                  : rowNumber === headerSize && cell.value.toString()
                  ? getBgColorByColProposedMovements(colNumber)
                  : "FFFFFF",
            },
          },
          right: {
            style: "thin",
            color: {
              argb:
                cell.value.toString() && rowNumber >= 6
                  ? "bfbfbf"
                  : rowNumber === headerSize && cell.value.toString()
                  ? getBgColorByColProposedMovements(colNumber)
                  : "FFFFFF",
            },
          },
        };

        const colContainsValueNumber = validatorCelIsNumeric(cell);

        cell.alignment = {
          vertical:
            rowNumber === headerSize && colNumber === 1 ? "top" : "middle",
          horizontal:
            rowNumber >= headerSize && colNumber !== 1 ? "center" : "left",
          indent: colNumber === 1 ? 1 : undefined,
        };

        if (
          rowNumber >= 6 &&
          colContainsValueNumber &&
          (colNumber === 3 || colNumber === headerSize || colNumber === 7)
        ) {
          cell.value = parseInt(cell.value.toString()) / 100;
          cell.numFmt = "0%";
        }
      });
      row.commit();
    });

    this.makeDowload(
      workbook,
      `${objProposedMovements.tableTitle} - ${objProposedMovements.scenario}`
    );
  }

  setColumnsFinancialImpact(columns: any[]): string[] {
    const columnRet: string[] = [];
    columns.forEach((f, index) => {
      columnRet.push(f);

      if (index >= 1 && index <= columns.length - 1) {
        columnRet.push("");
        columnRet.push("");
      }

      if (index <= columns.length - 1) columnRet.push("");
    });

    return columnRet;
  }

  setColumnsProposedMovements(columns: any[]): string[] {
    const columnRet: string[] = [];
    columns.forEach((f, index) => {
      columnRet.push(f);
      if (index <= columns.length - 1) columnRet.push("");
    });

    return columnRet;
  }

  private makeDowload(workbook, name: string): void {
    workbook.xlsx.writeBuffer().then((data: any) => {
      const blob = new Blob([data], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      });
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      document.body.appendChild(a);
      a.setAttribute("style", "display: none");
      a.href = url;
      a.download = `${name}.xlsx`;
      a.click();
      window.URL.revokeObjectURL(url);
      a.remove();
    });
    this._spinnerService.hide();
  }
}
