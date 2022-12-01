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
    public class GetFullInfoFinancialImpactTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly UserTestDTO userData;
        private readonly GetLabelDisplayByInteractor getLabelDisplayByInteractor;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;

        public GetFullInfoFinancialImpactTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            userData = UnitOfWorkTest.RetrieveUserId();
            getLabelDisplayByInteractor = new GetLabelDisplayByInteractor(unitOfWork);
            getGlobalLabelsInteractor = new GetGlobalLabelsInteractor(unitOfWork);
        }

        [TestMethod]

        public async Task IsOK()
        {

            var getFullInfoPositioningFinancialImpactHandler =
                        new GetFullInfoFinancialImpactHandler(unitOfWork,
                        permissionUserInteractor,
                        getLabelDisplayByInteractor,
                        getGlobalLabelsInteractor);

            var request = new GetFullInfoFinancialImpactRequest
            {
                UserId = userData.UserId,
                ProjectId = userData.ProjectId,
                CategoryId = 1,
                CompaniesId = userData.Companies,
                Scenario = DisplayMMMIEnum.MI,
                DisplayBy = DisplayByPositioningEnum.ProfileId,
                SerieId = AnalyseFinancialImpactEnum.IFAMax,
                IsAsc = false,
                SortColumnId = 2
            };

            var result = await getFullInfoPositioningFinancialImpactHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFullInfoFinancialImpactRequest(), result);

        }

    }
}
