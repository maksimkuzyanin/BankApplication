using Bank.Infrastructure.Common.Abstraction.Security.Principals;

namespace Bank.Common.Security
{
    public class Manager : Consultant, IManagerPrincipal
    {
        public string Name { get; set; }

        public override bool CheckSetFioAccess()
        {
            return true;
        }

        public override bool CheckSetPassportAccess()
        {
            return true;
        }

        public override bool CheckSetDepartmentAccess()
        {
            return true;
        }
    }
}