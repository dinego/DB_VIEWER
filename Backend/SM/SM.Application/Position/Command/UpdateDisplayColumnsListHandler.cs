using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Application.Position.Validators;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Position.Command
{
    public class UpdateDisplayColumnsListRequest : IRequest
    {
        public long UserId { get; set; }
        public IEnumerable<DisplayColumnsList> DisplayColumns { get; set; }
    }
    public class DisplayColumnsList
    {
        public int ColumnId { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }

    public class UpdateDisplayColumnsListHandler : IRequestHandler<UpdateDisplayColumnsListRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateDisplayColumnsListPemissionException> _validator;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly ValidatorResponse _validatorResponse;

        public UpdateDisplayColumnsListHandler(IUnitOfWork unitOfWork,
            IValidator<UpdateDisplayColumnsListPemissionException> validator,
            IPermissionUserInteractor permissionUserInteractor,
            ValidatorResponse validatorResponse)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _permissionUserInteractor = permissionUserInteractor;
            _validatorResponse = validatorResponse;
        }
        public async Task<Unit> Handle(UpdateDisplayColumnsListRequest request, CancellationToken cancellationToken)
        {
            var resName = _validator.Validate(new UpdateDisplayColumnsListPemissionException
            {
                ColumnIds = request.DisplayColumns.Select(s => s.ColumnId),
                UserId = request.UserId
            });

            if (!resName.IsValid)
                _validatorResponse.AddNotifications(resName.Errors.ToList());


            //get all displayColumns by user
            var allDisplayColumns = await _unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
                .GetListAsync(x => x.Where(use => use.UsuarioId == request.UserId &&
                                                  use.ModuloSmid == (long)ModulesEnum.Position &&
                                                  use.ModuloSmsubItemId.HasValue &&
                                                  use.ModuloSmsubItemId.Value == (long)ModulesSuItemsEnum.Architecture));

            bool canEditColumns = await CanEditColumns(request);

            foreach (var displayColumn in request.DisplayColumns)
            {
                var value = allDisplayColumns?.FirstOrDefault(f => f.ColunaId == displayColumn.ColumnId);
                //update 
                if (value != null)
                {
                    value.ColunaId = displayColumn.ColumnId;
                    if (canEditColumns) value.Nome = displayColumn.Name;
                    value.Checado = displayColumn.IsChecked;
                    continue;
                }

                //add table
                _unitOfWork.GetRepository<UsuarioExibicaoSm, long>().Add(new UsuarioExibicaoSm
                {
                    UsuarioId = request.UserId,
                    Checado = displayColumn.IsChecked,
                    ColunaId = displayColumn.ColumnId,
                    ModuloSmid = (long)ModulesEnum.Position,
                    ModuloSmsubItemId = (long)ModulesSuItemsEnum.Architecture,
                    Nome = canEditColumns ? displayColumn.Name : DefaultColumnName(displayColumn.ColumnId)
                });
            }

            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }

        private async Task<bool> CanEditColumns(UpdateDisplayColumnsListRequest request)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            var resultPermission = permissionUser.Permission?.FirstOrDefault(p => p.Id == (long)PermissionItensEnum.RenameColumn);

            return resultPermission == null;
        }

        private string DefaultColumnName(int ColumnId)
        {

            var listListPositionColumns = (Enum.GetValues(typeof(PositionProjectColumnsEnum)) as
                IEnumerable<PositionProjectColumnsEnum>).Select(s => new { Id = (int)s, Value = s.GetDescription() });

            return listListPositionColumns.FirstOrDefault(f => f.Id == ColumnId).Value;
        }
    }
}
