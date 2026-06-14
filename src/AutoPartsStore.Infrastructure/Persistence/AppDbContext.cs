using AutoPartsStore.Domain.Entities;
using AutoPartsStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AutoPartsStore.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Appeal> Appeals => Set<Appeal>();
    public DbSet<SupplierRequest> SupplierRequests => Set<SupplierRequest>();
    public DbSet<ClientQuestion> ClientQuestions => Set<ClientQuestion>();
    public DbSet<AppealNumberCounter> AppealNumberCounters => Set<AppealNumberCounter>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<SupplierSection> SupplierSections => Set<SupplierSection>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Content> Contents => Set<Content>();
    public DbSet<News> News => Set<News>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductAttributeValue> ProductAttributeValues => Set<ProductAttributeValue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).HasMaxLength(200).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.PasswordHash).HasMaxLength(512).IsRequired();
            entity.Property(u => u.PhoneNumber).HasMaxLength(32);
            entity.Property(u => u.Role).HasConversion<string>().HasMaxLength(32);
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Appeal>(entity =>
        {
            entity.ToTable("appeals");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Number).HasMaxLength(32).IsRequired();
            entity.Property(a => a.ContactPhone).HasMaxLength(32).IsRequired();
            entity.Property(a => a.ContactEmail).HasMaxLength(256).IsRequired();
            entity.Property(a => a.ManagerComment).IsRequired();
            entity.Property(a => a.Status).HasConversion<string>().HasMaxLength(32);
            entity.HasIndex(a => a.Number).IsUnique();
            entity.HasOne(a => a.User)
                .WithMany(u => u.Appeals)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SupplierRequest>(entity =>
        {
            entity.ToTable("supplier_requests");
            entity.Property(s => s.CompanyName).HasMaxLength(300).IsRequired();
        });

        modelBuilder.Entity<ClientQuestion>(entity =>
        {
            entity.ToTable("client_questions");
            entity.Property(c => c.Category).HasConversion<string>().HasMaxLength(32);
        });

        modelBuilder.Entity<AppealNumberCounter>(entity =>
        {
            entity.ToTable("appeal_number_counters");
            entity.HasKey(c => c.Date);
        });

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.ToTable("certificates");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
            entity.Property(c => c.ImagePath).HasMaxLength(500);
        });

        modelBuilder.Entity<SupplierSection>(entity =>
        {
            entity.ToTable("supplier_sections");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Name).HasMaxLength(200).IsRequired();
            entity.Property(s => s.Description).HasMaxLength(2000);
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("company");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(300).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(2000);
            entity.Property(c => c.Address).HasMaxLength(500);
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("contacts");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(1000);
            entity.HasOne<Company>()
                .WithMany(c => c.Contacts)
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Content>(entity =>
        {
            entity.ToTable("contents");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(300).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(4000);
            entity.Property(c => c.ImagePath).HasMaxLength(500);
        });

        modelBuilder.Entity<News>(entity => entity.ToTable("news"));
        modelBuilder.Entity<Promotion>(entity => entity.ToTable("promotions"));

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.ToTable("brands");
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Name).HasMaxLength(200).IsRequired();
            entity.HasIndex(b => b.Name).IsUnique();
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.ToTable("product_categories");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
            entity.HasIndex(c => c.Name).IsUnique();
        });

        modelBuilder.Entity<ProductAttribute>(entity =>
        {
            entity.ToTable("product_attributes");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).HasMaxLength(200).IsRequired();
            entity.Property(a => a.Type).HasConversion<string>().HasMaxLength(32);
            entity.Property(a => a.Unit).HasMaxLength(32);
            entity.HasOne(a => a.Category)
                .WithMany(c => c.Attributes)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(a => new { a.CategoryId, a.Name }).IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).HasMaxLength(300).IsRequired();
            entity.Property(p => p.Article).HasMaxLength(100).IsRequired();
            entity.Property(p => p.Price).HasPrecision(18, 2);
            entity.Property(p => p.Description).HasMaxLength(4000);
            entity.Property(p => p.ImagePath).HasMaxLength(500);
            entity.Property(p => p.CreatedAt).IsRequired();
            entity.HasIndex(p => p.Article).IsUnique();
            entity.HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProductAttributeValue>(entity =>
        {
            entity.ToTable("product_attribute_values");
            entity.HasKey(v => new { v.ProductId, v.AttributeId });
            entity.Property(v => v.Value).HasMaxLength(500).IsRequired();
            entity.HasOne(v => v.Product)
                .WithMany(p => p.AttributeValues)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(v => v.Attribute)
                .WithMany(a => a.ProductValues)
                .HasForeignKey(v => v.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
