using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.TableSalary.Command;
using SM.Application.TableSalary.Validators;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.TableSalary
{
    [TestClass]
    public class UpdateDisplayColumnsTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public UpdateDisplayColumnsTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        // [TestMethod]
        // public async Task IsSaveOk()
        // {

        //     var permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
        //     var validator = new UpdateDisplayColumnsPemissionValidators(permissionUserInteractor);
        //     var validatorResponse = new ValidatorResponse();

        //     var updateDisplayColumnsHandler =
        //         new UpdateDisplayColumnsHandler(unitOfWork,
        //         validator, permissionUserInteractor, validatorResponse);

        //     var request = new UpdateDisplayColumnsRequest
        //     {
        //         UserId =  userData.UserId ,
        //         DisplayColumns = new List<DisplayColumns>
        //         {
        //             new DisplayColumns {
        //                 ColumnId = (int)TableSalaryColumnEnum.GSM,
        //                 IsChecked = true,
        //                 Name = "Grade"
        //             },
        //             new DisplayColumns {
        //                 ColumnId = (int)TableSalaryColumnEnum.TableSalaryName,
        //                 IsChecked = true,
        //                 Name = "Nome tabela Salarial"
        //             }
        //         }
        //     };

        //     await updateDisplayColumnsHandler.Handle(request, CancellationToken.None);

        //     var listResult = await unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
        //                         .GetListAsync(x => x.Where(g => g.UsuarioId == userData.UserId &&
        //                         g.ModuloSmid == (long)ModulesEnum.TableSalary &&
        //                         request.DisplayColumns.Select(s=> s.ColumnId).Contains(g.ColunaId)));

        //     Assert.AreEqual(listResult.Count(), request.DisplayColumns.Count());

        // }
    }
}
