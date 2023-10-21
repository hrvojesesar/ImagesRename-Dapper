namespace HrvojeDapper.Endpoints;

public static class SlikeEndpoints
{
    public static void MapSlikeEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPut("Slike/PreimenujSliku", async (int idTurnir, string imagePath, string file) =>
        {
            string newFileName = idTurnir.ToString();
            string newFilePath = Path.Combine(imagePath, newFileName + ".png");
            File.Move(file, newFilePath);

            return newFilePath;
        });

        builder.MapPut("Slike/PremjestiSliku", async (int idTurnir, string newImagePath, string newFilePath) =>
        {
            string newFileName = idTurnir.ToString();
            string newFilePath2 = Path.Combine(newImagePath, newFileName + ".png");
            File.Copy(newFilePath, newFilePath2);
            File.Delete(newFilePath);

            return newFilePath2;
        });


        builder.MapPut("Slike/DodijeliSliku", async (int idTurnir, string newImagePath, string newFilePath) =>
        {
            string newFileName = idTurnir.ToString();
            string newFilePath2 = Path.Combine(newImagePath, newFileName + ".png");
            File.Copy(newFilePath, newFilePath2);

            return newFilePath2;
        });
    }
}
