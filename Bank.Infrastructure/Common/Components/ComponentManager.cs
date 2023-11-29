using System;
using System.Collections;
using System.Linq;

namespace Bank.Infrastructure.Common.Components
{
	/// <summary>
	///     Отвечает за регистрацию и разрешение компонент
	/// </summary>
	public class ComponentManager
    {
        private static ComponentManager _instance;

        private readonly ArrayList _components;

        private ComponentManager()
        {
            _components = new ArrayList();
        }

        public static ComponentManager Instance
        {
            get
            {
                if (_instance == null) _instance = new ComponentManager();
                return _instance;
            }
        }

        public bool HasComponent<TIn>()
        {
            return _components.ToArray().Where(x => x is TIn).Any();
        }

        /// <summary>
        ///     todo: метод неуниверсальный, так у конструктора могут быть параметры
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        public void Register<TIn, TOut>()
            where TIn : class
            where TOut : TIn
        {
            var instance = (TIn) Activator.CreateInstance(typeof(TOut));
            _components.Add(instance);
        }

        public TIn Resolve<TIn>()
        {
            var instance = _components.ToArray().Where(x => x is TIn).FirstOrDefault();
            return (TIn) instance;
        }
    }
}