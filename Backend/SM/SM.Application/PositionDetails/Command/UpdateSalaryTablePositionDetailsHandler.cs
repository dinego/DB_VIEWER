using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.PositionDetails.Validators;

namespace SM.Application.PositionDetails.Command
{
    public class UpdateSalaryTablePositionDetailsRequest : IRequest
    {
        public long ProjectId { get; set; }
        public long PositionId { get; set; }
        public bool CanEditMappingPositionSM { get; set; }
        public List<SalaryTableMappingData> SalaryTableMappings { get; set; }
    }

    public class SalaryTableMappingData
    {
        public long TableId { get; set; }
        public long UnitId { get; set; }
        public int GSM { get; set; }
        public bool Deleted { get; set; }
        public bool Created { get; set; }
    }

    public class UpdateSalaryTablePositionDetailsHandler : IRequestHandler<UpdateSalaryTablePositionDetailsRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validatorResponse;
        private readonly UpdateSalaryTablePositionDetailsRule _validatorSalaryTableRule;
        public UpdateSalaryTablePositionDetailsHandler(IUnitOfWork unitOfWork,
        ValidatorResponse validatorResponse)
        {
            _unitOfWork = unitOfWork;
            _validatorResponse = validatorResponse;
            _validatorSalaryTableRule = new UpdateSalaryTablePositionDetailsRule(validatorResponse);
        }
        public async Task<Unit> Handle(UpdateSalaryTablePositionDetailsRequest request, CancellationToken cancellationToken)
        {
            if (!_validatorSalaryTableRule.IsSatisfiedBy(request))
                return Unit.Value;

            var positionMappings = await _unitOfWork.GetRepository<CargosProjetosSmmapeamento, long>()
                        .GetListAsync(x => x.Where(cpm => cpm.ProjetoId == request.ProjectId &&
                        cpm.CargoProjetoSmidLocal == request.PositionId));
            var positionMappingCreated = new List<CargosProjetosSmmapeamento>();
            request.SalaryTableMappings.ForEach(stm =>
            {
                if (!stm.Created)
                {
                    var positionUpdated = positionMappings.FirstOrDefault(pm => pm.TabelaSalarialIdLocal == stm.TableId && pm.EmpresaId == stm.UnitId);
                    if (positionUpdated != null)
                    {
                        positionUpdated.Gsm = stm.Deleted ? positionUpdated.Gsm : stm.GSM;
                        positionUpdated.Status = !stm.Deleted;
                    }
                    return;
                }
                var newMapping = new CargosProjetosSmmapeamento
                {
                    CargoProjetoSmidLocal = request.PositionId,
                    TabelaSalarialIdLocal = stm.TableId,
                    EmpresaId = stm.UnitId,
                    ProjetoId = request.ProjectId,
                    Gsm = stm.GSM,
                    Gsminicial = stm.GSM,
                    GsmOriginal = stm.GSM,
                    Status = true
                };
                positionMappingCreated.Add(newMapping);
            });

            _unitOfWork.GetRepository<CargosProjetosSmmapeamento, long>().Add(positionMappingCreated);
            _unitOfWork.GetRepository<CargosProjetosSmmapeamento, long>().Update(positionMappings);
            await _unitOfWork.CommitAsync();
            return Unit.Value;
        }
    }

}