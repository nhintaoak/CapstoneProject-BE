namespace CapstoneProject_BE.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long Phone { get; set; }
        public int RoleId { get; set; }
        public bool Status { get; set; }
        public virtual Role Role { get; set; }
        public virtual RefreshToken RefreshToken { get; set; }  
        public virtual ICollection<EmailToken> EmailTokens { get; set; }

    }
}
