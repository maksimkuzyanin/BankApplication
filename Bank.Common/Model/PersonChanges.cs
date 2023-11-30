using System;

namespace Bank.Common.Model
{
    public class PersonChanges
    {
        public PersonChanges()
        {
        }

        public PersonChanges(DateTime date, PersonCommonChanges[] commonChanges, string author)
        {
            Date = date;
            CommonChanges = commonChanges;
            Author = author;
        }

        /// <summary>
        ///     Дата и время изменения записи
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// </summary>
        public PersonCommonChanges[] CommonChanges { get; set; }

        /// <summary>
        ///     Автор изменений (консультант или менеджер)
        /// </summary>
        public string Author { get; set; }
    }
}