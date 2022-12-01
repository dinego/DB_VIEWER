using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.TableSalary.Queries.Response;

namespace SM.Application.TableSalary.Queries.GetSalaryTableValuesHandler
{
    public class GetGsmBySalaryTableRequest : IRequest<IEnumerable<GsmByTable>>
    {
        public long TableId { get; set; }
        public long ProjectId { get; set; }
        public long? UnitId { get; set; }
    }
    public class GetGsmBySalaryTableHandler : IRequestHandler<GetGsmBySalaryTableRequest, IEnumerable<GsmByTable>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetGsmBySalaryTableHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<GsmByTable>> Handle(GetGsmBySalaryTableRequest request, CancellationToken cancellationToken)
        {
            var lstGsms = await _unitOfWork.GetRepository<TabelasSalariaisValores, long>()
                                   .GetListAsync(x => x.Where(s => s.ProjetoId == request.ProjectId &&
                                                              (!request.UnitId.HasValue || (s.Projeto != null &&
                                                                s.Projeto.ProjetosSalaryMarkEmpresas
                                                                        .Any(psm => psm.EmpresaId == request.UnitId.Value &&
                                                                                    psm.ProjetoId == request.ProjectId))) &&

                                                                s.TabelaSalarialIdLocal == request.TableId &&
                                                                s.TabelasSalariais != null &&
                                                               (s.TabelasSalariais.Ativo.HasValue && s.TabelasSalariais.Ativo.Value))
                                                       .Select(s => new GsmByTable
                                                       {
                                                           Id = s.Grade,
                                                           Title = s.Grade.ToString()
                                                       })
                                                       .Distinct()
                                                       .OrderBy(gsm => gsm.Id)
                                                 );
            return lstGsms;
        }
    }
}