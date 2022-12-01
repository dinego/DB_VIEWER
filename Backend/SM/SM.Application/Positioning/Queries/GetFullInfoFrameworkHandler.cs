using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Positioning.Queries
{

    public class GetFullInfoFrameworkRequest : IRequest<GetFullInfoFrameworkResponse>
    {
        public long SalaryBaseId { get; set; }
        public bool IsMM { get; set; } = false;
        public bool IsMI { get; set; } = false;
        public long UserId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public long ProjectId { get; set; }
        public ContractTypeEnum ContractType { get; set; } = ContractTypeEnum.CLT;
        public DataBaseSalaryEnum HoursType { get; set; } = DataBaseSalaryEnum.MonthSalary;
    }
    public class GetFullInfoFrameworkResponse
    {
        public IEnumerable<PositionsSMFramework> PositionsSMFramework { get; set; }

        public IEnumerable<LabelControl> HeaderPosition { get; set; }
        = new List<LabelControl>();

        public GSMConfig GSMConfig { get; set; }
    }

    public class GSMConfig
    {
        public string Name { get; set; }
        public bool Visible { get; set; } = true;
    }

    public class LabelControl
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Visible { get; set; } = true;
        public string @Type { get; set; }
        public int PropertyId { get; set; }
    }

    public class GetFullInfoFrameworkResponseDTO
    {
        public long SalaryBaseId { get; set; }
        public long CompanyId { get; set; }
        public string EmployeeId { get; set; }
        public string Employee { get; set; }
        public string CurrentPosition { get; set; }
        public string UnitPlace { get; set; }
        public string Profile { get; set; }
        public string Level { get; set; }
        public int LevelId { get; set; }
        public double? HourlyBasis { get; set; }
        public double? Salary { get; set; }
        public long? SMCargoIdMM { get; set; }
        public long? SMCargoIdMI { get; set; }
        public long? SMCargoId { get; set; }


    }

    public class PositionsSMFramework
    {
        public string Type { get; set; }
        public long GSM { get; set; }
        public string Position { get; set; }
        public double Compare { get; set; }

    }


    public class GetFullInfoFrameworkHandler
        : IRequestHandler<GetFullInfoFrameworkRequest, GetFullInfoFrameworkResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IGetPositionProjectSMAndSalaryTableInteractor _getPositionProjectSMAndSalaryTableInteractor;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;

        public GetFullInfoFrameworkHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor,
            IGetPositionProjectSMAndSalaryTableInteractor getPositionProjectSMAndSalaryTableInteractor,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _getPositionProjectSMAndSalaryTableInteractor = getPositionProjectSMAndSalaryTableInteractor;
            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;
        }
        public async Task<GetFullInfoFrameworkResponse> Handle(GetFullInfoFrameworkRequest request, CancellationToken cancellationToken)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            //BaseSalarial table
            var salaryBase = await QuerySalaryBase(request, permissionUser);

            if (salaryBase == null)
                return new GetFullInfoFrameworkResponse();

            salaryBase.Level = await GetLevelCompanies(salaryBase.CompanyId, salaryBase.LevelId);

            //get all smIds
            IEnumerable<DataPositionProjectSMResponse> positionSm =
                await GetDataPositionSM(request, salaryBase, permissionUser);

            var positionsSMFrameworkList = new List<PositionsSMFramework>();

            if (request.IsMI)
            {
                var smPositionIdMI =
                    positionSm.FirstOrDefault(ps => ps.PositionId == salaryBase.SMCargoIdMI);

                if (smPositionIdMI != null)
                {
                    positionsSMFrameworkList.Add(new PositionsSMFramework
                    {
                        Compare = Math.Round(smPositionIdMI.CompareMidPoint * 100, 0),
                        GSM = smPositionIdMI.GSM,
                        Position = smPositionIdMI.PositionSm,
                        Type = DisplayMMMIEnum.MI.ToString()
                    });
                }
            }

            if (request.IsMM)
            {
                var smPositionIdMM =
                    positionSm.FirstOrDefault(ps => ps.PositionId == salaryBase.SMCargoIdMM);

                if (smPositionIdMM != null)
                {
                    positionsSMFrameworkList.Add(new PositionsSMFramework
                    {
                        Compare = Math.Round(smPositionIdMM.CompareMidPoint * 100, 0),
                        GSM = smPositionIdMM.GSM,
                        Position = smPositionIdMM.PositionSm,
                        Type = DisplayMMMIEnum.MM.ToString()
                    });
                }
            }

            //complete value 
            //profile CargoIdSM

            var profile = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                .Include("GruposProjetosSalaryMark")
                .GetAsync(x => x.Where(cm => cm.Ativo.HasValue &&
                                cm.Ativo.Value &&
                            cm.CargoProjetoSmidLocal == salaryBase.SMCargoId &&
                            cm.ProjetoId == request.ProjectId)?
                            .Select(s => s.GruposProjetosSalaryMark.GrupoSm));

            salaryBase.Profile = profile;

            //employee name 
            var employeeName = await _unitOfWork.GetRepository<Funcionarios, long>()
                .GetAsync(x => x.Where(f => f.CodigoFuncionario == salaryBase.EmployeeId &&
                 f.EmpresaId == salaryBase.CompanyId)?
                .Select(s => s.Nome));

            salaryBase.Employee = employeeName;

            var configGlobalLabels =
                        await _getGlobalLabelsInteractor.Handler(request.ProjectId);

            var gsmData = configGlobalLabels.FirstOrDefault(fe => fe.Id == (long)FrameworkFullInfoGSM.GSM);

            return new GetFullInfoFrameworkResponse
            {
                PositionsSMFramework = positionsSMFrameworkList ?? new List<PositionsSMFramework>(),
                HeaderPosition = await GetData(request, salaryBase, configGlobalLabels) ?? new List<LabelControl>(),
                GSMConfig = gsmData == null ? new GSMConfig() : new GSMConfig { Name = gsmData.Alias, Visible = gsmData.IsChecked }
            };
        }

        private async Task<IEnumerable<LabelControl>>
            GetData(GetFullInfoFrameworkRequest request,
            GetFullInfoFrameworkResponseDTO salaryBase,
            IEnumerable<GlobalLabelsJson> configGlobalLabels)
        {

            var columnsData = await QueryColumnsCustom(request);

            columnsData.ToList().ForEach(f =>
            {
                var globalLabel =
                configGlobalLabels.FirstOrDefault(fe => fe.Id == f.ColumnId);

                if (globalLabel != null)
                {
                    f.Name = globalLabel.Alias;
                }
            });

            var listEnum = Enum.GetValues(typeof(FrameworkFullInfoEnum)) as
                        IEnumerable<FrameworkFullInfoEnum>;

            var listLabels = new List<LabelControl>();

            foreach (var item in listEnum)
            {
                var checkCustomLabel =
                    columnsData.FirstOrDefault(f => f.ColumnId == (int)item);

                var property =
                            salaryBase.GetType()
                            .GetProperty(item.ToString())?
                            .GetValue(salaryBase);

                var value = property ?? string.Empty;

                if (checkCustomLabel != null)
                {
                    listLabels.Add(new LabelControl
                    {
                        Name = checkCustomLabel.Name,
                        Value = value.ToString(),
                        Visible = checkCustomLabel.IsChecked.GetValueOrDefault(),
                        Type = value.GetType().Name,
                        PropertyId = (int)item
                    });
                }
                else
                {
                    listLabels.Add(new LabelControl
                    {
                        Name = item.GetDescription(),
                        Value = value.ToString(),
                        Type = value.GetType().Name,
                        PropertyId = (int)item
                    });

                }
            }

            return listLabels;
        }

        private async Task<IEnumerable<ColumnDataDTO>> QueryColumnsCustom(GetFullInfoFrameworkRequest request)
        {
            return await _unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
                .GetListAsync(x => x.Where(ues => ues.UsuarioId == request.UserId &&
                 ues.ModuloSmid == (long)ModulesEnum.Positioning &&
                 ues.ModuloSmsubItemId.HasValue && ues.ModuloSmsubItemId.Value == (long)ModulesSuItemsEnum.Framework)
                ?.Select(s => new ColumnDataDTO
                {
                    ColumnId = s.ColunaId,
                    IsChecked = s.Checado,
                    Name = s.Nome
                }));
        }

        private async Task<GetFullInfoFrameworkResponseDTO>
            QuerySalaryBase(GetFullInfoFrameworkRequest request, PermissionJson userJson)
        {
            //filter levels and areas by user
            var levelsExp = userJson.Levels;
            var ignoreSituationPerson = !userJson.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);

            return await _unitOfWork.GetRepository<BaseSalariais, long>()
                .Include("CmcodeNavigation")
               .GetAsync(x => x.Where(bs => request.SalaryBaseId == bs.Id &&
                 request.CompaniesId.Contains(bs.EmpresaId.Value) &&
                 (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.CmcodeNavigation.NivelId)) &&
                (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active))
               ?.Select(s => new GetFullInfoFrameworkResponseDTO
               {
                   EmployeeId = s.FuncionarioId,
                   CurrentPosition = s.CargoEmpresa,
                   UnitPlace = s.Unidade,
                   LevelId = s.CmcodeNavigation.NivelId,
                   HourlyBasis = s.BaseHoraria,
                   Salary = s.SalarioFinalSm,
                   SalaryBaseId = s.Id,
                   SMCargoIdMI = s.CargoIdSMMI,
                   SMCargoIdMM = s.CargoIdSMMM,
                   SMCargoId = s.CargoIdSMMM,
                   CompanyId = s.EmpresaId.Value,
               }));
        }

        private async Task<IEnumerable<DataPositionProjectSMResponse>>
            GetDataPositionSM(GetFullInfoFrameworkRequest request,
            GetFullInfoFrameworkResponseDTO salaryBase,
            PermissionJson permissionUser)
        {
            IEnumerable<DataPositionProjectSMResponse> positionSm = new List<DataPositionProjectSMResponse>();
            var listDataPositionProjectSMRequest = new List<DataPositionProjectSMRequest>();

            if (request.IsMI || request.IsMM)
            {
                if (request.IsMI && salaryBase.SMCargoIdMI.HasValue)
                {
                    listDataPositionProjectSMRequest.Add(new DataPositionProjectSMRequest
                    {
                        CompanyId = salaryBase.CompanyId,
                        FinalSalary = salaryBase.Salary.GetValueOrDefault(0),
                        HoursBase = salaryBase.HourlyBasis.GetValueOrDefault(0),
                        PositionId = salaryBase.SMCargoIdMI.Value
                    });
                }

                if (request.IsMM && salaryBase.SMCargoIdMM.HasValue)
                {
                    listDataPositionProjectSMRequest.Add(new DataPositionProjectSMRequest
                    {
                        CompanyId = salaryBase.CompanyId,
                        FinalSalary = salaryBase.Salary.GetValueOrDefault(0),
                        HoursBase = salaryBase.HourlyBasis.GetValueOrDefault(0),
                        PositionId = salaryBase.SMCargoIdMM.Value
                    });
                }
                var listDataPositionProjectSMRequestDistinct =
                              listDataPositionProjectSMRequest
                              .GroupBy(r => r.PositionId)
                              .Select(g => g.First())
                              .ToList();


                positionSm = await _getPositionProjectSMAndSalaryTableInteractor.Handler(listDataPositionProjectSMRequestDistinct,
                     request.ProjectId, permissionUser, request.HoursType, request.ContractType);

            }

            return positionSm;
        }

        private async Task<string> GetLevelCompanies(long companyId, int levelId)
        {
            var levelCompanies = await _unitOfWork.GetRepository<NivelEmpresas, long>()
                .GetAsync(x => x.Where(ne => companyId == ne.EmpresaId &&
                 levelId == ne.NivelId)?
                .Select(s => s.NivelEmpresa));

            return levelCompanies ?? string.Empty;

        }
    }
}
