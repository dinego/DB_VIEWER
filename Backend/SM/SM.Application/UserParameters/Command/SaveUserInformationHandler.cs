using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using Newtonsoft.Json;
using SM.Domain.Common;
using SM.Application.UserParameters.Queries;
using SM.Application.UserParameters.Validators;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AgileObjects.AgileMapper.Extensions;

namespace SM.Application.UserParameters.Command
{
    public class SaveUserInformationRequest : IRequest
    {
        public bool IsAdmin { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Sector { get; set; }
        public bool Active { get; set; }
        public string Photo { get; set; }
        public DateTime LastAccess { get; set; }
        public SaveUserPermissions UserPermissions { get; set; }
    }

    public class SaveUserPermissions
    {
        public IEnumerable<SubFieldCheckedUser> Levels { get; set; }
        public IEnumerable<FieldCheckedUser> Sections { get; set; }
        public IEnumerable<FieldCheckedUser> Permission { get; set; }
        public IEnumerable<SubFieldCheckedUser> Areas { get; set; }
        public IEnumerable<FieldCheckedUser> Contents { get; set; }
    }
    public class SaveUserInformationHandler : IRequestHandler<SaveUserInformationRequest, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validator;

        public SaveUserInformationHandler(IUnitOfWork unitOfWork, ValidatorResponse validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public async Task<Unit> Handle(SaveUserInformationRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.GetRepository<Usuarios, long>()
                .Include("UsuarioPermissaoSm")
                .GetAsync(x => x.Where(up => up.Id == request.Id));
            var userValidator = new SaveUserValidatorPermissionRule(_validator);

            if (!userValidator.IsSatisfiedBy(user))
                return Unit.Value;
            user.Status = request.Active;
            var userPermission = user.UsuarioPermissaoSm.FirstOrDefault();
            var permission = new PermissionJson();
            userPermission.Permissao.TryParseJson(out permission);

            #region LEVELS/AREA
            var levelsExceptions = request.UserPermissions.Levels.Where(x => !x.IsChecked).Select(res => res.Id).ToList();
            List<long> areasExceptions = request.UserPermissions.Areas.Where(x => !x.IsChecked).Select(res => res.Id).ToList();
            #endregion

            #region SECTIONS
            var moduleExceptions = request.UserPermissions.Sections.Where(x => !x.IsChecked ||
                (x.IsChecked && x.SubItems.Any(sb => !sb.IsChecked)))
                .Select(res => new FieldCheckedUserJson
                {
                    Id = res.Id,
                    SubItems = res.SubItems.Where(sb => !sb.IsChecked).Select(sbr => sbr.Id)
                }).ToList();
            #endregion
            var userPersonsExceptions = request.UserPermissions.Contents
                                            .Where(ty => ty.Id == (long)ParameterDisplay.Person);
            #region CONTENTS
            var ignoreContents = new List<long> { (long)ParameterDisplay.Scenario, (long)ParameterDisplay.Analysis, (long)ParameterDisplay.Person };
            var userContentExceptions = request.UserPermissions.Contents.Where(x => !ignoreContents.Contains(x.Id) &&
                            (!x.IsChecked || x.SubItems.Any(sb => !sb.IsChecked)))
                            .Select(res => new FieldCheckedUserJson
                            {
                                Id = res.Id,
                                SubItems = res.SubItems.Where(sb => !sb.IsChecked).Select(sbr => sbr.Id)
                            }).ToList();

            if (userPersonsExceptions.Any(person => person.SubItems.Any(sb => sb.Id == (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions && !person.IsChecked)) &&
                !permission.Contents.Any(c => c.Id != (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions))
                userContentExceptions.Add(new FieldCheckedUserJson { Id = (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions });
            else if (userPersonsExceptions.Any(person => person.SubItems.Any(sb => sb.Id == (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions && person.IsChecked)) &&
                permission.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions))
                userContentExceptions = userContentExceptions.Where(c => c.Id != (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions).ToList();
            #endregion

            #region PERMISSION            
            var userPermissionExceptions = request.UserPermissions.Permission
                                            .Select(res => new PermissionFieldJson
                                            {
                                                Id = res.Id,
                                                IsChecked = res.IsChecked,
                                                SubItems = res.SubItems.Where(sb => !sb.IsChecked).Select(sbr => sbr.Id)
                                            }).ToList();

            var userAnalysisException = request.UserPermissions.Contents
                                         .Where(c => c.Id == (long)ParameterDisplay.Analysis &&
                                         c.SubItems.Any(sb => sb.Id == (long)PermissionItensEnum.MoveNextStep))
                                         .SelectMany(res => res.SubItems).FirstOrDefault();
            if (userAnalysisException != null)
                userPermissionExceptions.Add(userAnalysisException.Map().ToANew<PermissionFieldJson>());
            if (userPersonsExceptions.Safe().Any(person => person.SubItems.Any(sb => sb.Id == (long)PermissionItensEnum.InactivePerson)))
            {
                var inactivePerson = userPersonsExceptions.Where(person => person.SubItems
                                                                            .Any(sb => sb.Id == (long)PermissionItensEnum.InactivePerson))
                                                          .SelectMany(res => res.SubItems.Where(sb => sb.Id == (long)PermissionItensEnum.InactivePerson))
                                                          .FirstOrDefault();
                if (inactivePerson != null)
                    userPermissionExceptions.Add(inactivePerson.Map().ToANew<PermissionFieldJson>());
            }

            var permissionSaved = permission.Permission.Where(x => !userPermissionExceptions.Any(up => up.Id == x.Id) ||
                                                              x.Id == (long)PermissionItensEnum.Share).ToList();
            userPermissionExceptions.Where(up => up.Id == (long)PermissionItensEnum.Share || (!up.IsChecked ||
                                          (up.IsChecked && up.SubItems.Any()))).ForEach(up =>
            {
                switch (up.Id)
                {
                    case (long)PermissionItensEnum.Share:
                        var share = permissionSaved.FirstOrDefault(x => x.Id == (long)PermissionItensEnum.Share);
                        if (share != null)
                            share.IsChecked = up.IsChecked;
                        break;
                    default:
                        permissionSaved.Add(up);
                        break;
                }
            });
            #endregion

            #region SCENARIOS
            var userScenarioExceptions = request.UserPermissions.Contents
                            .Where(x => x.Id == (long)ParameterDisplay.Scenario &&
                                        x.SubItems.Any(sb => !sb.IsChecked))
                            .SelectMany(res => res.SubItems.Where(sb => !sb.IsChecked).Select(sb => sb.Id)).ToList();
            if (permission != null && permission.Display != null)
            {
                foreach (var scenario in permission.Display.Scenario)
                {
                    scenario.IsChecked = !userScenarioExceptions.Contains(scenario.Id);
                }
            }
            #endregion

            if (permission != null)
            {
                permission.Areas = areasExceptions;
                permission.Contents = userContentExceptions.Safe();
                permission.Levels = levelsExceptions.Safe();
                permission.Permission = permissionSaved;
                permission.Modules = moduleExceptions;
                userPermission.Permissao = JsonConvert.SerializeObject(permission);
                _unitOfWork.GetRepository<Usuarios, long>().Update(user);
                _unitOfWork.Commit();
            }
            return Unit.Value;
        }
    }
}
