using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Application.Common.Queries;
using SM.Application.GetTableSalary;
using SM.Application.TableSalary.Queries.GetSalaryTableValuesHandler;
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
    public class CommonController : BaseController
    {
        /// <summary>
        /// TableSalary controller return a TableSalary response object
        /// </summary>
        /// /// <param name="unitId">unit Id</param>
        /// <returns>GetAllSalaryTablesResponse list of object response</returns>
        /// <response code="200">GetAllSalaryTablesResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetAllSalaryTables")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllSalaryTablesResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetAllSalaryTables(long? unitId)
        {
            try
            {
                var response = await Mediator.Send(new GetAllSalaryTablesRequest
                {
                    UserId = User.GetUserId(),
                    Units = User.GetUserCompanies().ToArray(),
                    ProjectId = User.GetProjectId(),
                    UnitId = unitId
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get all parameter by project
        /// </summary>
        /// <returns>list of parameters response</returns>
        /// <response code="200">list of parameters retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetAllParameters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCommonParametersResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetAllParameters()
        {
            try
            {
                var response = await Mediator.Send(new CommonParametersRequest
                {
                    ProjectId = User.GetProjectId(),
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get all parameter by project
        /// </summary>
        ///<param name="parameters">parameter id related of career axis</param>
        /// <returns>list of parameters response</returns>
        /// <response code="200">list of parameters retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetAllCareerAxis")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetAllCareerAxisResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetAllCareerAxis(string parameters)
        {
            try
            {
                var response = await Mediator.Send(new GetAllCareerAxisRequest
                {
                    ProjectId = User.GetProjectId(),
                    Parameters = !string.IsNullOrEmpty(parameters) ? JsonConvert.DeserializeObject<List<long>>(parameters) : new List<long>()
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get all level company of project
        /// </summary>
        /// <returns>list of levels response</returns>
        /// <response code="200">list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetAllLevels")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllLevelsRequest))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetAllLevels()
        {
            try
            {
                var response = await Mediator.Send(new GetAllLevelsRequest
                {
                    CompanyId = User.GetCompanyId(),
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get all gsm by table
        /// </summary>
        ///<param name="tableId">table id</param>
        ///<param name="unitId">unit id</param>
        /// <returns>list of gsm to salary table response</returns>
        /// <response code="200">list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetGsmBySalaryTable")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllLevelsRequest))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetGsmBySalaryTable([Required] long tableId, long? unitId)
        {
            try
            {
                var response = await Mediator.Send(new GetGsmBySalaryTableRequest
                {
                    ProjectId = User.GetProjectId(),
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