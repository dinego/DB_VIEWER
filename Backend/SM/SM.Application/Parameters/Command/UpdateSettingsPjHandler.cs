using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Parameters.Command
{

    public class UpdateSettingsPjRequest : IRequest
    {
        public long UserId { get; set; }
        public bool IsAdmin { get; set; }
        public long ProjectId { get; set; }
        public long ContractTypeId { get; set; }
        public IEnumerable<SettingsPjDataRequest> Data { get; set; }
    }


    public class UpdateSettingsPjViewModel
    {
        public long ContractTypeId { get; set; }
        public IEnumerable<SettingsPjDataRequest> Data { get; set; }

    }
    public class SettingsPjDataRequest
    {
        public long ItemTypeId { get; set; }
        public long PJSettingsId { get; set; }
        public double Percentage { get; set; }

    }
    public class UpdateSettingsPjHandler
        : IRequestHandler<UpdateSettingsPjRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSettingsPjHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateSettingsPjRequest request,
            CancellationToken cancellationToken)
        {
            PreparaDataToSave(request);
            await PreparaDataToUpdate(request);
            await _unitOfWork.CommitAsync();
            return Unit.Value;
        }

        private void PreparaDataToSave(UpdateSettingsPjRequest request)
        {
            var newSettings = request.Data.Where(s => s.PJSettingsId <= 0 && s.ItemTypeId > 0).ToList();
            if (newSettings.Count <= 0)
                return;

            var settingsToSave = new List<ConfiguracoesSistemaPj>();
            foreach (var item in newSettings)
            {
                var newItem = new ConfiguracoesSistemaPj
                {
                    ProjetoId = request.ProjectId,
                    ItemTipodeContratoId = item.ItemTypeId,
                    Valor = Math.Round(item.Percentage / 100, 2),
                    TipoDeContratoId = request.ContractTypeId,
                    Ativo = true
                };
                settingsToSave.Add(newItem);
            }
            _unitOfWork.GetRepository<ConfiguracoesSistemaPj, long>()
                .Add(settingsToSave);
        }

        private async Task PreparaDataToUpdate(UpdateSettingsPjRequest request)
        {
            var updateSettings = request.Data.Where(s => s.PJSettingsId > 0 && s.ItemTypeId > 0).ToList();
            var pJSettingsIds = updateSettings.Select(s => s.PJSettingsId);

            var dataBaseSettings = await _unitOfWork.GetRepository<ConfiguracoesSistemaPj, long>()
                .GetListAsync(x => x.Where(cs => pJSettingsIds.Contains(cs.Id) &&
                 cs.Ativo.HasValue &&
                 cs.Ativo.Value &&
                 cs.ProjetoId == request.ProjectId &&
                 cs.TipoDeContratoId == request.ContractTypeId
                ));
            foreach (var item in dataBaseSettings)
            {
                var pjSet = updateSettings.FirstOrDefault(s => s.PJSettingsId == item.Id);
                if (pjSet != null)
                    item.Valor = Math.Round(pjSet.Percentage / 100, 2);
            }

            _unitOfWork.GetRepository<ConfiguracoesSistemaPj, long>()
                .Update(dataBaseSettings);
        }
    }
}
