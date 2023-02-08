using Microsoft.EntityFrameworkCore;

namespace CapstoneProject_BE.Models
{
    public class InventoryManagementContext : DbContext
    {
        public InventoryManagementContext(DbContextOptions<InventoryManagementContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<EmailToken> EmailTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("User");
                e.HasKey(u => u.UserId);
                e.HasOne(u => u.Role)
                .WithMany(u=>u.Users)
                .HasForeignKey(u=>u.RoleId);
                e.Property(u => u.Email).IsRequired();
                e.Property(u=>u.Password).IsRequired();
                e.Property(u => u.RoleId).HasDefaultValue(0);
                e.Property(u => u.UserId).UseIdentityColumn();
                e.Property(u => u.Status).HasDefaultValue(false);
            });
            modelBuilder.Entity<Role>(e =>
            {
                e.ToTable("Role");
                e.HasKey(r => r.RoleId);
                e.Property(r => r.RoleName).IsRequired();
                e.Property(u => u.RoleId).UseIdentityColumn();
            });
            modelBuilder.Entity<RefreshToken>(e =>
            {
                e.ToTable("RefreshToken");
                e.HasKey("TokenId");
                e.HasOne(rf => rf.User)
                .WithOne(u => u.RefreshToken)
                .HasForeignKey<RefreshToken>(u => u.UserId);
                e.Property(u => u.IsRevoked).HasDefaultValue(false);
                e.Property(u => u.TokenId).UseIdentityColumn();
            });
            modelBuilder.Entity<EmailToken>(e =>
            {
                e.ToTable("EmailToken");
                e.HasKey(r => r.TokenId);
                e.HasOne(t=>t.User)
                .WithMany(a=>a.EmailTokens)
                .HasForeignKey(u => u.UserId);
                e.Property(u => u.Token).IsRequired();
                e.Property(u => u.TokenId).UseIdentityColumn();
            });
        }
    }
}
