using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Api.ViewModel;
using SM.Application.TableSalary.Command;
using SM.Application.TableSalary.Queries;
using SM.Application.PositionDetails.Queries.Response;
using SM.Application.PositionDetails.Queries;
using SM.Domain.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using SM.Application.PositionDetails.Command;
using System.Collections.Generic;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class PositionDetailsController : BaseController
    {
        /// <summary>
        /// Get Salary Table Values By GSM
        /// </summary>
        ///<param name="tableId">table id selected</param>
        ///<param name="unitId">unit selected</param>
        /// <param name="gsm">gsm selected</param>
        /// <param name="contractType"></param>
        /// <param name="hoursType"></param>
        /// <returns>return salary values by gsm</returns>
        /// <response code="200">retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetSalaryTableValuesByGSM")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DataBodyPositionDetail>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetSalaryTableValuesByGSM([Required] long tableId,
                                                                   [Required] long unitId,
                                                                   [Required] long gsm,
                                                                   ContractTypeEnum contractType = ContractTypeEnum.CLT,
                                                                   DataBaseSalaryEnum hoursType = DataBaseSalaryEnum.MonthSalary)
        {
            try
            {
                var response = await Mediator.Send(new GetSalaryTableValuesByGsmRequest
                {
                    ProjectId = User.GetProjectId(),
                    UserId = User.GetUserId(),
                    TableId = tableId,
                    UnitId = unitId,
                    GSM = gsm,
                    ContractType = contractType,
                    HoursType = hoursType
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get position details
        /// </summary>
        ///<param name="positionId">position id to get details</param>
        ///<param name="moduleId">module</param>
        /// <returns>return details of position</returns>
        /// <response code="200">retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetDetails")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DetailsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetDetails([Required] long positionId, [Required] ModulesEnum moduleId)
        {
            try
            {
                var response = await Mediator.Send(new GetDetailsRequest
                {
                    ProjectId = User.GetProjectId(),
                    UserId = User.GetUserId(),
                    PositionId = positionId
                });

                await LogUserAccess(LogActionsEnum.AccessPositionDetails.GetDescription(), moduleId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        ///  Update position details
        /// </summary>
        /// <param name="viewModel">parameters to edit position details</param>
        /// <returns>status</returns>
        /// <response code="200">Columns save with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpPut("UpdatePositionDetails")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UpdatePositionDetails([FromBody] PositionDetailViewModel viewModel)
        {
            try
            {
                var response = await Mediator.Send(new UpdatePositionDetailRequest
                {
                    ProjectId = User.GetProjectId(),
                    CanEditListPosition = User.CanEditListPosition(),
                    PositionData = viewModel.Map().ToANew<UpdatePositionDetailData>()
                });
                await LogUserAccess(LogActionsEnum.SavePositionDetails.GetDescription(), viewModel.ModuleId);

                return Ok(new { message = "Detalhes do cargo foram salvos com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get salary table position
        /// </summary>
        /// <returns>return salary table position</returns>
        ///<param name="positionId">position id to get table</param>
        ///<param name="moduleId">used to log module</param>
        ///<param name="tableId">table id to get table</param>
        ///<param name="unitId">unit id</param>
        /// <param name="contractType"></param>
        /// <param name="hoursType"></param>
        /// <param name="isAsc"></param>
        /// <param name="sortColumnId"></param>
        /// <response code="200">retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetSalaryTableMapping")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSalaryTablePositionDetailsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetSalaryTableMapping([Required] long positionId,
                                                               [Required] ModulesEnum moduleId,
                                                               [Required] long tableId,
                                                               long? unitId,
                                                               ContractTypeEnum contractType = ContractTypeEnum.CLT,
                                                               DataBaseSalaryEnum hoursType = DataBaseSalaryEnum.MonthSalary,
                                                               bool? isAsc = null,
                                                               int? sortColumnId = null)
        {
            try
            {
                var response = await Mediator.Send(new GetSalaryTablePositionDetailsRequest
                {
                    ProjectId = User.GetProjectId(),
                    UserId = User.GetUserId(),
                    PositionId = positionId,
                    TableId = tableId,
                    UnitId = unitId,
                    ContractType = contractType,
                    HoursType = hoursType,
                    IsAsc = isAsc,
                    SortColumnId = sortColumnId,
                });

                await LogUserAccess(LogActionsEnum.AccessSalaryTablePositionDetails.GetDescription(), moduleId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        ///  Update salary table position
        /// </summary>
        /// <param name="viewModel">parameters to edit salary table position</param>
        /// <returns>status</returns>
        /// <response code="200">Columns save with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpPut("UpdateSalaryTableMapping")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UpdateSalaryTableMapping([FromBody] UpdateSalaryTablePositionDetailsViewModel viewModel)
        {
            try
            {
                var response = await Mediator.Send(new UpdateSalaryTablePositionDetailsRequest
                {
                    ProjectId = User.GetProjectId(),
                    CanEditMappingPositionSM = User.CanEditMappingPositionSM(),
                    SalaryTableMappings = viewModel.SalaryTableMappings.Map().ToANew<List<SalaryTableMappingData>>()
                });
                await LogUserAccess(LogActionsEnum.SaveSalaryTablePositionDetails.GetDescription(), viewModel.ModuleId);

                return Ok(new { message = "Detalhes do cargo foram salvos com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }
    }
}