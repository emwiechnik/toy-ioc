using System;
using System.Collections.Generic;
using System.Linq;

namespace IoC
{
    public class Container
    {
        private readonly Dictionary<Type, Type> mappings = new Dictionary<Type, Type>();

        public ContainerBuilder For<TSource>()
        {
            return For(typeof(TSource));
        }

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
            if (mappings.ContainsKey(sourceType))
            {
                var destinationType = mappings[sourceType];
                return CreateInstance(destinationType);
            }

            if (sourceType.IsGenericType && this.mappings.ContainsKey(sourceType.GetGenericTypeDefinition()))
            {
                var destinationType = this.mappings[sourceType.GetGenericTypeDefinition()];
                var closedDestination = destinationType.MakeGenericType(sourceType.GenericTypeArguments);
                return CreateInstance(closedDestination);
            }

            if (!sourceType.IsAbstract)
            {
                return CreateInstance(sourceType);
            }

            throw new InvalidOperationException($"Could not resolve the type {sourceType.Name}");
        }

        private object CreateInstance(Type destinationType)
        {
            var ctors = destinationType.GetConstructors();

            var ctor = ctors.OrderByDescending(c => c.GetParameters().Count()).First(); // assumption that there needs to be at least one public constructor

            var resolvedParams = ctor.GetParameters().Select(p => Resolve(p.ParameterType)).ToArray(); // important: Activator.CreateInstance expects it to be an array! Otherwise if we provide a non-array collection we will get a very unintuitive error message: Constructor on type '..' not found

            return Activator.CreateInstance(destinationType, resolvedParams);
        }

        public class ContainerBuilder
        {
            private readonly Container container;
            private readonly Type sourceType;

            public ContainerBuilder(Container container, Type sourceType)
            {
                this.container = container;
                this.sourceType = sourceType;
            }

            public ContainerBuilder Use<TDestination>()
            {
                return Use(typeof(TDestination));
            }

            public ContainerBuilder Use(Type destinationType)
            {
                this.container.mappings.Add(this.sourceType, destinationType);

                return this;
            }
        }
    }
}
