using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainerLib.Configuration
{
    public interface IConfiguration
    {
        Dictionary<Type, List<Container>> dependencies { get; }

        void register<TInterface, TImplementation>(Lifetime lifetime)  
            where TInterface : class
            where TImplementation : TInterface;

        void register(Type interfaceType, Type implementationType, Lifetime lifetime);
    }
}
