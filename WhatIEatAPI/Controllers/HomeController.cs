using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http.Headers;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace WhatIEatAPI.Controllers
{
    //[Route("Home")]
    //[Route("~/")]
    //[Route("/")]
    //[Route("[Controller]")]
    //[Route("api/[controller]")]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //private IHostingEnvironment hostingEnv; // To get web server paths
        //private IMemoryCache _cache; // To use In-Memory cache

        //public HomeController(IHostingEnvironment env, IMemoryCache memoryCache)
        //{
        //    this.hostingEnv = env;
        //    _cache = memoryCache;
        //}

        //[ApiExplorerSettings(IgnoreApi = true)] // This is for swagger to ignore this method
        //public IMemoryCache ReturnCache()
        //{
        //    return _cache;
        //}

        //[HttpPost]
        //public async Task<IActionResult> Post()
        //{
        //    var filePath = new FileResult();

        //    long size = 0;
        //    var files = Request.Form.Files;
        //    foreach (var file in files)
        //    {
        //        var filename = ContentDispositionHeaderValue
        //                        .Parse(file.ContentDisposition)
        //                        .FileName
        //                        .Trim('"');
        //        filename = hostingEnv.WebRootPath + $@"\{filename}";

        //        filePath.FileName = filename;

        //        // Logging
        //        System.IO.File.AppendAllText(@"D:\WhatIEat\WebAPIInAspNetCore\Log\Log.txt", filename + Environment.NewLine);

        //        size += file.Length;
        //        using (FileStream fs = System.IO.File.Create(filename))
        //        {
        //            file.CopyTo(fs);
        //            fs.Flush();
        //        }
        //    }
        //    //string message = $"{files.Count} file(s) {size} bytes uploaded successfully!";

        //    // Call OCR API tp process the image and return the text
        //    var recognizedText = await CallOCR(filePath.FileName);

        //    //return Ok();
        //    return new ObjectResult(recognizedText);
        //    //return new ObjectResult("OK");
        //    //return Json(message);
        //}

        //[ApiExplorerSettings(IgnoreApi = true)] // This is for swagger to ignore this method
        //private async Task<string> CallOCR(string _filename)
        //{
        //    // Create an HttpClient and wire up the progress handler
        //    var client = new HttpClient();

        //    // Set the request timeout as large uploads can take longer than the default 2 minute timeout
        //    client.Timeout = TimeSpan.FromMinutes(20);

        //    // Open the file we want to upload and submit it
        //    using (FileStream fileStream = new FileStream(_filename, FileMode.Open, FileAccess.Read, FileShare.Read, 1024, useAsync: true))
        //    {
        //        // Create a stream content for the file
        //        //const int BufferSize = 1024;
        //        StreamContent content = new StreamContent(fileStream, 1024);

        //        // Create Multipart form data content, add our submitter data and our stream content
        //        MultipartFormDataContent formData = new MultipartFormDataContent();
        //        //formData.Add(new StringContent("Me"), "submitter");
        //        formData.Add(content, "filename", _filename);

        //        // Post the MIME multipart form data upload with the file
        //        Uri address = new Uri("http://localhost:50231/api/fileupload");
        //        HttpResponseMessage response = await client.PostAsync(address, formData);

        //        //FileResult recognizedText = await response.Content. Read<FileResult>();
        //        string recognizedText = await response.Content.ReadAsStringAsync();
        //        //Console.WriteLine("Recognized Text: {0}", recognizedText);

        //        //return recognizedText;
        //        return TextAnalyser(recognizedText);

        //        //Console.Write("Press any key to continue . . . ");
        //        //Console.ReadKey(true);                
        //    }
        //}

        //[ApiExplorerSettings(IgnoreApi = true)] // This is for swagger to ignore this method
        //private string TextAnalyser(string recognizedText)
        //{
        //    // Get rid of the new line symbols (\n)
        //    //string newLineFreeString = Regex.Replace(recognizedText, @"\t|\n|\r", "");
        //    var l1 = recognizedText.Replace("\r\n", "");
        //    var l2 = l1.Replace(@"\n", "");
        //    var l3 = l2.Replace(@"\t|\n|\r", String.Empty);

        //    //string newLineFreeString = Regex.Replace(recognizedText, "\n", String.Empty);
        //    //string newLineFreeString = Regex.Replace(recognizedText, Environment.NewLine, "");
        //    //recognizedText.Replace(Environment.NewLine, "");
        //    //string newLineFreeString = recognizedText.ToString().TrimEnd('\r', '\n');


        //    // Split into a list of strings by a ,/./%/[/]
        //    List<string> rawListOfIngredients = l3.Split(':',',','.','%','[',']','(',')').ToList();

        //    // Analyze ingredients
        //    Random r = new Random(); // This is just for testing - to genrate the random score

        //    var ListOfIngredients = new List<Ingredient2>(); // initialize an empty list
        //    foreach (var ing in rawListOfIngredients)
        //    {
        //        // Preprocess raw ingredient value (remove spaces and other non-related symbols)
        //        var ingredient = ing.Trim();
                
        //        // Method call to analyse each engredient
        //        var checkedIngredient = IngredientAnalyser(ingredient, r.Next(1, 10));
        //        // !!! important parameter. 4 - filters out results that are not relevant at all
        //        if ((ingredient.Length > 4) & (ingredient.Length - checkedIngredient.FuzzyDistance) > 4)
        //        {
        //            ListOfIngredients.Add(checkedIngredient);
        //        }
        //        else if ((ingredient.Length <= 4) & (ingredient.Length - checkedIngredient.FuzzyDistance) > 1 )
        //        {
        //            ListOfIngredients.Add(checkedIngredient);
        //        }
        //        else
        //        {
        //            //string createText = ingredient + " - exact match preprocessing" + Environment.NewLine;
        //            System.IO.File.AppendAllText(@"D:\WhatIEat\WebAPIInAspNetCore\Log\Log.txt", ingredient + " - not relevant" + Environment.NewLine);
        //        }
        //    };

        //    // Convert List of strings to JSON
        //    //var orderedIngredients = JsonConvert.SerializeObject(rawListOfIngredients);

        //    // Generates HTML out of ListOfIngredients (takes top 15 from the list)
        //    var html = new XElement("div", new XAttribute("id", "list"),
        //            new XElement("ul", new XAttribute("id", "unorderedlist"),
        //                (from l in ListOfIngredients
        //                 orderby l.IngredientDangerLevel descending
        //                 select new XElement("li",
        //                    new XAttribute("class", l.ConvertScoreToColour()),
        //                        l.IngredientName + " (s:" + l.IngredientDangerLevel.ToString() + ") (d:" + l.FuzzyDistance + ")")).Take(14))).ToString();

        //    // Test HTML code
        //    //string html = "<div id=\"list\">" +
        //    //    "<ul id=\"unorderedlist\">" +
        //    //        "<li class=\"red\">one</li>" +
        //    //        "<li class=\"yellow\">two</li>" +
        //    //        "<li class=\"green\">three</li>" +
        //    //    "</ul>" +
        //    //    "</div>";

        //    return html;
        //}

        //[ApiExplorerSettings(IgnoreApi = true)] // This is for swagger to ignore this method
        //// Finds ingredient and assigns a score to it
        //private Ingredient2 IngredientAnalyser(string ingredient, int randomScore)
        //{
        //    var Ing = new Ingredient2();
        //    Dictionary<string, int> workingset = new Dictionary<string, int>();

        //    // First check for the exact match of the short words
        //    string matchShort = null;
        //    foreach (var key in _cache.Get<List<string>>("knowledgeBaseShort")) // read from the cache, which is configured in the SratUp.cs
        //    {
        //        if (ingredient == key)
        //        {
        //            matchShort = ingredient;

        //            Ing.IngredientName = ingredient;
        //            Ing.FuzzyDistance = 0;
        //            //Ing.IngredientDangerLevel = (short)randomScore;
        //            Ing.IngredientDangerLevel = _cache.Get<IEnumerable<WebAPIInAspNetCore.Models.Ingredient>>("knowledgeBaseEverything")
        //                                        .Where(x => x.IngredientName == ingredient)
        //                                        .Select(x => x.IngredientDangerLevel)
        //                                        .FirstOrDefault();

        //            // Logging !!!
        //            //string createText = ingredient + " - exact match preprocessing" + Environment.NewLine;
        //            //System.IO.File.AppendAllText(@"D:\WhatIEat\WebAPIInAspNetCore\Log\Log.txt", createText);

        //            break;
        //        }
        //    }

        //    // Check for the exact match of the long words
        //    string matchLong = null;
        //    foreach (var key in _cache.Get<List<string>>("knowledgeBaseAll")) // read from the cache, which is configured in the SratUp.cs
        //    {
        //        if (ingredient == key)
        //        {
        //            matchLong = ingredient;

        //            Ing.IngredientName = ingredient;
        //            Ing.FuzzyDistance = 0;
        //            //Ing.IngredientDangerLevel = (short)randomScore;
        //            Ing.IngredientDangerLevel = _cache.Get<IEnumerable<WebAPIInAspNetCore.Models.Ingredient>>("knowledgeBaseEverything")
        //                                        .Where(x => x.IngredientName == ingredient)
        //                                        .Select(x => x.IngredientDangerLevel)
        //                                        .FirstOrDefault();

        //            // Logging !!!
        //            //string createText = ingredient + " - exact match preprocessing" + Environment.NewLine;
        //            //System.IO.File.AppendAllText(@"D:\WhatIEat\WebAPIInAspNetCore\Log\Log.txt", createText);

        //            break;
        //        }
        //    }
            
        //    // Check for the fuzzy match
        //    if (matchShort == null & matchLong == null)
        //    {

        //        if (ingredient.Length <= 5)
        //        {
        //            // Applies Levenshtein distance algorithm to find matches between the ingredient nad the records in the knowledgebase
        //            foreach (var kbWord in _cache.Get<List<string>>("knowledgeBaseShort")) // read from the cache, which is configured in the SratUp.cs
        //            {
                        
        //                // Fuzzy match for the short strings
        //                var distance = LevenshteinDistance.Compute(ingredient, kbWord);
        //                workingset.Add(kbWord, distance);

        //                /// Change This path !!!!
        //                //string createText = ingredient + " - " + kbWord + " - " + distance.ToString() + Environment.NewLine;
        //                //System.IO.File.AppendAllText(@"D:\WhatIEat\WebAPIInAspNetCore\Log\Log.txt", createText);
                        
        //            }
        //        }
        //        else
        //        {
        //            // Applies Levenshtein distance algorithm to find matches between the ingredient nad the records in the knowledgebase
        //            foreach (var kbWord in _cache.Get<List<string>>("knowledgeBaseAll"))
        //            {
                        
        //                // Fuzzy match for the long strings
        //                var distance = LevenshteinDistance.Compute(ingredient, kbWord);
        //                workingset.Add(kbWord, distance);

        //                /// Change This path !!!!
        //                //string createText = ingredient + " - " + kbWord + " - " + distance.ToString() + Environment.NewLine;
        //                //System.IO.File.AppendAllText(@"D:\WhatIEat\WebAPIInAspNetCore\Log\Log.txt", createText);
                        
        //            }
        //        }

        //        var keyWithMinValue = workingset.OrderBy(kvp => kvp.Value).First(); // selects one with the shortest distance
        //        Ing.IngredientName = keyWithMinValue.Key;
        //        Ing.FuzzyDistance = keyWithMinValue.Value;

        //        // Retrieve ingredient danger level from the cache
        //        //var dangerLevel = _cache.Get<IEnumerable<WebAPIInAspNetCore.Models.Ingredient>>("knowledgeBaseEverything");

        //        Ing.IngredientDangerLevel = _cache.Get<IEnumerable<WebAPIInAspNetCore.Models.Ingredient>>("knowledgeBaseEverything")
        //                                    .Where(x => x.IngredientName == keyWithMinValue.Key)
        //                                    .Select(x => x.IngredientDangerLevel)
        //                                    .FirstOrDefault();

        //        //Ing.IngredientDangerLevel = dangerLevel2;
        //        //Ing.IngredientDangerLevel = (short)randomScore;
        //    }
                    
        //    return Ing;
        //}


    }

}
