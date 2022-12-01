using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Parameters.Command;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Parameters
{
    [TestClass]
    public class UpdateLevelsConfigurationTest
    {
        private readonly ValidatorResponse _validator;
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public UpdateLevelsConfigurationTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
            _validator = new ValidatorResponse();
        }

        [TestMethod]
        public async Task IsGetOk()
        {
            if (userData == null) return;

            var updateLevelsConfigurationHandler = new UpdateLevelsConfigurationHandler(unitOfWork, _validator);

            var request = new UpdateLevelsConfigurationRequest
            {
                Levels = new List<UpdateLevelsConfiguration>()
                {
                    new UpdateLevelsConfiguration
                    {
                        LevelId = 14,
                        Enabled= true,
                        Level = "Gerente Sr."
                    }
                },
                UserId =  userData.UserId ,
                CompanyId = userData.CompanyId
            };
            _ = await updateLevelsConfigurationHandler.Handle(request, CancellationToken.None);

            Assert.IsFalse(_validator.HasNotifications);
        }
    }
}
