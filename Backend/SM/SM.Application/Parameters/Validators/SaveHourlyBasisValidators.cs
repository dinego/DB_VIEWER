using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using FluentValidation;
using FluentValidation.Results;
using SM.Application.Parameters.Command;
using System.Linq;

namespace SM.Application.Parameters.Validators
{
    public class SaveHourlyBasisValidators : AbstractValidator<SaveHourlyBasisRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SaveHourlyBasisValidators(IUnitOfWork unitOfWork)
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
