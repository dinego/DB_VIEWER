using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Api.ViewModel;
using SM.Application.Helpers;
using SM.Application.Positioning.Command;
using SM.Application.Positioning.Queries;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class PositioningController : BaseController
    {
        /// <summary>
        ///  Update/Add display columns SM (Framework Positioning)
        /// </summary>
        /// <param name="updateDisplayColumnsFrameworkRequest">updateDisplayColumnsFrameworkRequest</param>
        /// <returns>status</returns>
        /// <response code="200">Columns save with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpPost("UpdateDisplayColumnsFramework")]
        [ModuleAuthorize(ModulesEnum.Positioning, ModulesSuItemsEnum.Framework)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UpdateDisplayColumnsFramework([FromBody] UpdateDisplayColumnsFrameworkViewModel updateDisplayColumnsFrameworkRequest)
        {
            try
            {
                var request = updateDisplayColumnsFrameworkRequest.Map().ToANew<UpdateDisplayColumnsFrameworkRequest>();
                if (request == null)
                    return BadRequest(new { message = "Não encontramos os parâmetros de entrada para processar sua requisição." });
                request.UserId = User.GetUserId();
                var response = await Mediator.Send(request);
                await LogUserAccess(LogActionsEnum.SavePositioningUpdateFramework.GetDescription(),
                    ModulesEnum.Positioning, ModulesSuItemsEnum.Framework);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Framework
        /// </summary>
        ///<param name="isMM">default is false</param>
        ///<param name="isMI">default is false</param>
        ///<param name="page">default is 1</param>
        ///<param name="pageSize">default is 10</param>
        ///<param name="contractType">default is ContractTypeEnum.CLT (0)</param>
        ///<param name="hoursType">default is DataBaseSalaryEnum.MonthSalary (0)</param>
        ///<param name="term">term</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        /// <param name="isAsc">isAsc</param>
        /// <param name="sortColumnId">sortColumnId</param>
        /// <param name="columns"></param>
        /// <returns>GetFrameworkResponse list of object response (map position)</returns>
        /// <response code="200">GetFrameworkResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFramework")]
        [ModuleAuthorize(ModulesEnum.Positioning, ModulesSuItemsEnum.Framework)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFrameworkResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFramework(
            bool isMM = false,
            bool isMI = false,
            int page = 1,
            int pageSize = 20,
            ContractTypeEnum contractType = ContractTypeEnum.CLT,
            DataBaseSalaryEnum hoursType = DataBaseSalaryEnum.MonthSalary,
            string term = null,
            long? unitId = null,
            bool? isAsc = null,
            int? sortColumnId = null,
            string columns = null)
        {
            try
            {
                var response = await Mediator.Send(new GetFrameworkRequest
                {
                    ContractType = contractType,
                    HoursType = hoursType,
                    IsMI = isMI,
                    IsMM = isMM,
                    Page = page,
                    PageSize = pageSize,
                    ProjectId = User.GetProjectId(),
                    Term = term,
                    UnitId = unitId,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies(),
                    IsAsc = isAsc,
                    SortColumnId = sortColumnId,
                    Columns = !string.IsNullOrEmpty(columns) ? JsonConvert.DeserializeObject<List<int>>(columns) : new List<int>()
                });
                await LogUserAccess(LogActionsEnum.AccessPositioningFramework.GetDescription(),
                    ModulesEnum.Positioning, ModulesSuItemsEnum.Framework);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Framework EXcel
        /// </summary>
        ///<param name="isMM">default is false</param>
        ///<param name="isMI">default is false</param>
        ///<param name="contractType">default is ContractTypeEnum.CLT (0)</param>
        ///<param name="hoursType">default is DataBaseSalaryEnum.MonthSalary (0)</param>
        ///<param name="term">term</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        /// <param name="isAsc">isAsc</param>
        /// <param name="sortColumnId">sortColumnId</param>
        /// <param name="columns"></param>
        /// <returns>GetFrameworkExcelResponse list of object response (map position)</returns>
        /// <response code="200">GetFrameworkExcelResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFrameworkExcel")]
        [ModuleAuthorize(ModulesEnum.Positioning, ModulesSuItemsEnum.Framework)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFrameworkExcelResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFrameworkExcel(
            bool isMM = false,
            bool isMI = false,
            ContractTypeEnum contractType = ContractTypeEnum.CLT,
            DataBaseSalaryEnum hoursType = DataBaseSalaryEnum.MonthSalary,
            string term = null,
            long? unitId = null,
            bool? isAsc = null,
            int? sortColumnId = null,
            string columns = null)
        {
            try
            {
                var response = await Mediator.Send(new GetFrameworkExcelRequest
                {
                    ContractType = contractType,
                    HoursType = hoursType,
                    IsMI = isMI,
                    IsMM = isMM,
                    ProjectId = User.GetProjectId(),
                    Term = term,
                    UnitId = unitId,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies(),
                    IsAsc = isAsc,
                    SortColumnId = sortColumnId,
                    Columns = !string.IsNullOrEmpty(columns) ? JsonConvert.DeserializeObject<List<int>>(columns) : new List<int>()
                });
                await LogExcelAccess(LogActionsEnum.DownloadPositioningFramework.GetDescription(),
                                ModulesEnum.Positioning, ModulesSuItemsEnum.Framework);

                return File(
                 fileContents: response.File,
                 contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                 fileDownloadName: $"{response.FileName}.xlsx"
            );
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// FullInfo from Framework (dialog)
        /// </summary>
        /// <param name="salaryBaseId">salaryBaseId is required</param>
        ///<param name="isMM">default is false</param>
        ///<param name="isMI">default is false</param>
        ///<param name="contractType">default is ContractTypeEnum.CLT (0)</param>
        ///<param name="hoursType">default is DataBaseSalaryEnum.MonthSalary (0)</param>
        /// <param name="unitId">unitId is optional</param>
        /// <returns>GetFullInfoFramework object response </returns>
        /// <response code="200">GetFullInfoFrameworkResponse retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFullInfoFramework")]
        [ModuleAuthorize(ModulesEnum.Positioning, ModulesSuItemsEnum.Framework)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFullInfoFrameworkResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFullInfoFramework(
            [Required] long salaryBaseId,
            bool isMM = false,
            bool isMI = false,
            ContractTypeEnum contractType = ContractTypeEnum.CLT,
            DataBaseSalaryEnum hoursType = DataBaseSalaryEnum.MonthSalary,
            long? unitId = null)
        {
            try
            {
                var response = await Mediator.Send(new GetFullInfoFrameworkRequest
                {
                    SalaryBaseId = salaryBaseId,
                    ContractType = contractType,
                    HoursType = hoursType,
                    IsMI = isMI,
                    IsMM = isMM,
                    ProjectId = User.GetProjectId(),
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies(),
                    UnitId = unitId
                });
                await LogUserAccess(LogActionsEnum.DialogPositioningFramework.GetDescription(),
                    ModulesEnum.Positioning, ModulesSuItemsEnum.Framework);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Get Financial Impact chart
        /// </summary>
        /// <param name="displayBy">default is DisplayByFinancialImpactEnum.Profile (1)</param>
        /// <param name="scenario">default is DisplayMMMIEnum.MM (1)</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        /// <returns>GetFinancialImpactResponse list of object response (map position)</returns>
        /// <response code="200">GetFinancialImpactResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFinancialImpact")]
        [ModuleAuthorize(ModulesEnum.Positioning, ModulesSuItemsEnum.FinancialImpact)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetFinancialImpactResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFinancialImpact(
            DisplayByPositioningEnum displayBy = DisplayByPositioningEnum.ProfileId,
            DisplayMMMIEnum scenario = DisplayMMMIEnum.MM,
            long? unitId = null)
        {
            try
            {
                var response = await Mediator.Send(new GetFinancialImpactRequest
                {
                    DisplayBy = displayBy,
                    ProjectId = User.GetProjectId(),
                    Scenario = scenario,
                    UnitId = unitId,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies()
                });
                await LogUserAccess(LogActionsEnum.AccessPositioningImpactFinancial.GetDescription(),
                    ModulesEnum.Positioning, ModulesSuItemsEnum.FinancialImpact);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Get FullInfo FinancialImpact
        /// </summary>
        /// <param name="categoryId">categoryId</param>
        /// <param name="serieId">serieId</param>
        /// <param name="displayBy">default is DisplayByFinancialImpactEnum.Profile (1)</param>
        /// <param name="scenario">default is DisplayMMMIEnum.MM (1)</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        /// <param name="isAsc">isAsc</param>
        /// <param name="sortColumnId">sortColumnId</param>
        /// <returns>GetFullInfoPositioningFinancialImpactResponse is the object response</returns>
        /// <response code="200">GetFullInfoPositioningFinancialImpactResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFullInfoFinancialImpact")]
        [ModuleAuthorize(ModulesEnum.Positioning, ModulesSuItemsEnum.FinancialImpact)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFullInfoFinancialImpactResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFullInfoFinancialImpact(
            [Required] string categoryId,
            AnalyseFinancialImpactEnum serieId = AnalyseFinancialImpactEnum.IFAMax,
            DisplayByPositioningEnum displayBy = DisplayByPositioningEnum.ProfileId,
            DisplayMMMIEnum scenario = DisplayMMMIEnum.MM,
            long? unitId = null,
            bool? isAsc = null,
            int? sortColumnId = null)
        {
            try
            {
                var response = await Mediator.Send(new GetFullInfoFinancialImpactRequest
                {
                    DisplayBy = displayBy,
                    ProjectId = User.GetProjectId(),
                    Scenario = scenario,
                    UnitId = unitId,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies(),
                    CategoryId = ConverterObject.TryConvertToInt(categoryId),
                    SerieId = serieId,
                    IsAsc = isAsc,
                    SortColumnId = sortColumnId
                });
                await LogUserAccess(LogActionsEnum.DialogPositioningImpactFinancial.GetDescription(),
                    ModulesEnum.Positioning, ModulesSuItemsEnum.FinancialImpact);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Get ProposedMovements chart
        /// </summary>
        /// <param name="displayBy">default is DisplayByFinancialImpactEnum.Profile (1)</param>
        /// <param name="scenario">default is DisplayMMMIEnum.MM (1)</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        /// <returns>GetProposedMovementsResponse list of object response (map position)</returns>
        /// <response code="200">GetProposedMovementsResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetProposedMovements")]
        [ModuleAuthorize(ModulesEnum.Positioning, ModulesSuItemsEnum.ProposedMovements)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProposedMovementsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetProposedMovements(
            DisplayByPositioningEnum displayBy = DisplayByPositioningEnum.ProfileId,
            DisplayMMMIEnum scenario = DisplayMMMIEnum.MM,
            long? unitId = null)
        {
            try
            {
                var response = await Mediator.Send(new GetProposedMovementsRequest
                {
                    DisplayBy = displayBy,
                    ProjectId = User.GetProjectId(),
                    Scenario = scenario,
                    UnitId = unitId,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies()
                });
                await LogUserAccess(LogActionsEnum.AccessPositioningProposedMovements.GetDescription(),
                    ModulesEnum.Positioning, ModulesSuItemsEnum.ProposedMovements);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Get Proposed Movements
        /// </summary>
        /// <param name="categoryId">categoryId</param>
        /// <param name="displayBy">default is DisplayByFinancialImpactEnum.Profile (1)</param>
        /// <param name="scenario">default is DisplayMMMIEnum.MM (1)</param>
        /// <param name="serieId">default is ProposedMovementsEnum.AdequacyOfNomenclature (1)</param>
        /// <param name="page">page</param>
        /// <param name="pageSize">pageSize</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        /// <param name="isAsc">isAsc</param>
        /// <param name="sortColumnId">sortColumnId</param>
        /// <returns>GetFullInfoProposedMovementsResponse is the object response</returns>
        /// <response code="200">GetFullInfoProposedMovementsResponse retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFullInfoProposedMovements")]
        [ModuleAuthorize(ModulesEnum.Positioning, ModulesSuItemsEnum.ProposedMovements)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFullInfoProposedMovementsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFullInfoProposedMovements(
            [Required] string categoryId,
            DisplayByPositioningEnum displayBy = DisplayByPositioningEnum.ProfileId,
            DisplayMMMIEnum scenario = DisplayMMMIEnum.MM,
            ProposedMovementsEnum serieId = ProposedMovementsEnum.AdequacyOfNomenclature,
            int page = 1,
            int pageSize = 20,
            long? unitId = null,
            bool? isAsc = null,
            int? sortColumnId = null)
        {
            try
            {
                var response = await Mediator.Send(new GetFullInfoProposedMovementsRequest
                {
                    DisplayBy = displayBy,
                    ProjectId = User.GetProjectId(),
                    Scenario = scenario,
                    UnitId = unitId,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies(),
                    SerieId = serieId,
                    CategoryId = ConverterObject.TryConvertToInt(categoryId),
                    Page = page,
                    PageSize = pageSize,
                    IsAsc = isAsc,
                    SortColumnId = sortColumnId

                });
                await LogUserAccess(LogActionsEnum.DialogPositioningProposedMovements.GetDescription(),
                    ModulesEnum.Positioning, ModulesSuItemsEnum.ProposedMovements);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Get Distribution Analysis chart
        /// </summary>
        /// <param name="displayBy">default is DisplayByFinancialImpactEnum.Profile (1)</param>
        /// <param name="scenario">default is DisplayMMMIEnum.MM (1)</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        /// <returns>GetDistributionAnalysisResponse list of object response</returns>
        /// <response code="200">GetDistributionAnalysisResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetDistributionAnalysis")]
        [ModuleAuthorize(ModulesEnum.Positioning, ModulesSuItemsEnum.DistributionAnalysis)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDistributionAnalysisResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetDistributionAnalysis(
            DisplayByPositioningEnum displayBy = DisplayByPositioningEnum.ProfileId,
            DisplayMMMIEnum scenario = DisplayMMMIEnum.MM,
            long? unitId = null)
        {
            try
            {
                var response = await Mediator.Send(new GetDistributionAnalysisRequest
                {
                    DisplayBy = displayBy,
                    ProjectId = User.GetProjectId(),
                    Scenario = scenario,
                    UnitId = unitId,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies()
                });
                await LogUserAccess(LogActionsEnum.AccessPositioningDistributionAnalysis.GetDescription(),
                    ModulesEnum.Positioning, ModulesSuItemsEnum.DistributionAnalysis);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Get Comparative Analysis Chart
        /// </summary>
        /// <param name="displayBy">default is DisplayByFinancialImpactEnum.Profile (1)</param>
        /// <param name="scenario">default is DisplayMMMIEnum.MM (1)</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        /// <returns>GetComparativeAnalysisChartResponse list of object response</returns>
        /// <response code="200">GetComparativeAnalysisChartResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetComparativeAnalysisChart")]
        [ModuleAuthorize(ModulesEnum.Positioning, ModulesSuItemsEnum.ComparativeAnalysis)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetComparativeAnalysisChartResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetComparativeAnalysisChart(
            DisplayByPositioningEnum displayBy = DisplayByPositioningEnum.ProfileId,
            DisplayMMMIEnum scenario = DisplayMMMIEnum.MM,
            long? unitId = null)
        {
            try
            {
                var response = await Mediator.Send(new GetComparativeAnalysisChartRequest
                {
                    DisplayBy = displayBy,
                    ProjectId = User.GetProjectId(),
                    Scenario = scenario,
                    UnitId = unitId,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies()
                });
                await LogUserAccess(LogActionsEnum.AccessPositioningComparativeAnalysisChart.GetDescription(),
                    ModulesEnum.Positioning, ModulesSuItemsEnum.ComparativeAnalysis);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Get FullInfo Comparative Analysis
        /// </summary>
        /// <param name="categoryId">categoryId</param>
        /// <param name="displayBy">default is DisplayByFinancialImpactEnum.Profile (1)</param>
        /// <param name="scenario">default is DisplayMMMIEnum.MM (1)</param>
        /// <param name="page">page</param>
        /// <param name="pageSize">pageSize</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        /// <param name="sortColumnId">sortColumnId</param>
        /// <param name="isAsc">isAsc</param>
        /// <param name="careerAxis">careerAxis</param>
        /// <returns>GetFullInfoComparativeAnalysisResponse is the object response</returns>
        /// <response code="200">GetFullInfoComparativeAnalysisResponse retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFullInfoComparativeAnalysis")]
        [ModuleAuthorize(ModulesEnum.Positioning, ModulesSuItemsEnum.ComparativeAnalysis)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFullInfoComparativeAnalysisResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFullInfoComparativeAnalysis(
            [Required] string categoryId,
            DisplayByPositioningEnum displayBy = DisplayByPositioningEnum.ProfileId,
            DisplayMMMIEnum scenario = DisplayMMMIEnum.MM,
            int page = 1,
            int pageSize = 20,
            long? unitId = null,
            int? sortColumnId = null,
            bool? isAsc = null,
            string careerAxis = null)
        {
            try
            {
                var response = await Mediator.Send(new GetFullInfoComparativeAnalysisRequest
                {
                    DisplayBy = displayBy,
                    ProjectId = User.GetProjectId(),
                    Scenario = scenario,
                    UnitId = unitId,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies(),
                    CategoryId = ConverterObject.TryConvertToInt(categoryId),
                    Page = page,
                    PageSize = pageSize,
                    SortColumnId = sortColumnId,
                    IsAsc = isAsc,
                    CareerAxis = careerAxis
                });
                await LogUserAccess(LogActionsEnum.DialogPositioningComparativeAnalysis.GetDescription(),
                    ModulesEnum.Positioning, ModulesSuItemsEnum.ComparativeAnalysis);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Get Comparative Analysis Table
        /// </summary>
        /// <param name="displayBy">default is DisplayByFinancialImpactEnum.Profile (1)</param>
        /// <param name="scenario">default is DisplayMMMIEnum.MM (1)</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        /// <returns>GetComparativeAnalysisTableResponse list of object response</returns>
        /// <response code="200">GetComparativeAnalysisTableResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetComparativeAnalysisTable")]
        [ModuleAuthorize(ModulesEnum.Positioning, ModulesSuItemsEnum.ComparativeAnalysis)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetComparativeAnalysisTableResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetComparativeAnalysisTable(
            DisplayByPositioningEnum displayBy = DisplayByPositioningEnum.ProfileId,
            DisplayMMMIEnum scenario = DisplayMMMIEnum.MM,
            long? unitId = null)
        {
            try
            {
                var response = await Mediator.Send(new GetComparativeAnalysisTableRequest
                {
                    DisplayBy = displayBy,
                    ProjectId = User.GetProjectId(),
                    Scenario = scenario,
                    UnitId = unitId,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies()
                });
                await LogUserAccess(LogActionsEnum.AccessPositioningComparativeAnalysisTable.GetDescription(),
                    ModulesEnum.Positioning, ModulesSuItemsEnum.ComparativeAnalysis);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Get Positions Filters
        /// </summary>
        /// <param name="displayBy">default is DisplayByFinancialImpactEnum.Profile (1)</param>
        /// <param name="scenario">default is DisplayMMMIEnum.MM (1)</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        /// <returns>GetComparativeAnalysisTableResponse list of object response</returns>
        /// <response code="200">GetComparativeAnalysisTableResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetDisplayFilter")]
        [ModuleAuthorize(ModulesEnum.Positioning, ModulesSuItemsEnum.ComparativeAnalysis)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetFilterPositionDisplayByResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetDisplayFilter(
            DisplayByPositioningEnum displayBy = DisplayByPositioningEnum.ProfileId,
            DisplayMMMIEnum scenario = DisplayMMMIEnum.MM,
            long? unitId = null)
        {
            try
            {
                var response = await Mediator.Send(new GetFilterPositionDisplayByRequest
                {
                    DisplayBy = displayBy,
                    ProjectId = User.GetProjectId(),
                    Scenario = scenario,
                    UnitId = unitId,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies()
                });
                await LogUserAccess(LogActionsEnum.AccessPositioningFilterByDashboard.GetDescription(),
                    ModulesEnum.Positioning, ModulesSuItemsEnum.ComparativeAnalysis);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Get Comparative Analysis Table Excel
        /// </summary>
        /// <param name="displayBy">default is DisplayByFinancialImpactEnum.Profile (1)</param>
        /// <param name="scenario">default is DisplayMMMIEnum.MM (1)</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        /// <returns>GetComparativeAnalysisTableExcelResponse list of object response</returns>
        /// <response code="200">GetComparativeAnalysisTableExcelResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetComparativeAnalysisTableExcel")]
        [ModuleAuthorize(ModulesEnum.Positioning, ModulesSuItemsEnum.ComparativeAnalysis)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetComparativeAnalysisTableExcelResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetComparativeAnalysisTableExcel(
            DisplayByPositioningEnum displayBy = DisplayByPositioningEnum.ProfileId,
            DisplayMMMIEnum scenario = DisplayMMMIEnum.MM,
            long? unitId = null)
        {
            try
            {
                var response = await Mediator.Send(new GetComparativeAnalysisTableExcelRequest
                {
                    DisplayBy = displayBy,
                    ProjectId = User.GetProjectId(),
                    Scenario = scenario,
                    UnitId = unitId,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies()
                });
                await LogUserAccess(LogActionsEnum.DownloadPositioningComparativeAnalysis.GetDescription(),
                    ModulesEnum.Positioning, ModulesSuItemsEnum.ComparativeAnalysis);

                return File(
                                 fileContents: response.File,
                                 contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                 fileDownloadName: $"{response.FileName}.xlsx"
                            );
            }
            catch (Exception ex)
            {
                return BadRequest($"Ocorreu um erro interno: {ex.Message}");
            }
        }



        /// <summary>
        /// Display By controller return a list of displayBy response object
        /// </summary>
        /// <returns>GetDisplayByPositioningResponse list of object response</returns>
        /// <response code="200">GetDisplayByPositioningResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetDisplayBy")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetDisplayByPositioningResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetDisplayBy()
        {
            try
            {
                var response = await Mediator.Send(new GetDisplayByPositioningRequest
                {
                    UserId = User.GetUserId(),
                    ProjectId = User.GetProjectId()
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }


    }
}