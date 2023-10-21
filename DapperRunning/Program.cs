using System.Net.Http.Json;
using System.Text;
using HrvojeDapper.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

Console.WriteLine("Dobro dosli \n");

while (true)
{
    var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
    var configuration = builder.Build();

    string vanjskaKuca = configuration["Folder:vanjskiIzvor"];
    string unutarnjaKuca = configuration["Folder:unutarnjiIzvor"];
    string posrednikFolder = configuration["Folder:posrednik"];
    string slikeFolder = configuration["Folder:slikee"];


    Console.WriteLine("Molimo unesite opciju:  \n");
    Console.WriteLine("1. Dodaj turnir \n");
    Console.WriteLine("2. Azuriraj turnir(e) \n");
    Console.WriteLine("3. Prikazi turnire koji nemaju sliku te im dodijeli \n");

    int opcija;
    opcija = int.Parse(Console.ReadLine());
    switch (opcija)
    {
        case 1:
            Console.WriteLine("Unesite podatke za novi turnir!");

            Console.WriteLine("Unesite IDTurnir: ");
            int idTurnir = int.Parse(Console.ReadLine());
            Console.WriteLine("Unesite SportID: ");
            short sportID = short.Parse(Console.ReadLine());
            Console.WriteLine("Unesite KategorijaID: ");
            int kategorijaID = int.Parse(Console.ReadLine());
            Console.WriteLine("Unesite SuperTurnirID: ");
            int superTurnirID = int.Parse(Console.ReadLine());
            Console.WriteLine("Unesite BetradarTournamentID: ");
            int betradarTournamentID = int.Parse(Console.ReadLine());
            Console.WriteLine("Unesite BRSuperTournamentID: ");
            int brSuperTournamentID = int.Parse(Console.ReadLine());
            Console.WriteLine("Unesite MinParova: ");
            byte minParova = byte.Parse(Console.ReadLine());
            Console.WriteLine("Unesite MaxParova: ");
            byte maxParova = byte.Parse(Console.ReadLine());
            Console.WriteLine("Unesite MaxUlog: ");
            decimal maxUlog = decimal.Parse(Console.ReadLine());
            Console.WriteLine("Unesite SastavniTurnir (0 ili 1): ");
            byte sastavniTurnir = byte.Parse(Console.ReadLine());
            Console.WriteLine("Unesite TurnirIDSastavni: ");
            int turnirIDSastavni = int.Parse(Console.ReadLine());
            Console.WriteLine("Unesite Sink (npr. '2023-10-10 15:30:00.000'): ");
            DateTime sink = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("Unesite Aktivan (0 ili 1): ");
            byte aktivan = byte.Parse(Console.ReadLine());
            Console.WriteLine("Unesite RedniBrojIspis: ");
            short redniBrojIspis = short.Parse(Console.ReadLine());
            Console.WriteLine("Unesite RedniBrojFavorit: ");
            short redniBrojFavorit = short.Parse(Console.ReadLine());
            Console.WriteLine("Unesite TjednaPonuda (0 ili 1): ");
            byte tjednaPonuda = byte.Parse(Console.ReadLine());
            Console.WriteLine("Unesite GrupaOkladaID: ");
            short grupaOkladaID = short.Parse(Console.ReadLine());
            Console.WriteLine("Unesite TournamentName: ");
            string tournamentName = Console.ReadLine();
            Console.WriteLine("Unesite BetSourceID: ");
            byte betSourceID = byte.Parse(Console.ReadLine());
            Console.WriteLine("Unesite SourceTournamentID: ");
            string sourceTournamentID = Console.ReadLine();
            Console.WriteLine("Unesite TimeStampUTC: ");
            long timeStampUTC = long.Parse(Console.ReadLine());
            Console.WriteLine("Unesite PrevodNapomena: ");
            string prevodNapomena = Console.ReadLine();
            Console.WriteLine("\n");

            var newTurnir = new Turnir_S
            {
                IDTurnir = idTurnir,
                SportID = sportID,
                KategorijaID = kategorijaID,
                SuperTurnirID = superTurnirID,
                BetradarTournamentID = betradarTournamentID,
                BRSuperTournamentID = brSuperTournamentID,
                MinParova = minParova,
                MaxParova = maxParova,
                MaxUlog = maxUlog,
                SastavniTurnir = sastavniTurnir,
                TurnirIDSastavni = turnirIDSastavni,
                Sink = sink,
                Aktivan = aktivan,
                RedniBrojIspis = redniBrojIspis,
                RedniBrojFavorit = redniBrojFavorit,
                TjednaPonuda = tjednaPonuda,
                GrupaOkladaID = grupaOkladaID,
                TournamentName = tournamentName,
                BetSourceID = betSourceID,
                SourceTournamentID = sourceTournamentID,
                TimeStampUTC = timeStampUTC,
                PrevodNapomena = prevodNapomena
            };

            var json = System.Text.Json.JsonSerializer.Serialize(newTurnir);
            Console.WriteLine(json);
            Console.WriteLine("\n");

            var client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7034/");

            var response = await client.PostAsync("Turnir_S/DodajNoviTurnir", new StringContent(json, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Turnir s ID-em {idTurnir} je uspjesno dodan!");
            }
            else
            {
                Console.WriteLine("Greska kod dodavanja novog turnira!");
            }
            break;
        case 2:
            int isUploaded = 0; 
            var client2 = new HttpClient();

            client2.BaseAddress = new Uri("https://localhost:7034/");

            var currentValueResponse = client2.GetAsync("CurrentID/GetCurrentValue").Result;

            if (currentValueResponse.IsSuccessStatusCode)
            {
                var content = await currentValueResponse.Content.ReadAsStringAsync();

                // Pretpostavljajući da je content JSON string
                var jsonObject = JObject.Parse(content);
                int currentValue = jsonObject["currentValue"].Value<int>();

                if (currentValue != 0)
                {
                    var turniri = client2.GetFromJsonAsync<List<Turnir_S>>("Turnir_S/GetTurniriFromTurnirS").Result;

                    if (turniri != null)
                    {
                        var filteredTurniri = turniri.Where(turnir => turnir.IDTurnir > currentValue).ToList();

                        foreach (var turnir in filteredTurniri)
                        {
                            int turnirID = turnir.IDTurnir;
                            int idBetradarTournament = turnir.BetradarTournamentID;

                            string existingImagePath = Path.Combine(unutarnjaKuca, turnirID + ".png");
                            if (File.Exists(existingImagePath))
                            {
                                Console.WriteLine("Slika za turnir ID-a " + turnirID + " vec postoji!");
                                var updateResponse = await client2.PutAsync($"TurnirImage/UpdateTurnirImageTable/{turnirID}", null);

                                if (updateResponse.IsSuccessStatusCode)
                                {
                                    Console.WriteLine($"Turnir s id-em {turnirID} je uspješno ažuriran u TurnirImage");
                                }
                                else
                                {
                                    Console.WriteLine($"Ažuriranje turnira s id-em {turnirID} nije uspjelo. Status kod: {updateResponse.StatusCode}");
                                }

                                var update2Response = await client2.PutAsync($"TurnirImage/UpdateStatusImageID/{turnirID}", null);

                                if (update2Response.IsSuccessStatusCode)
                                {
                                    Console.WriteLine($"Polje StatusImageID za turnir s ID-em {turnirID} je uspješno ažurirano.");
                                }
                                else
                                {
                                    Console.WriteLine($"Ažuriranje polja StatusImageID za turnir s ID-em {turnirID} nije uspjelo. Status kod: {update2Response.StatusCode}");
                                }
                                continue;
                            }


                            bool neMoze = false;

                            foreach (string file in Directory.EnumerateFiles(vanjskaKuca, "*.png"))
                            {
                                string fileName = Path.GetFileNameWithoutExtension(file);
                                if (turnirID == idBetradarTournament && turnirID.ToString() == fileName)
                                {
    
                                    string apiUrl2 = $"api/Images/PremjestiSliku?idTurnir={turnirID}&newImagePath={unutarnjaKuca}&newFilePath={file}";
                                    HttpResponseMessage preimenuj = await client2.PutAsync(apiUrl2, null);

                                    if (preimenuj.IsSuccessStatusCode)
                                    {
                                        Console.WriteLine("Slika je uspjesno premjestena!");
                                        isUploaded = 1;
                                        break;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Greska u premjestanju!");
                                    }
                                }
                                if (fileName == idBetradarTournament.ToString())
                                {
                                    string newFileName = turnirID.ToString();
                                    string oldName = idBetradarTournament.ToString();

                                    foreach (string file2 in Directory.EnumerateFiles(vanjskaKuca, "*.png"))
                                    {
                                        string fileName2 = Path.GetFileNameWithoutExtension(file2);
                                        if (newFileName == fileName2)
                                        {
                                            neMoze = true;
                                            break;
                                        }
                                    }
                                    if (neMoze == true)
                                    {
                                        string apiUrl2 = $"api/Images/PremjestiSliku?idTurnir={idBetradarTournament}&newImagePath={posrednikFolder}&newFilePath={file}";
                                        HttpResponseMessage preimenuj2 = await client2.PutAsync(apiUrl2, null);

                                        if (preimenuj2.IsSuccessStatusCode)
                                        {
                                            Console.WriteLine("Slika je uspjesno premjestena u folder 'posrednik'!");

                                            string resultContent = await preimenuj2.Content.ReadAsStringAsync();

                                            string apiUrl3 = $"api/Images/PreimenujSliku?idTurnir={turnirID}&imagePath={posrednikFolder}&file={resultContent}"; 

                                            HttpResponseMessage preimenuj3 = await client2.PutAsync(apiUrl3, null);

                                            var resultContent2 = await preimenuj3.Content.ReadAsStringAsync();

                                            if (preimenuj3.IsSuccessStatusCode)
                                            {
                                                Console.WriteLine("Slika je uspjesno preimenovana!");
                                                string apiUrl4 = $"api/Images/PremjestiSliku?idTurnir={turnirID}&newImagePath={unutarnjaKuca}&newFilePath={resultContent2}"; //ovdje je problem
                                                HttpResponseMessage premjesti = await client2.PutAsync(apiUrl4, null);

                                                if (premjesti.IsSuccessStatusCode)
                                                {
                                                    Console.WriteLine("Slika je uspjesno premjestena u folder 'unutarnjiIzvor'!");
                                                    isUploaded = 1;
                                                    break;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Greska u premjestanju!");
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Greska u preimenovanju!");
                                            }                                                                                    
                                        }
                                        else
                                        {
                                            Console.WriteLine("Greska u premjestanju!");
                                        }
                                        isUploaded = 1;
                                        break;
                                    }
                                   

                                    string apiUrl = $"api/Images/PreimenujSliku?idTurnir={turnirID}&imagePath={vanjskaKuca}&file={file}";

                                    HttpResponseMessage preimenuj = await client2.PutAsync(apiUrl, null);

                                    var content2 = await preimenuj.Content.ReadAsStringAsync();

                                    if (preimenuj.IsSuccessStatusCode)
                                    {
                                        Console.WriteLine("Slika je uspjesno preimenovana!");
                                        string apiUrl2 = $"api/Images/PremjestiSliku?idTurnir={turnirID}&newImagePath={unutarnjaKuca}&newFilePath={content2}";
                                        HttpResponseMessage premjesti = await client2.PutAsync(apiUrl2, null);

                                        if (premjesti.IsSuccessStatusCode)
                                        {
                                            Console.WriteLine("Slika je uspjesno premjestena!");
                                            isUploaded = 1;
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Greska u premjestanju!");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Greska u preimenovanju!");
                                    }
                                }
                            }

                            if (isUploaded == 1)
                            {
                                var updateResponse = await client2.PutAsync($"TurnirImage/UpdateTurnirImageTable/{turnirID}", null);

                                if (updateResponse.IsSuccessStatusCode)
                                {
                                    Console.WriteLine($"Turnir s id-em {turnirID} je uspješno ažuriran u TurnirImage");
                                }
                                else
                                {
                                    Console.WriteLine($"Ažuriranje turnira s id-em {turnirID} nije uspjelo. Status kod: {updateResponse.StatusCode}");
                                }

                                var update2Response = await client2.PutAsync($"TurnirImage/UpdateStatusImageID/{turnirID}", null);

                                if (update2Response.IsSuccessStatusCode)
                                {
                                    Console.WriteLine($"Polje StatusImageID za turnir s ID-em {turnirID} je uspješno ažurirano.");
                                }
                                else
                                {
                                    Console.WriteLine($"Ažuriranje polja StatusImageID za turnir s ID-em {turnirID} nije uspjelo. Status kod: {update2Response.StatusCode}");
                                }
                                isUploaded = 0;
                            }
                            else
                            {
                                Console.WriteLine("Slika za turnir ID-a " + turnirID + " ne postoji!");

                                var updateResponse = await client2.PutAsync($"TurnirImage/UpdateTurnirImageTable/{turnirID}", null);

                                if (updateResponse.IsSuccessStatusCode)
                                {
                                    Console.WriteLine($"Turnir s id-em {turnirID} je uspješno ažuriran u TurnirImage");
                                }
                                else
                                {
                                    Console.WriteLine($"Ažuriranje turnira s id-em {turnirID} nije uspjelo. Status kod: {updateResponse.StatusCode}");
                                }
                            }
                        }
                    }

                }
            }
                break;
        case 3:
            Console.WriteLine("Prikaz turnira koji nemaju sliku: \n");
            await PrikaziTurnireBezSlika();
            await DodijeliSliku();

            break;

        default:
            Console.WriteLine("Nepostojeca opcija!");
            break;
    }
}

static async Task PrikaziTurnireBezSlika()
{
    using var client = new HttpClient();
    client.BaseAddress = new Uri("https://localhost:7034/");

    HttpResponseMessage response = await client.GetAsync("TurnirImage/GetTurniriBezSlika");

    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();

        // Deserializiraj JSON u listu Turnira
        var turniri = JsonConvert.DeserializeObject<List<TurnirImage>>(content);

        // Prolazak kroz svaki turnir
        foreach (var turnir in turniri)
        {
            Console.WriteLine($"IDTurnir: {turnir.TurnirID}, StatusImageID: {turnir.StatusImageID}");
      
        }
    }
    else
    {
        Console.WriteLine("Greška pri dohvacanju turnira bez slika. Status kod: " + response.StatusCode);
    }
}

static async Task DodijeliSliku()
{
    var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
    var configuration = builder.Build();

    string vanjskaKuca = configuration["Folder:vanjskiIzvor"];
    string unutarnjaKuca = configuration["Folder:unutarnjiIzvor"];
    string posrednikFolder = configuration["Folder:posrednik"];
    string slikeFolder = configuration["Folder:slikee"];


    using var client = new HttpClient();
    client.BaseAddress = new Uri("https://localhost:7034/");

    HttpResponseMessage response = await client.GetAsync("TurnirImage/GetTurniriBezSlika");

    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();

        var turniri = JsonConvert.DeserializeObject<List<TurnirImage>>(content);

        foreach (var turnir in turniri)
        {
            int isUploaded = 0;

            int? turnirIDNullable = turnir.TurnirID;
            Console.WriteLine(turnirIDNullable);
            if (turnirIDNullable != null)
            {
                int turnirID = turnirIDNullable.Value;
               
            }

            foreach (string file in Directory.EnumerateFiles(slikeFolder, "*.png"))
            {
                string apiUrl = $"api/Images/PreimenujSliku?idTurnir={turnirIDNullable}&imagePath={slikeFolder}&file={file}";
                HttpResponseMessage preimenuj = await client.PutAsync(apiUrl, null);

                var content2 = await preimenuj.Content.ReadAsStringAsync();

                if (preimenuj.IsSuccessStatusCode)
                {
                    Console.WriteLine("Slika je uspjesno preimenovana!");
                    string apiUrl2 = $"api/Images/DodijeliSliku?idTurnir={turnirIDNullable}&newImagePath={unutarnjaKuca}&newFilePath={content2}";
                    HttpResponseMessage premjesti = await client.PutAsync(apiUrl2, null);

                    if (premjesti.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Slika je uspjesno premjestena!");
                        isUploaded = 1;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Greska u premjestanju!");
                    }
                }
                else
                {
                    Console.WriteLine("Greska u preimenovanju!");
                }
            }
            if (isUploaded == 1)
            {
                string updateUrl = $"TurnirImage/UpdateStatusImageID/{turnirIDNullable}"; // Pretpostavlja se da očekujete turnirID kao dio URL-a

                HttpResponseMessage updateResponse = await client.PutAsync(updateUrl, null);

                if (updateResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Polje StatusImageID za turnir s ID-em {turnirIDNullable} je uspješno ažurirano.");
                }
                else
                {
                    Console.WriteLine($"Ažuriranje polja StatusImageID za turnir s ID-em {turnirIDNullable} nije uspjelo. Status kod: {updateResponse.StatusCode}");
                }
            }
        }
    }
    else
    {
        Console.WriteLine("Greška pri dohvacanju turnira bez slika. Status kod: " + response.StatusCode);
    }
}




