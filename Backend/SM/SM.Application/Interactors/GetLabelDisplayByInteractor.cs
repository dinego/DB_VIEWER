using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum.Positioning;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SM.Application.Interactors
{
    public class GetLabelDisplayByInteractor : IGetLabelDisplayByInteractor
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLabelDisplayByInteractor(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> Handler(object category,long projectId, DisplayByPositioningEnum displayBy)
        {

            switch (displayBy)
            {
                case DisplayByPositioningEnum.ProfileId:
                    return await _unitOfWork.GetRepository<GruposProjetosSalaryMark, long>()
                        .GetAsync(x => x.Where(g => g.GruposProjetosSmidLocal == Convert.ToInt64(category) && projectId == g.ProjetoId)?
                        .Select(se => se.GrupoSm));

                case DisplayByPositioningEnum.LevelId:
                    return await _unitOfWork.GetRepository<Niveis, int>()
                                .GetAsync(x => x.Where(g => g.Id == Convert.ToInt32(category))?
                                .Select(se => se.Nivel));
                default:
                    return category.ToString();
            }
        }
    }
}
