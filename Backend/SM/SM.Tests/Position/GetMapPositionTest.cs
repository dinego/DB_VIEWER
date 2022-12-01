using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Position.Queries;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Position
{
    [TestClass]
    public class GetMapPositionTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;

        public GetMapPositionTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            getGlobalLabelsInteractor = new GetGlobalLabelsInteractor(unitOfWork);
        }

        [TestMethod]
        public async Task GetMapPosition()
        {

            var getMapPositionHandler =
                new GetMapPositionHandler(unitOfWork, 
                permissionUserInteractor, 
                getGlobalLabelsInteractor);

            var request = new GetMapPositionRequest
            {
                CompaniesId = userData.Companies,
                UserId =  userData.UserId ,
                DisplayBy = DisplayByMapPositionEnum.AxisCarreira,
                ProjectId = userData.ProjectId,
                RemoveRowsEmpty = true
            };

            var result = await getMapPositionHandler.Handle(request, CancellationToken.None);

            //todo fix test
            Assert.AreNotSame(new GetMapPositionResponse(), result);

        }

        [TestMethod]
        public async Task GetMapPositionWithOccupants()
        {

            var getMapPositionHandler =
                new GetMapPositionHandler(unitOfWork,
                permissionUserInteractor, 
                getGlobalLabelsInteractor);

            var request = new GetMapPositionRequest
            {
                CompaniesId = userData.Companies,
                UserId =  userData.UserId ,
                ShowJustWithOccupants = true,
                DisplayBy = DisplayByMapPositionEnum.AxisCarreira,
                ProjectId = 1
            };

            var result = await getMapPositionHandler.Handle(request, CancellationToken.None);

            //todo fix test
            Assert.AreNotSame(new GetMapPositionResponse(), result);

        }

        [TestMethod]
        public async Task GetMapPositionRemoveRowsEmpty()
        {

            var getMapPositionHandler =
                new GetMapPositionHandler(unitOfWork, 
                permissionUserInteractor,
                getGlobalLabelsInteractor);

            var request = new GetMapPositionRequest
            {
                CompaniesId = userData.Companies,
                UserId =  userData.UserId ,
                RemoveRowsEmpty = false,
                DisplayBy = DisplayByMapPositionEnum.AxisCarreira,
                ProjectId = 1
            };

            var result = await getMapPositionHandler.Handle(request, CancellationToken.None);

            //todo fix test
            Assert.AreNotSame(new GetMapPositionResponse(), result);

        }

        [TestMethod]
        public async Task GetMapPositionByTerm()
        {

            var getMapPositionHandler =
                new GetMapPositionHandler(unitOfWork, 
                permissionUserInteractor,
                getGlobalLabelsInteractor);

            var request = new GetMapPositionRequest
            {
                CompaniesId = userData.Companies,
                UserId =  userData.UserId ,
                Term = "Porteiro",
                DisplayBy = DisplayByMapPositionEnum.AxisCarreira,
                ProjectId = 1
            };

            var result = await getMapPositionHandler.Handle(request, CancellationToken.None);

            //todo fix test
            Assert.AreNotSame(new GetMapPositionResponse(), result);

        }

        [TestMethod]
        public async Task GetMapPositionByGroupId()
        {

            var getMapPositionHandler =
                new GetMapPositionHandler(unitOfWork, 
                permissionUserInteractor,
                getGlobalLabelsInteractor);

            var request = new GetMapPositionRequest
            {
                CompaniesId = userData.Companies,
                UserId =  userData.UserId ,
                GroupId = 3,
                DisplayBy = DisplayByMapPositionEnum.AxisCarreira,
                ProjectId = 1
            };

            var result = await getMapPositionHandler.Handle(request, CancellationToken.None);

            //todo fix test
            Assert.AreNotSame(new GetMapPositionResponse(), result);

        }

        [TestMethod]
        public async Task GetMapPositionByTableId()
        {

            var getMapPositionHandler =
                new GetMapPositionHandler(unitOfWork,
                permissionUserInteractor,
                getGlobalLabelsInteractor);

            var request = new GetMapPositionRequest
            {
                CompaniesId = userData.Companies,
                UserId =  userData.UserId ,
                TableId = 3,
                DisplayBy = DisplayByMapPositionEnum.AxisCarreira,
                ProjectId = 1
            };

            var result = await getMapPositionHandler.Handle(request, CancellationToken.None);

            //todo fix test
            Assert.AreNotSame(new GetMapPositionResponse(), result);

        }

        [TestMethod]
        public async Task GetMapPositionByAreasAndLevels()
        {

            var getMapPositionHandler =
                new GetMapPositionHandler(unitOfWork,
                permissionUserInteractor,
                getGlobalLabelsInteractor);

            var request = new GetMapPositionRequest
            {
                CompaniesId = userData.Companies,
                UserId =  userData.UserId ,
                DisplayBy = DisplayByMapPositionEnum.AxisCarreira,
                ProjectId = 1
            };

            var result = await getMapPositionHandler.Handle(request, CancellationToken.None);

            //todo fix test
            Assert.AreNotSame(new GetMapPositionResponse(), result);

        }

        [TestMethod]
        public async Task GetMapPositionByUnitId()
        {

            var getMapPositionHandler =
                new GetMapPositionHandler(unitOfWork, 
                permissionUserInteractor,
                getGlobalLabelsInteractor);

            var request = new GetMapPositionRequest
            {
                CompaniesId = userData.Companies,
                UserId =  userData.UserId ,
                UnitId =  userData.CompanyId ,
                DisplayBy = DisplayByMapPositionEnum.AxisCarreira,
                ProjectId = 1
            };

            var result = await getMapPositionHandler.Handle(request, CancellationToken.None);

            //todo fix test
            Assert.AreNotSame(new GetMapPositionResponse(), result);

        }

        [TestMethod]
        public async Task GetMapPositionByColumnsShare()
        {

            var getMapPositionHandler =
                new GetMapPositionHandler(unitOfWork,
                permissionUserInteractor,
                getGlobalLabelsInteractor);

            var request = new GetMapPositionRequest
            {
                CompaniesId = userData.Companies,
                UserId =  userData.UserId ,
                Columns = new List<string> { "Recursos Humanos", "Administração Geral" },
                DisplayBy = DisplayByMapPositionEnum.AxisCarreira,
                ProjectId = 1
            };

            var result = await getMapPositionHandler.Handle(request, CancellationToken.None);

            //todo fix test
            Assert.AreNotSame(new GetMapPositionResponse(), result);

        }

        [TestMethod]
        public async Task GetMapPositionByColumnsDisplay()
        {

            var getMapPositionHandler =
                new GetMapPositionHandler(unitOfWork, 
                permissionUserInteractor,
                getGlobalLabelsInteractor);

            var request = new GetMapPositionRequest
            {
                CompaniesId = userData.Companies,
                UserId =  userData.UserId ,
                DisplayBy = DisplayByMapPositionEnum.AxisCarreira,
                ProjectId = 1
            };

            var result = await getMapPositionHandler.Handle(request, CancellationToken.None);

            //todo fix test
            Assert.AreNotSame(new GetMapPositionResponse(), result);

        }
    }
}
