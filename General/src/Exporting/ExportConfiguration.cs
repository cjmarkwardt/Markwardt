namespace Markwardt;

public interface IExportConfiguration
{
    void ConfigurePlan(Type type, IExportPlan? plan);
    IExportPlan? GetPlan(Type type, bool allowAuto = true);
}

public static class ExportConfigurationExtensions
{
    public static void ConfigureTypeName(IExportConfiguration configuration, Type type, string typeName)
    {
        IConfigurableExportPlan plan = configuration.GetPlan(type, false) as IConfigurableExportPlan ?? new ReflectionExportPlan();
        plan.ConfigureTypeName(typeName);
    }

    public static void ConfigureValues(IExportConfiguration configuration, Type type, MemberInfo getter, MemberInfo? setter)
    {
        IConfigurableExportPlan plan = configuration.GetPlan(type, false) as IConfigurableExportPlan ?? new ReflectionExportPlan();
        plan.ConfigureValues(getter, setter);
    }

    public static void ConfigureValues(IExportConfiguration configuration, Type type, IEnumerable<MemberInfo> getters, IEnumerable<MemberInfo> setters)
    {
        IConfigurableExportPlan plan = configuration.GetPlan(type, false) as IConfigurableExportPlan ?? new ReflectionExportPlan();
        plan.ConfigureValues(getters, setters);
    }

    public static void ConfigureProperty(IExportConfiguration configuration, Type type, string property, MemberInfo getter, MemberInfo setter)
    {
        IConfigurableExportPlan plan = configuration.GetPlan(type, false) as IConfigurableExportPlan ?? new ReflectionExportPlan();
        plan.ConfigureProperty(property, getter, setter);
    }
}