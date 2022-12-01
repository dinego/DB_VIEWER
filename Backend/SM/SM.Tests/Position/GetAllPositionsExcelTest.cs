using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Position.Queries;
using SM.Domain.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Position
{
    [TestClass]
    public class GetAllPositionsExcelTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly MultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor;
        private readonly MultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly GenerateExcelFileInteractor generateExcelFileInteractor;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;

        public GetAllPositionsExcelTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
            multiplierFactorHourlyInteractor = new MultiplierFactorHourlyInteractor(unitOfWork);
            var validatorResponse = new ValidatorResponse();
            multiplierFactorTypeContratcInteractor = new MultiplierFactorTypeContratcInteractor(unitOfWork, validatorResponse);
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            generateExcelFileInteractor = new GenerateExcelFileInteractor(new ColorScheme
            {
                Colors = new List<ColorData>
                           {
                               new ColorData
                               {
                                   Color = "#fafafa",
                                   Min = 50,
                                   Max = 90
                               }
                           }
            });
            getGlobalLabelsInteractor = new GetGlobalLabelsInteractor(unitOfWork);
        }
        [TestMethod]
        public async Task IsGetOk()
        {

            var getAllPositionsExcelHandler =
                new GetAllPositionsExcelHandler(unitOfWork,
                permissionUserInteractor,
                generateExcelFileInteractor,
                new InfoApp
                {
                    Name = "SalaryMark"
                },
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor,
                getGlobalLabelsInteractor);


            var result = await getAllPositionsExcelHandler.Handle(
                                  new GetAllPositionsExcelRequest
                                  {
                                      ProjectId = userData.ProjectId,
                                      TableId = 1,
                                      UserId = userData.UserId,
                                      CompaniesId = userData.Companies
                                  }, CancellationToken.None);

            Assert.AreNotSame(new GetAllPositionsExcelRequest(), result);

        }
    }
}
