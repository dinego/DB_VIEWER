using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Positioning.Queries;
using SM.Domain.Enum.Positioning;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Positioning
{
    [TestClass]
    public class GetProposedMovementsTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;

        public GetProposedMovementsTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
        }

        [TestMethod]
        public async Task TestProfile()
        {

            var getProposedMovementsHandler =
                        new GetProposedMovementsHandler(unitOfWork,
                        permissionUserInteractor);

                var request = new GetProposedMovementsRequest
                {
                    UserId =  userData.UserId ,
                    ProjectId =  userData.ProjectId ,
                    CompaniesId = userData.Companies,
                    DisplayBy = DisplayByPositioningEnum.ProfileId,
                    CategoriesExp = new List<object> {1000}
                };

                var result = await getProposedMovementsHandler.Handle(request, CancellationToken.None);

                Assert.AreNotSame(new GetFrameworkRequest(), result);

        }


    }
}
