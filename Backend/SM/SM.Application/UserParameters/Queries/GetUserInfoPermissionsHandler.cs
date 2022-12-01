using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using MediatR;
using Newtonsoft.Json;
using SM.Application.Interactors.Interfaces;
using SM.Application.UserParameters.Validators;
using SM.Domain.Common;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.UserParameters.Queries
{
    public class GetUserInfoPermissionsRequest : IRequest<GetUserInfoPermissionsResponse>
    {
        public long UserId { get; set; }
        public bool IsAdmin { get; set; }
        public long CompanyId { get; set; }
        public long AdminId { get; set; }
    }
    public class GetUserInfoPermissionsResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Sector { get; set; }
        public bool Active { get; set; }
        public string Photo { get; set; }
        public DateTime LastAccess { get; set; }
        public UserPermissions UserPermissions { get; set; }
    }

    public class UserPermissions
    {
        public IEnumerable<SubFieldCheckedUser> Levels { get; set; }
        public IEnumerable<FieldCheckedUser> Sections { get; set; }
        public IEnumerable<FieldCheckedUser> Permission { get; set; }
        public IEnumerable<SubFieldCheckedUser> Areas { get; set; }
        public IEnumerable<FieldCheckedUser> Contents { get; set; }
    }

    public class FieldCheckedUser
    {
        public string Name { get; set; }
        public long Id { get; set; } = 0;
        public bool IsChecked { get; set; } = true;
        public IEnumerable<SubFieldCheckedUser> SubItems { get; set; }

    }

    public class SubFieldCheckedUser
    {
        public string Name { get; set; }
        public long Id { get; set; } = 0;
        public bool IsChecked { get; set; } = true;

    }

    public class ProjectCompanyUser
    {
        public long ProjectId { get; set; }
        public long? CompanyFatherId { get; set; }
    }
    public class GetUserInfoPermissionsHandler : IRequestHandler<GetUserInfoPermissionsRequest, GetUserInfoPermissionsResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validator;
        private readonly IPermissionUserInteractor _permissionUserInteractor;

        public GetUserInfoPermissionsHandler(IUnitOfWork unitOfWork,
            ValidatorResponse validator,
            IPermissionUserInteractor permissionUserInteractor)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _permissionUserInteractor = permissionUserInteractor;
        }

        public async Task<GetUserInfoPermissionsResponse> Handle(GetUserInfoPermissionsRequest request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.GetRepository<Usuarios, long>()
                .Include("UsuarioPermissaoSm")
                .Include("Departamento")
                .GetAsync(x => x.Where(up => up.Id == request.UserId)
                .Select(s => new
                {
                    s.Id,
                    Name = s.Nome,
                    s.Email,
                    Photo = s.FotoPerfil,
                    LastAccess = s.UltimoAcesso,
                    Active = s.Status.HasValue && s.Status.Value,
                    Permissions = s.UsuarioPermissaoSm.FirstOrDefault() != null ? s.UsuarioPermissaoSm.FirstOrDefault().Permissao : string.Empty,
                    Sector = s.Departamento != null ? s.Departamento.Departamento : string.Empty
                }));

            if (user == null) { _validator.AddNotification("Usuário não encontrado"); return null; }

            var adminPermission = await _permissionUserInteractor.Handler(request.AdminId);
            if (adminPermission == null)
            {
                _validator.AddNotification("Não foi encontrado nenhum dado de permissionamento para o admnistrador.");
                return null;
            }

            var result = user.Map().ToANew<GetUserInfoPermissionsResponse>();

            var projectCompany = await _unitOfWork.GetRepository<ProjetosSalaryMarkEmpresas, long>()
                .GetAsync(x => x.Where(emp => emp.EmpresaId == request.CompanyId)
                                .Select(res => new ProjectCompanyUser
                                {
                                    ProjectId = res.ProjetoId,
                                    CompanyFatherId = res.Projeto != null ? res.Projeto.CodigoPai : (long?)null
                                }));

            var userPermission = new PermissionJson();
            user.Permissions.TryParseJson<PermissionJson>(out userPermission);

            var userPermissionValidator = new UserValidatorPermissionRule(_validator);
            var userProjectValidator = new UserValidatorProjectRule(_validator);

            if (!userPermissionValidator.IsSatisfiedBy(userPermission) || !userProjectValidator.IsSatisfiedBy(projectCompany))
                return null;

            if (userPermission != null && projectCompany != null && projectCompany.CompanyFatherId.HasValue)
            {
                var modules = await PrepareModules(userPermission, adminPermission);
                var permission = await PreparePermissions(userPermission, adminPermission);
                var levels = await PrepareLevels(userPermission, projectCompany.CompanyFatherId.Value, adminPermission);
                var areas = await PrepareAreas(userPermission, projectCompany, adminPermission);
                var contents = await PrepareContents(userPermission, projectCompany.ProjectId, projectCompany.CompanyFatherId.Value, adminPermission);

                result.UserPermissions = new UserPermissions
                {
                    Areas = areas,
                    Contents = contents,
                    Levels = levels,
                    Permission = permission,
                    Sections = modules
                };
            }
            return result;
        }

        private async Task<List<FieldCheckedUser>> PrepareModules(PermissionJson userPermission, PermissionJson adminPermission)
        {
            var lstModulesIgnored = new List<long> { (long)ModulesEnum.Parameters, (long)ModulesEnum.Home };
            var userExcept = userPermission.Modules.SelectMany(x => x.SubItems);
            var modules = await _unitOfWork.GetRepository<ModulosSm, long>()
                .Include("ModulosSmsubItens")
                .GetListAsync(x => x.Where(ms => ms.Status.HasValue && ms.Status.Value && !lstModulesIgnored.Contains(ms.Id))
                .Select(s => new FieldCheckedUser
                {
                    Id = s.Id,
                    Name = s.Nome,
                    SubItems = s.ModulosSmsubItens
                                .Where(msi => msi.Status.HasValue && msi.Status.Value)
                                .Select(se => new SubFieldCheckedUser
                                {
                                    Id = se.Id,
                                    IsChecked = !userExcept.Any(x => x == se.Id),
                                    Name = se.SubItem
                                })
                }));

            return PrepareData(adminPermission, modules);
        }
        private async Task<List<FieldCheckedUser>> PreparePermissions(PermissionJson userPermission, PermissionJson adminPermission)
        {
            var permissionExcept = userPermission?
                    .Permission.Where(x => !x.IsChecked)
                    .Select(s => s.Id);

            var subPermissionExcept = userPermission?
                    .Permission.Where(x => !x.IsChecked || (x.SubItems.Any()))
                    .SelectMany(s => s.SubItems.Safe());

            var adminExcept = adminPermission.Permission.Where(x =>
                !x.IsChecked || x.SubItems.Any())
                .Select(x => x.Id);

            var permission = await _unitOfWork.GetRepository<Permissionamento, long>()
                            .Include("PermissionamentoSubItens")
                            .GetListAsync(x => x.Where(ms => ms.Status.HasValue && ms.Status.Value)
                            .Where(w => w.Id != (long)PermissionItensEnum.InactivePerson &&
                            w.Id != (long)PermissionItensEnum.RenameColumn &&
                            w.Id != (long)PermissionItensEnum.Parameter &&
                            w.Id != (long)PermissionItensEnum.MoveNextStep &&
                            !adminExcept.Contains(w.Id))
                            .Select(s => new FieldCheckedUser
                            {
                                Id = s.Id,
                                Name = s.Nome,
                                IsChecked = !permissionExcept.Contains(s.Id),
                                SubItems = s.PermissionamentoSubItens
                                            .Where(msi => msi.Status.HasValue && msi.Status.Value)
                                            .Select(se => new SubFieldCheckedUser
                                            {
                                                Id = se.Id,
                                                Name = se.SubItem,
                                                IsChecked = subPermissionExcept.Safe().Any() ?
                                                        !subPermissionExcept.Safe().Contains(se.Id) : true
                                            })
                            }));

            return permission;
        }
        private async Task<IEnumerable<SubFieldCheckedUser>> PrepareLevels(PermissionJson userPermission, long companyId, PermissionJson adminPermission)
        {
            var levelsResult = await _unitOfWork.GetRepository<Niveis, int>()
                .GetListAsync(x =>
                x.Where(n => n.Ativo.HasValue && n.Ativo.Value && !adminPermission.Levels.Contains(n.Id)));

            var levelsIdFilter = await _unitOfWork.GetRepository<NivelEmpresas, long>()
                .GetListAsync(x => x.Where(c => c.EmpresaId == companyId)
                .Select(s => s.NivelId));

            levelsResult = levelsResult.Where(l => levelsIdFilter.Contains(l.Id)).ToList();

            var levels = levelsResult.Count() > 0 ? levelsResult
                .Select(s => new SubFieldCheckedUser
                {
                    Id = s.Id,
                    Name = s.Nivel,
                    IsChecked = userPermission.Levels.Any() ?
                    !userPermission.Levels.Contains(s.Id) : true,
                }) :
            new List<SubFieldCheckedUser>();
            return levels;
        }
        private async Task<IEnumerable<SubFieldCheckedUser>> PrepareAreas(PermissionJson userPermission, ProjectCompanyUser projectCompany, PermissionJson adminPermission)
        {
            var areaParameters = await _unitOfWork.GetRepository<ParametrosProjetosSMLista, long>()
                              .GetListAsync(x => x.Where(cps => cps.Ativo &&
                              cps.ParametroSMTipoId == (long)ParametersProjectsTypes.Area &&
                              cps.ProjetoId == projectCompany.ProjectId &&
                              !adminPermission.Areas.Contains(cps.Id))
                              .Select(res => new { res.Id, res.ParametroProjetoSMLista }));

            var areas = areaParameters.Count() > 0 ?
               areaParameters.Distinct()
               .Select(res => new SubFieldCheckedUser
               {
                   Id = res.Id,
                   Name = res.ParametroProjetoSMLista,
                   IsChecked = !userPermission.Areas.Contains(res.Id),
               })
               .OrderBy(o => o.Name).ToList() :
               new List<SubFieldCheckedUser>();
            return areas;
        }
        private async Task<IEnumerable<FieldCheckedUser>> PrepareContents(PermissionJson userPermission, long projectId, long companyId, PermissionJson adminPermission)
        {
            var contents = new List<FieldCheckedUser>();

            var adminUnitsExcept = adminPermission.Contents.Where(x => x.Id == (long)ContentsSubItens.Units).SelectMany(x => x.SubItems);
            //units
            var companyByProjectSalary = await _unitOfWork.GetRepository<ProjetosSalaryMarkEmpresas, long>()
                .Include("Projeto")
                .Include("Empresa")
                .GetListAsync(x => x.Where(ps => ps.ProjetoId == projectId && !adminUnitsExcept.Contains(ps.EmpresaId)));

            var validator = new UserValidatorSalaryMarkProjectRule(_validator);

            if (!validator.IsSatisfiedBy(companyByProjectSalary)) return null;

            var expUnits = userPermission.Contents.Where(s => s.Id == (int)ContentsSubItemsEnum.Units)?
            .SelectMany(se => se.SubItems);

            var unitsResult = new List<SubFieldCheckedUser>();

            unitsResult = companyByProjectSalary
                .Select(se => new SubFieldCheckedUser
                {
                    Id = se.EmpresaId,
                    Name = string.IsNullOrWhiteSpace(se.Empresa.NomeFantasia) ?
                    (string.IsNullOrWhiteSpace(se.Empresa.RazaoSocial) ? string.Empty : se.Empresa.RazaoSocial) :
                    se.Empresa.NomeFantasia,
                    IsChecked = expUnits.Any() ?
                    !expUnits.Contains(se.EmpresaId) : true,
                }).ToList();

            contents.Add(new FieldCheckedUser
            {
                Name = ContentsSubItemsEnum.Units.GetDescription(),
                Id = (int)ContentsSubItemsEnum.Units,
                SubItems = unitsResult.Count() > 0 ? unitsResult : new List<SubFieldCheckedUser>()
            });

            var adminGroupsExcept = adminPermission.Contents.Where(x => x.Id == (long)ContentsSubItens.Group).SelectMany(x => x.SubItems);
            //groups 
            var groupsResult = await _unitOfWork.GetRepository<GruposProjetosSalaryMark, long>()
                .GetListAsync(x => x.Where(x => x.ProjetoId == projectId &&
                                                 x.Ativo.HasValue && x.Ativo.Value &&
                                                 !adminGroupsExcept.Contains(x.GruposProjetosSmidLocal)));

            var groupValidator = new UserValidatorGroupsRule(_validator);

            if (!groupValidator.IsSatisfiedBy(groupsResult)) return null;

            var expGroups = userPermission.Contents.Where(s => s.Id == (int)ContentsSubItemsEnum.Group)?
                .SelectMany(se => se.SubItems);

            contents.Add(new FieldCheckedUser
            {
                Name = ContentsSubItemsEnum.Group.GetDescription(),
                Id = (int)ContentsSubItemsEnum.Group,
                SubItems = groupsResult.Select(s => new SubFieldCheckedUser
                {
                    Id = s.Id,
                    Name = s.GrupoSm,
                    IsChecked = expGroups.Any() ?
                    !expGroups.Contains(s.Id) : true,
                }).ToList()
            });

            // salary scale
            var adminTablessExcept = adminPermission.Contents.Where(x => x.Id == (long)ContentsSubItens.SalaryResult).SelectMany(x => x.SubItems);
            var expSalaryResult = userPermission.Contents.Where(s => s.Id == (int)ContentsSubItemsEnum.SalaryTable)?
                            .SelectMany(se => se.SubItems);

            var salaryResult = await _unitOfWork.GetRepository<TabelasSalariais, long>()
                                    .GetListAsync(x => x.Where(x => x.ProjetoId == projectId &&
                                                                    x.Ativo.HasValue && x.Ativo.Value &&
                                                                    !adminTablessExcept.Contains(x.TabelaSalarialIdLocal)));

            var salaryTableValidator = new UserValidatorSalaryTableRule(_validator);
            if (!salaryTableValidator.IsSatisfiedBy(salaryResult)) return null;

            contents.Add(new FieldCheckedUser
            {
                Name = ContentsSubItemsEnum.SalaryTable.GetDescription(),
                Id = (int)ContentsSubItemsEnum.SalaryTable,
                SubItems = salaryResult.Select(s => new SubFieldCheckedUser
                {
                    Id = s.Id,
                    Name = s.TabelaSalarial,
                    IsChecked = expSalaryResult.Any() ?
                                !expSalaryResult.Contains(s.Id) : true,
                }).ToList()
            });

            //positions 
            var expPositions = userPermission.Contents.Where(s => s.Id == (int)ContentsSubItemsEnum.Positions)?
                .SelectMany(se => se.SubItems);

            contents.Add(new FieldCheckedUser
            {
                Name = ContentsSubItemsEnum.Positions.GetDescription(),
                Id = (int)ContentsSubItemsEnum.Positions,
                SubItems = new List<SubFieldCheckedUser>
                {
                    new SubFieldCheckedUser
                    {
                        Id = 0,
                        Name = "Somente cargos com ocupantes",
                        IsChecked = expPositions.Any() ? !expPositions.Contains(0) : true,
                    }
                }
            });

            var pemissionCompany = await _unitOfWork.GetRepository<EmpresaPermissaoSm, long>()
                                        .GetAsync(x => x.Where(g => g.EmpresaId == companyId));

            #region SCENARIOS
            if (adminPermission.Display != null &&
                adminPermission.Display.Scenario.Safe().Any(sc => sc.IsChecked))
            {
                var adminScenarios = adminPermission.Display.Scenario.Where(sc => sc.IsChecked).Select(res => res.Id);
                var scenarios = new FieldCheckedUser
                {
                    Id = (long)ParameterDisplay.Scenario,
                    Name = ParameterDisplay.Scenario.GetDescription(),
                    SubItems = userPermission.Display != null && userPermission.Display.Scenario.Safe().Any() ?
                               userPermission.Display.Scenario.Where(sc => adminScenarios.Contains(sc.Id))
                                                        .Select(res => new SubFieldCheckedUser
                                                        {
                                                            Id = res.Id,
                                                            Name = res.Name,
                                                            IsChecked = res.IsChecked
                                                        }) : new List<SubFieldCheckedUser>()
                };
                contents.Add(scenarios);
            }
            #endregion

            #region PERSONS
            var personDisplays = new List<SubFieldCheckedUser>();
            if (!adminPermission.Permission.Any(p => p.Id == (long)PermissionItensEnum.InactivePerson &&
                                                 !p.IsChecked))
                personDisplays.Add(new SubFieldCheckedUser
                {
                    Id = (long)PermissionItensEnum.InactivePerson,
                    Name = PermissionItensEnum.InactivePerson.GetDescription(),
                    IsChecked = !userPermission.Permission.Safe().Any(p => p.Id == (long)PermissionItensEnum.InactivePerson &&
                                                 !p.IsChecked)
                });
            if (!adminPermission.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions))
                personDisplays.Add(new SubFieldCheckedUser
                {
                    Id = (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions,
                    Name = ContentsSubItemsEnum.ShowPeopleWithoutPositions.GetDescription(),
                    IsChecked = !userPermission.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.ShowPeopleWithoutPositions)
                });
            if (personDisplays.Any())
            {
                var persons = new FieldCheckedUser
                {
                    Id = (int)ParameterDisplay.Person,
                    Name = ParameterDisplay.Person.GetDescription(),
                    SubItems = personDisplays
                };
                contents.Add(persons);
            }
            #endregion

            #region ANALYSIS
            if (!adminPermission.Permission.Any(c => c.Id == (long)PermissionItensEnum.MoveNextStep))
            {
                var analysisDisplay = new FieldCheckedUser
                {
                    Id = (int)ParameterDisplay.Analysis,
                    Name = ParameterDisplay.Analysis.GetDescription(),
                    SubItems = new List<SubFieldCheckedUser>
                    {
                        new SubFieldCheckedUser
                        {
                            Id = (long)PermissionItensEnum.MoveNextStep,
                            Name = PermissionItensEnum.MoveNextStep.GetDescription(),
                            IsChecked = !userPermission.Permission.Safe().Any(c => c.Id == (long)PermissionItensEnum.MoveNextStep)
                        }
                    }
                };
                contents.Add(analysisDisplay);
            }
            #endregion

            #region TECNICAL ADJUST
            if (!adminPermission.Contents.Any(c => c.Id == (long)ContentsSubItemsEnum.ShowTecnicalAdjust))
            {
                var tecnicalAdjustDisplay = new FieldCheckedUser
                {
                    Id = (int)ParameterDisplay.TecnicalAdjust,
                    Name = ParameterDisplay.TecnicalAdjust.GetDescription(),
                    SubItems = new List<SubFieldCheckedUser>{
                        new SubFieldCheckedUser{
                            Id = (long)ContentsSubItemsEnum.ShowTecnicalAdjust,
                            Name = ContentsSubItemsEnum.ShowTecnicalAdjust.GetDescription(),
                            IsChecked = !userPermission.Contents.Safe().Any(c => c.Id == (long)ContentsSubItemsEnum.ShowTecnicalAdjust)
                        }
                    }
                };
                contents.Add(tecnicalAdjustDisplay);
            }
            #endregion

            contents.ForEach(f =>
            {
                f.IsChecked = f.SubItems.Any() ?
                !f.SubItems.All(a => !a.IsChecked) : true;
            });
            return contents;
        }

        private List<FieldCheckedUser> PrepareData(PermissionJson adminPermission, List<FieldCheckedUser> data)
        {
            var dataAux = new List<FieldCheckedUser>();
            data.ForEach(dataItem =>
            {
                var resultModule = adminPermission.Modules.FirstOrDefault(f => f.Id == dataItem.Id);
                if (resultModule == null)
                {
                    dataAux.Add(dataItem);
                    return;
                }

                if (!resultModule.SubItems.Safe().Any()) return;

                if (resultModule.SubItems.SequenceEqual(dataItem.SubItems.Select(x => x.Id))) return;

                var dtAux = dataItem;

                dtAux.SubItems = dataItem.SubItems.Where(ms => !resultModule.SubItems.Any(sb => sb == ms.Id));
                dataAux.Add(dtAux);

                dataItem.IsChecked = dataItem.SubItems.Any() ?
                                    !dataItem.SubItems.All(a => !a.IsChecked) : true;
            });
            return dataAux;
        }
    }
}
