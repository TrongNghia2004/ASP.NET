﻿namespace luongtrongnghia.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string ImageUrl { get; set; }

        // Added Quantity property
        public int? Quantity { get; set; }  

        public int CategoryId { get; set; }
    }
}
