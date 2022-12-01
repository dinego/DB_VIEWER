using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Application.TableSalary.Validators;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.TableSalary.Command
{
    public class UpdateSalaryTableRequest : IRequest
    {
        public long UserId { get; set; }
        public AuxSalaryTable SalaryTable { get; set; }
        public long ProjectId { get; set; }
        public long TableId { get; set; }
    }

    #region Auxiliary classes

    public class AuxSalaryTable
    {
        public AuxSalaryTable()
        {
            SalaryTableValues = new List<AuxSalaryTableValues>();
        }

        public string SalaryTableName { get; set; }
        public int GsmInitial { get; set; }
        public int GsmFinal { get; set; }
        public string Justify { get; set; }
        public double? Multiply { get; set; }
        public int? TypeMultiply { get; set; }
        public List<AuxSalaryTableValues> SalaryTableValues { get; set; }
    }
    public class AuxSalaryTableUpdate
    {
        public int GsmInitial { get; set; }
        public int GsmFinal { get; set; }
        public int? TypeMultiply { get; set; }
        public double? Multiply { get; set; }
    }

    public class AuxSalaryTableValues
    {
        public int Gsm { get; set; }
        public long SalaryTableLocalId { get; set; }
        public double? Minor6 { get; set; }
        public double? Minor5 { get; set; }
        public double? Minor4 { get; set; }
        public double? Minor3 { get; set; }
        public double? Minor2 { get; set; }
        public double? Minor1 { get; set; }
        public double Mid { get; set; }
        public double? Plus1 { get; set; }
        public double? Plus2 { get; set; }
        public double? Plus3 { get; set; }
        public double? Plus4 { get; set; }
        public double? Plus5 { get; set; }
        public double? Plus6 { get; set; }
    }

    #endregion

    public class UpdateSalaryTableHandler : IRequestHandler<UpdateSalaryTableRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateSalaryTablePermissionException> _validator;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly ValidatorResponse _validatorResponse;

        public UpdateSalaryTableHandler(IUnitOfWork unitOfWork,
                                        IValidator<UpdateSalaryTablePermissionException> validator,
                                        IPermissionUserInteractor permissionUserInteractor,
                                        ValidatorResponse validatorResponse)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _permissionUserInteractor = permissionUserInteractor;
            _validatorResponse = validatorResponse;
        }

        public async Task<Unit> Handle(UpdateSalaryTableRequest request, CancellationToken cancellationToken)
        {
            var resName = _validator.Validate(new UpdateSalaryTablePermissionException
            {
                SalaryTable = request.SalaryTable,
                UserId = request.UserId
            });

            if (!resName.IsValid)
                _validatorResponse.AddNotifications(resName.Errors.ToList());

            var dateNow = System.DateTime.Now;

            //update tabela salarial
            var salaryTableForUpdate = await _unitOfWork.GetRepository<TabelasSalariais, long>()
                                                        .GetAsync(x => x.Where(w => w.TabelaSalarialIdLocal == request.TableId &&
                                                                                    w.ProjetoId == request.ProjectId));

            if (salaryTableForUpdate.TabelaSalarial.Trim().ToLower() != request.SalaryTable.SalaryTableName.Trim().ToLower())
            {
                salaryTableForUpdate.TabelaSalarial = request.SalaryTable.SalaryTableName.Trim();

                _unitOfWork.GetRepository<TabelasSalariais, long>().Update(salaryTableForUpdate);
            }

            var salaryTableValues = await _unitOfWork.GetRepository<TabelasSalariaisValores, long>()
                                                     .GetListAsync(x => x.Where(w => w.TabelaSalarialIdLocal == request.TableId &&
                                                                                     w.ProjetoId == request.ProjectId));

            var newSalaryTableHistory = new List<TabelasSalariaisValoresHistoricos>();

            salaryTableValues.ForEach(values =>
            {
                newSalaryTableHistory.Add(CreateHistoryData(values));

                var findValues = request.SalaryTable != null && request.SalaryTable.SalaryTableValues.Count > 0
                                 ? request.SalaryTable.SalaryTableValues.FirstOrDefault(f => f.Gsm == values.Grade)
                                 : null;

                if (findValues == null)
                    return;

                values.DataAtualizacao = dateNow;

                values.FaixaMenos6 = findValues.Minor6;
                values.FaixaMenos5 = findValues.Minor5;
                values.FaixaMenos4 = findValues.Minor4;
                values.FaixaMenos3 = findValues.Minor3;
                values.FaixaMenos2 = findValues.Minor2;
                values.FaixaMenos1 = findValues.Minor1;

                values.FaixaMidPoint = findValues.Mid;
                
                values.FaixaMais1 = findValues.Plus1;
                values.FaixaMais2 = findValues.Plus2;
                values.FaixaMais3 = findValues.Plus3;
                values.FaixaMais4 = findValues.Plus4;
                values.FaixaMais5 = findValues.Plus5;
                values.FaixaMais6 = findValues.Plus6;
            });

            var newSalaryTableUpdate = new TabelasSalariaisAtualizacoes
            {
                Atualizacao = dateNow,
                Justificativa = request.SalaryTable.Justify,
                ProjetoId = request.ProjectId,
                TabelaSalarialIdLocal = request.TableId,
                UsuarioId = request.UserId,
                OperacaoId = (long)SalaryTableOperationsEnum.EditRangesAndValuesIndividually,
                TabelasSalariaisValoresHistoricos = newSalaryTableHistory
            };

            _unitOfWork.GetRepository<TabelasSalariaisAtualizacoes, long>().Add(newSalaryTableUpdate);

            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }

        #region  Methods

        private TabelasSalariaisValoresHistoricos CreateHistoryData(TabelasSalariaisValores values)
        {
            return new TabelasSalariaisValoresHistoricos
            {
                TabelaSalarialValoresId = values.Id,
                TabelaSalarialId = values.TabelaSalarialId,
                TabelaSalarialIdLocal = values.TabelaSalarialIdLocal,
                TempoId = values.TempoId,
                ProjetoId = values.ProjetoId,
                Grade = values.Grade,
                DataAtualizacao = values.DataAtualizacao,
                FaixaMais1 = values.FaixaMais1,
                FaixaMais2 = values.FaixaMais2,
                FaixaMais3 = values.FaixaMais3,
                FaixaMais4 = values.FaixaMais4,
                FaixaMais5 = values.FaixaMais5,
                FaixaMais6 = values.FaixaMais6,
                FaixaMidPoint = values.FaixaMidPoint,
                FaixaMenos1 = values.FaixaMenos1,
                FaixaMenos2 = values.FaixaMenos2,
                FaixaMenos3 = values.FaixaMenos3,
                FaixaMenos4 = values.FaixaMenos4,
                FaixaMenos5 = values.FaixaMenos5,
                FaixaMenos6 = values.FaixaMenos6,
            };
        }

        #endregion
    }
}
