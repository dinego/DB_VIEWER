using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using SM.Application.Interactors;
using SM.Application.Interactors.Interfaces;
using SM.Application.TableSalary.Validators;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.TableSalary.Command
{
    public class UpdateDisplayColumnsRequest : IRequest
    {
        public long UserId { get; set; }
        public IEnumerable<DisplayColumns> DisplayColumns { get; set; }
        public bool CanEditLocalLabels { get; set; }
        public bool CanEditGlobalLabels { get; set; }
    }
    public class DisplayColumns
    {
        public int ColumnId { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }

    public class UpdateDisplayColumnsHandler : IRequestHandler<UpdateDisplayColumnsRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateDisplayColumnsPemissionException> _validator;
        private readonly ValidatorResponse _validatorResponse;
        private readonly ISaveInternalLabelsInteractor _saveInternalLabelsInteractor;
        private readonly ISaveEnabledLabelsByModulesInteractor _saveEnabledLabelsByModulesInteractor;

        public UpdateDisplayColumnsHandler(IUnitOfWork unitOfWork,
            IValidator<UpdateDisplayColumnsPemissionException> validator,
            ValidatorResponse validatorResponse,
            ISaveInternalLabelsInteractor saveInternalLabelsInteractor,
            ISaveEnabledLabelsByModulesInteractor saveEnabledLabelsByModulesInteractor)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _validatorResponse = validatorResponse;
            _saveInternalLabelsInteractor = saveInternalLabelsInteractor;
            _saveEnabledLabelsByModulesInteractor = saveEnabledLabelsByModulesInteractor;
        }
        public async Task<Unit> Handle(UpdateDisplayColumnsRequest request, CancellationToken cancellationToken)
        {
            var resName = _validator.Validate(new UpdateDisplayColumnsPemissionException
            {
                ColumnIds = request.DisplayColumns.Select(s => s.ColumnId),
                UserId = request.UserId,
                CanEditGlobalLabels = request.CanEditGlobalLabels
            });

            if (!resName.IsValid)
                _validatorResponse.AddNotifications(resName.Errors.ToList());

            var enabledRequest = new EnabledLabelsByModulesRequest
            {
                CanEditLocalLabels = request.CanEditLocalLabels,
                EnabledLabels = request.DisplayColumns.Map().ToANew<List<EnabledLabels>>(),
                Module = ModulesEnum.TableSalary,
                UserId = request.UserId
            };
            var internalLabelRequest = new InternalLabelsRequest
            {
                CanEditLocalLabels = request.CanEditLocalLabels,
                InternalLabels = request.DisplayColumns.Map().ToANew<List<SaveInternalLabel>>(),
                UserId = request.UserId,
                CanEditGlobalLabels = request.CanEditGlobalLabels
            };
            await _saveEnabledLabelsByModulesInteractor.Handler(enabledRequest);
            await _saveInternalLabelsInteractor.Handler(internalLabelRequest);

            return Unit.Value;
        }
    }
}
