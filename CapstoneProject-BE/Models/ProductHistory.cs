﻿using System.Text.Json.Serialization;

namespace CapstoneProject_BE.Models
{
    public class ProductHistory
    {
        public int HistoryId { get; set; }
        public int ProductId { get; set; }
        public int ActionType { get; set; }
        public DateTime Date { get; set; }
        public float? CostPrice { get; set; }
        public string? CostPriceDifferential { get; set; }
        public float? Price { get; set; }
        public string? PriceDifferential { get; set; }
        public int? Amount { get; set; }
        public string? AmountDifferential { get; set; }
        public string? Note { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; }
    }
}
