using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WhatIEatAPI.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace WhatIEatAPI.Controllers
{
    public class Ingredient
    {
        public string IngredientName { get; set; }
        public Int16 IngredientDangerLevel { get; set; }
        public string IngredientDescription { get; set; }
    }


    [Route("api/[controller]")]
    public class ProductController : Controller
    {

        private NorthwindDbContext db;

        public ProductController(NorthwindDbContext db)
        {
            this.db = db;
        }


        //[HttpPost]
        //public IActionResult Post([FromBody] string obj)
        //{
        //    // Logging
        //    System.IO.File.AppendAllText(@"D:\WhatIEat\WebAPIInAspNetCore\Log\Log.txt", obj + Environment.NewLine);

        //    return Content(obj, "application/json");
        //    //return new ObjectResult("Text was successfully stored!: " + obj.ProductName.Length.ToString());
        //}

        [HttpPost]
        public IActionResult Post([FromBody]Product obj)
        {
            using (NorthwindDbContext db = new NorthwindDbContext())
            {
                if (obj.ProductName.Length == 0)
                {
                    return NotFound("There are no words to analyse");
                }

                // Logging
                System.IO.File.AppendAllText(@"D:\WhatIEat\WebAPIInAspNetCore\Log\Log.txt", obj.ProductName + Environment.NewLine);

                // Record the arrived string of words to a database
                db.Products.Add(obj);
                db.SaveChanges();

                // Split the string with multiple spaces into a list of words
                string[] listOfWords = obj.ProductName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Get a Danger level and a description for every ingredient
                List<Ingredient> ingredientList = new List<Ingredient>();
                foreach (var word in listOfWords)
                {
                    var IngredientDetails = db.Ingredients.Where(c => c.IngredientName == word).SingleOrDefault();

                    if (IngredientDetails != null)
                    {
                        var i = new Ingredient
                        {
                            IngredientName = word,
                            IngredientDangerLevel = IngredientDetails.IngredientDangerLevel,
                            IngredientDescription = IngredientDetails.IngredientDescription
                        };
                        ingredientList.Add(i);
                    }
                    else
                    {
                        var i = new Ingredient
                        {
                            IngredientName = word,
                            IngredientDangerLevel = 0,
                            IngredientDescription = "Unknown Ingredient"
                        };
                        ingredientList.Add(i);
                    }
                }

                // Serialise the list of objects into a JSON array
                var json = JsonConvert.SerializeObject(new { Ingredients = ingredientList });

                // Logging
                System.IO.File.AppendAllText(@"D:\WhatIEat\WebAPIInAspNetCore\Log\Log.txt", json + Environment.NewLine);


                return Content(json, "application/json");
                //return new ObjectResult("Text was successfully stored!: " + obj.ProductName.Length.ToString());
            }
        }
    }
}
