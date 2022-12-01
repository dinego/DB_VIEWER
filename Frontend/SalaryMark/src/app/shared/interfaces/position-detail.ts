export interface PositionDetail {
  header: Header;
  parameters: Parameter[];
}

export interface PositionDetailRequest {
  positionId: number;
  position: string;
  smCode: string;
  levelId: number;
  groupId: number;
  moduleId: number;
  parameters: ParamRequest[];
}

export interface Header {
  positionId: number;
  cmCode: number;
  position: string;
  smCode: string;
  levelId: number;
  groupId: number;
  positionSalaryMarkLabel: string;
  moduleId?: number;
}

export interface ProjetParameter {
  id: number;
  title: string;
}

export interface Parameter {
  parameterId: number;
  title: string;
  projetParameters: ProjetParameter[];
}

export interface ParamRequest {
  parameterId: number;
  projectParameters: number[];
  newProjectParameters?: string[];
}
