using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Fiap.Core.Context;
using Microsoft.AspNetCore.Builder; // Namespace do seu DbContext

var builder = WebApplication.CreateBuilder(args);

// Configure a string de conexão
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FiapDataContext>(options =>
    options.UseNpgsql(connectionString));

// Adicione serviços
builder.Services.AddControllers();

var app = builder.Build();

// Configure o pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers(); // Mapeie os controladores

app.Run();
