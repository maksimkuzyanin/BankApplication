using System;
using System.Collections.Generic;
using System.Linq;
using Bank.Common.Dao;
using Bank.Common.DTO;
using Bank.Common.Model;

namespace Bank.Services
{
    public class PersonsService
    {
        private readonly List<PersonCommonChanges> _commonChangesList;
        private readonly DepartmentsService _departmentsService;
        private readonly PersonDao _personDao;

        public PersonsService()
        {
            _commonChangesList = new List<PersonCommonChanges>();
            _departmentsService = new DepartmentsService();
            _personDao = new PersonDao();
        }

        public void SavePerson(string author, PersonDto dto)
        {
            var editMode = dto.Id > 0;

            var curPerson = _personDao.GetById(dto.Id);
            var setPerson = MapPerson(dto, curPerson?.Changes ?? new List<PersonChanges>());

            _personDao.Save(setPerson);

            if (editMode)
            {
                UpdateChangesHistory(curPerson, dto);
                CommitChangesHistory(author, dto.Id);
            }

            _departmentsService.SetDepartment(setPerson.Id, dto.Department.Id);
        }

        public bool HasChanges(int personId)
        {
            return _personDao.HasChanges(personId);
        }

        private void UpdateChangesHistory(Person person, PersonDto dto)
        {
            if (person.LastName != dto.LastName)
                _commonChangesList.Add(new PersonCommonChanges("Фамилия", dto.LastName));

            if (person.FirstName != dto.FirstName)
                _commonChangesList.Add(new PersonCommonChanges("Имя", dto.FirstName));

            if (person.MiddleName != dto.MiddleName)
                _commonChangesList.Add(new PersonCommonChanges("Отчество", dto.MiddleName));

            if (person.Phone != dto.Phone) _commonChangesList.Add(new PersonCommonChanges("Телефон", dto.Phone));

            if (person.Passport.Series != dto.Series)
                _commonChangesList.Add(new PersonCommonChanges("Паспорт.Серия", dto.Series));

            if (person.Passport.Number != dto.Number)
                _commonChangesList.Add(new PersonCommonChanges("Паспорт.Номер", dto.Number));

            var curDepartment = _departmentsService.GetDepartment(person.Id);
            var setDepartment = dto.Department;

            if (curDepartment.Id != setDepartment.Id)
                _commonChangesList.Add(new PersonCommonChanges("Отдел", setDepartment.Name));
        }

        private void CommitChangesHistory(string author, int editedPersonId)
        {
            if (!_commonChangesList.Any()) return;

            var curPerson = _personDao.GetById(editedPersonId);

            var date = DateTime.Now;
            var commonChanges = _commonChangesList.ToArray();

            curPerson.Changes.Add(new PersonChanges(date, commonChanges, author));

            _personDao.Save(curPerson);
        }

        private Person MapPerson(PersonDto dto, List<PersonChanges> changes)
        {
            return new Person
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MiddleName = dto.MiddleName,
                Phone = dto.Phone,
                Passport = {Series = dto.Series, Number = dto.Number},
                Changes = changes
            };
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public NamedEntityDto[] GetPersons(int[] personId = null)
        {
            Func<Person, bool> spec = x => true;
            Func<Person, NamedEntityDto> proj = x => new NamedEntityDto {Id = x.Id, Name = x.Fio};

            if (personId != null) spec = x => personId.Contains(x.Id);

            var result = new PersonDao().Get(spec, proj).ToArray();
            return result.ToArray();
        }

        /// <summary>
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public NamedEntityDto GetPersonByAccountId(int accountId)
        {
            var personDao = new PersonDao();
            var accountDao = new AccountDao();
            var account = accountDao.GetById(accountId);

            Func<Person, bool> spec = x => x.Id == account?.PersonId;
            Func<Person, NamedEntityDto> proj = x => new NamedEntityDto {Id = x.Id, Name = x.Fio};

            var person = personDao.Get(spec, proj).FirstOrDefault();
            return person;
        }
    }
}