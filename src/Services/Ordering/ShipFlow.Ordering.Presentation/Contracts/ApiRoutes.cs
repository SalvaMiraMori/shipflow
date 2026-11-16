namespace ShipFlow.Ordering.Presentation.Contracts;

/// <summary>
/// Route templates relative to the controller route (<c>api/[controller]</c>), as in EcoGreen.
/// </summary>
public static class ApiRoutes
{
    public static class Orders
    {
        public const string Place = "";
        public const string GetAll = "";
        public const string GetById = "{orderId:guid}";
        public const string Cancel = "{orderId:guid}/cancel";
    }
}
