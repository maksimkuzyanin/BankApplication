using System.Linq;
using Bank.Common.Model;

namespace Bank.Common.Dao
{
    public class DepartmentDao : EntityDao<Department>
    {
        protected override string Connection => "departments.json";

        /// <summary>
        ///     Ищет отдел по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Department GetByName(string name)
        {
            return Query.Where(x => x.Name == name).FirstOrDefault();
        }
    }
}