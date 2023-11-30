using System.Collections.Generic;

namespace Bank.Common.Abstraction
{
    public interface IEntityDao<T> : IEntityDao
    {
        T GetById(int id);

        void Save(T item);

        void Save(IEnumerable<T> items);

        IEnumerable<T> GetAll();
    }

    public interface IEntityDao
    {
        object GetById(int id);

        void Save(object item);

        void Save(IEnumerable<object> items);

        IEnumerable<object> GetAll();
    }
}