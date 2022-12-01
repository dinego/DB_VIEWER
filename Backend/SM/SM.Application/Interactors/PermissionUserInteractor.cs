using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace SM.Application.Interactors
{
    public class PermissionUserInteractor : IPermissionUserInteractor
    {
        private readonly IUnitOfWork _unitOfWork;
        public PermissionUserInteractor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PermissionJson> Handler(long userId)
        {
            var user = await _unitOfWork.GetRepository<UsuarioPermissaoSm, long>()
                .GetAsync(x => x.Where(up => up.UsuarioId == userId));

            if (user == null)
                throw new Exception("Usuário não encontrado");

            var permission = new PermissionJson();
            user.Permissao.TryParseJson(out permission);

            return permission;
        }
    }
}
