using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Api.ViewModel;
using SM.Application.Position.Command;
using SM.Application.Position.Querie;
using SM.Application.Position.Queries;
using SM.Application.Position.Queries.Response;
using SM.Domain.Enum;
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
    public class PositionController : BaseController
    {
        /// <summary>
        ///  Update/Add display columns SM (Map Position)
        /// </summary>
        /// <param name="updateDisplayColumnsMapRequest">updateDisplayColumnsMapRequest</param>
        /// <returns>status</returns>
        /// <response code="200">Columns save with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpPost("UpdateDisplayColumnsMap")]
        [ModuleAuthorize(ModulesEnum.Position, ModulesSuItemsEnum.Map)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UpdateDisplayColumnsMap([FromBody] UpdateDisplayColumnsMapViewModel updateDisplayColumnsMapRequest)
        {
            try
            {
                var request = updateDisplayColumnsMapRequest.Map().ToANew<UpdateDisplayColumnsMapRequest>();
                if (request == null)
                    return BadRequest(new { message = "Não encontramos os parâmetros de entrada para processar sua requisição." });
                request.UserId = User.GetUserId();
                var response = await Mediator.Send(request);
                await LogUserAccess(LogActionsEnum.SavePositionUpdateMap.GetDescription(),
                    ModulesEnum.Position, ModulesSuItemsEnum.Map);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        ///  Update/Add display columns SM (List Position)
        /// </summary>
        /// <param name="updateDisplayColumnsListRequest">updateDisplayColumnsListRequest</param>
        /// <returns>status</returns>
        /// <response code="200">Columns save with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpPost("UpdateDisplayColumnsList")]
        [ModuleAuthorize(ModulesEnum.Position, ModulesSuItemsEnum.Architecture)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UpdateDisplayColumnsList([FromBody] UpdateDisplayColumnsListViewModel updateDisplayColumnsListRequest)
        {
            try
            {
                var request = updateDisplayColumnsListRequest.Map().ToANew<UpdateDisplayColumnsListRequest>();
                if (request == null)
                    return BadRequest(new { message = "Não encontramos os parâmetros de entrada para processar sua requisição." });
                request.UserId = User.GetUserId();
                var response = await Mediator.Send(request);
                await LogUserAccess(LogActionsEnum.SavePositionUpdateList.GetDescription(),
                    ModulesEnum.Position, ModulesSuItemsEnum.Architecture);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Map Position
        /// </summary>
        ///<param name="displayBy">displayBy is default 1</param>
        ///<param name="groupId">groupId when null consider all groupIds</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        ///<param name="page">default is 1</param>
        ///<param name="pageSize">default is 10</param>
        /// <param name="isAsc">isAsc</param>
        /// <param name="columns"></param>
        ///<param name="removeRowsEmpty">removeRowsEmpty is false</param>
        ///<param name="tableId">tableId when null consider all tableIds</param>
        ///<param name="term">term</param>
        ///<param name="showJustWithOccupants">default is false</param>
        /// <returns>GetMapPositionResponse list of object response (map position)</returns>
        /// <response code="200">GetMapPositionResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetMapPosition")]
        [ModuleAuthorize(ModulesEnum.Position, ModulesSuItemsEnum.Map)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMapPositionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetMapPosition(
            DisplayByMapPositionEnum displayBy = DisplayByMapPositionEnum.AxisCarreira,
            string term = null,
            long? tableId = null,
            long? unitId = null,
            long? groupId = null,
            bool removeRowsEmpty = false,
            bool showJustWithOccupants = false,
            int page = 1,
            int pageSize = 20,
            bool? isAsc = null,
            string columns = null)
        {
            try
            {
                var response = await Mediator.Send(new GetMapPositionRequest
                {
                    DisplayBy = displayBy,
                    GroupId = groupId,
                    Page = page,
                    PageSize = pageSize,
                    ProjectId = User.GetProjectId(),
                    RemoveRowsEmpty = removeRowsEmpty,
                    TableId = tableId,
                    UnitId = unitId,
                    Term = term,
                    ShowJustWithOccupants = showJustWithOccupants,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies(),
                    IsAsc = isAsc,
                    Columns = !string.IsNullOrEmpty(columns) ? JsonConvert.DeserializeObject<List<string>>(columns) : new List<string>()
                });
                await LogUserAccess(LogActionsEnum.AccessPositionMap.GetDescription(),
                    ModulesEnum.Position, ModulesSuItemsEnum.Map);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Map Position Excel
        /// </summary>
        ///<param name="displayBy">displayBy is default 1</param>
        ///<param name="groupId">groupId when null consider all groupIds</param>
        ///<param name="unitId">unitId when null consider all unitIds</param>
        ///<param name="removeRowsEmpty">removeRowsEmpty is false</param>
        ///<param name="tableId">tableId when null consider all tableIds</param>
        ///<param name="term">term</param>
        ///<param name="showJustWithOccupants">default is false</param>
        /// <param name="isAsc">isAsc</param>
        /// <param name="columns"></param>
        /// <returns>GetMapPositionExcelResponse list of object response (map position)</returns>
        /// <response code="200">GetMapPositionExcelResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetMapPositionExcel")]
        [ModuleAuthorize(ModulesEnum.Position, ModulesSuItemsEnum.Map)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMapPositionExcelResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetMapPositionExcel(
            DisplayByMapPositionEnum displayBy = DisplayByMapPositionEnum.AxisCarreira,
            string term = null,
            long? tableId = null,
            long? unitId = null,
            long? groupId = null,
            bool removeRowsEmpty = false,
            bool showJustWithOccupants = false,
            bool? isAsc = null,
            string columns = null)
        {
            try
            {
                var response = await Mediator.Send(new GetMapPositionExcelRequest
                {
                    DisplayBy = displayBy,
                    GroupId = groupId,
                    ProjectId = User.GetProjectId(),
                    RemoveRowsEmpty = removeRowsEmpty,
                    TableId = tableId,
                    UnitId = unitId,
                    Term = term,
                    ShowJustWithOccupants = showJustWithOccupants,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies(),
                    IsAsc = isAsc,
                    Columns = !string.IsNullOrEmpty(columns) ? JsonConvert.DeserializeObject<List<string>>(columns) : new List<string>()
                });

                await LogExcelAccess(LogActionsEnum.DownloadPositionMap.GetDescription(),
                    ModulesEnum.Position, ModulesSuItemsEnum.Map);

                return File(
                                 fileContents: response.File,
                                 contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                 fileDownloadName: $"{response.FileName}.xlsx"
                            );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get Positions in a response object
        /// </summary>
        /// <returns>GetAllPositions list of object response</returns>
        /// <param name="tableId">tableId is Required</param>
        /// <param name="term">term</param>
        /// <param name="unitId">unitId when null consider all unitIds</param>
        /// <param name="showJustWithOccupants">default is false</param>
        /// <param name="contractType">default is ContractTypeEnum.CLT(0)</param>
        /// <param name="hoursType">defautl is DataBaseSalaryEnum.MonthSalary(0)</param>
        /// <param name="page">default is 1</param>
        /// <param name="pageSize">default is 10</param>
        /// <param name="sortColumnId">sortColumnId</param>
        /// <param name="isAsc">isAsc</param>
        /// <param name="columns"></param>
        /// <response code="200">GetAllPositions list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetAllPositions")]
        [ModuleAuthorize(ModulesEnum.Position, ModulesSuItemsEnum.Architecture)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllPositionsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetAllPositions(
            [Required] long tableId,
            string term = null,
            long? unitId = null,
            bool showJustWithOccupants = false,
            ContractTypeEnum contractType = ContractTypeEnum.CLT,
            DataBaseSalaryEnum hoursType = DataBaseSalaryEnum.MonthSalary,
            int page = 1,
            int pageSize = 20,
            int? sortColumnId = null,
            bool? isAsc = null,
            string columns = null)
        {
            try
            {
                var response = await Mediator.Send(
                    new GetAllPositionsRequest
                    {
                        Page = page,
                        PageSize = pageSize,
                        CompaniesId = User.GetUserCompanies(),
                        ContractType = contractType,
                        HoursType = hoursType,
                        ProjectId = User.GetProjectId(),
                        TableId = tableId,
                        Term = term,
                        UnitId = unitId,
                        UserId = User.GetUserId(),
                        ShowJustWithOccupants = showJustWithOccupants,
                        SortColumnId = sortColumnId,
                        IsAsc = isAsc,
                        ColumnsExcluded = !string.IsNullOrEmpty(columns) ? JsonConvert.DeserializeObject<List<int>>(columns) : new List<int>()
                    });
                await LogUserAccess(LogActionsEnum.AccessPositionList.GetDescription(),
                    ModulesEnum.Position, ModulesSuItemsEnum.Architecture);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get Positions in a response object - Excel file
        /// </summary>
        /// <returns>GetAllPositionsExcel list of object response</returns>
        /// <param name="tableId">tableId is Required</param>
        /// <param name="term">term</param>
        /// <param name="showJustWithOccupants"></param>
        /// <param name="unitId">unitId when null consider all unitIds</param>
        /// <param name="contractType">default is ContractTypeEnum.CLT(0)</param>
        /// <param name="hoursType">defautl is DataBaseSalaryEnum.MonthSalary(0)</param>
        /// <param name="isAsc">isAsc</param>
        /// <param name="sortColumnId">sortColumnId</param>
        /// <param name="columns"></param>
        /// <response code="200">GetAllPositions list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetAllPositionsExcel")]
        [ModuleAuthorize(ModulesEnum.Position, ModulesSuItemsEnum.Architecture)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllPositionsExcelResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetAllPositionsExcel(
            [Required] long tableId,
            string term = null,
            bool showJustWithOccupants = false,
            long? unitId = null,
            ContractTypeEnum contractType = ContractTypeEnum.CLT,
            DataBaseSalaryEnum hoursType = DataBaseSalaryEnum.MonthSalary,
            bool? isAsc = null,
            int? sortColumnId = null,
            string columns = null)
        {
            try
            {
                var response = await Mediator.Send(
                    new GetAllPositionsExcelRequest
                    {
                        CompaniesId = User.GetUserCompanies(),
                        ContractType = contractType,
                        HoursType = hoursType,
                        ProjectId = User.GetProjectId(),
                        TableId = tableId,
                        Term = term,
                        UnitId = unitId,
                        UserId = User.GetUserId(),
                        IsAsc = isAsc,
                        SortColumnId = sortColumnId,
                        ShowJustWithOccupants = showJustWithOccupants,
                        Columns = !string.IsNullOrEmpty(columns) ? JsonConvert.DeserializeObject<List<int>>(columns) : new List<int>()
                    });

                await LogExcelAccess(LogActionsEnum.DownloadPositionList.GetDescription(),
                                    ModulesEnum.Position, ModulesSuItemsEnum.Architecture);

                return File(
                         fileContents: response.File,
                         contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                         fileDownloadName: $"{response.FileName}.xlsx"
                     );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get All DisplayBy list
        /// </summary>
        /// <returns>GetAllPositions list of object response</returns>
        /// <response code="200">GetAllPositions list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetAllDisplayBy")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllDisplayByResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetAllDisplayBy()
        {
            try
            {
                var response = await Mediator.Send(new GetAllDisplayByRequest
                {
                    ProjectId = User.GetProjectId()
                });

                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get full info from positionSm (dialog)
        /// </summary>
        /// <param name="positionSmId">positionSmId is required</param>
        /// <returns>GetFullInfoPositionResponse list of object response</returns>
        /// <response code="200">GetFullInfoPositionResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFullInfoPosition")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFullInfoPositionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFullInfoPosition([Required] long positionSmId)
        {
            try
            {
                var response = await Mediator.Send(new GetFullInfoPositionRequest
                {
                    UserId = User.GetUserId(),
                    PositionSmIdLocal = positionSmId,
                    ProjectId = User.GetProjectId(),
                    CompaniesId = User.GetUserCompanies()
                });
                await LogUserAccess(LogActionsEnum.AccessDialogPosition.GetDescription(),
                    ModulesEnum.Position);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }


    }
}