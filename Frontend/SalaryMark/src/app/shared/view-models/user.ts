import { AccessTypeCompany } from "../enum/access-type-company";

export enum ProductTypeEnum {
  CS = 1,
  SM = 2,
}

export interface IProduct {
  projectId: number;
  productType: ProductTypeEnum;
  productName: string;
  userCompanies: number[];
  accessType: AccessTypeCompany;
  modules: IModules[];
}

export interface IUser {
  id: number;
  name: string;
  email: string;
  companyId: number;
  company: string;
  key: number;
  simulated: boolean;
  isFirstAccess: boolean;
  photo: string;
  isAdmin: boolean;
}

export interface IModules {
  id: number;
  subItems: number[];
}

export interface IToken {
  products: IProduct;
  user: IUser;
}
