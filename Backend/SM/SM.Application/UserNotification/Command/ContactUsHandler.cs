using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using SM.Domain.Options;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.ContactUs
{
    public class ContactUsRequest : IRequest
    {
        public long UserId { get; set; }
        public IFormFile Attachment { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

    public class ContactUsHandler : IRequestHandler<ContactUsRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly SmtpConfiguration _smtpConfiguration;
        public ContactUsHandler(IUnitOfWork unitOfWork, IMediator mediator, SmtpConfiguration smtpConfiguration)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _smtpConfiguration = smtpConfiguration;
        }

        public async Task<Unit> Handle(ContactUsRequest request, CancellationToken cancellationToken)
        {
            string formatedMesage = await CreateMessage(request);
            await _mediator.Publish(new MailNotification.MailNotification
            { 
                Files = new System.Collections.Generic.List<CMC.Common.Abstractions.Notification.FileUpload> { 
                    new CMC.Common.Abstractions.Notification.FileUpload { 
                        File = request.Attachment?.OpenReadStream(), FileName = request.Attachment.FileName 
                    } 
                }, 
                To = _smtpConfiguration.ContactUs, 
                Subject = request.Subject, Message = formatedMesage 
            });

            return Unit.Value;
        }

        public async Task<string> CreateMessage(ContactUsRequest request)
        {
                var result = await _unitOfWork.GetRepository<Usuarios, long>()
                               .Include("Empresa")
                               .GetAsync(x => x.Where(u => u.Id == request.UserId));

                if (result == null)
                    throw new Exception("Usuário não encontrado.");
                StringBuilder textBuilder = new StringBuilder();
                textBuilder.AppendLine($"Usuário Solicitante: {result.Nome}");
                textBuilder.AppendLine($"Empresa: {result.Empresa.NomeFantasia}");
                textBuilder.AppendLine($"Mensagem: {request.Message}");
                return textBuilder.ToString();
        }
    }
}

