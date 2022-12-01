using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Application.Interactors
{
    public class EnabledLabelsByModulesRequest
    {
        public long UserId { get; set; }
        public List<EnabledLabels> EnabledLabels { get; set; }
        public ModulesEnum Module { get; set; }
        public ModulesSuItemsEnum? SubModule { get; set; }
        public bool CanEditLocalLabels { get; set; }
    }
    public class EnabledLabels
    {
        public int ColumnId { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
    public class SaveEnabledLabelsByModulesInteractor : ISaveEnabledLabelsByModulesInteractor
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaveEnabledLabelsByModulesInteractor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Handler(EnabledLabelsByModulesRequest request)
        {
            if (!request.EnabledLabels.Safe().Any() ||
                !request.CanEditLocalLabels) return;

            var userEnabledLabel = await _unitOfWork.GetRepository<UsuarioRotuloHabilitadosSM, long>()
                .GetListAsync(x => x.Where(up => up.UsuarioId == request.UserId && up.ModuloSMId == (long)request.Module));

            var enabledUserLabels = new List<UsuarioRotuloHabilitadosSM>();
            request.EnabledLabels.ForEach(il =>
            {
                var userLabelSaved = userEnabledLabel.FirstOrDefault(ul => ul.CodigoInternoColuna == (int)il.ColumnId);
                if (userLabelSaved == null)
                {
                    var newLabel = new UsuarioRotuloHabilitadosSM
                    {
                        CodigoInternoColuna = (int)il.ColumnId,
                        UsuarioId = request.UserId,
                        ModuloSMId = (long)request.Module,
                        ModuloSMSubItemId = (long?)request.SubModule,
                        Habilitado = il.IsChecked
                    };
                    enabledUserLabels.Add(newLabel);
                    return;
                }
                userLabelSaved.Habilitado = il.IsChecked;
            });
            _unitOfWork.GetRepository<UsuarioRotuloHabilitadosSM, long>().Add(enabledUserLabels);
            _unitOfWork.GetRepository<UsuarioRotuloHabilitadosSM, long>().Update(userEnabledLabel);
            await _unitOfWork.CommitAsync();
        }
    }
}
