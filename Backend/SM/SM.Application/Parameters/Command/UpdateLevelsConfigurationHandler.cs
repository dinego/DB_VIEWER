using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using Newtonsoft.Json;
using SM.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Parameters.Command
{

    public class UpdateLevelsConfigurationRequest : IRequest
    {
        public IEnumerable<UpdateLevelsConfiguration> Levels { get; set; }
        public long UserId { get; set; }
        public long CompanyId { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class UpdateLevelsConfiguration
    {
        public int LevelId { get; set; }
        public string Level { get; set; }
        public bool Enabled { get; set; } = true;
    }

    public class UpdateLevelsConfigurationHandler
        : IRequestHandler<UpdateLevelsConfigurationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validator;

        public UpdateLevelsConfigurationHandler(IUnitOfWork unitOfWork, ValidatorResponse validatorResponse)
        {
            _unitOfWork = unitOfWork;
            _validator = validatorResponse;
        }
        public async Task<Unit> Handle(UpdateLevelsConfigurationRequest request, CancellationToken cancellationToken)
        {
            var companyPermissions = await _unitOfWork.GetRepository<EmpresaPermissaoSm, long>()
                                               .Include("Empresa")
                                               .Include("Empresa.ProjetosSalaryMarkEmpresas")
                                               .GetAsync(x => x.Where(c => c.EmpresaId == request.CompanyId ||
                                                                     (c.Empresa != null && c.Empresa.ProjetosSalaryMarkEmpresas.Select(x => x.EmpresaId).Any(empId => empId == request.CompanyId)))
                                               .Select(res => new
                                               {
                                                   CompanyFather = res.Empresa != null && res.Empresa.CodigoPai.HasValue ? res.Empresa.CodigoPai : res.EmpresaId,
                                                   Permission = res.Permissao
                                               }));

            if (companyPermissions == null) { _validator.AddNotification("Não encontramos dados de permissionamento para a empresa selecionada."); return Unit.Value; }

            PermissionJson companyExceptions = JsonConvert.DeserializeObject<PermissionJson>(companyPermissions.Permission);

            var companiesLevels = await _unitOfWork.GetRepository<NivelEmpresas, long>()
                            .GetListAsync(x =>
                            x.Where(n => companyPermissions.CompanyFather == n.EmpresaId &&
                            (companyExceptions.Levels.Safe().Count() <= 0 || !companyExceptions.Levels.Contains(n.NivelId))));

            var lstNewCompanyLevels = new List<NivelEmpresas>();
            foreach (var level in request.Levels)
            {
                var companiesLevel = companiesLevels.FirstOrDefault(f => f.NivelId == level.LevelId);
                if (companiesLevel != null)
                {
                    companiesLevel.NivelEmpresa = (level.Enabled && !string.IsNullOrWhiteSpace(level.Level)) ?
                        level.Level : string.Empty;
                }
                else
                {
                    var newCompanyLevel = new NivelEmpresas
                    {
                        NivelId = level.LevelId,
                        EmpresaId = companyPermissions.CompanyFather.Value,
                        NivelEmpresa = level.Level
                    };
                    lstNewCompanyLevels.Add(newCompanyLevel);
                }
            }
            _unitOfWork.GetRepository<NivelEmpresas, long>().Update(companiesLevels);
            _unitOfWork.GetRepository<NivelEmpresas, long>().Add(lstNewCompanyLevels);
            await _unitOfWork.CommitAsync();
            return Unit.Value;

        }
    }
}
