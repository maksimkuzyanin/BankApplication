namespace Bank.Common.Model
{
	/// <summary>
	///     Связь клиента и отдела
	/// </summary>
	public class PersonDepartment
    {
        public PersonDepartment()
        {
        }

        public PersonDepartment(int personId, int departmentId)
        {
            PersonId = personId;
            DepartmentId = departmentId;
        }

        /// <summary>
        ///     Отдел
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        ///     Клиент
        /// </summary>
        public int PersonId { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as PersonDepartment);
        }

        public bool Equals(PersonDepartment other)
        {
            return !(other is null) &&
                   DepartmentId == other.DepartmentId &&
                   PersonId == other.PersonId;
        }

        public override int GetHashCode()
        {
            var hashCode = 1636326935;
            hashCode = hashCode * -1521134295 + DepartmentId.GetHashCode();
            hashCode = hashCode * -1521134295 + PersonId.GetHashCode();
            return hashCode;
        }
    }
}