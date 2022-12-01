using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using SM.Domain.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Repositories.Extensions;
using SM.Domain.Helpers;

namespace SM.Application.Positioning.Queries
{
    public class GetFrameworkExcelRequest
        : IRequest<GetFrameworkExcelResponse>
    {
        public long UserId { get; set; }
        public long ProjectId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public bool IsMM { get; set; } = false;
        public bool IsMI { get; set; } = false;
        public ContractTypeEnum ContractType { get; set; } = ContractTypeEnum.CLT;
        public DataBaseSalaryEnum HoursType { get; set; } = DataBaseSalaryEnum.MonthSalary;
        public string Term { get; set; } = null;
        public IEnumerable<int> Columns { get; set; } = new List<int>(); //using to share (EXP)
        public bool? IsAsc { get; set; } = null;
        public int? SortColumnId { get; set; } = null;
    }
    public class GetFrameworkExcelResponse
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
    }

    public class SalaryBaseFrameworkExcelDTO
    {
        public long SalaryBaseId { get; set; }
        public string CurrentPosition { get; set; }
        public string Company { get; set; }
        public long CompanyId { get; set; }
        public string UnitPlace { get; set; }
        public string Business { get; set; }
        public string Setor { get; set; }
        public string Employee { get; set; }
        public string ImmediateSupervisor { get; set; }
        public string Contract { get; set; }
        public double? HourlyBasis { get; set; }
        public double? Salary { get; set; }
        public long? SMCargoIdMM { get; set; }
        public long? SMCargoIdMI { get; set; }
        public string TypeOfContract { get; set; }
        public string FuncionarioID { get; set; }
        public string PositionSm { get; set; }
        public string Profile { get; set; }
        public long ProfileId { get; set; }
        public long GSM { get; set; }
        public IEnumerable<DataSalaryTable> SalaryTable { get; set; }
        public double CompareMidPoint { get; set; }
    }

    public class ColumnDataExcelDTO
    {
        public long ColumnId { get; set; }
        public bool? IsChecked { get; set; }
        public string Name { get; set; }

    }
    public class GetFrameworkExcelHandler
        : IRequestHandler<GetFrameworkExcelRequest, GetFrameworkExcelResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IGetPositionProjectSMAndSalaryTableInteractor _getPositionProjectSMAndSalaryTableInteractor;
        private readonly IGenerateExcelFileInteractor _generateExcelFileInteractor;
        private readonly InfoApp _infoApp;
        private readonly IEnumerable<FrameworkColumnsMainExcelEnum> _listEnumMain;
        private readonly IEnumerable<FrameworkColumnsScenarioExcelEnum> _listEnumScenario;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;

        public GetFrameworkExcelHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor,
            IGetPositionProjectSMAndSalaryTableInteractor getPositionProjectSMAndSalaryTableInteractor,
            IGenerateExcelFileInteractor generateExcelFileInteractor,
            InfoApp infoApp,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _getPositionProjectSMAndSalaryTableInteractor = getPositionProjectSMAndSalaryTableInteractor;
            _generateExcelFileInteractor = generateExcelFileInteractor;
            _infoApp = infoApp;
            _listEnumMain = Enum.GetValues(typeof(FrameworkColumnsMainExcelEnum)) as
                        IEnumerable<FrameworkColumnsMainExcelEnum>;

            _listEnumMain = _listEnumMain.First().GetWithOrder().Select(s =>
            (FrameworkColumnsMainExcelEnum)Enum.Parse(typeof(FrameworkColumnsMainExcelEnum), s));

            _listEnumScenario = Enum.GetValues(typeof(FrameworkColumnsScenarioExcelEnum)) as
                        IEnumerable<FrameworkColumnsScenarioExcelEnum>;

            _listEnumScenario = _listEnumScenario.First().GetWithOrder().Select(s =>
            (FrameworkColumnsScenarioExcelEnum)Enum.Parse(typeof(FrameworkColumnsScenarioExcelEnum), s));

            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;

        }

        public async Task<GetFrameworkExcelResponse> Handle(GetFrameworkExcelRequest request, CancellationToken cancellationToken)
        {

            var unitName = "Todas as Unidades";

            if (request.UnitId.HasValue)
            {
                var unit = await _unitOfWork.GetRepository<Empresas, long>().GetAsync(x => x.Where(s => s.Id == request.UnitId.Value));

                if (unit != null)
                {
                    unitName = string.IsNullOrWhiteSpace(unit.NomeFantasia) ?
                        (string.IsNullOrWhiteSpace(unit.RazaoSocial) ? string.Empty : unit.RazaoSocial) :
                        unit.NomeFantasia;
                }
            }

            var titleName = $"{ModulesSuItemsEnum.Framework.GetDescription()} | {unitName}";

            var sheetName = $"{ModulesSuItemsEnum.Framework.GetDescription()}";
            var fileName = $"{_infoApp.Name}_{sheetName}";

            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            //configuration global labels 
            var configGlobalLabels =
                await _getGlobalLabelsInteractor.Handler(request.ProjectId);

            if (configGlobalLabels.Any())
            {
                var rangeGlobalIds =
                    configGlobalLabels
                    .Where(co => !co.IsChecked)?
                    .Select(s => (int)s.Id)?
                    .ToList();

                if (rangeGlobalIds.Safe().Any())
                {
                    var columnsExp = request.Columns.ToList();

                    columnsExp.AddRange(rangeGlobalIds);

                    request.Columns = columnsExp.Distinct();
                }

            }

            //BaseSalarial table
            List<SalaryBaseFrameworkExcelDTO> salaryBaseList = await QuerySalaryBase(request, permissionUser);

            if (!salaryBaseList.Any())
                return new GetFrameworkExcelResponse
                {
                    File = new byte[0],
                    FileName = fileName
                };

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

            if (columnsData.Any(a => a.IsChecked.HasValue && !a.IsChecked.Value))
            {

                var expColumns = request.Columns.ToList();

                expColumns.AddRange(
                    columnsData
                    .Where(c => c.IsChecked.HasValue && !c.IsChecked.Value).Select(s => (int)s.ColumnId));

                request.Columns = expColumns.Distinct();

            }

            var dictonaryMMMI = new Dictionary<DisplayMMMIEnum, string>();
            if (request.UnitId.HasValue)
            {
                var permissionCompanySM = await _unitOfWork.GetRepository<EmpresaPermissaoSm, long>()
                    .GetAsync(g => g.Where(c => c.EmpresaId == request.UnitId.Value)?
                    .Select(s => new { s.MI, s.MM }));

                if (permissionCompanySM != null)
                {
                    dictonaryMMMI.Add(DisplayMMMIEnum.MM, permissionCompanySM.MM);
                    dictonaryMMMI.Add(DisplayMMMIEnum.MI, permissionCompanySM.MI);
                }
            }

            var subTitleName = string.Empty;

            if (request.IsMI)
            {
                subTitleName = !dictonaryMMMI.Any() ?
                    DisplayMMMIEnum.MI.GetDescription() : dictonaryMMMI[DisplayMMMIEnum.MI];
            }
            else
            {
                subTitleName = !dictonaryMMMI.Any() ?
                        DisplayMMMIEnum.MM.GetDescription() : dictonaryMMMI[DisplayMMMIEnum.MM];
            }


            //header
            var headerExcel = await GetHeader(columnsData, salaryBaseList, request, configGlobalLabels);
            var bodyExcel = GetBody(request, salaryBaseList);

            var fileExcel = await _generateExcelFileInteractor
                        .Handler(new GenerateExcelFileRequest
                        {
                            FileName = fileName,
                            Body = bodyExcel,
                            Header = headerExcel,
                            SheetName = sheetName,
                            TitleSheet = titleName,
                            SubTitleSheet = subTitleName
                        });

            return new GetFrameworkExcelResponse
            {
                FileName = fileName,
                File = fileExcel
            };
        }

        private List<List<ExcelBody>> GetBody(GetFrameworkExcelRequest request, List<SalaryBaseFrameworkExcelDTO> salaryBaseList)
        {

            //body
            var bodyExcel = new List<List<ExcelBody>>();

            //rows
            foreach (var salaryBaseData in salaryBaseList)
            {
                var cols = new List<ExcelBody>();
                //fixed 
                foreach (FrameworkColumnsMainExcelEnum itemMain in _listEnumMain)
                {
                    if (!request.Columns.Any(a => a == (int)itemMain))
                    {
                        var value =
                            salaryBaseData.GetType()
                            .GetProperty(itemMain.ToString())?
                            .GetValue(salaryBaseData);

                        switch (itemMain)
                        {
                            default:
                                cols.Add(new ExcelBody
                                {
                                    Value = value == null ? string.Empty : value.ToString()
                                });
                                break;
                        }
                    }
                }

                //if (request.IsMI)
                //{
                //    var valuePositionSmMI =
                //        positionSmAndSalaryBase
                //        .FirstOrDefault(p => p.SalaryBaseId == salaryBaseData.SalaryBaseId &&
                //                             p.PositionId == salaryBaseData.SMCargoIdMI);

                //    GetBodyScenario(request, cols, valuePositionSmMI, salaryBaseData);
                //}

                //if (request.IsMM)
                //{
                //    var valuePositionSmMM =
                //        positionSmAndSalaryBase.FirstOrDefault(p => p.SalaryBaseId == salaryBaseData.SalaryBaseId && 
                //        p.PositionId == salaryBaseData.SMCargoIdMM);

                //    GetBodyScenario(request, cols, valuePositionSmMM, salaryBaseData);
                //}
                GetBodyScenario(request, cols, salaryBaseData);
                bodyExcel.Add(cols);
            }

            return bodyExcel;
        }

        private void GetBodyScenario(GetFrameworkExcelRequest request, List<ExcelBody> cols, SalaryBaseFrameworkExcelDTO salaryBase)
        {
            foreach (FrameworkColumnsScenarioExcelEnum itemScenario in _listEnumScenario)
            {
                if (!request.Columns.Any(a => a == (int)itemScenario))
                {
                    var salaryBaseItem = salaryBase.GetType()
                                    .GetProperty(itemScenario.ToString())?
                                    .GetValue(salaryBase);


                    switch (itemScenario)
                    {
                        case FrameworkColumnsScenarioExcelEnum.GSM:
                            cols.Add(new ExcelBody
                            {
                                Value = salaryBaseItem == null ? "0" :
                                    Convert.ToDouble(salaryBaseItem).ToString(CultureInfo.InvariantCulture)
                            });
                            break;
                        case FrameworkColumnsScenarioExcelEnum.PercentagemArea:
                            foreach (var valueSalary in salaryBase.SalaryTable)
                            {
                                cols.Add(new ExcelBody
                                {
                                    Value = valueSalary == null ? "0" :
                                   Convert.ToDouble(valueSalary.Value).ToString(CultureInfo.InvariantCulture)
                                });
                            }

                            break;
                        case FrameworkColumnsScenarioExcelEnum.CompareMidPoint:
                            cols.Add(new ExcelBody
                            {
                                Value = salaryBaseItem == null ? "0" :
                                        Convert.ToDouble(salaryBaseItem).ToString(CultureInfo.InvariantCulture)
                            });
                            break;
                        case FrameworkColumnsScenarioExcelEnum.HourlyBasis:
                            cols.Add(new ExcelBody
                            {
                                Value = salaryBaseItem == null ? "0" :
                                Convert.ToDouble(salaryBaseItem).ToString(CultureInfo.InvariantCulture)
                            });
                            break;
                        case FrameworkColumnsScenarioExcelEnum.Salary:
                            cols.Add(new ExcelBody
                            {
                                Value = salaryBaseItem == null ? "0" :
                                Convert.ToDouble(salaryBaseItem).ToString(CultureInfo.InvariantCulture)
                            });
                            break;
                        case FrameworkColumnsScenarioExcelEnum.TypeOfContract:
                            cols.Add(new ExcelBody
                            {
                                Value = salaryBaseItem == null ? string.Empty : salaryBaseItem.ToString()
                            });
                            break;
                        default:
                            cols.Add(new ExcelBody
                            {
                                Value = salaryBaseItem == null ? string.Empty : salaryBaseItem.ToString()
                            });
                            break;
                    }
                }
            }
        }

        private async Task<List<ExcelHeader>> GetHeader(IEnumerable<ColumnDataExcelDTO> columnsData,
            List<SalaryBaseFrameworkExcelDTO> salaryBaseList,
            GetFrameworkExcelRequest request,
            IEnumerable<GlobalLabelsJson> configGlobalLabels)
        {
            var headerMain = new List<ExcelHeader>();

            var dataPositionSM = salaryBaseList.FirstOrDefault();

            //main header
            foreach (FrameworkColumnsMainExcelEnum item in _listEnumMain)
            {
                if (!request.Columns.Any(a => a == (int)item))
                {
                    var columnData =
                                columnsData.FirstOrDefault(f => f.ColumnId == (long)item);


                    switch (item)
                    {
                        default:
                            headerMain.Add(new ExcelHeader
                            {
                                Value = columnData == null ? item.GetDescription() :
                                            columnData.Name,
                            });
                            break;

                    }
                }
            }

            await GetHeaderScenario(columnsData, request, headerMain, dataPositionSM, configGlobalLabels);

            return headerMain;

        }

        private async Task GetHeaderScenario(IEnumerable<ColumnDataExcelDTO> columnsData,
            GetFrameworkExcelRequest request,
            List<ExcelHeader> headerMain,
            SalaryBaseFrameworkExcelDTO dataPositionSM,
            IEnumerable<GlobalLabelsJson> globalLabelsJsons)
        {
            var companyDefaultHeader = await _unitOfWork.GetRepository<EmpresaPermissaoSm, long>().GetAsync(x => x.Where(ep => request.CompaniesId.Contains(ep.EmpresaId)));
            foreach (FrameworkColumnsScenarioExcelEnum item in _listEnumScenario)
            {
                if (!request.Columns.Any(a => a == (int)item))
                {
                    GlobalLabelsJson globalLabel = globalLabelsJsons.FirstOrDefault(fe => fe.Id == (long)item);
                    var columnData = columnsData.FirstOrDefault(f => f.ColumnId == (long)item);
                    switch (item)
                    {
                        case FrameworkColumnsScenarioExcelEnum.GSM:
                            headerMain.Add(new ExcelHeader
                            {
                                Value = globalLabel == null ? item.GetDescription() :
                                string.IsNullOrEmpty(globalLabel.Alias) ? globalLabel.Name : globalLabel.Alias,
                                Type = ExcelFieldType.NumberSimples,
                            });
                            break;
                        case FrameworkColumnsScenarioExcelEnum.PercentagemArea:
                            if (dataPositionSM == null)
                                break;

                            var tracksPercentage = dataPositionSM.SalaryTable;
                            foreach (var trackPercentage in tracksPercentage)
                            {

                                headerMain.Add(new ExcelHeader
                                {
                                    IsBold = true,
                                    Type = ExcelFieldType.NumberSimples,
                                    Value = $"{Math.Round(trackPercentage.Multiplicator * 100, 0).ToString(CultureInfo.InvariantCulture)}%"
                                });
                            }
                            break;
                        case FrameworkColumnsScenarioExcelEnum.CompareMidPoint:
                            headerMain.Add(new ExcelHeader
                            {
                                Value = columnData == null ? item.GetDescription() :
                                        columnData.Name,
                                Type = ExcelFieldType.Percentagem,
                                IsBold = true,
                                IsConditionalFormatting = true,
                                IsCenter = true
                            });
                            break;
                        case FrameworkColumnsScenarioExcelEnum.PositionSm:
                            string name = columnData == null && companyDefaultHeader != null ?
                                companyDefaultHeader.CargoSalaryMark : columnData == null ? item.GetDescription() :
                               columnData.Name;
                            headerMain.Add(new ExcelHeader
                            {
                                Value = name
                            });
                            break;
                        case FrameworkColumnsScenarioExcelEnum.HourlyBasis:
                            headerMain.Add(new ExcelHeader
                            {
                                Value = columnData == null ? item.GetDescription() :
                                        columnData.Name,
                                Type = ExcelFieldType.NumberSimples
                            });
                            break;
                        case FrameworkColumnsScenarioExcelEnum.Salary:
                            headerMain.Add(new ExcelHeader
                            {
                                Value = columnData == null ? item.GetDescription() :
                                        columnData.Name,
                                Type = ExcelFieldType.Money
                            });
                            break;
                        default:
                            headerMain.Add(new ExcelHeader
                            {
                                Value = columnData == null ? item.GetDescription() :
                                        columnData.Name
                            });
                            break;
                    }
                }
            }
        }

        private async Task<IEnumerable<ColumnDataExcelDTO>> QueryColumnsCustom(GetFrameworkExcelRequest request)
        {
            return await _unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
                .GetListAsync(x => x.Where(ues => ues.UsuarioId == request.UserId &&
                 ues.ModuloSmid == (long)ModulesEnum.Positioning &&
                 ues.ModuloSmsubItemId.HasValue && ues.ModuloSmsubItemId.Value == (long)ModulesSuItemsEnum.Framework)
                ?.Select(s => new ColumnDataExcelDTO
                {
                    ColumnId = s.ColunaId,
                    IsChecked = s.Checado,
                    Name = s.Nome
                }));
        }
        private async Task<IEnumerable<DataPositionProjectSMResponse>> GetDataPostionSM(GetFrameworkExcelRequest request,
            List<SalaryBaseFrameworkExcelDTO> salaryBaseList,
            PermissionJson permissionUser)
        {
            IEnumerable<DataPositionProjectSMResponse> positionSm = new List<DataPositionProjectSMResponse>();
            var listDataPositionProjectSMRequest = new List<DataPositionProjectSMRequest>();

            if (request.IsMI)
            {

                var isMIRequest = salaryBaseList.Where(sb => sb.SMCargoIdMI.HasValue)?
                    .Select(s => new DataPositionProjectSMRequest
                    {
                        CompanyId = s.CompanyId,
                        FinalSalary = s.Salary.GetValueOrDefault(0),
                        HoursBase = s.HourlyBasis.GetValueOrDefault(0),
                        PositionId = s.SMCargoIdMI.Value,
                        SalaryBaseId = s.SalaryBaseId
                    });

                if (isMIRequest.Safe().Any())
                    listDataPositionProjectSMRequest.AddRange(isMIRequest);
            }


            if (request.IsMM)
            {
                var isMMRequest = salaryBaseList.Where(sb => sb.SMCargoIdMM.HasValue)?
                    .Select(s => new DataPositionProjectSMRequest
                    {
                        CompanyId = s.CompanyId,
                        FinalSalary = s.Salary.GetValueOrDefault(0),
                        HoursBase = s.HourlyBasis.GetValueOrDefault(0),
                        PositionId = s.SMCargoIdMM.Value,
                        SalaryBaseId = s.SalaryBaseId
                    });

                if (isMMRequest.Safe().Any())
                    listDataPositionProjectSMRequest.AddRange(isMMRequest);
            }

            var listDataPositionProjectSMRequestDistinct =
                          listDataPositionProjectSMRequest
                          .GroupBy(r => new { r.PositionId, r.SalaryBaseId })
                          .Select(g => g.First())
                         .ToList();

            positionSm = await _getPositionProjectSMAndSalaryTableInteractor
                .Handler(listDataPositionProjectSMRequestDistinct,
                request.ProjectId, permissionUser, request.HoursType, request.ContractType);

            return positionSm;
        }
        private async Task<List<SalaryBaseFrameworkExcelDTO>> QuerySalaryBase(GetFrameworkExcelRequest request,
            PermissionJson userJson)
        {
            var term = request.Term == null ? string.Empty : request.Term.ToLower().Trim();

            //filter levels and areas by user
            var levelsExp = userJson.Levels;

            var areasExp = userJson.Areas;

            var groupsExp =
                        userJson.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                        .SubItems;
            var tablesExp =
                        userJson.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                        .SubItems;
            var ignoreSituationPerson = !userJson.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);

            var idsExpProjectsPositions = new List<long>();
            var idsProjectsPositionsTerms = new List<long>();
            var groupIdsTerms = new List<long>();

            //get exp Ids
            if (areasExp.Safe().Any() || groupsExp.Safe().Any())
            {
                idsExpProjectsPositions = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                    .GetListAsync(x => x.Where(cps => cps.Ativo.HasValue &&
                    cps.Ativo.Value &&
                    cps.ProjetoId == request.ProjectId &&
                     (!areasExp.Safe().Any() /*|| areasExp.Contains(cps.Area)*/) &&
                     (!groupsExp.Safe().Any() || groupsExp.Contains(cps.GrupoSmidLocal)))?
                    .Select(s => s.CargoProjetoSmidLocal)
                    .Distinct());
            }

            //term in CargosProjetosSm
            if (!string.IsNullOrWhiteSpace(term))
            {
                var positionsSM = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                                .Include("GruposProjetosSalaryMark")
                                .GetListAsync(x => x.Where(cps => cps.Ativo.HasValue &&
                                cps.Ativo.Value &&
                                cps.ProjetoId == request.ProjectId &&
                                 (string.IsNullOrEmpty(term) ||
                                 (!request.Columns.Any(a => a == (int)FrameworkColumnsMainEnum.GSM) &&
                                 cps.CargosProjetosSmmapeamento.Any(f => f.CargoProjetoSmidLocal == cps.CargoProjetoSmidLocal) ||
                                 cps.CargosProjetosSmmapeamento.FirstOrDefault(f => f.CargoProjetoSmidLocal == cps.CargoProjetoSmidLocal).Gsm.ToString().Equals(term)) ||
                                 (!request.Columns.Any(a => a == (int)FrameworkColumnsMainEnum.Profile) &&
                                        cps.GruposProjetosSalaryMark != null &&
                                        !string.IsNullOrWhiteSpace(cps.GruposProjetosSalaryMark.GrupoSm) &&
                                        cps.GruposProjetosSalaryMark.GrupoSm.ToLower().Trim().Contains(term))
                                 ))?
                                .Select(s => new { s.CargoProjetoSmidLocal, s.GrupoSmidLocal })
                                .Distinct());

                idsProjectsPositionsTerms = positionsSM.Select(res => res.CargoProjetoSmidLocal).Distinct().ToList();
                groupIdsTerms = positionsSM.Select(res => res.GrupoSmidLocal).Distinct().ToList();
            }

            var employeeyCodes = await _unitOfWork.GetRepository<Funcionarios, long>()
                    .GetListAsync(x => x.Where(f => f.Nome.ToLower().Trim().Contains(term) &&
                     !string.IsNullOrWhiteSpace(f.CodigoFuncionario))
                    ?.Select(s => s.CodigoFuncionario));

            var immediateSupervisorCodes = await _unitOfWork.GetRepository<ChefiaImediata, long>()
                    .GetListAsync(x => x.Where(f => f.Nome.ToLower().Trim().Contains(term) &&
                     !string.IsNullOrWhiteSpace(f.CodigoChefiaImediata))
                    ?.Select(s => s.CodigoChefiaImediata));

            var isDesc = request.IsAsc.HasValue ? !request.IsAsc.Value : false;
            var defaultListEnum = Enum.GetValues(typeof(FrameworkColumnsMainEnum)) as
                        IEnumerable<FrameworkColumnsMainEnum>;

            var sortColumnProperty = request.SortColumnId.HasValue ?
                            defaultListEnum.FirstOrDefault(f => (int)f == request.SortColumnId.Value).ToString() :
                            FrameworkColumnsMainEnum.CurrentPosition.ToString();

            var immediateSupervisor = await _unitOfWork.GetRepository<ChefiaImediata, long>()
                                       .GetListAsync(x => x.Where(cf => cf.ProjetoId == request.ProjectId)
                                       .Select(x => new
                                       {
                                           immediateSupervisor = x.CodigoChefiaImediata,
                                           name = x.Nome
                                       }));

            var result = await _unitOfWork.GetRepository<BaseSalariais, long>()
               .Include("Empresa")
               .Include("RegimeContratacao")
               .Include("CmcodeNavigation")
               .Include("Empresa.Funcionarios")
               .Include("Empresa.ProjetosSalaryMark.ChefiaImediata")
               .Include("Empresa.GruposProjetosSalaryMarkMapeamento.GruposProjetosSalaryMark")
                              .GetListAsync(x => x.Where(bs =>
                              bs.Empresa != null && bs.Empresa.GruposProjetosSalaryMarkMapeamento != null &&
                              (!tablesExp.Safe().Any() || !bs.Empresa.GruposProjetosSalaryMarkMapeamento.Any(x => tablesExp.Contains(x.TabelaSalarialIdLocal))) &&
                              bs.CargoIdSMMM.HasValue && bs.CargoIdSMMM.HasValue &&
                                  (!idsProjectsPositionsTerms.Any() ||
                                  idsProjectsPositionsTerms.Contains(bs.CargoIdSMMM.Value) ||
                                  idsProjectsPositionsTerms.Contains(bs.CargoIdSMMI.Value)) &&
                              (!idsExpProjectsPositions.Any() ||
                              !idsExpProjectsPositions.Contains(bs.CargoIdSMMM.Value) ||
                              !idsExpProjectsPositions.Contains(bs.CargoIdSMMI.Value)) &&
                            (request.UnitId.HasValue || request.CompaniesId.Contains(bs.EmpresaId.Value)) &&
                            (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value) &&
                            (string.IsNullOrWhiteSpace(term) ||

                            (!request.Columns.Any(a => a == (int)FrameworkColumnsMainEnum.CurrentPosition) &&
                            !string.IsNullOrWhiteSpace(bs.CargoEmpresa) && bs.CargoEmpresa.ToLower().Trim().Contains(term)) ||

                            (!request.Columns.Any(a => a == (int)FrameworkColumnsMainEnum.Profile) &&
                                bs.Empresa.GruposProjetosSalaryMarkMapeamento.Any(gp => gp.GruposProjetosSalaryMark != null && gp.GruposProjetosSalaryMark.GrupoSm.Equals(term))) ||

                            (!request.Columns.Any(a => a == (int)FrameworkColumnsMainEnum.Company) &&
                            bs.Empresa != null && !string.IsNullOrWhiteSpace(bs.Empresa.NomeFantasia) && bs.Empresa.NomeFantasia.ToLower().Trim().Contains(term)) ||

                            (!request.Columns.Any(a => a == (int)FrameworkColumnsMainEnum.UnitPlace) &&
                            !string.IsNullOrWhiteSpace(bs.Unidade) && bs.Unidade.ToLower().Trim().Contains(term)) ||

                            (!request.Columns.Any(a => a == (int)FrameworkColumnsMainEnum.Business) &&
                            !string.IsNullOrWhiteSpace(bs.Negocio) && bs.Negocio.ToLower().Trim().Contains(term)) ||

                            (!request.Columns.Any(a => a == (int)FrameworkColumnsMainEnum.Setor) &&
                            !string.IsNullOrWhiteSpace(bs.Setor1) && bs.Setor1.ToLower().Trim().Contains(term)) ||

                            (!request.Columns.Any(a => a == (int)FrameworkColumnsMainEnum.TypeOfContract) && bs.RegimeContratacaoId.HasValue &&
                              bs.RegimeContratacao != null && !string.IsNullOrWhiteSpace(bs.RegimeContratacao.Regime) && bs.RegimeContratacao.Regime.Equals(term)) ||

                            (!request.Columns.Any(a => a == (int)FrameworkColumnsMainEnum.HourlyBasis) &&
                            bs.BaseHoraria.HasValue && bs.BaseHoraria.ToString().Equals(term)) ||

                           (!request.Columns.Any(a => a == (int)FrameworkColumnsMainEnum.Employee) &&
                                employeeyCodes.Any() &&
                                employeeyCodes.Any(c => c.Equals(bs.FuncionarioId))) ||

                            (!request.Columns.Any(a => a == (int)FrameworkColumnsMainEnum.ImmediateSupervisor) &&
                                immediateSupervisorCodes.Any() &&
                                immediateSupervisorCodes.Any(c => c.Equals(bs.ChefiaImediata)))

                            ) &&
                            (!levelsExp.Safe().Any() || !levelsExp.Contains(bs.CmcodeNavigation.NivelId)) &&
                            (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active))
               ?.Select(s => new SalaryBaseFrameworkExcelDTO
               {
                   SalaryBaseId = s.Id,
                   CurrentPosition = s.CargoEmpresa,
                   Company = s.Empresa.NomeFantasia,
                   CompanyId = s.EmpresaId.Value,
                   UnitPlace = s.Unidade,
                   Business = s.Negocio,
                   Setor = s.Setor1,
                   Employee = s.Empresa.Funcionarios.Any(a => a.CodigoFuncionario.Equals(s.FuncionarioId)) ?
                              s.Empresa.Funcionarios.FirstOrDefault(f => f.CodigoFuncionario.Equals(s.FuncionarioId)).Nome : string.Empty,

                   ImmediateSupervisor = s.ChefiaImediata,
                   HourlyBasis = s.BaseHoraria,
                   Salary = s.SalarioFinalSm,
                   SMCargoIdMM = s.CargoIdSMMM,
                   SMCargoIdMI = s.CargoIdSMMI,
                   FuncionarioID = s.FuncionarioId,
                   TypeOfContract = s.RegimeContratacao != null ? s.RegimeContratacao.Regime : string.Empty,
                   Profile = string.Empty,
                   GSM = 0,
                   PositionSm = string.Empty,
                   CompareMidPoint = 0
               })
               .OrderBy(sortColumnProperty, isDesc));

            //get all smIds
            IEnumerable<DataPositionProjectSMResponse> positionSm = await GetDataPostionSM(request, result, userJson);

            result.ForEach(salary =>
            {
                salary.ImmediateSupervisor = immediateSupervisor.FirstOrDefault(imm => imm.immediateSupervisor.Equals(salary.ImmediateSupervisor)) != null ?
                                             immediateSupervisor.FirstOrDefault(imm => imm.immediateSupervisor.Equals(salary.ImmediateSupervisor)).name : string.Empty;

                var positionValues = request.IsMM ?
                positionSm.FirstOrDefault(f => f.PositionId == salary.SMCargoIdMM && f.SalaryBaseId == salary.SalaryBaseId) :
                positionSm.FirstOrDefault(f => f.PositionId == salary.SMCargoIdMI && f.SalaryBaseId == salary.SalaryBaseId);

                if (positionValues != null)
                {
                    salary.CompareMidPoint = positionValues.CompareMidPoint;
                    //salary.PositionId = positionValues.PositionId;
                    salary.PositionSm = positionValues.PositionSm;
                    salary.Profile = positionValues.Profile;
                    salary.ProfileId = positionValues.ProfileId;
                    salary.GSM = positionValues.GSM;
                    salary.SalaryTable = positionValues.SalaryTable;
                    salary.SalaryBaseId = positionValues.SalaryBaseId;
                }
            });

            if (groupIdsTerms.Any())
                result = result.Where(x => string.IsNullOrWhiteSpace(term) ||
                                          (!request.Columns.Any(a => a == (int)FrameworkColumnsMainEnum.Profile) &&
                                           x.Profile.ToLower().Trim().Contains(term))).ToList();
            switch (request.SortColumnId)
            {
                case (int)FrameworkColumnsMainEnum.ImmediateSupervisor:
                    result = isDesc ? result.OrderByDescending(x => x.ImmediateSupervisor).ToList() : result.OrderBy(x => x.ImmediateSupervisor).ToList();
                    break;
                case (int)FrameworkColumnsMainEnum.GSM:
                    result = isDesc ? result.OrderByDescending(x => x.GSM).ToList() : result.OrderBy(x => x.GSM).ToList();
                    break;
                case (int)FrameworkColumnsMainEnum.Profile:
                    result = isDesc ? result.OrderByDescending(x => x.Profile).ToList() : result.OrderBy(x => x.Profile).ToList();
                    break;
                case (int)FrameworkColumnsMainEnum.PositionSm:
                    result = isDesc ? result.OrderByDescending(x => x.PositionSm).ToList() : result.OrderBy(x => x.PositionSm).ToList();
                    break;
                case (int)FrameworkColumnsMainEnum.CompareMidPoint:
                    result = isDesc ? result.OrderByDescending(x => x.CompareMidPoint).ToList() : result.OrderBy(x => x.CompareMidPoint).ToList();
                    break;
            }

            return result;
        }
    }
}
