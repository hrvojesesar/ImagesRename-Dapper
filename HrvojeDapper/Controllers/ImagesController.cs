using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HrvojeDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        [HttpPut("PreimenujSliku")]
        public async Task<string> PreimenujSliku(int idTurnir, string imagePath, string file)
        {
            string newFileName = idTurnir.ToString();
            string newFilePath = Path.Combine(imagePath, newFileName + ".png");
            System.IO.File.Move(file, newFilePath);

            return newFilePath;
        }

        [HttpPut("PremjestiSliku")]
        public async Task<string> PremjestiSliku(int idTurnir, string newImagePath, string newFilePath)
        {
            string newFileName = idTurnir.ToString();
            string newFilePath2 = Path.Combine(newImagePath, newFileName + ".png");
            System.IO.File.Copy(newFilePath, newFilePath2);
            System.IO.File.Delete(newFilePath);

            return newFilePath2;
        }

        [HttpPut("DodijeliSliku")]
        public async Task<string> DodijeliSliku(int idTurnir, string newImagePath, string newFilePath)
        {
            string newFileName = idTurnir.ToString();
            string newFilePath2 = Path.Combine(newImagePath, newFileName + ".png");
            System.IO.File.Copy(newFilePath, newFilePath2);

            return newFilePath2;
        }
    }
}
