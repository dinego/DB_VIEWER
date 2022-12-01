using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.GetAllContractTypes
{
    public class GetContractTypesPjSettingsRequest : IRequest<GetContractTypesPjSettingsResponse>
    {
        public long UserId { get; set; }
    }

    public class ContractTypesPjSettingsResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
    }
    public class GetContractTypesPjSettingsResponse
    {
        public IEnumerable<ContractTypesPjSettingsResponse> ContractTypesResponse { get; set; }
    }

    public class GetContractTypesPjSettingsHandler : IRequestHandler<GetContractTypesPjSettingsRequest, GetContractTypesPjSettingsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetContractTypesPjSettingsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GetContractTypesPjSettingsResponse> Handle(GetContractTypesPjSettingsRequest request, CancellationToken cancellationToken)
        {
            var contractUser = await _unitOfWork.GetRepository<UsuarioPermissaoSm, long>()
                                    .GetAsync(x => x.Where(u => u.UsuarioId == request.UserId)
                                               .Select(x => x.Permissao)
                                                 );

            if (contractUser == null)
                return new GetContractTypesPjSettingsResponse();

            object userContractTypeObj = JsonConvert.DeserializeObject(contractUser);
            JObject obj = JObject.Parse(userContractTypeObj.ToString());
            IList<JToken> jItens = obj["TypeOfContract"].Children().ToList();
            List<long> listInteger = new List<long>();
            if (jItens.Count > 0)
            {
                for (int i = 0; i < jItens.Count; i++)
                {
                    listInteger.Add((long)jItens[i]);
                }
            }

            var contractTypeUser = await _unitOfWork.GetRepository<TipoDeContrato, long>()
                                   .GetListAsync(x => x.Where(u => !listInteger.Contains(u.Id) && u.Id != (long)ContractTypeEnum.CLT
                                                                    && u.Ativo.HasValue && u.Ativo.Value)
                                              .Select(x => new ContractTypesPjSettingsResponse
                                              {
                                                  Id = x.Id,
                                                  Title = x.Nome
                                              })
                                            );

            return new GetContractTypesPjSettingsResponse() { ContractTypesResponse = contractTypeUser };
        }
    }
}

