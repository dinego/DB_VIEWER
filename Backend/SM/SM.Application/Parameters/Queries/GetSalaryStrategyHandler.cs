using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Extensions;
using SM.Application.Interactors.Interfaces;
using System.Globalization;
using SM.Domain.Helpers;
using CMC.Common.Repositories.Extensions;

namespace SM.Application.Parameters.Queries
{
    public class GetSalaryStrategyRequest : IRequest<GetSalaryStrategyResponse>
    {
        public long UserId { get; set; }
        public long ProjectId { get; set; }
        public bool IsAdmin { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public bool? IsAsc { get; set; } = null;
        public int? SortColumnId { get; set; } = null;
        public long TableId { get; set; }
    }

    public class GetSalaryStrategyResponse
    {
        public IEnumerable<HeaderInfoPositionSalaryStrategy> Header { get; set; }
        public List<List<DataBodyPositionSalaryStrategy>> Body { get; set; }
        public int NextPage { get; set; }
    }

    public class HeaderInfoPositionSalaryStrategy
    {
        public int ColPos { get; set; }
        public string ColName { get; set; }
        public int ColumnId { get; set; } = 0;
        public bool Sortable { get; set; } = false;
        public string @Type { get; set; }
    }

    public class DataBodyPositionSalaryStrategy
    {
        public int ColPos { get; set; }
        public string Value { get; set; }
        public string @Type { get; set; }
        public long GroupId { get; set; }
        public long TrackId { get; set; }
    }
    public class SalaryStrategyDTO
    {
        public string Company { get; set; }
        public long CompanyId { get; set; }
        public string Group { get; set; }
        public long GroupId { get; set; }
        public string Table { get; set; }
        public long TableId { get; set; }
        public string Panel { get; set; }
        public double ReferenceMedian { get; set; }
        public long MinTrack { get; set; }
        public long MaxTrack { get; set; }
        public IEnumerable<DataSalaryTableGetSalaryStrategyHandler> Percentagens { get; set; }
    }

    public class DataSalaryTableGetSalaryStrategyHandler
    {
        public double Multiplicator { get; set; }
        public string Value { get; set; }
        public long TrackId { get; set; }
        public long GroupId { get; set; }

    }
    public class GetSalaryStrategyHandler
        : IRequestHandler<GetSalaryStrategyRequest, GetSalaryStrategyResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IEnumerable<SalaryStrategyColumnEnum> _listEnums;

        public GetSalaryStrategyHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _listEnums = Enum.GetValues(typeof(SalaryStrategyColumnEnum)) as
                IEnumerable<SalaryStrategyColumnEnum>;

            _listEnums = _listEnums.First().GetWithOrder().Select(s =>
                        (SalaryStrategyColumnEnum)Enum.Parse(typeof(SalaryStrategyColumnEnum), s));
        }
        public async Task<GetSalaryStrategyResponse> Handle(GetSalaryStrategyRequest request, CancellationToken cancellationToken)
        {
            var groupMaps = await GetData(request);
            if (!groupMaps.Any())
                return new GetSalaryStrategyResponse();

            List<HeaderInfoPositionSalaryStrategy> header =
                GetHeader(groupMaps.FirstOrDefault().Percentagens);

            List<List<DataBodyPositionSalaryStrategy>> body = GetBody(groupMaps);

            return new GetSalaryStrategyResponse
            {
                Body = body,
                Header = header,
            };
        }

        private async Task<List<SalaryStrategyDTO>> GetData(GetSalaryStrategyRequest request)
        {
            var isDesc = request.IsAsc.HasValue ? !request.IsAsc.Value : false;
            var sortColumnProperty = request.SortColumnId.HasValue ?
                _listEnums.FirstOrDefault(f => (int)f == request.SortColumnId.Value).ToString() :
                SalaryStrategyColumnEnum.Company.ToString();

            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);
            var groupsExp =
                        permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                        .SubItems;

            var tablesExp =
                permissionUser.Contents?
                .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                .SubItems;

            var groupMaps = await _unitOfWork.GetRepository<GruposProjetosSalaryMarkMapeamento, long>()
                               .Include("Empresa")
                               .Include("GruposProjetosSalaryMark")
                               .Include("Painel")
                               .GetListAsync(g => g.Where(w => w.TabelaSalarialIdLocal == request.TableId &&
                                request.CompaniesId.Contains(w.EmpresaId) &&
                                w.ProjetoId == request.ProjectId &&
                                (!groupsExp.Safe().Any() || !groupsExp.Contains(w.GrupoProjetoSmidLocal)) &&
                                (!tablesExp.Safe().Any() || !tablesExp.Contains(w.TabelaSalarialIdLocal)))?
                               .Select(s => new SalaryStrategyDTO
                               {
                                   Company = s.Empresa == null ? string.Empty : string.IsNullOrWhiteSpace(s.Empresa.NomeFantasia) ?
                                   (string.IsNullOrWhiteSpace(s.Empresa.RazaoSocial) ? string.Empty : s.Empresa.RazaoSocial) : s.Empresa.NomeFantasia,
                                   CompanyId = s.EmpresaId,
                                   Group = s.GruposProjetosSalaryMark.GrupoSm,
                                   GroupId = s.GrupoProjetoSmidLocal,
                                   Panel = s.Painel.Titulo,
                                   ReferenceMedian = s.ReferenciaMediana.GetValueOrDefault(0),
                                   TableId = s.TabelaSalarialIdLocal,
                                   MaxTrack = s.FaixaIdSuperior,
                                   MinTrack = s.FaixaIdInferior
                               }).OrderBy(sortColumnProperty, isDesc));

            if (!groupMaps.Any())
                return new List<SalaryStrategyDTO>();

            var listTableIds = groupMaps?.Select(s => s.TableId).Distinct();

            var salaryTableResult = await _unitOfWork.GetRepository<TabelasSalariais, long>()
                .Include("TabelasSalariaisFaixas")
                .Include("TabelasSalariaisValores")
                .GetListAsync(x => x.Where(ts => listTableIds.Contains(ts.TabelaSalarialIdLocal) &&
                 ts.ProjetoId == request.ProjectId)?
                .Select(s => new
                {
                    TableId = s.TabelaSalarialIdLocal,
                    Tracks = s.TabelasSalariaisFaixas
                    .Select(fm => new { FactorMulti = fm.AmplitudeMidPoint, TrackId = fm.FaixaSalarialId }),
                    MidPoint = s.TabelasSalariaisValores.Select(mp => new
                    {
                        MidPoint = mp.FaixaMidPoint,
                        GSM = mp.Grade,
                    })
                }));

            if (!salaryTableResult.Any())
                return new List<SalaryStrategyDTO>();

            //TabelasSalariaisParametrosPoliticas
            var groupsIds = groupMaps.Select(s => s.GroupId).Distinct();
            var tracksIds = salaryTableResult.SelectMany(s => s.Tracks).Select(s => s.TrackId).Distinct();

            var labels = await _unitOfWork.GetRepository<TabelasSalariaisParametrosPolitica, long>()
                .GetListAsync(g => g.Where(tpp => tpp.ProjetoId == request.ProjectId &&
                 groupsIds.Contains(tpp.GrupoProjetoMidLocal) && tracksIds.Contains(tpp.FaixaSalarialId))?
                .Select(s => new
                {
                    GroupId = s.GrupoProjetoMidLocal,
                    TrackId = s.FaixaSalarialId,
                    Label = s.RotuloPolitica
                }));

            var tracks = salaryTableResult
                 .OrderByDescending(o => o.Tracks.Count())
                 .Take(1)
                 .SelectMany(s => s.Tracks)
                 .OrderBy(o => o.TrackId);

            foreach (var groupMap in groupMaps)
            {
                var companyId = groupMap.CompanyId;
                var groupId = groupMap.GroupId;

                var tableId = groupMap.TableId;
                var maxTrackId = groupMap.MaxTrack;
                var minTrackId = groupMap.MinTrack;

                var salaryTable =
                        salaryTableResult.Where(st => tableId == st.TableId);

                if (salaryTable != null)
                {
                    var listSalaryTable = new List<DataSalaryTableGetSalaryStrategyHandler>();

                    foreach (var track in tracks)
                    {
                        if (track.TrackId < minTrackId || track.TrackId > maxTrackId)
                        {
                            listSalaryTable.Add(new DataSalaryTableGetSalaryStrategyHandler
                            {
                                Value = string.Empty,
                                Multiplicator = track.FactorMulti,
                                TrackId = track.TrackId,
                                GroupId = groupId
                            });
                            continue;
                        }

                        var fFM = salaryTable
                        .SelectMany(a => a.Tracks)
                        .Where(s => s.TrackId == track.TrackId)?
                        .Average(a => a.FactorMulti);

                        if (fFM.HasValue)
                        {
                            var value = labels.FirstOrDefault(f => f.GroupId == groupId &&
                                            f.TrackId == track.TrackId);

                            listSalaryTable.Add(new DataSalaryTableGetSalaryStrategyHandler
                            {
                                Value = value == null ? string.Empty : value.Label,
                                Multiplicator = fFM.Value,
                                TrackId = track.TrackId,
                                GroupId = groupId
                            });
                        }
                        else
                        {
                            listSalaryTable.Add(new DataSalaryTableGetSalaryStrategyHandler
                            {
                                Value = string.Empty,
                                Multiplicator = track.FactorMulti,
                                TrackId = track.TrackId,
                                GroupId = groupId
                            });
                        }
                    }

                    groupMap.Percentagens = listSalaryTable;
                }
            }

            return groupMaps;
        }

        private List<List<DataBodyPositionSalaryStrategy>> GetBody(List<SalaryStrategyDTO> groupMaps)
        {
            var body = new List<List<DataBodyPositionSalaryStrategy>>();

            foreach (var groupMap in groupMaps)
            {
                var columns = new List<DataBodyPositionSalaryStrategy>();

                int countColumnsBody = 0;

                foreach (SalaryStrategyColumnEnum item in _listEnums)
                {
                    switch (item)
                    {
                        case SalaryStrategyColumnEnum.Percentagens:
                            foreach (var percentage in groupMap.Percentagens)
                            {
                                columns.Add(new DataBodyPositionSalaryStrategy
                                {
                                    ColPos = countColumnsBody,
                                    Value = percentage.Value,
                                    Type = percentage.Value.GetType().Name,
                                    TrackId = percentage.TrackId,
                                    GroupId = percentage.GroupId
                                });
                                countColumnsBody++;
                            }
                            continue;

                        case SalaryStrategyColumnEnum.ReferenceMedian:

                            var valueReferenceMedian = Convert.ToDouble(groupMap
                                .GetType()
                                .GetProperty(item.ToString())
                                .GetValue(groupMap));

                            columns.Add(new DataBodyPositionSalaryStrategy
                            {
                                ColPos = countColumnsBody,
                                Value = Math.Round(valueReferenceMedian * 100, 0).ToString(CultureInfo.InvariantCulture),
                                Type = valueReferenceMedian.GetType().Name,
                            });

                            break;
                        default:
                            var value =
                                    groupMap.GetType().GetProperty(item.ToString());
                            columns.Add(new DataBodyPositionSalaryStrategy
                            {
                                ColPos = countColumnsBody,
                                Value = value == null ? string.Empty :
                                (value.GetValue(groupMap) == null ? string.Empty : value.GetValue(groupMap).ToString()),
                                Type = value == null ? typeof(string).Name :
                                (value.GetValue(groupMap) == null ? typeof(string).Name : value.GetValue(groupMap).GetType().Name),
                            });
                            break;
                    }
                    countColumnsBody++;
                }

                body.Add(columns);
            }

            return body;
        }

        private List<HeaderInfoPositionSalaryStrategy> GetHeader(IEnumerable<DataSalaryTableGetSalaryStrategyHandler> tracks)
        {
            var header = new List<HeaderInfoPositionSalaryStrategy>();
            int countColumnsHeader = 0;
            foreach (SalaryStrategyColumnEnum item in _listEnums)
            {

                if (item == SalaryStrategyColumnEnum.Percentagens)
                {
                    foreach (var track in tracks)
                    {
                        header.Add(new HeaderInfoPositionSalaryStrategy
                        {
                            Type = typeof(string).Name,
                            ColName = $"{Math.Round(track.Multiplicator * 100, 0)}%",
                            ColPos = countColumnsHeader
                        });

                        countColumnsHeader++;
                    }
                    continue;
                }

                header.Add(new HeaderInfoPositionSalaryStrategy
                {
                    Type = typeof(string).Name,
                    ColName = item.GetDescription(),
                    ColPos = countColumnsHeader,
                    ColumnId = (int)item,
                    Sortable = true
                });

                countColumnsHeader++;
            }

            return header;
        }

    }
}
