namespace GeneralUnifiedTestSystemYard.Core.ClassExtensions;

public static class TypeExtensions
{
    public static string ToStringFully(this Type t)
    {
        return $"{t.Assembly.Location} {t.AssemblyQualifiedName}";
    }
}