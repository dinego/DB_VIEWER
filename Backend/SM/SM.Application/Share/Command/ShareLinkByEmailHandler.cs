using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using SM.Domain.Options;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Share.Command
{
    public class ShareLinkByEmailRequest : IRequest
    {
        public long UserId { get; set; }
        public string To { get; set; }
        public string URL { get; set; }
    }

    public class ShareLinkByEmailHandler : IRequestHandler<ShareLinkByEmailRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly SmtpConfiguration _smtpConfiguration;
        public ShareLinkByEmailHandler(IUnitOfWork unitOfWork, IMediator mediator, SmtpConfiguration smtpConfiguration)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _smtpConfiguration = smtpConfiguration;
        }
        public async Task<Unit> Handle(ShareLinkByEmailRequest request, CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(request.To))
                throw new Exception("E-mail de destino não é válido.");
            if (String.IsNullOrEmpty(request.URL))
                throw new Exception("URL de destino não é válida.");

            string formatedMesage = await CreateMessage(request);

            _smtpConfiguration.ContactUs = request.To;

            await _mediator.Publish(new MailNotification.MailNotification
            { To = _smtpConfiguration.ContactUs, Subject = "Compartilhamento de página", Message = formatedMesage });

            return Unit.Value;
        }

        public async Task<string> CreateMessage(ShareLinkByEmailRequest request)
        {
            var result = await _unitOfWork.GetRepository<Usuarios, long>()
                           .GetAsync(x => x.Where(u => u.Id == request.UserId));

            if (result == null)
                throw new Exception("Usuário não encontrado.");
            StringBuilder textBuilder = new StringBuilder();
            textBuilder.AppendLine($"O usuário: {result.Nome} deseja realizar um compartilhamento de informações pelo link: {request.URL}");
            textBuilder.AppendLine($"Obrigado,");
            textBuilder.AppendLine($"");
            textBuilder.AppendLine($"Salary Mark");
            return textBuilder.ToString();
        }
    }
}
