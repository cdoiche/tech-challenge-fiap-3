using Fiap.Consumer;
using Fiap.Core.Context;
using Fiap.Core.Interfaces;
using Fiap.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddHostedService<Worker>();

builder.Services.AddDbContext<FiapDataContext>(options => options.UseNpgsql(connectionString));

var host = builder.Build();

// Run initial migrations
//using (var scope = host.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var context = services.GetRequiredService<FiapDataContext>();
//        context.Database.Migrate();
//    }
//    catch (Exception ex)
//    {
//        // Log the error or handle it as needed
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "An error occurred while migrating the database.");
//    }
//}

host.Run();
