using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Repositories.Extensions;

namespace SM.Application.Reports.Queries
{
    public class GetReportsRequest : IRequest<List<GetReportsResponse>>
    {
        public long UserId { get; set; }
        public List<long> CompaniesId { get; set; }
        public string Term { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public OrderTypeEnum OrderType { get; set; } = OrderTypeEnum.DataAsc;
    }

    public class GetReportsResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Image { get; set; }
        public ReportType ReportType { get; set; }
        public string Html { get; set; }
        public string Script { get; set; }
        public string FileName { get; set; }
    }
    public class GetReportsHandler : IRequestHandler<GetReportsRequest, List<GetReportsResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetReportsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task<List<GetReportsResponse>> Handle(GetReportsRequest request, CancellationToken cancellationToken)
        {
            Regex regex = new Regex(@"(?<html><div[\s\w\W\d\D]+<\/div>)[\W\w\D\d\n\s]*<script>[\n\s\r]*(?<script>[\W\w\D\d]+)<\/script>");

            string orderByProperty = request.OrderType switch
            {
                OrderTypeEnum.DataAsc => OrderTypeEnum.DataAsc.GetDescription(),
                OrderTypeEnum.DataDes => OrderTypeEnum.DataDes.GetDescription(),
                OrderTypeEnum.TitleAsc => OrderTypeEnum.TitleAsc.GetDescription(),
                OrderTypeEnum.TitleDes => OrderTypeEnum.TitleDes.GetDescription(),
                _ => OrderTypeEnum.DataAsc.GetDescription()
            };

            bool isDesc = request.OrderType == OrderTypeEnum.DataDes || request.OrderType == OrderTypeEnum.TitleDes;

            var result = _unitOfWork.GetRepository<MeusRelatorios, long>().GetQueryList(x =>
                                        x.Where(r => (string.IsNullOrEmpty(request.Term) || r.Titulo.Contains(request.Term))
                                                      && ((!r.EmpresaId.HasValue && r.Ativo) || 
                                                      (r.EmpresaId.HasValue && request.CompaniesId.Contains(r.EmpresaId.Value) )) &&
                                                      ((r.ProdutoId.HasValue && r.ProdutoId.Value == (int)ProductTypesEnum.SM) ||
                                                        !r.ProdutoId.HasValue) &&
                                                      r.Ativo)
                                        .Select(s => new GetReportsResponse
                                        {
                                            Id = s.Id,
                                            Title = s.Titulo,
                                            Date = s.DataArquivo,
                                            Image = s.ImagemCapa,
                                            ReportType = ReportType.Embed.GetDescription().Contains(s.TipoArquivo) ? ReportType.Embed : ReportType.File,
                                            FileName = s.NomeArquivo
                                        }).OrderBy(orderByProperty, isDesc)
                                         .Skip((request.Page - 1) * request.PageSize)
                                         .Take(request.PageSize));

            var ids = result.Select(x => x.Id);
            var files = _unitOfWork.GetRepository<MeusRelatorios, long>()
                    .GetQueryList(x => x.Where(s => ids.Contains(s.Id) && ReportType.Embed.GetDescription().Contains(s.TipoArquivo))
                    .Select(res => new { res.Id, res.Arquivo }));

            foreach(var res in result)
            {
                var file = files.FirstOrDefault(f => f.Id == res.Id);
                if (file != null)
                {
                    res.Html = regex.Match(file.Arquivo).Success ? regex.Match(file.Arquivo).Groups["html"].Value : null;
                    res.Script = regex.Match(file.Arquivo).Success ? regex.Match(file.Arquivo).Groups["script"].Value : null;
                }
            }

            return Task.FromResult(result.ToList());
        }
    }
}
