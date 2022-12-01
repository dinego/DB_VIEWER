using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Positioning.Queries;
using SM.Domain.Enum.Positioning;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Positioning
{
    [TestClass]
    public class GetFullInfoProposedMovementsTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly GetLabelDisplayByInteractor getLabelDisplayByInteractor;
        private readonly UserTestDTO userData;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;

        public GetFullInfoProposedMovementsTest()
        {
            
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            getLabelDisplayByInteractor  = new GetLabelDisplayByInteractor(unitOfWork);
            userData = UnitOfWorkTest.RetrieveUserId();
            getGlobalLabelsInteractor = new GetGlobalLabelsInteractor(unitOfWork);
        }

        [TestMethod]

        public async Task IsOK()
        {

            var getFullInfoProposedMovementsHandler =
                        new GetFullInfoProposedMovementsHandler(unitOfWork,
                        permissionUserInteractor,
                        getLabelDisplayByInteractor,
                        getGlobalLabelsInteractor);

            var request = new GetFullInfoProposedMovementsRequest
            {
                UserId = userData.UserId,
                ProjectId = userData.ProjectId,
                CategoryId = 1,
                CompaniesId = userData.Companies,
                Scenario = DisplayMMMIEnum.MI,
                DisplayBy = DisplayByPositioningEnum.ProfileId,
                IsAsc = true,
                SortColumnId = 1000
            };

            var result = await getFullInfoProposedMovementsHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFullInfoProposedMovementsRequest(), result);

        }
    }
}
