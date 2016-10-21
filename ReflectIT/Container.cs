using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectIT
{
    public class Container
    {
        Dictionary<Type, Type> _map = new Dictionary<Type, Type>();

        // Helps with the code which knows the type at the compile time
        public ContainerBuilder For<TSource>()
        {
            return For(typeof(TSource));
        }

        // Helps with the code which either uses reflection, or knows the 
        // type only at runtime.
        public ContainerBuilder For(Type sourceType)
        {
            return new ContainerBuilder(this, sourceType);
        }

        public TSource Resolve<TSource>()
        {
            return (TSource)Resolve(typeof(TSource));
        }

        public object Resolve(Type sourceType)
        {
            if (_map.ContainsKey(sourceType))
            {
                var destinationType = _map[sourceType];
                return CreateInstance(destinationType);
            }
            else if (sourceType.IsGenericType &&
                    _map.ContainsKey(sourceType.GetGenericTypeDefinition()))
            {
                var destination = _map[sourceType.GetGenericTypeDefinition()];
                var closedType = destination.MakeGenericType(sourceType.GenericTypeArguments);
                return CreateInstance(closedType);
            }
            else if (!sourceType.IsAbstract)
            {
                return CreateInstance(sourceType);
            }

            else
            {
                throw new InvalidOperationException("Could not resolve " + sourceType.FullName);
            }
        }

        private object CreateInstance(Type destinationType)
        {
            var parameters = destinationType.GetConstructors()
                                            .OrderByDescending(c => c.GetParameters().Count())
                                            .First()
                                            .GetParameters()
                                            .Select(p => Resolve(p.ParameterType))
                                            .ToArray();

            return Activator.CreateInstance(destinationType, parameters);
        }

        public class ContainerBuilder
        {
            Container _container;
            Type _sourceType;

            public ContainerBuilder(Container container, Type sourceType)
            {
                _container = container;
                _sourceType = sourceType;
            }

            public ContainerBuilder Use<TDestination>()
            {
                return Use(typeof(TDestination));
            }

            public ContainerBuilder Use(Type destinationType)
            {
                _container._map.Add(_sourceType, destinationType);
                return this;
            }
        }
    }
}
