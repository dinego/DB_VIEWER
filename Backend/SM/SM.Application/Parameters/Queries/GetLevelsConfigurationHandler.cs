using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using MediatR;
using Newtonsoft.Json;
using SM.Domain.Common;
using SM.Domain.Enum;
using SM.Domain.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Parameters.Queries
{

    public class GetLevelsConfigurationRequest : IRequest<GetLevelsConfigurationResponse>
    {
        public long UserId { get; set; }
        public bool IsAdmin { get; set; }
        public long CompanyId { get; set; }
    }

    public class GetLevelsConfigurationResponse
    {
        public LevelsStructure Strategic { get; set; }
        public LevelsStructure Tatic { get; set; }
        public LevelsStructure Operational { get; set; }
    }

    public class Contributors
    {
        public long Id { get; set; }
        public string Level { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
    }

    public class ContributorStructure
    {
        public List<Contributors> LeadershipContributors { get; set; }
        public List<Contributors> IndividualContributors { get; set; }
    }

    public class LevelsStructure
    {
        public ContributorStructure SalaryMarkStructure { get; set; }
        public ContributorStructure YourCompanyStructure { get; set; }
    }

    public class LevelDTO
    {
        public long LevelId { get; set; }
        public string Name { get; set; }
    }

    public class GetLevelsConfigurationHandler : IRequestHandler<GetLevelsConfigurationRequest, GetLevelsConfigurationResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validator;
        private readonly LevelsConfiguration _levelsConfiguration;

        public GetLevelsConfigurationHandler(IUnitOfWork unitOfWork,
            ValidatorResponse validator,
            LevelsConfiguration levelsConfiguration)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _levelsConfiguration = levelsConfiguration;

        }
        public async Task<GetLevelsConfigurationResponse> Handle(GetLevelsConfigurationRequest request, CancellationToken cancellationToken)
        {
            var companyPermissions = await _unitOfWork.GetRepository<EmpresaPermissaoSm, long>()
                                               .Include("Empresa")
                                               .Include("Empresa.ProjetosSalaryMarkEmpresas")
                                               .GetAsync(x => x.Where(c => c.EmpresaId == request.CompanyId ||
                                                                     (c.Empresa != null && c.Empresa.ProjetosSalaryMarkEmpresas.Select(x => x.EmpresaId).Any(empId => empId == request.CompanyId)))
                                               .Select(res => new
                                               {
                                                   CompanyFather = res.Empresa != null && res.Empresa.CodigoPai.HasValue ? res.Empresa.CodigoPai : res.EmpresaId,
                                                   Permission = res.Permissao
                                               }));

            if (companyPermissions == null) { _validator.AddNotification("Não encontramos dados de permissionamento para a empresa selecionada."); return null; }

            PermissionJson companyExceptions = JsonConvert.DeserializeObject<PermissionJson>(companyPermissions.Permission);

            //levels
            var levels = await _unitOfWork.GetRepository<Niveis, int>()
                .GetListAsync(x => x.Where(l => !companyExceptions.Levels.Contains(l.Id))
                .Select(s => new LevelDTO
                {
                    LevelId = s.Id,
                    Name = s.Nivel
                }));

            if (!levels.Any()) { _validator.AddNotification("Não foi encontrado permissionamento de nível para a empresa selecionada."); return null; }

            //company levels
            var companyLevels = await _unitOfWork.GetRepository<NivelEmpresas, long>()
                .GetListAsync(x =>
                x.Where(ne => ne.EmpresaId == companyPermissions.CompanyFather &&
                             !companyExceptions.Levels.Contains(ne.NivelId))
                .Select(s => new LevelDTO
                {
                    LevelId = s.NivelId,
                    Name = s.NivelEmpresa
                }));

            var response = new GetLevelsConfigurationResponse
            {
                Operational = CreateOperationalStructure(levels, companyLevels),
                Tatic = CreateTaticStructure(levels, companyLevels),
                Strategic = CreateStrategicStructure(levels, companyLevels)
            };

            return response;
        }

        private LevelsStructure CreateStrategicStructure(List<LevelDTO> levels, List<LevelDTO> companyLevels)
        {
            var levelStructure = new LevelsStructure
            {
                SalaryMarkStructure = new ContributorStructure { IndividualContributors = new List<Contributors>(), LeadershipContributors = new List<Contributors>() },
                YourCompanyStructure = new ContributorStructure { IndividualContributors = new List<Contributors>(), LeadershipContributors = new List<Contributors>() }
            };
            _levelsConfiguration.Strategic.ForEach(levelConfig =>
            {
                switch (levelConfig.ColumnLevelType)
                {
                    case ColumnLevelType.Leadership:
                        PrepareLeadershipContributors(levels, companyLevels, levelConfig, levelStructure);
                        break;
                    case ColumnLevelType.IndividualContributors:
                        PrepareIndividualContributors(levels, companyLevels, levelConfig, levelStructure);
                        break;
                }
            });
            return levelStructure;
        }

        private LevelsStructure CreateTaticStructure(List<LevelDTO> levels, List<LevelDTO> companyLevels)
        {
            var levelStructure = new LevelsStructure
            {
                SalaryMarkStructure = new ContributorStructure { IndividualContributors = new List<Contributors>(), LeadershipContributors = new List<Contributors>() },
                YourCompanyStructure = new ContributorStructure { IndividualContributors = new List<Contributors>(), LeadershipContributors = new List<Contributors>() }
            };
            _levelsConfiguration.Tatic.ForEach(levelConfig =>
            {
                var strategicLevel = levels.FirstOrDefault(l => levelConfig.Level == l.LevelId);
                var strategicCompanyLevel = companyLevels.FirstOrDefault(l => levelConfig.Level == l.LevelId);
                switch (levelConfig.ColumnLevelType)
                {
                    case ColumnLevelType.Leadership:
                        PrepareLeadershipContributors(levels, companyLevels, levelConfig, levelStructure);
                        break;
                    case ColumnLevelType.IndividualContributors:
                        PrepareIndividualContributors(levels, companyLevels, levelConfig, levelStructure);
                        break;
                }
            });
            return levelStructure;
        }

        private LevelsStructure CreateOperationalStructure(List<LevelDTO> levels, List<LevelDTO> companyLevels)
        {
            var levelStructure = new LevelsStructure
            {
                SalaryMarkStructure = new ContributorStructure { IndividualContributors = new List<Contributors>(), LeadershipContributors = new List<Contributors>() },
                YourCompanyStructure = new ContributorStructure { IndividualContributors = new List<Contributors>(), LeadershipContributors = new List<Contributors>() }
            };
            _levelsConfiguration.Operational.ForEach(levelConfig =>
            {
                switch (levelConfig.ColumnLevelType)
                {
                    case ColumnLevelType.Leadership:
                        PrepareLeadershipContributors(levels, companyLevels, levelConfig, levelStructure);
                        break;
                    case ColumnLevelType.IndividualContributors:
                        PrepareIndividualContributors(levels, companyLevels, levelConfig, levelStructure);
                        break;
                }
            });
            return levelStructure;
        }

        private void PrepareLeadershipContributors(List<LevelDTO> levels, List<LevelDTO> companyLevels, LevelStructureConfiguration levelConfig, LevelsStructure levelStructure)
        {
            var salaryMarkLevel = levels.FirstOrDefault(l => levelConfig.Level == l.LevelId);
            var companyLevel = companyLevels.FirstOrDefault(l => levelConfig.Level == l.LevelId);
            if (salaryMarkLevel != null)
            {
                var leadershipContributorSM = new Contributors
                {
                    Id = salaryMarkLevel.LevelId,
                    Level = salaryMarkLevel.Name,
                    Code = levelConfig.Code
                };
                levelStructure.SalaryMarkStructure.LeadershipContributors.Add(leadershipContributorSM);

                var leadershipContributorCompany = new Contributors
                {
                    Id = salaryMarkLevel.LevelId,
                    Level = companyLevel != null ? companyLevel.Name : null,
                    Active = companyLevel != null && !string.IsNullOrEmpty(companyLevel.Name)
                };
                levelStructure.YourCompanyStructure.LeadershipContributors.Add(leadershipContributorCompany);
            }
        }

        private void PrepareIndividualContributors(List<LevelDTO> levels, List<LevelDTO> companyLevels, LevelStructureConfiguration levelConfig, LevelsStructure levelStructure)
        {
            var salaryMarkLevel = levels.FirstOrDefault(l => levelConfig.Level == l.LevelId);
            var companyLevel = companyLevels.FirstOrDefault(l => levelConfig.Level == l.LevelId);
            if (salaryMarkLevel != null)
            {
                var individualContributorSM = new Contributors
                {
                    Id = salaryMarkLevel.LevelId,
                    Level = salaryMarkLevel.Name,
                    Code = levelConfig.Code
                };
                levelStructure.SalaryMarkStructure.IndividualContributors.Add(individualContributorSM);

                var individualContributors = new Contributors
                {
                    Id = salaryMarkLevel.LevelId,
                    Level = companyLevel != null ? companyLevel.Name : null,
                    Active = companyLevel != null && !string.IsNullOrEmpty(companyLevel.Name)
                };
                levelStructure.YourCompanyStructure.IndividualContributors.Add(individualContributors);
            }
        }
    }
}
