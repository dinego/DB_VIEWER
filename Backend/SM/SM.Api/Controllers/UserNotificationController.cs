using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Api.ViewModel;
using SM.Application.ContactUs;
using SM.Application.GetAllNotification;
using SM.Application.GetAllNotRead;
using SM.Application.RemoveNotificationById;
using SM.Application.SetProfileImage;
using SM.Application.SetReadNotification;
using SM.Application.UserNotification.Command;
using SM.Domain.Enum;
using System;
using System.Threading.Tasks;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    [ModuleAuthorize(ModulesEnum.Home, ModulesSuItemsEnum.None)]
    public class UserNotificationController : BaseController
    {
        /// <summary>
        /// User Notification controller return a Notifications response object
        /// </summary>
        /// <param name="page">number of page</param>
        /// <param name="pageSize">lines per page</param>
        /// <returns>GetAllNotificationResponse list object response</returns>
        /// <response code="200">GetAllNotificationResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetAllNotification")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllNotificationResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetAllNotification(int page = 1, int pageSize = 20)
        {
            try
            {
                var response = await Mediator.Send(new GetAllNotificationRequest { UserId = User.GetUserId(), Page = page, PageSize = pageSize });

                await LogUserAccess(LogActionsEnum.AccessNotification.GetDescription(),
                    ModulesEnum.Home);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// User Notification controller return a all notification not read response object
        /// </summary>
        /// <returns>User Notification list object response not read</returns>
        /// <response code="200">GetAllNotReadResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetAllNotificationNotRead")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllNotReadResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetAllNotificationNotRead()
        {
            try
            {
                var response = await Mediator.Send(new GetAllNotReadRequest { UserId = User.GetUserId() });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// User Notification controller not return any value
        /// </summary>
        /// <returns>User Notification set to true notification read</returns>
        /// <response code="200">SetReadNotificationResponse not return value</response>
        /// <response code="400">Internal Error</response>
        [HttpPut("SetReadNotification")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> SetReadNotification([FromBody] SetReadNotificationViewModel viewModel)
        {
            try
            {
                var request = viewModel.Map().ToANew<SetReadNotificationRequest>();
                if (request == null)
                    return BadRequest(new { message = "Não encontramos os parâmetros de entrada para processar sua requisição." });

                request.UserId = User.GetUserId();
                var response = await Mediator.Send(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// User Notification controller not return any value
        /// </summary>
        /// <returns>User Notification remove notification</returns>
        /// <response code="200">RemoveNotificationById not return value</response>
        /// <response code="400">Internal Error</response>
        [HttpDelete("RemoveNotificationById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> RemoveNotificationById(long notificationId)
        {
            try
            {
                var response = await Mediator.Send(new RemoveNotificationByIdRequest { NotificationId = notificationId, UserId = User.GetUserId() });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// User Notification controller not return any value
        /// </summary>
        /// <returns>ContactUs send email</returns>
        /// <response code="200">ContactUs not return value</response>
        /// <response code="400">Internal Error</response>
        [HttpPut("ContactUs")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> ContactUs(ContactUsRequestViewModel viewModel)
        {
            try
            {

                var response = await Mediator.Send(new ContactUsRequest
                {
                    Attachment = viewModel.Attachment,
                    Message = viewModel.Message,
                    Subject = viewModel.Subject,
                    UserId = User.GetUserId()
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// User Notification controller not return any value
        /// </summary>
        /// <returns>UploadPhoto set image profile</returns>
        /// <response code="200">UploadPhoto not return value</response>
        /// <response code="400">Internal Error</response>
        [HttpPut("UploadPhoto")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UploadPhoto([FromForm] IFormFile attachment)
        {
            try
            {
                var response = await Mediator.Send(new SetProfileImageRequest { UserId = User.GetUserId(), Attachment = attachment });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// User Notification controller not return any value
        /// </summary>
        /// <returns>RemovePhoto remove image profile</returns>
        /// <response code="200">RemovePhoto not return value</response>
        /// <response code="400">Internal Error</response>
        [HttpPut("RemovePhoto")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(void))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> RemovePhoto()
        {
            try
            {
                var response = await Mediator.Send(new SetProfileImageRequest { UserId = User.GetUserId() });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// User Notification controller not return any value
        /// </summary>
        /// <returns>ContactUs send email</returns>
        /// <response code="200">ContactUs not return value</response>
        /// <response code="400">Internal Error</response>
        [HttpPut("SendLinkAccess")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> SendLinkAccess(SendLinkViewModel viewModel)
        {
            try
            {
                var response = await Mediator.Send(new SendLinkRequest
                {
                    To = viewModel.To,
                    Message = viewModel.Message,
                    Url = viewModel.Url,
                    UserId = User.GetUserId()
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