namespace Bank.Infrastructure.Common.Abstraction.Security.Principals
{
    public interface IAccessPrincipal
    {
	    /// <summary>
	    ///     Проверяет доступ на изменение ФИО
	    /// </summary>
	    /// <returns></returns>
	    bool CheckSetFioAccess();

	    /// <summary>
	    ///     Проверяет доступ на изменение паспорта
	    /// </summary>
	    /// <returns></returns>
	    bool CheckSetPassportAccess();

	    /// <summary>
	    ///     Проверяет доступ на изменение отдела
	    /// </summary>
	    /// <returns></returns>
	    bool CheckSetDepartmentAccess();
    }
}