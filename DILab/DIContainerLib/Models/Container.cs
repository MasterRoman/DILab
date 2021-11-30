using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainerLib
{
    public class Container
    {
        public Type type { get; }
        public Lifetime lifetime { get; }

        public Container(Type type, Lifetime lifetime)
        {
            this.type = type;
            this.lifetime = lifetime;
        }
    }

    public enum Lifetime
    {
        Singleton,
        Instance
    }
}
