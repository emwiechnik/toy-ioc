using IoC.Tests.TestTypes;
using System;
using Xunit;

namespace IoC.Tests
{
    public class IoCTests
    {
        [Fact]
        public void Can_Resolve_A_Simple_Type()
        {
            // Arrange
            var ioc = new Container();
            ioc.For<IObject>().Use<ConcreteObject2>();

            // Act
            var logger = ioc.Resolve<IObject>();

            // Assert
            Assert.Equal(typeof(ConcreteObject2), logger.GetType());
        }

        [Fact]
        public void Throws_Proper_Exception_If_Not_Able_To_Resolve_A_Simple_Type()
        {
            // Arrange
            var ioc = new Container();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => ioc.Resolve<IObject>());
        }

        [Fact]
        public void Can_Resolve_A_Type_Without_A_Default_Ctor()
        {
            // Arrange
            var ioc = new Container();
            ioc.For<IObject>().Use<ConcreteObject2>();
            ioc.For<IWrapper<ConcreteObject1>>().Use<ConcreteWrapper<ConcreteObject1>>();

            // Act
            var repository = ioc.Resolve<IWrapper<ConcreteObject1>>();

            // Assert
            Assert.Equal(typeof(ConcreteWrapper<ConcreteObject1>), repository.GetType());
        }

        [Fact]
        public void Can_Resolve_A_Concrete_Type_That_Has_Not_Been_Registered_With_The_Container_As_Long_As_Its_Dependencies_Have_Been()
        {
            // Arrange
            var ioc = new Container();
            ioc.For<IObject>().Use<ConcreteObject2>();
            ioc.For(typeof(IWrapper<>)).Use(typeof(ConcreteWrapper<>));

            // Act
            var service = ioc.Resolve<ComplexConcreteObject>();

            // Assert
            Assert.Equal(typeof(ComplexConcreteObject), service.GetType());
        }
    }
}
