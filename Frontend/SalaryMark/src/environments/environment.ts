// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

import { env } from "./.env";

const enviro = "dev";

const server = `https://salarymark-api-${enviro}.carreira.com.br/api/`;
//const server = "http://localhost:5000/api/";
const serverCS = `https://csnew-api-${enviro}.carreira.com.br/api/`;
const account = `https://autenticador-api-${enviro}.carreira.com.br/api/`;

export const environment = {
  fakeToken:
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyIjoie1xyXG4gIFwiaWRcIjogMzY3LFxyXG4gIFwibmFtZVwiOiBcIlRoaWFnbyBTaWx2ZXJpb1wiLFxyXG4gIFwiY29tcGFueUlkXCI6IDc4OTgsXHJcbiAgXCJrZXlcIjogXCJcIixcclxuICBcImVtYWlsXCI6IFwidGhpYWdvc2lsdmVyaW9AY2FycmVpcmEubGFuXCIsXHJcbiAgXCJzaW11bGF0ZWRcIjogZmFsc2UsXHJcbiAgXCJjb21wYW55XCI6IFwiQ2FycmVpcmEgTXVsbGVyXCIsXHJcbiAgXCJpc0ZpcnN0QWNjZXNzXCI6IGZhbHNlLFxyXG4gIFwiaXNBZG1pblwiOiB0cnVlLFxyXG4gIFwiY29tcGFueVN1c3BlbmRlZFwiOiBmYWxzZSxcclxuICBcInByb2ZpbGVcIjogMFxyXG59IiwicHJvZHVjdHMiOiJ7XHJcbiAgXCJwcm9kdWN0VHlwZVwiOiAyLFxyXG4gIFwicHJvZHVjdE5hbWVcIjogXCJTYWxhcnlNYXJrXCIsXHJcbiAgXCJ1c2VyQ29tcGFuaWVzXCI6IFtcclxuICAgIDY0NDcsXHJcbiAgICA3ODk4XHJcbiAgXSxcclxuICBcInByb2plY3RJZFwiOiA1LFxyXG4gIFwiYWNjZXNzVHlwZVwiOiBudWxsLFxyXG4gIFwibW9kdWxlc1wiOiBbXHJcbiAgICB7XHJcbiAgICAgIFwiaWRcIjogMSxcclxuICAgICAgXCJzdWJJdGVtc1wiOiBbXHJcbiAgICAgICAgMSxcclxuICAgICAgICAyXHJcbiAgICAgIF1cclxuICAgIH0sXHJcbiAgICB7XHJcbiAgICAgIFwiaWRcIjogMixcclxuICAgICAgXCJzdWJJdGVtc1wiOiBbXVxyXG4gICAgfSxcclxuICAgIHtcclxuICAgICAgXCJpZFwiOiAzLFxyXG4gICAgICBcInN1Ykl0ZW1zXCI6IFtdXHJcbiAgICB9LFxyXG4gICAge1xyXG4gICAgICBcImlkXCI6IDQsXHJcbiAgICAgIFwic3ViSXRlbXNcIjogW1xyXG4gICAgICAgIDMsXHJcbiAgICAgICAgNFxyXG4gICAgICBdXHJcbiAgICB9LFxyXG4gICAge1xyXG4gICAgICBcImlkXCI6IDUsXHJcbiAgICAgIFwic3ViSXRlbXNcIjogW1xyXG4gICAgICAgIDUsXHJcbiAgICAgICAgNixcclxuICAgICAgICA3LFxyXG4gICAgICAgIDgsXHJcbiAgICAgICAgOVxyXG4gICAgICBdXHJcbiAgICB9LFxyXG4gICAge1xyXG4gICAgICBcImlkXCI6IDYsXHJcbiAgICAgIFwic3ViSXRlbXNcIjogW11cclxuICAgIH0sXHJcbiAgICB7XHJcbiAgICAgIFwiaWRcIjogNyxcclxuICAgICAgXCJzdWJJdGVtc1wiOiBbXHJcbiAgICAgICAgMTAsXHJcbiAgICAgICAgMTEsXHJcbiAgICAgICAgMTIsXHJcbiAgICAgICAgMTMsXHJcbiAgICAgICAgMTQsXHJcbiAgICAgICAgMTUsXHJcbiAgICAgICAgMTZcclxuICAgICAgXVxyXG4gICAgfSxcclxuICAgIHtcclxuICAgICAgXCJpZFwiOiA4LFxyXG4gICAgICBcInN1Ykl0ZW1zXCI6IFtdXHJcbiAgICB9LFxyXG4gICAge1xyXG4gICAgICBcImlkXCI6IDksXHJcbiAgICAgIFwic3ViSXRlbXNcIjogW1xyXG4gICAgICAgIDE3LFxyXG4gICAgICAgIDE4XHJcbiAgICAgIF1cclxuICAgIH1cclxuICBdLFxyXG4gIFwicGVybWlzc2lvbnNcIjoge1xyXG4gICAgXCJjYW5GaWx0ZXJUeXBlb2ZDb250cmFjdFwiOiB0cnVlLFxyXG4gICAgXCJjYW5GaWx0ZXJNTVwiOiB0cnVlLFxyXG4gICAgXCJjYW5GaWx0ZXJNSVwiOiB0cnVlLFxyXG4gICAgXCJjYW5GaWx0ZXJPY2N1cGFudHNcIjogZmFsc2UsXHJcbiAgICBcImNhbkRvd25sb2FkRXhjZWxcIjogdHJ1ZSxcclxuICAgIFwiY2FuUmVuYW1lQ29sdW1uc1wiOiBmYWxzZSxcclxuICAgIFwiY2FuU2hhcmVcIjogdHJ1ZSxcclxuICAgIFwiY2FuRWRpdExldmVsc1wiOiB0cnVlLFxyXG4gICAgXCJjYW5FZGl0U2FsYXJ5U3RyYXRlZ3lcIjogdHJ1ZSxcclxuICAgIFwiY2FuRWRpdEhvdXJseUJhc2lzXCI6IHRydWUsXHJcbiAgICBcImNhbkVkaXRDb25maWdQSlwiOiB0cnVlLFxyXG4gICAgXCJjYW5FZGl0VXNlclwiOiB0cnVlLFxyXG4gICAgXCJjYW5FZGl0R2xvYmFsTGFiZWxzXCI6IHRydWUsXHJcbiAgICBcImNhbkVkaXRMb2NhbExhYmVsc1wiOiB0cnVlLFxyXG4gICAgXCJpbmFjdGl2ZVBlcnNvblwiOiBmYWxzZSxcclxuICAgIFwiY2FuRGlzcGxheUVtcGxveWVlTmFtZVwiOiB0cnVlLFxyXG4gICAgXCJjYW5EaXNwbGF5Qm9zc05hbWVcIjogdHJ1ZSxcclxuICAgIFwiY2FuRWRpdFByb2plY3RTYWxhcnlUYWJsZXNWYWx1ZXNcIjogdHJ1ZSxcclxuICAgIFwiY2FuQ2hvb3NlRGVmYXVsdFBhcmFtZXRlclwiOiB0cnVlLFxyXG4gICAgXCJjYW5Nb3ZlTmV4dFN0ZXBcIjogdHJ1ZSxcclxuICAgIFwiY2FuQWRkUG9zaXRpb25cIjogdHJ1ZSxcclxuICAgIFwiY2FuRWRpdFBvc2l0aW9uXCI6IHRydWUsXHJcbiAgICBcImNhbkRlbGV0ZVBvc2l0aW9uXCI6IHRydWUsXHJcbiAgICBcImNhbkVkaXRMaXN0UG9zaXRpb25cIjogdHJ1ZSxcclxuICAgIFwiY2FuRWRpdEdTTU1hcHBpbmdUYWJsZVwiOiB0cnVlLFxyXG4gICAgXCJjYW5FZGl0U2FsYXJ5VGFibGVWYWx1ZXNcIjogdHJ1ZSxcclxuICAgIFwiY2FuQWRkUGVvcGxlXCI6IHRydWUsXHJcbiAgICBcImNhbkRlbGV0ZVBlb3BsZVwiOiB0cnVlLFxyXG4gICAgXCJjYW5FZGl0UGVvcGxlXCI6IHRydWUsXHJcbiAgICBcImNhbkVkaXRNYXBwaW5nUG9zaXRpb25TTVwiOiB0cnVlXHJcbiAgfVxyXG59IiwibmJmIjoxNjUwNDAzOTE4LCJleHAiOjE2NTA0OTAzMTgsImlhdCI6MTY1MDQwMzkxOCwiaXNzIjoiaHR0cHM6Ly9hdXRob3JpemF0b3IuY2FycmVpcmEuY29tLmJyL0NNQy1BVVRILUlTU1VFUiIsImF1ZCI6Imh0dHBzOi8vYXV0aG9yaXphdG9yLmNhcnJlaXJhLmNvbS5ici9DTUMtQVVUSC1BVURJRU5DRSJ9.Ifd872aSRRSgjOBdYr7Tx6xB7RWuti1juC6wtcFwuVQ",
  production: false,
  version: env.npm_package_version + "-dev",
  autenticatorUrl: "https://autenticador-dev.carreira.com.br/login/logout",
  baseUrl: "localhost:4200/",
  siteCsUrl: "https://csnew-dev.carreira.com.br",
  zendeskUrl: "https://carreiramuller.zendesk.com/hc/pt-br",
  showVideoFirstAccess: false,
  SetNotFirstAccess: `${server}/api/Notification/SetNotFirstAccess`,
  youtube: {
    playlistIdWebinar: "PLiXaaP4U-6F2UY4SDyyDOU2qpkujki877",
    playlistIdPodcasts: "PLiXaaP4U-6F1g5hHPddh8u3k71_tnK6_H",
    playlists: "playlistItems/",
    videos: "videos/",
    api: "https://www.googleapis.com/youtube/v3/",
    key: "AIzaSyDXbiLq27dGAd2jMBh5_EmAN0rxDpnB2xA",
  },
  api: {
    account: {
      login: `${account}Account/Login`,
      resetPassword: `${account}Account/ResetPassword`,
      recoveryPassword: `${account}Account/RecoverPassword`,
      generateLinkAccess: `${account}Account/GenerateLinkAccess`,
      getUserPhoto: `${account}Account/GetUserPhoto`,
    },
    dashboard: {
      getFinancialImpactCards: `${server}DashBoard/GetFinancialImpactCards`,
      getPositionsChart: `${server}DashBoard/GetPositionsChart`,
      getProposedMovementsChart: `${server}DashBoard/GetProposedMovementsChart`,
      getDistributionAnalysisChart: `${server}DashBoard/GetDistributionAnalysisChart`,
      getComparativeAnalysisDash: `${server}DashBoard/GetComparativeAnalysisChart`,
    },
    home: {
      getNotifications: `${server}UserNotification/GetAllNotification`,
      getNotificationsNotReaded: `${server}UserNotification/GetAllNotificationNotRead`,
      removeNotificationById: `${server}UserNotification/RemoveNotificationById`,
      setReadNotification: `${server}UserNotification/SetReadNotification`,
      contactUs: `${server}UserNotification/ContactUs`,
      uploadPhoto: `${server}UserNotification/UploadPhoto`,
      removePhoto: `${server}UserNotification/RemovePhoto`,
      sendLinkAccess: `${server}UserNotification/SendLinkAccess`,
    },
    salaryTable: {
      getSalaryTable: `${server}TableSalary/GetSalaryTable`,
      getSalaryPositionTable: `${server}TableSalary/GetSalaryTablePosition`,
      getSalaryGraph: `${server}TableSalary/GetSalaryGraph`,
      getSalaryTableExcel: `${server}TableSalary/getSalaryTableExcel`,
      getEditTableValues: `${server}TableSalary/GetEditTableValues`,
      updateDisplayColumns: `${server}TableSalary/UpdateDisplayColumns`,
      updateSalaryTable: `${server}TableSalary/UpdateSalaryTable`,
      updateSalaryTableInfo: `${server}TableSalary/UpdateSalaryTableInfo`,
      importExcel: `${server}TableSalary/ImportExcel`,
      getShareData: `${server}ShareTableSalary/GetShareData`,
      getShareSalaryTable: `${server}ShareTableSalary/GetSalaryTable`,
      getShareSalaryPositionTable: `${server}ShareTableSalary/GetSalaryTablePosition`,
      getShareSalaryGraph: `${server}ShareTableSalary/GetSalaryGraph`,
      getRangeSalaryGraph: `${server}TableSalary/GetRangeSalaryGraph`,
    },
    positionDetails: {
      getDetails: `${server}PositionDetails/GetDetails`,
      updatePositionDetail: `${server}PositionDetails/UpdatePositionDetails`,
      getSalaryTableMapping: `${server}PositionDetails/GetSalaryTableMapping`,
      updateSalaryTableMapping: `${server}PositionDetails/UpdateSalaryTableMapping`,
      getSalaryTableValuesByGSM: `${server}PositionDetails/GetSalaryTableValuesByGSM`,
      addNewParameter: `${server}PositionDetails/AddNewParameter`,
    },
    salaryStrategy: {
      getAllSalaryStrategy: `${server}Parameter/GetSalaryStrategy`,
      updateSalaryStrategy: `${server}Parameter/UpdateSalaryStrategy`,
    },
    positioning: {
      updateDisplayColumnsFramework: `${server}Positioning/UpdateDisplayColumnsFramework`,
      getFramework: `${server}Positioning/GetFramework`,
      getFrameworkExcel: `${server}Positioning/getFrameworkExcel`,
      getFinancialImpact: `${server}Positioning/GetFinancialImpact`,
      getFinancialImpactExcel: `${server}Positioning/getFinancialImpactExcel`,
      getShareFinancialImpact: `${server}Share/GetFinancialImpact`,
      getFullInfoPositioningFinancialImpact: `${server}Positioning/GetFullInfoFinancialImpact`,
      getFullInfoPositioningFinancialImpactShare: `${server}Share/GetFullInfoFinancialImpact`,
      getProposedMovements: `${server}Positioning/GetProposedMovements`,
      getShareProposedMovements: `${server}Share/GetProposedMovements`,
      getFullInfoProposedMovements: `${server}Positioning/GetFullInfoProposedMovements`,
      getFullInfoProposedMovementsShare: `${server}Share/GetFullInfoProposedMovements`,
      getDistributionAnalysis: `${server}Positioning/GetDistributionAnalysis`,
      getShareDistributionAnalysis: `${server}Share/GetDistributionAnalysis`,
      getComparativeAnalysisChart: `${server}Positioning/GetComparativeAnalysisChart`,
      getComparativeAnalysisTable: `${server}Positioning/GetComparativeAnalysisTable`,
      getFullInfoFramework: `${server}Positioning/GetFullInfoFramework`,
      getFullInfoFrameworkShare: `${server}share/GetFullInfoFramework`,
      getDisplayBy: `${server}Positioning/GetDisplayBy`,
      getFullInfoComparativeAnalysis: `${server}Positioning/GetFullInfoComparativeAnalysis`,
      getFullInfoComparativeAnalysisShare: `${server}Share/GetFullInfoComparativeAnalysis`,
      shareFramework: `${server}Share/GetFramework`,
      getSharedComparativeAnalysisChart: `${server}Share/GetComparativeAnalysisChart`,
      getSharedComparativeAnalysisTable: `${server}Share/GetComparativeAnalysisTable`,
      getComparativeAnalysisTableExcel: `${server}Positioning/GetComparativeAnalysisTableExcel`,
      getDisplayFilter: `${server}Positioning/GetDisplayFilter`,
    },
    position: {
      savePosition: `${server}Position/UpdateDisplayColumnsMap`,
      updateDisplayColumnsMap: `${server}Position/UpdateDisplayColumnsMap`,
      updateDisplayColumnsList: `${server}Position/UpdateDisplayColumnsList`,
      getMapPosition: `${server}Position/GetMapPosition`,
      getMapPositionExcel: `${server}Position/GetMapPositionExcel`,
      getAllPositions: `${server}Position/GetAllPositions`,
      getAllPositionsExcel: `${server}Position/getAllPositionsExcel`,
      getAllDisplayBy: `${server}Position/GetAllDisplayBy`,
      getFullInfoPosition: `${server}Position/GetFullInfoPosition`,
      sharePositions: `${server}Share/GetAllPositions`,
      shareMap: `${server}Share/GetMapPosition`,
      getDescription: `${serverCS}Position/GetDescription`,
    },
    profile: {
      getAllProfiles: `${server}Profile/GetAllProfiles`,
    },
    share: {
      generateKeySave: `${server}Share/GenerateKeySave`,
      shareLink: `${server}Share/ShareLink`,
    },
    userParameter: {
      getAll: `${server}UserParameter/GetAll`,
      changeStatusUser: `${server}UserParameter/ChangeStatusUser`,
      getUserInformation: `${server}UserParameter/GetUserInformation`,
      saveUserInformation: `${server}UserParameter/SaveUserInformation`,
      canAccessUsers: `${server}UserParameter/CanAccessUsers`,
    },
    report: {
      downloadFile: `${server}Reports/DownloadFile`,
      getReports: `${server}Reports/GetReports`,
      registerLog: `${server}Reports/RegisterLog`,
    },
    studies: {
      getPublications: `${serverCS}StudiesPublications/GetStudies`,
      getStudies: `${serverCS}StudiesPublications/GetStudies`,
      getPublicationsFile: `${serverCS}StudiesPublications/ExportFile`,
      getUrlFile: `${serverCS}Position/ExportPdf`,
      getMyReportsFile: `${serverCS}MyReports/ExportFile`,
      getStudyShared: `${serverCS}Share/GetStudyShared`,
    },
    getFullInfoPosition: `${server}Position/GetFullInfoPosition`,
    level: {
      getLevels: `${server}Parameter/GetLevels`,
      saveLevels: `${server}Parameter/SaveLevels`,
    },
    hourlyBasis: {
      getHourlyBasis: `${server}Parameter/GetHourlyBasis`,
      saveHourlyBasis: `${server}Parameter/SaveHourlyBasis`,
    },
    contractTypes: {
      getAllContractTypes: `${server}ContractTypes/GetAllContractTypes`,
      getContractTypesPjSettings: `${server}ContractTypes/GetContractTypesPjSettings`,
    },
    pjSettings: {
      getPJSettings: `${server}Parameter/GetPJSettings`,
      updateSettingsPj: `${server}Parameter/UpdateSettingsPj`,
    },
    displaySettins: {
      getDisplayConfiguration: `${server}Parameter/GetDisplayConfiguration`,
      updateDisplayConfiguration: `${server}Parameter/UpdateDisplayConfiguration`,
      updateGlobalLabels: `${server}Parameter/UpdateGlobalLabels`,
    },
    globalLabels: {
      getGlobalLabels: `${server}Parameter/getGlobalLabels`,
      updateGlobalLabels: `${server}Parameter/updateGlobalLabels`,
    },
    common: {
      contractTypes: `${server}ContractTypes/GetAllContractTypes`,
      hourlyBase: `${server}HourBase/GetHoursBase`,
      profile: `${server}Profile/GetAllProfiles`,
      units: `${server}Units/GetUnitsByUser`,
      movements: `${server}Movements/GetMovements`,
      unitsByFilter: `${server}Units/GetUnitsByFilter`,
      canAccessHoursBase: `${server}HourBase/CanAccessHoursBase`,
      getAllParameters: `${server}Common/GetAllParameters`,
      getAllCareerAxis: `${server}Common/GetAllCareerAxis`,
      getAllLevels: `${server}Common/GetAllLevels`,
      getAllSalaryTables: `${server}Common/GetAllSalaryTables`,
      getGsmBySalaryTable: `${server}Common/GetGsmBySalaryTable`,
    },
    user: {
      simulate: `${account}Account/SimulateUser`,
    },
  },
};
