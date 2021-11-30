using System;
using System.Collections.Generic;
using System.Linq;

using System.Reflection;
using DIContainerLib.Configuration;

namespace DIContainerLib.Validation
{
    public class Validator: IValidator
    {
        private readonly Stack<Type> nestedTypes;
        private readonly IConfiguration configuration;

        public Validator(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.nestedTypes = new Stack<Type>();
        }

        private bool validate(Type instanceType)
        {
            nestedTypes.Push(instanceType);
            var constructors = instanceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();
                foreach (var parameter in parameters)
                {
                    Type parameterType;
                    if (parameter.ParameterType.ContainsGenericParameters)
                    {
                        parameterType = parameter.ParameterType.GetInterfaces()[0];
                    }
                    else
                    {
                        parameterType = parameter.ParameterType;
                    }

                    if (parameterType.IsInterface && configuration.dependencies.ContainsKey(parameterType)) continue;
                    this.nestedTypes.Pop();
                    return false;
                }
            }

            this.nestedTypes.Pop();
            return true;
        }

        public bool isValid()
        {
            return this.configuration.dependencies.Values.All(implementations => implementations.
            All(implementation => validate(implementation.type)));
        }
    }
}
