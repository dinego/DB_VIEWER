using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Application.GetAllHoursBase;
using System;
using System.Threading.Tasks;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class HourBaseController : BaseController
    {
        /// <summary>
        /// HoursBase controller return a HoursBase response object
        /// </summary>
        /// <returns>GetHoursBaseResponse list of object response</returns>
        /// <response code="200">GetHoursBaseResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetHoursBase")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllHoursBaseResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetHoursBase()
        {
            try
            {
                var response = await Mediator.Send(new GetAllHourBaseRequest { UserId = User.GetUserId(), IsAdmin = User.IsAdmin(), CompanyId = User.GetCompanyId()});
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// validate if user has access in houry base section
        /// </summary>
        /// <returns>true or false</returns>
        /// <response code="200">true/false</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("CanAccessHoursBase")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> CanAccessHoursBase()
        {
            try
            {
                var response = await Mediator.Send(new CanAccessHoursBaseRequest { UserId = User.GetUserId() });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

    }
}