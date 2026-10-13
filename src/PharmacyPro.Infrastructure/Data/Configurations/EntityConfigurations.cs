using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PharmacyPro.Domain.Entities;

namespace PharmacyPro.Infrastructure.Data.Configurations;

public class FirmConfiguration : IEntityTypeConfiguration<Firm>
{
    public void Configure(EntityTypeBuilder<Firm> b)
    {
        b.HasQueryFilter(x => !x.IsDeleted);
        b.Property(x => x.Name).IsRequired().HasMaxLength(150);
        b.Property(x => x.Phone).HasMaxLength(30);
        b.Property(x => x.Address).HasMaxLength(250);
        b.HasIndex(x => x.Name).IsUnique().HasFilter("\"IsDeleted\" = false");
    }
}

public class FirmPaymentConfiguration : IEntityTypeConfiguration<FirmPayment>
{
    public void Configure(EntityTypeBuilder<FirmPayment> b)
    {
        b.HasQueryFilter(x => !x.IsDeleted);
        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.Note).HasMaxLength(500);
        b.Property(x => x.Method).HasConversion<int>();
        b.HasIndex(x => x.PaymentDate);
        b.HasOne(x => x.Firm).WithMany(f => f.Payments)
            .HasForeignKey(x => x.FirmId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> b)
    {
        b.HasQueryFilter(x => !x.IsDeleted);
        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.Note).HasMaxLength(500);
        b.Property(x => x.Method).HasConversion<int>();
        b.Property(x => x.CreatedByUserId).IsRequired().HasMaxLength(450);
        b.HasIndex(x => x.SaleDate);
    }
}

public class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> b)
    {
        b.HasQueryFilter(x => !x.IsDeleted);
        b.Property(x => x.Name).IsRequired().HasMaxLength(100);
        b.HasIndex(x => x.Name).IsUnique().HasFilter("\"IsDeleted\" = false");
    }
}

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> b)
    {
        b.HasQueryFilter(x => !x.IsDeleted);
        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.Note).HasMaxLength(500);
        b.Property(x => x.CreatedByUserId).IsRequired().HasMaxLength(450);
        b.HasIndex(x => x.ExpenseDate);
        b.HasOne(x => x.Category).WithMany(c => c.Expenses)
            .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}