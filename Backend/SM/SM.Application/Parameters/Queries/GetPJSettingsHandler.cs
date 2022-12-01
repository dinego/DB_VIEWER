using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Parameters.Queries
{

    public class GetPJSettingsRequest
        : IRequest<GetPJSettingsResponse>
    {
        public long ContractTypeId { get; set; }
        public long ProjectId { get; set; }
        public bool IsAdmin { get; set; }
        public long UserId { get; set; }
    }

    public class GetPJSettingsResponse
    {
        public double ContractTypePercentageTotal { get; set; }
        public double PJSettingsPercentageTotal { get; set; }
        public IEnumerable<GetPJSettingsItemsResponse> Items { get; set; }

    }

    public class GetPJSettingsItemsResponse
    {
        public string Name { get; set; }
        public long PJSettingsId { get; set; }
        public long ItemTypeId { get; set; }
        public double ContractTypePercentage { get; set; }
        public double PJSettingsPercentage { get; set; }
        public IEnumerable<GetPJSettingsSubItemResponse> SubItems { get; set; }
    }

    public class GetPJSettingsSubItemResponse
    {
        public string Name { get; set; }
        public double ContractTypePercentage { get; set; }

    }
    public class GetPJSettingsHandler
        : IRequestHandler<GetPJSettingsRequest, GetPJSettingsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPJSettingsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GetPJSettingsResponse> Handle(GetPJSettingsRequest request, CancellationToken cancellationToken)
        {
            var typeContractItems = await _unitOfWork.GetRepository<ItensTipoDeContrato, long>()
                .Include("SubItensTipoDeContrato")
                .Include("ConfiguracoesSistemaPj")
                .GetListAsync(x =>
                x.Where(cs => cs.Ativo.HasValue && cs.Ativo.Value &&
                           cs.SubItensTipoDeContrato.Any(sbi=> sbi.Ativo.HasValue && sbi.Ativo.Value && sbi.TipodeContratoId == request.ContractTypeId))
                .Select(res=> new
                {
                    ItemId = res.Id,
                    ItemName = res.Nome,
                    SubItems = res.SubItensTipoDeContrato.Select(sb=> new
                    {
                        sb.Id,
                        ItemId = sb.ItemTipoDeContratoId,
                        ContractTypeId = sb.TipodeContratoId,
                        Name = sb.Nome,
                        Value = sb.Valor
                    })
                }));

            if(typeContractItems == null)
                return new GetPJSettingsResponse();

            var settingsPj = await _unitOfWork.GetRepository<ConfiguracoesSistemaPj, long>()
                            .GetListAsync(x =>
                             x.Where(cs => cs.ProjetoId == request.ProjectId &&
                             cs.Ativo.HasValue && cs.Ativo.Value &&
                             cs.TipoDeContratoId == request.ContractTypeId &&
                             cs.ItemTipodeContrato.Ativo.HasValue &&
                             cs.ItemTipodeContrato.Ativo.Value)
             .Select(s => new
             {
                 s.Id,
                 ContractTypeId = s.TipoDeContratoId,
                 ItemTypeContractId = s.ItemTipodeContratoId,
                 Value = s.Valor
             }));

            var result = new List<GetPJSettingsItemsResponse>();
            foreach (var item in typeContractItems)
            {
                var subItems = item.SubItems.
                    Where(si => si.ItemId == item.ItemId && si.ContractTypeId == request.ContractTypeId)
                    .Select(s => new GetPJSettingsSubItemResponse
                    {
                        Name = s.Name,
                        ContractTypePercentage = Math.Round(s.Value * 100,0)
                    });

                var pjItem = settingsPj.FirstOrDefault(x => x.ItemTypeContractId == item.ItemId && x.ContractTypeId == request.ContractTypeId);

                result.Add(new GetPJSettingsItemsResponse
                {
                    ItemTypeId = item.ItemId,
                    PJSettingsPercentage = pjItem != null ? Math.Round(pjItem.Value * 100,0) : 0,
                    Name = item.ItemName,
                    PJSettingsId = pjItem != null ? pjItem.Id : 0,
                    ContractTypePercentage = subItems.Sum(s => s.ContractTypePercentage),
                    SubItems = subItems
                });
            }


            return new GetPJSettingsResponse
            {
                ContractTypePercentageTotal = result.Sum(s => s.ContractTypePercentage),
                PJSettingsPercentageTotal = result.Sum(s => s.PJSettingsPercentage),
                Items = result
            };
        }
    }
}
