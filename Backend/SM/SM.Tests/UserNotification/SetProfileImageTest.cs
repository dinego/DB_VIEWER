using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SM.Application.SetProfileImage;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.GetAllNotification
{
    [TestClass]
    public class SetProfileImageTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ValidatorResponse validator;
        private readonly UserTestDTO userData;

        public SetProfileImageTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); userData = UnitOfWorkTest.RetrieveUserId();
            validator = new ValidatorResponse();
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task ProfileImage()
        {
            var setProfileImageHandler = new SetProfileImageHandler(unitOfWork, validator);


            var result = await setProfileImageHandler.Handle(new SetProfileImageRequest { Attachment = HasImage("image/jpg"), UserId = userData.UserId } , CancellationToken.None);
            //var controller = new UserNotificationController();
            //var file = formFile.Object;
            //var result = controller.Scan(file);
            Assert.IsTrue(validator.HasNotifications);
        }

        [TestMethod]
        public async Task ProfileImageExceptionUser()
        {
            var setProfileImageHandler = new SetProfileImageHandler(unitOfWork, validator);
            var result = await setProfileImageHandler.Handle(new SetProfileImageRequest { Attachment = MakeAttachment(), UserId = userData.UserId } , CancellationToken.None);
            Assert.AreNotSame(string.Empty, result);
        }

        public IFormFile HasImage(string mockContentType)
        {
            var fileMock = new Mock<IFormFile>();
            //Imagens
            var sourceImg = File.OpenRead(@"C://Users/DemisLapTop/Pictures/959df842e9cfdf3703fd1ee7bfc46cd5-silhueta-de-trike-paraglider-by-vexels.jpg");
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(sourceImg);
            //writer.Write(physicalFile.OpenRead());
            writer.Flush();
            ms.Position = 0;
            var fileName = sourceImg.Name;
            //Setup mock file using info from physical file
            fileMock.Setup(f => f.FileName).Returns(fileName).Verifiable();
            fileMock.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) => ms.CopyToAsync(stream))
                .Verifiable();
            //fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.ContentDisposition).Returns(string.Format("inline; filename={0}", fileName));
            fileMock.Setup(_ => _.ContentType).Returns(mockContentType);
            // fileMock.Verify();
            var inputFile = fileMock.Object;
            return inputFile;
        }

        private IFormFile MakeAttachment()
        {
            //Or create a IFormFile real
            //Need install package asp.net core http
            //IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file to test - Demis")), 0, 0, "Data", "dummy-test-file.txt");

            //Setting attachment
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "Hello World do meu arquivo fake";
            var fileName = "test-demis.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            return fileMock.Object;
        }
    }
}