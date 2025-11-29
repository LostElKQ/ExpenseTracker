using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Context;

public sealed class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories", "tracker");
            entity.HasKey(e => e.Id).HasName("category_pk");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsRequired()
                .HasColumnName("name");

            entity.HasIndex(e => e.Name).IsUnique().HasDatabaseName("category_name_idx");
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.ToTable("expenses", "tracker");
            entity.HasKey(e => e.Id).HasName("expense_pk");

            
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");

            entity.Property(e => e.CategoryId)
                .IsRequired()
                .HasColumnName("category_id");

            entity.Property(e => e.Amount)
                .IsRequired()
                .HasColumnName("amount");

            entity.Property(e => e.Date)
                .IsRequired()
                .HasColumnName("date");

            entity.Property(e => e.Comment)
                .HasMaxLength(256)
                .HasColumnName("comment");
            
            
            entity.HasOne(d => d.Category)
                .WithMany(p => p.Expenses)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("expenses_categories_fk");
        });
    }
}