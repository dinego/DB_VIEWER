using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.RemoveProfileImage
{
    public class RemoveProfileImageRequest : IRequest
    {
        public long UserId { get; set; }
    }

    public class RemoveProfileImageHandler : IRequestHandler<RemoveProfileImageRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validator;
        public RemoveProfileImageHandler(IUnitOfWork unitOfWork, ValidatorResponse validatorResponse)
        {
            _unitOfWork = unitOfWork;
            _validator = validatorResponse;
        }

        public async Task<Unit> Handle(RemoveProfileImageRequest request, CancellationToken cancellationToken)
        {
            var userData = await _unitOfWork.GetRepository<Usuarios, long>()
                                    .GetAsync(x => x.Where(u => u.Id == request.UserId)
                                    );

            if (userData == null)
            {
                _validator.AddNotification("Usuário não encontrado.");
                return Unit.Value;
            }
                
            userData.FotoPerfil = string.Empty;
            _unitOfWork.GetRepository<Usuarios, long>().Update(userData);
            _unitOfWork.Commit();
            return Unit.Value;
        }
    }
}

