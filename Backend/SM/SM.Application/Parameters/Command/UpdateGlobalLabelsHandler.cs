using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using Newtonsoft.Json;
using SM.Domain.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Parameters.Command
{
    public class UpdateGlobalLabelsRequest : IRequest
    {
        public long UserId { get; set; }
        public bool IsAdmin { get; set; }
        public long ProjectId { get; set; }
        public IEnumerable<GlobalLabelRequest> GlobalLabels { get; set; } = new List<GlobalLabelRequest>();
    }

    public class GlobalLabelRequest
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public bool IsChecked { get; set; }
        public bool IsDefault { get; set; }
    }

    public class UpdateGlobalLabelsHandler : IRequestHandler<UpdateGlobalLabelsRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validator;
        public UpdateGlobalLabelsHandler(IUnitOfWork unitOfWork,
        ValidatorResponse validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public async Task<Unit> Handle(UpdateGlobalLabelsRequest request, CancellationToken cancellationToken)
        {
            if (!request.GlobalLabels.Safe().Any())
                return Unit.Value;
            long? projectCompanyId = await _unitOfWork.GetRepository<ProjetosSalaryMark, long>()
                .GetAsync(x => x.Where(g => g.Id == request.ProjectId)
                .Select(res => res.CodigoPai));

            if (!projectCompanyId.HasValue)
            {
                _validator.AddNotification("Não foi encontrado nenhum projeto");
                return Unit.Value;
            }

            var companyPermission = await _unitOfWork.GetRepository<EmpresaPermissaoSm, long>()
                .GetAsync(x => x.Where(g => g.EmpresaId == projectCompanyId));
            if (companyPermission == null) return Unit.Value;

            var companyGlobalLabels = new List<GlobalLabelsJson>();
            companyPermission.RotulosGlobais.TryParseJson(out companyGlobalLabels);
            companyGlobalLabels.ForEach(gl =>
            {
                var globalLabelEdited = request.GlobalLabels.FirstOrDefault(ugl => ugl.Id == gl.Id);
                if (globalLabelEdited == null)
                    return;
                gl.Alias = globalLabelEdited.Alias;
                gl.IsDefault = globalLabelEdited.IsDefault;
                gl.IsChecked = globalLabelEdited.IsChecked;
            });

            companyPermission.RotulosGlobais = JsonConvert.SerializeObject(companyGlobalLabels);
            _unitOfWork.GetRepository<EmpresaPermissaoSm, long>().Update(companyPermission);

            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }
    }
}
