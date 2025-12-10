using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eCommerce.OrdersService.DAL.Entities;

public class OrderItem
{
    private OrderItem() { }
    
    public OrderItem(Guid productId, decimal price, int quantity)
    {
        ProductId = productId;
        ItemPrice = price;
        Quantity = quantity;
        _id = Guid.NewGuid();
    }
    
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid _id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid ProductId { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal ItemPrice { get; private set; } 

    [BsonRepresentation(BsonType.Int32)]
    public int Quantity { get; private set; } 

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal TotalPrice => ItemPrice * Quantity;
}