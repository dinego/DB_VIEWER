using CMC.Common.Repositories;
using MediatR;
using SM.Domain.Options;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Abstractions.Behaviours;
using SM.Application.UserNotification.Validators;

namespace SM.Application.UserNotification.Command
{
    public class SendLinkRequest : IRequest
    {
        public long UserId { get; set; }
        public string To { get; set; }
        public string Url { get; set; }
        public string Message { get; set; }
    }

    public class SendLinkAccessHandler : IRequestHandler<SendLinkRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ValidatorResponse _validator;
        private readonly SendLinkValidatorsRule _sendLinkValidator;
        public SendLinkAccessHandler(IUnitOfWork unitOfWork,
            IMediator mediator,
            ValidatorResponse validator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _sendLinkValidator = new SendLinkValidatorsRule(_validator);
        }

        public async Task<Unit> Handle(SendLinkRequest request, CancellationToken cancellationToken)
        {
            if (!_sendLinkValidator.IsSatisfiedBy(request))
                return Unit.Value;
            await _mediator.Publish(new MailNotification.MailNotification
            {
                To = request.To,
                Subject = "[Carreira Muller] - Link acesso Salary Mark",
                Message = request.Message
            });

            return Unit.Value;
        }
    }
}

