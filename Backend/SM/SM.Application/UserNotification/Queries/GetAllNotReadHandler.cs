using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.GetAllNotRead
{
    public class GetAllNotReadRequest : IRequest<GetAllNotReadResponse>
    {
        public long UserId { get; set; }
    }

    public class NotificationNotReadResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Create { get; set; }
        public bool IsRead { get; set; }
        public int Amount { get; set; }
    }
    public class GetAllNotReadResponse
    {
        public IEnumerable<NotificationNotReadResponse> Notifications { get; set; }
        //public int Amount { get; set; }
        //public int AmountNotRead { get; set; }
    }

    public class GetAllNotReadHandler : IRequestHandler<GetAllNotReadRequest, GetAllNotReadResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllNotReadHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GetAllNotReadResponse> Handle(GetAllNotReadRequest request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.GetRepository<NotificacaoUsuario, long>()
                                    .Include("Notification")
                                    .GetListAsync(x => x.Where(u => u.UsuarioId == request.UserId
                                                            && u.Ativo == true
                                                            && u.Lido == false
                                                            && (u.Notification.ProdutoType == (int?)ProductTypesEnum.SM || u.Notification.ProdutoType == (int?)ProductTypesEnum.All))
                                                  .Select(res => new NotificationNotReadResponse
                                                  {
                                                      Id = res.Notification.Id,
                                                      Title = res.Notification.Titulo,
                                                      Description = res.Notification.Descricao,
                                                      Create = res.Notification.DataDeCriacao,
                                                      IsRead = res.Lido
                                                  })
                                                  .OrderByDescending(x => x.Create)
                                    );

            return new GetAllNotReadResponse { Notifications = result};
        }
    }
}

