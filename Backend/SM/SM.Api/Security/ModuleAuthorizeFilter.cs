using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Common;
using SM.Domain.Enum;
using System.Linq;
using System.Threading.Tasks;
namespace SM.Api.Security
{
    public class ModuleAuthorize : TypeFilterAttribute
    {
        public ModuleAuthorize(ModulesEnum module, ModulesSuItemsEnum modulesSuItemsEnum) : base(typeof(ModuleAuthorizeFilter))
        {
            Arguments = new object[] { module, modulesSuItemsEnum };
        }

    }

    public class ModuleAuthorizeFilter : IAuthorizationFilter
    {
        private readonly ModulesEnum _module;
        private readonly ModulesSuItemsEnum _subModule;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IUnitOfWork _unitOfWork;
        public ModuleAuthorizeFilter(IPermissionUserInteractor permissionUserInteractor,
            IUnitOfWork unitOfWork,
            ModulesEnum module,
            ModulesSuItemsEnum subModule)
        {
            _module = module;
            _subModule = subModule;
            _permissionUserInteractor = permissionUserInteractor;
            _unitOfWork = unitOfWork;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            long userId = context.HttpContext.User.GetUserId();
            Permissions userPermissions = context.HttpContext.User.GetUserPermissions();
            PermissionJson userPermission = _permissionUserInteractor.Handler(userId).Result;
            Permissions dbUserPermissions = GetDataBasePermissions(userPermission).Result;

            if (!userPermissions.Equals(dbUserPermissions))
            {
                context.Result = new UnauthorizedObjectResult(new { message = $"Suas permissões foram atualizadas e você será redirecionado para a tela de login." });
                return;
            }

            if (userPermission.Modules.Any(x => x.Id == (long)_module && !x.SubItems.Safe().Any() &&
                                          _subModule == ModulesSuItemsEnum.None))
            {
                context.Result = new UnauthorizedObjectResult(new { message = $"Suas permissões foram atualizadas e você será redirecionado para a tela de login." });
                return;
            }

            if (userPermission.Modules.Any(x => x.Id == (long)_module &&
            x.SubItems.Safe().Any(sb => sb == (long)_subModule)))
            {
                context.Result = new UnauthorizedObjectResult(new { message = $"Suas permissões foram atualizadas e você será redirecionado para a tela de login." });
                return;
            }
        }

        private async Task<Permissions> GetDataBasePermissions(PermissionJson user)
        {
            var typeOfContract = await _unitOfWork.GetRepository<TipoDeContrato, long>()
                                                    .GetListAsync(x => x.Where(ms => ms.Ativo.HasValue &&
                                                                               ms.Ativo.Value)
                                                    .Select(s => s.Id));

            var positionContents = user.Contents.Any(x => x.Id == (long)ContentsSubItens.Positions);

            var permission = new Permissions
            {
                CanDownloadExcel = !user.Permission.Any(x => x.Id == (long)PermissionItensEnum.Excel),
                CanShare = user.Permission.Any(x => x.Id == (long)PermissionItensEnum.Share && x.IsChecked),
                CanFilterOccupants = positionContents,
                CanEditConfigPJ = !user.Permission.Any(x => x.Id == (long)PermissionItensEnum.Parameter && x.SubItems.Any(sb => sb == (long)PermissionSubItemEnum.EditConfigPj)),
                CanEditHourlyBasis = !user.Permission.Any(x => x.Id == (long)PermissionItensEnum.Parameter && x.SubItems.Any(sb => sb == (long)PermissionSubItemEnum.EditHourlyBasis)),
                CanEditLevels = !user.Permission.Any(x => x.Id == (long)PermissionItensEnum.Parameter && x.SubItems.Any(sb => sb == (long)PermissionSubItemEnum.EditLevel)),
                CanEditSalaryStrategy = !user.Permission.Any(x => x.Id == (long)PermissionItensEnum.Parameter && x.SubItems.Any(sb => sb == (long)PermissionSubItemEnum.EditSalaryStrategic)),
                CanEditUser = !user.Permission.Any(x => x.Id == (long)PermissionItensEnum.Parameter && x.SubItems.Any(sb => sb == (long)PermissionSubItemEnum.EditUsers)),
                CanFilterTypeofContract = user.TypeOfContract.Safe().Count() < typeOfContract.Count(),
                CanFilterMI = user.Display != null && user.Display.Scenario.Safe().Any(sc => sc.Id == (long)FieldValueDefault.MI && sc.IsChecked),
                CanFilterMM = user.Display != null && user.Display.Scenario.Safe().Any(sc => sc.Id == (long)FieldValueDefault.MM && sc.IsChecked),
                CanEditGlobalLabels = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.RenameColumn &&
                                                                    x.SubItems.Any(sb => sb == (long)RenameColumnSubItemEnum.EditGlobalLabels)),
                CanEditLocalLabels = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.RenameColumn &&
                                                                    x.SubItems.Any(sb => sb == (long)RenameColumnSubItemEnum.EditLocalLabels)),
                InactivePerson = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.InactivePerson),
                CanDisplayEmployeeName = !user.Display.DisplaySections.Safe().Any(ds => ds.Id == (long)FieldValueDefault.Names &&
                                                                           ds.SubItems.Any(sb => sb == (long)NamePermissionEnum.DisplayEmployeeName)),
                CanDisplayBossName = !user.Display.DisplaySections.Safe().Any(ds => ds.Id == (long)FieldValueDefault.Names &&
                                                                           ds.SubItems.Any(sb => sb == (long)NamePermissionEnum.DisplayBossName)),
                CanEditProjectSalaryTablesValues = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.Parameter &&
                                                                  x.SubItems.Any(sb => sb == (long)PermissionSubItemEnum.EditSalaryTables)),
                CanChooseDefaultParameter = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.Parameter &&
                                                                        x.SubItems.Any(sb => sb == (long)PermissionSubItemEnum.EditDefaultParameter)),
                CanMoveNextStep = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.MoveNextStep),
                CanAddPeople = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.ManagementPeople &&
                                                                        x.SubItems.Any(sb => sb == (long)ManagementPeopleEnum.AddPeople)),
                CanAddPosition = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.ManagementPosition &&
                                                                        x.SubItems.Any(sb => sb == (long)ManagementPositionEnum.AddPosition)),
                CanDeletePeople = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.ManagementPeople &&
                                                                                        x.SubItems.Any(sb => sb == (long)ManagementPeopleEnum.DeletePeople)),
                CanDeletePosition = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.ManagementPosition &&
                                                                                         x.SubItems.Any(sb => sb == (long)ManagementPositionEnum.DeletePosition)),
                CanEditGSMMappingTable = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.ManagementPosition &&
                                                                                         x.SubItems.Any(sb => sb == (long)ManagementPositionEnum.EditGSMMappingTable)),
                CanEditListPosition = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.ManagementPosition &&
                                                                                        x.SubItems.Any(sb => sb == (long)ManagementPositionEnum.EditListPosition)),
                CanEditMappingPositionSM = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.ManagementPeople &&
                                                                                        x.SubItems.Any(sb => sb == (long)ManagementPeopleEnum.EditMappingPositionSM)),
                CanEditPeople = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.ManagementPeople &&
                                                                                        x.SubItems.Any(sb => sb == (long)ManagementPeopleEnum.EditPeople)),
                CanEditPosition = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.ManagementPosition &&
                                                                                                      x.SubItems.Any(sb => sb == (long)ManagementPositionEnum.EditPosition)),
                CanEditSalaryTableValues = !user.Permission.Safe().Any(x => x.Id == (long)PermissionItensEnum.ManagementPosition &&
                                                                                                        x.SubItems.Any(sb => sb == (long)ManagementPositionEnum.EditSalaryTableValues)),
            };
            return permission;
        }
    }
}
