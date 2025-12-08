using AutoMapper;
using eCommerce.OrdersService.BLL.DTOs;
using eCommerce.OrdersService.DAL.Entities;

namespace eCommerce.OrdersService.BLL.Mappers;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        //responses maps
        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();
        
        //create request
        CreateMap<AddOrderRequest, Order>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            // Will calculate in BLL
            // .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(s => s.OrderItems))
            .ForMember(dest => dest._id, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.Total, opt => opt.Ignore());
        
        //update request
        CreateMap<UpdateOrderRequest, Order>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(s => s.OrderItems))
            .ForMember(dest => dest._id, opt => opt.Ignore())
            .ForMember(dest => dest.Total, opt => opt.Ignore());
        
        //order item 
        CreateMap<OrderItemRequest, OrderItem>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ItemPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest._id, opt => opt.Ignore());
        
        //filters 
        CreateMap<OrderFilterDto, OrderFilter>();
    }
}