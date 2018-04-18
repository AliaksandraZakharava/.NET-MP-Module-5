using System;

namespace NETMP.Module5.IoCContainer
{
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectingDependenciesAttribute : Attribute
    {
        public Type Contract { get; }

        public InjectingDependenciesAttribute()
        {
        }

        public InjectingDependenciesAttribute(Type contract)
        {
            Contract = contract;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ImportPropertyAttribute : InjectingDependenciesAttribute
    {
        public ImportPropertyAttribute()
        {
        }

        public ImportPropertyAttribute(Type contract) : base(contract)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ImportAttribute : InjectingDependenciesAttribute
    {
        public ImportAttribute()
        {
        }

        public ImportAttribute(Type contract) : base(contract)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ExportAttribute : InjectingDependenciesAttribute
    {
        public ExportAttribute()
        {
        }

        public ExportAttribute(Type contract) : base(contract)
        {
        }
    }
}
