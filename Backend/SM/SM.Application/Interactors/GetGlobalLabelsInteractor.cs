using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using Newtonsoft.Json;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Application.Interactors
{
    public class GetGlobalLabelsInteractor : IGetGlobalLabelsInteractor
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetGlobalLabelsInteractor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<GlobalLabelsJson>> Handler(long projectId)
        {
            var project = await _unitOfWork.GetRepository<ProjetosSalaryMark, long>()
                .GetAsync(x => x.Where(up => up.Id == projectId));

            if (project == null)
                throw new Exception("Projeto não encontrado");

            var company = await _unitOfWork.GetRepository<EmpresaPermissaoSm, long>()
                .GetAsync(g => g.Where(ep => ep.EmpresaId == project.CodigoPai));

            if (string.IsNullOrWhiteSpace(company.RotulosGlobais))
                return new List<GlobalLabelsJson>(); ;

            return JsonConvert.DeserializeObject<IEnumerable<GlobalLabelsJson>>
                (company.RotulosGlobais);

        }
    }
}
