using System;
using System.Linq;
using Bank.Common.Dao;
using Bank.Common.DTO;
using Bank.Common.Model;

namespace Bank.Services
{
    public class DepartmentsService
    {
        private readonly DepartmentDao _departmentDao;
        private readonly PersonDepartmentDao _personDepartmentDao;

        public DepartmentsService()
        {
            _departmentDao = new DepartmentDao();
            _personDepartmentDao = new PersonDepartmentDao();
        }

        public NamedEntityDto[] GetDepartments()
        {
            Func<Department, bool> spec = x => true;
            Func<Department, NamedEntityDto> proj = x => new NamedEntityDto {Id = x.Id, Name = x.Name};

            var departments = _departmentDao.Get(spec, proj).ToArray();
            return departments;
        }

        public int GetDepartmentId(int personId)
        {
            Func<PersonDepartment, bool> spec = x => x.PersonId == personId;
            Func<PersonDepartment, int> proj = x => x.DepartmentId;

            var departmentId = _personDepartmentDao.Get(spec, proj).FirstOrDefault();
            return departmentId;
        }

        public NamedEntityDto GetDepartment(int personId)
        {
            var departmentId = GetDepartmentId(personId);

            Func<Department, bool> spec = x => x.Id == departmentId;
            Func<Department, NamedEntityDto> proj = x => new NamedEntityDto {Id = x.Id, Name = x.Name};

            var department = _departmentDao.Get(spec, proj).FirstOrDefault();
            return department;
        }

        public void SetDepartment(int personId, int departmentId)
        {
            var personDepartment = new PersonDepartment(personId, departmentId);
            _personDepartmentDao.Delete(personId);
            _personDepartmentDao.Save(personDepartment);
        }
    }
}