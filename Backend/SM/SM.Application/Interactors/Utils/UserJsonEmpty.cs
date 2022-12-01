using Newtonsoft.Json;
using System.Collections.Generic;
using SM.Domain.Common;

namespace SM.Application.Interactors.Utils
{
    public static class UserJsonEmpty
    {

        public static string GetUserJsonEmpty()
        {

            return JsonConvert.SerializeObject(new PermissionJson
            {
                Levels = new List<long>(),
                Areas = new List<long>(),
                TypeOfContract = new List<long>(),
                DataBase = new List<long>(),
                Modules = new List<FieldCheckedUserJson>(),
                Contents = new List<FieldCheckedUserJson>(),
                Permission = new List<PermissionFieldJson>(),
                Display = new DisplayFieldJson()
            });
        }
    }
}
