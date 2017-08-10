namespace WhatIEatAPI.Controllers
{
    public class Ingredient2
    {
        // Name of the Ingredient.
        public string IngredientName { get; set; }

        // Description of the Ingredient
        public string IngredientDescription { get; set; }

        // Ingredient score
        public short IngredientDangerLevel { get; set; }

        // Ingredient score
        public int FuzzyDistance { get; set; }

        // Method converts Score into a Colour
        public string ConvertScoreToColour()
        {

            string colour = "green";

            if (IngredientDangerLevel < 4)
            { colour = "green"; }
            else if (IngredientDangerLevel >= 4 & IngredientDangerLevel < 7)
            { colour = "yellow"; }
            else
            { colour = "red"; }

            return colour;
        }
    }

}