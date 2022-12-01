using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Positioning.Command
{
    public class UpdateDisplayColumnsFrameworkRequest : IRequest
    {
        public long UserId { get; set; }
        public IEnumerable<DisplayColumnsFramework> DisplayColumns { get; set; }
    }
    public class DisplayColumnsFramework
    {
        public int ColumnId { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }

    public class UpdateDisplayColumnsFrameworkHandler : IRequestHandler<UpdateDisplayColumnsFrameworkRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;

        public UpdateDisplayColumnsFrameworkHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
        }
        public async Task<Unit> Handle(UpdateDisplayColumnsFrameworkRequest request, CancellationToken cancellationToken)
        {

            //get all displayColumns by user
            var allDisplayColumns = await _unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
                .GetListAsync(x => x.Where(use => use.UsuarioId == request.UserId &&
                                                  use.ModuloSmid == (long)ModulesEnum.Positioning &&
                                                  use.ModuloSmsubItemId.HasValue &&
                                                  use.ModuloSmsubItemId.Value == (long)ModulesSuItemsEnum.Framework));

            bool canEditColumns = await CanEditColumns(request);

            var listEnum = Enum.GetValues(typeof(FrameworkColumnsMainEnum)) as
            IEnumerable<FrameworkColumnsMainEnum>;

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
                    ModuloSmid = (long)ModulesEnum.Positioning,
                    ModuloSmsubItemId = (long)ModulesSuItemsEnum.Framework,
                    Nome = canEditColumns ? displayColumn.Name : listEnum.FirstOrDefault(f => (int)f == displayColumn.ColumnId).GetDescription()
                });
            }

            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }

        private async Task<bool> CanEditColumns(UpdateDisplayColumnsFrameworkRequest request)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            var resultPermission = permissionUser.Permission?.FirstOrDefault(p => p.Id == (long)PermissionItensEnum.RenameColumn);

            return resultPermission == null;
        }
    }
}
