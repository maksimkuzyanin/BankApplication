using System.Linq;
using Bank.Common.Model;

namespace Bank.Common.Dao
{
    public class PersonDepartmentDao : EntityDao<PersonDepartment>
    {
        protected override string Connection => "personsDepartments.json";

        /// <summary>
        /// </summary>
        /// <param name="personId"></param>
        public void Delete(int personId)
        {
            Save(Query.Where(x => x.PersonId != personId).ToList());
        }
    }
}