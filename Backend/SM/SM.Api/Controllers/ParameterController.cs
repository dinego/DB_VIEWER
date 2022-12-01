using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Api.ViewModel;
using SM.Application.Parameters.Command;
using SM.Application.Parameters.Queries;
using SM.Application.Parameters.Queries.Response;
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
    public class ParameterController : BaseController
    {

        /// <summary>
        /// Level Configuration
        /// </summary>
        /// <returns>GetLevelsConfigurationResponse list of object response with levels mapping</returns>
        /// <response code="200">GetLevelsConfigurationResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetLevels")]
        [ModuleAuthorize(ModulesEnum.Parameters, ModulesSuItemsEnum.Levels)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetLevelsConfigurationResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetLevelsConfiguration()
        {
            try
            {
                var response = await Mediator.Send(new GetLevelsConfigurationRequest
                {
                    UserId = User.GetUserId(),
                    CompanyId = User.GetCompanyId(),
                    IsAdmin = User.IsAdmin()
                });

                await LogUserAccess(LogActionsEnum.AccessParametersLevels.GetDescription(),
                    ModulesEnum.Parameters, ModulesSuItemsEnum.Levels);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        ///  Update/Add configuration for levels
        /// </summary>
        /// <returns>status</returns>
        /// <response code="200">Columns save with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpPut("SaveLevels")]
        [ModuleAuthorize(ModulesEnum.Parameters, ModulesSuItemsEnum.Levels)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UpdateLevelsConfiguration([FromBody] SaveLevelViewModel saveLevelViewModel)
        {
            try
            {
                var request = saveLevelViewModel.Map().ToANew<UpdateLevelsConfigurationRequest>();
                if (request == null)
                    return BadRequest(new { message = "Não encontramos os parâmetros de entrada para processar sua requisição." });

                request.UserId = User.GetUserId();
                request.CompanyId = User.GetCompanyId();
                request.IsAdmin = User.IsAdmin();
                var response = await Mediator.Send(request);

                await LogUserAccess(LogActionsEnum.AccessParametersSaveLevels.GetDescription(),
                    ModulesEnum.Parameters, ModulesSuItemsEnum.Levels);

                return Ok(new { message = "Níveis salvos com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        ///  Get all hourly basis 
        /// </summary>
        /// <returns>status</returns>
        /// <response code="200">return list of hourlyBasis</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetHourlyBasis")]
        [ModuleAuthorize(ModulesEnum.Parameters, ModulesSuItemsEnum.HoursBase)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetHourlyBasisResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetHourlyBasis()
        {
            try
            {
                var response = await Mediator.Send(new GetHourlyBasisRequest { IsAdmin = User.IsAdmin(), CompanyId = User.GetCompanyId(), UserId = User.GetUserId(), ProjectId = User.GetProjectId() });
                await LogUserAccess(LogActionsEnum.AccessParametersHoursBase.GetDescription(),
                    ModulesEnum.Parameters, ModulesSuItemsEnum.HoursBase);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        ///  Save hourly basis settings
        /// </summary>
        /// <returns>status</returns>
        /// <response code="200">return save hourlyBasis</response>
        /// <response code="400">Internal Error</response>
        [HttpPut("SaveHourlyBasis")]
        [ModuleAuthorize(ModulesEnum.Parameters, ModulesSuItemsEnum.HoursBase)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetHourlyBasisResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> SaveHourlyBasis([FromBody] SaveHourlyBasisViewModel viewModel)
        {
            try
            {
                var request = viewModel.Map().ToANew<SaveHourlyBasisRequest>();
                if (request != null)
                {
                    request.IsAdmin = User.IsAdmin();
                    request.CompanyId = User.GetCompanyId();
                    request.UserId = User.GetUserId();
                    request.ProjectId = User.GetProjectId();
                }
                var response = await Mediator.Send(request);
                await LogUserAccess(LogActionsEnum.AccessParametersSaveHoursBase.GetDescription(),
                    ModulesEnum.Parameters, ModulesSuItemsEnum.HoursBase);

                return Ok(new { message = "Base Horária foi salvo com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Salary Strategy
        /// </summary>
        /// <param name="tableId">tableId is required</param>
        /// <param name="sortColumnId"></param>
        /// <param name="isAsc">isAsc</param>
        /// <returns>GetSalaryStrategyResponse list of object</returns>
        /// <response code="200">GetSalaryStrategyResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetSalaryStrategy")]
        [ModuleAuthorize(ModulesEnum.Parameters, ModulesSuItemsEnum.SalaryStrategy)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSalaryStrategyResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetSalaryStrategy(long tableId,
             int? sortColumnId = null,
             bool? isAsc = null)
        {
            try
            {
                var response = await Mediator.Send(new GetSalaryStrategyRequest
                {
                    TableId = tableId,
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies(),
                    IsAdmin = User.IsAdmin(),
                    ProjectId = User.GetProjectId(),
                    IsAsc = isAsc,
                    SortColumnId = sortColumnId
                });
                await LogUserAccess(LogActionsEnum.AccessParametersSalaryStrategy.GetDescription(),
                    ModulesEnum.Parameters, ModulesSuItemsEnum.SalaryStrategy);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        ///  Update pj configuration
        /// </summary>
        /// <returns>status</returns>
        /// <response code="200">Columns save with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpPut("UpdateSalaryStrategy")]
        [ModuleAuthorize(ModulesEnum.Parameters, ModulesSuItemsEnum.SalaryStrategy)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UpdateSalaryStrategy([FromBody] UpdateSalaryStrategyViewModel viewModel)
        {
            try
            {
                var request = viewModel.Map().ToANew<UpdateSalaryStrategyRequest>();
                if (request == null)
                    return BadRequest(new { message = "Não encontramos os parâmetros de entrada para processar sua requisição." });

                request.IsAdmin = User.IsAdmin();
                request.ProjectId = User.GetProjectId();

                var response = await Mediator.Send(request);
                await LogUserAccess(LogActionsEnum.UpdateParametersSalaryStrategy.GetDescription(),
                    ModulesEnum.Parameters, ModulesSuItemsEnum.SettingsPJ);
                return Ok(new { message = "Estratégia Salarial foi salvo com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// PJ Settings
        /// </summary>
        /// <param name="contractTypeId">contractTypeId</param>
        /// <returns>GetPJSettingsResponse list of object</returns>
        /// <response code="200">GetPJSettingsResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetPJSettings")]
        [ModuleAuthorize(ModulesEnum.Parameters, ModulesSuItemsEnum.SettingsPJ)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetPJSettingsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetPJSettings(long contractTypeId)
        {
            try
            {
                var response = await Mediator.Send(new GetPJSettingsRequest
                {
                    ContractTypeId = contractTypeId,
                    IsAdmin = User.IsAdmin(),
                    ProjectId = User.GetProjectId(),
                    UserId = User.GetUserId()
                });
                await LogUserAccess(LogActionsEnum.AccessParametersPJSettings.GetDescription(),
                    ModulesEnum.Parameters, ModulesSuItemsEnum.SettingsPJ);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        ///  Update pj configuration
        /// </summary>
        /// <returns>status</returns>
        /// <response code="200">Columns save with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpPut("UpdateSettingsPj")]
        [ModuleAuthorize(ModulesEnum.Parameters, ModulesSuItemsEnum.SettingsPJ)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UpdateSettingsPj([FromBody] UpdateSettingsPjViewModel updateSettingsPjViewModel)
        {
            try
            {
                var request = updateSettingsPjViewModel.Map().ToANew<UpdateSettingsPjRequest>();
                if (request == null)
                    return BadRequest(new { message = "Não encontramos os parâmetros de entrada para processar sua requisição." });

                request.UserId = User.GetUserId();
                request.IsAdmin = User.IsAdmin();
                request.ProjectId = User.GetProjectId();

                var response = await Mediator.Send(request);
                await LogUserAccess(LogActionsEnum.AccessParametersSavePJSettings.GetDescription(),
                    ModulesEnum.Parameters, ModulesSuItemsEnum.SettingsPJ);
                return Ok(new { message = "Configurações PJ foi salvo com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        ///  Update/Add global labels
        /// </summary>
        /// <param name="viewModels">List of global labels</param>
        /// <returns>status</returns>
        /// <response code="200">Columns save with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpPut("UpdateGlobalLabels")]
        [ModuleAuthorize(ModulesEnum.Parameters, ModulesSuItemsEnum.GlobalLabels)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UpdateGlobalLabels([FromBody] List<GlobalLabelViewModel> viewModels)
        {
            try
            {
                var response = await Mediator.Send(new UpdateGlobalLabelsRequest
                {
                    UserId = User.GetUserId(),
                    ProjectId = User.GetProjectId(),
                    IsAdmin = User.IsAdmin(),
                    GlobalLabels = viewModels.Map().ToANew<List<GlobalLabelRequest>>()
                });
                await LogUserAccess(LogActionsEnum.SaveParameterGlobalLabels.GetDescription(),
                    ModulesEnum.Parameters, ModulesSuItemsEnum.GlobalLabels);

                return Ok(new { message = "Preferências foram salvas com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get Display Configuration return a response object with preferences and display items
        /// </summary>
        /// <returns>return display configuration object</returns>
        /// <response code="200">returned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetDisplayConfiguration")]
        [ModuleAuthorize(ModulesEnum.Parameters, ModulesSuItemsEnum.None)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDisplayConfigurationResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetDisplayConfiguration()
        {
            try
            {
                var response = await Mediator.Send(new GetDisplayConfigurationRequest
                {
                    UserId = User.GetUserId(),
                    ProjectId = User.GetProjectId(),
                    IsAdmin = User.IsAdmin()
                });

                await LogUserAccess(LogActionsEnum.AccessDisplayConfiguration.GetDescription(), ModulesEnum.Parameters);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        ///  Update/Add global labels
        /// </summary>
        /// <param name="viewModels"></param>
        /// <returns>status</returns>
        /// <response code="200">Columns save with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpPut("UpdateDisplayConfiguration")]
        [ModuleAuthorize(ModulesEnum.Parameters, ModulesSuItemsEnum.DisplayConfiguration)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> UpdateDisplayConfiguration([FromBody] List<DisplayConfigurationViewModel> viewModels)
        {
            try
            {
                var response = await Mediator.Send(new UpdateDisplayConfigurationRequest
                {
                    ProjectId = User.GetProjectId(),
                    IsAdmin = User.IsAdmin(),
                    UserId = User.GetUserId(),
                    DisplayConfiguration = viewModels.Map().ToANew<List<DisplayConfigurationData>>()
                });
                await LogUserAccess(LogActionsEnum.SaveDisplayConfiguration.GetDescription(),
                    ModulesEnum.Parameters, ModulesSuItemsEnum.DisplayConfiguration);

                return Ok(new { message = "Configurações de Exibição foram salvas com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }
    }
}