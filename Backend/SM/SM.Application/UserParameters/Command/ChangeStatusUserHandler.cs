using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using SM.Application.UserParameters.Validators;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.UserParameters.Command
{
    public class ChangeStatusUserRequest: IRequest
    {
        public long UserId { get; set; }
        public bool Active { get; set; }
        public bool IsAdmin { get; set; }
    }
    public class ChangeStatusUserHandler : IRequestHandler<ChangeStatusUserRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validator;

        public ChangeStatusUserHandler(IUnitOfWork unitOfWork, ValidatorResponse validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public async Task<Unit> Handle(ChangeStatusUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.GetRepository<Usuarios, long>().GetAsync(x => x.Where(u => u.Id == request.UserId));
            var userValidator = new ChangeStatusUserValidatorRule(_validator);
            
            if (!userValidator.IsSatisfiedBy(user)) return Unit.Value;
            
            user.Status = request.Active;
            _unitOfWork.GetRepository<Usuarios, long>().Update(user);
            _unitOfWork.Commit();
            return Unit.Value;
        }
    }
}
