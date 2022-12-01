using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using Newtonsoft.Json;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Application.Interactors
{
    public class InternalLabelsRequest
    {
        public long UserId { get; set; }
        public long CompanyId { get; set; }
        public List<SaveInternalLabel> InternalLabels { get; set; }
        public bool CanEditLocalLabels { get; set; }
        public bool CanEditGlobalLabels { get; set; }
    }
    public class SaveInternalLabel
    {
        public InternalLabelsEnum ColumnId { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
    public class SaveInternalLabelsInteractor : ISaveInternalLabelsInteractor
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaveInternalLabelsInteractor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Handler(InternalLabelsRequest request)
        {
            var internalLabels = Enum.GetValues(typeof(InternalLabelsEnum)) as IEnumerable<InternalLabelsEnum>;

            if (!request.InternalLabels.Safe().Any() ||
                !request.CanEditLocalLabels) return;

            var globals = new List<InternalLabelsEnum>{ InternalLabelsEnum.GSM, InternalLabelsEnum.Area,
            InternalLabelsEnum.Profile, InternalLabelsEnum.PositionSalaryMark, InternalLabelsEnum.ParameterOne,
            InternalLabelsEnum.ParameterTwo, InternalLabelsEnum.ParameterThree };
            await UpdateGlobalLabels(request, globals);

            var userLabels = await _unitOfWork.GetRepository<UsuarioRotulosSM, long>()
                .GetListAsync(x => x.Where(up => up.UsuarioId == request.UserId));

            var internalUserLabels = new List<UsuarioRotulosSM>();
            request.InternalLabels
            .Where(inl => internalLabels.Contains(inl.ColumnId) &&
                          !globals.Contains(inl.ColumnId)).ToList()
            .ForEach(il =>
            {
                var userLabelSaved = userLabels.FirstOrDefault(ul => ul.CodigoInternoRoluto == (int)il.ColumnId);
                if (userLabelSaved == null)
                {
                    var newLabel = new UsuarioRotulosSM
                    {
                        CodigoInternoRoluto = (int)il.ColumnId,
                        NovoRotulo = il.Name,
                        UsuarioId = request.UserId
                    };
                    internalUserLabels.Add(newLabel);
                    return;
                }
                userLabelSaved.NovoRotulo = il.Name;
            });
            _unitOfWork.GetRepository<UsuarioRotulosSM, long>().Add(internalUserLabels);
            _unitOfWork.GetRepository<UsuarioRotulosSM, long>().Update(userLabels);
            await _unitOfWork.CommitAsync();
        }

        private async Task UpdateGlobalLabels(InternalLabelsRequest request, List<InternalLabelsEnum> globals)
        {
            if (!request.InternalLabels.Any(il => globals.Contains(il.ColumnId)) && request.CanEditGlobalLabels)
                return;
            var companyProfile = await _unitOfWork.GetRepository<EmpresaPermissaoSm, long>()
                            .GetAsync(g => g.Where(ep => ep.EmpresaId == request.CompanyId));
            if (companyProfile == null)
                return;
            var globalLabels = new List<GlobalLabelsJson>();
            companyProfile.RotulosGlobais.TryParseJson(out globalLabels);
            globalLabels.ForEach(gl =>
            {
                switch (gl.Id)
                {
                    case (long)GlobalLabelEnum.Area:
                        var global = request.InternalLabels.FirstOrDefault(il => il.ColumnId == InternalLabelsEnum.Area);
                        if (global != null)
                            gl.Alias = global.Name;
                        break;
                    case (long)GlobalLabelEnum.CareerAxis:
                        var area = request.InternalLabels.FirstOrDefault(il => il.ColumnId == InternalLabelsEnum.Area);
                        if (area != null)
                            gl.Alias = area.Name;
                        break;
                    case (long)GlobalLabelEnum.GSM:
                        var gsm = request.InternalLabels.FirstOrDefault(il => il.ColumnId == InternalLabelsEnum.Area);
                        if (gsm != null)
                            gl.Alias = gsm.Name;
                        break;
                    case (long)GlobalLabelEnum.Level:
                        var level = request.InternalLabels.FirstOrDefault(il => il.ColumnId == InternalLabelsEnum.Area);
                        if (level != null)
                            gl.Alias = level.Name;
                        break;
                    case (long)GlobalLabelEnum.Parameter1:
                        var param1 = request.InternalLabels.FirstOrDefault(il => il.ColumnId == InternalLabelsEnum.Area);
                        if (param1 != null)
                            gl.Alias = param1.Name;
                        break;
                    case (long)GlobalLabelEnum.Parameter2:
                        var param2 = request.InternalLabels.FirstOrDefault(il => il.ColumnId == InternalLabelsEnum.Area);
                        if (param2 != null)
                            gl.Alias = param2.Name;
                        break;
                    case (long)GlobalLabelEnum.Parameter3:
                        var param3 = request.InternalLabels.FirstOrDefault(il => il.ColumnId == InternalLabelsEnum.Area);
                        if (param3 != null)
                            gl.Alias = param3.Name;
                        break;
                    case (long)GlobalLabelEnum.Profile:
                        var profile = request.InternalLabels.FirstOrDefault(il => il.ColumnId == InternalLabelsEnum.Area);
                        if (profile != null)
                            gl.Alias = profile.Name;
                        break;
                    case (long)GlobalLabelEnum.PositionSalaryMark:
                        var positionSalaryMark = request.InternalLabels.FirstOrDefault(il => il.ColumnId == InternalLabelsEnum.Area);
                        if (positionSalaryMark != null)
                            gl.Alias = positionSalaryMark.Name;
                        break;
                }
            });
            companyProfile.RotulosGlobais = JsonConvert.SerializeObject(globalLabels);
            _unitOfWork.GetRepository<EmpresaPermissaoSm, long>().Update(companyProfile);
        }
    }
}
