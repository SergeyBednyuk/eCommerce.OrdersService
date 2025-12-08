using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eCommerce.OrdersService.DAL.Entities;

public class OrderItem(Guid productId, decimal price, int quantity)
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid _id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid ProductId { get; set; } = productId;

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal ItemPrice { get; private set; } = price;

    [BsonRepresentation(BsonType.Int32)]
    public int Quantity { get; private set; } = quantity;

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal TotalPrice => ItemPrice * Quantity;
}