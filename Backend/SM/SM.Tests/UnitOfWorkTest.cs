using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using CMC.Common.Repositories.EntityFrameworkCore;
using CMC.Common.Repositories.EntityFrameworkCore.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Tests
{
    public static class UnitOfWorkTest
    {
        public static IUnitOfWork GetUnitOfWork() =>
             new EfSqlUnitOfWork("Server=172.21.10.11,1433;Database=dbCM_DEV;User Id=csuser;password=Carreira@0126;Trusted_Connection=False;MultipleActiveResultSets=true;", EfUnitOfWork.MapEntityTypesFrom<Cmcodes>);

        public static UserTestDTO RetrieveUserId()
        {
            try
            {
                var unitOfWork = GetUnitOfWork();
                var user = unitOfWork.GetRepository<Usuarios, long>()
                    .Include("Empresa")
                    .Include("Empresa.ProjetosSalaryMarkEmpresas")
                    .Include("Empresa.ProjetosSalaryMark")
                    .Include("UsuarioPermissaoSm")
                    .Get(x => x.Where(u => u.EmpresaId == 7898 && u.Empresa != null && u.Empresa.ProjetosSalaryMarkEmpresas.Any() && u.UsuarioPermissaoSm.Any())
                              .Select(res => new UserTestDTO
                              {
                                  UserId = res.Id,
                                  CompanyId = res.EmpresaId,
                                  ProjectId = res.Empresa.ProjetosSalaryMark.FirstOrDefault().Id,
                                  ProjectCompanyId = res.Empresa != null && res.Empresa.ProjetosSalaryMark.FirstOrDefault() != null ?
                                  res.Empresa.ProjetosSalaryMark.FirstOrDefault().CodigoPai : (long?)null,
                                  Companies = res.Empresa != null && res.Empresa.ProjetosSalaryMarkEmpresas != null ?
                                  res.Empresa.ProjetosSalaryMarkEmpresas.Select(x => x.EmpresaId) : null
                              }));
                if (user != null)
                    return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }


        public static async Task<IEnumerable<PositionProjectSMAndSalaryTableDTO>>
            PositionProjectSMAndSalaryTableTest()
        {
            try
            {
                var userData = RetrieveUserId();
                IUnitOfWork unitOfWork = GetUnitOfWork();

                var result = await unitOfWork.GetRepository<VwBaseSalarialSalaryMarkMm, Guid>()
                    .GetListAsync(x => x.Where(u => u.CargoIdSmmm.HasValue &&
                    userData.Companies.Contains(u.EmpresaId.Value) &&
                    userData.ProjectId == u.ProjetoId)?
                              .Select(res => new PositionProjectSMAndSalaryTableDTO
                              {
                                  CompanyId = res.EmpresaId.Value,
                                  FinalSalary = res.SalarioFinalSm,
                                  HoursBase = res.BaseHoraria,
                                  PositionId = res.CargoIdSmmm.Value
                              }));
                if (result != null)
                    return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return null;
        }

    }


    public class UserTestDTO
    {
        public long UserId { get; set; }
        public long? ProjectCompanyId { get; set; }
        public long ProjectId { get; set; }
        public long CompanyId { get; set; }
        public IEnumerable<long> Companies { get; set; }
    }

    public class PositionProjectSMAndSalaryTableDTO
    {
        public long CompanyId { get; set; }
        public double? FinalSalary { get; set; }
        public double? HoursBase { get; set; }
        public long PositionId { get; set; }
    }

}
