using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhatIEatAPI.Models
{
    [Table("Ingredients")]
    public class Ingredient
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ID { get; set; }
        [Required]
        public string IngredientName { get; set; }
        [Required]
        public Int16 IngredientDangerLevel { get; set; }
        [Required]
        public string IngredientDescription { get; set; }
        [Required]
        public Int16 IngredientPopularity { get; set; }
    }

}
