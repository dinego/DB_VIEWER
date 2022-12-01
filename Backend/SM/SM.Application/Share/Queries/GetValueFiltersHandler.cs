using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using SM.Domain.Enum.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Share.Queries
{
    public class GetValueFiltersRequest : IRequest<GetValueFiltersResponse>
    {
        public long ProjectId { get; set; }
        public object Object { get; set; }
    }

    public class GetValueFiltersResponse
    {
        public string Table { get; set; }
        public long? TableId { get; set; }
        public string Group { get; set; }
        public long? GroupId { get; set; }
        public string ContractType { get; set; }
        public int ContractTypeId { get; set; }
        public string HoursType { get; set; }
        public int HoursTypeId { get; set; }
        public string Unit { get; set; }
        public long? UnitId { get; set; }
        public string WithOccupants { get; set; }
        public bool IsWithOccupants { get; set; }
        public string DisplayBy { get; set; }
        public int DisplayById { get; set; }
        public string Scenario { get; set; }
        public int ScenarioId { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
    }

    public class GetValueFiltersHandler : IRequestHandler<GetValueFiltersRequest, GetValueFiltersResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetValueFiltersHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<GetValueFiltersResponse> Handle(GetValueFiltersRequest request, CancellationToken cancellationToken)
        {
            var obj = request.Object;
            Type type = obj.GetType();

            var listEnum = Enum.GetValues(typeof(ValueFiltersShareType)) as
                            IEnumerable<ValueFiltersShareType>;

            var result = new GetValueFiltersResponse();

            foreach (var item in listEnum)
            {
                var property = type.GetProperty(item.ToString());

                if (property == null) continue;

                switch (item)
                {
                    case ValueFiltersShareType.TableId:

                        result.Table = await GetTable(request.ProjectId, obj, property);
                        result.TableId = GetId<long?>(obj, property);
                        continue;

                    case ValueFiltersShareType.GroupId:
                        result.Group = await GetGroup(request.ProjectId, obj, property);
                        result.GroupId = GetId<long?>(obj, property);
                        continue;

                    case ValueFiltersShareType.ContractType:

                        result.ContractType = GetContractType(obj, property);
                        result.ContractTypeId = GetId<int>(obj, property);
                        continue;

                    case ValueFiltersShareType.HoursType:

                        result.HoursType = GetHoursType(obj, property);
                        result.HoursTypeId = GetId<int>(obj, property);
                        continue;

                    case ValueFiltersShareType.UnitId:

                        result.Unit = await GetUnit(obj, property);
                        result.UnitId = GetId<long?>(obj, property);
                        continue;

                    case ValueFiltersShareType.ShowJustWithOccupants:

                        result.WithOccupants = GetWithOccupants(obj, property);
                        result.IsWithOccupants = GetId<bool>(obj, property);
                        continue;

                    case ValueFiltersShareType.DisplayBy:

                        result.DisplayBy = GetDisplayBy(obj, property);
                        result.DisplayById = GetId<int>(obj, property);
                        break;
                    case ValueFiltersShareType.Scenario:

                        result.Scenario = GetScenario(obj, property);
                        result.ScenarioId = GetId<int>(obj, property);
                        continue;

                    case ValueFiltersShareType.IsMM:

                        result.Scenario = GetMM(obj, property);
                        result.ScenarioId = (int)DisplayMMMIEnum.MM;
                        continue;

                    case ValueFiltersShareType.IsMI:

                        result.Scenario = GetMI(obj, property);
                        result.ScenarioId = (int)DisplayMMMIEnum.MI;
                        continue;

                    default:
                        break;
                }

            }

            return result;
        }

        private string GetDisplayBy(object obj, PropertyInfo property)
        {
            var value = property.GetValue(obj);
            if (value != null)
            {
                if (property.PropertyType.Name.Equals(typeof(DisplayByMapPositionEnum).Name))
                {
                    return ((DisplayByMapPositionEnum)value).GetDescription();
                }

                if (property.PropertyType.Name.Equals(typeof(DisplayByPositioningEnum).Name))
                {
                    return ((DisplayByPositioningEnum)value).GetDescription();
                }
            }
            return string.Empty;
        }

        private string GetMM(object obj, PropertyInfo property)
        {
            var value = property.GetValue(obj);
            if (value != null && (bool)value)
            {
                return DisplayMMMIEnum.MM.GetDescription();
            }
            return string.Empty;
        }

        private string GetMI(object obj, PropertyInfo property)
        {
            var value = property.GetValue(obj);
            if (value != null && (bool)value)
            {
                return DisplayMMMIEnum.MI.GetDescription();
            }
            return string.Empty;
        }

        private async Task<string> GetGroup(long projectId, object obj, PropertyInfo property)
        {
            var value = property.GetValue(obj);
            if (value == null)
                return "Todos";
            else
                return await _unitOfWork.GetRepository<GruposProjetosSalaryMark, long>()
                    .GetAsync(x => x.Where(gp => gp.GruposProjetosSmidLocal == (long)value &&
                     gp.ProjetoId == projectId).Select(s => s.GrupoSm));
        }

        private T GetId<T>(object obj, PropertyInfo property)
        {
            var value = property.GetValue(obj);

            return (T)value;
        }
        private async Task<string> GetTable(long projectId, object obj, PropertyInfo property)
        {
            var value = property.GetValue(obj);

            if (value == null)
            {
                return "Todas";
            }
            else
            {
                return await _unitOfWork.GetRepository<TabelasSalariais, long>()
                    .GetAsync(x => x.Where(ts => ts.TabelaSalarialIdLocal == (long)value &&
                     ts.ProjetoId == projectId).Select(s => s.TabelaSalarial));
            }
        }

        private string GetContractType(object obj, PropertyInfo property)
        {
            var value = property.GetValue(obj);
            if (value == null)
                return ContractTypeEnum.CLT.GetDescription();

            return ((ContractTypeEnum)value).GetDescription();
        }
        private string GetHoursType(object obj, PropertyInfo property)
        {
            var value = property.GetValue(obj);
            if (value == null)
                return DataBaseSalaryEnum.MonthSalary.GetDescription();

            return ((DataBaseSalaryEnum)value).GetDescription();
        }

        private string GetScenario(object obj, PropertyInfo property)
        {
            var value = property.GetValue(obj);
            if (value == null)
                return DisplayMMMIEnum.MM.GetDescription();

            return ((DisplayMMMIEnum)value).GetDescription();
        }
        private string GetWithOccupants(object obj, PropertyInfo property)
        {
            var value = property.GetValue(obj);
            if (value == null || !(bool)value)
                return "Sem Ocupantes";

            return "Com Ocupantes";
        }

        private async Task<string> GetUnit(object obj, PropertyInfo property)
        {
            var value = property.GetValue(obj);
            if (value == null)
            {
                return "Todas";
            }

            return await _unitOfWork.GetRepository<Empresas, long>().GetAsync(x =>
                                x.Where(e => e.Id == (long)value).Select(s =>
                                string.IsNullOrWhiteSpace(s.NomeFantasia) ?
                                (string.IsNullOrWhiteSpace(s.RazaoSocial) ? string.Empty : s.RazaoSocial) :
                                s.NomeFantasia));
        }
    }



}
