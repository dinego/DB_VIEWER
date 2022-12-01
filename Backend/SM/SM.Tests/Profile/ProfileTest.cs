using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.GetAllProfiles;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.GetProfile
{
    [TestClass]
    public class ProfileTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public ProfileTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task GetAllProfiles()
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
            //, '{"Levels":[],"TypeOfContract":[],"DataBase":[],"Modules":[],"SectionDisplay":null,"Permission":[],"Areas":[],"Contents":[{"Id":3,"SubItems":[3,5]},{"Id":2,"SubItems":[3]}],"Scenario":[]}'
            //, null)
            var getAllProfilesHandleHandler = new GetAllProfilesHandler(unitOfWork);
            var result = await getAllProfilesHandleHandler.Handle(
                                                        new GetAllProfilesRequest
                                                        {
                                                            UserId =  userData.UserId ,
                                                            ProjectId =  userData.ProjectId ,
                                                            Units = userData.Companies.ToArray()
                                                        }, CancellationToken.None);
            Assert.AreNotSame(new GetAllProfilesResponse(), result);
        }

        [TestMethod]
        public async Task GetAllProfilesHandleWrongUnits()
        {

                var getAllProfilesHandleHandler = new GetAllProfilesHandler(unitOfWork);
                var result = await getAllProfilesHandleHandler.Handle(
                                                            new GetAllProfilesRequest
                                                            {
                                                                ProjectId =  userData.ProjectId ,
                                                                UserId =  userData.UserId ,
                                                                Units = userData.Companies.ToArray()
                                                            }, CancellationToken.None);

            Assert.AreNotSame(new GetAllProfilesRequest(), result);
        }

        [TestMethod]
        public async Task GetAllProfilesHandleWhitoutUserBlocks()
        {
            var getAllProfilesHandleHandler = new GetAllProfilesHandler(unitOfWork);
            var result = await getAllProfilesHandleHandler.Handle(
                                                        new GetAllProfilesRequest
                                                        {
                                                            ProjectId =  userData.ProjectId ,
                                                            UserId =  userData.UserId ,
                                                            Units = new long[]{
                                                              111111111,
                                                            }
                                                        }, CancellationToken.None);
            Assert.AreNotSame(new GetAllProfilesResponse(), result);
        }
    }
}