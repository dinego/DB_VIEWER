using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace SM.Application.Interactors
{
    public class PermissionCompanyResponse
    {
        public PermissionJson Permission { get; set; }
        public List<GlobalLabelsJson> GlobalLabels { get; set; }
    }
    public class PermissionCompanyInteractor : IPermissionCompanyInteractor
    {
        private readonly IUnitOfWork _unitOfWork;
        public PermissionCompanyInteractor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PermissionCompanyResponse> Handler(long projectId)
        {
            long? companyId = await _unitOfWork.GetRepository<ProjetosSalaryMark, long>()
                .GetAsync(x => x.Where(up => up.Id == projectId).Select(res => res.CodigoPai));

            if (!companyId.HasValue)
                throw new Exception("Projeto não foi encontrado.");

            var companyPermission = await _unitOfWork.GetRepository<EmpresaPermissaoSm, long>()
                .GetAsync(g => g.Where(ep => ep.EmpresaId == companyId.Value)
                .Select(res => new { res.RotulosGlobais, res.Permissao }));

            if (companyPermission == null)
                throw new Exception("Permissões da empresa não foram encontrado.");

            var globalLabels = new List<GlobalLabelsJson>();
            var permission = new PermissionJson();
            companyPermission.Permissao.TryParseJson(out permission);
            companyPermission.RotulosGlobais.TryParseJson(out globalLabels);
            var result = new PermissionCompanyResponse
            {
                Permission = permission,
                GlobalLabels = globalLabels
            };
            return result;
        }
    }
}
