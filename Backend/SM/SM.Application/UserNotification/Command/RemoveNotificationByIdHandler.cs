using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using SM.Domain.Enum;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.RemoveNotificationById
{
    public class RemoveNotificationByIdRequest : IRequest
    {
        public long NotificationId { get; set; }
        public long UserId { get; set; }
    }

    public class RemoveNotificationByIdHandler : IRequestHandler<RemoveNotificationByIdRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validator;

        public RemoveNotificationByIdHandler(IUnitOfWork unitOfWork, ValidatorResponse validatorResponse)
        {
            _unitOfWork = unitOfWork;
            _validator = validatorResponse;
        }

        public async Task<Unit> Handle(RemoveNotificationByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.GetRepository<NotificacaoUsuario, long>()
                                    .Include("Notification")
                                    .GetAsync(x => x.Where(u => u.NotificationId == request.NotificationId && 
                                                                u.UsuarioId == request.UserId &&
                                                                (u.Notification.ProdutoType == (int?)ProductTypesEnum.SM || u.Notification.ProdutoType == (int?)ProductTypesEnum.All))
                                    );

            if (result == null)
            {
                _validator.AddNotification("Notificação não encontrada.");
                return Unit.Value;
            }
            result.Ativo = false;
            _unitOfWork.GetRepository<NotificacaoUsuario, long>().Update(result);
            _unitOfWork.Commit();
            return Unit.Value;
        }
    }
}

