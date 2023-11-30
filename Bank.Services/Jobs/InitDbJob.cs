using System.Collections.Generic;
using Bank.Common.Dao;
using Bank.Common.Model;
using FluentScheduler;

namespace Bank.Services.Jobs
{
    public class InitDbJob : IJob
    {
        public void Execute()
        {
            InitDepartments();
        }

        private void InitDepartments()
        {
            var collection = new List<Department>
            {
                new Department {Name = "Отдел работы с обычными клиентами"},
                new Department {Name = "Отдел работы с VIP клиентами"},
                new Department {Name = "Отдел работы с юридическими лицами"}
            };

            var dao = new DepartmentDao();
            foreach (var department in collection)
                if (dao.GetByName(department.Name) == null)
                    dao.Save(department);
        }
    }
}