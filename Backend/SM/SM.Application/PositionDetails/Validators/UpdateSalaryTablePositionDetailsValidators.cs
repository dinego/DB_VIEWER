using System.Collections.Generic;
using System.Linq;
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Abstractions.Validators.Abstractions;
using CMC.Common.Extensions;
using SM.Application.PositionDetails.Command;

namespace SM.Application.PositionDetails.Validators
{
    public class UpdateSalaryTablePositionDetailsRule : CompositeRule<UpdateSalaryTablePositionDetailsRequest>
    {
        private ValidatorResponse _validator;

        public UpdateSalaryTablePositionDetailsRule(ValidatorResponse validator)
        {
            _validator = validator;
        }
        public override bool IsSatisfiedBy(UpdateSalaryTablePositionDetailsRequest request)
        {
            if (!request.CanEditMappingPositionSM)
                _validator.AddNotification("Você não tem permissão para editar o mapeamento das tabelas.");
            if (!request.SalaryTableMappings.Safe().Any())
                _validator.AddNotification("Não encontramos os dados necessários para processar sua solicitação.");

            var salaryTableMappingDuplicate = new List<SalaryTableMappingData>();
            bool hasDuplicateData = false;
            request.SalaryTableMappings.ForEach(stm =>
            {
                if (salaryTableMappingDuplicate.Any(cl => cl.GSM == stm.GSM && cl.UnitId == stm.UnitId && cl.TableId == stm.TableId))
                {
                    hasDuplicateData = true;
                    return;
                }
                salaryTableMappingDuplicate.Add(stm);
            });

            if (hasDuplicateData)
                _validator.AddNotification("Não é permitido ter o mesmo GSM ser usado mais de uma vez para a mesma empresa.");
            return !_validator.HasNotifications;
        }
    }
}
