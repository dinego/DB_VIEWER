using CMC.Common.Extensions;
using SM.Domain.Enum;

namespace SM.Application.Helpers
{
    public static class PJOrCLT
    {
        public static bool? IsPJ(string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            return null;

            return value.ToLower().Trim().Equals(ContractTypeEnum.PJ.GetDescription().ToLower());
        }

        public static bool? IsCLT(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return !value.ToLower().Trim().Equals(ContractTypeEnum.PJ.GetDescription().ToLower());
        }
    }

    

}
