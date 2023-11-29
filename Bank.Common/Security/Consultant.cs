using Bank.Infrastructure.Common.Abstraction.Security.Principals;

namespace Bank.Common.Security
{
    public class Consultant : IBankPrincipal, IConsultantPrincipal
    {
        public int UserId { get; set; }

        public string NickName { get; set; }

        public virtual bool CheckSetFioAccess()
        {
            return false;
        }

        public virtual bool CheckSetPassportAccess()
        {
            return false;
        }

        public virtual bool CheckSetDepartmentAccess()
        {
            return false;
        }

        public string Name { get; set; }
    }
}