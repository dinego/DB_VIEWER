using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using Newtonsoft.Json;
using SM.Application.Interactors.Utils;
using SM.Domain.Common;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.GetAllProfiles
{
    public class GetAllProfilesRequest : IRequest<GetAllProfilesResponse>
    {
        public long UserId { get; set; }
        public long ProjectId { get; set; }
        public long[] Units { get; set; }
        public long? TableId { get; set; }
        public long? UnitId { get; set; }
    }

    public class ProfilesResponse
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
    }
    public class GetAllProfilesResponse
    {
        public IEnumerable<ProfilesResponse> ProfilesResponse { get; set; }
    }

    public class GetAllProfilesHandler : IRequestHandler<GetAllProfilesRequest, GetAllProfilesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAllProfilesHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GetAllProfilesResponse> Handle(GetAllProfilesRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<ProfilesResponse> returnData = null;

            var exceptionUser = await _unitOfWork.GetRepository<UsuarioPermissaoSm, long>()
                                    .GetAsync(x => x.Where(u => u.UsuarioId == request.UserId)?
                                               .Select(x => x.Permissao));

            var contents = exceptionUser == null ?
                new List<FieldCheckedUserJson>()
                : JsonConvert.DeserializeObject<PermissionJson>(exceptionUser).Contents;


            if (!contents.Safe().Any(a => a.Id == (long)ContentsSubItemsEnum.Group && a.SubItems.Safe().Any()))
            {
                returnData = await _unitOfWork.GetRepository<GruposProjetosSalaryMark, long>()
                                          .Include("GruposProjetosSalaryMarkMapeamento")
                                          .GetListAsync(x => x.Where(u => u.ProjetoId == request.ProjectId &&
                                                      u.Ativo.HasValue && u.Ativo.Value &&
                                                      ((!request.TableId.HasValue || !request.UnitId.HasValue) ||
                                                      (u.GruposProjetosSalaryMarkMapeamento != null &&
                                                       u.GruposProjetosSalaryMarkMapeamento.Any(gpm => gpm.TabelaSalarialIdLocal == request.TableId.Value && gpm.EmpresaId == request.UnitId.Value))) &&
                                                      ((!request.TableId.HasValue || request.UnitId.HasValue) ||
                                                      (u.GruposProjetosSalaryMarkMapeamento != null &&
                                                       u.GruposProjetosSalaryMarkMapeamento.Any(gpm => gpm.TabelaSalarialIdLocal == request.TableId.Value && request.Units.Contains(gpm.EmpresaId)))))
                                          .Select(x => new ProfilesResponse
                                          {
                                              Id = x.GruposProjetosSmidLocal,
                                              Title = x.GrupoSm,
                                              Order = x.Ordem.GetValueOrDefault()
                                          }).OrderBy(o => o.Order));

                return new GetAllProfilesResponse() { ProfilesResponse = returnData };
            }

            var expGroup = contents.FirstOrDefault(f => f.Id == (long)ContentsSubItemsEnum.Group).SubItems;

            returnData = await _unitOfWork.GetRepository<GruposProjetosSalaryMark, long>()
                                   .Include("GruposProjetosSalaryMarkMapeamento")
                                   .GetListAsync(x => x.Where(u => u.ProjetoId == request.ProjectId &&
                                                                u.Ativo.HasValue && u.Ativo.Value &&
                                                               !expGroup.Contains(u.GruposProjetosSmidLocal) &&
                                                               ((!request.TableId.HasValue || !request.UnitId.HasValue) ||
                                                               (u.GruposProjetosSalaryMarkMapeamento != null &&
                                                                u.GruposProjetosSalaryMarkMapeamento.Any(gpm => gpm.TabelaSalarialIdLocal == request.TableId.Value && gpm.EmpresaId == request.UnitId.Value))) &&
                                                               ((!request.TableId.HasValue || request.UnitId.HasValue) ||
                                                               (u.GruposProjetosSalaryMarkMapeamento != null &&
                                                                u.GruposProjetosSalaryMarkMapeamento.Any(gpm => gpm.TabelaSalarialIdLocal == request.TableId.Value && request.Units.Contains(gpm.EmpresaId)))))
                                   .Select(x => new ProfilesResponse
                                   {
                                       Id = x.GruposProjetosSmidLocal,
                                       Title = x.GrupoSm,
                                       Order = x.Ordem.GetValueOrDefault()
                                   }).OrderBy(o => o.Order));
            return new GetAllProfilesResponse() { ProfilesResponse = returnData };
        }
    }
}

