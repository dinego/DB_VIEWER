using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Reports.Command;
using SM.Application.Reports.Queries;
using SM.Domain.Enum;
using SM.Domain.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Reports
{
    [TestClass]
    public class ReportsTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly ValidatorResponse validator;
        private readonly PermissionUserInteractor permissionUser;
        public ReportsTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            validator = new ValidatorResponse();
            permissionUser = new PermissionUserInteractor(unitOfWork);
        }
        [TestMethod]
        public async Task GetReports()
        {
            if (userData == null) return;
            var getReportHandler = new GetReportsHandler(unitOfWork);
            var reports = await getReportHandler.Handle(new GetReportsRequest
            {
                CompaniesId = userData.Companies.ToList(),
                UserId =  userData.UserId ,
                OrderType = OrderTypeEnum.DataAsc,
                Page = 1,
                PageSize = 10,
            }, CancellationToken.None);

            Assert.AreNotEqual(new List<GetReportsResponse>(), reports);
        }

        [TestMethod]
        public async Task GetReportsDataAsc()
        {
            
            if (userData == null) return;
            var getReportHandler = new GetReportsHandler(unitOfWork);
            var reports = await getReportHandler.Handle(new GetReportsRequest
            {
                CompaniesId = userData.Companies.ToList(),
                UserId =  userData.UserId ,
                OrderType = OrderTypeEnum.DataAsc,
                Page = 1,
                PageSize = 10,
            }, CancellationToken.None);

            Assert.AreNotEqual(new List<GetReportsResponse>(), reports);
        }
        [TestMethod]
        public async Task GetReportsDataDes()
        {
          
            if (userData == null) return;
            var getReportHandler = new GetReportsHandler(unitOfWork);
            var reports = await getReportHandler.Handle(new GetReportsRequest
            {
                CompaniesId = userData.Companies.ToList(),
                UserId =  userData.UserId ,
                OrderType = OrderTypeEnum.DataDes,
                Page = 1,
                PageSize = 10,
            }, CancellationToken.None);

            Assert.AreNotEqual(new List<GetReportsResponse>(), reports);
        }
        [TestMethod]
        public async Task GetReportsTitleAsc()
        {
        
            if (userData == null) return;
            var getReportHandler = new GetReportsHandler(unitOfWork);
            var reports = await getReportHandler.Handle(new GetReportsRequest
            {
                CompaniesId = userData.Companies.ToList(),
                UserId =  userData.UserId ,
                OrderType = OrderTypeEnum.TitleAsc,
                Page = 1,
                PageSize = 10,
            }, CancellationToken.None);

            Assert.AreNotEqual(new List<GetReportsResponse>(), reports);
        }
        [TestMethod]
        public async Task GetReportsTitleDes()
        {
           
            if (userData == null) return;
            var getReportHandler = new GetReportsHandler(unitOfWork);
            var reports = await getReportHandler.Handle(new GetReportsRequest
            {
                CompaniesId = userData.Companies.ToList(),
                UserId =  userData.UserId ,
                OrderType = OrderTypeEnum.TitleDes,
                Page = 1,
                PageSize = 10,
            }, CancellationToken.None);

            Assert.AreNotEqual(new List<GetReportsResponse>(), reports);
        }

        [TestMethod]
        public async Task DownloadReport()
        { 
            if (userData == null) return;

            var report = unitOfWork.GetRepository<MeusRelatorios, long>().Get(x => x);
            if (report == null) return;

            var downloadReportHandler = new DownloadReportHandler(unitOfWork, validator);
            var reports = await downloadReportHandler.Handle(new DownloadReportRequest
            {
                ReportId = report.Id,
                User = userData.UserId,
            }, CancellationToken.None);

            Assert.IsNotNull(reports.File);
        }
    }
}
