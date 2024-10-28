using HtmlAgilityPack;
using IR_WebScraper.Models;

namespace IR_WebScraper
{
    public static class GsmScraper
    {
        private static string _gmsArenaUrl = "https://www.gsmarena.com/makers.php3";
        private static readonly HttpClient client = new();

        public static async Task<List<ManufacturerModel>> GetManufacturers()
        {
            var manufacturers = new List<ManufacturerModel>();
            var html = await client.GetStringAsync(_gmsArenaUrl);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nodes = doc.DocumentNode.SelectNodes("//div[@class='st-text']/table//td/a");

            foreach (var node in nodes)
            {
                var manufacturer = new ManufacturerModel
                {
                    Name = node.InnerHtml.Split("<br>").First(),
                    Url = "https://www.gsmarena.com/" + node.Attributes["href"].Value
                };

                manufacturers.Add(manufacturer);
            }

            return manufacturers;
        }

        public static async Task<List<DeviceDetails>> GetModels(string url)
        {
            var deviceDetails = new List<DeviceDetails>();
            string nextPageUrl = url;

            while (nextPageUrl != null)
            {
                var html = await client.GetStringAsync(nextPageUrl);
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var nodes = doc.DocumentNode.SelectNodes("//div[@class='makers']//li/a");
                if (nodes == null)
                {
                    Console.WriteLine("Failed to find phone model nodes.");
                    break;
                }

                foreach (var node in nodes)
                {
                    var nameNode = node.SelectSingleNode(".//strong/span");
                    if (nameNode == null)
                    {
                        Console.WriteLine("Failed to find phone model name node.");
                        continue;
                    }

                    var typeNode = node.SelectSingleNode(".//span[@class='note']");
                    string type = nameNode.InnerText.Trim().Contains("Tab", StringComparison.OrdinalIgnoreCase) ? "Tablet" :
                                  nameNode.InnerText.Trim().Contains("Watch", StringComparison.OrdinalIgnoreCase) ? "Smartwatch" : "Smartphone";

                    deviceDetails.Add(new DeviceDetails
                    {
                        Model = nameNode.InnerText.Trim(),
                        DeviceType = type,
                        SpecsUrl = "https://www.gsmarena.com/" + node.Attributes["href"].Value
                    });
                }

                // Check for next page link
                var nextPageNodes = doc.DocumentNode.SelectNodes("//a[@class='prevnextbutton']");
                if (nextPageNodes != null && nextPageNodes.Any(node => node.Attributes["title"].Value == "Next page"))
                {
                    var nextPageNode = nextPageNodes.Single(node => node.Attributes["title"].Value == "Next page");
                    nextPageUrl = nextPageNode != null ? "https://www.gsmarena.com/" + nextPageNode.Attributes["href"].Value : null;
                }
                else
                {
                    nextPageUrl = null;
                }
            }

            return deviceDetails;
        }
    }
}
