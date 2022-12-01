using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
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
    public class UpdateDisplayColumnsMapTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public UpdateDisplayColumnsMapTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task IsSaveOk()
        {
            await CleanData();
            var permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            var validator = new UpdateDisplayColumnsMapPemissionValidators(permissionUserInteractor);
            var validatorResponse = new ValidatorResponse();

            var updateDisplayColumnsMapHandler =
                new UpdateDisplayColumnsMapHandler(unitOfWork,
                validator, validatorResponse);

            var request = new UpdateDisplayColumnsMapRequest
            {
                UserId =  userData.UserId ,
                DisplayColumns = new List<DisplayColumnsMap>
                {
                    new DisplayColumnsMap
                    {
                        IsChecked = false,
                        Name = "Recursos Humanos"
                    }

                }
            };

            await updateDisplayColumnsMapHandler.Handle(request, CancellationToken.None);


            var listResult = await unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
                            .GetListAsync(x => x.Where(g => g.UsuarioId == 1095 &&
                            g.ModuloSmid == (long)ModulesEnum.Position &&
                            g.ModuloSmsubItemId.HasValue &&
                            g.ModuloSmsubItemId == (long)ModulesSuItemsEnum.Map &&
                            request.DisplayColumns.Select(s => s.Name).Contains(g.Nome)));

            Assert.AreEqual(listResult.Count(), request.DisplayColumns.Count());

        }

        private async Task CleanData()
        {
            //clean data 

            var listClean = await unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
                .GetListAsync(x => x.Where(g => g.UsuarioId == 1095 &&
                 g.ModuloSmid == (long)ModulesEnum.Position &&
                 g.ModuloSmsubItemId.HasValue &&
                 g.ModuloSmsubItemId == (long)ModulesSuItemsEnum.Map));

            unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
                .Delete(listClean);
            await unitOfWork.CommitAsync();
        }
    }
}
