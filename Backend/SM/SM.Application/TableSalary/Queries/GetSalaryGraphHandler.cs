using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using SM.Application.TableSalary.Validators;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AgileObjects.AgileMapper.Extensions;
using SM.Application.Helpers;
using SM.Application.TableSalary.Queries.Response;

namespace SM.Application.TableSalary.Queries
{
    public class GetSalaryGraphRequest : IRequest<GetSalaryGraphResponse>
    {
        public long ProjectId { get; set; }
        public long TableId { get; set; }
        public long InitRange { get; set; }
        public long FinalRange { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public ContractTypeEnum ContractType { get; set; } = ContractTypeEnum.CLT;
        public DataBaseSalaryEnum HoursType { get; set; } = DataBaseSalaryEnum.MonthSalary;
        public long UserId { get; set; }
        public long? GroupId { get; set; }
        public long? UnitId { get; set; }
    }

    public class GetSalaryGraphResponse
    {
        public double MaxValue { get; set; }
        public List<Categories> Categories { get; set; }
        public List<ChartData> Chart { get; set; }
        public long RangeMin { get; set; }
        public long RangeMax { get; set; }
    }
    public class Categories
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class ChartData
    {
        public List<Data> Data { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }
    public class Data
    {
        public Data()
        {
            Positions = new List<Position>();
        }
        public int Gsm { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public long TableId { get; set; }
        public List<Position> Positions { get; set; }
        public long GroupId { get; set; }
        public long CompanyId { get; set; }
    }
    public class Position
    {
        public long Id { get; set; }
        public string PositionDescription { get; set; }
        public bool InCompany { get; set; }
        public bool OccupantCLT { get; set; }
        public bool OccupantPJ { get; set; }
    }

    public class SalaryMarkMappingProjectsPositionGroups
    {
        public long GroupId { get; set; }
        public long CompanyId { get; set; }
        public long RangeUp { get; set; }
        public long RangeDown { get; set; }
        public long TableId { get; set; }
    }

    public class GetSalaryGraphHandler
        : IRequestHandler<GetSalaryGraphRequest, GetSalaryGraphResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMultiplierFactorHourlyInteractor _multiplierFactorHourlyInteractor;
        private readonly IMultiplierFactorTypeContratcInteractor _multiplierFactorTypeContratcInteractor;
        private readonly IValidator<GetTableSalaryPermissionValidatorsException> _validator;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly ValidatorResponse _validatorResponse;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;

        public GetSalaryGraphHandler(IUnitOfWork unitOfWork,
            IMultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor,
            IMultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor,
            IValidator<GetTableSalaryPermissionValidatorsException> validator,
            IPermissionUserInteractor permissionUserInteractor,
            ValidatorResponse validatorResponse,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _multiplierFactorHourlyInteractor = multiplierFactorHourlyInteractor;
            _multiplierFactorTypeContratcInteractor = multiplierFactorTypeContratcInteractor;
            _validator = validator;
            _permissionUserInteractor = permissionUserInteractor;
            _validatorResponse = validatorResponse;
            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;
        }
        public async Task<GetSalaryGraphResponse> Handle(GetSalaryGraphRequest request,
            CancellationToken cancellationToken)
        {

            var resName = _validator.Validate(new GetTableSalaryPermissionValidatorsException
            {
                ContractType = request.ContractType,
                GroupId = request.GroupId.GetValueOrDefault(0),
                HourType = request.HoursType,
                TableId = request.TableId,
                UserId = request.UserId
            });

            if (!resName.IsValid)
                _validatorResponse.AddNotifications(resName.Errors.ToList());

            var configGlobalLabels =
                await _getGlobalLabelsInteractor.Handler(request.ProjectId);

            var factorHour = await _multiplierFactorHourlyInteractor.Handler(request.ProjectId, request.HoursType);
            var multiplierFactor = await _multiplierFactorTypeContratcInteractor.Handler(request.ProjectId, request.ContractType);
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);
            var groupsExp = permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                        .SubItems;

            var tablesExp = permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                        .SubItems;
            var ignoreSituationPerson = !permissionUser.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);
            var levelsExcept = permissionUser.Levels.Safe().Any() ? permissionUser.Levels.ToList() : new List<long>();

            var groupProjects = await _unitOfWork.GetRepository<GruposProjetosSalaryMarkMapeamento, long>()
                .GetListAsync(g => g.Where(gpm => gpm.ProjetoId == request.ProjectId &&
                                                                 gpm.TabelaSalarialIdLocal == request.TableId &&
                                                                 (!request.UnitId.HasValue || gpm.EmpresaId == request.UnitId.Value) &&
                                                                 !groupsExp.Safe().Contains(gpm.GrupoProjetoSmidLocal) &&
                                                                 (!request.GroupId.HasValue || gpm.GrupoProjetoSmidLocal == request.GroupId.Value))
                .Select(g => new SalaryMarkMappingProjectsPositionGroups
                {
                    GroupId = g.GrupoProjetoSmidLocal,
                    CompanyId = g.EmpresaId,
                    RangeDown = g.FaixaIdInferior,
                    RangeUp = g.FaixaIdSuperior,
                    TableId = g.TabelaSalarialIdLocal
                }));

            var groupsMapping = groupProjects.Select(res => res.GroupId);
            var companyByTableId = groupProjects.Select(res => res.CompanyId).Distinct();

            var salaryTableRanges = await _unitOfWork.GetRepository<TabelasSalariaisFaixas, long>()
            .GetListAsync(x => x.Where(str => str.TabelaSalarialIdLocal == request.TableId &&
                                        str.ProjetoId == request.ProjectId)
                                        .Select(res => new
                                        {
                                            Id = res.Id,
                                            SalaryRangeId = res.FaixaSalarialId,
                                            AmplitudeMidPoint = res.AmplitudeMidPoint,
                                            LocalIdSalaryTable = res.TabelaSalarialIdLocal
                                        }).OrderBy(o => o.SalaryRangeId));

            var datas = await _unitOfWork.GetRepository<TabelasSalariaisValores, long>()
                .Include("TabelasSalariais.GruposProjetosSalaryMarkMapeamento")
                .GetListAsync(x => x
                .Where(s => s.ProjetoId == request.ProjectId &&
                    s.Grade >= request.InitRange && s.Grade <= request.FinalRange &&

                    s.TabelasSalariais.GruposProjetosSalaryMarkMapeamento
                                                                    .Any(a => a.TabelaSalarialIdLocal == request.TableId &&
                                                                              a.ProjetoId == request.ProjectId &&
                                                                              groupsMapping.Safe().Contains(a.GrupoProjetoSmidLocal)) &&

                    (!request.UnitId.HasValue || (s.Projeto != null &&
                                                                s.Projeto.ProjetosSalaryMarkEmpresas
                                                                        .Any(psm => psm.EmpresaId == request.UnitId.Value &&
                                                                                    psm.ProjetoId == request.ProjectId))) &&

                    (!tablesExp.Safe().Any() || !tablesExp.Contains(s.TabelaSalarialIdLocal)) &&
                                                                s.TabelaSalarialIdLocal == request.TableId &&
                                                                s.TabelasSalariais != null &&
                                                               (s.TabelasSalariais.Ativo.HasValue && s.TabelasSalariais.Ativo.Value))
                    .Select(s => new Data
                    {
                        Name = s.Grade.ToString(),
                        Gsm = s.Grade, // front controller color only
                        Value = s.FaixaMidPoint,
                        TableId = s.TabelaSalarialIdLocal
                    }));

            var responseGraph = new GetSalaryGraphResponse()
            {
                Categories = new List<Categories>(),
                Chart = new List<ChartData>()
            };

            if (datas.Count() == 0)
                return responseGraph;

            var listGsms = datas.Select(s => s.Gsm).ToList();

            var positionsByGsm = await GetPositionProject(request.ProjectId, listGsms, permissionUser, request.GroupId, request.TableId);
            var positionsByGsmSmIdList = positionsByGsm.Select(x => x.ProjectPositionSMId).Distinct().ToList();
            var baseSalaries = await _unitOfWork.GetRepository<BaseSalariais, long>()
                            .Include("RegimeContratacao")
                            .Include("CmcodeNavigation.RotuloCargos")
                            .Include("CmcodeNavigation")
                            .GetListAsync(x => x
                            .Where(bs1 => companyByTableId.Contains(bs1.EmpresaId.Value) &&
                                   bs1.CargoIdSMMM.HasValue &&
                                   (!levelsExcept.Safe().Any() || !levelsExcept.Contains(bs1.CmcodeNavigation.NivelId)) &&
                                   (ignoreSituationPerson || bs1.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active) &&
                                   positionsByGsmSmIdList.Contains(bs1.CargoIdSMMM.Value))
                            .Select(res => new
                            {
                                PositionSMMM = res.CargoIdSMMM,
                                TypeOfContractId = res.RegimeContratacaoId,
                                TypeOfContract = res.RegimeContratacao != null ? res.RegimeContratacao.Regime : string.Empty,
                                PositionName = res.CargoEmpresa,
                                CompanyId = res.EmpresaId
                            }).Distinct());
            var salaryTableAll = new List<Data>();
            groupProjects.ForEach(gp =>
            {
                var gsmByGroup = positionsByGsm.Where(mp => mp.GroupId == gp.GroupId)
                                               .Select(res => res.GSM).ToList();
                var salaryValues = datas.Where(x => x.TableId == gp.TableId &&
                        (!request.GroupId.HasValue || gsmByGroup.Contains(x.Gsm)));
                var lstValues = new List<Data>(salaryValues);
                lstValues.AsParallel().ForAll(sl =>
                {
                    var values = sl.Map().ToANew<Data>();
                    lock (salaryTableAll)
                    {
                        values.GroupId = gp.GroupId;
                        if (!salaryTableAll.Any(st => st.Gsm == sl.Gsm))
                            salaryTableAll.Add(values);
                    }
                });
            });
            salaryTableAll.ForEach(data =>
            {
                var salaryTableRange = salaryTableRanges.FirstOrDefault(st => st.LocalIdSalaryTable == data.TableId &&
                                                                               st.SalaryRangeId == (int)SalarialRanges.MidPoint);
                data.Value = salaryTableRange != null ? salaryTableRange.AmplitudeMidPoint * data.Value * factorHour * multiplierFactor.GetValueOrDefault(0) : 0;
                var findPosition = positionsByGsm.FindAll(f => f.GSM == data.Gsm &&
                                                               (!request.UnitId.HasValue || f.CompanyId == request.UnitId.Value));
                if (findPosition.Count == 0)
                    return;

                var findToPosition = new List<Position>();
                findPosition.ForEach(position =>
                {
                    var positionOnBaseSalary = baseSalaries
                        .Where(x => x.CompanyId == position.CompanyId &&
                                    x.PositionSMMM.HasValue &&
                                    position.ProjectPositionSMId == x.PositionSMMM.Value &&
                                    x.TypeOfContractId.HasValue &&
                                    !permissionUser.TypeOfContract.Contains(x.TypeOfContractId.Value));

                    bool isOccupantCLT = positionOnBaseSalary.Safe().Any() ?
                            positionOnBaseSalary.ToList().Any(item => PJOrCLT.IsCLT(item?.TypeOfContract).GetValueOrDefault(false)) : false;

                    bool isOccupantPJ = positionOnBaseSalary.Safe().Any() ?
                            positionOnBaseSalary.ToList().Any(item => PJOrCLT.IsPJ(item?.TypeOfContract).GetValueOrDefault(false)) : false;

                    findToPosition.Add(new Position
                    {
                        Id = position.ProjectPositionSMId,
                        PositionDescription = position.Position,
                        InCompany = true,
                        OccupantCLT = isOccupantCLT,
                        OccupantPJ = isOccupantPJ
                    });
                });
                data.Positions.AddRange(findToPosition);
            });
            if (!salaryTableAll.Safe().Any())
                return responseGraph;
            salaryTableAll = salaryTableAll.OrderBy(o => o.Gsm).ToList();
            var salaryGraphData = new List<ChartData>();
            var mxValue = salaryTableAll.Select(s => s.Value).Max();
            var chartResult = SetBiggerData(salaryTableAll, mxValue);

            for (var i = 1; i <= 4; i++)
            {
                var chartData = new ChartData()
                {
                    Data = i == 1 ? chartResult : salaryTableAll,
                    Name = $"data{i}",
                    Type = i == 1 || i == 4 ? 1 : i,
                };
                salaryGraphData.Add(chartData);
            }

            responseGraph.Chart = salaryGraphData;
            responseGraph.RangeMin = salaryTableAll.Select(res => res.Gsm).Min();
            responseGraph.RangeMax = salaryTableAll.Select(res => res.Gsm).Max();
            responseGraph.MaxValue = request.HoursType == DataBaseSalaryEnum.HourSalary ?
                                Math.Round(mxValue, 2) :
                                Math.Round(mxValue, 0);

            return responseGraph;
        }

        private List<Data> SetBiggerData(List<Data> datas,
         double maxValue)
        {
            List<Data> results = new List<Data>();
            datas.Safe().ForEach(data =>
            {
                var dataBigger = new Data();
                dataBigger.Gsm = data.Gsm;
                dataBigger.Name = data.Name;
                dataBigger.Value = (maxValue * 3) + data.Value;

                results.Add(dataBigger);
            });
            return results.OrderBy(o => o.Gsm).ToList();
        }

        private async Task<List<ProjectPositionSM>> GetPositionProject
            (long projectId,
            List<int> listGsms,
            PermissionJson permissionUser,
            long? groupId,
            long tableId)
        {

            var areasExcept = permissionUser.Areas.Safe().Any() ? permissionUser.Areas.ToList() : new List<long>();
            var levelsExcept = permissionUser.Levels.Safe().Any() ? permissionUser.Levels.ToList() : new List<long>();

            var groupsExp = permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                        .SubItems;

            var positionsByGsm = await _unitOfWork.GetRepository<CargosProjetosSmmapeamento, long>()
                        .Include("CargosProjetosSm")
                        .Include("Empresa")
                .GetListAsync(x => x
                .Where(s => s.ProjetoId == projectId &&
                            s.TabelaSalarialIdLocal == tableId && listGsms.Contains(s.Gsm.GetValueOrDefault()) &&
                            s.CargosProjetosSm != null &&
                            s.CargosProjetosSm.Ativo.HasValue &&
                            s.CargosProjetosSm.Ativo.Value &&
                            (!groupId.HasValue || s.CargosProjetosSm.GrupoSmidLocal == groupId.Value) &&
                            s.CargosProjetosSm.CmcodeNavigation != null &&
                            (!levelsExcept.Safe().Any() || !levelsExcept.Contains(s.CargosProjetosSm.CmcodeNavigation.NivelId)) &&
                            (!groupsExp.Safe().Any() || !groupsExp.Contains(s.CargosProjetosSm.GrupoSmidLocal)))
               .Select(res => new ProjectPositionSM
               {
                   ProjectPositionSMId = res.CargoProjetoSmidLocal,
                   GroupId = res.CargosProjetosSm.GrupoSmidLocal,
                   Position = res.CargosProjetosSm.CargoSm,
                   GSM = res.Gsm.GetValueOrDefault(),
                   CompanyId = res.EmpresaId
               }).Distinct());

            return positionsByGsm;
        }
    }
}
