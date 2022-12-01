
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.TableSalary.Command;
using SM.Application.TableSalary.Validators;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.TableSalary
{
    [TestClass]
    public class UpdateSalarialTableTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public UpdateSalarialTableTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task IsSaveOk()
        {

            var permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            var validator = new UpdateSalarialTablePemissionValidators(permissionUserInteractor);
            var validatorResponse = new ValidatorResponse();

            var updateSalaryTableValues =
                new UpdateSalaryTableHandler(unitOfWork,
                validator, permissionUserInteractor, validatorResponse);

            var request = new UpdateSalaryTableRequest
            {
                UserId = 288, //flavio pavan
                ProjectId = 5,
                TableId = 1,
                SalaryTable = new AuxSalaryTable
                {
                    GsmFinal = 54,
                    GsmInitial = 2,
                    Justify = "Salvando dados pelo Teste Integrado",
                    Multiply = 2.02,
                    SalaryTableName = "Tabela Salarial 1 Teste Integrado",
                    TypeMultiply = 1,
                    SalaryTableValues = new List<AuxSalaryTableValues>
                    {
                        new AuxSalaryTableValues
                        {
                            Gsm = 1,
                            Mid = 1.851,
                            Minor1 = 1.670,
                            Minor2 = 1.481,
                            Minor3 = 1.281,
                            Plus1 = 2.036,
                            Plus2 = 2.221
                        }
                    }
                }
            };

            var compare = await updateSalaryTableValues.Handle(request, CancellationToken.None);

            Assert.IsNotNull(compare);
        }

        [TestMethod]
        public async Task IsSaveWrong()
        {

            var permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            var validator = new UpdateSalarialTablePemissionValidators(permissionUserInteractor);
            var validatorResponse = new ValidatorResponse();

            var updateSalaryTableValues =
                new UpdateSalaryTableHandler(unitOfWork,
                validator, permissionUserInteractor, validatorResponse);

            var request = new UpdateSalaryTableRequest
            {
                UserId = 0, //flavio pavan
                ProjectId = 0,
                TableId = 0,
                SalaryTable = null,
            };
            
            await updateSalaryTableValues.Handle(request, CancellationToken.None);
            Assert.IsTrue(validatorResponse.Notifications.Count > 0);
        }
    }
}
