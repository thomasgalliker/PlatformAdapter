using System;
using System.Linq;
using System.Reflection;

using CrossPlatformAdapter;
using CrossPlatformAdapter.Exceptions;

using FluentAssertions;

using Moq;

using PlatformAdapter.Tests.PlatformDemoAbstraction;
using PlatformAdapter.Tests.PlatformDemoAssembly;

using Xunit;

namespace PlatformAdapter.Tests
{
    public class ProbingAdapterResolverTests
    {
        [Fact]
        public void ShouldResolvePlatformSpecificObjectForInterface()
        {
            // Arrange
            var testRegistrationConvention = new TestProbingStrategy();
            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(testRegistrationConvention);
            var interfaceToResolve = typeof(IDemoService);

            // Act
            var classType = probingAdapterResolver.Resolve(interfaceToResolve);

            // Assert
            classType.Should().BeOfType<DemoService>();
        }

        [Fact]
        public void ShouldResolvePlatformSpecificObjectForInterfaceGeneric()
        {
            // Arrange
            var testRegistrationConvention = new TestProbingStrategy();
            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(testRegistrationConvention);

            // Act
            var classType = probingAdapterResolver.Resolve<IDemoService>();

            // Assert
            classType.Should().BeOfType<DemoService>();
        }

        [Fact]
        public void ShouldReturnNullWhenTryResolveFails()
        {
            // Arrange
            var testRegistrationConvention = new TestProbingStrategy();
            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(testRegistrationConvention);
            var interfaceToResolve = typeof(IDemoServiceWithNoImplementation);

            // Act
            var instance = probingAdapterResolver.TryResolve(interfaceToResolve);

            // Assert
            instance.Should().BeNull();
        }

        [Fact]
        public void ShouldReturnNullWhenTryResolveFailsGeneric()
        {
            // Arrange
            var testRegistrationConvention = new TestProbingStrategy();
            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(testRegistrationConvention);

            // Act
            var instance = probingAdapterResolver.TryResolve<IDemoServiceWithNoImplementation>();

            // Assert
            instance.Should().BeNull();
        }

        [Fact]
        public void ShouldResolvePlatformSpecificClassTypeForInterface()
        {
            // Arrange
            var testRegistrationConvention = new TestProbingStrategy();
            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(testRegistrationConvention);
            var interfaceToResolve = typeof(IDemoService);

            // Act
            var classType = probingAdapterResolver.ResolveClassType(interfaceToResolve);

            // Assert
            classType.Should().Be<DemoService>();
        }

        [Fact]
        public void ShouldResolvePlatformSpecificClassTypeForInterfaceGeneric()
        {
            // Arrange
            var testRegistrationConvention = new TestProbingStrategy();
            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(testRegistrationConvention);

            // Act
            var classType = probingAdapterResolver.ResolveClassType<IDemoService>();

            // Assert
            classType.Should().Be<DemoService>();
        }

        ////[Fact]
        ////public void ShouldOverrideDefaultRegistrationConvention()
        ////{
        ////    // Arrange
        ////    var registrationConventionMock = new Mock<IProbingStrategy>();
        ////    IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(registrationConventionMock.Object);
        ////    var interfaceToResolve = typeof(IDemoService);

        ////    // Act
        ////    probingAdapterResolver.ProbingStrategy = new TestProbingStrategy();

        ////    // Assert
        ////    var classType = probingAdapterResolver.ResolveClassType(interfaceToResolve);
        ////    classType.Should().Be<DemoService>();
        ////}

        [Fact]
        public void ShouldReturnNullWhenTryResolveClassTypeFails()
        {
            // Arrange
            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(); // Default ctor uses DefaultProbingStrategy which doesnt work with these unit tests
            var interfaceToResolve = typeof(IDemoService);

            // Act
            var classType = probingAdapterResolver.TryResolveClassType(interfaceToResolve);

            // Assert
            classType.Should().BeNull();
        }

        [Fact]
        public void ShouldReturnNullWhenTryResolveClassTypeFailsGeneric()
        {
            // Arrange
            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(); // Default ctor uses DefaultProbingStrategy which doesnt work with these unit tests

            // Act
            var classType = probingAdapterResolver.TryResolveClassType<IDemoService>();

            // Assert
            classType.Should().BeNull();
        }

        [Fact]
        public void ShouldThrowPlatformSpecificAssemblyNotFoundExceptionIfUnableToProbeForPlatformSpecificAssembly()
        {
            // Arrange
            var registrationConventionMock = new Mock<IProbingStrategy>();
            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(registrationConventionMock.Object);
            var interfaceToResolve = typeof(IDemoService);

            // Act
            Action resolveAction = () => probingAdapterResolver.ResolveClassType(interfaceToResolve);

            // Assert
            var aggregateException = Assert.Throws<AggregateException>(resolveAction);
            aggregateException.InnerExceptions.Should().HaveCount(1);
            aggregateException.InnerExceptions.Should().ContainItemsAssignableTo<PlatformSpecificAssemblyNotFoundException>();
        }

        [Fact]
        public void ShouldThrowPlatformSpecificTypeNotFoundExceptionIfUnableToFindTargetClassInAssembly()
        {
            // Arrange
            var probingStrategy = new Mock<IProbingStrategy>();
            probingStrategy.Setup(strategy => strategy.PlatformNamingConvention(It.IsAny<AssemblyName>())).Returns((AssemblyName assemblyName) => assemblyName.Name);
            probingStrategy.Setup(strategy => strategy.InterfaceToClassNamingConvention(It.IsAny<Type>())).Returns((Type t) => "TypeWhichDoesNotExist");


            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(probingStrategy.Object);
            var interfaceToResolve = typeof(IDemoServiceWithNoImplementation);

            // Act
            Action resolveAction = () => probingAdapterResolver.ResolveClassType(interfaceToResolve);

            // Assert
            var aggregateException = Assert.Throws<AggregateException>(resolveAction);
            aggregateException.InnerExceptions.Should().HaveCount(1);
            aggregateException.InnerExceptions.ElementAt(0).Should().BeOfType<PlatformSpecificTypeNotFoundException>();
        }

        [Fact]
        public void ShouldThrowMultipleExceptionsIfMoreThanOneProblemOccurs()
        {
            // Arrange
            var probingStrategy1 = new Mock<IProbingStrategy>();
            probingStrategy1.Setup(strategy => strategy.PlatformNamingConvention(It.IsAny<AssemblyName>())).Returns((AssemblyName assemblyName) => assemblyName.Name + ".NonExistentAssembly");
            probingStrategy1.Setup(strategy => strategy.InterfaceToClassNamingConvention(It.IsAny<Type>())).Returns((Type t) => "TypeWhichDoesNotExist");

            var probingStrategy2 = new Mock<IProbingStrategy>();
            probingStrategy2.Setup(strategy => strategy.PlatformNamingConvention(It.IsAny<AssemblyName>())).Returns((AssemblyName assemblyName) => assemblyName.Name);
            probingStrategy2.Setup(strategy => strategy.InterfaceToClassNamingConvention(It.IsAny<Type>())).Returns((Type t) => "TypeWhichDoesNotExist");


            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(probingStrategy1.Object, probingStrategy2.Object);
            var interfaceToResolve = typeof(IDemoServiceWithNoImplementation);

            // Act
            Action resolveAction = () => probingAdapterResolver.ResolveClassType(interfaceToResolve);

            // Assert
            var aggregateException = Assert.Throws<AggregateException>(resolveAction);
            aggregateException.InnerExceptions.Should().HaveCount(2); // Because we injected two different probing strategies
            aggregateException.InnerExceptions.ElementAt(0).Should().BeOfType<PlatformSpecificAssemblyNotFoundException>();
            aggregateException.InnerExceptions.ElementAt(1).Should().BeOfType<PlatformSpecificTypeNotFoundException>();
        }
    }
}