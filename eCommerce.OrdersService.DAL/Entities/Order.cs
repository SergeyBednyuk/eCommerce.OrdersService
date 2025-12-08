using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eCommerce.OrdersService.DAL.Entities;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid _id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid OrderId { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    [BsonRepresentation(BsonType.DateTime)]
    public DateTime OrderDate { get; set; }
    
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Total { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
}