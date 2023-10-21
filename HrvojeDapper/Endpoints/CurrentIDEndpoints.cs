using Dapper;
using HrvojeDapper.Models;
using HrvojeDapper.Services;

namespace HrvojeDapper.Endpoints;

public static class CurrentIDEndpoints
{
    public static void MapCurrentIDEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("CurrentID/GetCurrentValue", async (SqlConnectionFactory sqlConnectionFactory) =>
        {
            using var connection = sqlConnectionFactory.Create();

            const string sql = "SELECT CurrentValue FROM CurrentID WHERE TableName='Turnir_S'";

            var currentID = await connection.QueryFirstOrDefaultAsync<CurrentID>(sql);

            return Results.Ok(currentID);
          
        });

    }
}
