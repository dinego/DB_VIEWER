using System.Linq;
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Abstractions.Validators.Abstractions;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using SM.Application.Parameters.Command;

namespace SM.Application.Parameters.Validators
{
    public class SalaryStrategyValidatorRule : CompositeRule<UpdateSalaryStrategyRequest>
    {
        private ValidatorResponse _validator;
        private IUnitOfWork _unitOfWork;

        public SalaryStrategyValidatorRule(ValidatorResponse validator, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _unitOfWork = unitOfWork;
        }
        public override bool IsSatisfiedBy(UpdateSalaryStrategyRequest request)
        {
            bool isValid = request.IsAdmin;
            if (!isValid)
                _validator.AddNotification("Você não tem acesso a tela Estratégia Salarial.");
            if (string.IsNullOrEmpty(request.Table))
            {
                _validator.AddNotification("O nome da tabela não pode ser vazio.");
                isValid = false;
            }
            if (request.SalaryStrategy.Safe().Any(ss => string.IsNullOrEmpty(ss.Value)))
            {
                _validator.AddNotification("O nome da faixa não pode ser vazio.");
                isValid = false;
            }

            if (!string.IsNullOrEmpty(request.Table))
            {
                long? salaryTable = _unitOfWork.GetRepository<TabelasSalariais, long>()
                .Get(v => v.Where(w => w.TabelaSalarialIdLocal != request.TableId &&
                                       w.ProjetoId == request.ProjectId &&
                                       w.TabelaSalarial.Trim().ToLower().Equals(request.Table.Trim().ToLower()))
                           .Select(res => res.Id));
                if (salaryTable.HasValue && salaryTable.Value > 0)
                {
                    _validator.AddNotification("Já existe uma tabela com o nome informado.");
                    isValid = false;
                }
            }

            return isValid;
        }
    }
}
