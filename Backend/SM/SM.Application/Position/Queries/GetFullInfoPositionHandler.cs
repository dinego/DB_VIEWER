using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions;
using CMC.Common.Repositories;
using FluentValidation;
using MediatR;
using SM.Application.Interactors.Interfaces;
using SM.Application.Position.Querie;
using SM.Application.Position.Queries.Response;
using SM.Application.Position.Validators;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.Position.Queries
{
    public class GetFullInfoPositionRequest : IRequest<GetFullInfoPositionResponse>
    {
        public long PositionSmIdLocal { get; set; }
        public long ProjectId { get; set; }
        public IEnumerable<long> CompaniesId { get; set; }
        public long UserId { get; set; }
    }
    public class GetFullInfoPositionResponse
    {

        public IEnumerable<Framework> Framework { get; set; }
        = new List<Framework>();

        public IEnumerable<LabelControlPosition> HeaderPosition { get; set; }
                = new List<LabelControlPosition>();
    }

    public class LabelControlPosition
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public bool Visible { get; set; } = true;
        public string @Type { get; set; }
        public int PropertyId { get; set; }
    }


    public class GetFullInfoPositionDTO
    {
        public long LocalId { get; set; }
        public string PositionSalaryMark { get; set; }
        public string Smcode { get; set; }
        public long GSM { get; set; }
        public string Level { get; set; }
        public string AxisCareer { get; set; }
        public double HourBase { get; set; }
        public string Profile { get; set; }
        public string Parameter02 { get; set; }
        public long LevelId { get; set; }
        public string Area { get; set; }
        public long GroupId { get; set; }
        public long CmCode { get; set; }
    }
    public class Framework
    {

        public string Unit { get; set; }
        public string CurrentyPosition { get; set; }
        public int AmountEmployees { get; set; }

    }

    public class FrameworkDto
    {

        public string Unit { get; set; }
        public string CurrentyPosition { get; set; }
        public int AmountEmployees { get; set; }

    }

    public class GetFullInfoPositionHandler
        : IRequestHandler<GetFullInfoPositionRequest, GetFullInfoPositionResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<GetFullInfoPositionValidatorsException> _validator;
        private readonly ValidatorResponse _validatorResponse;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        private readonly IGetGlobalLabelsInteractor _getGlobalLabelsInteractor;

        public GetFullInfoPositionHandler(IUnitOfWork unitOfWork,
            IValidator<GetFullInfoPositionValidatorsException> validator,
            ValidatorResponse validatorResponse,
            IPermissionUserInteractor permissionUserInteractor,
            IGetGlobalLabelsInteractor getGlobalLabelsInteractor)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _validatorResponse = validatorResponse;
            _permissionUserInteractor = permissionUserInteractor;
            _getGlobalLabelsInteractor = getGlobalLabelsInteractor;

        }
        public async Task<GetFullInfoPositionResponse> Handle(GetFullInfoPositionRequest request, CancellationToken cancellationToken)
        {
            var permissionUser = await _permissionUserInteractor.Handler(request.UserId);

            var groupsExp =
                permissionUser.Contents?
                .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group)?
                .SubItems;

            var areaExp =
            permissionUser.Areas;

            var positionSm = await _unitOfWork.GetRepository<CargosProjetosSm, long>()
                .Include("GruposProjetosSalaryMark")
                .Include("CmcodeNavigation")
                .GetAsync(x => x.Where(cps => cps.CargoProjetoSmidLocal == request.PositionSmIdLocal &&
                cps.ProjetoId == request.ProjectId &&
                 cps.Ativo.HasValue && cps.Ativo.Value &&
                 (!groupsExp.Safe().Any() || !groupsExp.Contains(cps.GrupoSmidLocal)) &&
                 (!areaExp.Safe().Any() /*|| !areaExp.Contains(cps.Area)*/))?
                .Select(s => new GetFullInfoPositionDTO
                {
                    LocalId = s.CargoProjetoSmidLocal,
                    //AxisCareer = s.EixoCarreira,
                    PositionSalaryMark = s.CargoSm,
                    Smcode = s.Smcode,
                    CmCode = s.Cmcode,
                    //GSM = s.Gsm,
                    HourBase = s.BaseHoraria,
                    Profile = s.GruposProjetosSalaryMark.GrupoSm,
                    //Parameter02 = s.Parametro2,
                    //Area = s.Area,
                    LevelId = s.CmcodeNavigation.NivelId,
                    GroupId = s.GrupoSmidLocal,
                }));

            if (positionSm == null)
                throw new
                    Exception($"Não foi encontrado nenhum cargo Sm para ID {request.PositionSmIdLocal} para o usuário id {request.UserId}");

            positionSm.Level = await GetLevelCompanies(request, positionSm.LevelId);

            var resName = _validator.Validate(new GetFullInfoPositionValidatorsException
            {
                UserId = request.UserId,
                //   Area = positionSm.Area,
                GroupId = positionSm.GroupId,
                LevelId = positionSm.LevelId
            });

            if (!resName.IsValid)
                _validatorResponse.AddNotifications(resName.Errors.ToList());

            var positionId = positionSm.LocalId;
            var ignoreSituationPerson = !permissionUser.Permission.Any(x => x.Id == (long)PermissionItensEnum.InactivePerson);

            var framework = await _unitOfWork.GetRepository<BaseSalariais, long>()
                .Include("Empresa")
                .GetListAsync(x => x.Where(bs => bs.CargoIdSMMM.HasValue &&
                bs.CargoIdSMMM.Value == positionId &&
                (ignoreSituationPerson || bs.SituacaoFuncionarioId == (int)EmployeeSituationEnum.Active) &&
                request.CompaniesId.Contains(bs.EmpresaId.Value))?
                .Select(s => new FrameworkDto
                {
                    CurrentyPosition = s.CargoEmpresa,
                    Unit = string.IsNullOrWhiteSpace(s.Empresa.NomeFantasia) ?
                   (string.IsNullOrWhiteSpace(s.Empresa.RazaoSocial) ? string.Empty : s.Empresa.RazaoSocial) :
                   s.Empresa.NomeFantasia
                })
                );

            var frameworkresult = !framework.Any() ? null : framework.ToList().GroupBy(g => g.CurrentyPosition,
                (key, value) =>
                 new Framework
                 {
                     CurrentyPosition = key,
                     AmountEmployees = value.Count(),
                     Unit = value.FirstOrDefault().Unit
                 });

            return new GetFullInfoPositionResponse
            {
                Framework = frameworkresult ?? new List<Framework>(),
                HeaderPosition = await GetData(request, positionSm) ?? new List<LabelControlPosition>()
            };
        }

        private async Task<IEnumerable<LabelControlPosition>>
            GetData(GetFullInfoPositionRequest request,
            GetFullInfoPositionDTO positionSm)
        {
            var configGlobalLabels =
                await _getGlobalLabelsInteractor.Handler(request.ProjectId);

            var columnsData = await QueryColumnsCustom(request);

            columnsData.ToList().ForEach(f =>
            {
                var globalLabel =
                configGlobalLabels.FirstOrDefault(fe => fe.Id == f.ColumnId);

                if (globalLabel != null)
                {
                    f.Name = globalLabel.Alias;
                }
            });

            var listEnum = Enum.GetValues(typeof(FullInfoPositionEnum)) as
                        IEnumerable<FullInfoPositionEnum>;

            var listLabels = new List<LabelControlPosition>();

            foreach (var item in listEnum)
            {
                var checkCustomLabel =
                    columnsData.FirstOrDefault(f => f.ColumnId == (int)item);

                var value =
                            positionSm.GetType()
                            .GetProperty(item.ToString())?
                            .GetValue(positionSm);

                if (checkCustomLabel != null)
                {
                    listLabels.Add(new LabelControlPosition
                    {
                        Name = checkCustomLabel.Name,
                        Value = value == null ? string.Empty : value.ToString(),
                        Visible = checkCustomLabel.IsChecked.GetValueOrDefault(),
                        Type = value == null ? typeof(string).Name : value.GetType().Name,
                        PropertyId = (int)item
                    });
                }
                else
                {
                    listLabels.Add(new LabelControlPosition
                    {
                        Name = item.GetDescription(),
                        Value = value == null ? string.Empty : value.ToString(),
                        Type = value == null ? typeof(string).Name : value.GetType().Name,
                        PropertyId = (int)item
                    });

                }
            }

            return listLabels;
        }

        private async Task<IEnumerable<ColumnDataPositionDTO>> QueryColumnsCustom(GetFullInfoPositionRequest request)
        {
            var companyDefaultHeader = await _unitOfWork.GetRepository<EmpresaPermissaoSm, long>().GetAsync(x => x.Where(ep => request.CompaniesId.Contains(ep.EmpresaId)));


            var result = await _unitOfWork.GetRepository<UsuarioExibicaoSm, long>()
                .GetListAsync(x => x.Where(ues => ues.UsuarioId == request.UserId &&
                 ues.ModuloSmid == (long)ModulesEnum.Position &&
                 ues.ModuloSmsubItemId.HasValue && ues.ModuloSmsubItemId.Value == (long)ModulesSuItemsEnum.Architecture)
                ?.Select(s => new ColumnDataPositionDTO
                {
                    ColumnId = s.ColunaId,
                    IsChecked = s.Checado,
                    Name = s.ColunaId == (long)FullInfoPositionEnum.PositionSalaryMark && companyDefaultHeader != null ?
                    companyDefaultHeader.CargoSalaryMark : s.ColunaId == (long)FullInfoPositionEnum.PositionSalaryMark ?
                    PositionProjectColumnsEnum.PositionSalaryMark.GetDescription() : s.Nome
                }));

            if (!result.Safe().Any())
                return new List<ColumnDataPositionDTO> {
                            new ColumnDataPositionDTO{
                                ColumnId = (long)FullInfoPositionEnum.PositionSalaryMark,
                                IsChecked = true,
                                Name = companyDefaultHeader.CargoSalaryMark
                            }
                    };

            return result;
        }
        private async Task<string> GetLevelCompanies(GetFullInfoPositionRequest request, long levelId)
        {
            var levelCompanies = await _unitOfWork.GetRepository<NivelEmpresas, long>()
                .GetAsync(x => x.Where(ne => request.CompaniesId.Contains(ne.EmpresaId) &&
                 levelId == ne.NivelId)?
                .Select(s => s.NivelEmpresa));

            return levelCompanies ?? string.Empty;

        }
    }
}
