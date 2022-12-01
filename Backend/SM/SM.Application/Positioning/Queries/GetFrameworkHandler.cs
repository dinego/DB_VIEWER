using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using CMC.Common.Repositories.Extensions;
using SM.Application.Interactors;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using SM.Application.Share.Queries;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using SM.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Positioning.Queries
{
    public class GetFrameworkRequest : IRequest<GetFrameworkResponse>
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
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsAsc { get; set; } = null;
        public int? SortColumnId { get; set; } = null;
        public bool Share { get; set; }
    }



    public class GetFrameworkResponse
    {
        public InfoFramework Framework { get; set; }
        public int NextPage { get; set; }
        public ShareFrameworkResponse Share { get; set; }
    }

    public class ShareFrameworkResponse
    {
        public string User { get; set; } = null;
        public DateTime? Date { get; set; } = null;
        public string ContractType { get; set; }
        public int ContractTypeId { get; set; }
        public string HoursType { get; set; }
        public int HoursTypeId { get; set; }
        public string Unit { get; set; }
        public long? UnitId { get; set; }
        public string WithOccupants { get; set; }
        public bool IsWithOccupants { get; set; }
        public string ScenarioLabel { get; set; }
        public PermissionShared Permissions { get; set; }
    }
    public class InfoFramework
    {
        public IEnumerable<HeadeFullInfoFramework> HeaderToShow { get; set; }

        public IEnumerable<TableInfoFramework> Tables { get; set; }
    }

    public class TableInfoFramework
    {
        public DisplayMMMIEnum DisplayType { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<HeadeFullInfoFramework> Header { get; set; }
        public IEnumerable<IEnumerable<DataBodyFramework>> Body { get; set; }
    }

    public class HeadeFullInfoFramework
    {
        public int ColPos { get; set; }
        public string ColName { get; set; }
        public string NickName { get; set; }
        public bool Disabled { get; set; } = false;
        public bool Editable { get; set; } = true;
        public bool IsChecked { get; set; } = true;
        public bool Visible { get; set; } = true;
        public int? ColumnId { get; set; } = null;
        public bool Sortable { get; set; } = false;
    }

    public class HeadeSimpleInfoFramework
    {
        public int ColPos { get; set; }
        public string ColName { get; set; }
        public string NickName { get; set; }
    }

    public class DataBodyFramework
    {
        public int ColPos { get; set; }
        public string Value { get; set; }
        public long? SalaryBaseId { get; set; } = null;
        public string @Type { get; set; }
    }

    public class SalaryBaseFrameworkDTO
    {
        public long SalaryBaseId { get; set; }
        public string CurrentPosition { get; set; }
        public string Company { get; set; }
        public long CompanyId { get; set; }
        public string UnitPlace { get; set; }
        public string Business { get; set; }
        public string Setor { get; set; }
        public string ImmediateSupervisor { get; set; }
        public double? HourlyBasis { get; set; }
        public double? Salary { get; set; }
        public long? SMCargoIdMM { get; set; }
        public long? SMCargoIdMI { get; set; }
        public string Employee { get; set; }
        public string TypeOfContract { get; set; }
        public string FuncionarioID { get; set; }
        public string PositionSm { get; set; }
        public string Profile { get; set; }
        public long ProfileId { get; set; }
        public long GSM { get; set; }
        public IEnumerable<DataSalaryTable> SalaryTable { get; set; }
        public double CompareMidPoint { get; set; }
    }

    public class ColumnDataDTO
    {
        public long ColumnId { get; set; }
        public bool? IsChecked { get; set; }
        public string Name { get; set; }

    }

    public class GetFrameworkHandler : IRequestHandler<GetFrameworkRequest, GetFrameworkResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IGetPositionProjectSMAndSalaryTableInteractor _getPositionProjectSMAndSalaryTableInteractor;
        private IEnumerable<FrameworkColumnsMainEnum> _listEnum;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;

        public GetFrameworkHandler(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor,
            IGetPositionProjectSMAndSalaryTableInteractor getPositionProjectSMAndSalaryTableInteractor,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _permissionUserInteractor = permissionUserInteractor;
            _getPositionProjectSMAndSalaryTableInteractor = getPositionProjectSMAndSalaryTableInteractor;
            _listEnum = Enum.GetValues(typeof(FrameworkColumnsMainEnum)) as
                        IEnumerable<FrameworkColumnsMainEnum>;

            _listEnum = _listEnum.First().GetWithOrder().Select(s =>
            (FrameworkColumnsMainEnum)Enum.Parse(typeof(FrameworkColumnsMainEnum), s));

            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;

        }
        public async Task<GetFrameworkResponse> Handle(GetFrameworkRequest request, CancellationToken cancellationToken)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            //configuration global labels 
            var configGlobalLabels =
                await _getGlobalLabelsInteractor.Handler(request.ProjectId);

            //BaseSalarial table
            List<SalaryBaseFrameworkDTO> salaryBaseList = await QuerySalaryBase(request, permissionUser);

            if (!salaryBaseList.Any())
                return new GetFrameworkResponse
                {
                    Framework = new InfoFramework()
                };

            request.Columns = request.Share ? request.Columns : new List<int>();
            _listEnum = _listEnum.Where(l => !request.Columns.Contains((int)l) &&
                                (!configGlobalLabels.Any(aa => aa.Id == (int)l && !aa.IsChecked)));

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

            //permissions user
            var canEditColumns = CanEditColumns(permissionUser);

            //header
            var headerMain = await GetHeader(request, columnsData, canEditColumns, salaryBaseList);

            headerMain.ForEach(f =>
            {
                var globalLabel =
                configGlobalLabels.FirstOrDefault(fe => fe.Id == f.ColumnId);

                if (globalLabel != null)
                {
                    f.ColName = globalLabel.Alias;
                    f.NickName = globalLabel.Alias;
                }
            });

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

            //body
            var listTable = new List<TableInfoFramework>();
            if (request.IsMI)
            {
                listTable.Add(new TableInfoFramework
                {
                    DisplayType = DisplayMMMIEnum.MI,
                    DisplayName = !dictonaryMMMI.Any() ? DisplayMMMIEnum.MI.GetDescription() : dictonaryMMMI[DisplayMMMIEnum.MI],
                    Body = GetBody(salaryBaseList, DisplayMMMIEnum.MI, request),
                    Header = headerMain
                });
            }

            if (request.IsMM)
            {
                listTable.Add(new TableInfoFramework
                {
                    DisplayType = DisplayMMMIEnum.MM,
                    DisplayName = !dictonaryMMMI.Any() ? DisplayMMMIEnum.MM.GetDescription() : dictonaryMMMI[DisplayMMMIEnum.MM],
                    Body = GetBody(salaryBaseList, DisplayMMMIEnum.MM, request),
                    Header = headerMain
                });
            }

            return new GetFrameworkResponse
            {
                NextPage = request.Page + 1,
                Framework = new InfoFramework
                {
                    HeaderToShow = headerMain,
                    Tables = listTable
                }
            };
        }

        private async Task<List<HeadeFullInfoFramework>> GetHeader(GetFrameworkRequest request, IEnumerable<ColumnDataDTO> columnsData, bool canEditColumns, List<SalaryBaseFrameworkDTO> salaryBaseList)
        {
            var companyDefaultHeader = await _unitOfWork.GetRepository<EmpresaPermissaoSm, long>().GetAsync(x => x.Where(ep => request.CompaniesId.Contains(ep.EmpresaId)));
            var headerMain = new List<HeadeFullInfoFramework>();

            var countHeader = 0;
            foreach (FrameworkColumnsMainEnum item in _listEnum)
            {
                var columnData =
                            columnsData.FirstOrDefault(f => f.ColumnId == (long)item);

                switch (item)
                {
                    case FrameworkColumnsMainEnum.CurrentPosition:
                        headerMain.Add(new HeadeFullInfoFramework
                        {
                            ColPos = countHeader,
                            ColName = item.GetDescription(),
                            NickName = columnData == null ? item.GetDescription() :
                            columnData.Name,
                            Disabled = true,
                            ColumnId = (int)item,
                            Editable = false,
                            IsChecked = true,
                            Sortable = true
                        });

                        break;
                    case FrameworkColumnsMainEnum.Company:
                        headerMain.Add(new HeadeFullInfoFramework
                        {
                            ColPos = countHeader,
                            ColName = item.GetDescription(),
                            NickName = columnData == null ? item.GetDescription() :
                                        columnData.Name,
                            ColumnId = (int)item,
                            Editable = false,
                            IsChecked = columnData == null ? true : columnData.IsChecked.Value,
                            Sortable = true
                        });
                        break;
                    case FrameworkColumnsMainEnum.UnitPlace:
                        headerMain.Add(new HeadeFullInfoFramework
                        {
                            ColPos = countHeader,
                            ColName = item.GetDescription(),
                            NickName = columnData == null ? item.GetDescription() :
                                       columnData.Name,
                            ColumnId = (int)item,
                            Editable = canEditColumns,
                            IsChecked = columnData == null ? true : columnData.IsChecked.Value,
                            Sortable = true
                        });
                        break;
                    case FrameworkColumnsMainEnum.Business:
                        headerMain.Add(new HeadeFullInfoFramework
                        {
                            ColPos = countHeader,
                            ColName = item.GetDescription(),
                            NickName = columnData == null ? item.GetDescription() :
                                        columnData.Name,
                            ColumnId = (int)item,
                            Editable = canEditColumns,
                            IsChecked = columnData == null ? true : columnData.IsChecked.Value,
                            Sortable = true
                        });
                        break;
                    case FrameworkColumnsMainEnum.Setor:
                        headerMain.Add(new HeadeFullInfoFramework
                        {
                            ColPos = countHeader,
                            ColName = item.GetDescription(),
                            NickName = columnData == null ? item.GetDescription() :
                                        columnData.Name,
                            ColumnId = (int)item,
                            Editable = canEditColumns,
                            IsChecked = columnData == null ? true : columnData.IsChecked.Value,
                            Sortable = true
                        });
                        break;
                    case FrameworkColumnsMainEnum.Employee:
                        headerMain.Add(new HeadeFullInfoFramework
                        {
                            ColPos = countHeader,
                            ColName = item.GetDescription(),
                            NickName = columnData == null ? item.GetDescription() :
                                        columnData.Name,
                            ColumnId = (int)item,
                            Editable = false,
                            IsChecked = columnData == null ? true : columnData.IsChecked.Value,
                            Sortable = true
                        });
                        break;
                    case FrameworkColumnsMainEnum.ImmediateSupervisor:
                        headerMain.Add(new HeadeFullInfoFramework
                        {
                            ColPos = countHeader,
                            ColName = item.GetDescription(),
                            NickName = columnData == null ? item.GetDescription() :
                                        columnData.Name,
                            ColumnId = (int)item,
                            Editable = false,
                            IsChecked = columnData == null ? true : columnData.IsChecked.Value,
                            Sortable = true
                        });
                        break;
                    case FrameworkColumnsMainEnum.HourlyBasis:
                        headerMain.Add(new HeadeFullInfoFramework
                        {
                            ColPos = countHeader,
                            ColName = item.GetDescription(),
                            NickName = columnData == null ? item.GetDescription() :
                                        columnData.Name,
                            ColumnId = (int)item,
                            Editable = false,
                            IsChecked = columnData == null ? true : columnData.IsChecked.Value,
                            Sortable = true
                        });
                        break;
                    case FrameworkColumnsMainEnum.Salary:
                        headerMain.Add(new HeadeFullInfoFramework
                        {
                            ColPos = countHeader,
                            ColName = item.GetDescription(),
                            NickName = columnData == null ? item.GetDescription() :
                            columnData.Name,
                            Disabled = true,
                            ColumnId = (int)item,
                            Editable = false,
                            IsChecked = true,
                            Sortable = true
                        });
                        break;
                    case FrameworkColumnsMainEnum.PositionSm:
                        string name = columnData == null && companyDefaultHeader != null ?
                               companyDefaultHeader.CargoSalaryMark : columnData == null ? item.GetDescription() :
                               columnData.Name;
                        headerMain.Add(new HeadeFullInfoFramework
                        {
                            ColPos = countHeader,
                            ColName = name,
                            NickName = name,
                            Disabled = true,
                            ColumnId = (int)item,
                            Editable = false,
                            IsChecked = true,
                            Sortable = true
                        });
                        break;
                    case FrameworkColumnsMainEnum.Profile:
                        headerMain.Add(new HeadeFullInfoFramework
                        {
                            ColPos = countHeader,
                            ColName = item.GetDescription(),
                            NickName = columnData == null ? item.GetDescription() :
                                        columnData.Name,
                            ColumnId = (int)item,
                            Editable = false,
                            IsChecked = columnData == null ? true : columnData.IsChecked.Value,
                            Sortable = true
                        });
                        break;
                    case FrameworkColumnsMainEnum.GSM:
                        headerMain.Add(new HeadeFullInfoFramework
                        {
                            ColPos = countHeader,
                            ColName = item.GetDescription(),
                            NickName = columnData == null ? item.GetDescription() :
                                       columnData.Name,
                            ColumnId = (int)item,
                            Disabled = true,
                            Editable = false,
                            IsChecked = columnData == null ? true : columnData.IsChecked.Value,
                            Sortable = true
                        });
                        break;
                    case FrameworkColumnsMainEnum.PercentagemArea:
                        var dataPositionSM = salaryBaseList.FirstOrDefault();
                        if (dataPositionSM == null)
                            break;

                        var tracksPercentage = dataPositionSM.SalaryTable;

                        foreach (var trackPercentage in tracksPercentage)
                        {

                            headerMain.Add(new HeadeFullInfoFramework
                            {
                                ColPos = countHeader,
                                ColName = $"{Math.Round(trackPercentage.Multiplicator * 100, 0).ToString(CultureInfo.InvariantCulture)}%",
                                NickName = $"{Math.Round(trackPercentage.Multiplicator * 100, 0).ToString(CultureInfo.InvariantCulture)}%",
                                Editable = false,
                                Visible = false,
                                IsChecked = true
                            });
                            countHeader++;
                        }

                        continue;
                    case FrameworkColumnsMainEnum.CompareMidPoint:
                        headerMain.Add(new HeadeFullInfoFramework
                        {
                            ColPos = countHeader,
                            ColName = item.GetDescription(),
                            NickName = columnData == null ? item.GetDescription() :
                                        columnData.Name,
                            ColumnId = (int)item,
                            Editable = false,
                            IsChecked = columnData == null ? true : columnData.IsChecked.Value,
                            Sortable = true
                        });
                        break;
                    case FrameworkColumnsMainEnum.TypeOfContract:
                        headerMain.Add(new HeadeFullInfoFramework
                        {
                            ColPos = countHeader,
                            ColName = item.GetDescription(),
                            NickName = item.GetDescription(),
                            ColumnId = (int)item,
                            Editable = false,
                            IsChecked = columnData == null ? true : columnData.IsChecked.Value,
                            Sortable = true
                        });
                        break;
                }

                countHeader++;
            }

            return headerMain;

        }

        private List<List<DataBodyFramework>> GetBody(List<SalaryBaseFrameworkDTO> salaryBaseList, DisplayMMMIEnum displayMMMIEnum, GetFrameworkRequest request)
        {

            var listCols = new List<List<DataBodyFramework>>();

            foreach (var salaryBaseData in salaryBaseList)
            {
                var cols = new List<DataBodyFramework>();
                int count = 0;
                foreach (FrameworkColumnsMainEnum item in _listEnum)
                {

                    switch (item)
                    {
                        case FrameworkColumnsMainEnum.PercentagemArea:
                            //if (valuePositionSm == null) continue;

                            foreach (var valueSalary in salaryBaseData.SalaryTable)
                            {
                                cols.Add(new DataBodyFramework
                                {
                                    ColPos = count,
                                    SalaryBaseId = salaryBaseData.SalaryBaseId,
                                    @Type = valueSalary.Value == 0 ? typeof(string).Name : valueSalary.Value.GetType().Name,
                                    Value = valueSalary.Value == 0 ? "-" :
                                            (request.HoursType == DataBaseSalaryEnum.HourSalary ? Math.Round(valueSalary.Value, 2).ToString("N2", CultureInfo.InvariantCulture) :
                                            Math.Round(valueSalary.Value, 0).ToString(CultureInfo.InvariantCulture))
                                });
                                count++;
                            }
                            continue;
                        case FrameworkColumnsMainEnum.CompareMidPoint:
                            //if (valuePositionSm == null) continue;

                            var valueMidpoint = Math.Round(salaryBaseData.CompareMidPoint * 100, 0);

                            cols.Add(new DataBodyFramework
                            {
                                ColPos = count,
                                SalaryBaseId = salaryBaseData.SalaryBaseId,
                                @Type = valueMidpoint.GetType().Name,
                                Value = valueMidpoint.ToString()
                            });

                            break;

                        case FrameworkColumnsMainEnum.PositionSm:
                            cols.Add(new DataBodyFramework
                            {
                                ColPos = count,
                                SalaryBaseId = salaryBaseData.SalaryBaseId,
                                @Type = salaryBaseData == null ? typeof(string).Name : salaryBaseData.PositionSm.GetType().Name,
                                Value = salaryBaseData == null ? string.Empty : salaryBaseData.PositionSm.ToString()
                            });

                            break;

                        case FrameworkColumnsMainEnum.GSM:
                            cols.Add(new DataBodyFramework
                            {
                                ColPos = count,
                                SalaryBaseId = salaryBaseData.SalaryBaseId,
                                @Type = salaryBaseData == null ? typeof(string).Name : salaryBaseData.GSM.GetType().Name,
                                Value = salaryBaseData == null ? string.Empty : salaryBaseData.GSM.ToString()
                            });
                            break;
                        case FrameworkColumnsMainEnum.Profile:
                            cols.Add(new DataBodyFramework
                            {
                                ColPos = count,
                                SalaryBaseId = salaryBaseData.SalaryBaseId,
                                @Type = salaryBaseData == null ? typeof(string).Name : salaryBaseData.Profile.GetType().Name,
                                Value = salaryBaseData == null ? string.Empty : salaryBaseData.Profile.ToString()
                            });

                            break;

                        case FrameworkColumnsMainEnum.Salary:
                            cols.Add(new DataBodyFramework
                            {
                                ColPos = count,
                                SalaryBaseId = salaryBaseData.SalaryBaseId,
                                @Type = !salaryBaseData.Salary.HasValue ? typeof(string).Name : salaryBaseData.Salary.Value.GetType().Name,
                                Value = !salaryBaseData.Salary.HasValue ? string.Empty : Math.Truncate(salaryBaseData.Salary.Value).ToString(CultureInfo.InvariantCulture)
                            });

                            break;
                        default:
                            var value = salaryBaseData
                                .GetType()
                                .GetProperty(item.ToString());

                            cols.Add(new DataBodyFramework
                            {
                                ColPos = count,
                                SalaryBaseId = salaryBaseData.SalaryBaseId,
                                @Type = value == null ? typeof(string).Name :
                                (value.GetValue(salaryBaseData) == null ? typeof(string).Name : value.GetValue(salaryBaseData).GetType().Name),
                                Value = value == null ? string.Empty : (value.GetValue(salaryBaseData) == null ? string.Empty : value.GetValue(salaryBaseData).ToString())

                            });
                            break;
                    }
                    count++;
                }

                listCols.Add(cols);
            }

            return listCols;
        }

        private async Task<IEnumerable<DataPositionProjectSMResponse>> GetDataPostionSM(GetFrameworkRequest request,
            List<SalaryBaseFrameworkDTO> salaryBaseList,
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
            if (listDataPositionProjectSMRequestDistinct == null) return null;

            positionSm = await _getPositionProjectSMAndSalaryTableInteractor
                .Handler(listDataPositionProjectSMRequestDistinct,
                request.ProjectId, permissionUser, request.HoursType, request.ContractType);

            return positionSm;
        }

        private async Task<IEnumerable<ColumnDataDTO>> QueryColumnsCustom(GetFrameworkRequest request)
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

        private async Task<List<SalaryBaseFrameworkDTO>> QuerySalaryBase(GetFrameworkRequest request, PermissionJson userJson)
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
                                cps.ProjetoId == request.ProjectId)?
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
            var sortColumnProperty = request.SortColumnId.HasValue ?
                _listEnum.FirstOrDefault(f => (int)f == request.SortColumnId.Value).ToString() :
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
                              bs.Empresa.GruposProjetosSalaryMarkMapeamento.Any(gp => gp.GruposProjetosSalaryMark != null && gp.GruposProjetosSalaryMark.GrupoSm.ToLower().Trim().Contains(term))) ||

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
               ?.Select(s => new SalaryBaseFrameworkDTO
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

            result = result.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
            return result;
        }

        private bool CanEditColumns(PermissionJson userJson)
        {
            var resultPermission = userJson.Permission?.FirstOrDefault(p => p.Id == (long)PermissionItensEnum.RenameColumn);

            return resultPermission == null;
        }

    }
}

