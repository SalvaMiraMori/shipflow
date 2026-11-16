using System.Reflection;

namespace ShipFlow.Ordering.Domain;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
