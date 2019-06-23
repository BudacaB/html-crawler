using System;
using System.Threading.Tasks;
using System.IO;
using AngleSharp;
using AngleSharp.Html.Parser;
using System.Linq;
using Newtonsoft.Json;
using AngleSharp.Dom;

namespace html_crawler
{
    class Program
    {
        public static string GetHTML(string path)
        {
            string stringSource = string.Empty;

            using (StreamReader sr = File.OpenText(path))
            {
                string liniaCurenta;
                while ((liniaCurenta = sr.ReadLine()) != null)
                {
                   stringSource += liniaCurenta;
                }
            }
            return stringSource;
        }

        public static async Task<AngleSharp.Dom.IDocument> GetDocument(string source)
        {
            var config = Configuration.Default;
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(req => req.Content(source));
            return document;
        }

        public static string[] getLanguages(IDocument document)
        {
            var centralFeaturedDiv = document.All.First(m => m.ClassList.Contains("central-featured"));

            //var allLangs = centralFeaturedDiv
            //    .Children
            //    .Where(element => element.ClassList.Contains("central-featured-lang"))
            //    .Select(element => element.TextContent)
            //    .ToArray();

            var allLangsAsJson = centralFeaturedDiv
                .Children
                .Where(element => element.ClassList.Contains("central-featured-lang"))
                .Select(LanguageObject)
                .Select(StringifyExtractedText)
                .ToArray();

            return allLangsAsJson;
        }

        public static ExtractedText LanguageObject(IElement element)
        {
            var contentFromAnchor = element.Children.First(item => item.LocalName == "a");
            var contentFromStrong = contentFromAnchor.Children.First(item => item.LocalName == "strong").TextContent;
            var contentFromSmall = contentFromAnchor.Children.First(item => item.LocalName == "small").TextContent;
            var langObject = new ExtractedText();
            langObject.Language = contentFromStrong;
            langObject.Details = contentFromSmall;
            return langObject;
        }

        public static string StringifyExtractedText(ExtractedText extractedText)
        {
            return JsonConvert.SerializeObject(extractedText);
        }

        static async Task Main(string[] args)
        {
            string source = GetHTML(@"D:\Coding\html-crawler\html-crawler\resources\Wikipedia.html");
            //Console.WriteLine(source);

            var document = await GetDocument(source);

            var langs = getLanguages(document);

            foreach (var lang in langs)
            {
                Console.WriteLine(lang);
            }
        }
    }

    class ExtractedText
    {
        public string Language { get; set; }
        public string Details { get; set; }
    }
}
