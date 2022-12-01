using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors;
using SM.Application.Interactors.Interfaces;
using SM.Application.Interactors.Utils;
using SM.Domain.Enum;
using SM.Domain.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SM.Application.Helpers;
using SM.Domain.Helpers;
using SM.Domain.Common;

namespace SM.Application.Position.Queries
{
    public class GetAllPositionsExcelRequest
        : IRequest<GetAllPositionsExcelResponse>
    {
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public long UserId { get; set; }
        public long ProjectId { get; set; }
        public long TableId { get; set; }
        public string Term { get; set; }
        public bool ShowJustWithOccupants { get; set; } = false;
        public ContractTypeEnum ContractType { get; set; } = ContractTypeEnum.CLT;
        public DataBaseSalaryEnum HoursType { get; set; } = DataBaseSalaryEnum.MonthSalary;
        public IEnumerable<int> Columns { get; set; } = new List<int>(); //using to share (EXP)
        public bool? IsAsc { get; set; } = null;
        public int? SortColumnId { get; set; } = null;


    }

    public class GetAllPositionsExcelResponse
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
    }

    public class ProjectPositionsExcel
    {
        public long PositionSMLocalId { get; set; }
        public string PositionSalaryMark { get; set; }
        public string Profile { get; set; }
        public int LevelId { get; set; }
        public string Level { get; set; }
        public string Parameter01 { get; set; }
        public string Parameter02 { get; set; }
        public string Parameter03 { get; set; }
        public double HourBase { get; set; }
        public long GSM { get; set; }
        public string TechnicalAdjustment { get; set; }
        public long GroupId { get; set; }
        public string Area { get; set; }
        public string Smcode { get; set; }
        public IEnumerable<SalaryTableAverageExcel> SalaryTableValues { get; set; }
    }

    public class SalaryTableAverageExcel
    {
        public double Value { get; set; }
        public double Multiplicator { get; set; }

    }
    public class LevelCompanyAllPositionsExcelDTO
    {
        public int LevelId { get; set; }
        public string Level { get; set; }

    }

    public class ColumnDataPositionExcelDTO
    {
        public long ColumnId { get; set; }
        public bool? IsChecked { get; set; }
        public string Name { get; set; }

    }

    public class GetAllPositionsExcelHandler
        : IRequestHandler<GetAllPositionsExcelRequest, GetAllPositionsExcelResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IGenerateExcelFileInteractor _generateExcelFileInteractor;
        private readonly IEnumerable<PositionProjectColumnsEnum> _listEnums;
        private readonly IMultiplierFactorHourlyInteractor _multiplierFactorHourlyInteractor;
        private readonly IMultiplierFactorTypeContratcInteractor _multiplierFactorTypeContratcInteractor;
        private readonly InfoApp _infoApp;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;

        public GetAllPositionsExcelHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor,
            IGenerateExcelFileInteractor generateExcelFileInteractor,
            InfoApp infoApp,
            IMultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor,
            IMultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _generateExcelFileInteractor = generateExcelFileInteractor;
            _listEnums = Enum.GetValues(typeof(PositionProjectColumnsEnum)) as IEnumerable<PositionProjectColumnsEnum>;

            _listEnums = _listEnums.First().GetWithOrder().Select(s =>
            (PositionProjectColumnsEnum)Enum.Parse(typeof(PositionProjectColumnsEnum), s));

            _multiplierFactorHourlyInteractor = multiplierFactorHourlyInteractor;
            _multiplierFactorTypeContratcInteractor = multiplierFactorTypeContratcInteractor;
            _infoApp = infoApp;
            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;
        }
        public async Task<GetAllPositionsExcelResponse> Handle(GetAllPositionsExcelRequest request, CancellationToken cancellationToken)
        {
            var term = request.Term == null ? string.Empty : request.Term.ToLower().Trim();

            var projectData = await
                            _unitOfWork.GetRepository<ProjetosSalaryMark, long>().GetAsync(g => g.Where(f => f.Id == request.ProjectId));

            var projectName = projectData == null ? string.Empty : projectData.Projeto;



            var sheetName = "Arquitetura Cargos";
            var fileName = $"{_infoApp.Name}_{sheetName}";

            List<ProjectPositionsExcel> projectPositionsExcelList = await QueryPositionSM(request, term);

            if (!projectPositionsExcelList.Any())
                return new GetAllPositionsExcelResponse
                {
                    File = new byte[0],
                    FileName = fileName
                };

            request.Columns = new List<int>();

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

            await GetLevelsCompanies(request, projectPositionsExcelList);

            var columnsData = await QueryColumnsCustom(request, configGlobalLabels.ToList());

            if (columnsData.Any(a => a.IsChecked.HasValue && !a.IsChecked.Value))
            {

                var expColumns = request.Columns.ToList();

                expColumns.AddRange(
                    columnsData
                    .Where(c => c.IsChecked.HasValue && !c.IsChecked.Value).Select(s => (int)s.ColumnId));

                request.Columns = expColumns.Distinct();

            }



            //salary Base 
            var projectPositionsListIds = projectPositionsExcelList.Select(pp => pp.PositionSMLocalId).Distinct();

            var groupIds = projectPositionsExcelList.Select(s => s.GroupId).Distinct();

            var groupsIdxTableId = await _unitOfWork.GetRepository<GruposProjetosSalaryMarkMapeamento, long>()
                            .GetListAsync(x => x.Where(gpm => gpm.ProjetoId == request.ProjectId &&
                                gpm.TabelaSalarialIdLocal == request.TableId &&
                               gpm.Ativo.HasValue && gpm.Ativo.Value &&
                               request.CompaniesId.Contains(gpm.EmpresaId))?
                               .Where(gpm1 => !request.UnitId.HasValue || request.UnitId.Value == gpm1.EmpresaId)?
                               .Where(gpm2 => groupIds.Contains(gpm2.GrupoProjetoSmidLocal))?
                              .Select(s => new
                              {
                                  GroupId = s.GrupoProjetoSmidLocal,
                                  MaxTrack = s.FaixaIdSuperior,
                                  MinTrack = s.FaixaIdInferior
                              }));




            if (!groupsIdxTableId.Any())
                return new GetAllPositionsExcelResponse
                {
                    File = new byte[0],
                    FileName = fileName
                };

            var groupsIdxTableIdMaxTrack = groupsIdxTableId.Max(s => s.MaxTrack);
            var groupsIdxTableIdMinTrack = groupsIdxTableId.Min(s => s.MinTrack);

            var gsms = projectPositionsExcelList.Select(s => s.GSM).Distinct();

            var salaryValuesTables = await _unitOfWork.GetRepository<TabelasSalariaisValores, long>()
                                        .GetListAsync(x => x.Where(tv => tv.ProjetoId == request.ProjectId &&
                                         request.TableId == tv.TabelaSalarialIdLocal)?
                                        .Where(tv1 => gsms.Contains(tv1.Grade))?
                                        .Select(s => new
                                        {
                                            GSM = s.Grade,
                                            MidPoint = s.FaixaMidPoint
                                        }));

            if (!salaryValuesTables.Any())
                return new GetAllPositionsExcelResponse
                {
                    File = new byte[0],
                    FileName = fileName
                };

            var rangeTableSalary = await _unitOfWork.GetRepository<TabelasSalariaisFaixas, long>()
                 .GetListAsync(x => x.Where(tsv => tsv.ProjetoId == request.ProjectId &&
                 request.TableId == tsv.TabelaSalarialIdLocal &&
                 tsv.FaixaSalarialId >= groupsIdxTableIdMinTrack && tsv.FaixaSalarialId <= groupsIdxTableIdMaxTrack)
                 ?.Select(s => new
                 {
                     TrackId = s.FaixaSalarialId,
                     TableId = s.TabelaSalarialIdLocal,
                     FactorMulti = s.AmplitudeMidPoint,
                 }).OrderBy(o => o.FactorMulti));

            if (!rangeTableSalary.Any())
                return new GetAllPositionsExcelResponse
                {
                    File = new byte[0],
                    FileName = fileName
                };

            var minMidPoint = await _unitOfWork.GetRepository<TabelasSalariaisGrades, long>()
               .GetAsync(g => g.Where(t => t.ProjetoId == request.ProjectId && t.TabelaSalarialIdLocal == request.TableId)
               .Select(s => s.MenorSalario));

            var multiplierFactorHourlyInteractor = await _multiplierFactorHourlyInteractor.Handler(request.ProjectId, request.HoursType);
            var multiplierFactorTypeContratcInteractor = await _multiplierFactorTypeContratcInteractor.Handler(request.ProjectId, request.ContractType);

            foreach (var positonSM in projectPositionsExcelList)
            {
                var salaryTableValues = new List<SalaryTableAverageExcel>();

                var midPoint = salaryValuesTables
                    .FirstOrDefault(f => f.GSM == positonSM.GSM)?
                    .MidPoint;

                var maxTrack = groupsIdxTableId
                    .FirstOrDefault(f => f.GroupId == positonSM.GroupId)?
                    .MaxTrack;

                var minTrack = groupsIdxTableId
                    .FirstOrDefault(f => f.GroupId == positonSM.GroupId)?
                    .MinTrack;

                if (request.HoursType == DataBaseSalaryEnum.HourSalary)
                {
                    multiplierFactorHourlyInteractor = 1 / positonSM.HourBase;
                }

                foreach (var item in rangeTableSalary)
                {
                    //default value
                    var midPointResult = midPoint.GetValueOrDefault(0) *
                        item.FactorMulti;

                    if (item.TrackId >= minTrack && item.TrackId <= maxTrack &&
                        midPointResult >= minMidPoint)
                    {
                        var fixMidPoint = midPointResult *
                                            multiplierFactorHourlyInteractor *
                                            multiplierFactorTypeContratcInteractor.GetValueOrDefault(0);

                        salaryTableValues.Add(new SalaryTableAverageExcel
                        {
                            Multiplicator = item.FactorMulti,
                            Value = fixMidPoint
                        });
                    }
                    else
                    {
                        salaryTableValues.Add(new SalaryTableAverageExcel
                        {
                            Multiplicator = item.FactorMulti,
                            Value = 0
                        });

                    }

                }

                positonSM.SalaryTableValues = salaryTableValues;
            }


            //header
            var headerDinamic = projectPositionsExcelList.FirstOrDefault().SalaryTableValues;

            var headerExcel = await GetHeader(request, columnsData,
                headerDinamic);


            var bodyExcel = GetBody(request, projectPositionsExcelList);

            var fileExcel = await _generateExcelFileInteractor.Handler(new GenerateExcelFileRequest
            {
                FileName = fileName,
                Body = bodyExcel,
                Header = headerExcel,
                SheetName = sheetName,
                TitleSheet = sheetName
            });

            return new GetAllPositionsExcelResponse
            {
                FileName = fileName,
                File = fileExcel
            };
        }

        private List<List<ExcelBody>> GetBody(GetAllPositionsExcelRequest request,
            List<ProjectPositionsExcel> projectPositionsList)
        {
            var result = new List<List<ExcelBody>>();

            foreach (var projectPosition in projectPositionsList)
            {
                var listBodyLine = new List<ExcelBody>();

                //fixed columns
                foreach (PositionProjectColumnsEnum item in _listEnums)
                {
                    if (!request.Columns.Any(a => a == (int)item))
                    {
                        var value = projectPosition
                           .GetType()
                           .GetProperty(item.ToString())
                           .GetValue(projectPosition);

                        listBodyLine.Add(new ExcelBody
                        {
                            Value = value == null ? string.Empty : value.ToString()
                        });
                    }
                }

                //dinamic columns
                if (projectPosition.SalaryTableValues.Any())
                {

                    foreach (var tbg in projectPosition.SalaryTableValues)
                    {
                        listBodyLine.Add(new ExcelBody
                        {
                            Value = tbg.Value.ToString(CultureInfo.InvariantCulture)
                        });
                    }
                }

                result.Add(listBodyLine);
            }
            return result;

        }

        private async Task<List<ExcelHeader>> GetHeader(GetAllPositionsExcelRequest request,
                    IEnumerable<ColumnDataPositionExcelDTO> columnsData,
                    IEnumerable<SalaryTableAverageExcel> headerDinamic)
        {
            var companyDefaultHeader = await _unitOfWork.GetRepository<EmpresaPermissaoSm, long>().GetAsync(x => x.Where(ep => request.CompaniesId.Contains(ep.EmpresaId)));
            var listHeader = new List<ExcelHeader>();

            foreach (PositionProjectColumnsEnum item in _listEnums)
            {
                var columnData =
                        columnsData.FirstOrDefault(f => f.ColumnId == (long)item);

                if (!request.Columns.Any(a => a == (int)item))
                {
                    switch (item)
                    {
                        case PositionProjectColumnsEnum.HourBase:
                            listHeader.Add(new ExcelHeader
                            {
                                Type = ExcelFieldType.NumberSimples,
                                Value = columnData == null ? item.GetDescription() :
                                            columnData.Name,
                                IsBold = true
                            });
                            break;
                        case PositionProjectColumnsEnum.GSM:
                            listHeader.Add(new ExcelHeader
                            {
                                Type = ExcelFieldType.NumberSimples,
                                Value = columnData == null ? item.GetDescription() :
                                            columnData.Name,
                            });
                            break;
                        case PositionProjectColumnsEnum.PositionSalaryMark:
                            string name = columnData == null && companyDefaultHeader != null ?
                               companyDefaultHeader.CargoSalaryMark : columnData == null ? item.GetDescription() :
                               columnData.Name;
                            listHeader.Add(new ExcelHeader
                            {
                                Value = name
                            });
                            break;
                        case PositionProjectColumnsEnum.Parameter02:
                            listHeader.Add(new ExcelHeader
                            {
                                Value = columnData == null ? item.GetDescription() : columnData.Name,
                                Width = 50
                            });

                            break;
                        default:
                            listHeader.Add(new ExcelHeader
                            {
                                Value = columnData == null ? item.GetDescription() :
                                            columnData.Name,
                            });
                            break;
                    }
                }
            }

            //dinamic columns

            var table =
                        await _unitOfWork.GetRepository<TabelasSalariais, long>().GetAsync(g => g.Where(x => x.TabelaSalarialIdLocal == request.TableId));

            foreach (var hd in headerDinamic)
            {
                listHeader.Add(new ExcelHeader
                {
                    Type = ExcelFieldType.NumberSimples,
                    Value = $"{Math.Round(hd.Multiplicator * 100, 0)}%",
                    IsBold = true,
                    GroupName = table == null ? string.Empty : table.TabelaSalarial
                });
            }

            return listHeader;
        }

        private async Task GetLevelsCompanies(GetAllPositionsExcelRequest request,
            List<ProjectPositionsExcel> projectPositionsList)
        {
            //levels company
            var levelsIdFromPositionSM = projectPositionsList.Select(s => s.LevelId);
            var levelsCompanies = await _unitOfWork.GetRepository<NivelEmpresas, long>()
                .GetListAsync(x => x.Where(ne => request.CompaniesId.Contains(ne.EmpresaId) &&
                 levelsIdFromPositionSM.Contains(ne.NivelId))
                .Where(bs1 => !request.UnitId.HasValue || bs1.EmpresaId == request.UnitId.Value)?
                .Select(s => new LevelCompanyAllPositionsExcelDTO
                {
                    LevelId = s.NivelId,
                    Level = s.NivelEmpresa
                }));

            if (levelsCompanies.Any())
            {
                levelsCompanies = levelsCompanies.GroupBy(g => g.LevelId, (key, value) =>
                new LevelCompanyAllPositionsExcelDTO
                {
                    LevelId = key,
                    Level = value.FirstOrDefault().Level
                }).ToList();
            }

            foreach (var projectPosition in projectPositionsList)
            {
                var level = levelsCompanies
                    .FirstOrDefault(f => f.LevelId == projectPosition.LevelId);

                projectPosition.Level = level == null ? string.Empty : level.Level;
            }
        }

        private async Task<List<ProjectPositionsExcel>> QueryPositionSM(GetAllPositionsExcelRequest request, string term)
        {

            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            var areasExp = permissionUser.Areas;
            var levelsExp = permissionUser.Levels;

            var groupsExp =
                            permissionUser.Contents?
                            .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                            .SubItems;
            var ignoreSituationPerson = !permissionUser.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);

            bool displayAllOccupants = permissionUser.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.Positions);

            request.ShowJustWithOccupants = displayAllOccupants ? !displayAllOccupants : request.ShowJustWithOccupants;

            var smIds = new List<long>();


            var isDesc = request.IsAsc.HasValue ? !request.IsAsc.Value : false;
            var sortColumnProperty = request.SortColumnId.HasValue ?
                _listEnums.FirstOrDefault(f => (int)f == request.SortColumnId.Value).ToString() :
                PositionProjectColumnsEnum.PositionSalaryMark.ToString();

            if (request.ShowJustWithOccupants)
            {
                smIds = await _unitOfWork.GetRepository<BaseSalariais, long>()
                           .GetListAsync(g => g.Where(bs => bs.CargoIdSMMM.HasValue &&
                            bs.RegimeContratacaoId.HasValue &&
                           (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active) &&
                           (request.UnitId.HasValue || request.CompaniesId.Contains(bs.EmpresaId.Value)) &&
                           (!request.UnitId.HasValue || bs.EmpresaId.Value == request.UnitId.Value))?
                           .Select(s => s.CargoIdSMMM.Value)
                           .Distinct());
            }

            return null;
            //return await _unitOfWork.GetRepository<CargosProjetosSm, long>()
            //                       .Include("AjusteTecnicoMotivo")
            //                       .Include("GruposProjetosSalaryMark")
            //                       .Include("CmcodeNavigation")
            //                       .Include("CargosProjetosSmmapeamento")
            //                       .GetListAsync(x => x.Where(s => (s.Ativo.HasValue && s.Ativo.Value) &&
            //                       s.ProjetoId == request.ProjectId &&
            //                       s.CargosProjetosSmmapeamento.Any(cpm => cpm.TabelaSalarialIdLocal == request.TableId) &&
            //                       (!request.ShowJustWithOccupants || (smIds.Any() && smIds.Contains(s.CargoProjetoSmidLocal))) &&
            //                       (
            //                           string.IsNullOrWhiteSpace(term) ||
            //                           (!request.Columns.Any(a => a == (int)PositionProjectColumnsEnum.PositionSalaryMark) && !string.IsNullOrWhiteSpace(s.CargoSm) &&
            //                           s.CargoSm.ToLower().Trim().Contains(term)) ||

            //                           (!request.Columns.Any(a => a == (int)PositionProjectColumnsEnum.Profile) &&
            //                           s.GruposProjetosSalaryMark != null &&
            //                           string.IsNullOrWhiteSpace(s.GruposProjetosSalaryMark.GrupoSm) && s.GruposProjetosSalaryMark.GrupoSm.ToLower().Trim().Contains(term)) ||

            //                           (!request.Columns.Any(a => a == (int)PositionProjectColumnsEnum.Level) &&
            //                           !string.IsNullOrWhiteSpace(s.CmcodeNavigation.Nivel.Rotulo) && s.CmcodeNavigation.Nivel.Rotulo.ToLower().Trim().Contains(term)) ||

            //                           (!request.Columns.Any(a => a == (int)PositionProjectColumnsEnum.TechnicalAdjustment) &&
            //                           s.BaseHoraria.ToString().ToLower().Trim().Contains(term)) ||
            //                           //(s.AjusteTecnicoMotivo != null && string.IsNullOrWhiteSpace(s.AjusteTecnicoMotivo.AjusteTecnicoMotivo1) && s.AjusteTecnicoMotivo.AjusteTecnicoMotivo1.ToLower().Trim().Contains(term)) ||

            //                           (!request.Columns.Any(a => a == (int)PositionProjectColumnsEnum.Smcode) &&
            //                           !string.IsNullOrWhiteSpace(s.Smcode) && s.Smcode.ToLower().Trim().Contains(term)) ||

            //                           (!request.Columns.Any(a => a == (int)PositionProjectColumnsEnum.GSM) &&
            //                           //s.Gsm.ToString().Equals(term)) ||

            //                           (!request.Columns.Any(a => a == (int)PositionProjectColumnsEnum.Area) &&
            //                           !string.IsNullOrWhiteSpace(s.Area) && s.Area.ToLower().Trim().Contains(term)) ||

            //                           (!request.Columns.Any(a => a == (int)PositionProjectColumnsEnum.Parameter01) &&
            //                           !string.IsNullOrWhiteSpace(s.Parametro1) && s.Parametro1.ToLower().Trim().Contains(term)) ||

            //                           (!request.Columns.Any(a => a == (int)PositionProjectColumnsEnum.Parameter02) &&
            //                           !string.IsNullOrWhiteSpace(s.Parametro2) && s.Parametro2.ToLower().Trim().Contains(term)) ||

            //                           (!request.Columns.Any(a => a == (int)PositionProjectColumnsEnum.Parameter03) &&
            //                           !string.IsNullOrWhiteSpace(s.Parametro3) && s.Parametro3.ToLower().Trim().Contains(term))
            //                       ) &&
            //                       (!groupsExp.Safe().Any() || !groupsExp.Contains(s.GrupoSmidLocal)) &&
            //                       (!areasExp.Safe().Any() || !areasExp.Contains(s.Area)) &&
            //                       (!levelsExp.Safe().Any() || !levelsExp.Contains(s.CmcodeNavigation.NivelId)))?
            //                                           .Select(s => new ProjectPositionsExcel
            //                                           {
            //                                               PositionSMLocalId = s.CargoProjetoSmidLocal,
            //                                               PositionSalaryMark = s.CargoSm,
            //                                               Profile = s.GruposProjetosSalaryMark != null ? s.GruposProjetosSalaryMark.GrupoSm : string.Empty,
            //                                               LevelId = s.CmcodeNavigation.NivelId,
            //                                               Parameter01 = s.Parametro1,
            //                                               Parameter03 = s.Parametro3,
            //                                               Parameter02 = s.Parametro2,
            //                                               HourBase = s.BaseHoraria,
            //                                               GSM = s.Gsm,
            //                                               Area = s.Area,
            //                                               Smcode = s.Smcode,
            //                                               GroupId = s.GrupoSmidLocal,
            //                                               TechnicalAdjustment = s.AjusteTecnicoMotivo != null ?
            //                                               s.AjusteTecnicoMotivo.AjusteTecnicoMotivo1 : string.Empty,
            //                                           })
            //                                           .OrderBy(sortColumnProperty, isDesc)
            //                                     );
        }

        private async Task<IEnumerable<ColumnDataPositionExcelDTO>> QueryColumnsCustom(GetAllPositionsExcelRequest request,
        List<GlobalLabelsJson> globalLabelsJsons)
        {
            var result = await _unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
                .GetListAsync(x => x.Where(ues => ues.UsuarioId == request.UserId &&
                 ues.ModuloSmid == (long)ModulesEnum.Position &&
                 ues.ModuloSmsubItemId.HasValue && ues.ModuloSmsubItemId.Value == (long)ModulesSuItemsEnum.Architecture)
                ?.Select(s => new ColumnDataPositionExcelDTO
                {
                    ColumnId = s.ColunaId,
                    IsChecked = s.Checado,
                    Name = s.Nome
                }));

            if (result.Count <= 0)
            {
                globalLabelsJsons.ForEach(gb =>
                {
                    result.Add(new ColumnDataPositionExcelDTO { ColumnId = gb.Id, Name = gb.Alias, IsChecked = gb.IsChecked });
                });
                return result;
            }

            result.ForEach(f =>
            {
                var globalLabel = globalLabelsJsons.FirstOrDefault(fe => fe.Id == f.ColumnId);
                if (globalLabel != null)
                    f.Name = globalLabel.Alias;
            });

            var resultId = result.Select(x => x.ColumnId);
            var globalLabelFiltered = globalLabelsJsons.Where(x => !resultId.Contains(x.Id))
                                                       .Select(x => new ColumnDataPositionExcelDTO { ColumnId = x.Id, IsChecked = x.IsChecked, Name = x.Alias });
            result.AddRange(globalLabelFiltered);

            return result;
        }
    }


}
