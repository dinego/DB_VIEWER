using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SM.Api.Security
{
    public static class AuthExtensions
    {
        public static Permissions GetUserPermissions(this ClaimsPrincipal claimsPrincipal)
        {
            var products = claimsPrincipal.FindFirstValue(SigningConfiguration.Products);

            if (string.IsNullOrWhiteSpace(products))
                throw new Exception("Não foi localizado no token os dados do Produto");

            var productSM = JsonConvert.DeserializeObject<ProductInfo>(products);

            if (productSM == null)
                throw new Exception("Não foi localizado no token os dados do Produto SM");

            return productSM.Permissions;
        }
        public static long GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var user = claimsPrincipal.FindFirstValue(SigningConfiguration.User);
            if (string.IsNullOrWhiteSpace(user))
                throw new Exception("Não foi localizado no token os dados do usuário");

            var userInfoResult = JsonConvert.DeserializeObject<UserInfo>(user);

            return userInfoResult.Id;
        }

        public static long GetCompanyId(this ClaimsPrincipal claimsPrincipal)
        {
            var user = claimsPrincipal.FindFirstValue(SigningConfiguration.User);
            if (string.IsNullOrWhiteSpace(user))
                throw new Exception("Não foi localizado no token os dados do usuário");

            var userInfoResult = JsonConvert.DeserializeObject<UserInfo>(user);

            return userInfoResult.CompanyId;
        }

        public static List<long> GetUserCompanies(this ClaimsPrincipal claimsPrincipal)
        {
            var products = claimsPrincipal.FindFirstValue(SigningConfiguration.Products);

            if (string.IsNullOrWhiteSpace(products))
                throw new Exception("Não foi localizado no token os dados do Produto");

            var productSM = JsonConvert.DeserializeObject<ProductInfo>(products);

            if (productSM == null)
                throw new Exception("Não foi localizado no token os dados do Produto SM");

            return productSM.UserCompanies;

        }

        public static long GetProjectId(this ClaimsPrincipal claimsPrincipal)
        {
            var products = claimsPrincipal.FindFirstValue(SigningConfiguration.Products);

            if (string.IsNullOrWhiteSpace(products))
                throw new Exception("Não foi localizado no token os dados do Produto");

            var productSM = JsonConvert.DeserializeObject<ProductInfo>(products);

            if (productSM == null)
                throw new Exception("Não foi localizado no token os dados do Produto SM");

            var projectId = productSM.ProjectId;

            if (!projectId.HasValue)
                throw new Exception("Nenhum projeto foi vinculado a este usuário");

            return projectId.Value;

        }

        public static bool IsSimulated(this ClaimsPrincipal claimsPrincipal)
        {
            var user = claimsPrincipal.FindFirstValue(SigningConfiguration.User);
            if (string.IsNullOrWhiteSpace(user))
                return false;

            var userInfoResult = JsonConvert.DeserializeObject<UserInfo>(user);

            return userInfoResult.Simulated;
        }

        public static bool IsAdmin(this ClaimsPrincipal claimsPrincipal)
        {
            var user = claimsPrincipal.FindFirstValue(SigningConfiguration.User);
            if (string.IsNullOrWhiteSpace(user))
                return false;

            var userInfoResult = JsonConvert.DeserializeObject<UserInfo>(user);

            return userInfoResult.IsAdmin;
        }

        public static bool CanEditLocalLabels(this ClaimsPrincipal claimsPrincipal)
        {
            return GetUserPermissions(claimsPrincipal).CanEditLocalLabels;
        }
        public static bool CanEditGlobalLabels(this ClaimsPrincipal claimsPrincipal)
        {
            return GetUserPermissions(claimsPrincipal).CanEditGlobalLabels;
        }
        public static bool CanEditListPosition(this ClaimsPrincipal claimsPrincipal)
        {
            return GetUserPermissions(claimsPrincipal).CanEditListPosition;
        }
        public static bool CanEditMappingPositionSM(this ClaimsPrincipal claimsPrincipal)
        {
            return GetUserPermissions(claimsPrincipal).CanEditMappingPositionSM;
        }
    }
}
