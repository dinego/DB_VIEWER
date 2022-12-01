using CMC.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Application.DashBoard.Queries;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    [ModuleAuthorize(ModulesEnum.DashBoard, ModulesSuItemsEnum.None)]
    public class DashBoardController : BaseController
    {
        /// <summary>
        /// Get data for render positions chart
        /// </summary>
        /// <returns>GetPositionsChartResponse is the object response</returns>
        /// <response code="200">GetPositionsChartResponse retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetPositionsChart")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPositionsChartResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetPositionsChart(long? unit, DisplayMMMIEnum movements, DisplayByPositioningEnum displayBy = DisplayByPositioningEnum.ProfileId)
        {
            try
            {
                var response = await Mediator.Send(new GetPositionsChartRequest
                { 
                    UserId = User.GetUserId(),
                    DisplayBy = displayBy,
                    ProjectId = User.GetProjectId(),
                    CompaniesId = User.GetUserCompanies(),
                    Unit = unit,
                    Moviments = movements,
                    
                });
                await LogUserAccess(LogActionsEnum.AccessDashBoardPosition.GetDescription(), 
                    ModulesEnum.DashBoard);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get data for render Financial Impact Cards
        /// </summary>
        /// <returns>GetFinancialImpactCardsResponse is the object response</returns>
        /// <response code="200">GetFinancialImpactCardsResponse retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFinancialImpactCards")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFinancialImpactCardsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFinancialImpactCards(
            long? unit,
            DisplayMMMIEnum movements = DisplayMMMIEnum.MM,
            DisplayByPositioningEnum displayBy = DisplayByPositioningEnum.ProfileId)
        {
            try
            {
                var response = await Mediator.Send(new GetFinancialImpactCardsRequest
                {
                    DisplayBy = displayBy,
                    UserId = User.GetUserId(),
                    ProjectId = User.GetProjectId(),
                    CompaniesId = User.GetUserCompanies(),
                    UnitId = unit,
                    Scenario = movements
                });
                await LogUserAccess(LogActionsEnum.AccessDashBoardFinancial.GetDescription(), 
                    ModulesEnum.DashBoard);

                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }


        /// <summary>
        /// Proposed Movements controller return a list of movements response object
        /// </summary>
        /// <returns>GetProposedMovementsTypesResponse list of object response</returns>
        /// <response code="200">GetProposedMovementsTypesResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [Obsolete]
        [HttpGet("GetProposedMovementsTypes")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetProposedMovementsTypesResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetProposedMovementsTypes()
        {
            try
            {
                var response = await Mediator.Send(new GetProposedMovementsTypesRequest { UserId = User.GetUserId() });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get data for render Proposed Movements Chart
        /// </summary>
        /// <returns>GetProposedMovementsChartResponse is the object response</returns>
        /// <response code="200">GetProposedMovementsChartResponse retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetProposedMovementsChart")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProposedMovementsChartResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetProposedMovementsChart(
            long? unit,
            DisplayMMMIEnum movements,
            ProposedMovementsEnum proposedMovements)
        {
            try
            {
                var response = await Mediator.Send(new GetProposedMovementsChartRequest
                {
                    UserId = User.GetUserId(),
                    ProjectId = User.GetProjectId(),
                    CompaniesId = User.GetUserCompanies(),
                    UnitId = unit,
                    Scenario = movements,
                    ProposedMovements = proposedMovements
                });
                await LogUserAccess(LogActionsEnum.AccessDashBoardProposedMovements.GetDescription(),
                            ModulesEnum.DashBoard);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get data for render Distribution Analysis Chart
        /// </summary>
        /// <returns>GetDistributionAnalysisChartResponse is the object response</returns>
        /// <response code="200">GetDistributionAnalysisChartResponse retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetDistributionAnalysisChart")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDistributionAnalysisChartResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetDistributionAnalysisChart(
            long? unit,
            DisplayMMMIEnum movements, 
            DisplayByPositioningEnum displayBy = DisplayByPositioningEnum.ProfileId)
        {
            try
            {
                var response = await Mediator.Send(new GetDistributionAnalysisChartRequest
                {
                    UserId = User.GetUserId(),
                    ProjectId = User.GetProjectId(),
                    CompaniesId = User.GetUserCompanies(),
                    UnitId = unit,
                    Scenario = movements,
                    DisplayBy = displayBy,
                });;
                await LogUserAccess(LogActionsEnum.AccessDashBoardDistributionAnalysis.GetDescription(),
                    ModulesEnum.DashBoard);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get data for render Distribution Analysis Chart
        /// </summary>
        /// <param name="displayBy">default is DisplayByFinancialImpactEnum.Profile (1)</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        /// <param name="movements"></param>
        /// <returns>GetComparativeAnalysisDashResponse is the list of object response</returns>
        /// <response code="200">GetComparativeAnalysisDashResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetComparativeAnalysisChart")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetComparativeAnalysisDashResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetComparativeAnalysisChart(
            long? unitId,
            DisplayMMMIEnum movements,
            DisplayByPositioningEnum displayBy = DisplayByPositioningEnum.ProfileId)
        {
            try
            {
                var response = await Mediator.Send(new GetComparativeAnalysisDashRequest
                {
                    UserId = User.GetUserId(),
                    DisplayBy = displayBy,
                    ProjectId = User.GetProjectId(),
                    CompaniesId = User.GetUserCompanies(),
                    UnitId = unitId,
                    Scenario = movements,
                });
                await LogUserAccess(LogActionsEnum.AccessDashBoardComparativeAnalysis.GetDescription(),
                    ModulesEnum.DashBoard);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }
    }
}