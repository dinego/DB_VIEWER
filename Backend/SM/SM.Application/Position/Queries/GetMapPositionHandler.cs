using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Helpers;
using SM.Application.Interactors.Interfaces;
using SM.Application.Share.Queries;
using SM.Domain.Enum;
using CMC.Common.Repositories.Extensions;
namespace SM.Application.Position.Queries
{
    public class GetMapPositionRequest : IRequest<GetMapPositionResponse>
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
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public IEnumerable<string> Columns { get; set; } = new List<string>(); //using to share (EXP)
        public bool? IsAsc { get; set; } = null;
        public bool Share { get; set; }
    }

    public class GetMapPositionResponse
    {
        public TableInfoMap Table { get; set; }
        public int NextPage { get; set; }
        public ShareMapPositionResponse Share { get; set; }
    }

    public class ShareMapPositionResponse
    {
        public string User { get; set; } = null;
        public DateTime? Date { get; set; } = null;
        public string Unit { get; set; }
        public long? UnitId { get; set; }
        public string WithOccupants { get; set; }
        public bool IsWithOccupants { get; set; }
        public string Group { get; set; }
        public long? GroupId { get; set; }
        public string Table { get; set; }
        public long? TableId { get; set; }
        public string DisplayBy { get; set; }
        public int DisplayById { get; set; }
        public PermissionShared Permissions { get; set; }
    }

    public class TableInfoMap
    {
        public IEnumerable<HeaderInfoMap> Header { get; set; }
        public List<List<DataBodyMap>> Body { get; set; }
    }

    public class HeaderInfoMap
    {
        public int ColPos { get; set; }
        public string ColName { get; set; }
        public string NickName { get; set; }
        public bool Disabled { get; set; } = false;
        public bool Editable { get; set; } = false;
        public bool IsChecked { get; set; } = true;
        public bool Visible { get; set; } = true;
        public string ColumnId { get; set; } = null;
        public bool Sortable { get; set; } = false;

    }

    public class DataBodyMap
    {
        public int ColPos { get; set; }
        public string @Type { get; set; }
        public IEnumerable<DataBodyPositionMap> Positions { get; set; }
    }

    public class DataBodyPositionMap
    {
        public string Value { get; set; }
        public long? PositionSMId { get; set; } = null;
        public List<SubItemsMap> Tooltips { get; set; } = new List<SubItemsMap>();
    }

    public class SubItemsMap
    {
        public string Position { get; set; }
        public string OccupantType { get; set; }
        public int? OccupantTypeId { get; set; }
        public bool? OccupantPJ { get; set; } = null;
        public bool? OccupantCLT { get; set; } = null;
        public int Amount { get; set; }
        public long CompanyId { get; set; }
    }

    public class SubItemsDTO
    {
        public long PositionSMId { get; set; }
        public IEnumerable<SubItemsMap> SubItems { get; set; }

    }

    public class GetMapPositionDTO
    {
        public int GSM { get; set; }
        public string SMPosition { get; set; }
        public long SMPositionId { get; set; }
        public List<ParametersDTO> Parameters { get; set; }
        public long CmCode { get; set; }
        public long Id { get; set; }
        public long? LevelId { get; set; }
    }

    public class ParametersDTO
    {
        public long ParameterId { get; set; }
        public string Parameter { get; set; }
    }

    public class ResultDTO
    {
        public int GSM { get; set; }
        public string SMPosition { get; set; }
        public long SMPositionId { get; set; }
        public long Id { get; set; }
        public long? LevelId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }

    }
    public class GetMapPositionHandler : IRequestHandler<GetMapPositionRequest, GetMapPositionResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;

        public GetMapPositionHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;
        }
        public async Task<GetMapPositionResponse> Handle(GetMapPositionRequest request, CancellationToken cancellationToken)
        {

            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            var tablesExp =
                        permissionUser.Contents?
                        .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable)?
                        .SubItems;

            List<int> gsms = await GetGsms(request, tablesExp);


            var areasExp = permissionUser.Areas;
            var levelsExp = permissionUser.Levels;
            var displayAllOccupants = permissionUser.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.Positions);

            //configuration global labels 
            var configGlobalLabels = await _getGlobalLabelsInteractor.Handler(request.ProjectId);

            var projectPostions = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                            .Include("CmcodeNavigation")
                            .Include("GruposProjetosSalaryMark.GruposProjetosSalaryMarkMapeamento")
                            .Include("CargosProjetoSMParametrosMapeamento.ParametrosProjetosSMLista.ParametrosProjetosSMTipos")
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
                            .Select(s => new GetMapPositionDTO
                            {
                                Id = s.Id,
                                GSM = s.CargosProjetosSmmapeamento.Any(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal) ? (int)s.CargosProjetosSmmapeamento.FirstOrDefault(f => f.CargoProjetoSmidLocal == s.CargoProjetoSmidLocal).Gsm : 0,
                                Parameters = s.CargosProjetoSMParametrosMapeamento.Where(w => w.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal).Select(par => new ParametersDTO
                                {
                                    Parameter = par.ParametrosProjetosSMLista.ParametrosProjetosSMTipos.Parametro,
                                    ParameterId = par.ParametrosProjetosSMLista.ParametroSMTipoId
                                }).ToList(),
                                SMPosition = s.CargoSm,
                                SMPositionId = s.CargoProjetoSmidLocal,
                                CmCode = s.Cmcode,

                                //.Any(f => f.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal) ? s.CargosProjetoSMParametrosMapeamento.FirstOrDefault(f => f.CargoProjetoSMIdLocal == s.CargoProjetoSmidLocal).ParametrosProjetosSMLista.ParametrosProjetosSMTipos.ParametroSMTipo : string.Empty,
                                //AxisCarreira = s.EixoCarreira,
                                //Parameter3 = s.Parametro3,
                                //Parameter2 = s.Parametro2,
                                //Parameter1 = s.Parametro1,
                                LevelId = s.CmcodeNavigation != null ? s.CmcodeNavigation.NivelId : (long?)null
                            }));

            if (!projectPostions.Any())
                return new GetMapPositionResponse();

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
            var salaryBaseData = new List<SubItemsDTO>();
            if (salaryBase.Any())
            {
                salaryBaseData = salaryBase
                .GroupBy(g => g.PositionSMId, (key, value) => new SubItemsDTO
                {
                    PositionSMId = key,
                    SubItems = value.Select(val => new SubItemsMap
                    {
                        Position = val.PositionCompany,
                        CompanyId = val.CompanyId,
                        OccupantType = val.OccupantType,
                        OccupantTypeId = val.OccupantTypeId
                    })
                }).ToList();
            }

            IEnumerable<IGrouping<string, GetMapPositionDTO>> projectPostionsGroupy =
                new List<IGrouping<string, GetMapPositionDTO>>();

            //switch (request.DisplayBy)
            //{
            //    case DisplayByMapPositionEnum.AxisCarreira:
            //        projectPostionsGroupy = projectPostions
            //            .Where(a => !string.IsNullOrWhiteSpace(a.Parameter))
            //            .GroupBy(g => g.Parameter);
            //        break;
            //    case DisplayByMapPositionEnum.Area:
            //        projectPostionsGroupy = projectPostions
            //            .Where(cp => !string.IsNullOrWhiteSpace(cp.Parameter) &&
            //            (!areasExp.Safe().Any() || areasExp.Contains(cp.Parameter)))
            //            .GroupBy(g => g.Parameter);
            //        break;
            //    case DisplayByMapPositionEnum.Parameter01:
            //        projectPostionsGroupy = projectPostions
            //            .Where(d => !string.IsNullOrWhiteSpace(d.Parameter))
            //            .GroupBy(g => g.Parameter);
            //        break;
            //    case DisplayByMapPositionEnum.Parameter03:
            //        projectPostionsGroupy = projectPostions
            //            .Where(d => !string.IsNullOrWhiteSpace(d.Parameter))
            //            .GroupBy(g => g.Parameter);
            //        break;
            //    case DisplayByMapPositionEnum.Parameter02:
            //        projectPostionsGroupy = projectPostions
            //            .Where(s => !string.IsNullOrWhiteSpace(s.Parameter))
            //            .GroupBy(g => g.Parameter);
            //        break;
            //    default:
            //        break;
            //}

            if (!projectPostionsGroupy.Safe().Any())
                return new GetMapPositionResponse();

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
                    .Where(f => f.PositionSMId == s.SMPositionId)?
                    .SelectMany(s => s.SubItems)
                    .Select(se => se.CompanyId)
                    .Distinct()
                    .ToList(),
                    LevelId = s.LevelId
                })
            }).OrderBy(x => x.Header).ToList();


            if (results == null)
                return new GetMapPositionResponse();

            //filter columns (share)
            if (request.Share && request.Columns.Any())
            {
                var headersRequest = request.Columns.Select(s => s.ToLower().Trim());

                results = results
                    .Where(r => !headersRequest.Contains(r.Header.Trim().ToLower()))
                    .ToList();
            }

            //build header
            var displayColumns = await _unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
                .GetListAsync(x => x.Where(g => g.UsuarioId == request.UserId &&
                g.ModuloSmid == (long)ModulesEnum.Position &&
                g.ModuloSmsubItemId.HasValue &&
                g.ModuloSmsubItemId == (long)ModulesSuItemsEnum.Map)
                ?.Select(s => new
                {
                    ColumnId = s.ColunaId,
                    Name = s.Nome,
                    IsChecked = s.Checado
                }));

            var headers = new List<HeaderInfoMap>();
            //check if show gsm 
            var gsmGlobalLabelExp =
                configGlobalLabels.FirstOrDefault(f => f.Id == (long)MapPositionColumnEnum.GSM);

            var headerCountGsm = 0;
            if (gsmGlobalLabelExp.IsChecked)
            {
                headers.Add(new HeaderInfoMap
                {
                    ColName = MapPositionColumnEnum.GSM.GetDescription(),
                    NickName = gsmGlobalLabelExp.Alias,
                    ColPos = headerCountGsm,
                    Editable = false,
                    Disabled = true,
                    Sortable = true,
                    IsChecked = true
                });
                headerCountGsm++;
            }

            foreach (var header in results.Select((value, index) => new { index, value }))
            {
                int colId = header.index + headerCountGsm;
                if (!request.Columns.Any(a => a == header.value.Header))
                {
                    var displayItem = displayColumns?.FirstOrDefault(f => f.Name.ToLower().Trim()
                    .Equals(header.value.Header.ToLower().Trim()))?.IsChecked;

                    headers.Add(new HeaderInfoMap
                    {
                        ColName = header.value.Header,
                        NickName = header.value.Header,
                        ColPos = colId,
                        Sortable = false,
                        Editable = false,
                        IsChecked = displayItem ?? true,
                        ColumnId = header.value.Header
                    });
                }
            }

            //build body
            var body = new List<List<DataBodyMap>>();

            if (!string.IsNullOrEmpty(request.Term))
            {
                var lstGsmFiltered = results.SelectMany(r => r.Data.Select(d => d.GSM));
                gsms = gsms.Where(gsm => lstGsmFiltered.Any(g => g == gsm)).ToList();
            }

            foreach (var gsm in gsms)
            {
                var dataBodyMap = new List<DataBodyMap>();
                var gsmCountInBody = 0;

                if (gsmGlobalLabelExp.IsChecked)
                {
                    dataBodyMap.Add(new DataBodyMap
                    {
                        ColPos = gsmCountInBody,
                        Type = gsm.GetType().Name,
                        Positions = new List<DataBodyPositionMap>
                            {
                                new DataBodyPositionMap
                                {
                                 Value = gsm.ToString(),
                                 Tooltips = new List<SubItemsMap>()
                                }
                            }

                    });
                    gsmCountInBody++;
                }

                foreach (var result in
                    results.Select((value, index) => new { index, value }))
                {

                    var positions = result.value.Data?
                            .Where(w => w.GSM == gsm &&
                            (!levelsExp.Safe().Any() || !levelsExp.Contains(w.LevelId.GetValueOrDefault(0))));

                    //filter with occupants

                    var positionResult = new DataBodyMap
                    {
                        ColPos = result.index + gsmCountInBody,
                        Type = typeof(string).Name
                    };

                    if (positions.Safe().Any())
                    {

                        var dataBodyPositionMap = new List<DataBodyPositionMap>();

                        foreach (var position in positions)
                        {

                            var tooltip = salaryBaseData
                                             .Where(f => position.SMPositionId == f.PositionSMId)?
                                             .SelectMany(s => s.SubItems)
                                             .GroupBy(g => g.Position,
                                             (key, value) => new SubItemsMap
                                             {
                                                 Amount = value.Count(),
                                                 Position = key,
                                                 CompanyId = value.FirstOrDefault().CompanyId,
                                                 OccupantCLT = value.Any(v => PJOrCLT.IsCLT(v.OccupantType).GetValueOrDefault(false)),
                                                 OccupantPJ = value.Any(v => PJOrCLT.IsPJ(v.OccupantType).GetValueOrDefault(false)),
                                                 OccupantTypeId = value.FirstOrDefault()?.OccupantTypeId
                                             }).ToList();

                            //filter
                            if (request.ShowJustWithOccupants)
                            {
                                if (tooltip.Safe().Any(a => a.OccupantTypeId.HasValue))
                                {
                                    dataBodyPositionMap.Add(new DataBodyPositionMap
                                    {
                                        PositionSMId = position.SMPositionId,
                                        Value = position.SMPosition,
                                        Tooltips = tooltip
                                    });
                                }
                                else
                                {
                                    dataBodyPositionMap.Add(new DataBodyPositionMap
                                    {
                                        Value = string.Empty,
                                        Tooltips = new List<SubItemsMap>()
                                    });

                                }

                                continue;
                            }

                            if (displayAllOccupants || tooltip.Safe().Any(a => a.OccupantTypeId.HasValue))
                                dataBodyPositionMap.Add(new DataBodyPositionMap
                                {
                                    PositionSMId = position.SMPositionId,
                                    Tooltips = tooltip,
                                    Value = position.SMPosition
                                });
                        }

                        positionResult.Positions = dataBodyPositionMap;

                    }
                    else
                    {
                        positionResult.Positions = new List<DataBodyPositionMap>();
                    }

                    dataBodyMap.Add(positionResult);

                }

                body.Add(dataBodyMap);
            }

            if (request.ShowJustWithOccupants || request.RemoveRowsEmpty)
            {
                var lstWithValues = body.Where(x => x.Any(bo => bo.ColPos != 0 && bo.Positions.Any(p => !string.IsNullOrEmpty(p.Value)))).ToList();
                body = lstWithValues;
            }
            return new GetMapPositionResponse
            {
                //NextPage = request.Page + 1,
                NextPage = 0,
                Table = new TableInfoMap
                {
                    Body = body,
                    Header = headers
                }
            };
        }

        private async Task<List<int>> GetGsms(GetMapPositionRequest request,
            IEnumerable<long> tablesExp)
        {
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
