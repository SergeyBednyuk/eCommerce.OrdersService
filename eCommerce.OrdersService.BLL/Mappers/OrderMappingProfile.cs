using AutoMapper;
using eCommerce.OrdersService.BLL.DTOs;
using eCommerce.OrdersService.DAL.Entities;

namespace eCommerce.OrdersService.BLL.Mappers;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
// --- RESPONSES (Read) ---
        CreateMap<Order, OrderDto>();

        // Consolidated Item Mapping
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.ItemPrice));
        // Note: ProductId, Quantity, TotalPrice match automatically.

        // --- REQUESTS (Write) ---
        CreateMap<AddOrderRequest, Order>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(s => s.OrderItems))
            // Security: Ignore generated fields
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.Total, opt => opt.Ignore()) 
            .ForMember(dest => dest.OrderDate, opt => opt.Ignore());

        CreateMap<OrderItemRequest, OrderItem>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ItemPrice, opt => opt.MapFrom(src => src.UnitPrice)) // UnitPrice -> ItemPrice
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest._id, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore());

        // Update Mapping
        CreateMap<UpdateOrderRequest, Order>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Never update PK
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems)) // Prevent accidental item wipe
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<OrderItemRequest, UpdateStockDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity)).ReverseMap();

        // Filters
        CreateMap<GetOrdersQuery, OrderFilter>();
        CreateMap<OrderFilterDto, OrderFilter>();
    }
}