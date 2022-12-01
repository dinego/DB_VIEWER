using CMC.Common.Extensions;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Positioning.Command;
using SM.Application.Positioning.Validators;
using SM.Domain.Enum.Positioning;
using System.Collections.Generic;

namespace SM.Tests.Validators
{
    [TestClass]
    public class UpdateDisplayColumnsFrameworkTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly UserTestDTO userData;

        public UpdateDisplayColumnsFrameworkTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public void CheckValidateIsOk()
        {
            
            var validator = new UpdateDisplayColumnsFrameworkRequestValidators(permissionUserInteractor);

            var result = validator.Validate(new UpdateDisplayColumnsFrameworkRequest
            {
                UserId =  userData.UserId ,
                DisplayColumns = new List<DisplayColumnsFramework>
                {
                    new DisplayColumnsFramework {
                        ColumnId = (int)FrameworkColumnsMainEnum.Business,
                        IsChecked = true,
                        Name = FrameworkColumnsMainEnum.Business.GetDescription()
                    },
                    new DisplayColumnsFramework {
                        ColumnId = (int)FrameworkColumnsMainEnum.Company,
                        IsChecked = true,
                        Name = FrameworkColumnsMainEnum.Company.GetDescription()
                    }
                }
            });


            Assert.AreEqual(result.IsValid,true);
        }

    }
}
