using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Api.ViewModel;
using SM.Application.GetSalaryPositionTable;
using SM.Application.GetSalaryTable;
using SM.Application.PositionDetails.Queries.Response;
using SM.Application.TableSalary.Command;
using SM.Application.TableSalary.Queries;
using SM.Application.TableSalary.Queries.Response;
using SM.Domain.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    [ModuleAuthorize(ModulesEnum.TableSalary, ModulesSuItemsEnum.None)]
    public class TableSalaryController : BaseController
    {
        /// <summary>
        /// TableSalary controller return a TableSalary response object
        /// </summary>
        /// <param name="contractType">contractTypefilter</param>
        /// <param name="groupId">groupId filter</param>
        /// <param name="hoursType">hoursTypefilter</param>
        /// <param name="tableId">tableId is required</param>
        /// <param name="unitId">unitId filter</param>
        /// <param name="page">page is opt</param>
        /// <param name="pageSize">pageSize is opt</param>
        /// <param name="isAsc">isAsc is optional</param>
        /// <param name="showAllGsm">show all gsm</param>
        /// <param name="sortColumnId">column id to ordenable</param>
        /// <returns>GetSalaryTableResponse list of object response</returns>
        /// <response code="200">GetSalaryTableResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetSalaryTable")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSalaryTableResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetSalaryTable(
            [Required] long tableId,
            long? unitId,
            long? groupId,
            ContractTypeEnum contractType = ContractTypeEnum.CLT,
            DataBaseSalaryEnum hoursType = DataBaseSalaryEnum.MonthSalary,
            int page = 1,
            int pageSize = 20,
            bool? isAsc = null,
            bool? showAllGsm = null,
            int? sortColumnId = null)
        {
            try
            {
                var response = await Mediator.Send(new GetSalaryTableRequest
                {
                    ProjectId = User.GetProjectId(),
                    CompaniesId = User.GetUserCompanies(),
                    ContractType = contractType,
                    GroupId = groupId,
                    UserId = User.GetUserId(),
                    HoursType = hoursType,
                    TableId = tableId,
                    Page = page,
                    UnitId = unitId,
                    PageSize = pageSize,
                    IsAsc = isAsc,
                    ShowAllGsm = showAllGsm.GetValueOrDefault(false),
                    SortColumnId = sortColumnId,
                    CanEditGlobalLabels = User.CanEditGlobalLabels()
                });
                await LogUserAccess(LogActionsEnum.AccessSalaryTable.GetDescription(),
                    ModulesEnum.TableSalary);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// TableSalaryPosition controller return a TableSalary response object
        /// </summary>
        /// <param name="contractType">contractType filter</param>
        /// <param name="groupId">groupId filter</param>
        /// <param name="hoursType">hoursType filter</param>
        /// <param name="tableId">tableId filter</param>
        /// <param name="unitId">unitId filter</param>
        /// <param name="page">page is opt</param>
        /// <param name="pageSize">pageSize is opt</param>
        /// <param name="isAsc">isAsc is optional</param>
        /// <param name="sortColumnId">column id to ordenable</param>
        /// <param name="filterSearch">filter position is optional</param>
        /// <param name="ignorePagination">used to ignore pagination (export excel)</param>
        /// <returns>GetSalaryTableResponse list of object response</returns>
        /// <response code="200">GetSalaryTableResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetSalaryTablePosition")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSalaryTablePositionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetSalaryTablePosition(
            [Required] long tableId,
            long? unitId,
            long? groupId,
            ContractTypeEnum contractType = ContractTypeEnum.CLT,
            DataBaseSalaryEnum hoursType = DataBaseSalaryEnum.MonthSalary,
            int page = 1,
            int pageSize = 20,
            string filterSearch = "",
            bool? ignorePagination = false,
            bool? isAsc = null,
            int? sortColumnId = null
            )
        {
            try
            {
                var response = await Mediator.Send(new GetSalaryTablePositionRequest
                {
                    ProjectId = User.GetProjectId(),
                    CompaniesId = User.GetUserCompanies(),
                    ContractType = contractType,
                    GroupId = groupId,
                    UserId = User.GetUserId(),
                    HoursType = hoursType,
                    TableId = tableId,
                    Page = page,
                    UnitId = unitId,
                    PageSize = pageSize,
                    IsAsc = isAsc,
                    FilterSearch = filterSearch,
                    CanEditGlobalLabels = User.CanEditGlobalLabels(),
                    IgnorePagination = ignorePagination.GetValueOrDefault(),
                    SortColumnId = sortColumnId
                });
                await LogUserAccess(LogActionsEnum.AccessPostionSalaryTable.GetDescription(),
                    ModulesEnum.TableSalary);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }


        /// <summary>
        /// TableSalary controller return a TableSalary Excel response object
        /// </summary>
        /// <param name="contractType">contractType is required</param>
        /// <param name="groupId">groupId is required</param>
        /// <param name="hoursType">hoursType is required</param>
        /// <param name="isAsc">isAsc</param>
        /// <param name="tableId">tableId is required</param>
        /// <param name="unitId"></param>
        /// <param name="showAllGsm"></param>
        /// <param name="sortColumnId"></param>
        /// <returns>GetSalaryTableResponse list of object response</returns>
        /// <response code="200">GetSalaryTableResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetSalaryTableExcel")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSalaryTableExcelResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetSalaryTableExcel(
            [Required] long tableId,
            long? unitId,
            long? groupId,
            ContractTypeEnum contractType = ContractTypeEnum.CLT,
            DataBaseSalaryEnum hoursType = DataBaseSalaryEnum.MonthSalary,
            bool? isAsc = null,
            bool? showAllGsm = null,
            int? sortColumnId = null
            )
        {
            try
            {
                var response = await Mediator.Send(new GetSalaryTableExcelRequest
                {
                    ProjectId = User.GetProjectId(),
                    UnitId = unitId,
                    ContractType = contractType,
                    GroupId = groupId,
                    UserId = User.GetUserId(),
                    HoursType = hoursType,
                    TableId = tableId,
                    CompaniesId = User.GetUserCompanies(),
                    IsAsc = isAsc,
                    ShowAllGsm = showAllGsm.GetValueOrDefault(false),
                    SortColumnId = sortColumnId
                });

                await LogExcelAccess(LogActionsEnum.DownloadSalaryTable.GetDescription(),
                        ModulesEnum.TableSalary);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }
        /// <summary>
        ///  Update/Add display columns SM
        /// </summary>
        /// <param name="updateDisplayColumnsRequest">updateDisplayColumnsRequest</param>
        /// <returns>status</returns>
        /// <response code="200">Columns save with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpPost("UpdateDisplayColumns")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UpdateDisplayColumns([FromBody] UpdateDisplayColumnsSalaryTableViewModel updateDisplayColumnsRequest)
        {
            try
            {
                var request = updateDisplayColumnsRequest.Map().ToANew<UpdateDisplayColumnsRequest>();
                if (request == null)
                    return BadRequest(new { message = "Não encontramos os parâmetros de entrada para processar sua requisição." });
                request.UserId = User.GetUserId();
                request.CanEditGlobalLabels = User.CanEditGlobalLabels();
                request.CanEditLocalLabels = User.CanEditLocalLabels();
                var response = await Mediator.Send(request);
                await LogUserAccess(LogActionsEnum.SaveSalaryTableUpdate.GetDescription(),
                    ModulesEnum.TableSalary);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        ///  Get Salary Values for update salary Table
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="projectId"></param>
        /// <param name="groupId"></param>
        /// <returns>status</returns>
        /// <response code="200">Return values of tableId</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetEditTableValues")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetEditTableValuesResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetEditTableValues(
            [Required] long tableId,
            [Required] long projectId,
            long? groupId)
        {
            try
            {
                var response = await Mediator.Send(new GetEditTableValuesRequest
                {
                    ProjectId = User.GetProjectId(),
                    CompanyId = User.GetCompanyId(),
                    GroupId = groupId,
                    UserId = User.GetUserId(),
                    TableId = tableId,
                });
                await LogUserAccess(LogActionsEnum.AccessGetSalarialTableValues.GetDescription(),
                    ModulesEnum.TableSalary);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        ///  Update SalarialTable Values
        /// </summary>
        /// <param name="UpdateSalarialTableRequest">UpdateSalarialTableRequest</param>
        /// <returns>status</returns>
        /// <response code="200">Update Salary Table Values</response>
        /// <response code="400">Internal Error</response>
        [HttpPost("UpdateSalaryTable")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UpdateSalaryTable([FromBody] UpdateSalaryTableViewModel UpdateSalarialTableRequest)
        {
            try
            {
                var request = UpdateSalarialTableRequest.Map().ToANew<UpdateSalaryTableRequest>();

                if (request == null)
                    return BadRequest(new { message = "Não encontramos os parâmetros de entrada para processar sua requisição." });

                request.UserId = User.GetUserId();
                var response = await Mediator.Send(request);

                await LogUserAccess(LogActionsEnum.SaveSalaryTableUpdate.GetDescription(),
                    ModulesEnum.TableSalary);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        ///  Update SalarialTable Info
        /// </summary>
        /// <param name="updateSalaryTableInfoRequest">updateSalarialTableInfoRequest</param>
        /// <returns>status</returns>
        /// <response code="200">Update Salary Info</response>
        /// <response code="400">Internal Error</response>
        [HttpPost("UpdateSalaryTableInfo")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UpdateSalaryTableInfo([FromBody] SalaryTableInfoViewModel updateSalaryTableInfoRequest)
        {
            try
            {
                var request = updateSalaryTableInfoRequest.Map().ToANew<UpdateSalaryTableInfoRequest>();
                if (request == null)
                    return BadRequest(new { message = "Não encontramos os parâmetros de entrada para processar sua requisição." });
                request.UserId = User.GetUserId();
                var response = await Mediator.Send(request);
                await LogUserAccess(LogActionsEnum.SaveSalaryTableUpdate.GetDescription(),
                    ModulesEnum.TableSalary);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Return Data for populate Graph Data in Salary Table
        /// </summary>
        /// <param name="contractType">contractType is required</param>
        /// <param name="hoursType">hoursType is required</param>
        /// <param name="tableId">tableId is required</param>
        /// <param name="groupId">groupId is optional</param>
        /// <param name="unitId">unit is optional</param>
        /// <param name="rangeInit"></param>
        /// <param name="rangeFinal"></param>
        /// <returns>GetSalaryGraphResponse object response</returns>
        /// <response code="200">GetSalaryGraphResponse object retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetSalaryGraph")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSalaryGraphResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetSalaryGraph(
            [Required] long tableId,
            int rangeInit,
            int rangeFinal,
            long? unitId,
            long? groupId,
            ContractTypeEnum contractType = ContractTypeEnum.CLT,
            DataBaseSalaryEnum hoursType = DataBaseSalaryEnum.MonthSalary)
        {
            try
            {
                var response = await Mediator.Send(new GetSalaryGraphRequest
                {
                    InitRange = rangeInit,
                    FinalRange = rangeFinal,
                    ProjectId = User.GetProjectId(),
                    CompaniesId = User.GetUserCompanies(),
                    ContractType = contractType,
                    GroupId = groupId,
                    UserId = User.GetUserId(),
                    HoursType = hoursType,
                    TableId = tableId,
                    UnitId = unitId,
                });
                await LogUserAccess(LogActionsEnum.AccessGraphSalaryTable.GetDescription(),
                    ModulesEnum.TableSalary);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Return Range for populate Graph Data in Salary Table
        /// </summary>
        /// <param name="tableId">tableId is required</param>
        /// <param name="groupId">groupId is optional</param>
        /// <param name="unitId">unit is optional</param>
        /// <returns>GetSalaryGraphResponse object response</returns>
        /// <response code="200">GetSalaryGraphResponse object retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetRangeSalaryGraph")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetRangeSalaryGraphResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetRangeSalaryGraph(
            [Required] long tableId,
            long? unitId,
            long? groupId)
        {
            try
            {
                var response = await Mediator.Send(new GetRangeSalaryGraphRequest
                {
                    ProjectId = User.GetProjectId(),
                    GroupId = groupId,
                    UserId = User.GetUserId(),
                    TableId = tableId,
                    UnitId = unitId
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