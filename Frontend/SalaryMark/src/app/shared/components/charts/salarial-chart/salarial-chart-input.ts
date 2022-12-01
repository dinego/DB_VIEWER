export class SalaryChartInput {
  name = "";
  data: SalaryChartInputData[];
  type: number;
  gsm: number;
  constructor(result?: SalaryChartInput) {
    Object.assign(this, result);
  }
}

export class SalaryChartInputData {
  gsm: number;
  name: string;
  value: number;
  positions: Position[];
}

export class Position {
  id: number;
  occupantCLT: boolean;
  occupantPJ: boolean;
  positionDescription: string;
}
