using System.Data;
using Dapper;
using HrvojeDapper.DTOs;
using HrvojeDapper.Models;
using HrvojeDapper.Services;

namespace HrvojeDapper.Endpoints;

public static class Turnir_SEndpoints
{
    public static void MapTurnir_SEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("Turnir_S/DodajNoviTurnir", async (Turnir_S turnir_s, SqlConnectionFactory sqlConnectionFactory) =>
        {
            using var connection = sqlConnectionFactory.Create();

            const string sql = "INSERT INTO Turnir_S (IDTurnir, SportID, KategorijaID, SuperTurnirID, BetradarTournamentID, BRSuperTournamentID, MinParova, MaxParova, MaxUlog, SastavniTurnir, TurnirIDSastavni, Sink, Aktivan, RedniBrojIspis, RedniBrojFavorit, TjednaPonuda, GrupaOkladaID, TournamentName, BetSourceID, SourceTournamentID, TimeStampUTC, PrevodNapomena)\r\n    VALUES (@IDTurnir, @SportID, @KategorijaID, @SuperTurnirID, @BetradarTournamentID, @BRSuperTournamentID, @MinParova, @MaxParova, @MaxUlog, @SastavniTurnir, @TurnirIDSastavni, @Sink, @Aktivan, @RedniBrojIspis, @RedniBrojFavorit, @TjednaPonuda, @GrupaOkladaID, @TournamentName, @BetSourceID, @SourceTournamentID, @TimeStampUTC, @PrevodNapomena);";

            await connection.ExecuteAsync(sql, turnir_s);

            return Results.Ok($"Novi turnir s ID-em "+turnir_s.IDTurnir+" je uspješno dodan!");
        });
        builder.MapGet("Turnir_S/GetTurniriFromTurnirS", async (SqlConnectionFactory sqlConnectionFactory) =>
        {
            using var connection = sqlConnectionFactory.Create();

            // Naziv spremljene procedure
            string storedProcedureName = "GetTurniriFromTurnir_S";

            var turniri = (List<Turnir_SDTO>)await connection.QueryAsync<Turnir_SDTO>(storedProcedureName, commandType: CommandType.StoredProcedure);

            return turniri;
      
        });

    }
}
