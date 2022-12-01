import { IDefault } from "@/shared/interfaces/positions";

export class PositionSm {
  positionSm: string = "";
  positionIdByLibrary?: number = null;
  levelId?: number = null;
  groupId?: number = null;
  parameters: Parameter[] = [];
  tables: TablePosition[] = [];
}

export class Parameter {
  parameterId?: number = null;
  parameter?: string = "";
  paramSelectedId: number = null;
}

export class TablePosition {
  posTableId: number = 0;
  posUnitId: number = 0;
  gsm: number = 0;
}
