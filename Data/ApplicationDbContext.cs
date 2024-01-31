using Microsoft.EntityFrameworkCore;
using ProductsCRUDMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

/***/

//using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ProductsCRUDMVC.Data
{
    public class ApplicationDbContext : DbContext
    { 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options )
        {
        }   

        public DbSet<product> Products { get; set; }

       /* protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
        }*/

    }
}

/*public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<client>
{
    public void Configure(EntityTypeBuilder<client> builder)
    {
        builder.Property(x => x.firstName).HasMaxLength(25);
        builder.Property(x => x.lastName).HasMaxLength(25);
        builder.Property(x => x.type).HasMaxLength(15);
    }
}*/
