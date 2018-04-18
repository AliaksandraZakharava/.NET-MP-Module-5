using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace NETMP.Module5.IoCContainer.Tests
{
    public class ContainerTests
    {
        private readonly Container _container;
        private readonly Assembly _assembly;

        public ContainerTests()
        {
            _container = new Container();
            _assembly = Assembly.GetExecutingAssembly();
        }

        [Fact]
        public void RegisteringTypesOfAllAssembly_Test()
        {
            _container.RegisterAllMarkedAssemblyTypes(_assembly);

            var dependenciesToRegisterNumber = GetAssemblyMarkedDependenciesNumber(_assembly);
            var registeredDependenciesNumber = GetContainerDependenciesNumber(_container);

            Assert.Equal(dependenciesToRegisterNumber, registeredDependenciesNumber);
        }

        [Fact]
        public void RegisteringTypesManually_Test()
        {
            int dependenciesToRegisterNumber = 2;

            _container.RegisterType(typeof(IExportClass), typeof(ExportClassAsInterface));
            _container.RegisterTypeAsSelf(typeof(ClassWithConstructorImport));

            var registeredDependenciesNumber = GetContainerDependenciesNumber(_container);

            Assert.Equal(dependenciesToRegisterNumber, registeredDependenciesNumber);
        }

        [Fact]
        public void CreateTypeInstance_ExportTypes_Test()
        {
            _container.RegisterAllMarkedAssemblyTypes(_assembly);

            var exportClassAsSelf = _container.CreateInstance<ExportClassAsSelf>();
            var exportClassAsInterface = _container.CreateInstance<IExportClass>();

            Assert.NotNull(exportClassAsSelf);
            Assert.NotNull(exportClassAsInterface);

            Assert.IsType<ExportClassAsSelf>(exportClassAsSelf);
            Assert.True(exportClassAsInterface is IExportClass);
        }

        [Fact]
        public void CreateTypeInstance_ImportTypes_Test()
        {
            _container.RegisterAllMarkedAssemblyTypes(_assembly);

            var classWithConstructorImport = _container.CreateInstance<ClassWithConstructorImport>();
            var classWithPropertyImport = _container.CreateInstance<ClassWithPropertyImport>();

            Assert.NotNull(classWithConstructorImport);
            Assert.NotNull(classWithConstructorImport);

            Assert.IsType<ClassWithConstructorImport>(classWithConstructorImport);
            Assert.IsType<ClassWithPropertyImport>(classWithPropertyImport);
        }

        #region Private methods

        private int? GetContainerDependenciesNumber(Container container)
        {
            var fieldInfo = container.GetType().GetField("_dependencies", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField);

            var fieldValue = fieldInfo.GetValue(container) as Dictionary<Type, Type>;

            return fieldValue?.Count;
        }

        private int? GetAssemblyMarkedDependenciesNumber(Assembly assembly)
        {
            var typesToRegister = assembly.GetTypes().Where(type => type.IsDefined(typeof(InjectingDependenciesAttribute)) ||
                                                                    GetImportProperties(type).Any());

            return typesToRegister.Count();
        }

        private IEnumerable<PropertyInfo> GetImportProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                       .Where(property => Attribute.IsDefined(property, typeof(ImportPropertyAttribute)));
        }

        #endregion
    }
}
