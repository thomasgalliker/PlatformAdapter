﻿using System;
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
            var testRegistrationConvention = new TestRegistrationConvention();
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
            var testRegistrationConvention = new TestRegistrationConvention();
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
            var testRegistrationConvention = new TestRegistrationConvention();
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
            var testRegistrationConvention = new TestRegistrationConvention();
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
            var testRegistrationConvention = new TestRegistrationConvention();
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
            var testRegistrationConvention = new TestRegistrationConvention();
            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(testRegistrationConvention);

            // Act
            var classType = probingAdapterResolver.ResolveClassType<IDemoService>();

            // Assert
            classType.Should().Be<DemoService>();
        }

        [Fact]
        public void ShouldOverrideDefaultRegistrationConvention()
        {
            // Arrange
            var registrationConventionMock = new Mock<IRegistrationConvention>();
            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(registrationConventionMock.Object);
            var interfaceToResolve = typeof(IDemoService);

            // Act
            probingAdapterResolver.RegistrationConvention = new TestRegistrationConvention();

            // Assert
            var classType = probingAdapterResolver.ResolveClassType(interfaceToResolve);
            classType.Should().Be<DemoService>();
        }

        [Fact]
        public void ShouldReturnNullWhenTryResolveClassTypeFails()
        {
            // Arrange
            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(); // Default ctor uses DefaultRegistrationConvention which doesnt work with these unit tests
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
            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(); // Default ctor uses DefaultRegistrationConvention which doesnt work with these unit tests

            // Act
            var classType = probingAdapterResolver.TryResolveClassType<IDemoService>();

            // Assert
            classType.Should().BeNull();
        }

        [Fact]
        public void ShouldThrowPlatformNotSupportedExceptionIfUnableToProbeForPlatformSpecificAssembly()
        {
            // Arrange
            var registrationConventionMock = new Mock<IRegistrationConvention>();
            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(registrationConventionMock.Object);
            var interfaceToResolve = typeof(IDemoService);

            // Act
            Action resolveAction = () => probingAdapterResolver.ResolveClassType(interfaceToResolve);

            // Assert
            Assert.Throws<PlatformSpecificAssemblyNotFoundException>(resolveAction);
        }

        [Fact]
        public void ShouldThrowPlatformNotSupportedExceptionIfUnableToFindTargetClassInAssembly()
        {
            // Arrange
            var registrationConventionMock = new Mock<IRegistrationConvention>();
            registrationConventionMock.Setup(registrationConvention => registrationConvention.PlatformNamingConvention(It.IsAny<AssemblyName>()))
                .Returns((AssemblyName assemblyName) => assemblyName.Name);
            registrationConventionMock.Setup(registrationConvention => registrationConvention.InterfaceToClassNamingConvention(It.IsAny<Type>()))
                .Returns((Type t) => "TypeWhichDoesNotExist");

            IAdapterResolver probingAdapterResolver = new ProbingAdapterResolver(registrationConventionMock.Object);
            var interfaceToResolve = typeof(IDemoServiceWithNoImplementation);

            // Act
            Action resolveAction = () => probingAdapterResolver.ResolveClassType(interfaceToResolve);

            // Assert
            Assert.Throws<PlatformSpecificTypeNotFoundException>(resolveAction);
        }
    }
}