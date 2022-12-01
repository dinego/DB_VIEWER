using CMC.Common.Abstractions.Behaviours;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Threading.Tasks;

namespace SM.Api.Helpers
{
    public class NotificationFilter : IAsyncResultFilter
    {
        private readonly ValidatorResponse _validator;

        public NotificationFilter(ValidatorResponse validator)
        {
            _validator = validator;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (_validator.HasNotifications)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.HttpContext.Response.ContentType = "application/json";
                var contractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() };
                var notifications = JsonConvert.SerializeObject(_validator.Notifications,
                    new JsonSerializerSettings { ContractResolver = contractResolver, Formatting = Formatting.Indented });
                await context.HttpContext.Response.WriteAsync(notifications);
                return;
            }
            await next();
        }
    }
}
