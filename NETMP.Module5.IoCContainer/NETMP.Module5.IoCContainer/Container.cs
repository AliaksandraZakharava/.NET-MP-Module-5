using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NETMP.Module5.IoCContainer
{
    public class Container
    {
        private readonly Dictionary<Type, Type> _dependencies;

        public Container()
        {
            _dependencies = new Dictionary<Type, Type>();
        }

        public void RegisterType(Type exportType, Type importType)
        {
            if (exportType == null)
            {
                throw new ArgumentNullException(nameof(exportType));
            }

            if (importType == null)
            {
                throw new ArgumentNullException(nameof(importType));
            }

            if (_dependencies.ContainsKey(importType))
            {
                throw new InvalidOperationException("Trying to register type, that has been already registered.");
            }

            _dependencies.Add(importType, exportType);
        }

        public void RegisterTypeAsSelf(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            RegisterType(type, type);
        }
        
        public void RegisterAllMarkedAssemblyTypes(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var typesToRegister = assembly.GetTypes().Where(type => type.IsDefined(typeof(InjectingDependenciesAttribute)) ||
                                                                    GetImportProperties(type).Any());

            foreach (var type in typesToRegister)
            {
                var injectAttr = (InjectingDependenciesAttribute)type.GetCustomAttributes()
                                                                     .SingleOrDefault(attr => attr is InjectingDependenciesAttribute);

                if (injectAttr == null || injectAttr.Contract == null)
                {
                    RegisterTypeAsSelf(type);
                }
                else
                {
                    RegisterType(type, injectAttr.Contract);
                }
            }
        }
        
        public object CreateInstance(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!_dependencies.ContainsKey(type))
            {
                throw new InvalidOperationException($"Type {type} is not registered.");
            }

            var exportType = _dependencies[type];

            if (HasTypeConstructorImports(exportType))
            {
                return CreateInstanceWithConstructorDependencies(exportType);
            }

            if (HasTypePropertyImports(exportType))
            {
                return CreateInstanceWithPropertyDependencies(exportType);
            }

            return Activator.CreateInstance(exportType);
        }

        public T CreateInstance<T>()
        {
            var type = typeof(T);

            return (T)CreateInstance(type);
        }

        #region Private methods

        private object CreateInstanceWithConstructorDependencies(Type type)
        {
            var constructors = type.GetConstructors();

            if (constructors.Length > 1)
            {
                throw new InvalidOperationException("Can not create instance of a type having several constructors.");
            }

            var ctrParams = constructors.First().GetParameters();

            var ctrParamsInstances = new List<object>();

            foreach (var param in ctrParams)
            {
                ctrParamsInstances.Add(CreateInstance(param.ParameterType));
            }

            return Activator.CreateInstance(type, ctrParamsInstances.ToArray());
        }

        private object CreateInstanceWithPropertyDependencies(Type type)
        {
            var constructors = type.GetConstructors();

            if (constructors.Length > 1)
            {
                throw new InvalidOperationException("Can not create instance of a type having several constructors.");
            }

            if (constructors.First().GetParameters().Any())
            {
                throw new InvalidOperationException("Can not create instance of a type with property dependencies as type has not parameterless constructor.");
            }

            var instance = Activator.CreateInstance(type);

            var properties = GetImportProperties(type);

            foreach (var property in properties)
            {
                property.SetValue(instance,CreateInstance(property.PropertyType), null);
            }

            return instance;
        }

        private bool HasTypeConstructorImports(Type type)
        {
            return type.IsDefined(typeof(ImportAttribute));
        }

        private bool HasTypePropertyImports(Type type)
        {
            return GetImportProperties(type).Any();
        }

        private IEnumerable<PropertyInfo> GetImportProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                       .Where(property => Attribute.IsDefined(property, typeof(ImportPropertyAttribute)));
        }

        #endregion
    }
}
