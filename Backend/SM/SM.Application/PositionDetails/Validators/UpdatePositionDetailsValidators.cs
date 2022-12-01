using System.Linq;
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Abstractions.Validators.Abstractions;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using SM.Application.TableSalary.Command;

namespace SM.Application.Parameters.Validators
{
    public class UpdatePositionDetailsRule : CompositeRule<UpdatePositionDetailRequest>
    {
        private ValidatorResponse _validator;
        private IUnitOfWork _unitOfWork;

        public UpdatePositionDetailsRule(ValidatorResponse validator, IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _unitOfWork = unitOfWork;
        }
        public override bool IsSatisfiedBy(UpdatePositionDetailRequest request)
        {
            bool isValid = request.CanEditListPosition;
            if (!isValid)
                _validator.AddNotification("Você não tem permissão para editar as características do cargo.");
            if (request.PositionData == null)
            {
                _validator.AddNotification("Não encontramos os dados necessários para processar sua solicitação.");
                isValid = false;
            }
            if (request.PositionData != null && string.IsNullOrWhiteSpace(request.PositionData.Position))
            {
                _validator.AddNotification("O nome do cargo é de preenchimento obrigatório.");
                isValid = false;
            }
            if (request.PositionData != null && request.PositionData.LevelId <= 0)
            {
                _validator.AddNotification("O nível é de preenchimento obrigatório.");
                isValid = false;
            }
            if (request.PositionData != null && request.PositionData.GroupId <= 0)
            {
                _validator.AddNotification("O perfil é de preenchimento obrigatório.");
                isValid = false;
            }
            if (request.PositionData != null && !request.PositionData.Parameters.Safe().Any())
            {
                _validator.AddNotification("É necessário ter pelo menos um parâmetro para o cargo.");
                isValid = false;
            }

            if (request.PositionData != null)
            {
                var position = _unitOfWork.GetRepository<CargosProjetosSm, long>()
                                        .Get(x => x.Where(cp => cp.ProjetoId == request.ProjectId &&
                                                                     cp.CargoProjetoSmidLocal != request.PositionData.PositionId &&
                                                                        request.PositionData.Position.ToLower().Equals(cp.CargoSm.ToLower())));
                if (position != null)
                {
                    _validator.AddNotification("Já existe um cargo com o nome informado.");
                    isValid = false;
                }
            }
            return isValid;
        }
    }
}
