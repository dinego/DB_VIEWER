using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.GetTableSalary
{
    public class GetAllSalaryTablesRequest : IRequest<GetAllSalaryTablesResponse>
    {
        public long? UnitId { get; set; }
        public long UserId { get; set; }
        public long[] Units { get; set; }
        public long ProjectId { get; set; }
    }

    public class TableSalaryResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long ProjectId { get; set; }
    }

    public class GetAllSalaryTablesResponse
    {
        public IEnumerable<TableSalaryResponse> TableSalaryResponses { get; set; }
    }

    public class CompanyJson
    {
        public IEnumerable<long> Levels { get; set; }
        public IEnumerable<long> TypeOfContract { get; set; }
        public IEnumerable<long> DataBase { get; set; }
        public IEnumerable<FieldCheckedCompanyJson> Modules { get; set; }
        public IEnumerable<FieldCheckedCompanyJson> Permission { get; set; }
        public IEnumerable<string> Areas { get; set; }
        public IEnumerable<FieldCheckedCompanyJson> Contents { get; set; }
        public IEnumerable<int> Scenario { get; set; }
    }

    public class FieldCheckedCompanyJson
    {
        public long Id { get; set; }
        public IEnumerable<long> SubItems { get; set; }
    }

    public class GetAllSalaryTablesHandler : IRequestHandler<GetAllSalaryTablesRequest, GetAllSalaryTablesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllSalaryTablesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GetAllSalaryTablesResponse> Handle(GetAllSalaryTablesRequest request, CancellationToken cancellationToken)
        {

            var exceptionUser = await _unitOfWork.GetRepository<UsuarioPermissaoSm, long>()
                                    .GetAsync(x => x.Where(u => u.UsuarioId == request.UserId)
                                    .Select(x => x.Permissao));
            if (!string.IsNullOrEmpty(exceptionUser))
            {
                CompanyJson userExceptionObj = JsonConvert.DeserializeObject<CompanyJson>(exceptionUser);
                if (userExceptionObj.Contents.Count() > 0 && userExceptionObj.Contents.Any(x => x.Id == (long)ContentsSubItemsEnum.SalaryTable))
                {
                    var proibitedItems = userExceptionObj.Contents.Select(x => x.SubItems).ToList();

                    List<long> listInteger = new List<long>();
                    for (int i = 0; i < proibitedItems.Count; i++)
                    {
                        foreach (var item in proibitedItems[i])
                        {
                            listInteger.Add(item);
                        }
                    }

                    var resultProjetosSalaryMarkEmpresas = await _unitOfWork.GetRepository<TabelasSalariais, long>()
                                           .GetListAsync(x => x.Where(u => u.ProjetoId == request.ProjectId &&
                                                                      u.Ativo.HasValue && u.Ativo.Value
                                                                      && !listInteger.Contains(u.TabelaSalarialIdLocal) &&
                                                                      (request.UnitId.HasValue || (u.GruposProjetosSalaryMarkMapeamento != null &&
                                                                                                   u.GruposProjetosSalaryMarkMapeamento.Any(tse => request.Units.Contains(tse.EmpresaId)))) &&
                                                                      (!request.UnitId.HasValue || (u.GruposProjetosSalaryMarkMapeamento != null &&
                                                                                                    u.GruposProjetosSalaryMarkMapeamento.Any(tse => tse.EmpresaId == request.UnitId.Value))))
                                           .Select(x => new TableSalaryResponse
                                           {
                                               Id = x.TabelaSalarialIdLocal,
                                               Title = x.TabelaSalarial,
                                               ProjectId = x.ProjetoId
                                           }));

                    return new GetAllSalaryTablesResponse() { TableSalaryResponses = resultProjetosSalaryMarkEmpresas };
                }
            }
            var resultProjetosSalaryMarkEmpresasWhitoutBlock = await _unitOfWork.GetRepository<TabelasSalariais, long>()
                                       .Include("GruposProjetosSalaryMarkMapeamento")

                                       .GetListAsync(x => x.Where(u => u.ProjetoId == request.ProjectId &&
                                                        u.Ativo.HasValue && u.Ativo.Value &&
                                                      (request.UnitId.HasValue || (u.GruposProjetosSalaryMarkMapeamento != null &&
                                                                                      u.GruposProjetosSalaryMarkMapeamento.Any(tse => request.Units.Contains(tse.EmpresaId)))) &&
                                                      (!request.UnitId.HasValue || (u.GruposProjetosSalaryMarkMapeamento != null &&
                                                                                       u.GruposProjetosSalaryMarkMapeamento.Any(tse => tse.EmpresaId == request.UnitId.Value))))
                                       .Select(x => new TableSalaryResponse
                                       {
                                           Id = x.TabelaSalarialIdLocal,
                                           Title = x.TabelaSalarial,
                                           ProjectId = x.ProjetoId
                                       })
                                       );

            return new GetAllSalaryTablesResponse() { TableSalaryResponses = resultProjetosSalaryMarkEmpresasWhitoutBlock };
        }
    }
}

