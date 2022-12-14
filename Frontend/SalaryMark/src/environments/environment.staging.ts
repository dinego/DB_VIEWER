// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

import { env } from "./.env";

const server = "https://salarymark-api-homolog.carreira.com.br/api/";
const serverCS = "https://csnew-api.carreira.com.br/api/";
const account = "https://autenticador-api-homolog.carreira.com.br/api/";

export const environment = {
  fakeToken: "",
  production: true,
  version: env.npm_package_version + "-staging",
  autenticatorUrl: "https://autenticador-homolog.carreira.com.br/login/logout",
  baseUrl: "https://salarymark-homolog.carreira.com.br/",
  siteCsUrl: "https://csnew-homolog.carreira.com.br",
  zendeskUrl: "https://carreiramuller.zendesk.com/hc/pt-br",
  SetNotFirstAccess: `${server}/api/Notification/SetNotFirstAccess`,
  showVideoFirstAccess: false,
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
