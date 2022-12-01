using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Application.GetAllContractTypes;
using System;
using System.Threading.Tasks;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class ContractTypesController : BaseController
    {
        /// <summary>
        /// ContractTypes controller return a ContractTypes response object
        /// </summary>
        /// <returns>GetAllContractTypessResponse list of object response</returns>
        /// <response code="200">GetAllContractTypessResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetAllContractTypes")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllContractTypesResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetAllContractTypes()
        {
            try
            {
                var response = await Mediator.Send(new GetAllContractTypesRequest { UserId = User.GetUserId()});
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get contract types to pj settings
        /// </summary>
        /// <returns>GetAllContractTypessResponse list of object response</returns>
        /// <response code="200">GetContractTypesPjSettingsResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetContractTypesPjSettings")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetContractTypesPjSettingsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetContractTypesPjSettings()
        {
            try
            {
                var response = await Mediator.Send(new GetContractTypesPjSettingsRequest { UserId = User.GetUserId() });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }
    }
}