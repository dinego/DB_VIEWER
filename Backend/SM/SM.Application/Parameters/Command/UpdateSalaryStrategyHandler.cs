using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.Parameters.Validators;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Parameters.Command
{
    public class UpdateSalaryStrategyRequest : IRequest
    {
        public long ProjectId { get; set; }
        public long TableId { get; set; }
        public string Table { get; set; }
        public List<SalaryStrategyRequest> SalaryStrategy { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class SalaryStrategyRequest
    {
        public int ColPos { get; set; }
        public string Value { get; set; }
        public string @Type { get; set; }
        public long GroupId { get; set; }
        public long TrackId { get; set; }
    }

    public class UpdateSalaryStrategyHandler : IRequestHandler<UpdateSalaryStrategyRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validator;
        private readonly SalaryStrategyValidatorRule _validatorSalaryStrategy;
        public UpdateSalaryStrategyHandler(IUnitOfWork unitOfWork,
        ValidatorResponse validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _validatorSalaryStrategy = new SalaryStrategyValidatorRule(_validator, unitOfWork);
        }
        public async Task<Unit> Handle(UpdateSalaryStrategyRequest request, CancellationToken cancellationToken)
        {
            if (!_validatorSalaryStrategy.IsSatisfiedBy(request))
                return Unit.Value;

            var salaryTable = await _unitOfWork.GetRepository<TabelasSalariais, long>()
                .GetAsync(v => v.Where(w => w.TabelaSalarialIdLocal == request.TableId &&
                            w.ProjetoId == request.ProjectId));

            if (!salaryTable.TabelaSalarial.Trim().ToLower().Equals(request.Table.Trim().ToLower()))
            {
                salaryTable.TabelaSalarial = request.Table;
                _unitOfWork.GetRepository<TabelasSalariais, long>().Update(salaryTable);
            }
            var groupsId = request.SalaryStrategy.Select(res => res.GroupId).Distinct();
            var lstSaved = await _unitOfWork.GetRepository<TabelasSalariaisParametrosPolitica, long>()
            .GetListAsync(tpp => tpp.Where(tp => tp.ProjetoId == request.ProjectId &&
              groupsId.Contains(tp.GrupoProjetoMidLocal)));
            lstSaved.ForEach(ss =>
            {
                var salaryStrategy = request.SalaryStrategy.FirstOrDefault(sa => sa.GroupId == ss.GrupoProjetoMidLocal &&
                                                                            sa.TrackId == ss.FaixaSalarialId);
                if (salaryStrategy != null)
                    ss.RotuloPolitica = salaryStrategy.Value;
            });

            _unitOfWork.GetRepository<TabelasSalariaisParametrosPolitica, long>().Update(lstSaved);
            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
