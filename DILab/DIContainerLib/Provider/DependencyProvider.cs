using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DIContainerLib.Configuration;
using DIContainerLib.Models;

using DIContainerLib.Validation;

namespace DIContainerLib.Provider
{
    public class DependencyProvider: IProvider
    {
        private readonly IConfiguration configuration;
        private readonly Dictionary<Type, List<SingletonContainer>> singletons;

        public DependencyProvider(IConfiguration configuration)
        {
            IValidator validator = new Validator(configuration);
            if (!validator.isValid())
            {
                throw new ArgumentException("Configuration error");
            }

            this.configuration = configuration;
            this.singletons = new Dictionary<Type, List<SingletonContainer>>();
        }

        public TInterface Resolve<TInterface>()
            where TInterface : class
        {
            return (TInterface)Resolve(typeof(TInterface));
        }

        public object Resolve(Type interfaceType)
        {
            object result;
            Container container = getImplementationByType(interfaceType);
            Type generatedType = getGeneratedType(interfaceType, container.type);
             result = resolveType(interfaceType, generatedType, container.lifetime);

            return result;
        }

        private object resolveType(Type interfaceType,Type generatedType, Lifetime lifetime)
        {
            if (lifetime != Lifetime.Singleton)
            {
                return createInstance(generatedType);
            }

            lock (configuration)
            {
                if (isSingleton(interfaceType, generatedType))
                {
                    return singletons[interfaceType].First().instance;
                }

                var instance = createInstance(generatedType);
                addSingleton(interfaceType, instance);
                return instance;
            }
        }

        private Container getImplementationByType(Type interfaceType)
        {
            Container container;
            if (interfaceType.IsGenericType)
            {
                container = getContainer(interfaceType);
                if (container == null) {
                    container = getContainer(interfaceType.GetGenericTypeDefinition());
                }
            }
            else
            {
                container = getContainer(interfaceType);
            }

            return container;
        }

        private object createInstance(Type implementationType)
        {
            var constructors = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            foreach (var constructor in constructors)
            {
                var constructorParams = constructor.GetParameters();
                var generatedParams = new List<dynamic>();
                foreach (var parameterInfo in constructorParams)
                {
                    dynamic parameter;
                    if (parameterInfo.ParameterType.IsInterface)
                    {
                        parameter = Resolve(parameterInfo.ParameterType);
                    }
                    else
                    {
                        break;
                    }

                    generatedParams.Add(parameter);
                }

                return constructor.Invoke(generatedParams.ToArray());
            }

            throw new ArgumentException("Can't create instance of class");
        }

        private Type getGeneratedType(Type interfaceType, Type implementationType)
        {
            if (interfaceType.IsGenericType && implementationType.IsGenericTypeDefinition)
            {
                return implementationType.MakeGenericType(interfaceType.GetGenericArguments());
            }

            return implementationType;
        }

        private Container getContainer(Type interfaceType)
        {
            if (configuration.dependencies.ContainsKey(interfaceType))
            {
                return this.configuration.dependencies[interfaceType].First();//.FindLast(container => number.HasFlag(container.ImplNumber));
            }

            return null;
        }

        private void addSingleton(Type dependencyType, object implementation)
        {
            if (singletons.ContainsKey(dependencyType))
            {
                singletons[dependencyType].Add(new SingletonContainer(implementation));
            }
            else
            {
                singletons.Add(dependencyType, new List<SingletonContainer>()
                {
                    new SingletonContainer(implementation)
                });
            }
        }

        private bool isSingleton(Type interfaceType, Type implementationType)
        {
            var singleton = singletons.ContainsKey(interfaceType) ? this.singletons[interfaceType] : null;
            return !(singleton?.Find(container => container.instance.GetType() == implementationType) is null);
        }
    }
}
