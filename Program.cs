using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using SimpleWebScraper.Builders;
using SimpleWebScraper.Data;
using SimpleWebScraper.workers;

namespace SimpleWebScraper
{
    class Program
    {
        private const string Method = "search";

        static void Main(string[] args)
        {
            Console.WriteLine("Please enter which city you would like to scrape information from: ");
            var craiglistCity = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Please enter the Craiglist category that you would like to scrape");
            var craiglistCaategoryName = Console.ReadLine() ?? string.Empty;

            using (WebClient client = new WebClient())
            {
                string content = client.DownloadString($"http://{craiglistCity.Replace(" ", string.Empty)}.craiglist.org/{Method}/{craiglistCaategoryName}");

                ScrapeCriteria scrapeCriteria = new ScrapeCriteriaBuilder()
                    .WithData(content)
                    .WithRegex(@"<a href=\""(.*?)\"" data-id=\""(.*?)\""result-title drrlnk\"">(.*?)</a>")
                    .WithRegexOption(RegexOptions.ExplicitCapture)
                    .WithPart(new ScrapeCriteriaPartBuilder()
                        .WithRegex(@">(.*?)</a>")
                        .WithRegexOption(RegexOptions.Singleline)
                        .Build())
                    .WithPart(new ScrapeCriteriaPartBuilder()
                        .WithRegex(@"href=\""(.*?)\""")
                        .WithRegexOption(RegexOptions.Singleline)
                        .Build())
                    .Build();

                Scraper scraper = new Scraper();

                var scrapedElements = scraper.Scrape(scrapeCriteria);

                if (scrapedElements.Any())
                {
                    foreach (var scrapedElement in scrapedElements)
                    {
                        Console.WriteLine(scrapedElement);
                    }
                }
                else
                {
                    Console.WriteLine("There were not matches for the specified scrapre criteria");
                }
            }
        }
    }
}
