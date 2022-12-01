using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Parameters.Command;

namespace SM.Tests.Parameters
{
    [TestClass]
    public class UpdateSettingsPjTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public UpdateSettingsPjTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task IsOK()
        {

            var updateSettingsPjHandler =
                        new UpdateSettingsPjHandler(unitOfWork);

            var request = new UpdateSettingsPjRequest
            {
                ProjectId =  userData.ProjectId ,
                IsAdmin = true,
                UserId =  userData.UserId ,
                Data = new List<SettingsPjDataRequest>
                {
                    new SettingsPjDataRequest
                    {
                        Percentage = 0.15,
                        PJSettingsId = 1
                    },
                    new SettingsPjDataRequest
                    {
                        Percentage = 0.15,
                        PJSettingsId = 2
                    }
                }
            };

            var result = await updateSettingsPjHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new UpdateSettingsPjRequest(), result);

        }
    }
}
