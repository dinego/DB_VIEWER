using CMC.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SM.Api.Helpers;
using SM.Api.ViewModel;
using SM.Application.GetSalaryPositionTable;
using SM.Application.GetSalaryTable;
using SM.Application.Share.Queries;
using SM.Application.TableSalary.Queries;
using SM.Application.TableSalary.Queries.Response;
using SM.Domain.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ShareTableSalaryController : BaseController
    {
        /// <summary>
        /// Get share data for secretkey
        /// </summary>
        /// <param name="secretKey">secret key</param>
        /// <returns>parameters to use on salary table screen</returns>
        /// <response code="200">Key retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetShareData")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetShareData([Required] string secretKey)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var parameters = new ShareDataSalaryTableViewModel();
                shareResult.Parameters.TryParseJson(out parameters);
                parameters.User = shareResult.UserShared;
                parameters.Date = shareResult.DateShared;
                return Ok(parameters);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }
        /// <summary>
        /// Get table salary to share
        /// </summary>
        /// <param name="secretKey">secretKey</param>
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
        /// <returns>Table salary share and share data of object response</returns>
        /// <response code="200">GetTableSalaryResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetSalaryTable")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSalaryTableResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetSalaryTable([Required] string secretKey,
            [Required] long tableId,
            ContractTypeEnum contractType,
            DataBaseSalaryEnum hoursType,
            int page = 1,
            int pageSize = 20,
            long? unitId = null,
            long? groupId = null,
            bool? isAsc = null,
            bool? showAllGsm = null,
            int? sortColumnId = null)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var response = await Mediator.Send(new GetSalaryTableRequest
                {
                    ProjectId = shareResult.ProjectId,
                    CompaniesId = shareResult.CompaniesId,
                    ContractType = contractType,
                    GroupId = groupId,
                    UserId = shareResult.UserId,
                    HoursType = hoursType,
                    TableId = tableId,
                    Page = page,
                    UnitId = unitId,
                    PageSize = pageSize,
                    IsAsc = isAsc,
                    ShowAllGsm = showAllGsm.GetValueOrDefault(false),
                    SortColumnId = sortColumnId,
                    ColumnsExcluded = shareResult.ColumnsExcluded.Safe().Select(s => Convert.ToInt32(s)).ToList()
                });
                await LogShareAccess(secretKey);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get position table salary to share
        /// </summary>
        /// <param name="secretKey">secretKey</param>
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
        /// <returns>Table salary share and share data of object response</returns>
        /// <response code="200">GetTableSalaryResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetSalaryTablePosition")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSalaryTableResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetSalaryTablePosition([Required] string secretKey,
            [Required] long tableId,
            ContractTypeEnum contractType = ContractTypeEnum.CLT,
            DataBaseSalaryEnum hoursType = DataBaseSalaryEnum.MonthSalary,
            int page = 1,
            int pageSize = 20,
            string filterSearch = "",
            long? unitId = null,
            long? groupId = null,
            bool? isAsc = null,
            int? sortColumnId = null)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var response = await Mediator.Send(new GetSalaryTablePositionRequest
                {
                    ProjectId = shareResult.ProjectId,
                    CompaniesId = shareResult.CompaniesId,
                    ContractType = contractType,
                    GroupId = groupId,
                    UserId = shareResult.UserId,
                    HoursType = hoursType,
                    TableId = tableId,
                    Page = page,
                    UnitId = unitId,
                    PageSize = pageSize,
                    IsAsc = isAsc,
                    FilterSearch = filterSearch,
                    SortColumnId = sortColumnId,
                    ColumnsExcluded = shareResult.ColumnsExcluded.Safe().Select(s => Convert.ToInt32(s)).ToList()
                });

                await LogShareAccess(secretKey);
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
        /// <param name="secretKey">secretKey</param>
        /// <param name="contractType">contractType is required</param>
        /// <param name="groupId">groupId is required</param>
        /// <param name="hoursType">hoursType is required</param>
        /// <param name="tableId">tableId is required</param>
        /// <param name="rangeInit"></param>
        /// <param name="rangeFinal"></param>
        /// <param name="unitId"></param>
        /// <returns>GetSalaryGraphResponse object response</returns>
        /// <response code="200">GetSalaryGraphResponse object retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetSalaryGraph")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSalaryTableResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetSalaryGraph([Required] string secretKey,
        [Required] long tableId,
            int rangeInit,
            int rangeFinal,
            long? unitId,
            long? groupId,
            ContractTypeEnum contractType,
            DataBaseSalaryEnum hoursType)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var response = await Mediator.Send(new GetSalaryGraphRequest
                {
                    InitRange = rangeInit,
                    FinalRange = rangeFinal,
                    ProjectId = shareResult.ProjectId,
                    CompaniesId = shareResult.CompaniesId,
                    ContractType = contractType,
                    GroupId = groupId,
                    UserId = shareResult.UserId,
                    HoursType = hoursType,
                    TableId = tableId,
                    UnitId = unitId
                });
                await LogShareAccess(secretKey);
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
        /// <param name="secretKey">secretKey is required</param>
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
            [Required] string secretKey,
            [Required] long tableId,
            long? unitId,
            long? groupId)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var response = await Mediator.Send(new GetRangeSalaryGraphRequest
                {
                    ProjectId = shareResult.ProjectId,
                    GroupId = groupId,
                    UserId = shareResult.UserId,
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
