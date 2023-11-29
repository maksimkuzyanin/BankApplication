namespace Bank.Common.Model
{
    public class PersonCommonChanges
    {
        public PersonCommonChanges(string field, string value)
        {
            Field = field;
            Value = value;
        }

        /// <summary>
        ///     Какие данные изменены
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        ///     Тип изменений
        /// </summary>
        public string Value { get; set; }
    }
}