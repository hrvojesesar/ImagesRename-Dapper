using System.ComponentModel.DataAnnotations.Schema;

namespace HrvojeDapper.Models;

public class Turnir_S
{
    public int IDTurnir { get; set; }
    public short SportID { get; set; }
    public int KategorijaID { get; set; }
    public int SuperTurnirID { get; set; }
    public int BetradarTournamentID { get; set; }
    public int BRSuperTournamentID { get; set; }
    public byte MinParova { get; set; }
    public byte MaxParova { get; set; }
    public decimal MaxUlog { get; set; }
    public byte SastavniTurnir { get; set; }
    public int TurnirIDSastavni { get; set; }
    public DateTime Sink { get; set; }
    public byte Aktivan { get; set; }
    public short RedniBrojIspis { get; set; }
    public short RedniBrojFavorit { get; set; }
    public byte TjednaPonuda { get; set; }
    public short GrupaOkladaID { get; set; }
    public string? TournamentName { get; set; }
    public byte BetSourceID { get; set; }
    public string SourceTournamentID { get; set; }
    public long? TimeStampUTC { get; set; }
    public string? PrevodNapomena { get; set; }
}
