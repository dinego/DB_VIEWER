using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Application.Interactors
{
    public class DataPositionProjectSMRequest
    {
        public long PositionId { get; set; }
        public long CompanyId { get; set; }
        public double HoursBase { get; set; }
        public double FinalSalary { get; set; }
        public long SalaryBaseId { get; set; }
    }

    public class DataPositionProjectSMDTO
    {
        public long PositionId { get; set; }
        public string PositionSm { get; set; }
        public string Profile { get; set; }
        public double HoursBaseSM { get; set; }
        public long GSM { get; set; }
        public IEnumerable<DataSalaryTable> SalaryTable { get; set; }
        public double CompareMidPoint { get; set; }
        public long GroupId { get; set; }
        public string Contract { get; set; }
    }

    public class DataPositionProjectSMResponse
    {
        public long PositionId { get; set; }
        public string PositionSm { get; set; }
        public string Profile { get; set; }
        public long ProfileId { get; set; }
        //public long GSM { get; set; }
        public IEnumerable<DataSalaryTable> SalaryTable { get; set; }
        public double CompareMidPoint { get; set; }
        public long SalaryBaseId { get; set; }
        public long GSM { get; set; }
    }
    public class DataSalaryTable
    {
        public double Multiplicator { get; set; }
        public double Value { get; set; }
        public long TrackId { get; set; }

    }
    public class GetPositionProjectSMAndSalaryTableInteractor : IGetPositionProjectSMAndSalaryTableInteractor
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMultiplierFactorHourlyInteractor _multiplierFactorHourlyInteractor;
        private readonly IMultiplierFactorTypeContratcInteractor _multiplierFactorTypeContratcInteractor;

        public GetPositionProjectSMAndSalaryTableInteractor(IUnitOfWork unitOfWork,
            IMultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor,
            IMultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor)
        {
            _unitOfWork = unitOfWork;
            _multiplierFactorHourlyInteractor = multiplierFactorHourlyInteractor;
            _multiplierFactorTypeContratcInteractor = multiplierFactorTypeContratcInteractor;
        }
        public async Task<IEnumerable<DataPositionProjectSMResponse>>
            Handler(IEnumerable<DataPositionProjectSMRequest> request, long projectId,
            PermissionJson permissionUser,
            DataBaseSalaryEnum dataBaseType,
            ContractTypeEnum contratcType)
        {

            var postionIds = request.Select(s => s.PositionId).Distinct();

            var groupsExp =
                permissionUser.Contents?
                .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                .SubItems;

            var tablesExp =
                permissionUser.Contents?
                .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                .SubItems;

            var areaExp =
                        permissionUser.Areas;

            var positionSmResult = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                .Include("GruposProjetosSalaryMark")
                .Include("CargosProjetosSmmapeamento")
                .GetListAsync(x => x.Where(sps => sps.Ativo.HasValue &&
                sps.Ativo.HasValue &&
                sps.Ativo.Value &&
                postionIds.Contains(sps.CargoProjetoSmidLocal) &&
                 sps.ProjetoId == projectId &&
                 (!areaExp.Safe().Any() /*|| !areaExp.Contains(sps.Area)*/) &&
                 (!groupsExp.Safe().Any() || !groupsExp.Contains(sps.GrupoSmidLocal)))?
                 .Select(s => new DataPositionProjectSMDTO
                 {
                     GSM = s.CargosProjetosSmmapeamento.Any(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal) ? (long)s.CargosProjetosSmmapeamento.FirstOrDefault(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal).Gsm : 0,
                     PositionSm = s.CargoSm,
                     PositionId = s.CargoProjetoSmidLocal,
                     HoursBaseSM = s.BaseHoraria,
                     Profile = s.GruposProjetosSalaryMark.GrupoSm,
                     GroupId = s.GrupoSmidLocal,
                 }));



            if (!positionSmResult.Any())
                return new List<DataPositionProjectSMResponse>();

            var groupIds = positionSmResult.Select(s => s.GroupId).Distinct().ToList();

            var groupsData = await _unitOfWork.GetRepository<GruposProjetosSalaryMarkMapeamento, long>()
                .GetListAsync(x => x.Where(g => g.ProjetoId == projectId &&
                (!tablesExp.Safe().Any() || !tablesExp.Contains(g.TabelaSalarialIdLocal)) &&
                 groupIds.Contains(g.GrupoProjetoSmidLocal))
                .Select(s => new
                {
                    MinTrackId = s.FaixaIdInferior,
                    MaxTrackId = s.FaixaIdSuperior,
                    GroupId = s.GrupoProjetoSmidLocal,
                    CompanyId = s.EmpresaId,
                    TableId = s.TabelaSalarialIdLocal
                }));

            if (!groupsData.Any())
                return new List<DataPositionProjectSMResponse>();

            var multiplierFactorHourlyInteractor = await _multiplierFactorHourlyInteractor.Handler(projectId, dataBaseType);
            var multiplierFactorTypeContratcInteractor = await _multiplierFactorTypeContratcInteractor.Handler(projectId, contratcType);

            var listTableIds = groupsData?.Select(s => s.TableId).Distinct();

            var salaryTableResult = await _unitOfWork.GetRepository<TabelasSalariais, long>()
                .Include("TabelasSalariaisFaixas")
                .Include("TabelasSalariaisValores")
                .GetListAsync(x => x.Where(ts => listTableIds.Contains(ts.TabelaSalarialIdLocal) &&
                 ts.ProjetoId == projectId)
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
                throw new Exception("Não foi encontrado nenhum item TabelasSalariais");

            var minMidPointList = await _unitOfWork.GetRepository<TabelasSalariaisGrades, long>()
                 .GetListAsync(x => x.Where(ts => ts.ProjetoId == projectId && listTableIds.Contains(ts.TabelaSalarialIdLocal))
                 .Select(s => new
                 {
                     TableId = s.TabelaSalarialIdLocal,
                     MinMidPoint = s.MenorSalario
                 }));

            var tracks = salaryTableResult
                .OrderByDescending(o => o.Tracks.Count())
                .Take(1)
                .SelectMany(s => s.Tracks)
                .OrderBy(o => o.TrackId);

            var result = new List<DataPositionProjectSMResponse>();

            foreach (var salarybase in request)
            {

                var positionSm = positionSmResult.FirstOrDefault(f => f.PositionId == salarybase.PositionId);

                var companyId = salarybase.CompanyId;
                var groupId = positionSm.GroupId;

                var mapGroup = groupsData
                    .Where(f => f.CompanyId == companyId && f.GroupId == groupId).ToList();

                var tableIds = mapGroup?.Select(s => s.TableId);

                var maxTrackId = mapGroup.Count > 0 ? mapGroup
                                .Where(s => tableIds.Safe().Any() && tableIds.Contains(s.TableId))?
                                .Max(m => m.MaxTrackId) : 0;

                var minTrackId = mapGroup.Count > 0 ? mapGroup
                                .Where(s => tableIds.Safe().Any() && tableIds.Contains(s.TableId))?
                                .Min(m => m.MinTrackId) : 0;

                var hoursBase = salarybase.HoursBase;
                var finalSalary = salarybase.FinalSalary;

                var salaryTable = salaryTableResult.Where(st => tableIds.Safe().Any() && tableIds.Contains(st.TableId)).ToList();

                var listSalaryTable = new List<DataSalaryTable>();

                if (salaryTable.Count > 0)
                {
                    var midPoint = salaryTable.SelectMany(s => s.MidPoint)?
                                                .Where(s => s.GSM == positionSm.GSM)?
                                                .Average(a => a.MidPoint);
                    foreach (var track in tracks)
                    {
                        if (track.TrackId < minTrackId || track.TrackId > maxTrackId)
                        {
                            listSalaryTable.Add(new DataSalaryTable
                            {
                                Value = 0,
                                Multiplicator = track.FactorMulti,
                                TrackId = track.TrackId
                            });
                            continue;
                        }


                        var fFM = salaryTable
                        .SelectMany(a => a.Tracks)
                        .Where(s => s.TrackId == track.TrackId)?
                        .Average(a => a.FactorMulti);

                        if (dataBaseType == DataBaseSalaryEnum.HourSalary)
                        {
                            multiplierFactorHourlyInteractor = 1 / positionSm.HoursBaseSM;
                        }

                        var valueMidPoint = midPoint.HasValue ?
                                (midPoint.Value * fFM / positionSm.HoursBaseSM * hoursBase)
                                * multiplierFactorHourlyInteractor * multiplierFactorTypeContratcInteractor.GetValueOrDefault(0)
                                : 0;

                        double? minMidPoint =
                        (minMidPointList.Where(f => tableIds.Contains(f.TableId)).Average(a => a.MinMidPoint) / positionSm.HoursBaseSM * hoursBase)
                                * multiplierFactorHourlyInteractor * multiplierFactorTypeContratcInteractor.GetValueOrDefault(0);

                        minMidPoint = minMidPoint.HasValue && minMidPoint.Value < (minMidPointList.Where(f => tableIds.Contains(f.TableId)).Average(a => a.MinMidPoint) / positionSm.HoursBaseSM * hoursBase) ?
                                     null : minMidPoint;

                        if (fFM.HasValue && valueMidPoint >= minMidPoint)
                        {
                            listSalaryTable.Add(new DataSalaryTable
                            {
                                Value = (double)valueMidPoint,
                                Multiplicator = fFM.Value,
                                TrackId = track.TrackId
                            });
                        }
                        else
                        {
                            listSalaryTable.Add(new DataSalaryTable
                            {
                                Value = 0,
                                Multiplicator = track.FactorMulti,
                                TrackId = track.TrackId
                            });
                        }
                    }

                    //compare always with month
                    var midpointFixed =
                                (midPoint.Value / positionSm.HoursBaseSM * hoursBase)
                                 * multiplierFactorTypeContratcInteractor.GetValueOrDefault(0);

                    var compareMidPoint = midpointFixed <= 0 ? 0 :
                              (finalSalary / midpointFixed);

                    result.Add(new DataPositionProjectSMResponse
                    {
                        CompareMidPoint = compareMidPoint,
                        GSM = positionSm.GSM,
                        PositionId = positionSm.PositionId,
                        PositionSm = positionSm.PositionSm,
                        Profile = positionSm.Profile,
                        ProfileId = positionSm.GroupId,
                        SalaryTable = listSalaryTable,
                        SalaryBaseId = salarybase.SalaryBaseId
                    });
                }
            }

            return result;
        }

    }
}

