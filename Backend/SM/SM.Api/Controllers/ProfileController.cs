using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Application.GetAllProfiles;
using System;
using System.Threading.Tasks;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class ProfileController : BaseController
    {
        /// <summary>
        /// Profile controller return a Profile response object
        /// </summary>
        /// <param name="unitId">unitId</param>
        /// <param name="tableId">tableId</param>
        /// <returns>GetAllProfilesResponse list of object response</returns>
        /// <response code="200">GetAllProfilesResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetAllProfiles")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllProfilesResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetAllProfiles(long? tableId, long? unitId)
        {
            try
            {
                var response = await Mediator.Send(new GetAllProfilesRequest {
                    UserId = User.GetUserId(),
                    Units = User.GetUserCompanies().ToArray(),
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