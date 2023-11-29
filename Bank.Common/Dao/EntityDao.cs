using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Bank.Common.Abstraction;
using Newtonsoft.Json;

namespace Bank.Common.Dao
{
    public abstract class EntityDao
    {
        protected EntityDao()
        {
            if (string.IsNullOrEmpty(Connection)) throw new Exception("connection is empty");

            Connect();
        }

        /// <summary>
        ///     Путь к файлу на диске
        /// </summary>
        protected virtual string Connection { get; set; }

        /// <summary>
        ///     Создает файл
        /// </summary>
        private void Connect()
        {
            if (!File.Exists(Connection))
                using (File.Create(Connection))
                {
                }
        }

        /// <summary>
        ///     Получает данные
        /// </summary>
        protected IEnumerable<T> Query<T>()
        {
            var source = new List<T>();

            using (var sr = new StreamReader(Connection))
            {
                var json = sr.ReadToEnd();
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented
                };
                source = JsonConvert.DeserializeObject<List<T>>(json, settings);
            }

            return source ?? new List<T>();
        }
    }

    public class EntityDao<T> : EntityDao, IEntityDao<T>
    {
        protected IEnumerable<T> Query => Query<T>();

        /// <summary>
        /// </summary>
        /// <param name="entity"></param>
        public void Save(T item)
        {
            if (item == null) throw new ArgumentNullException();

            var items = Query<T>().ToList();

            var idEntity = item as IEntity;

            if (idEntity == null)
            {
                // нужно определить методы сравнения в сущности
                if (!items.Contains(item)) items.Add(item);
            }
            else
            {
                var loaded = items.FirstOrDefault(x => ((IEntity) x).Id == idEntity.Id);
                if (loaded == null)
                {
                    idEntity.Id = NewId();
                    items.Add((T) idEntity);
                }
                else
                {
                    new CopyService().Copy(loaded, item);
                }
            }

            Save(items);
        }

        /// <summary>
        /// </summary>
        /// <param name="items"></param>
        public void Save(IEnumerable<T> items)
        {
            using (var file = new StreamWriter(Connection))
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Formatting.Indented
                };
                var serializer = JsonSerializer.Create(settings);
                serializer.Serialize(file, items);
            }
        }

        public T GetById(int id)
        {
            if (!typeof(IEntity).IsAssignableFrom(typeof(T)))
                throw new NotSupportedException("supported only for IEntity types");

            return Query<T>().FirstOrDefault(x => ((IEntity) x).Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return Query.ToList();
        }

        object IEntityDao.GetById(int id)
        {
            return GetById(id);
        }

        public void Save(object item)
        {
            Save((T) item);
        }

        public void Save(IEnumerable<object> items)
        {
            Save(items.Cast<T>());
        }

        IEnumerable<object> IEntityDao.GetAll()
        {
            return Query.ToList().Cast<object>();
        }

        public IEnumerable<T> Get(Func<T, bool> spec)
        {
            return Query<T>().Where(spec);
        }

        public IEnumerable<TOut> Get<TIn, TOut>(Func<TIn, bool> spec, Func<TIn, TOut> proj)
            where TIn : T
        {
            return Query<TIn>().Where(spec).Select(proj);
        }

        public IEnumerable<TOut> GetAll<TIn, TOut>(Func<TIn, TOut> proj)
            where TIn : T
        {
            return Query<TIn>().Select(proj);
        }

        public IEnumerable<T> GetByIds(int[] ids)
        {
            if (!typeof(IEntity).IsAssignableFrom(typeof(T)))
                throw new NotSupportedException("supported only for IEntity types");

            Func<T, bool> spec = x => ids.Any() && ids.Contains(((IEntity) x).Id);
            return Query<T>().Where(spec).ToList();
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private int NewId()
        {
            return Query.OrderBy(x => ((IEntity) x).Id).Select(x => ((IEntity) x).Id).LastOrDefault() + 1;
        }
    }

    internal class CopyService
    {
        public void Copy<T>(T dest, T src)
        {
            var typeDest = dest.GetType();
            var typeSrc = src.GetType();

            var srcProps = typeSrc.GetProperties();
            foreach (var srcProp in srcProps)
            {
                if (srcProp.Name == "Id") continue;
                if (!srcProp.CanRead) continue;

                var targetProperty = typeDest.GetProperty(srcProp.Name);
                if (targetProperty == null) continue;
                if (!targetProperty.CanWrite) continue;
                if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate) continue;
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0) continue;
                if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType)) continue;

                targetProperty.SetValue(dest, srcProp.GetValue(src, null), null);
            }
        }
    }
}