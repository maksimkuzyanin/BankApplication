using Bank.Common.Abstraction;
using Newtonsoft.Json;

namespace Bank.Common.Model
{
    public class Department : IEntity
    {
        public string Name { get; set; }

        [JsonIgnore] public string Address { get; set; }

        public Department MainDepartment { get; set; }
        public int Id { get; set; }
    }
}