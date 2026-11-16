namespace ShipFlow.Ordering.Persistence.Constants;

internal static class SchemaNames
{
    /// <summary>Each service owns a schema, so they could share a SQL server without sharing tables.</summary>
    internal const string Ordering = "ordering";
}
