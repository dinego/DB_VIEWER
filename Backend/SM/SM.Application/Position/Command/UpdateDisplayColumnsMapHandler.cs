using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using SM.Application.Position.Validators;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Position.Command
{
    public class UpdateDisplayColumnsMapRequest : IRequest
    {
        public long UserId { get; set; }
        public IEnumerable<DisplayColumnsMap> DisplayColumns { get; set; }
    }
    public class DisplayColumnsMap
    {
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }

    public class UpdateDisplayColumnsMapHandler : IRequestHandler<UpdateDisplayColumnsMapRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateDisplayColumnsMapPemissionException> _validator;
        private readonly ValidatorResponse _validatorResponse;

        public UpdateDisplayColumnsMapHandler(IUnitOfWork unitOfWork,
            IValidator<UpdateDisplayColumnsMapPemissionException> validator,
            ValidatorResponse validatorResponse)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _validatorResponse = validatorResponse;
        }
        public async Task<Unit> Handle(UpdateDisplayColumnsMapRequest request, CancellationToken cancellationToken)
        {
            var resName = _validator.Validate(new UpdateDisplayColumnsMapPemissionException
            {
                UserId = request.UserId
            });

            if (!resName.IsValid)
                _validatorResponse.AddNotifications(resName.Errors.ToList());


            //get all displayColumns by user
            var allDisplayColumns = await _unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
                .GetListAsync(x => x.Where(use => use.UsuarioId == request.UserId &&
                                                  use.ModuloSmid == (long)ModulesEnum.Position &&
                                                  use.ModuloSmsubItemId.HasValue &&
                                                  use.ModuloSmsubItemId.Value == (long)ModulesSuItemsEnum.Map));

            foreach (var displayColumn in request.DisplayColumns)
            {
                var value = allDisplayColumns?.FirstOrDefault(f => f.Nome == displayColumn.Name);
                //update 
                if (value != null)
                {
                    value.ColunaId = 0;
                    value.Nome = displayColumn.Name;
                    value.Checado = displayColumn.IsChecked;
                    continue;
                }

                //add table
                _unitOfWork.GetRepository<UsuarioExibicaoSm, long>().Add(new UsuarioExibicaoSm
                {
                    UsuarioId = request.UserId,
                    Checado = displayColumn.IsChecked,
                    ColunaId = 0,
                    ModuloSmid = (long)ModulesEnum.Position,
                    ModuloSmsubItemId = (long)ModulesSuItemsEnum.Map,
                    Nome = displayColumn.Name
                });
            }

            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
