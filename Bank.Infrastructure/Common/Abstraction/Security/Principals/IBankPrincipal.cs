namespace Bank.Infrastructure.Common.Abstraction.Security.Principals
{
    public interface IBankPrincipal : IAccessPrincipal
    {
        int UserId { get; set; }

        string NickName { get; set; }
    }
}