using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using SM.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.DashBoard.Queries
{

    public class GetFinancialImpactCardsRequest : IRequest<IEnumerable<GetFinancialImpactCardsResponse>>
    {
        public long UserId { get; set; }
        public DisplayByPositioningEnum DisplayBy { get; set; } = DisplayByPositioningEnum.ProfileId;
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public DisplayMMMIEnum Scenario { get; set; } = DisplayMMMIEnum.MM;
        public long ProjectId { get; set; }
    }

    public class GetFinancialImpactCardsResponse
    {
        public double EmployeeAmount { get; set; } = 0;
        public double EmployeePercentage { get; set; } = 0;
        public double Cost { get; set; } = 0;
        public double CostPercentage { get; set; } = 0;
        public AnalyseFinancialImpactEnum AnalyseFinancialImpactId { get; set; }
        public string AnalyseFinancialImpactName { get; set; }
    }

    public class BaseSalaryFinancialImpactCardsDTO
    {
        public double? FinalSalarySM { get; set; }
        public int? InternalFrequency { get; set; }
        public long CompanyId { get; set; }
        public long ProfileId { get; set; }
        public long LevelId { get; set; }
        public string Area { get; set; }
        public string Parameter01 { get; set; }
        public string Parameter02 { get; set; }
        public string Parameter03 { get; set; }
        public long GSM { get; set; }
        public double IFAMin { get; set; } = 0;
        public double IFPS { get; set; } = 0;
        public double IFMP { get; set; } = 0;
        public double IFAMax { get; set; } = 0;
        public double HoursBaseSM { get; set; }
        public double HoursBase { get; set; }
        public long PositionId { get; set; }

    }

    public class PositionSalaryFinancialImpactDTO
    {
        public long PositionId { get; set; }
        public long GroupId { get; set; }
        public int GSM { get; set; }
        public string Area { get; set; }
        public string Parameter01 { get; set; }
        public string Parameter02 { get; set; }
        public string Parameter03 { get; set; }
        public double HoursBaseSM { get; set; }
    }

    public class DataSalaryTableFinancialImpact
    {
        public double Multiplicator { get; set; }
        public double Value { get; set; }
        public long TrackId { get; set; }

    }

    public class GroupXTableFinancialImpactCardsDTO
    {
        public long GroupId { get; set; }
        public long TableId { get; set; }
        public long CompanyId { get; set; }
    }
    public class SalaryTableValuesFinancialImpactCardsDTO
    {
        public long TableId { get; set; }
        public double Midpoint { get; set; }
        public int GSM { get; set; }
        public IEnumerable<long> GroupIds { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
    }
    public class SalaryTableTracksFinancialImpactCardsDTO
    {
        public long TableId { get; set; }
        public double Multiplier { get; set; }
    }

    public class SalaryTableResultFinancialImpactCardsDTO
    {
        public double Multiplier { get; set; }
        public double Value { get; set; }
    }

    public class ResultFinancialImpactCardsDTO
    {
        public double EmployeeAmount { get; set; } = 0;
        public double EmployeePercentage { get; set; } = 0;
        public double Cost { get; set; } = 0;
        public double CostPercentage { get; set; } = 0;
        public AnalyseFinancialImpactEnum AnalyseFinancialImpact { get; set; }
        public string ItemGrouped { get; set; }

    }

    public class ResultListGroupByFinancialImpactDTO
    {
        public string Name { get; set; }
        public IEnumerable<BaseSalaryFinancialImpactCardsDTO> Values { get; set; }
    }
    public class GetFinancialImpactCardsHandler
        : IRequestHandler<GetFinancialImpactCardsRequest,
            IEnumerable<GetFinancialImpactCardsResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IEnumerable<AnalyseFinancialImpactEnum> _listEnum;

        public GetFinancialImpactCardsHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _listEnum = Enum.GetValues(typeof(AnalyseFinancialImpactEnum)) as
                IEnumerable<AnalyseFinancialImpactEnum>;

            _listEnum = _listEnum.First().GetWithOrder().Select(s =>
            (AnalyseFinancialImpactEnum)Enum.Parse(typeof(AnalyseFinancialImpactEnum), s));
        }
        private async Task<IEnumerable<GetFinancialImpactCardsResponse>> _profileAsync(GetFinancialImpactCardsRequest request, List<BaseSalaryFinancialImpactCardsDTO> result)
        {

            var profiles = result
                        .GroupBy(g => g.ProfileId, (key, value) => new { key, value });

            var groupsIds = profiles.Select(s => s.key);

            var groupsProfiles = await _unitOfWork.GetRepository<GruposProjetosSalaryMark, long>()
                .GetListAsync(x => x.Where(g => groupsIds.Contains(g.GruposProjetosSmidLocal) &&
                 request.ProjectId == g.ProjetoId)?.
                Select(s => new
                {
                    Id = s.GruposProjetosSmidLocal,
                    Name = s.GrupoSm
                }));

            var profilesOrder = profiles.OrderBy(o => o.key)
                .Select(s => new ResultListGroupByFinancialImpactDTO
                {
                    Name = groupsProfiles.FirstOrDefault(f => f.Id == s.key)?.Name,
                    Values = s.value,
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();

            return _processFilterAsync(request, result, profilesOrder);
        }
        private async Task<IEnumerable<GetFinancialImpactCardsResponse>> _levelAsync(GetFinancialImpactCardsRequest request, List<BaseSalaryFinancialImpactCardsDTO> result)
        {
            var groupsIds = result.Select(s => s.LevelId).Distinct();

            var groupsLevels = await _unitOfWork.GetRepository<NivelEmpresas, long>()
                        .GetListAsync(x => x.Where(g => groupsIds.Contains(g.NivelId) &&
                                    request.CompaniesId.Contains(g.EmpresaId) &&
                                    (!request.UnitId.HasValue || g.EmpresaId == request.UnitId.Value))?.
                        Select(s => new
                        {
                            Id = s.NivelId,
                            Name = s.NivelEmpresa
                        }));

            var levels = result
                        .GroupBy(g => groupsLevels.FirstOrDefault(f => f.Id == g.LevelId)?.Name, (key, value) => new { key, value });

            levels
                .OrderBy(o => o.value.FirstOrDefault().LevelId)
                .Select(s => new ResultListGroupByFinancialImpactDTO
                {
                    Name = s.key,
                    Values = s.value,
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();

            var levelsOrder = levels.OrderBy(o => o.key)
                .Select(s => new ResultListGroupByFinancialImpactDTO
                {
                    Name = groupsLevels.FirstOrDefault(f => f.Name == s.key)?.Name,
                    Values = s.value,
                }).Where(re => !string.IsNullOrWhiteSpace(re.Name)).ToList();

            return _processFilterAsync(request, result, levelsOrder);
        }
        private IEnumerable<GetFinancialImpactCardsResponse> _processFilterAsync(GetFinancialImpactCardsRequest request, List<BaseSalaryFinancialImpactCardsDTO> result, List<ResultListGroupByFinancialImpactDTO> profilesOrder)
        {
            var resultFinancialImpactDTOList = new List<GetFinancialImpactCardsResponse>();
            var employeeAmountTotal = result.Sum(s => Convert.ToDouble(s.InternalFrequency.GetValueOrDefault(0)));
            var costTotal = result.Sum(s => Convert.ToDouble(s.FinalSalarySM));

            foreach (AnalyseFinancialImpactEnum itemEnum in _listEnum)
            {
                double? employeeAmount = 0;
                double? cost = 0;
                foreach (var order in profilesOrder)
                {

                    employeeAmount += result
                        .Where(s => Convert.ToDouble(s.GetType().GetProperty(itemEnum.ToString()).GetValue(s)) > 0)?
                        .Sum(s => Convert.ToDouble(s.InternalFrequency.GetValueOrDefault(0)));

                    cost += order.Values
                        .Where(s => Convert.ToDouble(s.GetType().GetProperty(itemEnum.ToString()).GetValue(s)) > 0)?
                        .Sum(s => Convert.ToDouble(s.GetType().GetProperty(itemEnum.ToString()).GetValue(s)));
                }


                resultFinancialImpactDTOList.Add(new GetFinancialImpactCardsResponse
                {
                    AnalyseFinancialImpactId = itemEnum,
                    AnalyseFinancialImpactName = itemEnum.GetDescription(),
                    EmployeeAmount = Math.Round(employeeAmount.GetValueOrDefault(0), 0),
                    Cost = Math.Round(cost.GetValueOrDefault(0), 0),
                    CostPercentage = costTotal == 0 ? 0 :
                        Math.Round(100 * (cost.GetValueOrDefault(0) / costTotal), 1),
                    EmployeePercentage = employeeAmountTotal == 0 ? 0 :
                        Math.Round(100 * (employeeAmount.GetValueOrDefault(0) / employeeAmountTotal), 0),
                });
            }

            return resultFinancialImpactDTOList;
        }
        public async Task<IEnumerable<GetFinancialImpactCardsResponse>> Handle(GetFinancialImpactCardsRequest request, CancellationToken cancellationToken)
        {
            var result = await GetResultConsolidated(request);

            if (!result.Any())
                return new List<GetFinancialImpactCardsResponse>();

            switch (request.DisplayBy)
            {
                case DisplayByPositioningEnum.ProfileId:
                    return await _profileAsync(request, result);

                case DisplayByPositioningEnum.LevelId:
                    return await _levelAsync(request, result);

                //case DisplayByPositioningEnum.Area:
                //    return Area(request, result);

                //case DisplayByPositioningEnum.Parameter01:
                //    return Parameter01(request, result);

                //case DisplayByPositioningEnum.Parameter02:
                //    return Parameter02(request, result);

                //case DisplayByPositioningEnum.Parameter03:
                //    return Parameter03(request, result);

                default:
                    return await _profileAsync(request, result);
            }
        }

        private async Task<List<BaseSalaryFinancialImpactCardsDTO>> GetResultConsolidated(GetFinancialImpactCardsRequest request)
        {

            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            IEnumerable<BaseSalaryFinancialImpactCardsDTO> result =
                await GetSalaryBaseXPositionSM(request, permissionUser);

            var tablesExp =
                permissionUser.Contents?
                .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                .SubItems;

            var groupIds = result
               .Select(s => s.ProfileId).Distinct().ToList();

            var groupsData = await _unitOfWork.GetRepository<GruposProjetosSalaryMarkMapeamento, long>()
                    .GetListAsync(x => x.Where(g => g.ProjetoId == request.ProjectId &&
                     groupIds.Contains(g.GrupoProjetoSmidLocal) &&
                     (!tablesExp.Safe().Any() || !tablesExp.Contains(g.TabelaSalarialIdLocal)) &&
                     (request.UnitId.HasValue || request.CompaniesId.Contains(g.EmpresaId)) &&
                     (!request.UnitId.HasValue || g.EmpresaId == request.UnitId.Value))?
                    .Select(s => new
                    {
                        MinTrackId = s.FaixaIdInferior,
                        MaxTrackId = s.FaixaIdSuperior,
                        GroupId = s.GrupoProjetoSmidLocal,
                        CompanyId = s.EmpresaId,
                        TableId = s.TabelaSalarialIdLocal
                    }));

            if (!groupsData.Any())
                return new List<BaseSalaryFinancialImpactCardsDTO>();

            var listTableIds = groupsData?.Select(s => s.TableId).Distinct();

            var salaryTableResult = await _unitOfWork.GetRepository<TabelasSalariais, long>()
                .Include("TabelasSalariaisFaixas")
                .Include("TabelasSalariaisValores")
                .GetListAsync(x => x.Where(ts => listTableIds.Contains(ts.TabelaSalarialIdLocal) &&
                 ts.ProjetoId == request.ProjectId)
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
                return new List<BaseSalaryFinancialImpactCardsDTO>();

            var tracks = salaryTableResult
                .OrderByDescending(o => o.Tracks.Count())
                .Take(1)
                .SelectMany(s => s.Tracks)
                .OrderBy(o => o.TrackId);

            var minSalaries = await _unitOfWork.GetRepository<TabelasSalariaisGrades, long>()
                                .GetListAsync(g => g.Where(tg => tg.ProjetoId == request.ProjectId &&
                                 listTableIds.Contains(tg.TabelaSalarialIdLocal))?
                                 .Select(se => new
                                 {
                                     TableId = se.TabelaSalarialIdLocal,
                                     SalaryMin = se.MenorSalario
                                 }));

            foreach (var salaryBasePositionSM in result)
            {

                var companyId = salaryBasePositionSM.CompanyId;
                var groupId = salaryBasePositionSM.ProfileId;
                var mapGroup = groupsData
                                    .Where(f => f.CompanyId == companyId && f.GroupId == groupId).ToList();

                var tableIds = mapGroup?.Select(s => s.TableId);

                var maxTrackId = mapGroup.Count > 0 ? mapGroup
                                .Where(s => tableIds.Safe().Any() && tableIds.Contains(s.TableId))?
                                .Max(m => m.MaxTrackId) : 0;

                var minTrackId = mapGroup.Count > 0 ? mapGroup
                                .Where(s => tableIds.Safe().Any() && tableIds.Contains(s.TableId))?
                                .Min(m => m.MinTrackId) : 0;

                var hoursBase = salaryBasePositionSM.HoursBase;
                var hoursBaseSM = salaryBasePositionSM.HoursBaseSM;

                var minSalary = minSalaries.Where(w => tableIds.Contains(w.TableId)).Average(a => a.SalaryMin) / hoursBaseSM * hoursBase;
                var finalSalary = salaryBasePositionSM.FinalSalarySM;

                var salaryTable =
                        salaryTableResult.Where(st =>
                                tableIds.Safe().Any() && tableIds.Contains(st.TableId)).ToList();

                if (salaryTable.Safe().Any())
                {
                    double? midPoint = null;

                    var gsmData = salaryTable.SelectMany(s => s.MidPoint)?
                         .Where(s => s.GSM == salaryBasePositionSM.GSM);

                    if (gsmData.Any())
                        midPoint = gsmData
                         .Average(a => a.MidPoint) / hoursBaseSM * hoursBase;

                    var listSalaryTable = new List<DataSalaryTableFinancialImpact>();

                    foreach (var track in tracks)
                    {
                        if (track.TrackId < minTrackId || track.TrackId > maxTrackId)
                        {
                            listSalaryTable.Add(new DataSalaryTableFinancialImpact
                            {
                                Value = 0,
                                Multiplicator = track.FactorMulti,
                                TrackId = track.TrackId
                            });
                            continue;
                        }

                        var fFM = salaryTable
                        .SelectMany(a => a.Tracks)
                        .Where(s => s.TrackId == track.TrackId
                        )?
                        .Average(a => a.FactorMulti);

                        var midPointResult = midPoint.HasValue ?
                                (midPoint.Value * fFM)
                                : 0;

                        if (fFM.HasValue && (midPointResult >= minSalary))
                        {
                            listSalaryTable.Add(new DataSalaryTableFinancialImpact
                            {
                                Value = Math.Round((double)midPointResult, 0),
                                Multiplicator = fFM.Value,
                                TrackId = track.TrackId
                            });
                        }
                        else
                        {
                            listSalaryTable.Add(new DataSalaryTableFinancialImpact
                            {
                                Value = 0,
                                Multiplicator = track.FactorMulti,
                                TrackId = track.TrackId
                            });
                        }
                    }

                    salaryBasePositionSM.IFAMin = GetIFAMin(listSalaryTable.Select(s => s.Value),
                                finalSalary);

                    salaryBasePositionSM.IFAMax = GetIFAMax(listSalaryTable.Select(s => s.Value),
                                    finalSalary);


                    salaryBasePositionSM.IFPS = GetIFPS(
                                    listSalaryTable.Where(m => m.TrackId <= (int)TrackIdDefault.MidPoint && m.Value > 0)
                                    .Select(s => s.Value),
                                    finalSalary,
                                    salaryBasePositionSM.IFAMin);

                    salaryBasePositionSM.IFMP = GetIFMP(midPoint,
                        finalSalary);

                }
            }

            return result.ToList();
        }
        private async Task<IEnumerable<BaseSalaryFinancialImpactCardsDTO>> GetSalaryBaseXPositionSM(GetFinancialImpactCardsRequest request,
            PermissionJson permissionUser)
        {
            IEnumerable<BaseSalaryFinancialImpactCardsDTO> salaryBaseList =
                        new List<BaseSalaryFinancialImpactCardsDTO>();


            var groupsExp =
                        permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                        .SubItems;

            var areaExp = permissionUser.Areas;

            var levelsExp = permissionUser.Levels;
            var ignoreSituationPerson = !permissionUser.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);

            if (request.Scenario == DisplayMMMIEnum.MM)
                salaryBaseList = await _unitOfWork.GetRepository<BaseSalariais, long>()
                    .Include("CmcodeNavigation")
                    .GetListAsync(x => x.Where(bs =>
                          bs.CargoIdSMMM.HasValue &&
                          request.CompaniesId.Contains(bs.EmpresaId.Value) &&
                          (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                          (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.CmcodeNavigation.NivelId)) &&
                          (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active)
                          )?
                         .Select(s => new BaseSalaryFinancialImpactCardsDTO
                         {
                             FinalSalarySM = s.SalarioFinalSm,
                             CompanyId = s.EmpresaId.Value,
                             HoursBase = s.BaseHoraria.GetValueOrDefault(0),
                             InternalFrequency = s.FrequenciaInterna,
                             LevelId = s.CmcodeNavigation.NivelId,
                             PositionId = s.CargoIdSMMM.Value
                         }));

            if (request.Scenario == DisplayMMMIEnum.MI)
                salaryBaseList = await _unitOfWork.GetRepository<BaseSalariais, long>()
                    .Include("CmcodeNavigation")
                    .GetListAsync(x => x.Where(bs =>
                            bs.CargoIdSMMI.HasValue &&
                          (request.CompaniesId.Contains(bs.EmpresaId.Value)) &&
                          (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                          (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.CmcodeNavigation.NivelId)) &&
                          (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active)
                          )?
                         .Select(s => new BaseSalaryFinancialImpactCardsDTO
                         {
                             FinalSalarySM = s.SalarioFinalSm,
                             CompanyId = s.EmpresaId.Value,
                             HoursBase = s.BaseHoraria.GetValueOrDefault(0),
                             InternalFrequency = s.FrequenciaInterna,
                             LevelId = s.CmcodeNavigation.NivelId,
                             PositionId = s.CargoIdSMMI.Value
                         }));

            if (!salaryBaseList.Any())
                return new List<BaseSalaryFinancialImpactCardsDTO>();

            var positionsIds = salaryBaseList.Select(s => s.PositionId).Distinct();

            var positionsSM = await _unitOfWork.GetRepository<CargosProjetosSmmapeamento, long>()
                .Include("CargosProjetosSm")
                .GetListAsync(g => g
                .Where(cp =>
                cp.CargosProjetosSm.Ativo.HasValue &&
                cp.CargosProjetosSm.Ativo.Value &&
                positionsIds.Contains(cp.CargoProjetoSmidLocal) && cp.ProjetoId == request.ProjectId)?.
                Select(s => new PositionSalaryFinancialImpactDTO
                {
                    PositionId = s.CargoProjetoSmidLocal,
                    //Area = s.Area,
                    //GroupId = s.GrupoSmidLocal,
                    ////GSM = s.Gsm,
                    //HoursBaseSM = s.BaseHoraria,
                    //Parameter01 = s.Parametro1,
                    //Parameter02 = s.Parametro2,
                    //Parameter03 = s.Parametro3
                }));

            foreach (var salary in salaryBaseList)
            {
                var positionValues = positionsSM.FirstOrDefault(f => f.PositionId == salary.PositionId);

                //salary.Area = positionValues.Area;
                //salary.ProfileId = positionValues.GroupId;
                //salary.GSM = positionValues.GSM;
                //salary.HoursBaseSM = positionValues.HoursBaseSM;
                //salary.Parameter01 = positionValues.Parameter01;
                //salary.Parameter02 = positionValues.Parameter02;
                //salary.Parameter03 = positionValues.Parameter03;
            }

            return salaryBaseList.Where(s => (!groupsExp.Safe().Any() || !groupsExp.Contains(s.ProfileId)) &&
                           (!areaExp.Safe().Any()));// || !areaExp.Contains(s.Area)));
        }
        private double GetIFAMin(IEnumerable<double> values, double? finalSalary)
        {
            if (!values.Any() || finalSalary.GetValueOrDefault(0) == 0) return 0;

            var list = values.Where(s => s > 0)?.ToList();

            if (!list.Safe().Any()) return 0;

            var diff = finalSalary.Value / list.Min();

            if (diff < 1) return list.Min() - finalSalary.Value;

            return 0;
        }
        private double GetIFAMax(IEnumerable<double> values, double? finalSalary)
        {
            if (!values.Any() || finalSalary.GetValueOrDefault(0) == 0) return 0;

            var list = values.Where(s => s > 0)?.ToList();

            if (!list.Safe().Any()) return 0;

            var diff = finalSalary.Value / list.Max();

            if (diff > 1) return finalSalary.Value - list.Max();

            return 0;
        }
        private double GetIFPS(IEnumerable<double> values,
            double? finalSalary,
            double iFAMin)
        {
            if (iFAMin > 1) return iFAMin;

            if (!values.Any()) return 0;

            double nextStep = values
                .FirstOrDefault(f => f > finalSalary.GetValueOrDefault(0));

            return nextStep == 0 ? 0 : nextStep
                - finalSalary.GetValueOrDefault(0);
        }

        private double GetIFMP(double? midpoint,
                        double? finalSalary)
        {
            return midpoint.GetValueOrDefault(0) - finalSalary.GetValueOrDefault(0);
        }
    }
}
