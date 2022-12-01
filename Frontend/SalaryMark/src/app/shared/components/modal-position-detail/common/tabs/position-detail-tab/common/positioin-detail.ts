import { IParameters } from "@/shared/interfaces/parameters";

export interface IPositionDetail {
  position: string;
  positionId: number;
  positioSmCode: string;
  levelId: number;
  level: string;
  groupId: number;
  group: string;
  parameters: IParameterPositionDetail[];
}

export interface IParameterPositionDetail {
  parameterId: number;
  parameter: string;
}

export interface IListParametersDetail {
  titleParameter: string;
  parameters: IParameterPositionDetail[];
}

export interface IDescriptionPosition {
  descriptionArea: IDescriptionArea;
  responsabilityArea: IResponsabilityArea;
  hasAccess: boolean;
  trainingAndExperiences: ITrainingAndExperiences;
}

export interface IDescriptionArea {
  positionId: number;
  summary: string;
  full: string;
  position: string;
  area: string;
  setor: string;
  subject: string;
  level: string;
  smCode: string;
  positionCompanyNames: string;
  othersPositionCsNames: string;
}

export interface IResponsabilityArea {
  responsabilities: string;
}

export interface ITrainingAndExperiences {
  academicTraining: IAcademicTraining;
  behavioralSkills: IBehavioralSkills[];
  desiredExperience: IDesiredExperience;
  technicalSkills: [];
}

export interface IAcademicTraining {
  desirableFormation: string;
  id: number;
  studyArea: string;
  trainingRequired: string;
}

export interface IBehavioralSkills {
  id: number;
  intensity?: number;
  intensityId?: number;
  order: number;
  skillDescription: string;
  skillId: number;
  skillName: string;
  typeEnum: number;
}

export interface IDesiredExperience {
  especification: string;
  experienceTime: string;
  id: number;
  leadership?: number;
  minimumExperience: string;
}

export interface IHeader {
  positionId: number;
  cmCode: number;
  position: string;
  smCode: string;
  levelId: number;
  groupId: number;
  positionSalaryMarkLabel: string;
  moduleId?: number;
}

export interface IProjetParameter {
  id: number;
  parentParameterId?: number;
  title: string;
}

export interface IParameter {
  parameterId: number;
  title: string;
  projetParameters: IProjetParameter[];
  newProjectParameters?: string[];
}

export interface IModalPositionDetail {
  header: IHeader;
  parameters: IParameter[];
}

export interface IParameterGroupItems {
  parameter: IParameter;
  list: IParameters[];
}

export enum ParametersEnum {
  Area = 1,
  ParameterOne = 2,
  ParameterTwo = 3,
  ParameterThree = 4,
  CareerAxis = 5,
}
