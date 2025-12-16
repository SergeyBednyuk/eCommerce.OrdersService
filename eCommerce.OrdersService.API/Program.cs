using eCommerce.OrdersService.API.Middlewares;
using eCommerce.OrdersService.BLL.Extensions;
using eCommerce.OrdersService.DAL.Extensions;

namespace eCommerce.OrdersService.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDataAccessLayer(builder.Configuration);
        builder.Services.AddBusinesLogicLayer(builder.Configuration);

        builder.Services.AddControllers();

        //Add API explorer services
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyBuilder =>
            {
                policyBuilder.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        var app = builder.Build();
        
        //custom middlewares
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        //Routing
        app.UseRouting();
        
        //Cors
        app.UseCors();
        
        //Swagger
        app.UseSwagger();
        app.UseSwaggerUI();
        
        //Auth
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        //Endpoints
        app.MapControllers();
        
        app.Run();
    }
}