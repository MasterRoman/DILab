using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainerLib.Configuration
{
    public class DependenciesConfiguration: IConfiguration
    {
        public Dictionary<Type, List<Container>> dependencies { get; private set; }

        public DependenciesConfiguration()
        {
            this.dependencies = new Dictionary<Type, List<Container>>();
        }

        public void register<TInterface, TImplementation>(Lifetime lifetime)
            where TInterface : class
            where TImplementation : TInterface
        {
            register(typeof(TInterface), typeof(TImplementation),lifetime);
        }

        public void register(Type interfaceType, Type implementationType,Lifetime lifetime)
        {
            if (!isDependency(interfaceType, implementationType))
            {
                throw new ArgumentException("Incompatible parameters");
            }

            var container = new Container(implementationType, lifetime);
            if (!dependencies.ContainsKey(interfaceType))
            {
                var list = new List<Container>();
                list.Add(container);
                dependencies.Add(interfaceType, list);
            }
            else
            {
                var index = dependencies[interfaceType].FindIndex(elem => elem.type == container.type);
                if (index != -1)
                {
                    dependencies[interfaceType].RemoveAt(index);
                }

                dependencies[interfaceType].Add(container);
            }
        }

        private bool isDependency(Type interfaceType, Type implementationType)
        {
            return implementationType.GetInterfaces().Any(i => i.ToString() == interfaceType.ToString()) || implementationType.IsAssignableFrom(interfaceType);
        }
    }
}
