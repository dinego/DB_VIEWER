using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Interactors;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using SM.Domain.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Repositories.Extensions;

namespace SM.Application.Position.Queries
{
    public class GetMapPositionExcelRequest
        : IRequest<GetMapPositionExcelResponse>
    {
        public long ProjectId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long? UnitId { get; set; }
        public long UserId { get; set; }
        public bool ShowJustWithOccupants { get; set; } = false;
        public bool RemoveRowsEmpty { get; set; } = false;
        public string Term { get; set; }
        public long? GroupId { get; set; } // null is all groupIds
        public long? TableId { get; set; }// null is all TablelIds
        public DisplayByMapPositionEnum DisplayBy { get; set; } = DisplayByMapPositionEnum.AxisCarreira;
        public IEnumerable<string> Columns { get; set; } = new List<string>(); //using to share (EXP)
        public bool? IsAsc { get; set; } = null;
    }
    public class GetMapPositionExcelResponse
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
    }

    public class GetMapPositionExcelDTO
    {
        public int GSM { get; set; }
        public string SMPosition { get; set; }
        public long SMPositionId { get; set; }
        public string AxisCarreira { get; set; }
        public string Area { get; set; }
        public string Parameter3 { get; set; }
        public string Parameter2 { get; set; }
        public string Parameter1 { get; set; }
        public long CmCode { get; set; }
        public long Id { get; set; }
        public long? LevelId { get; set; }

    }
    public class SubItemsExcelDTO
    {
        public long PositionSMId { get; set; }
        public IEnumerable<SubItemsMapExcel> SubItems { get; set; }

    }
    public class SubItemsMapExcel
    {
        public string Position { get; set; }
        public string OccupantType { get; set; }
        public int? OccupantTypeId { get; set; }
        public bool? OccupantPJ { get; set; } = null;
        public bool? OccupantCLT { get; set; } = null;
        public int Amount { get; set; }
        public long CompanyId { get; set; }
    }

    public class BodyMap
    {
        public string Value { get; set; }
        public bool IsOccupants { get; set; } = false;
    }
    public class GetMapPositionExcelHandler
        : IRequestHandler<GetMapPositionExcelRequest, GetMapPositionExcelResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly InfoApp _infoApp;
        private readonly IGenerateExcelFileInteractor _generateExcelFileInteractor;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;

        public GetMapPositionExcelHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor,
            IGenerateExcelFileInteractor generateExcelFileInteractor,
            InfoApp infoApp,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _infoApp = infoApp;
            _generateExcelFileInteractor = generateExcelFileInteractor;
            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;
        }
        public async Task<GetMapPositionExcelResponse>
            Handle(GetMapPositionExcelRequest request, CancellationToken cancellationToken)
        {
            var projectData = await
                           _unitOfWork.GetRepository<ProjetosSalaryMark, long>().GetAsync(g => g.Where(f => f.Id == request.ProjectId));

            var projectName = projectData == null ? string.Empty : projectData.Projeto;

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



            var sheetName = $"{ModulesEnum.Position.GetDescription()} | {projectName}";
            var titleName = $"{ModulesEnum.Position.GetDescription()} | {projectName}|{unitName}";
            var fileName = $"{_infoApp.Name}_{sheetName}";

            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            var tablesExp = permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                        .SubItems;
            List<int> gsms = await GetGsms(request, tablesExp);

            var areasExp = permissionUser.Areas;
            var levelsExp = permissionUser.Levels;
            var displayAllOccupants = permissionUser.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.Positions);
            request.ShowJustWithOccupants = displayAllOccupants ? displayAllOccupants : request.ShowJustWithOccupants;

            //configuration global labels 
            var configGlobalLabels = await _getGlobalLabelsInteractor.Handler(request.ProjectId);

            var projectPostions = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                .Include("CmcodeNavigation")
                .Include("GruposProjetosSalaryMark.GruposProjetosSalaryMarkMapeamento")
                .GetListAsync(x => x.Where(cp => cp.ProjetoId == request.ProjectId &&
                 cp.Ativo.HasValue && cp.Ativo.Value &&
                  cp.GruposProjetosSalaryMark
                                 .GruposProjetosSalaryMarkMapeamento
                                    .Any(a => a.GrupoProjetoSmidLocal == cp.GrupoSmidLocal &&
                                        (request.UnitId.HasValue || request.CompaniesId.Contains(a.EmpresaId)) &&
                                        (!request.UnitId.HasValue || request.UnitId == a.EmpresaId)) &&
                 (!request.TableId.HasValue || cp.GruposProjetosSalaryMark
                 .GruposProjetosSalaryMarkMapeamento.Any(s => s.TabelaSalarialIdLocal == request.TableId.Value)) &&
                 (!request.GroupId.HasValue || cp.GrupoSmidLocal == request.GroupId.Value) &&
                 (!areasExp.Safe().Any() /*|| !areasExp.Contains(cp.Area)*/) &&
                 (!levelsExp.Safe().Any() || !levelsExp.Contains(cp.CmcodeNavigation.NivelId)) &&
                 (!request.UnitId.HasValue || (cp.Projeto != null && cp.Projeto.ProjetosSalaryMarkEmpresas.Any(psm => psm.EmpresaId == request.UnitId.Value))))
                .Select(s => new GetMapPositionExcelDTO
                {
                    Id = s.Id,
                    //GSM = s.Gsm,
                    //Area = s.Area,
                    //AxisCarreira = s.EixoCarreira,
                    SMPosition = s.CargoSm,
                    SMPositionId = s.CargoProjetoSmidLocal,
                    CmCode = s.Cmcode,
                    //Parameter3 = s.Parametro3,
                    //Parameter2 = s.Parametro2,
                    //Parameter1 = s.Parametro1,
                    LevelId = s.CmcodeNavigation != null ? s.CmcodeNavigation.NivelId : (long?)null
                }));

            if (!projectPostions.Any())
                throw new Exception("Não foi possivel determinar os dados da header");

            //use in BaseSalarial
            var positionSmIDs = projectPostions.Select(s => s.SMPositionId)
                .Distinct().ToList();

            var ignoreSituationPerson = !permissionUser.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);

            //BaseSalarial 
            var salaryBase = await _unitOfWork.GetRepository<BaseSalariais, long>()
                .Include("RegimeContratacao")
                .GetListAsync(x => x
                .Where(bs1 => bs1.CargoIdSMMM.HasValue &&
                              positionSmIDs.Contains(bs1.CargoIdSMMM.Value) &&
                              (request.UnitId.HasValue || request.CompaniesId.Contains(bs1.EmpresaId.Value)) &&
                              (!request.UnitId.HasValue || bs1.EmpresaId.Value == request.UnitId.Value) &&
                              (ignoreSituationPerson || bs1.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active))?
                .Select(val => new
                {
                    PositionSMId = val.CargoIdSMMM.Value,
                    PositionCompany = val.CargoEmpresa,
                    CompanyId = val.EmpresaId.Value,
                    OccupantType = val.RegimeContratacao.Regime,
                    OccupantTypeId = val.RegimeContratacaoId,
                }));

            //find subitems  
            var salaryBaseData = new List<SubItemsExcelDTO>();
            if (salaryBase.Any())
            {
                salaryBaseData = salaryBase
                .GroupBy(g => g.PositionSMId, (key, value) => new SubItemsExcelDTO
                {
                    PositionSMId = key,
                    SubItems = value.Select(val => new SubItemsMapExcel
                    {
                        Position = val.PositionCompany,
                        CompanyId = val.CompanyId,
                        OccupantType = val.OccupantType,
                        OccupantTypeId = val.OccupantTypeId
                    })
                }).ToList();
            }

            IEnumerable<IGrouping<string, GetMapPositionExcelDTO>> projectPostionsGroupy =
                new List<IGrouping<string, GetMapPositionExcelDTO>>();

            var headersRequest = request.Columns.Safe().Any() ? request.Columns.Select(s => s.ToLower().Trim()) : null;
            switch (request.DisplayBy)
            {
                case DisplayByMapPositionEnum.AxisCarreira:
                    projectPostionsGroupy = projectPostions
                        .Where(a => !string.IsNullOrWhiteSpace(a.AxisCarreira) &&
                              (headersRequest == null || !headersRequest.Contains(a.AxisCarreira.ToLower())))
                        .GroupBy(g => g.AxisCarreira);
                    break;
                case DisplayByMapPositionEnum.Area:
                    projectPostionsGroupy = projectPostions
                        .Where(cp => !string.IsNullOrWhiteSpace(cp.Area) &&
                        (!areasExp.Safe().Any()) &&// || areasExp.Contains(cp.Area)) &&
                         (headersRequest == null || !headersRequest.Contains(cp.Area.ToLower())))
                        .GroupBy(g => g.Area);
                    break;
                case DisplayByMapPositionEnum.Parameter03:
                    projectPostionsGroupy = projectPostions
                        .Where(d => !string.IsNullOrWhiteSpace(d.Parameter1) &&
                              (headersRequest == null || !headersRequest.Contains(d.Parameter1.ToLower())))
                        .GroupBy(g => g.Parameter1);
                    break;
                case DisplayByMapPositionEnum.Parameter02:
                    projectPostionsGroupy = projectPostions
                        .Where(d => !string.IsNullOrWhiteSpace(d.Parameter3) &&
                              (headersRequest == null || !headersRequest.Contains(d.Parameter3.ToLower())))
                        .GroupBy(g => g.Parameter3);
                    break;
                case DisplayByMapPositionEnum.Parameter01:
                    projectPostionsGroupy = projectPostions
                        .Where(s => !string.IsNullOrWhiteSpace(s.Parameter2) &&
                              (headersRequest == null || !headersRequest.Contains(s.Parameter2.ToLower())))
                        .GroupBy(g => g.Parameter2);
                    break;
                default:
                    break;
            }

            var results = projectPostionsGroupy?.Select((se, index) => new
            {
                Header = se.Key,
                Data = se.Where(ppg => string.IsNullOrWhiteSpace(request.Term) ||
                              (!request.Columns.Contains(se.Key) && ppg.SMPosition.ToLower().Trim().Contains(request.Term.ToLower().Trim())))
                  .Select(s => new ResultDTO
                  {
                      GSM = s.GSM,
                      Id = s.Id,
                      SMPosition = s.SMPosition,
                      SMPositionId = s.SMPositionId,
                      CompaniesId = salaryBaseData?
                      .FirstOrDefault(f => f.PositionSMId == s.SMPositionId)?
                      .SubItems?.Select(s => s.CompanyId)?
                      .ToList(),
                      LevelId = s.LevelId
                  })
            }).OrderBy(x => x.Header).ToList();

            if (results == null)
                throw new Exception("Não foi possivel determinar os dados da header");

            var columnsData = await QueryColumnsCustom(request);

            if (columnsData.Any(a => a.IsChecked.HasValue && !a.IsChecked.Value))
            {

                var expColumns = request.Columns.ToList();

                expColumns.AddRange(
                    columnsData
                    .Where(c => c.IsChecked.HasValue && !c.IsChecked.Value).Select(s => s.Name));

                request.Columns = expColumns.Distinct();
            }


            //check if show gsm 
            var gsmGlobalLabelExp =
                configGlobalLabels.FirstOrDefault(f => f.Id == (long)MapPositionColumnEnum.GSM);

            var headerExcel = new List<ExcelHeader>();
            if (gsmGlobalLabelExp.IsChecked)
            {
                headerExcel.Add(
                    new ExcelHeader
                    {
                        Value = gsmGlobalLabelExp.Alias,
                        Type = ExcelFieldType.NumberSimples,
                        IsBold = true
                    }
                );
            }

            foreach (var header in results.Select((value, index) => new { index, value }))
            {
                if (!request.Columns.Any(a => a == header.value.Header))
                {
                    headerExcel.Add(new ExcelHeader
                    {
                        Value = header.value.Header
                    });
                }
            }

            //build body
            var bodyExcel = new List<List<BodyMap>>();

            if (!string.IsNullOrEmpty(request.Term))
            {
                var lstGsmFiltered = results.SelectMany(r => r.Data.Select(d => d.GSM));
                gsms = gsms.Where(gsm => lstGsmFiltered.Any(g => g == gsm)).ToList();
            }

            foreach (var gsm in gsms)
            {
                var dataBodyMap = new List<BodyMap>();
                if (gsmGlobalLabelExp.IsChecked)
                {
                    dataBodyMap.Add(
                        new BodyMap
                        {
                            Value = gsm.ToString(CultureInfo.InvariantCulture)
                        });
                }

                foreach (var result in
                    results.Select((value, index) => new { index, value }))
                {

                    if (!request.Columns.Any(a => a == result.value.Header))
                    {
                        var position = result
                        .value
                        .Data?
                        .Where(w => w.GSM == gsm && (!levelsExp.Safe().Any() || !levelsExp.Contains(w.LevelId.GetValueOrDefault(0))));

                        //filter with occupants

                        if (position.Safe().Any())
                        {
                            var isOccupants = salaryBaseData
                                .FirstOrDefault(f => position.Any(a => a.SMPositionId == f.PositionSMId))?.SubItems?.Any(a => a.OccupantTypeId.HasValue);

                            //filter
                            if (request.ShowJustWithOccupants)
                            {
                                if (isOccupants.HasValue && isOccupants.Value)
                                {
                                    dataBodyMap.Add(new BodyMap
                                    {
                                        Value = string.Join(Environment.NewLine, position.Select(s => s.SMPosition)),
                                        IsOccupants = isOccupants.Value
                                    });
                                }
                                else
                                {
                                    dataBodyMap.Add(new BodyMap
                                    {
                                        Value = string.Empty
                                    });
                                }
                                continue;
                            }

                            dataBodyMap.Add(new BodyMap
                            {
                                Value = string.Join(Environment.NewLine, position.Select(s => s.SMPosition))
                            });

                        }
                        else
                        {
                            dataBodyMap.Add(new BodyMap
                            {
                                Value = string.Empty
                            });
                        }
                    }
                }

                bodyExcel.Add(dataBodyMap);
            }

            var headersCount = headerExcel.Count();

            if (request.ShowJustWithOccupants)
            {
                var lstWithOccupants = bodyExcel.Where(x => x.Any(bo => bo.IsOccupants)).ToList();
                bodyExcel = lstWithOccupants;
            }

            if (request.RemoveRowsEmpty)
            {
                var lstWithValues = bodyExcel
                    .Where(x => x.Count(bo => string.IsNullOrWhiteSpace(bo.Value)) < headersCount).ToList();

                bodyExcel = lstWithValues;
            }

            var fileExcel = await _generateExcelFileInteractor
                .Handler(new GenerateExcelFileRequest
                {
                    FileName = fileName,
                    Body = bodyExcel.Map().ToANew<List<List<ExcelBody>>>(),
                    Header = headerExcel,
                    SheetName = sheetName,
                    TitleSheet = titleName
                });

            return new GetMapPositionExcelResponse
            {
                FileName = fileName,
                File = fileExcel
            };

        }

        private async Task<IEnumerable<ColumnDataPositionExcelDTO>> QueryColumnsCustom(GetMapPositionExcelRequest request)
        {
            return await _unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
                .GetListAsync(x => x.Where(ues => ues.UsuarioId == request.UserId &&
                 ues.ModuloSmid == (long)ModulesEnum.Position &&
                 ues.ModuloSmsubItemId.HasValue && ues.ModuloSmsubItemId.Value == (long)ModulesSuItemsEnum.Map)
                ?.Select(s => new ColumnDataPositionExcelDTO
                {
                    ColumnId = s.ColunaId,
                    IsChecked = s.Checado,
                    Name = s.Nome
                }));
        }

        private async Task<List<int>> GetGsms(GetMapPositionExcelRequest request, IEnumerable<long> tablesExp)
        {
            //var isDesc = request.IsAsc.HasValue ? !request.IsAsc.Value : false;
            //var positions = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
            //    .GetListAsync(x => x
            //    .Where(cp => cp.ProjetoId == request.ProjectId &&
            //                 cp.Ativo.HasValue && cp.Ativo.Value &&
            //                cp.GruposProjetosSalaryMark
            //                 .GruposProjetosSalaryMarkMapeamento
            //                    .Any(a => a.GrupoProjetoSmidLocal == cp.GrupoSmidLocal &&
            //                        (request.UnitId.HasValue || request.CompaniesId.Contains(a.EmpresaId)) &&
            //                        (!request.UnitId.HasValue || request.UnitId == a.EmpresaId)) &&
            //                 (!request.TableId.HasValue || cp.GruposProjetosSalaryMark.GruposProjetosSalaryMarkMapeamento.Any(s => s.TabelaSalarialIdLocal == request.TableId.Value)) &&
            //                 (!request.GroupId.HasValue || cp.GrupoSmidLocal == request.GroupId.Value) &&
            //                 (string.IsNullOrWhiteSpace(request.Term) || cp.CargoSm.ToLower().Trim().Contains(request.Term.ToLower().Trim())))
            //    .OrderBy("Gsm", isDesc));
            //return positions.Select(s => s.Gsm)
            //    .Distinct().ToList();
            var isDesc = request.IsAsc.HasValue ? !request.IsAsc.Value : false;

            var grades = await _unitOfWork.GetRepository<TabelasSalariaisValores, long>()
                .GetListAsync(x => x.
                Where(tv => tv.ProjetoId == request.ProjectId &&
                (!tablesExp.Safe().Any() || !tablesExp.Contains(tv.TabelaSalarialIdLocal)) &&
                (!request.TableId.HasValue || tv.TabelaSalarialIdLocal == request.TableId.Value))
                .OrderBy("Grade", isDesc)
                .Select(s => s.Grade));
            grades = grades.Distinct().ToList();
            return grades;
        }
    }
}
