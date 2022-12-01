using System.Threading.Tasks;

namespace SM.Application.Interactors.Interfaces
{
    public interface IGenerateExcelFileInteractor
    {
        Task<byte[]> Handler(GenerateExcelFileRequest request);
        byte[] GenerateCustomHandler(GenerateExcelFileRequest request, bool isAddTotal = true);
    }
}
