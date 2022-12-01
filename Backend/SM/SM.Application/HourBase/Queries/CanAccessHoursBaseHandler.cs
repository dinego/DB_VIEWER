using CMC.Common.Domain.Entities;
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
    public class CanAccessHoursBaseRequest : IRequest<bool>
    {
        public long UserId { get; set; }
    }

    public class CanAccessHoursBaseHandler : IRequestHandler<CanAccessHoursBaseRequest, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public CanAccessHoursBaseHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(CanAccessHoursBaseRequest request, CancellationToken cancellationToken)
        {
            var listEnums = Enum.GetValues(typeof(DataBaseSalaryEnum)) as IEnumerable<DataBaseSalaryEnum>;

            var exceptionUser = await _unitOfWork.GetRepository<UsuarioPermissaoSm, long>()
                                    .GetAsync(x => x.Where(u => u.UsuarioId == request.UserId)
                                               .Select(x => x.Permissao));

            var userExceptionObj = exceptionUser == null ? new List<long>() : JsonConvert.DeserializeObject<PermissionJson>(exceptionUser).DataBase;

            var result = listEnums.Any(hourlyBase => !userExceptionObj.Any() || !userExceptionObj.Contains((long)hourlyBase));

            return result;
        }

    }
}

