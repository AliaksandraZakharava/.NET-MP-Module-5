namespace NETMP.Module5.IoCContainer.Tests
{
    // Export classes

    [Export]
    public class ExportClassAsSelf
    {
        public string Name { get; set; }

        public ExportClassAsSelf()
        {
            Name = "ExportClassAsSelf";
        }
    }

    public interface IExportClass{}

    [Export(typeof(IExportClass))]
    public class ExportClassAsInterface : IExportClass
    {
        public string Name { get; set; }

        public ExportClassAsInterface()
        {
            Name = "ExportClassAsInterface";
        }
    }

    // Import classes

    [Import]
    public class ClassWithConstructorImport
    {
        public string Name { get; set; }

        public ClassWithConstructorImport(ExportClassAsSelf param1,
                                          IExportClass param2,
                                          ParamClassSimple param3, 
                                          ParamClassWithConstructorImport param4,
                                          ParamClassWithPropertyImport param5)
        {
            Name = "ClassWithConstructorImport";
        }
    }

    public class ClassWithPropertyImport
    {
        public string Name { get; set; }

        [ImportProperty]
        public ExportClassAsSelf Param1 { get; set; }

        [ImportProperty]
        public IExportClass Param2 { get; set; }

        [ImportProperty]
        public ParamClassSimple Param3 { get; set; }

        [ImportProperty]
        public ParamClassWithConstructorImport Param4 { get; set; }

        [ImportProperty]
        public ParamClassWithPropertyImport Param5 { get; set; }

        public ClassWithPropertyImport()
        {
            Name = "ClassWithConstructorImport";
        }
    }

    // Params
    [Import]
    public class ParamClassSimple
    {
        public string Name { get; set; }

        public ParamClassSimple()
        {
            Name = "ParamClassSimple";
        }
    }

    [Import]
    public class ParamClassWithConstructorImport
    {
        public string Name { get; set; }

        public ParamClassWithConstructorImport(ParamClassSimple paramClassSimple)
        {
            Name = "ParamClassWithConstructorImport";
        }
    }

    public class ParamClassWithPropertyImport
    {
        public string Name { get; set; }

        [ImportProperty]
        public ParamClassSimple ParamClassSimple { get; set; }

        public ParamClassWithPropertyImport()
        {
            Name = "ParamClassWithConstructorImport";
        }
    }
}
