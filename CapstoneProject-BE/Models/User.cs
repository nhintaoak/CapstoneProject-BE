﻿using System.Text.Json.Serialization;

namespace CapstoneProject_BE.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; }
        public long Phone { get; set; }
        public int RoleId { get; set; }
        public bool Status { get; set; }
        [JsonIgnore]
        public virtual ICollection<ImportOrder> ImportOrder { get; set; }
        [JsonIgnore]
        public virtual ICollection<ExportOrder> ExportOrder { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductHistory> ProductHistories { get; set; }
        [JsonIgnore]
        public virtual ICollection<StocktakeNote> CreatedStocktakeNotes { get; set; }
        [JsonIgnore]
        public virtual ICollection<StocktakeNote> UpdatedStocktakeNotes { get; set; }
        public virtual Role Role { get; set; }
        public virtual RefreshToken RefreshToken { get; set; }  
        public virtual ICollection<EmailToken> EmailTokens { get; set; }
        public int StorageId { get; set; }
        public Storage Storage { get; set; }

    }
}
