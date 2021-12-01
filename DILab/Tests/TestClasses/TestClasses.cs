using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestClasses
{
    public interface IClass { int a { get; } };

    public class TestClass : IClass
    {
        public int a { get; }

        public TestClass()
        {
            this.a = 5;
        }
    }

    public interface INewnterface
    {
        void doJob();
    }

    public class TestClass2 : INewnterface
    {

        public IClass cl;

        public TestClass2(IClass cl)
        {
            this.cl = cl;
        }

        public void doJob()
        {
            
        }
    }

    public interface p3<T> { };

    public class TestClass3<T> : TestClass2, p3<T>
    {

        public TestClass3(IClass cl) : base(cl)
        {
            
        }
    }

 
}
