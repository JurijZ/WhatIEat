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
using WhatIEatAPI.Models;
using Lucene.Net;
using Lucene.Net.Search.Spell;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Microsoft.Extensions.Logging;

namespace WhatIEatAPI.Controllers
{
    [Route("api/[controller]")]
    public class MAnalysisController : Controller
    {
        private IHostingEnvironment hostingEnv; // To get the web server paths
        private IMemoryCache _cache; // To use an In-Memory cache
        private readonly ILogger<MAnalysisController> _logger; // To use NLog logger

        public MAnalysisController(IHostingEnvironment env, IMemoryCache memoryCache, ILogger<MAnalysisController> logger)
        {
            this.hostingEnv = env;
            _cache = memoryCache;
            _logger = logger;
        }

        [ApiExplorerSettings(IgnoreApi = true)] // Swagger instruction to ignore this method
        public IMemoryCache ReturnCache()
        {
            return _cache;
        }

        [HttpGet]
        public IActionResult Get()
        {
            // Logging
            _logger.LogInformation(10, "External app is testing Web API availability");

            return Ok("OK");
        }

        // Object to receive incomming JSON. JSON must use "Text" as a key.
        public class MRecognizedText
        {
            public string Text { get; set; }
        }

        // Smarphone makes a Post to this method
        [HttpPost]
        public IActionResult Post([FromBody]MRecognizedText recognizedText)
        {
            // Logging
            _logger.LogInformation(100, "Received Text: " + recognizedText.Text);

            if (recognizedText.Text == null)
            {
                return new ObjectResult("An empty HTML body was sent to the server");
            }
            
            // Analyse text recognized on the smartphone;
            var analysedText = MTextAnalyser(recognizedText.Text);            
            return new ObjectResult(analysedText);
            //return new ObjectResult("OK");
            //return Json(message);
            //return Ok();
        }



        [ApiExplorerSettings(IgnoreApi = true)] // This is for swagger to ignore this method
        private string MTextAnalyser(string recognizedText)
        {
            // Get rid of the new line symbols (\n)
            var l1 = recognizedText.Replace("\r\n", "");
            var l2 = l1.Replace(@"\n", "");
            var l3 = l2.Replace(@"\t|\n|\r", String.Empty);
            
            // Split into a list of strings by using simple various separators:
            List<string> rawListOfIngredients = l3.ToLower().Split(':', ',', '.', '%', '&', '[', ']', '(', ')').ToList();
            
            var ListOfIngredients = new List<Ingredient2>(); // initialize an empty list of response ingredients
            foreach (var ing in rawListOfIngredients)
            {
                // Preprocess raw ingredient value (remove spaces and other non-related symbols)
                var ingredientString = ing.Trim();

                // Call ingredient Analyser Method to analyse each ingredient as a string and return an object
                var checkedIngredient = MIngredientAnalyser(ingredientString);

                // !!! important parameter. 4 - filters out results that are not relevant at all
                if ((ingredientString.Length > 4) & (ingredientString.Length - checkedIngredient.FuzzyDistance) > 4)
                {
                    ListOfIngredients.Add(checkedIngredient);
                }
                else if ((ingredientString.Length <= 4) & (ingredientString.Length - checkedIngredient.FuzzyDistance) > 1)
                {
                    ListOfIngredients.Add(checkedIngredient);
                }
                else
                {
                    // Logging
                    _logger.LogInformation(10, ingredientString + " - not relevant");
                }
            };
            
            // Orders ListOfIngredients by the danger level and takes top 10
            var orderedIngredients = (from l in ListOfIngredients
                                       orderby l.IngredientDangerLevel descending
                                       select l).Take(10);

            // If nothing is recognised then return a notification JSON
            if (orderedIngredients == null || !orderedIngredients.Any())
            {
                var JSON = "[{\"IngredientName\":\"No ingredients detected\",\"IngredientDescrioption\":\"No description\",\"IngredientDangerLevel\":0,\"FuzzyDistance\":0}]";
                return JSON;
            }
            else
            {
                // Converts collection of objects into JSON
                var JSON = JsonConvert.SerializeObject(orderedIngredients);

                // Logging
                _logger.LogInformation(100, JSON);

                return JSON;
            }
        }


        // Finds ingredient and assigns a score to it
        [ApiExplorerSettings(IgnoreApi = true)] // This is for swagger to ignore this method
        private Ingredient2 MIngredientAnalyser(string ingredient)
        {
            var Ing = new Ingredient2();
            Dictionary<string, int> workingset = new Dictionary<string, int>();

            // First check for the exact match of the short words
            string matchShort = null;
            foreach (var key in _cache.Get<List<string>>("knowledgeBaseShort")) // read from the cache, which is configured in the SratUp.cs
            {
                if (ingredient == key)
                {
                    matchShort = ingredient;

                    Ing.IngredientName = ingredient;
                    Ing.FuzzyDistance = 0;
                    Ing.IngredientDescription = _cache.Get<IEnumerable<WhatIEatAPI.Models.Ingredient>>("knowledgeBaseEverything")
                                                    .Where(x => x.IngredientName == ingredient)
                                                    .Select(x => x.IngredientDescription)
                                                    .FirstOrDefault();
                    Ing.IngredientDangerLevel = _cache.Get<IEnumerable<WhatIEatAPI.Models.Ingredient>>("knowledgeBaseEverything")
                                                    .Where(x => x.IngredientName == ingredient)
                                                    .Select(x => x.IngredientDangerLevel)
                                                    .FirstOrDefault();

                    // Logging !!!
                    //string createText = ingredient + " - exact match preprocessing" + Environment.NewLine;
                    //System.IO.File.AppendAllText(@"D:\WhatIEat\WebAPIInAspNetCore\Log\Log.txt", createText);

                    break;
                }
            }

            // Check for the exact match of the long words
            string matchLong = null;
            foreach (var key in _cache.Get<List<string>>("knowledgeBaseAll")) // read from the cache, which is configured in the SratUp.cs
            {
                if (ingredient == key)
                {
                    matchLong = ingredient;

                    Ing.IngredientName = ingredient;
                    Ing.FuzzyDistance = 0;
                    Ing.IngredientDescription = _cache.Get<IEnumerable<WhatIEatAPI.Models.Ingredient>>("knowledgeBaseEverything")
                                                    .Where(x => x.IngredientName == ingredient)
                                                    .Select(x => x.IngredientDescription)
                                                    .FirstOrDefault();
                    Ing.IngredientDangerLevel = _cache.Get<IEnumerable<WhatIEatAPI.Models.Ingredient>>("knowledgeBaseEverything")
                                                    .Where(x => x.IngredientName == ingredient)
                                                    .Select(x => x.IngredientDangerLevel)
                                                    .FirstOrDefault();

                    // Logging !!!
                    //string createText = ingredient + " - exact match preprocessing" + Environment.NewLine;
                    //System.IO.File.AppendAllText(@"D:\WhatIEat\WebAPIInAspNetCore\Log\Log.txt", createText);

                    break;
                }
            }

            // Check for the fuzzy match
            if (matchShort == null & matchLong == null)
            {
                // Short words
                if (ingredient.Length <= 5)
                {
                    // Applies Levenshtein distance algorithm to find matches between the ingredient nad the records in the knowledgebase
                    foreach (var kbWord in _cache.Get<List<string>>("knowledgeBaseShort")) // read from the cache, which is configured in the SratUp.cs
                    {

                        // Fuzzy match for the short strings
                        var distance = LevenshteinDistance.Compute(ingredient, kbWord);
                        workingset.Add(kbWord, distance);

                        // Log
                        string createText = ingredient + " - " + kbWord + " - " + distance.ToString() + Environment.NewLine;
                        //System.IO.File.AppendAllText(@"D:\WhatIEat\WhatIEatAPI\Log\Log.txt", createText);
                        _logger.LogInformation(10, createText);
                    }
                }
                // Long words
                else
                {
                    // First try simple algorithms
                    // Applies Levenshtein distance algorithm to find matches between the ingredient and the records in the knowledgebase
                    foreach (var kbWord in _cache.Get<List<string>>("knowledgeBaseAll"))
                    {
                        // Fuzzy match for the long strings
                        var distance = LevenshteinDistance.Compute(ingredient, kbWord);
                        workingset.Add(kbWord, distance);

                        // Log
                        string createText = ingredient + " - " + kbWord + " - " + distance.ToString() + Environment.NewLine;
                        //System.IO.File.AppendAllText(@"D:\WhatIEat\WhatIEatAPI\Log\Log.txt", createText);
                        _logger.LogInformation(10, createText, "Exception detected: ");
                    }

                    // If the resulting workingset does not provide enougth acuracy (distances are too large) then apply more advanced algorithms
                    var minDistance = workingset.Select(item => item.Value).Min();
                    if (minDistance > 10)
                    {
                        // Use Lucene library Spell Checker
                        DirectoryInfo path = new DirectoryInfo("D:\\WhatIEat\\WhatIEatAPI\\Index");
                        Lucene.Net.Store.Directory directory = new MMapDirectory(path);
                        SpellChecker spellchecker = new SpellChecker(directory);
                        
                        var dictionary = new PlainTextDictionary(new FileInfo("D:\\WhatIEat\\WhatIEatAPI\\Index\\PlainTextDictionary.txt"));
                        IndexWriterConfig config = new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_48, null);
                        spellchecker.IndexDictionary(dictionary, config, false);
                        
                        // Log
                        foreach (var suggestion in spellchecker.SuggestSimilar("wholeaincreal", 5))
                        {
                            //System.IO.File.AppendAllText(@"D:\WhatIEat\WhatIEatAPI\Log\Log.txt", "Suggested: " + suggestion + Environment.NewLine);
                            _logger.LogInformation(10, "Suggested: " + suggestion);
                        }

                        //System.IO.File.AppendAllText(@"D:\WhatIEat\WhatIEatAPI\Log\Log.txt", "Min value: " + minDistance + Environment.NewLine);
                        _logger.LogInformation(10, "Min value: " + minDistance);
                    }
                }

                var keyWithMinValue = workingset.OrderBy(kvp => kvp.Value).First(); // selects one with the shortest distance
                Ing.IngredientName = keyWithMinValue.Key;
                Ing.FuzzyDistance = keyWithMinValue.Value;

                _logger.LogInformation(100, "-" + keyWithMinValue.Key + "-");

                Ing.IngredientDescription = _cache.Get<IEnumerable<WhatIEatAPI.Models.Ingredient>>("knowledgeBaseEverything")
                                                .Where(x => x.IngredientName == keyWithMinValue.Key)
                                                .Select(x => x.IngredientDescription)
                                                .FirstOrDefault();
                Ing.IngredientDangerLevel = _cache.Get<IEnumerable<WhatIEatAPI.Models.Ingredient>>("knowledgeBaseEverything")
                                                .Where(x => x.IngredientName == keyWithMinValue.Key)
                                                .Select(x => x.IngredientDangerLevel)
                                                .FirstOrDefault();
                
            }

            return Ing;
        }
    }
}