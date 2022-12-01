using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Application.Units.Queries;
using System;
using System.Threading.Tasks;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class UnitsController : BaseController
    {
        /// <summary>
        /// Units controller return a companies/Units response object
        /// </summary>
        /// <returns>GetUnitsByUserResponse list of object response</returns>
        /// <response code="200">GetUnitsByUserResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetUnitsByUser")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUnitsByUserResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetUnitsByUser()
        {
            try
            {
                var response = await Mediator.Send(new GetUnitsByUserRequest { CompanyIds = User.GetUserCompanies() });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get units by filter
        /// </summary>
        /// <returns>GetUnitsByFilterResponse list of object response</returns>
        /// <response code="200">GetUnitsByFilterResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetUnitsByFilter")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUnitsByFilterResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetUnitsByFilter(long? tableId, long? groupId)
        {
            try
            {
                var response = await Mediator.Send(new GetUnitsByFilterRequest
                {
                    CompanyIds = User.GetUserCompanies(),
                    ProjectId = User.GetProjectId(),
                    TableId = tableId,
                    GroupId = groupId
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