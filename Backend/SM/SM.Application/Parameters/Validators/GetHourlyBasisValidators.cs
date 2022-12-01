using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Abstractions.Validators.Abstractions;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using FluentValidation;
using FluentValidation.Results;
using SM.Application.Parameters.Queries;
using System.Collections.Generic;
using System.Linq;

namespace SM.Application.Parameters.Validators
{
    public class HourlyBasisDataBaseRule : CompositeRule<List<BaseDeDados>>
    {
        private ValidatorResponse _validator;

        public HourlyBasisDataBaseRule(ValidatorResponse validator)
        {
            _validator = validator;
        }
        public override bool IsSatisfiedBy(List<BaseDeDados> model)
        {
            bool isValid = model != null && model.Count > 0;
            if (!isValid)
                _validator.AddNotification("Não foi encontrado nenhuma informação na tabela base de dados.");
            return isValid;
        }
    }

    public class HourlyBasisYearSalaryRule : CompositeRule<List<SalarioAnualParametro>>
    {
        private ValidatorResponse _validator;

        public HourlyBasisYearSalaryRule(ValidatorResponse validator)
        {
            _validator = validator;
        }
        public override bool IsSatisfiedBy(List<SalarioAnualParametro> model)
        {
            bool isValid = model != null && model.Any(y=> y.Ativo.HasValue && y.Ativo.Value);
            if (!isValid)
                _validator.AddNotification("Não foi encontrado nenhum parâmetro ativo na tabela de salários anuais parâmetros.");
            return isValid;
        }
    }

    public class GetHourlyBasisRequestValidator : AbstractValidator<GetHourlyBasisRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetHourlyBasisRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            RuleFor(x => x)
                        .Custom((request, context) => {

                            if (!request.IsAdmin)
                                context.AddFailure(new ValidationFailure("IsAdmin", "Somente os administradores podem acessar os dados de Base Horária."));

                            var companyId = unitOfWork.GetRepository<Empresas, long>()
                                                    .Include("ProjetosSalaryMarkEmpresas")
                                                    .Include("ProjetosSalaryMark")
                                                    .Get(x => x.Where(em => em.ProjetosSalaryMarkEmpresas.Any(ps => ps.EmpresaId == request.CompanyId))
                                                    .Select(res => res.Id));
                            if (companyId <= 0)
                                context.AddFailure(new ValidationFailure("CompanyId", $"Não foi encontrado nenhum projeto associado a empresa {request.CompanyId}."));
                        });
        }
    }
}
