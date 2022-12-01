using CMC.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SM.Api.Helpers;
using SM.Api.Security;
using SM.Api.ViewModel;
using SM.Application.Reports.Command;
using SM.Application.Reports.Queries;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SM.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize]
    [ModuleAuthorize(ModulesEnum.Reports, ModulesSuItemsEnum.None)]
    public class ReportsController : BaseController
    {
        /// <summary>
        /// Get all reports
        /// </summary>
        /// <param name="orderType">use values: dataAsc = 1, dataDes = 2, titleAsc = 3, titleDes = 4</param>
        /// <param name="page">set page</param>
        /// <param name="term">term is optional</param>
        /// <returns>List of the resports</returns>
        /// <response code="200">Reports retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        /// <response code="404">Not Found</response>
        [HttpGet("GetReports")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetReportsResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> Get([Required] OrderTypeEnum orderType, [Required] int page, string term = null)
        {
            try
            {
                var result = await Mediator.Send(new GetReportsRequest
                {
                    UserId = User.GetUserId(),
                    CompaniesId = User.GetUserCompanies(),
                    Term = term,
                    Page = page,
                    OrderType = orderType
                });
                await LogUserAccess(LogActionsEnum.AccessReports.GetDescription(),
                    ModulesEnum.Reports);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Download report file
        /// </summary>
        /// <param name="report">code of reportID selected</param>
        ///<returns>File</returns>
        /// <response code="200">Reports retorned with successfully</response>
        /// <response code="400">Internal Error</response>
        [HttpPost("DownloadFile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultResponse))]
        public async Task<IActionResult> DownloadFile(ReportViewModel report)
        {
            try
            {
                var file = await Mediator.Send(new DownloadReportRequest
                {
                    User = User.GetUserId(),
                    ReportId = report.ReportId
                });
                await LogUserAccess(LogActionsEnum.DownloadReports.GetDescription(),
                    ModulesEnum.Reports);

                await LogReportAccess(report.ReportId);
                

                return File(
                    fileContents: file.File,
                    contentType: file.Extension.Equals("pdf") ? $"application/{file.Extension}" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: file.FileName
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Register log to embeded report that not access.
        /// </summary>
        /// <param name="report">code of reportID selected</param>
        /// <returns>File</returns>
        /// <response code="200">log save with successfully</response>
        /// <response code="400">Internal Error</response>
        /// <response code="404">Not Found</response>
        [HttpPost("RegisterLog")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FileContentResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(FileContentResult))]
        public async Task<IActionResult> RegisterLog([FromBody] ReportViewModel report)
        {
            try
            {
                if (!User.IsSimulated())
                    await LogReportAccess(report.ReportId);
                return Ok(new { message = "Salvo com sucesso." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
