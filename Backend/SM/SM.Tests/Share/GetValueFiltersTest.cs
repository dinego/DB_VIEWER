using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Positioning.Queries;
using SM.Application.Share.Queries;
using SM.Domain.Enum.Positioning;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Share
{
    [TestClass]
    public class GetValueFiltersTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public GetValueFiltersTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
        }


        [TestMethod]
        public async Task TestMapPosition()
        {
            var getValueFiltersHandler =
                new GetValueFiltersHandler(unitOfWork);

            var request = new GetComparativeAnalysisTableRequest
            {
                Scenario = DisplayMMMIEnum.MM,
                DisplayBy = DisplayByPositioningEnum.Parameter01,
            };

            var result = await getValueFiltersHandler.Handle(new GetValueFiltersRequest
            {
                ProjectId =  userData.ProjectId ,
                Object = request
            }, CancellationToken.None);

            Assert.AreNotSame(new GetValueFiltersRequest(), result);
        }
        

    [TestMethod]
    public async Task TestMapFinancialImpact()
    {
        var getValueFiltersHandler =
            new GetValueFiltersHandler(unitOfWork);

        var request = new GetFinancialImpactRequest
        {
            Scenario = DisplayMMMIEnum.MI,
            UnitId =  userData.CompanyId ,
            DisplayBy = DisplayByPositioningEnum.ProfileId
        };


        var result = await getValueFiltersHandler.Handle(new GetValueFiltersRequest
        {
            ProjectId =  userData.ProjectId ,
            Object = request
        }, CancellationToken.None);

            Assert.AreNotSame(new GetValueFiltersRequest(), result);
        }
}
    }
