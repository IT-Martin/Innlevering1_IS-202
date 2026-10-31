using Microsoft.EntityFrameworkCore;
using Innlevering1_Gruppe2;
using Innlevering1_Gruppe2.Models.Entities;



namespace Innlevering1_Gruppe2.DataAccess
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Resource> Resources { get; set; }
        public DbSet<ProviderAccount> ProviderAccounts { get; set; }
        public DbSet<ResourceType> ResourceTypes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Resource>(entity =>
            {
                entity.ToTable("Resource");

                // Primærnøkkel
                entity.HasKey(e => e.resource_id);

                // Vanlige kolonner
                entity.Property(e => e.resource_name)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.description)
                    .HasMaxLength(4000)
                    .IsRequired();

                entity.Property(e => e.contact_details)
                    .HasMaxLength(1000)
                    .IsRequired();

                entity.Property(e => e.verification_status)
                    .HasMaxLength(75)
                    .IsRequired();

                // Fremmednøkkel til kontoen som eier ressursen
                entity.HasOne(e => e.Account)
                    .WithMany(account => account.Resources)
                    .HasForeignKey(e => e.account_id)
                    .OnDelete(DeleteBehavior.Restrict);

                // Fremmednøkkel til ResourceType
                entity.HasOne(e => e.ResourceType)
                    .WithMany()
                    .HasForeignKey(e => e.resource_type_id)
                    .OnDelete(DeleteBehavior.Restrict);

                // Fremmednøkkel til Location
                entity.HasOne(e => e.Location)
                    .WithMany(l => l.Resources)
                    .HasForeignKey(e => e.location_id)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("Account");

                entity.HasKey(e => e.account_id);

                entity.Property(e => e.account_type)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.email)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(e => e.contact_name)
                    .HasMaxLength(200)
                    .HasDefaultValue(string.Empty)
                    .IsRequired();

                entity.Property(e => e.phone)
                    .HasMaxLength(50)
                    .HasDefaultValue(string.Empty)
                    .IsRequired();

                entity.HasIndex(e => e.email)
                    .IsUnique();

                entity.Property(e => e.password_hash)
                    .HasMaxLength(255)
                    .IsRequired();
            });

            modelBuilder.Entity<ProviderAccount>(entity =>
            {
                entity.ToTable("ProviderAccount");

                entity.HasKey(e => e.account_id);

                entity.Property(e => e.provider_type)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.contact_name)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.phone)
                    .HasMaxLength(50);

                entity.Property(e => e.address)
                    .HasMaxLength(100);

                entity.Property(e => e.organization_name)
                    .HasMaxLength(200);

                entity.Property(e => e.organization_number)
                    .HasMaxLength(100);

                entity.HasOne<Account>()
                    .WithOne()
                    .HasForeignKey<ProviderAccount>(e => e.account_id)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ResourceType>(entity =>
            {
                entity.ToTable("ResourceType");

                entity.HasKey(e => e.resource_type_id);

                entity.Property(e => e.category_name)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.HasIndex(e => e.category_name)
                    .IsUnique();

                entity.HasData(
                    new ResourceType
                    {
                        resource_type_id = 1,
                        category_name = "Kjøretøy"
                    },
                    new ResourceType
                    {
                        resource_type_id = 2,
                        category_name = "Heisekran"
                    },
                    new ResourceType
                    {
                        resource_type_id = 3,
                        category_name = "Frivillige"
                    },
                    new ResourceType
                    {
                        resource_type_id = 4,
                        category_name = "Tilhenger"
                    });
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("Location");

                entity.HasKey(e => e.location_id);

                entity.Property(e => e.address_or_area)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.latitude)
                    .HasPrecision(9, 6)
                    .IsRequired();

                entity.Property(e => e.longitude)
                    .HasPrecision(10, 7)
                    .IsRequired();
            });
        }
    }
}
