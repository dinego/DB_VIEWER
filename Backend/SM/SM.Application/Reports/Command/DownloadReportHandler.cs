using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Reports.Command
{
    public class DownloadReportRequest : IRequest<DownloadReportResponse>
    {
        public long User { get; set; }
        public long ReportId { get; set; }
    }
    public class DownloadReportResponse
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
    }
    public class DownloadReportHandler : IRequestHandler<DownloadReportRequest, DownloadReportResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validator;

        public DownloadReportHandler(IUnitOfWork unitOfWork, 
            ValidatorResponse validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public async Task<DownloadReportResponse> Handle(DownloadReportRequest request, CancellationToken cancellationToken)
        {
            var report = await _unitOfWork.GetRepository<MeusRelatorios, long>()
                                 .GetAsync(x => x.Where(r => r.Id == request.ReportId));

            if (report == null) { _validator.AddNotification("Nenhum relatório encontrado com este ID"); return null; }

            var extension = Regex.Match(report.NomeArquivo, @"\b(?<ext>\w+)$").Groups["ext"].Value;

            byte[] response = Convert.FromBase64String(report.Arquivo);

            if (response.Count() > 0)
            {
                return new DownloadReportResponse
                {
                    Extension = extension,
                    FileName = report.NomeArquivo,
                    File = response
                };
            }
            return null;
        }
    }
}
