using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Parameters.Command;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Position
{
    [TestClass]
    public class UpdateGlobalLabelsTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly ValidatorResponse validator;

        public UpdateGlobalLabelsTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
            validator = new ValidatorResponse();
        }

        [TestMethod]
        public async Task IsSaveOk()
        {

            var updateGlobalLabelsHandler =
                new UpdateGlobalLabelsHandler(unitOfWork, validator);

            var request = new UpdateGlobalLabelsRequest
            {
                UserId = userData.UserId,
                ProjectId = userData.ProjectId,
                IsAdmin = true,
                GlobalLabels = new List<GlobalLabelRequest>
                {
                    new GlobalLabelRequest {
                       Id = 1,
                       Alias = "teste",
                       IsChecked = false,
                       Name = "teste default"
                    },
                    new GlobalLabelRequest {
                       Id = 2,
                       Alias = "test2",
                       IsChecked = true,
                       Name = "teste2 default"
                    },
                }
            };

            var result = await updateGlobalLabelsHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new UpdateGlobalLabelsRequest(), result);

        }
    }
}
