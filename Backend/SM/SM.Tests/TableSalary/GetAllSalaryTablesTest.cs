using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using SM.Application.GetTableSalary;

namespace SM.Tests.GetTableSalary
{
    [TestClass]
    public class GetAllSalaryTablesTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public GetAllSalaryTablesTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task GetTableSalary()
        {
            //Test of insert
            //INSERT INTO[dbo].[UsuarioPermissaoSM]
            //([UsuarioId]
            //          ,[EmpresaPermissaoId]
            //          ,[PerfilPermissaoId]
            //          ,[Permissao]
            //          ,[Exibicao])
            //VALUES
            //(44
            //, 10
            //, 15
            //, '{"Levels":[],"TypeOfContract":[],"DataBase":[],"Modules":[],"SectionDisplay":null,"Permission":[],"Areas":[],"Contents":[{"Id":3,"SubItems":[3,5]}],"Scenario":[]}'
            //, null)
            var getTableSalaryHandler = new GetAllSalaryTablesHandler(unitOfWork);
            var result = await getTableSalaryHandler.Handle(
                                                        new GetAllSalaryTablesRequest
                                                        {
                                                            UserId =  userData.UserId ,
                                                            ProjectId =  userData.ProjectId ,
                                                            Units = new long[]{
                                                                5564,
                                                                6412,
                                                                9788,
                                                                10124,
                                                                10420,
                                                                10470,
                                                                11520,
                                                                11674,
                                                                11783,
                                                                13410
                                                            }
                                                        }, CancellationToken.None);
            Assert.AreNotSame(new GetAllSalaryTablesResponse(), result);
        }

        [TestMethod]
        public async Task GetTableSalaryWrongUnits()
        {
            var getTableSalaryHandler = new GetAllSalaryTablesHandler(unitOfWork);
            var result = await getTableSalaryHandler.Handle(
                                                        new GetAllSalaryTablesRequest
                                                        {
                                                            UserId =  userData.UserId ,
                                                            ProjectId =  userData.ProjectId ,
                                                            Units = new long[]{
                                                              111111111,
                                                            }
                                                        }, CancellationToken.None);
            Assert.AreNotSame(new GetAllSalaryTablesResponse(), result);
        }

        [TestMethod]
        public async Task GetTableSalaryWhitoutUserBlocks()
        {
            var getTableSalaryHandler = new GetAllSalaryTablesHandler(unitOfWork);
            var result = await getTableSalaryHandler.Handle(
                                                        new GetAllSalaryTablesRequest
                                                        {
                                                            UserId =  userData.UserId ,
                                                            ProjectId =  userData.ProjectId ,
                                                            Units = new long[]{
                                                              111111111,
                                                            }
                                                        }, CancellationToken.None);
            Assert.AreNotSame(new GetAllSalaryTablesResponse(), result);
        }
    }
}