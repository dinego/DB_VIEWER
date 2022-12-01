using System.ComponentModel;

namespace SM.Domain.Enum
{
    public enum LogActionsEnum
    {

        //dashboard
        [Description("Acessou a página de DashBoard")]
        AccessDashBoard = 1,
        [Description("Acessou o grafico de Cargos em DashBoard")]
        AccessDashBoardPosition = 2,
        [Description("Acessou os cards de Impacto Financeiro em DashBoard")]
        AccessDashBoardFinancial = 3,
        [Description("Acessou o gráfico movimento propostos em DashBoard")]
        AccessDashBoardProposedMovements = 4,
        [Description("Acessou o gráfico análise de distribuição em DashBoard")]
        AccessDashBoardDistributionAnalysis = 5,
        [Description("Acessou o gráfico análise de comparativa em DashBoard")]
        AccessDashBoardComparativeAnalysis = 6,
        //parameters
        [Description("Acessou níveis em Parâmetros")]
        AccessParametersLevels = 7,
        [Description("Salvou níveis em Parâmetros")]
        AccessParametersSaveLevels = 8,
        [Description("Acessou base horária em Parâmetros")]
        AccessParametersHoursBase = 9,
        [Description("Salvou base horária em Parametros")]
        AccessParametersSaveHoursBase = 10,
        [Description("Acessou estratégia salarial em Parâmetros")]
        AccessParametersSalaryStrategy = 11,
        [Description("Acessou configurações PJ em Parâmetros")]
        AccessParametersPJSettings = 12,
        [Description("Salvou configurações PJ em Parametros")]
        AccessParametersSavePJSettings = 13,
        //position
        [Description("Salvou a exibição do mapa (Posição)")]
        SavePositionUpdateMap = 14,
        [Description("Salvou a exibição do lista (Posição)")]
        SavePositionUpdateList = 15,
        [Description("Acessou o mapa (Posição)")]
        AccessPositionMap = 16,
        [Description("Download o excel do mapa (Posição)")]
        DownloadPositionMap = 17,
        [Description("Acessou a lista (Posição)")]
        AccessPositionList = 18,
        [Description("Download do excel da lista (Posição)")]
        DownloadPositionList = 19,
        [Description("Acessou o dropdown exibir por (Posição)")]
        AccessPositionAllDisplay = 20,
        [Description("Acessou o dialog (Posição)")]
        AccessDialogPosition = 21,
        //posicionamento
        [Description("Salvou a exibição do enquadramento (Posicionamento)")]
        SavePositioningUpdateFramework = 14,
        [Description("Acessou o enquadramento (Posicionamento)")]
        AccessPositioningFramework = 15,
        [Description("Download do excel do enquadramento (Posicionamento)")]
        DownloadPositioningFramework = 16,
        [Description("Acessou o dialog do enquadramento (Posicionamento)")]
        DialogPositioningFramework = 17,
        [Description("Acessou o impacto financeiro (Posicionamento)")]
        AccessPositioningImpactFinancial = 18,
        [Description("Acessou o dialog do impacto financeiro (Posicionamento)")]
        DialogPositioningImpactFinancial = 19,
        [Description("Acessou movimentos propostos (Posicionamento)")]
        AccessPositioningProposedMovements = 20,
        [Description("Acessou o dialog do movimentos propostos (Posicionamento)")]
        DialogPositioningProposedMovements = 21,
        [Description("Acessou análise de distribuição (Posicionamento)")]
        AccessPositioningDistributionAnalysis = 22,
        [Description("Acessou o gráfico análise comparativa (Posicionamento)")]
        AccessPositioningComparativeAnalysisChart = 23,
        [Description("Acessou o dialog análise comparativa (Posicionamento)")]
        DialogPositioningComparativeAnalysis = 24,
        [Description("Acessou a tabela análise comparativa (Posicionamento)")]
        AccessPositioningComparativeAnalysisTable = 25,
        [Description("Download do excel do analise comparativa (Posicionamento)")]
        DownloadPositioningComparativeAnalysis = 26,
        [Description("Acessou o dropdown exibir por (Posição)")]
        AccessPositioningAllDisplay = 27,
        [Description("Salvou os Rótulos Globais (Parâmetros)")]
        SaveParameterGlobalLabels = 38,
        [Description("Acessou Configurar Exibição (Parâmetros)")]
        AccessDisplayConfiguration = 39,
        //reports
        [Description("Acessou os relatórios")]
        AccessReports = 28,
        [Description("Download do excel dos relatórios")]
        DownloadReports = 29,
        //salary Table
        [Description("Acessou a tabela salarial")]
        AccessSalaryTable = 30,
        [Description("Download do excel da tabela salarial")]
        DownloadSalaryTable = 31,
        [Description("Salvou a exibição da tabela salarial")]
        SaveSalaryTableUpdate = 32,
        //users parameters 
        [Description("Mudar status do usuário (Parâmetros)")]
        ChangeStatusUserParameters = 33,
        [Description("Mostrar os usuários (Parâmetros)")]
        GetAllUserParameters = 34,
        [Description("Mostrar o usuário (Parâmetros)")]
        GetUserParameters = 35,
        [Description("Salvar o usuário (Parâmetros)")]
        SaveUserParameters = 36,
        //home
        [Description("Acessou as notificações")]
        AccessNotification = 37,
        [Description("Acessou o filtro de cargos (Dashboard)")]
        AccessPositioningFilterByDashboard = 38,
        [Description("Acessou atualizações de valores de tabelas salariais")]
        AccessGetSalarialTableValues = 39,

        [Description("Salvou dados de Exibição (Parâmetros)")]
        SaveDisplayConfiguration,
        [Description("Salvou dados de estratégia salarial em Parâmetros")]
        UpdateParametersSalaryStrategy,
        //salary Table
        [Description("Acessou a tabela de cargos em tabela salarial")]
        AccessPostionSalaryTable,
        //salary Table
        [Description("Acessou o gráfico em tabela salarial")]
        AccessGraphSalaryTable,
        [Description("Acessou as características do cargo.")]
        AccessPositionDetails,
        [Description("Editou as características do cargo.")]
        SavePositionDetails,
        [Description("Editou a trilha de carreira do cargo.")]
        SaveCareerPlanPosition,
        [Description("Acessou a trilha de carreira do cargo.")]
        AccessCareerPlanPosition,
        [Description("Acessou tabela salarial em detalhes do cargo.")]
        AccessSalaryTablePositionDetails,
        [Description("Editou os mapeamentos da tabela salarial em detalhes do cargo..")]
        SaveSalaryTablePositionDetails,
    }

}
