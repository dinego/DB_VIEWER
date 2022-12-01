import { ReportTypeEnum } from '../enum/report-type-enum';

export interface EnumItem<E> {
  id: E;
  name: keyof E;
}

export interface IReports {
  id: string;
  title: string;
  date: string;
  image: any;
  reportType: ReportTypeEnum;
  html: string;
  script: string;
  fileName: string;
}

export interface IDownloadMyReport{
  id: string;
  fileName: string;
}
