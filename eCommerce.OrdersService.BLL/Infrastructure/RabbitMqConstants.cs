namespace eCommerce.OrdersService.BLL.Infrastructure;

public static class RabbitMqConstants
{
    // Exchange Names
    public const string ProductsExchange = "products.exchange";
    public const string OrdersProductCreatedQueue = "orders.product-created.queue";

    // Routing Keys
    public static class ProductsRoutingKeys
    {
        public const string ProductCreated = "product.created";
        public const string ProductUpdated = "product.updated";
        public const string ProductDeleted = "product.deleted";
        public const string ProductStockUpdated = "product.stockupdated";
    }
}