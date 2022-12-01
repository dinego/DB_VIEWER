using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Position.Command;
using SM.Application.Position.Validators;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Position
{
    [TestClass]
    public class UpdateDisplayColumnsListTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly IEnumerable<DisplayColumnsList> listListPositionColumns;

        public UpdateDisplayColumnsListTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();

            listListPositionColumns = (Enum.GetValues(typeof(PositionProjectColumnsEnum)) as
                                IEnumerable<PositionProjectColumnsEnum>)
                                .Select(s => new DisplayColumnsList
                                {
                                    ColumnId = (int)s,
                                    IsChecked = true,
                                    Name = s.GetDescription()
                                });
        }

        [TestMethod]
        public async Task IsSaveOk()
        {
            var permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            var validator = new UpdateDisplayColumnsListPemissionValidators(permissionUserInteractor);
            var validatorResponse = new ValidatorResponse();
            var updateDisplayColumnsListHandler =
                new UpdateDisplayColumnsListHandler(unitOfWork,
                validator, permissionUserInteractor, validatorResponse);



            var request = new UpdateDisplayColumnsListRequest
            {
                UserId = userData.UserId,
                DisplayColumns = listListPositionColumns
            };

            await updateDisplayColumnsListHandler.Handle(request, CancellationToken.None);


            var listResult = await unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
                            .GetListAsync(x => x.Where(g => g.UsuarioId == userData.UserId &&
                            g.ModuloSmid == (long)ModulesEnum.Position &&
                            g.ModuloSmsubItemId.HasValue &&
                            g.ModuloSmsubItemId == (long)ModulesSuItemsEnum.Architecture &&
                            request.DisplayColumns.Select(s => s.ColumnId).Contains(g.ColunaId)));

            Assert.AreEqual(listResult.Count(), request.DisplayColumns.Count());

        }
    }
}
