using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Application.Interactors
{
    public class MultiplierFactorTypeContratcInteractor : IMultiplierFactorTypeContratcInteractor
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validatorResponse;

        public MultiplierFactorTypeContratcInteractor(IUnitOfWork unitOfWork,
            ValidatorResponse validatorResponse)
        {
            _unitOfWork = unitOfWork;
            _validatorResponse = validatorResponse;
        }

        public async Task<double?> Handler(long projectId, ContractTypeEnum contratcType = ContractTypeEnum.CLT)
        {
            if (contratcType == ContractTypeEnum.CLT)
                return 1;

            var result = await _unitOfWork.GetRepository<ConfiguracoesSistemaPj, long>()
                        .GetListAsync(x => x.Where(ctc => ctc.Ativo.HasValue &&
                        ctc.Ativo.Value && ctc.TipoDeContratoId == (long)contratcType &&
                        ctc.ProjetoId == projectId));

            if (result.Count() == 0)
                return null;

            return result.Sum(s => s.Valor);
        }
    }
}
