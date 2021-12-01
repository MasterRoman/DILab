using NUnit.Framework;

using DIContainerLib;
using DIContainerLib.Configuration;
using DIContainerLib.Provider;
using DIContainerLib.Validation;

using Tests.TestClasses;

namespace Tests
{
 
    public class Tests
    {
        IConfiguration configuration;

        [SetUp]
        public void Setup()
        {
            this.configuration = new DependenciesConfiguration();
        }

        [Test]
        public void TestSimpleDependencyWithSingleton()
        {

            configuration.register<IClass,TestClass>(Lifetime.Singleton);
            IProvider provider = new DependencyProvider(configuration);

            var instance = provider.Resolve(typeof(IClass));
            var senondInstanse = provider.Resolve(typeof(IClass));

            Assert.AreEqual(instance, senondInstanse);
         
           
        }

        [Test]
        public void TestSimpleDependencyWithInstanse()
        {

            configuration.register<IClass, TestClass>(Lifetime.Instance);
            IProvider provider = new DependencyProvider(configuration);

            var instance = provider.Resolve(typeof(IClass));
            var senondInstanse = provider.Resolve(typeof(IClass));

            Assert.AreNotEqual(instance, senondInstanse);

        }

        [Test]
        public void TestOpenGenerics()
        {
            configuration.register(typeof(p3<>), typeof(TestClass3<>), Lifetime.Instance);
            configuration.register<IClass,TestClass>(Lifetime.Instance);
            IProvider provider = new DependencyProvider(configuration);

            var instance = provider.Resolve<p3<int>>();
            TestClass3<int> castedClass = (TestClass3<int>)instance;
           
            Assert.AreEqual(castedClass.cl.a, 5);
            
        }

        [Test]
        public void TestValidation()
        {
            configuration.register(typeof(p3<>), typeof(TestClass3<>), Lifetime.Instance);
            var validator = new Validator(configuration);
            
            var result = validator.isValid();
            Assert.AreEqual(result, false);


            configuration.register(typeof(p3<>), typeof(TestClass3<>), Lifetime.Instance);
            configuration.register<IClass, TestClass>(Lifetime.Instance);
            validator = new Validator(configuration);

            result = validator.isValid();
            Assert.AreEqual(result, true);

        }
    }
}