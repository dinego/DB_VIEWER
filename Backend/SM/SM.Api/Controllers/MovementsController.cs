using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Application.Movements.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class MovementsController : BaseController
    {
        /// <summary>
        /// Movements controller return a list of movements response object
        /// </summary>
        /// <returns>GetMovementsResponse list of object response</returns>
        /// <response code="200">GetMovements list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetMovements")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetMovementsResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetMovements()
        {
            try
            {
                var response = await Mediator.Send(new GetMovementsRequest { UserId = User.GetUserId(), Companies = User.GetUserCompanies()});
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

      
    }
}