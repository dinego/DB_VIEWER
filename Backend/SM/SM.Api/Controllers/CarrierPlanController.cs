using System;
using System.Threading.Tasks;
using CMC.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Api.ViewModel;
using SM.Application.CareerPlan.Queries;
using SM.Application.CareerPlan.Queries.Response;
using SM.Domain.Enum;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    public class CareerPlanController : BaseController
    {
        /// <summary>
        /// Get carrier axis of position
        /// </summary>
        /// <returns>position carrier axis</returns>
        /// <response code="200">Retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetCareerAxis")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CareerPlanResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetCareerAxis(long positionId, ModulesEnum moduleId)
        {
            try
            {
                var response = await Mediator.Send(new CareerPlanRequest
                {
                    ProjectId = User.GetProjectId(),
                    PositionId = positionId
                });
                await LogUserAccess(LogActionsEnum.AccessCareerPlanPosition.GetDescription(), moduleId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        ///  Update carrier plan of position
        /// </summary>
        /// <param name="viewModel">parameters to edit carrier plan</param>
        /// <returns>status</returns>
        /// <response code="200">Columns save with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpPut("UpdateCareerPlan")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UpdateCareerPlan([FromBody] CareerPlanViewModel viewModel)
        {
            try
            {
                // var response = await Mediator.Send(new UpdatePositionDetailRequest
                // {
                //     ProjectId = User.GetProjectId(),
                //     CanEditListPosition = User.CanEditListPosition(),
                //     PositionData = viewModel.Map().ToANew<UpdatePositionDetailData>()
                // });
                await LogUserAccess(LogActionsEnum.SaveCareerPlanPosition.GetDescription(), viewModel.ModuleId);

                return Ok(new { message = "Detalhes do cargo foram salvos com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }
    }
}