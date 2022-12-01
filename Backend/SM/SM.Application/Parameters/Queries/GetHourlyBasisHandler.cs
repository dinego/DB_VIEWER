using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using Newtonsoft.Json;
using SM.Application.Interactors.Interfaces;
using SM.Application.Parameters.Validators;
using SM.Domain.Common;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Parameters.Queries
{
    public class GetHourlyBasisRequest : IRequest<List<GetHourlyBasisResponse>>
    {
        public bool IsAdmin { get; set; }
        public long CompanyId { get; set; }
        public long UserId { get; set; }
        public long ProjectId { get; set; }
    }
    public class GetHourlyBasisResponse
    {
        public long Id { get; set; }
        public string BaseSalary { get; set; }
        public bool Display { get; set; }
        public GetHourlyBasisParameter Parameters { get; set; }
    }
    public class GetHourlyBasisParameter
    {
        public double SelectedValue { get; set; }
        public bool Enabled { get; set; }
        public double[] Options { get; set; }
    }
    public class GetHourlyBasisHandler : IRequestHandler<GetHourlyBasisRequest, List<GetHourlyBasisResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validator;
        private readonly HourlyBasisDataBaseRule _validateDataBase;
        private readonly HourlyBasisYearSalaryRule _validateYearSalaries;
        private readonly IPermissionUserInteractor _permissionUserInteractor;

        public GetHourlyBasisHandler(IUnitOfWork unitOfWork, ValidatorResponse validator,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _permissionUserInteractor = permissionUserInteractor;
            _validateYearSalaries = new HourlyBasisYearSalaryRule(_validator);
            _validateDataBase = new HourlyBasisDataBaseRule(_validator);
        }
        public async Task<List<GetHourlyBasisResponse>> Handle(GetHourlyBasisRequest request, CancellationToken cancellationToken)
        {
            var result = new List<GetHourlyBasisResponse>();

            var usersPermissions = await _unitOfWork.GetRepository<UsuarioPermissaoSm, long>()
                                               .Include("EmpresaPermissao")
                                               .GetListAsync(x => x.Where(c => c.UsuarioId != request.UserId &&
                                                                          c.EmpresaPermissao != null && c.EmpresaPermissao.EmpresaId == request.CompanyId)
                                               .Select(x => JsonConvert.DeserializeObject<PermissionJson>(x.Permissao)));

            var userPermission = await _permissionUserInteractor.Handler(request.UserId);
            if (userPermission == null)
                throw new Exception("Não foi encontrado nenhum dado de permissionamento para o usuário.");

            var hourlyBasis = await _unitOfWork.GetRepository<BaseDeDados, long>()
                                               .GetListAsync(x => x.Where(bd => !userPermission.DataBase.Contains(bd.Id)));

            var yearSalaries = await _unitOfWork.GetRepository<SalarioAnualParametro, long>().GetListAsync(x => x.Where(s => s.Ativo.HasValue && s.Ativo.Value));

            if (!_validateDataBase.IsSatisfiedBy(hourlyBasis) || !_validateYearSalaries.IsSatisfiedBy(yearSalaries))
                return null;

            double? yearSalarayProjectValue = await _unitOfWork.GetRepository<SalarioAnualParametroProjeto, long>()
                                                      .Include("SalarioAnual")
                                                       .GetAsync(x => x.Where(sapp => sapp.ProjetoSalaryMarkId == request.ProjectId)
                                                       .Select(x => x.SalarioAnual != null ? x.SalarioAnual.Valor : (double?)null));

            hourlyBasis.ForEach(hb =>
            {
                var hourly = new GetHourlyBasisResponse
                {
                    Id = hb.Id,
                    BaseSalary = hb.Nome,
                    Display = !usersPermissions.Any(x => x.DataBase.Any(database => database == hb.Id)),
                    Parameters = new GetHourlyBasisParameter
                    {
                        Enabled = hb.Id == (long)DataBaseSalaryEnum.YearSalary,
                        SelectedValue = hb.Id == (long)DataBaseSalaryEnum.YearSalary && yearSalarayProjectValue.HasValue ? yearSalarayProjectValue.Value : yearSalaries.FirstOrDefault().Valor,
                        Options = hb.Id == (long)DataBaseSalaryEnum.YearSalary ? yearSalaries.Select(ys => ys.Valor).ToArray() : null
                    }
                };
                result.Add(hourly);
            });

            return result;
        }
    }
}
