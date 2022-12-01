using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.SetReadNotification
{
    public class SetReadNotificationRequest : IRequest
    {
        public long NotificationId { get; set; }
        public long UserId { get; set; }
    }

    public class SetReadNotificationHandler : IRequestHandler<SetReadNotificationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SetReadNotificationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(SetReadNotificationRequest request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.GetRepository<NotificacaoUsuario, long>()
                                    .GetAsync(x => x.Where(u => u.NotificationId == request.NotificationId && u.UsuarioId == request.UserId));

            if (result == null)
                throw new Exception("Notificação não encontrada.");
            result.Lido = true;
            _unitOfWork.GetRepository<NotificacaoUsuario, long>().Update(result);
            _unitOfWork.Commit();
            return Unit.Value;
        }
    }
}

