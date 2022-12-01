using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using Newtonsoft.Json;
using SM.Domain.Common;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Parameters.Command
{
    public class SaveHourlyBasisRequest : IRequest
    {
        public List<SaveHourlyBasisItems> HourlyBasis { get; set; }
        public bool IsAdmin { get; set; }
        public long CompanyId { get; set; }
        public long UserId { get; set; }
        public long ProjectId { get; set; }
    }

    public class SaveHourlyBasisItems
    {
        public long Id { get; set; }
        public bool Display { get; set; }
        public double SelectedValue { get; set; }
    }

    public class SaveHourlyBasisHandler : IRequestHandler<SaveHourlyBasisRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaveHourlyBasisHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(SaveHourlyBasisRequest request, CancellationToken cancellationToken)
        {
            var userPermissions = await _unitOfWork.GetRepository<UsuarioPermissaoSm, long>()
                                               .Include("EmpresaPermissao")
                                               .GetListAsync(x => x.Where(c => c.UsuarioId != request.UserId &&
                                                                          c.EmpresaPermissao != null && c.EmpresaPermissao.EmpresaId == request.CompanyId));

            userPermissions.ForEach(user =>
            {
                var userExceptions = JsonConvert.DeserializeObject<PermissionJson>(user.Permissao);

                request.HourlyBasis.ForEach(hourlyBasis =>
                {
                    switch (hourlyBasis.Display)
                    {
                        case true:
                            userExceptions.DataBase = userExceptions.DataBase.Where(db => db != hourlyBasis.Id).ToList();
                            user.Permissao = JsonConvert.SerializeObject(userExceptions);
                            break;
                        case false:
                            var userDataBase = userExceptions.DataBase.Where(db => db == hourlyBasis.Id).ToList();
                            if (userDataBase == null || userDataBase.Count <= 0)
                            {
                                var databases = userExceptions.DataBase.ToList();
                                databases.Add(hourlyBasis.Id);
                                userExceptions.DataBase = databases;
                            }
                            user.Permissao = JsonConvert.SerializeObject(userExceptions);
                            break;
                    }
                });
            });

            var yearSalaryProject = await _unitOfWork.GetRepository<SalarioAnualParametroProjeto, long>()
                                                .GetAsync(x => x.Where(s => s.ProjetoSalaryMarkId == request.ProjectId));

            var yearSalary = request.HourlyBasis.FirstOrDefault(x => x.Id == (long)DataBaseSalaryEnum.YearSalary);
            SalarioAnualParametro yearSalaryParameter = null;
            if (yearSalary != null)
                yearSalaryParameter = await _unitOfWork.GetRepository<SalarioAnualParametro, long>()
                                                .GetAsync(x => x.Where(s => s.Ativo.HasValue && s.Ativo.Value && s.Valor == yearSalary.SelectedValue));

            if (yearSalaryParameter != null)
            {
                _unitOfWork.GetRepository<SalarioAnualParametroProjeto, long>().Delete(yearSalaryProject);

                var yearSalarayProject = new SalarioAnualParametroProjeto
                {
                    ProjetoSalaryMarkId = request.ProjectId,
                    SalarioAnualParametroId = yearSalaryParameter.Id
                };
                _unitOfWork.GetRepository<SalarioAnualParametroProjeto, long>().Add(yearSalarayProject);
            }

            _unitOfWork.GetRepository<UsuarioPermissaoSm, long>().Update(userPermissions);
            _unitOfWork.Commit();
            return Unit.Value;
        }
    }
}
