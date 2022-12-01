using CMC.Common.Abstractions.Interactors.Interfaces;
using CMC.Common.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SM.Api.Security;
using SM.Domain.Enum;
using System;
using System.Threading.Tasks;

namespace SM.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        private IMediator _mediator;
        private ILogInteractor _logInteractor;
        private IHttpContextAccessor _httpContext;
        protected IMediator Mediator => _mediator ?? (_mediator = (IMediator)HttpContext.RequestServices.GetService(typeof(IMediator)));
        protected ILogInteractor LogUser => _logInteractor ?? (_logInteractor = (ILogInteractor)HttpContext.RequestServices.GetService(typeof(ILogInteractor)));
        protected IHttpContextAccessor context => _httpContext ?? (_httpContext = (IHttpContextAccessor)HttpContext.RequestServices.GetService(typeof(IHttpContextAccessor)));

        protected async Task LogUserAccess(string record, ModulesEnum module, ModulesSuItemsEnum? subModule = null)
        {
            if (User.IsSimulated()) return;
            await LogUser.RegisterHandler<LogUsuariosSm,long>(new LogUsuariosSm
            {
                UsuarioId = User.GetUserId(),
                DataAcesso = DateTime.Now,
                ModuloSmid = (long)module,
                Registro = record,
                ModuloSmsubItemId = (long?)subModule,
                Sessao = HttpContext.Request.Headers[HeaderNames.Authorization].ToString()?.Split(".")[2],
                Ip = context.HttpContext?.Connection?.RemoteIpAddress != null ? context.HttpContext.Connection.RemoteIpAddress.ToString() : string.Empty
            });
        }
        protected async Task LogShareAccess(string secretKey)
        {
            if (User.IsSimulated()) return;
            await LogUser.RegisterHandler<LogDadosCompartilhadosSm, long>(new LogDadosCompartilhadosSm
            {
                DataAcesso = DateTime.Now,
                ChaveSecreta = secretKey,
                Ip = context.HttpContext?.Connection?.RemoteIpAddress != null ? context.HttpContext.Connection.RemoteIpAddress.ToString() : string.Empty
            });
        }

        protected async Task LogExcelAccess(string record, ModulesEnum module, ModulesSuItemsEnum? subModule = null)
        {
            if (User.IsSimulated()) return;
            await LogUser.RegisterHandler<LogExportaExcelSm, long>(new LogExportaExcelSm
            {
                UsuarioId = User.GetUserId(),
                DataAcesso = DateTime.Now,
                ModuloSmid = (long)module,
                Registro = record,
                ModuloSmsubItemId = (long?)subModule,
                Sessao = HttpContext.Request.Headers[HeaderNames.Authorization].ToString()?.Split(".")[2],
                Ip = context.HttpContext?.Connection?.RemoteIpAddress != null ? context.HttpContext.Connection.RemoteIpAddress.ToString() : string.Empty
            });
        }

        protected async Task LogReportAccess(long reportId)
        {
            if (User.IsSimulated()) return;
            await LogUser.RegisterHandler<LogMeusRelatorios, long>(new LogMeusRelatorios
            {
                UsuarioId = User.GetUserId(),
                DataLog = DateTime.Now,
                FezDownload = true,
                RelatorioId = reportId,
                Sessao = HttpContext.Request.Headers[HeaderNames.Authorization].ToString()?.Split(".")[2],
            });
        }
    }
}