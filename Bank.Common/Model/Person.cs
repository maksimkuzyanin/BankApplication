using System.Collections.Generic;
using Bank.Common.Abstraction;

namespace Bank.Common.Model
{
    public class Person : IEntity
    {
        public Person()
        {
            Passport = new Document();
            Changes = new List<PersonChanges>();
        }

        public Person(int id) : this()
        {
            Id = id;
        }

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
        ///     Паспорт
        /// </summary>
        public Document Passport { get; set; }

        /// <summary>
        ///     Список изменений
        /// </summary>
        public List<PersonChanges> Changes { get; set; }

        public BankClient BankClient { get; set; }

        public string Fio => (LastName + " " + FirstName + " " + MiddleName).Trim();

        /// <summary>
        ///     Идентификатор
        /// </summary>
        public int Id { get; set; }
    }
}