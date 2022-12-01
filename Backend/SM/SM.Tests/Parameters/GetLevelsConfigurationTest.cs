using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SM.Application.Parameters.Queries;
using SM.Domain.Enum;
using SM.Domain.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Parameters
{
    [TestClass]
    public class GetLevelsConfigurationTest
    {
        private readonly ValidatorResponse validator;
        private readonly UserTestDTO userData;
        private readonly IUnitOfWork unitOfWork;
        private readonly LevelsConfiguration _levelsConfiguration;

        public GetLevelsConfigurationTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); userData = UnitOfWorkTest.RetrieveUserId();
            validator = new ValidatorResponse();
            userData = UnitOfWorkTest.RetrieveUserId();

            var levelsConfiguration = new LevelsConfiguration
            {
                Strategic = new List<LevelStructureConfiguration>
                {
                    new LevelStructureConfiguration {
                        Level = 11,
                        Code = "13a",
                        ColumnLevelType = ColumnLevelType.Leadership
                    },
                    new LevelStructureConfiguration {
                        Level = 12,
                        Code = "12a",
                        ColumnLevelType = ColumnLevelType.Leadership
                    },
                    new LevelStructureConfiguration {
                        Level = 13,
                        Code = "11a",
                        ColumnLevelType = ColumnLevelType.Leadership
                    },
                    new LevelStructureConfiguration {
                        Level = 14,
                        Code = "10a",
                        ColumnLevelType = ColumnLevelType.Leadership
                    }
                },
                Tatic = new List<LevelStructureConfiguration>
                {
                    new LevelStructureConfiguration {
                        Level = 16,
                        Code = "09a",
                        ColumnLevelType = ColumnLevelType.Leadership
                    },
                    new LevelStructureConfiguration {
                        Level = 17,
                        Code = "08a",
                        ColumnLevelType = ColumnLevelType.Leadership
                    },
                    new LevelStructureConfiguration {
                        Level = 19,
                        Code = "07a",
                        ColumnLevelType = ColumnLevelType.Leadership
                    },
                    new LevelStructureConfiguration {
                        Level = 18,
                        Code = "10a",
                        ColumnLevelType = ColumnLevelType.IndividualContributors
                    }
                },
                Operational = new List<LevelStructureConfiguration>
                {
                    new LevelStructureConfiguration {
                        Level = 20,
                        Code = "06b",
                        ColumnLevelType = ColumnLevelType.IndividualContributors
                    },
                    new LevelStructureConfiguration {
                        Level = 21,
                        Code = "08a",
                        ColumnLevelType = ColumnLevelType.IndividualContributors
                    },
                    new LevelStructureConfiguration {
                        Level = 22,
                        Code = "05b",
                        ColumnLevelType = ColumnLevelType.Leadership
                    },
                    new LevelStructureConfiguration {
                        Level = 23,
                        Code = "04b",
                        ColumnLevelType = ColumnLevelType.IndividualContributors
                    },
                    new LevelStructureConfiguration {
                        Level = 24,
                        Code = "03b",
                        ColumnLevelType = ColumnLevelType.IndividualContributors
                    },
                    new LevelStructureConfiguration {
                        Level = 25,
                        Code = "02b",
                        ColumnLevelType = ColumnLevelType.IndividualContributors
                    },
                    new LevelStructureConfiguration {
                        Level = 26,
                        Code = "01b",
                        ColumnLevelType = ColumnLevelType.IndividualContributors
                    }
                },
            };
            _levelsConfiguration = levelsConfiguration;
        }

        [TestMethod]
        public async Task IsGetOk()
        {

            var getLevelsConfigurationHandler = new GetLevelsConfigurationHandler(unitOfWork,validator, _levelsConfiguration);

            if (userData == null) return;
            var request = new GetLevelsConfigurationRequest
            {
                CompanyId = userData.CompanyId,
                UserId =  userData.UserId ,
                IsAdmin = true                
            };

            var result = await getLevelsConfigurationHandler.Handle(request, CancellationToken.None);
            var teste = JsonConvert.SerializeObject(result);
            Assert.AreNotSame(new GetLevelsConfigurationRequest(), result);

        }
    }
}
