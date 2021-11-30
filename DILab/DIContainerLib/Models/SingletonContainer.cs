using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIContainerLib.Models
{
    public class SingletonContainer
    {
        public object instance { get; }

        public SingletonContainer(object instance)
        {
            this.instance = instance;
        }
    }
}
