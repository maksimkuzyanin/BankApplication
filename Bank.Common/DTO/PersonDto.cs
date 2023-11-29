namespace Bank.Common.DTO
{
    public class PersonDto
    {
	    /// <summary>
	    ///     Идентификатор
	    /// </summary>
	    public int Id { get; set; }

	    /// <summary>
	    ///     Фамилия
	    /// </summary>
	    public string LastName { get; set; }

	    /// <summary>
	    ///     Имя
	    /// </summary>
	    public string FirstName { get; set; }

	    /// <summary>
	    /// </summary>
	    public string MiddleName { get; set; }

	    /// <summary>
	    ///     Номер телефона
	    /// </summary>
	    public string Phone { get; set; }

	    /// <summary>
	    ///     Серия
	    /// </summary>
	    public string Series { get; set; }

	    /// <summary>
	    ///     Номер
	    /// </summary>
	    public string Number { get; set; }

	    /// <summary>
	    ///     Департамент
	    /// </summary>
	    public NamedEntityDto Department { get; set; }
    }
}