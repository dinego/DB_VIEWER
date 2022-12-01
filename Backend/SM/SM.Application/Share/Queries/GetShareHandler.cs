using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using Newtonsoft.Json;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;

namespace SM.Application.Share.Queries
{

    public class GetShareRequest : IRequest<GetShareResponse>
    {
        public string SecretKey { get; set; }
    }

    public class GetShareResponse
    {
        public string Parameters { get; set; }
        public string UserShared { get; set; }
        public long UserId { get; set; }
        public DateTime DateShared { get; set; }
        public IEnumerable<object> ColumnsExcluded { get; set; } = new List<object>();
        public IEnumerable<long> CompaniesId { get; set; } = new List<long>();
        public long ProjectId { get; set; }
    }

    public class PermissionShared
    {
        public bool CanFilterTypeofContract { get; set; }
        public bool CanFilterMM { get; set; }
        public bool CanFilterMI { get; set; }
        public bool CanFilterOccupants { get; set; }
        public bool CanDownloadExcel { get; set; }
        public bool CanRenameColumns { get; set; }
        public bool CanShare { get; set; }
        public bool CanEditLevels { get; set; }
        public bool CanEditSalaryStrategy { get; set; }
        public bool CanEditHourlyBasis { get; set; }
        public bool CanEditConfigPJ { get; set; }
        public bool CanEditUser { get; set; }
        public bool CanEditGlobalLabels { get; set; }
        public bool CanEditLocalLabels { get; set; }
        public bool InactivePerson { get; set; }
        public bool CanDisplayEmployeeName { get; set; }
        public bool CanDisplayBossName { get; set; }
        public bool CanEditProjectSalaryTablesValues { get; set; }
        public bool CanChooseDefaultParameter { get; set; }
        public bool CanMoveNextStep { get; set; }
        public bool CanAddPosition { get; set; }
        public bool CanEditPosition { get; set; }
        public bool CanDeletePosition { get; set; }
        public bool CanEditListPosition { get; set; }
        public bool CanEditGSMMappingTable { get; set; }
        public bool CanEditSalaryTableValues { get; set; }
        public bool CanAddPeople { get; set; }
        public bool CanDeletePeople { get; set; }
        public bool CanEditPeople { get; set; }
        public bool CanEditMappingPositionSM { get; set; }

    }
    public class GetShareHandler : IRequestHandler<GetShareRequest, GetShareResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly ValidatorResponse _validatorResponse;

        public GetShareHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor,
            ValidatorResponse validatorResponse)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _validatorResponse = validatorResponse;
        }
        public async Task<GetShareResponse> Handle(GetShareRequest request, CancellationToken cancellationToken)
        {
            var shareData = await _unitOfWork.GetRepository<CompartilharSm, long>()
                                             .Include("Usuario")
                            .GetAsync(g => g.Where(c => c.ChaveSecreta.Equals(request.SecretKey)));

            if (shareData == null)
            {
                _validatorResponse.AddNotification("Não encontramos os dados compartilhados.");
                return null;
            }
            var userPermission = await _permissionUserInteractor.Handler(shareData.UsuarioId);
            if (userPermission == null)
            {
                _validatorResponse.AddNotification("Não encontramos as regras necessárias para exibir os dados compartilhados.");
                return null;
            }

            var sharePermission = userPermission.Permission.FirstOrDefault(p => p.Id == (long)PermissionItensEnum.Share);
            if (sharePermission == null ||
               !sharePermission.IsChecked ||
               !sharePermission.ExpireLinkDays.HasValue ||
               shareData.DataDeCompartilhamento.AddDays(sharePermission.ExpireLinkDays.Value) < DateTime.Today)
            {
                _validatorResponse.AddNotification("Os dados compartilhados não está mais disponível.");
                return null;
            }

            return new GetShareResponse
            {
                Parameters = shareData.Parametros,
                UserShared = shareData.Usuario.Nome,
                UserId = shareData.UsuarioId,
                DateShared = shareData.DataDeCompartilhamento,
                ColumnsExcluded = JsonConvert.DeserializeObject<IEnumerable<object>>(shareData.Colunas),
                CompaniesId = JsonConvert.DeserializeObject<IEnumerable<long>>(shareData.EmpresasId),
                ProjectId = shareData.ProjetoId
            };
        }
    }
}
