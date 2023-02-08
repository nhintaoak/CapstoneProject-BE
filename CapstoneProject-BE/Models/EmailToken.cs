using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;

namespace CapstoneProject_BE.Models
{
    public class EmailToken
    {
        public int TokenId { get; set; }
        public string Token { get; set; }
        public int UserId { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public virtual User User { get; set; }
    }
}
