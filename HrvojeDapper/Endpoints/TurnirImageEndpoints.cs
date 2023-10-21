using Dapper;
using HrvojeDapper.Models;
using HrvojeDapper.Services;

namespace HrvojeDapper.Endpoints;

public static class TurnirImageEndpoints
{
    public static void MapTurnirImageEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("TurnirImage/GetTurniriBezSlika", async (SqlConnectionFactory sqlConnectionFactory) =>
        {
            using var connection = sqlConnectionFactory.Create();

            const string sql = "SELECT * FROM TurnirImage WHERE StatusImageID=0;";

            var turnirImages = await connection.QueryAsync<TurnirImage>(sql);

            return Results.Ok(turnirImages);
        });

        builder.MapPut("TurnirImage/UpdateStatusImageID/{turnirID}", async (int turnirID, SqlConnectionFactory sqlConnectionFactory) =>
        {
            using var connection = sqlConnectionFactory.Create();

            const string procedureName = "UpdateStatusImageID";

            var parameters = new DynamicParameters();

            parameters.Add("TurnirID", turnirID, System.Data.DbType.Int32);

            var result = await connection.ExecuteAsync(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

            return Results.Ok("Polje StatusImageID za turnir s ID-em " + turnirID + " je uspješno ažurirano u TurnirImage");
        });

        builder.MapPut("TurnirImage/UpdateTurnirImageTable/{turnirID}", async (int turnirID, SqlConnectionFactory sqlConnectionFactory) =>
        {
            using var connection = sqlConnectionFactory.Create();

            const string procedureName = "UpdateTurnirImageTable";

            var parameters = new DynamicParameters();

            parameters.Add("IDTurnir", turnirID, System.Data.DbType.Int32);

            var result = await connection.ExecuteAsync(procedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

            return Results.Ok("Turnir s id-em "+turnirID+" je uspješno ažuriran u TurnirImage");
        });
    }
}

