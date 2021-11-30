using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainerLib.Provider
{
    public interface IProvider
    {
        TInterface Resolve<TInterface>()
            where TInterface : class;

        object Resolve(Type interfaceType);
    }
}
