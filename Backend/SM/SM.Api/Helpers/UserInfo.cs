namespace SM.Api.Security
{
    public class UserInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CompanyId { get; set; }
        public string Key { get; set; }
        public string Email { get; set; }
        public bool Simulated { get; set; }
        public string Company { get; set; }
        public bool IsFirstAccess { get; set; }
        public bool IsAdmin { get; set; }
    }
}
