using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
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
    public class UpdateSalaryTableInfoRequest : IRequest
    {
        public long UserId { get; set; }
        public long ProjectId { get; set; }
        public long TableId { get; set; }
        public string SalaryTableName { get; set; }
        public int GsmInitial { get; set; }
        public int GsmFinal { get; set; }
        public string Justify { get; set; }
        public double? Multiply { get; set; }
        public int? TypeMultiply { get; set; }
    }

    public class UpdateSalaryTableInfoHandler : IRequestHandler<UpdateSalaryTableInfoRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateSalaryTableInfoPermissionException> _validator;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly ValidatorResponse _validatorResponse;

        public UpdateSalaryTableInfoHandler(IUnitOfWork unitOfWork,
                                            IValidator<UpdateSalaryTableInfoPermissionException> validator,
                                            IPermissionUserInteractor permissionUserInteractor,
                                            ValidatorResponse validatorResponse)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _permissionUserInteractor = permissionUserInteractor;
            _validatorResponse = validatorResponse;
        }
        public async Task<Unit> Handle(UpdateSalaryTableInfoRequest request, CancellationToken cancellationToken)
        {
            var resName = _validator.Validate(new UpdateSalaryTableInfoPermissionException
            {
                SalaryTable = new AuxSalaryTable
                {
                    GsmFinal = request.GsmFinal,
                    GsmInitial = request.GsmInitial,
                    TypeMultiply = request.TypeMultiply,
                    Justify = request.Justify,
                    Multiply = request.Multiply,
                    SalaryTableName = request.SalaryTableName,
                    SalaryTableValues = null
                },
                UserId = request.UserId
            });

            if (!resName.IsValid)
                _validatorResponse.AddNotifications(resName.Errors.ToList());

            //update tabela salarial
            var dateNow = System.DateTime.Now;

            var salaryTableForUpdate = await _unitOfWork.GetRepository<TabelasSalariais, long>()
                                                        .GetAsync(x => x.Where(w => w.TabelaSalarialIdLocal == request.TableId &&
                                                                                    w.ProjetoId == request.ProjectId));

            if (salaryTableForUpdate.TabelaSalarial.Trim().ToLower() != request.SalaryTableName.Trim().ToLower())
            {
                salaryTableForUpdate.TabelaSalarial = request.SalaryTableName;
                
                _unitOfWork.GetRepository<TabelasSalariais, long>().Update(salaryTableForUpdate);
            }

            var salaryTableValues = await _unitOfWork.GetRepository<TabelasSalariaisValores, long>()
                                                     .GetListAsync(x => x.Where(w => w.TabelaSalarialIdLocal == request.TableId &&
                                                                                     w.ProjetoId == request.ProjectId));

            var newSalaryTableHistory = new List<TabelasSalariaisValoresHistoricos>();
            

            salaryTableValues.Safe().ForEach(value =>
            {
                newSalaryTableHistory.Add(CreateHistoryData(value));

                if (value.Grade >= request.GsmInitial && value.Grade <= request.GsmFinal)
                {
                    value.DataAtualizacao = dateNow;

                    value.FaixaMenos6 = value.FaixaMenos6 != null ? SetOperationValue(value.FaixaMenos6, request.Multiply, request.TypeMultiply) : null;
                    value.FaixaMenos5 = value.FaixaMenos5 != null ? SetOperationValue(value.FaixaMenos5, request.Multiply, request.TypeMultiply) : null;
                    value.FaixaMenos4 = value.FaixaMenos4 != null ? SetOperationValue(value.FaixaMenos4, request.Multiply, request.TypeMultiply) : null;
                    value.FaixaMenos3 = value.FaixaMenos3 != null ? SetOperationValue(value.FaixaMenos3, request.Multiply, request.TypeMultiply) : null;
                    value.FaixaMenos2 = value.FaixaMenos2 != null ? SetOperationValue(value.FaixaMenos2, request.Multiply, request.TypeMultiply) : null;
                    value.FaixaMenos1 = value.FaixaMenos1 != null ? SetOperationValue(value.FaixaMenos1, request.Multiply, request.TypeMultiply) : null;

                    value.FaixaMidPoint = (double)SetOperationValue(value.FaixaMidPoint, request.Multiply, request.TypeMultiply);

                    value.FaixaMais1 = value.FaixaMais1 != null ? SetOperationValue(value.FaixaMais1, request.Multiply, request.TypeMultiply) : null;
                    value.FaixaMais2 = value.FaixaMais2 != null ? SetOperationValue(value.FaixaMais2, request.Multiply, request.TypeMultiply) : null;
                    value.FaixaMais3 = value.FaixaMais3 != null ? SetOperationValue(value.FaixaMais3, request.Multiply, request.TypeMultiply) : null;
                    value.FaixaMais4 = value.FaixaMais4 != null ? SetOperationValue(value.FaixaMais4, request.Multiply, request.TypeMultiply) : null;
                    value.FaixaMais5 = value.FaixaMais5 != null ? SetOperationValue(value.FaixaMais5, request.Multiply, request.TypeMultiply) : null;
                    value.FaixaMais6 = value.FaixaMais6 != null ? SetOperationValue(value.FaixaMais6, request.Multiply, request.TypeMultiply) : null;

                }
            });

            var newSalaryTableUpdate = new TabelasSalariaisAtualizacoes
            {
                Atualizacao = dateNow,
                GSMFinal = request.GsmFinal,
                GSMInicial = request.GsmInitial,
                Justificativa = request.Justify,
                ProjetoId = request.ProjectId,
                TabelaSalarialIdLocal = request.TableId,
                Multiplicador = request.Multiply,
                TipoMultiplicador = request.TypeMultiply,
                UsuarioId = request.UserId,
                OperacaoId = (long)SalaryTableOperationsEnum.ApplyUpdate,
                TabelasSalariaisValoresHistoricos = newSalaryTableHistory
            };

            _unitOfWork.GetRepository<TabelasSalariaisAtualizacoes, long>().Add(newSalaryTableUpdate);

            await _unitOfWork.CommitAsync();

            return Unit.Value;
        }

        #region Methods

        private double? SetOperationValue(double? track,
                                          double? multiply,
                                          int? typeMultiply)
        {
            if (typeMultiply.HasValue && typeMultiply.Value == (int?)TypeMultiplyEnum.Percent)
                return ((multiply / 100) * track) + track;

            return track + multiply;
        }

        private TabelasSalariaisValoresHistoricos CreateHistoryData(TabelasSalariaisValores value)
        {
            return new TabelasSalariaisValoresHistoricos
            {
                TabelaSalarialValoresId = value.Id,
                TabelaSalarialId = value.TabelaSalarialId,
                TabelaSalarialIdLocal = value.TabelaSalarialIdLocal,
                TempoId = value.TempoId,
                ProjetoId = value.ProjetoId,
                Grade = value.Grade,
                DataAtualizacao = value.DataAtualizacao,
                FaixaMais1 = value.FaixaMais1,
                FaixaMais2 = value.FaixaMais2,
                FaixaMais3 = value.FaixaMais3,
                FaixaMais4 = value.FaixaMais4,
                FaixaMais5 = value.FaixaMais5,
                FaixaMais6 = value.FaixaMais6,
                FaixaMidPoint = value.FaixaMidPoint,
                FaixaMenos1 = value.FaixaMenos1,
                FaixaMenos2 = value.FaixaMenos2,
                FaixaMenos3 = value.FaixaMenos3,
                FaixaMenos4 = value.FaixaMenos4,
                FaixaMenos5 = value.FaixaMenos5,
                FaixaMenos6 = value.FaixaMenos6,
            };
        }

        #endregion
    }
}
