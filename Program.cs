using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;


//ucitavanje putanje .csv datoteke
string obrazac = @"(.+\\\w+\.csv)";
Regex r = new Regex(obrazac);
string putanjaUlaz, putanjaIzlaz; 


//provjera ispravnosti formata unosa putanje datoteke
//korisnik unosi putanju sve dok nije unesen ispravan format putanje i postojeca putanja ulazne datoteke
while (true)
{
    try
    {
        Console.Write("Unesite naziv datoteke koju zelite ucitati: ");
        putanjaUlaz = Console.ReadLine();

        //provjera je li putanja ispravnog formata
        if (string.IsNullOrEmpty(putanjaUlaz) || !r.IsMatch(putanjaUlaz))
            throw new Exception("Putanja datoteke nije ispravno unesena. Pokusajte ponovno!");
        else
        {
            //provjera postoji li datoteka na zadanoj putanji
            try
            {
                if (!File.Exists(putanjaUlaz))
                    throw new Exception("Ne postoji datoteka na zadanoj putanji");
                else
                {
                    //izlaznu datoteku pohranjujemo u isti direktorij u kojem se nalazi i ulazna datoteka
                    putanjaIzlaz = Path.Combine(Path.GetDirectoryName(putanjaUlaz),"Izlaz.csv");
                    break;
                }
            }
            catch (Exception unutarnjiEx)
            {
                Console.WriteLine(unutarnjiEx);
            }
        }
    }
    catch(Exception vanjskiEx)
    {
        Console.WriteLine(vanjskiEx);
    }
}

//nakon sto znamo da je putanja ispravna, ucitamo podatke iz odabrane datoteke
try
{
    var lines = File.ReadLines(putanjaUlaz);

    bool prviRedak = true;
    string ispis;

    foreach (var redak in lines)
    {
        if (prviRedak)
        {
            prviRedak = false;
            File.WriteAllText(putanjaIzlaz, "Ime,Prezime,DatumRodenja,Ime(hex),Prezime(hex),DatumRodenja(hex)\n");
            ispis = String.Format("{0,-15} {1,-15} {2,-15} {3,-20} {4,-20} {5,-20}", "Ime", "Prezime", "Datum rodenja", "Ime (hex)", "Prezime (hex)", "Datum rodenja (hex)");
            Console.WriteLine(ispis);
            continue;
        }

        string[] s = redak.Split(',', StringSplitOptions.TrimEntries);

        string ime = s[0].ToUpper();
        string prezime = s[1].ToUpper();
        string datumRodenja = ConvertDateFormat(s[2]);

        string imeHex = StringToHex(ime);
        string prezimeHex = StringToHex(prezime);
        string datumHex = StringToHex(datumRodenja);


        File.AppendAllText(putanjaIzlaz, $"{ime},{prezime},{datumRodenja},{imeHex},{prezimeHex},{datumHex}\n");
        ispis = String.Format("{0,-15} {1,-15} {2,-15} {3,-20} {4,-20} {5,-20}", ime, prezime, datumRodenja, imeHex, prezimeHex, datumHex);
        Console.WriteLine(ispis);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Pogreška prilikom učitavanja datoteke: {ex.Message}");
}


static string StringToHex(string input)
{
    byte[] bytes = Encoding.UTF8.GetBytes(input);
    return Convert.ToHexString(bytes);
}

static string ConvertDateFormat(string dateInput)
{
    DateTime parsedDate;
    string[] formats = { "dd.mm.yyyy.", "yyyy-mm-dd", "dd/mm/yyyy", "mmmm dd, yyyy", "d-mmm-yyyy",
            "dd-mm-yyyy", "yyyy/mm/dd", "d.m.yyyy.", "d-m-yyyy", "d/m/yyyy" };

    if (DateTime.TryParseExact(dateInput, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        return parsedDate.ToString("dd-MM-yyyy"); 

    return "Neispravan format datuma";
}




