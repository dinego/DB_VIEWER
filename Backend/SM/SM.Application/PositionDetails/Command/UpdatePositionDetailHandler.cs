using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using SM.Application.TableSalary.Validators;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using CMC.Common.Domain.Entities;
using System.Linq;
using SM.Application.Parameters.Validators;
using SM.Domain.Enum;

namespace SM.Application.TableSalary.Command
{
    public class UpdatePositionDetailRequest : IRequest
    {
        public long ProjectId { get; set; }
        public UpdatePositionDetailData PositionData { get; set; }
        public bool CanEditListPosition { get; set; }
    }

    public class UpdatePositionDetailData
    {
        public long PositionId { get; set; }
        public string Position { get; set; }
        public string SMCode { get; set; }
        public int LevelId { get; set; }
        public long GroupId { get; set; }
        public List<UpdateProjectParameters> Parameters { get; set; }
    }

    public class UpdateProjectParameters
    {
        public long ParameterId { get; set; }
        public List<long> ProjectParameters { get; set; }
        public List<string> NewProjectParameters { get; set; }
    }


    public class UpdatePositionDetailHandler : IRequestHandler<UpdatePositionDetailRequest>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ValidatorResponse _validatorResponse;
        private readonly UpdatePositionDetailsRule _validatorPositionDetailsRule;

        public UpdatePositionDetailHandler(IUnitOfWork unitOfWork,
            IValidator<UpdateDisplayColumnsPemissionException> validator,
            ValidatorResponse validatorResponse)
        {
            _unitOfWork = unitOfWork;
            _validatorResponse = validatorResponse;
            _validatorPositionDetailsRule = new UpdatePositionDetailsRule(_validatorResponse, _unitOfWork);
        }

        public async Task<Unit> Handle(UpdatePositionDetailRequest request, CancellationToken cancellationToken)
        {
            if (!_validatorPositionDetailsRule.IsSatisfiedBy(request))
                return Unit.Value;
            var positionSM = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                                        .Include("CargosProjetoSMParametrosMapeamento")
                                        .Include("CargosProjetoSMEixoCarreiraMapeamentos")
                                        .GetAsync(x => x.Where(cp => cp.ProjetoId == request.ProjectId &&
                                                                     cp.CargoProjetoSmidLocal == request.PositionData.PositionId));
            if (positionSM == null)
                return Unit.Value;
            positionSM.NivelId = request.PositionData.LevelId;
            positionSM.Smcode = request.PositionData.SMCode;
            positionSM.CargoSm = request.PositionData.Position;
            positionSM.GrupoSmidLocal = request.PositionData.GroupId;
            if (positionSM.CargosProjetoSMParametrosMapeamento.Any())
                _unitOfWork.GetRepository<CargosProjetoSMParametrosMapeamento, long>().Delete(positionSM.CargosProjetoSMParametrosMapeamento);
            if (positionSM.CargosProjetoSMEixoCarreiraMapeamentos.Any())
                _unitOfWork.GetRepository<CargosProjetoSMEixoCarreiraMapeamentos, long>().Delete(positionSM.CargosProjetoSMEixoCarreiraMapeamentos);

            positionSM.CargosProjetoSMParametrosMapeamento = new List<CargosProjetoSMParametrosMapeamento>();
            var newParameterList = new List<ParametrosProjetosSMLista>();
            var newCareerAxis = new List<EixoCarreiraSM>();
            request.PositionData.Parameters.ForEach(parameter =>
            {
                switch (parameter.ParameterId)
                {
                    case (long)ParametersProjectsTypes.CarreiraEixo:
                        parameter.ProjectParameters.ForEach(projectParameter =>
                        {
                            var careerAxisMapping = new CargosProjetoSMEixoCarreiraMapeamentos
                            {
                                CargoProjetoSmidLocal = request.PositionData.PositionId,
                                EixoCarreiraId = projectParameter,
                                ProjetoId = request.ProjectId
                            };
                            positionSM.CargosProjetoSMEixoCarreiraMapeamentos.Add(careerAxisMapping);
                        });
                        parameter.NewProjectParameters.ForEach(parameterProject =>
                        {
                            var paramCareerAxis = new EixoCarreiraSM
                            {
                                EixoCarreira = parameterProject,
                                Ativo = true,
                                ProjetoId = request.ProjectId,
                                CargosProjetoSMEixoCarreiraMapeamentos = new List<CargosProjetoSMEixoCarreiraMapeamentos>{
                                    new CargosProjetoSMEixoCarreiraMapeamentos
                                    {
                                        CargoProjetoSmidLocal = positionSM.CargoProjetoSmidLocal,
                                        ProjetoId = request.ProjectId
                                    }
                                }
                            };
                            newCareerAxis.Add(paramCareerAxis);
                        });
                        break;
                    default:
                        parameter.ProjectParameters.ForEach(projetctParameterId =>
                        {
                            var item = new CargosProjetoSMParametrosMapeamento
                            {
                                CargoProjetoSMIdLocal = request.PositionData.PositionId,
                                ParametroProjetoSMId = projetctParameterId,
                                ProjetoId = request.ProjectId
                            };
                            positionSM.CargosProjetoSMParametrosMapeamento.Add(item);
                        });
                        parameter.NewProjectParameters.ForEach(parameterProject =>
                        {
                            var paramList = new ParametrosProjetosSMLista
                            {
                                ParametroSMTipoId = parameter.ParameterId,
                                ParametroProjetoSMLista = parameterProject,
                                Ativo = true,
                                ProjetoId = request.ProjectId,
                                CargosProjetoSMParametrosMapeamento = new List<CargosProjetoSMParametrosMapeamento>{
                                    new CargosProjetoSMParametrosMapeamento
                                    {
                                        CargoProjetoSMIdLocal = positionSM.CargoProjetoSmidLocal,
                                        ProjetoId = request.ProjectId
                                    }
                                }
                            };
                            newParameterList.Add(paramList);
                        });
                        break;
                }
            });
            _unitOfWork.GetRepository<CargosProjetosSm, long>().Update(positionSM);
            _unitOfWork.GetRepository<ParametrosProjetosSMLista, long>().Add(newParameterList);
            _unitOfWork.GetRepository<EixoCarreiraSM, long>().Add(newCareerAxis);
            await _unitOfWork.CommitAsync();
            return Unit.Value;
        }
    }
}
