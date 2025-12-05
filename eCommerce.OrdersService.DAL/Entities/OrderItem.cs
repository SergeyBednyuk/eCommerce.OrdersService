using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eCommerce.OrdersService.DAL.Entities;

public class OrderItem
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid _id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid ProductId { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal ItemPrice { get; set; }
    
    [BsonRepresentation(BsonType.Int32)]
    public int Quantity { get; set; }
    
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal TotalPrice { get; set; }
}