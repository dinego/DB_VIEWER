import { OrderTypeENUM } from "./OrderTypeEnum";

export class Publications {
  title: string;
  date: Date;
  image: string;
  id: number;
  tags: string;
  article: string;
  studyType: number;
  html: string;
  script: string;
  hasAccess: boolean;
  fileName: string;
  message: string;
  iFrame: string;
}

export class StudyShared {
  userId: number;
  studyId: number;
  userName: string;
  create: Date;
  studies: Publications[];
}

export class Report {
  title: string;
  date: Date;
  image: string;
  id: number;
  studyType: number;
  hasAccess: boolean;
  fileName: string;
  html: string;
  script: string;
}

export class OrderPublication {
  title: string;
  type: OrderTypeENUM;
}
