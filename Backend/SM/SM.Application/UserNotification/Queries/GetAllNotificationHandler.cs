using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.GetAllNotification
{
    public class GetAllNotificationRequest : IRequest<GetAllNotificationResponse>
    {
        public long UserId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class NotificationItemResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Create { get; set; }
        public bool IsRead { get; set; }
    }
    public class GetAllNotificationResponse
    {
        public IEnumerable<NotificationItemResponse> Notifications { get; set; }
    }

    public class GetAllNotificationHandler : IRequestHandler<GetAllNotificationRequest, GetAllNotificationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllNotificationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GetAllNotificationResponse> Handle(GetAllNotificationRequest request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.GetRepository<NotificacaoUsuario, long>()
                                    .Include("Notification")
                                    .GetListAsync(x => x.Where(u => u.UsuarioId == request.UserId
                                                            && u.Ativo == true
                                                            && (u.Notification.ProdutoType == (int?)ProductTypesEnum.SM || u.Notification.ProdutoType == (int?)ProductTypesEnum.All))
                                                  .Select(res => new NotificationItemResponse
                                                  {
                                                      Id = res.Notification.Id,
                                                      Title = res.Notification.Titulo,
                                                      Description = res.Notification.Descricao,
                                                      Create = res.Notification.DataDeCriacao,
                                                      IsRead = res.Lido
                                                  })
                                                  .Skip((request.Page - 1) * request.PageSize)
                                                  .Take(request.PageSize)
                                                  .OrderBy(x => !x.IsRead)
                                                  .OrderByDescending(x => x.Create)
                                    );

            return new GetAllNotificationResponse { Notifications = result};
        }
    }
}

