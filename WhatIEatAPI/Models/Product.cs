using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhatIEatAPI.Models
{
    [Table("Products")]
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ProductID { get; set; }
        [Required]
        public string ProductName { get; set; }

    }

}
