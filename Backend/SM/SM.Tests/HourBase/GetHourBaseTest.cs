using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.GetAllHoursBase;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.GetTableSalary
{
    [TestClass]
    public class GetHourBaseTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public GetHourBaseTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task GetHoursBase()
        {
            //Test of insert
            //UPDATE[dbo].[UsuarioPermissaoSM]
            //SET[Permissao] = '{"Levels":[],"TypeOfContract":[],"DataBase":[1],"Modules":[],"SectionDisplay":null,"Permission":[],"Areas":[],"Contents":[{"Id":3,"SubItems":[3,5]},{"Id":2,"SubItems":[3]}],"Scenario":[]}'
            //WHERE id = 44

            var getHoursBaseHandler = new GetAllHourBaseHandler(unitOfWork);
            var result = await getHoursBaseHandler.Handle(
                                                        new GetAllHourBaseRequest
                                                        {
                                                            UserId = 44
                                                        }, CancellationToken.None);
            Assert.AreNotSame(new GetAllHoursBaseResponse(), result);
        }

        [TestMethod]
        public async Task GetHoursBaseUSerHaveJsonButNotDataBaseField()
        {
            var getHoursBaseHandler = new GetAllHourBaseHandler(unitOfWork);
            var result = await getHoursBaseHandler.Handle(
                                                        new GetAllHourBaseRequest
                                                        {
                                                            UserId = 520
                                                        }, CancellationToken.None);
            Assert.AreNotSame(new GetAllHourBaseRequest(), result);
        }

        [TestMethod]
        public async Task GetHoursBaseUSerWithoutJsonRecord()
        {
            var getHoursBaseHandler = new GetAllHourBaseHandler(unitOfWork);
            var result = await getHoursBaseHandler.Handle(
                                                        new GetAllHourBaseRequest
                                                        {
                                                            UserId = 2234324234
                                                        }, CancellationToken.None);
            Assert.AreNotSame(new GetAllHoursBaseResponse(), result);
        }
    }
}