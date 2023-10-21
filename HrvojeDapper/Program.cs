using Dapper;
using HrvojeDapper.Endpoints;
using HrvojeDapper.Models;
using HrvojeDapper.Services;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ApplicationException("Missing connection string");
    return new SqlConnectionFactory(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapTurnirImageEndpoints();
app.MapCurrentIDEndpoints();
app.MapTurnir_SEndpoints();
app.MapControllers();

app.Run();
