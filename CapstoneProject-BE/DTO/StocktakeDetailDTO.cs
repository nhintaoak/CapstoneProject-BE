﻿using CapstoneProject_BE.Models;

namespace CapstoneProject_BE.DTO
{
    public class StocktakeDetailDTO
    {
        public int StocktakeId { get; set; }
        public int ProductId { get; set; }
        public int? MeasuredUnitId { get; set; }
        public int CurrentStock { get; set; }
        public int ActualStock { get; set; }
        public string? Note { get; set; }
    }
}