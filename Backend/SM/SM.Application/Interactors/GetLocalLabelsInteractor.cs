using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Application.Interactors
{
    public class LocalLabelsRequest
    {
        public long UserId { get; set; }
        public ModulesEnum Module { get; set; }
        public ModulesSuItemsEnum? SubModules { get; set; }
    }

    public class LocalLabelsResponse
    {
        public int InternalCode { get; set; }
        public string Label { get; set; }
        public bool IsChecked { get; set; }
    }
    public class GetLocalLabelsInteractor : IGetLocalLabelsInteractor
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLocalLabelsInteractor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<LocalLabelsResponse>> Handler(LocalLabelsRequest request)
        {
            var result = new List<LocalLabelsResponse>();

            var userEnabledLabels = await _unitOfWork.GetRepository<UsuarioRotuloHabilitadosSM, long>()
                             .GetListAsync(x => x.Where(s => s.UsuarioId == request.UserId &&
                                                s.ModuloSMId == (long)request.Module &&
                                                (!request.SubModules.HasValue ||
                                                 s.ModuloSMSubItemId == (long)request.SubModules))
                             .Select(res => new
                             {
                                 res.CodigoInternoColuna,
                                 res.Habilitado
                             }));
            if (userEnabledLabels == null) return result;
            var userLabels = await _unitOfWork.GetRepository<UsuarioRotulosSM, long>()
                             .GetListAsync(x => x.Where(s => s.UsuarioId == request.UserId)
                             .Select(res => new
                             {
                                 res.CodigoInternoRoluto,
                                 res.NovoRotulo
                             }));

            userEnabledLabels.ForEach(usl =>
            {
                var userLabel = userLabels.FirstOrDefault(us => us.CodigoInternoRoluto == usl.CodigoInternoColuna);

                var localLabel = new LocalLabelsResponse
                {
                    InternalCode = usl.CodigoInternoColuna,
                    Label = userLabel != null ? userLabel.NovoRotulo : string.Empty,
                    IsChecked = usl.Habilitado
                };
                result.Add(localLabel);
            });
            return result;
        }
    }
}
