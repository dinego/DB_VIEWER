using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Api.ViewModel;
using SM.Application.UserParameters.Command;
using SM.Application.UserParameters.Queries;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    [ModuleAuthorize(ModulesEnum.Parameters, ModulesSuItemsEnum.Users)]
    public class UserParameterController : BaseController
    {
        /// <summary>
        /// Get all users by company
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        /// <response code="200">GetLevelsConfigurationResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetUsersResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(int page)
        {
            try
            {
                var result = await Mediator.Send(new GetUsersRequest
                {
                    UserCompanies = User.GetUserCompanies(),
                    Page = page,
                    UserId = User.GetUserId()
                });
                await LogUserAccess(LogActionsEnum.GetAllUserParameters.GetDescription(),
                                    ModulesEnum.Parameters,ModulesSuItemsEnum.Users);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get user permissions
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <response code="200">GetUserInfoPermissionsResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserInfoPermissionsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        [HttpGet("GetUserInformation")]
        public async Task<IActionResult> GetUserInformation(long userId)
        {
            try
            {
                var result = await Mediator.Send(new GetUserInfoPermissionsRequest
                {
                    UserId = userId,
                    IsAdmin = User.IsAdmin(),
                    CompanyId = User.GetCompanyId(),
                    AdminId = User.GetUserId()
                });
                await LogUserAccess(LogActionsEnum.GetUserParameters.GetDescription(),
                    ModulesEnum.Parameters, ModulesSuItemsEnum.Users);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Save users permissions
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        /// <response code="200">User permissions save with successfully</response>
        /// <response code="400">Internal Error</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        [HttpPost("SaveUserInformation")]
        public async Task<IActionResult> SaveUserInformation([FromBody] GetUserInfoPermissionsResponse userInfo)
        {
            try
            {
                var request = userInfo.Map().ToANew<SaveUserInformationRequest>();
                if (request == null)
                   return BadRequest(new { message = "Não encontramos os parâmetros de entrada para processar sua requisição." });

                request.IsAdmin = User.IsAdmin();

                var result = await Mediator.Send(request);
                await LogUserAccess(LogActionsEnum.SaveUserParameters.GetDescription(),
                    ModulesEnum.Parameters, ModulesSuItemsEnum.Users);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }
        /// <summary>
        /// Change user status
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <response code="200">User status change with successfully</response>
        /// <response code="400">Internal Error</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        [HttpPut("ChangeStatusUser")]
        public async Task<IActionResult> ChangeStatusUser([FromBody] ChangeStatusUserViewModel user)
        {
            try
            {
                var request = user.Map().ToANew<ChangeStatusUserRequest>();
                if (request == null)
                    return BadRequest(new { message = "Não encontramos os parâmetros de entrada para processar sua requisição." });

                request.IsAdmin = User.IsAdmin();

                var result = await Mediator.Send(request);
                await LogUserAccess(LogActionsEnum.ChangeStatusUserParameters.GetDescription(),
                    ModulesEnum.Parameters, ModulesSuItemsEnum.Users);
                return Ok(result);
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
        [HttpGet("CanAccessUsers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> CanAccessUsers()
        {
            try
            {
                var response = await Mediator.Send(new CanAccessUsersRequest { UserId = User.GetUserId(), UserCompanies = User.GetUserCompanies() });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }
    }
}
