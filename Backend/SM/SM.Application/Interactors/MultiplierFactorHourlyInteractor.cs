using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Application.Interactors
{
    public class MultiplierFactorHourlyInteractor : IMultiplierFactorHourlyInteractor
    {
        private readonly IUnitOfWork _unitOfWork;

        public MultiplierFactorHourlyInteractor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<double> Handler(long projectId, DataBaseSalaryEnum dataBaseType = DataBaseSalaryEnum.MonthSalary, bool isPosition = true)
        {
            switch (dataBaseType)
            {
                case DataBaseSalaryEnum.MonthSalary:
                    return 1;

                case DataBaseSalaryEnum.HourSalary:
                    var positionSm = 
                        await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                        .GetListAsync(x => x.Where(cps => cps.Ativo.HasValue &&
                        cps.Ativo.Value &&
                        cps.ProjetoId == projectId));

                    if(positionSm.Count() == 0)
                        throw new Exception("Não foi encontrado valor pra salário hora");

                    var valueMoreFrequenty = positionSm
                                    .GroupBy(q => q.BaseHoraria)
                                    .OrderByDescending(gp => gp.Count())
                                    .Take(1)
                                    .Select(g => g.Key)
                                    .FirstOrDefault();

                    return 1/valueMoreFrequenty;

                case DataBaseSalaryEnum.YearSalary:
                    double? yearSalaryProject = await _unitOfWork.GetRepository<SalarioAnualParametroProjeto, long>()
                        .Include("SalarioAnual")
                        .GetAsync(x => x.Where(sp => sp.ProjetoSalaryMarkId == projectId)
                        .Select(x=> x.SalarioAnual != null ? x.SalarioAnual.Valor : (double?)null ));

                    if (yearSalaryProject.HasValue)
                        return yearSalaryProject.Value;

                    var yearSalary = await _unitOfWork.GetRepository<SalarioAnualParametro, long>()
                        .GetAsync(x => x.Where(sp => sp.Ativo.HasValue && sp.Ativo.Value));

                    if (yearSalary == null)
                        throw new Exception("Não foi encontrado valor pra salário anual");

                    return yearSalary.Valor;

                default:
                    return 1;
            }

        }

    }
}
