using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Share.Queries;
using SM.Domain.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Share
{
    [TestClass]
    public class GetShareTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly ValidatorResponse validatorResponse;
        private readonly PermissionUserInteractor _permissionUserInteractor;

        public GetShareTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
            validatorResponse = new ValidatorResponse();
            _permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
        }

        [TestMethod]
        public async Task GetDataByKeyIsOk()
        {
            var shareData =
                 await unitOfWork.GetRepository<CompartilharSm, long>().GetAsync(x => x);

            if (shareData != null)
            {
                var shareConfiguration = new ShareConfiguration
                {
                    DaysExpired = 10
                };

                var getShareHandler =
                    new GetShareHandler(unitOfWork,
                    _permissionUserInteractor, validatorResponse);


                var result = await getShareHandler.Handle(
                    new GetShareRequest
                    {
                        SecretKey = shareData.ChaveSecreta
                    }, CancellationToken.None);

                var columns = result.ColumnsExcluded
                    .Select(s => Convert.ToInt32(s));

                Assert.IsNotNull(columns);
            }
        }
    }
}
