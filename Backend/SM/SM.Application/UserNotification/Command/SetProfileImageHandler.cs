using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.SetProfileImage
{
    public class SetProfileImageRequest : IRequest
    {
        public long UserId { get; set; }
        public IFormFile Attachment { get; set; }
    }

    public class SetProfileImageHandler : IRequestHandler<SetProfileImageRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validator;
        public SetProfileImageHandler(IUnitOfWork unitOfWork, ValidatorResponse validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<Unit> Handle(SetProfileImageRequest request, CancellationToken cancellationToken)
        {
            long attachmentMaxSize = 2097152;
            var image = request.Attachment?.OpenReadStream();
            string imageBase64 = string.Empty;
            if (image != null)
            {
                if (request.Attachment.Length > attachmentMaxSize)
                {
                    _validator.AddNotification("O arquivo é superior a 2Mb");
                    return Unit.Value;
                }
                var formatListAccept = new List<string>() { "image/gif", "image/jpeg", "image/jpg", "image/png" };
                if (!formatListAccept.Contains(request.Attachment.ContentType))
                {
                    _validator.AddNotification("Anexe um dos formatos gif/jpg/jpeg/png");
                    return Unit.Value;
                }
                byte[] bytes;
                using (var memoryStream = new MemoryStream())
                {
                    request.Attachment.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }

                imageBase64 = Convert.ToBase64String(bytes);
            }

            var userData = await _unitOfWork.GetRepository<Usuarios, long>()
                                    .GetAsync(x => x.Where(u => u.Id == request.UserId)
                                    );

            if (userData == null)
            {
                _validator.AddNotification("Usuário não encontrada.");
                return Unit.Value;
            }
            userData.FotoPerfil = imageBase64;
            _unitOfWork.GetRepository<Usuarios, long>().Update(userData);
            _unitOfWork.Commit();
            return Unit.Value;
        }
    }
}

