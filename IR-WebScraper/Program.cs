using IR_WebScraper;
using IR_WebScraper.Models;
using System.Data;

Dictionary<string, string> _manufacturers = [];

DataTable _models = new("Models");
_models.Columns.Add("Model", typeof(string));
_models.Columns.Add("DeviceType", typeof(string));
_models.Columns.Add("SpecsUrl", typeof(string));

foreach (var manufacturer in await GsmScraper.GetManufacturers())
{
    _manufacturers.Add(manufacturer.Name, manufacturer.Url);
}

Console.WriteLine("{0,-30} {1,30}", "Manufacturer", "Url");
Console.WriteLine(new string('-', 60));
Console.ForegroundColor = ConsoleColor.Green;
foreach (var manufacturer in _manufacturers)
{
    Console.WriteLine("{0,-30} {1,30}", manufacturer.Key, manufacturer.Value);
}
Console.ResetColor();
Console.WriteLine(new string('-', 60));


while (true)
{
    Console.WriteLine("Type quit to exit the application or type manufacturer name to get models:");
    var result = Console.ReadLine();
    if(result == "quit")
        break;

    try
    {
        var models = await GsmScraper.GetModels(_manufacturers[result]);
        Console.WriteLine("{0,-40} {1,-20} {2, 30}", "Model", "Type", "Specs Url");
        Console.WriteLine(new string('-', 90));
        Console.ForegroundColor = ConsoleColor.Green;
        foreach (var model in models)
        {
            Console.WriteLine("{0,-40} {1,-20} {2, 30}", model.Model, model.DeviceType, model.SpecsUrl);
        }
        Console.ResetColor();
        Console.WriteLine(new string('-', 90));
        Console.WriteLine($"Availbale manufacturers: {string.Join(", ", _manufacturers.Keys.ToList())}");
    }
    catch(Exception ex)
    {
        Console.WriteLine($"Error accured: {ex.Message}");
    }
}
