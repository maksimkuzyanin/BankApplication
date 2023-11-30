using System.Web.Caching;

namespace Bank.Infrastructure.Helpers
{
    public class CacheService
    {
        private static Cache _instance;

        public static Cache GetCache()
        {
            return _instance ?? (_instance = new Cache());
        }
    }

    public class States
    {
        public const string PersonId = "PersonId";
    }
}