using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Positioning.Command;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Positioning
{
    [TestClass]
    public class UpdateDisplayColumnsFrameworkTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;

        public UpdateDisplayColumnsFrameworkTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
        }

        [TestMethod]
        public async Task IsSaveOk()
        {
            
            var updateDisplayColumnsFrameworkHandler =
                new UpdateDisplayColumnsFrameworkHandler(unitOfWork,
                permissionUserInteractor);

            var request = new UpdateDisplayColumnsFrameworkRequest
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
            };

            await updateDisplayColumnsFrameworkHandler.Handle(request, CancellationToken.None);


                var listResult = await unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
                                .GetListAsync(x => x.Where(g => g.UsuarioId == userData.UserId &&
                                g.ModuloSmid == (long)ModulesEnum.Positioning &&
                                g.ModuloSmsubItemId.HasValue &&
                                g.ModuloSmsubItemId == (long)ModulesSuItemsEnum.Framework &&
                                request.DisplayColumns.Select(s=> s.ColumnId).Contains(g.ColunaId)));

            Assert.AreEqual(listResult.Count(), request.DisplayColumns.Count());

        }
    }
}
