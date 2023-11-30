using System.Collections.Generic;
using Bank.Infrastructure.Common.Abstraction.Security.Principals;

namespace Bank.Infrastructure.Common.Implementation.Context
{
    public class ExecutionContext : IExecutionContext
    {
        public IBankPrincipal Principal { get; set; }

        public void SetPrincipal(IBankPrincipal principal)
        {
            Principal = principal;
        }

        public void CleanPrincipal()
        {
            Principal = null;
        }

        public List<string> Notifications { get; set; }
    }

    public interface IExecutionContext
    {
        IBankPrincipal Principal { get; set; }

        List<string> Notifications { get; set; }

        void SetPrincipal(IBankPrincipal principal);

        void CleanPrincipal();
    }
}