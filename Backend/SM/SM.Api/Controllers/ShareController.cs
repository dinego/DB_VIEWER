using AgileObjects.AgileMapper.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Api.ViewModel;
using SM.Application.Helpers;
using SM.Application.Position.Querie;
using SM.Application.Position.Queries;
using SM.Application.Position.Queries.Response;
using SM.Application.Positioning.Queries;
using SM.Application.Share.Command;
using SM.Application.Share.Queries;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ShareController : BaseController
    {
        /// <summary>
        ///     Generate Key for share
        /// </summary>
        /// <param name="generateKeyRequest">generateKeyRequest</param>
        /// <returns>status</returns>
        /// <response code="200">Key retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [Authorize]
        [ModuleAuthorize(ModulesEnum.None, ModulesSuItemsEnum.None)]
        [HttpPost("GenerateKeySave")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GenerateKeySave([FromBody] GenerateKeySaveViewModel generateKeyRequest)
        {
            try
            {
                var request = generateKeyRequest.Map().ToANew<GenerateKeySaveRequest>();
                request.Parameters = generateKeyRequest.Parameters;
                if (request == null)
                    return BadRequest(new { message = "Não encontramos os parâmetros de entrada para processar sua requisição." });
                request.UserId = User.GetUserId();
                request.CompaniesId = User.GetUserCompanies();
                request.ProjectId = User.GetProjectId();
                var generateKey = await Mediator.Send(request);
                await LogShareAccess(generateKey);
                return Ok(generateKey);
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
        [Authorize]
        [ModuleAuthorize(ModulesEnum.None, ModulesSuItemsEnum.None)]
        [HttpPost("ShareLink")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DefaultResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> ShareLink(ShareLinkByEmailViewModel viewModel)
        {
            try
            {
                var request = viewModel.Map().ToANew<ShareLinkByEmailRequest>();
                var response = await Mediator.Send(new ShareLinkByEmailRequest
                {
                    UserId = User.GetUserId(),
                    To = request.To,
                    URL = request.URL
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get map position to share
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <param name="showJustWithOccupants"></param>
        /// <param name="removeRowsEmpty"></param>
        /// <param name="page">page</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="isAsc">isAsc</param>
        /// <returns>Table map postion and share data of object response</returns>
        /// <response code="200">GetMapPositionResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetMapPosition")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMapPositionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetMapPosition([Required] string secretKey,
            bool showJustWithOccupants,
            bool removeRowsEmpty,
            int page = 1,
            int pageSize = 20,
            bool? isAsc = null)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var result = JsonConvert.DeserializeObject<ShareMapPositionViewModel>(shareResult.Parameters);
                result.Columns = shareResult.ColumnsExcluded?
                    .Select(s => s.ToString());
                result.UserId = shareResult.UserId;
                result.ProjectId = shareResult.ProjectId;
                result.CompaniesId = shareResult.CompaniesId;
                var request = result.Map().ToANew<GetMapPositionRequest>();
                request.Page = page;
                request.PageSize = pageSize;
                request.RemoveRowsEmpty = removeRowsEmpty;
                request.ShowJustWithOccupants = showJustWithOccupants;
                request.IsAsc = isAsc;
                request.Share = true;

                var response = await Mediator.Send(request);

                //fix label filters
                var labels = await Mediator.Send(new GetValueFiltersRequest
                {
                    Object = request,
                    ProjectId = shareResult.ProjectId
                });

                labels.User = shareResult.UserShared;
                labels.Date = shareResult.DateShared;

                response.Share = labels.Map().ToANew<ShareMapPositionResponse>();
                response.Share.Permissions = result.Permissions.Map().ToANew<PermissionShared>();

                await LogShareAccess(secretKey);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get list position to share
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <param name="page">page</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="showJustWithOccupants"></param>
        /// <param name="sortColumnId">sortColumnId</param>
        /// <param name="isAsc">isAsc</param>
        /// <returns>Table list position and share data of object response</returns>
        /// <response code="200">GetListPositionResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetAllPositions")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAllPositionsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetAllPositions([Required] string secretKey,
            int page = 1,
            int pageSize = 20,
            bool showJustWithOccupants = false,
            int? sortColumnId = null,
            bool? isAsc = null)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var result = JsonConvert.DeserializeObject<ShareParametersViewModel>(shareResult.Parameters);
                result.ColumnsExcluded = shareResult.ColumnsExcluded?
                    .Select(s => Convert.ToInt32(s));
                result.UserId = shareResult.UserId;
                result.ProjectId = shareResult.ProjectId;
                result.CompaniesId = shareResult.CompaniesId;
                var request = result.Map().ToANew<GetAllPositionsRequest>();
                request.Page = page;
                request.PageSize = pageSize;
                request.IsAsc = isAsc;
                request.SortColumnId = sortColumnId;
                request.ShowJustWithOccupants = showJustWithOccupants;
                request.Share = true;

                var response = await Mediator.Send(request);

                //fix label filters
                var labels = await Mediator.Send(new GetValueFiltersRequest
                {
                    Object = request,
                    ProjectId = shareResult.ProjectId
                });

                labels.User = shareResult.UserShared;
                labels.Date = shareResult.DateShared;

                response.Share = labels.Map().ToANew<ShareAllPositionsResponse>();
                response.Share.Permissions = result.Permissions.Map().ToANew<PermissionShared>();
                await LogShareAccess(secretKey);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get full info list position to share (dialog)
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <param name="positionSmIdLocal"></param>
        /// <returns>Table list position and share data of object response</returns>
        /// <response code="200">GetFullInfoPositionResponse retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFullInfoPosition")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFullInfoPositionResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFullInfoPosition([Required] string secretKey,
            [Required] long positionSmIdLocal)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var result = JsonConvert.DeserializeObject<ShareParametersViewModel>(shareResult.Parameters);
                result.UserId = shareResult.UserId;
                result.ProjectId = shareResult.ProjectId;
                result.CompaniesId = shareResult.CompaniesId;

                var request = result.Map().ToANew<GetFullInfoPositionRequest>();
                request.PositionSmIdLocal = positionSmIdLocal;

                var response = await Mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get framework from positioning to share
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <param name="isMM">isMM</param>
        /// <param name="isMI">isMI</param>
        /// <param name="page">page</param>
        /// <param name="contractType"></param>
        /// <param name="hoursType"></param>
        /// <param name="unitId"></param>
        /// <param name="isAsc">isAsc</param>
        /// <param name="sortColumnId">sortColumnId</param>
        /// <returns>Table framework from positioning and share data of object response</returns>
        /// <response code="200">GetFrameworkResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFramework")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFrameworkResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFramework([Required] string secretKey,
            bool? isMM,
            bool? isMI,
            int? page = 1,
            ContractTypeEnum? contractType = null,
            DataBaseSalaryEnum? hoursType = null,
            long? unitId = null,
            bool? isAsc = null,
            int? sortColumnId = null)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var result = JsonConvert.DeserializeObject<ShareParametersViewModel>(shareResult.Parameters);
                result.ColumnsExcluded = shareResult.ColumnsExcluded?
                    .Select(s => Convert.ToInt32(s));
                result.UserId = shareResult.UserId;
                result.ProjectId = shareResult.ProjectId;
                result.CompaniesId = shareResult.CompaniesId;
                var request = result.Map().ToANew<GetFrameworkRequest>();
                request.IsMI = isMI.HasValue ? isMI.Value : request.IsMI;
                request.IsMM = isMM.HasValue ? isMM.Value : request.IsMM;
                request.Page = page.GetValueOrDefault(1);
                request.IsAsc = isAsc.HasValue ? isAsc.Value : request.IsAsc;
                request.SortColumnId = sortColumnId.HasValue ? sortColumnId.Value : request.SortColumnId;
                request.UnitId = unitId.HasValue ? unitId.Value : request.UnitId;
                request.ContractType = contractType.HasValue ? contractType.Value : request.ContractType;
                request.HoursType = hoursType.HasValue ? hoursType.Value : request.HoursType;
                request.Share = true;
                request.PageSize = 20;


                var response = await Mediator.Send(request);
                //fix label filters
                var labels = await Mediator.Send(new GetValueFiltersRequest
                {
                    Object = request,
                    ProjectId = shareResult.ProjectId
                });

                labels.User = shareResult.UserShared;
                labels.Date = shareResult.DateShared;

                response.Share = labels.Map().ToANew<ShareFrameworkResponse>();
                response.Share.ScenarioLabel = result.ScenarioLabel;
                response.Share.Permissions = result.Permissions.Map().ToANew<PermissionShared>();
                await LogShareAccess(secretKey);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get full info framework from positioning to share (dialog)
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <param name="salaryBaseId">salaryBaseId</param>
        /// <returns>Table framework from positioning and share data of object response</returns>
        /// <response code="200">GetFullInfoFrameworkResponse retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFullInfoFramework")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFullInfoFrameworkResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFullInfoFramework([Required] string secretKey,
           [Required] long salaryBaseId)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var result = JsonConvert.DeserializeObject<ShareParametersViewModel>(shareResult.Parameters);

                result.UserId = shareResult.UserId;
                result.CompaniesId = shareResult.CompaniesId;
                result.ProjectId = shareResult.ProjectId;

                var request = result.Map().ToANew<GetFullInfoFrameworkRequest>();
                request.SalaryBaseId = salaryBaseId;

                var response = await Mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get financial Impact from positioning to share
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <returns>Financial Impact share data of object response</returns>
        /// <response code="200">GetFinancialImpactResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFinancialImpact")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetResultFinancialImpactResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFinancialImpact([Required] string secretKey)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var result = JsonConvert.DeserializeObject<ShareParametersViewModel>(shareResult.Parameters);
                result.CategoriesExp = shareResult.ColumnsExcluded;

                result.UserId = shareResult.UserId;
                result.ProjectId = shareResult.ProjectId;
                result.CompaniesId = shareResult.CompaniesId;
                var request = result.Map().ToANew<GetFinancialImpactRequest>();

                var response = await Mediator.Send(request);
                //fix label filters
                var labels = await Mediator.Send(new GetValueFiltersRequest
                {
                    Object = request,
                    ProjectId = shareResult.ProjectId
                });

                labels.User = shareResult.UserShared;
                labels.Date = shareResult.DateShared;

                response.Share = labels.Map().ToANew<ShareResultFinancialImpactResponse>();
                response.Share.Permissions = result.Permissions.Map().ToANew<PermissionShared>();
                await LogShareAccess(secretKey);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get full info financial Impact from positioning to share
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <param name="categoryId">categoryId</param>
        /// <param name="serieId">serieId</param>
        /// <param name="isAsc">isAsc</param>
        /// <param name="sortColumnId">sortColumnId</param>
        /// <returns>Financial Impact share data of object response</returns>
        /// <response code="200">GetFullInfoFinancialImpactResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFullInfoFinancialImpact")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFullInfoFinancialImpactResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFullInfoFinancialImpact([Required] string secretKey,
            [Required] string categoryId,
            [Required] AnalyseFinancialImpactEnum serieId,
            bool? isAsc = null,
            int? sortColumnId = null)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var result = JsonConvert.DeserializeObject<ShareParametersViewModel>(shareResult.Parameters);

                result.UserId = shareResult.UserId;
                result.ProjectId = shareResult.ProjectId;
                result.CompaniesId = shareResult.CompaniesId;


                var request = result.Map().ToANew<GetFullInfoFinancialImpactRequest>();
                request.CategoryId = ConverterObject.TryConvertToInt(categoryId);
                request.SerieId = serieId;
                request.IsAsc = isAsc;
                request.SortColumnId = sortColumnId;

                var response = await Mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get Proposed Movements from positioning to share
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <returns>Proposed Movements share data of object response</returns>
        /// <response code="200">GetFinancialImpactResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetProposedMovements")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProposedMovementsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetProposedMovements([Required] string secretKey)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var result = JsonConvert.DeserializeObject<ShareParametersViewModel>(shareResult.Parameters);

                result.CategoriesExp = shareResult.ColumnsExcluded?
                    .Select(s => s);

                result.UserId = shareResult.UserId;
                result.ProjectId = shareResult.ProjectId;
                result.CompaniesId = shareResult.CompaniesId;
                var request = result.Map().ToANew<GetProposedMovementsRequest>();
                var response = await Mediator.Send(request);
                //fix label filters
                var labels = await Mediator.Send(new GetValueFiltersRequest
                {
                    Object = request,
                    ProjectId = shareResult.ProjectId
                });

                labels.User = shareResult.UserShared;
                labels.Date = shareResult.DateShared;

                response.Share = labels.Map().ToANew<ShareProposedMovementsResponse>();
                response.Share.Permissions = result.Permissions.Map().ToANew<PermissionShared>();
                await LogShareAccess(secretKey);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get full info Proposed Movements from positioning to share
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <param name="categoryId"></param>
        /// <param name="serieId">serieId</param>
        /// <param name="page">page</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="isAsc">isAsc</param>
        /// <param name="sortColumnId">sortColumnId</param>
        /// <returns>Full info Proposed Movements share data of object response</returns>
        /// <response code="200">GetFullInfoProposedMovementsResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFullInfoProposedMovements")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFullInfoProposedMovementsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFullInfoProposedMovements([Required] string secretKey,
            [Required] string categoryId,
            [Required] ProposedMovementsEnum serieId,
            int page = 1,
            int pageSize = 20,
            bool? isAsc = null,
            int? sortColumnId = null)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var result = JsonConvert.DeserializeObject<ShareParametersViewModel>(shareResult.Parameters);

                result.UserId = shareResult.UserId;
                result.ProjectId = shareResult.ProjectId;
                result.CompaniesId = shareResult.CompaniesId;
                var request = result.Map().ToANew<GetFullInfoProposedMovementsRequest>();
                request.Page = page;
                request.PageSize = pageSize;
                request.CategoryId = ConverterObject.TryConvertToInt(categoryId);
                request.SerieId = serieId;
                request.IsAsc = isAsc;
                request.SortColumnId = sortColumnId;


                var response = await Mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get DistributionA nalysis from positioning to share
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <returns>Distribution Analysis share data of object response</returns>
        /// <response code="200">GetDistributionAnalysisResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetDistributionAnalysis")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDistributionAnalysisResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetDistributionAnalysis([Required] string secretKey)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var result = JsonConvert.DeserializeObject<ShareParametersViewModel>(shareResult.Parameters);

                result.CategoriesExp = shareResult.ColumnsExcluded?
                    .Select(s => s);

                result.UserId = shareResult.UserId;
                result.ProjectId = shareResult.ProjectId;
                result.CompaniesId = shareResult.CompaniesId;
                var request = result.Map().ToANew<GetDistributionAnalysisRequest>();
                var response = await Mediator.Send(request);
                //fix label filters
                var labels = await Mediator.Send(new GetValueFiltersRequest
                {
                    Object = request,
                    ProjectId = shareResult.ProjectId
                });

                labels.User = shareResult.UserShared;
                labels.Date = shareResult.DateShared;

                response.Share = labels.Map().ToANew<ShareDistributionAnalysisResponse>();
                response.Share.Permissions = result.Permissions.Map().ToANew<PermissionShared>();
                await LogShareAccess(secretKey);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get Comparative AnalysisChart from positioning to share
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <returns>Comparative AnalysisChart share data of object response</returns>
        /// <response code="200">GetComparativeAnalysisChartResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetComparativeAnalysisChart")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetComparativeAnalysisChartResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetComparativeAnalysisChart([Required] string secretKey)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var result = JsonConvert.DeserializeObject<ShareParametersViewModel>(shareResult.Parameters);

                result.CategoriesExp = shareResult.ColumnsExcluded?
                    .Select(s => s);

                result.UserId = shareResult.UserId;
                result.ProjectId = shareResult.ProjectId;
                result.CompaniesId = shareResult.CompaniesId;
                var request = result.Map().ToANew<GetComparativeAnalysisChartRequest>();
                var response = await Mediator.Send(request);
                //fix label filters
                var labels = await Mediator.Send(new GetValueFiltersRequest
                {
                    Object = request,
                    ProjectId = shareResult.ProjectId
                });

                labels.User = shareResult.UserShared;
                labels.Date = shareResult.DateShared;

                response.Share = labels.Map().ToANew<ShareResultComparativeAnalysisChartResponse>();
                response.Share.Permissions = result.Permissions.Map().ToANew<PermissionShared>();
                await LogShareAccess(secretKey);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get Full info Comparative AnalysisChart from positioning to share
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <param name="categoryId">categoryId</param>
        /// <param name="page">page</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="isAsc">isAsc</param>
        /// <param name="sortColumnId">sortColumnId</param>
        /// <param name="careerAxis">careerAxis</param>
        /// <returns>Full info Comparative AnalysisChart share data of object response</returns>
        /// <response code="200">GetFullInfoComparativeAnalysisResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetFullInfoComparativeAnalysis")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFullInfoComparativeAnalysisResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetFullInfoComparativeAnalysis([Required] string secretKey,
            [Required] string categoryId,
            int page = 1,
            int pageSize = 20,
            bool? isAsc = null,
            int? sortColumnId = null,
            string careerAxis = null)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var result = JsonConvert.DeserializeObject<ShareParametersViewModel>(shareResult.Parameters);

                result.UserId = shareResult.UserId;
                result.ProjectId = shareResult.ProjectId;
                result.CompaniesId = shareResult.CompaniesId;
                var request = result.Map().ToANew<GetFullInfoComparativeAnalysisRequest>();
                request.Page = page;
                request.PageSize = pageSize;
                request.CategoryId = ConverterObject.TryConvertToInt(categoryId);
                request.IsAsc = isAsc;
                request.SortColumnId = sortColumnId;
                request.CareerAxis = careerAxis;
                var response = await Mediator.Send(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get Comparative AnalysisTable from positioning to share
        /// </summary>
        /// <param name="secretKey">secretKey</param>
        /// <returns>Comparative AnalysisTable share data of object response</returns>
        /// <response code="200">GetComparativeAnalysisTableResponse list retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpGet("GetComparativeAnalysisTable")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetComparativeAnalysisTableResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> GetComparativeAnalysisTable([Required] string secretKey)
        {
            try
            {
                var shareResult = await Mediator.Send(new GetShareRequest { SecretKey = secretKey });
                if (shareResult == null)
                    return NotFound(new { message = "Nenhum resultado encontrado" });

                var result = JsonConvert.DeserializeObject<ShareParametersViewModel>(shareResult.Parameters);
                result.CategoriesExp = shareResult.ColumnsExcluded?
                    .Select(s => s);

                result.UserId = shareResult.UserId;
                result.ProjectId = shareResult.ProjectId;
                result.CompaniesId = shareResult.CompaniesId;
                var request = result.Map().ToANew<GetComparativeAnalysisTableRequest>();

                var response = await Mediator.Send(request);
                //fix label filters
                var labels = await Mediator.Send(new GetValueFiltersRequest
                {
                    Object = request,
                    ProjectId = shareResult.ProjectId
                });

                labels.User = shareResult.UserShared;
                labels.Date = shareResult.DateShared;

                response.Share = labels.Map().ToANew<ShareResultComparativeAnalysisTableResponse>();
                response.Share.Permissions = result.Permissions.Map().ToANew<PermissionShared>();
                await LogShareAccess(secretKey);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Ocorreu um erro interno: {ex.Message}" });
            }
        }
    }
}
