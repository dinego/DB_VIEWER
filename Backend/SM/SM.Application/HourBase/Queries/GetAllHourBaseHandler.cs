using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using Newtonsoft.Json;
using SM.Domain.Common;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.GetAllHoursBase
{
    public class GetAllHourBaseRequest : IRequest<GetAllHoursBaseResponse>
    {
        public long UserId { get; set; }
        public bool IsAdmin { get; set; }
        public long CompanyId { get; set; }
    }

    public class HoursBaseResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
    }
    public class GetAllHoursBaseResponse
    {
        public IEnumerable<HoursBaseResponse> HoursBaseResponse { get; set; }
    }

    public class GetAllHourBaseHandler : IRequestHandler<GetAllHourBaseRequest, GetAllHoursBaseResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllHourBaseHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GetAllHoursBaseResponse> Handle(GetAllHourBaseRequest request, CancellationToken cancellationToken)
        {
            var listEnums = Enum.GetValues(typeof(DataBaseSalaryEnum)) as IEnumerable<DataBaseSalaryEnum>;

            var exceptionUser = await _unitOfWork.GetRepository<UsuarioPermissaoSm, long>()
                                    .GetAsync(x => x.Where(u => u.UsuarioId == request.UserId)
                                               .Select(x => x.Permissao));

            var userExceptionObj = exceptionUser == null ?
                new List<long>()
                : JsonConvert.DeserializeObject<PermissionJson>(exceptionUser).DataBase;

            if (request.IsAdmin)
            {
                var usersPermissions = await _unitOfWork.GetRepository<UsuarioPermissaoSm, long>()
                                               .Include("EmpresaPermissao")
                                               .GetListAsync(x => x.Where(c => c.UsuarioId != request.UserId &&
                                                                          c.EmpresaPermissao != null && c.EmpresaPermissao.EmpresaId == request.CompanyId)
                                               .Select(x => JsonConvert.DeserializeObject<PermissionJson>(x.Permissao)));

                var userPermissionsDataBase = usersPermissions.SelectMany(x => x.DataBase);

                userExceptionObj = userExceptionObj.Where(dataBase => !userPermissionsDataBase.Any(upData => upData == dataBase));
                userExceptionObj = userExceptionObj.Safe().Any() ? userExceptionObj : userPermissionsDataBase;
            }

            var hoursBase = listEnums.Where(l => !userExceptionObj.Any() || !userExceptionObj.Contains((long)l))?
                .Select(s => new HoursBaseResponse
                {
                    Id = (long)s,
                    Title = s.GetDescription()
                });

            return new GetAllHoursBaseResponse() { HoursBaseResponse = hoursBase };
        }

    }
}

