using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SM.Application.Share.Validators;
using SM.Domain.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Share.Command
{
    public class GenerateKeySaveRequest : IRequest<string>
    {
        public long UserId { get; set; }
        public long ProjectId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long ModuleId { get; set; }
        public long? ModuleSubItemId { get; set; }
        public object Parameters { get; set; }
        public IEnumerable<object> ColumnsExcluded { get; set; } = new List<object>(); // columns exp

    }

    public class GenerateKeySaveHandler : IRequestHandler<GenerateKeySaveRequest, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SigningConfiguration _signingConfiguration;
        private readonly IValidator<SharePemissionException> _validator;
        private readonly ValidatorResponse _validatorResponse;

        public GenerateKeySaveHandler(IUnitOfWork unitOfWork,
            SigningConfiguration signingConfiguration,
            IValidator<SharePemissionException> validator,
            ValidatorResponse validatorResponse)
        {
            _unitOfWork = unitOfWork;
            _signingConfiguration = signingConfiguration;
            _validator = validator;
            _validatorResponse = validatorResponse;
        }
        public async Task<string> Handle(GenerateKeySaveRequest request, CancellationToken cancellationToken)
        {

            var resName = _validator.Validate(new SharePemissionException
            {
                UserId = request.UserId
            });

            if (!resName.IsValid)
                _validatorResponse.AddNotifications(resName.Errors.ToList());

            //convert parameters to json
            string paramerters =
                JToken.Parse(request.Parameters.ToString()).ToString();


            string columns = !string.IsNullOrWhiteSpace(request.ColumnsExcluded.FirstOrDefault().ToString()) ?
                JToken.Parse(request.ColumnsExcluded.FirstOrDefault().ToString()).ToString() :
                string.Empty;


            string companiesId = request.CompaniesId.Any() ?
                    JsonConvert.SerializeObject(request.CompaniesId) :
                    string.Empty;

            //always save in table
            //create key
            string secretKey = $"{_signingConfiguration.SecretKey.GenerateSystemKey()}.{StringExtensions.GenerateRandomString(10)}".ToMD5Hash();

            _unitOfWork.GetRepository<CompartilharSm, long>()
                .Add(new CompartilharSm
                {
                    ChaveSecreta = secretKey,
                    DataDeCompartilhamento = DateTime.Today,
                    ModuloSmId = request.ModuleId,
                    ModuloSmItemId = request.ModuleSubItemId,
                    Parametros = paramerters,
                    UsuarioId = request.UserId,
                    EmpresasId = companiesId,
                    ProjetoId = request.ProjectId,
                    Colunas = columns
                });

            await _unitOfWork.CommitAsync();

            return secretKey;
        }
    }
}
