using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Abstractions.Behaviours;
using SM.Application.Parameters.Validators;
using AgileObjects.AgileMapper.Extensions;
using SM.Domain.Common;
using CMC.Common.Extensions;
using SM.Domain.Enum;
using Newtonsoft.Json;

namespace SM.Application.Parameters.Command
{
    public class UpdateDisplayConfigurationRequest : IRequest
    {
        public bool IsAdmin { get; set; }
        public long ProjectId { get; set; }
        public long UserId { get; set; }
        public IEnumerable<DisplayConfigurationData> DisplayConfiguration { get; set; }
    }

    public class DisplayConfigurationData
    {
        public long Id { get; set; }
        public List<long> SubItems { get; set; }
    }

    public class UpdateDisplayConfigurationHandler : IRequestHandler<UpdateDisplayConfigurationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly DisplayConfigurationRule _validateDisplay;
        private readonly ValidatorResponse _validator;

        public UpdateDisplayConfigurationHandler(IUnitOfWork unitOfWork,
        ValidatorResponse validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _validateDisplay = new DisplayConfigurationRule(_validator);
        }
        public async Task<Unit> Handle(UpdateDisplayConfigurationRequest request, CancellationToken cancellationToken)
        {
            if (!_validateDisplay.IsSatisfiedBy(request.Map().ToANew<DisplayConfigurationValidator>()))
                return Unit.Value;

            var userPermission = await _unitOfWork.GetRepository<UsuarioPermissaoSm, long>()
                .GetAsync(g => g.Where(up => up.UsuarioId == request.UserId)
                .Select(res => new
                {
                    CompanyPermissionId = res.EmpresaPermissaoId,
                    Permission = res.Permissao
                }));

            if (userPermission == null)
            {
                _validator.AddNotification("Permissões da empresa não foram encontradas.");
                return Unit.Value;
            }

            var adminPermission = new PermissionJson();
            userPermission.Permission.TryParseJson(out adminPermission);

            var usersPermissions = await _unitOfWork.GetRepository<UsuarioPermissaoSm, long>()
            .GetListAsync(g => g.Where(ep => ep.EmpresaPermissaoId == userPermission.CompanyPermissionId));

            var contents = adminPermission.Contents.ToList();

            #region TYPE OF CONTRACT
            var typeOfContract = request.DisplayConfiguration
                                        .Where(ty => ty.Id == (int)ParameterDisplay.TypeOfContract)
                                        .SelectMany(res => res.SubItems);
            adminPermission.TypeOfContract = typeOfContract.Safe().Any() ? typeOfContract : new List<long>();
            #endregion

            #region HOURLY BASIS
            var hourlyBasis = request.DisplayConfiguration
                                        .Where(ty => ty.Id == (int)ParameterDisplay.HourlyBasis)
                                        .SelectMany(res => res.SubItems);
            adminPermission.DataBase = hourlyBasis.Safe().Any() ? hourlyBasis : new List<long>();
            #endregion

            #region SCENARIOS
            var scenarios = request.DisplayConfiguration
                                        .Where(ty => ty.Id == (long)ParameterDisplay.Scenario)
                                        .SelectMany(res => res.SubItems);
            if (adminPermission.Display != null)
                adminPermission.Display.Scenario.Safe().ForEach(sc =>
                {
                    bool hasException = request.DisplayConfiguration
                                        .Any(ty => ty.Id == (long)ParameterDisplay.Scenario &&
                                             ty.SubItems.Contains(sc.Id));
                    sc.IsChecked = !hasException;
                });
            #endregion
            var permission = adminPermission.Permission.Safe().ToList();
            #region PERSONS

            if (request.DisplayConfiguration.Any(person => person.Id == (long)ParameterDisplay.Person &&
                                                 person.SubItems.Contains((long)PermissionItensEnum.InactivePerson)) &&
                !permission.Any(c => c.Id == (long)PermissionItensEnum.InactivePerson))
                permission.Add(new PermissionFieldJson { Id = (long)PermissionItensEnum.InactivePerson, IsChecked = false });
            if (!request.DisplayConfiguration.Any(person => person.Id == (long)ParameterDisplay.Person &&
                            person.SubItems.Contains((long)PermissionItensEnum.InactivePerson)) &&
                permission.Any(c => c.Id == (long)PermissionItensEnum.InactivePerson))
                permission = permission.Where(c => c.Id != (long)PermissionItensEnum.InactivePerson).ToList();

            if (request.DisplayConfiguration.Any(person => person.Id == (long)ParameterDisplay.Person &&
                            person.SubItems.Contains((long)ContentsSubItemsEnum.ShowPeopleWithoutPositions)) &&
                !adminPermission.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions))
                contents.Add(new FieldCheckedUserJson { Id = (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions });
            if (!request.DisplayConfiguration.Any(person => person.Id == (long)ParameterDisplay.Person &&
                            person.SubItems.Contains((long)ContentsSubItemsEnum.ShowPeopleWithoutPositions)) &&
                adminPermission.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions))
                contents = contents.Where(c => c.Id != (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions).ToList();

            #endregion

            #region ANALYSIS
            if (request.DisplayConfiguration.Any(ty => ty.Id == (long)ParameterDisplay.Analysis &&
                                                                ty.SubItems.Contains((long)PermissionItensEnum.MoveNextStep)) &&
                !permission.Any(c => c.Id == (long)PermissionItensEnum.MoveNextStep))
                permission.Add(new PermissionFieldJson { Id = (long)PermissionItensEnum.MoveNextStep, IsChecked = false });
            if (!request.DisplayConfiguration.Any(ty => ty.Id == (long)ParameterDisplay.Analysis &&
                            ty.SubItems.Contains((long)PermissionItensEnum.MoveNextStep)) &&
                permission.Any(c => c.Id == (long)PermissionItensEnum.MoveNextStep))
                permission = permission.Where(c => c.Id != (long)PermissionItensEnum.MoveNextStep).ToList();

            #endregion

            #region TECNICAL ADJUST
            var tecnicalAdjustDisplay = request.DisplayConfiguration
                                        .Where(ty => ty.Id == (long)ParameterDisplay.TecnicalAdjust)
                                        .SelectMany(res => res.SubItems);
            if (tecnicalAdjustDisplay.Safe().Any() &&
                !adminPermission.Contents.Any(c => c.Id != (long)ContentsSubItemsEnum.ShowTecnicalAdjust))
                contents.Add(new FieldCheckedUserJson { Id = (long)ContentsSubItemsEnum.ShowTecnicalAdjust });
            if (!tecnicalAdjustDisplay.Safe().Any() &&
                adminPermission.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.ShowTecnicalAdjust))
                contents = contents.Where(c => c.Id != (long)ContentsSubItemsEnum.ShowTecnicalAdjust).ToList();
            #endregion

            adminPermission.Contents = contents;

            string permissionEdited = JsonConvert.SerializeObject(adminPermission);
            usersPermissions.ForEach(up =>
            {
                if (up.UsuarioId == request.UserId)
                {
                    up.ConfiguraExibicaoAdmin = permissionEdited;
                    return;
                }
                up.Permissao = permissionEdited;
            });
            _unitOfWork.GetRepository<UsuarioPermissaoSm, long>().Update(usersPermissions);
            _unitOfWork.Commit();

            return Unit.Value;
        }
    }
}
