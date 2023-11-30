using System.Collections.Generic;
using System.Linq;
using Bank.Common.Model;

namespace Bank.Common.Dao
{
    public class PersonDao : EntityDao<Person>
    {
	    /// <summary>
	    ///     Путь к файлу на диске
	    /// </summary>
	    protected override string Connection => "persons.json";

	    /// <summary>
	    /// </summary>
	    /// <param name="id"></param>
	    /// <returns></returns>
	    public bool HasChanges(int id)
        {
            return GetChanges(id).Any();
        }

	    /// <summary>
	    /// </summary>
	    /// <param name="id"></param>
	    /// <returns></returns>
	    public IEnumerable<PersonChanges> GetChanges(int id)
        {
            return GetById(id).Changes;
        }
    }
}