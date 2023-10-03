using System;
using System.Collections.Generic;
using AseIsthmusAPI.Data.AseIsthmusModels;
using Microsoft.EntityFrameworkCore;

namespace AseIsthmusAPI.Data;

public partial class AseItshmusContext : DbContext
{
    public AseItshmusContext()
    {
    }

    public AseItshmusContext(DbContextOptions<AseItshmusContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agreement> Agreements { get; set; }

    public virtual DbSet<Audit> Audits { get; set; }

    public virtual DbSet<Beneficiary> Beneficiaries { get; set; }

    public virtual DbSet<Canton> Cantons { get; set; }

    public virtual DbSet<CategoryAgreement> CategoryAgreements { get; set; }

    public virtual DbSet<ContributionBalance> ContributionBalances { get; set; }

    public virtual DbSet<ContributionUsage> ContributionUsages { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<LoanBalance> LoanBalances { get; set; }

    public virtual DbSet<LoanRequest> LoanRequests { get; set; }

    public virtual DbSet<LoansType> LoansTypes { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SavingsBalance> SavingsBalances { get; set; }

    public virtual DbSet<SavingsRequest> SavingsRequests { get; set; }

    public virtual DbSet<SavingsType> SavingsTypes { get; set; }

    public virtual DbSet<TransactionLog> TransactionLogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<sp_GetLoanCalculation_Result> Sp_GetLoanCalculations { get; set; }

    public virtual DbSet<LoanCalculationType> LoanCalculationTypes { get; set; }

    public virtual DbSet<sp_GetContributionBalance_Result> Sp_GetContributionBalances { get; set; }

    public virtual DbSet<sp_GetSavingsBalance_Result> Sp_GetSavingsBalances { get; set; }

    public virtual DbSet<sp_GetLoanBalance_Result> Sp_GetLoanBalances { get; set; }

    public virtual DbSet<sp_GetAllActiveAgreements_Result> Sp_GetAllActiveAgreements { get; set; }

    public virtual DbSet<sp_GetAllActiveCategoriesWithAgreements_Results> Sp_GetAllActiveCategoriesWithAgreements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<sp_GetLoanCalculation_Result>().HasNoKey();
        modelBuilder.Entity<sp_GetContributionBalance_Result>().HasNoKey();
        modelBuilder.Entity<sp_GetSavingsBalance_Result>().HasNoKey();
        modelBuilder.Entity<sp_GetLoanBalance_Result>().HasNoKey();
        modelBuilder.Entity<sp_GetAllActiveAgreements_Result>().HasNoKey();
        modelBuilder.Entity<sp_GetAllActiveCategoriesWithAgreements_Results>().HasNoKey();

        modelBuilder.Entity<Agreement>(entity =>
        {
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.CategoryAgreement).WithMany(p => p.Agreements)
                .HasForeignKey(d => d.CategoryAgreementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Agreements_CategoryAgreements");
        });

        modelBuilder.Entity<Audit>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Audit");

            entity.Property(e => e.PersonId).HasMaxLength(12);

            entity.HasOne(d => d.Person).WithMany()
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Audit_Users");
        });

        modelBuilder.Entity<Beneficiary>(entity =>
        {
            entity.Property(e => e.BeneficiaryKin).HasMaxLength(20);
            entity.Property(e => e.BeneficiaryName).HasMaxLength(40);
            entity.Property(e => e.BeneficiaryNumberId).HasMaxLength(12);
            entity.Property(e => e.BeneficiaryPercentage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PersonId).HasMaxLength(12);

            entity.HasOne(d => d.Person).WithMany(p => p.Beneficiaries)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Beneficiaries_Users");
        });

        modelBuilder.Entity<Canton>(entity =>
        {
            entity.ToTable("Canton");

            entity.Property(e => e.CantonId).ValueGeneratedNever();
            entity.Property(e => e.CantonName).HasMaxLength(50);

            entity.HasOne(d => d.Province).WithMany(p => p.Cantons)
                .HasForeignKey(d => d.ProvinceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Canton_Province");
        });

        modelBuilder.Entity<CategoryAgreement>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(100);
        });

        modelBuilder.Entity<ContributionBalance>(entity =>
        {
            entity.Property(e => e.DeductedDate).HasColumnType("date");
            entity.Property(e => e.EmployeeContribution).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EmployerContribution).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PersonId).HasMaxLength(12);

            entity.HasOne(d => d.Person).WithMany(p => p.ContributionBalances)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContributionBalances_Users");
        });

        modelBuilder.Entity<ContributionUsage>(entity =>
        {
            entity.ToTable("ContributionUsage");

            entity.Property(e => e.Description).HasMaxLength(40);
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.ToTable("District");

            entity.Property(e => e.DistrictId).ValueGeneratedNever();
            entity.Property(e => e.DistrictName).HasMaxLength(50);

            entity.HasOne(d => d.Canton).WithMany(p => p.Districts)
                .HasForeignKey(d => d.CantonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_District_Canton");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK_[Documents");
        });

        modelBuilder.Entity<LoanBalance>(entity =>
        {
            entity.Property(e => e.BeginningBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CumulativeInterest).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.EndingBalance).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ExtraPayment).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Interest).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.InterestRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PaymentDate).HasColumnType("date");
            entity.Property(e => e.PersonId).HasMaxLength(12);
            entity.Property(e => e.Principal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ScheduledPayment).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPayment).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.LoanRequest).WithMany(p => p.LoanBalances)
                .HasForeignKey(d => d.LoanRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoanBalances_LoanRequests");

            entity.HasOne(d => d.Person).WithMany(p => p.LoanBalances)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoanBalances_Users");
        });

        modelBuilder.Entity<LoanRequest>(entity =>
        {
            entity.HasKey(e => e.LoanRequestId).HasName("PK_LoanRequest");

            entity.Property(e => e.AmountRequested).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ApprovedDate).HasColumnType("date");
            entity.Property(e => e.BankAccount).HasMaxLength(25);
            entity.Property(e => e.PersonId).HasMaxLength(12);
            entity.Property(e => e.RequestedDate).HasColumnType("date");
            entity.Property(e => e.ReviewRequiredDate).HasColumnType("date");

            entity.HasOne(d => d.LoansType).WithMany(p => p.LoanRequests)
                .HasForeignKey(d => d.LoansTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoanRequest_LoansType");

            entity.HasOne(d => d.Person).WithMany(p => p.LoanRequests)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoanRequests_Users");
        });

        modelBuilder.Entity<LoansType>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.InterestRate).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PercentageEmployeeCont).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PercentageEmployerCont).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.ContributionUsage).WithMany(p => p.LoansTypes)
                .HasForeignKey(d => d.ContributionUsageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LoansType_ContributionUsage");
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.ToTable("Login");

            entity.Property(e => e.PersonId).HasMaxLength(12);

            entity.HasOne(d => d.Person).WithMany(p => p.Logins)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Login_Users");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.ToTable("Province");

            entity.Property(e => e.ProvinceId).ValueGeneratedNever();
            entity.Property(e => e.ProvinceName).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.RoleDescription).HasMaxLength(20);
        });

        modelBuilder.Entity<SavingsBalance>(entity =>
        {
            entity.Property(e => e.LastAmountDeducted).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LastDeductedDate).HasColumnType("date");
            entity.Property(e => e.PersonId).HasMaxLength(12);

            entity.HasOne(d => d.Person).WithMany(p => p.SavingsBalances)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SavingsBalances_Users");

            entity.HasOne(d => d.SavingsRequest).WithMany(p => p.SavingsBalances)
                .HasForeignKey(d => d.SavingsRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SavingsBalances_SavingsRequests");
        });

        modelBuilder.Entity<SavingsRequest>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ApplicationDate).HasColumnType("date");
            entity.Property(e => e.ApprovedDate).HasColumnType("date");
            entity.Property(e => e.PersonId).HasMaxLength(12);



            entity.HasOne(d => d.Person).WithMany(p => p.SavingsRequests)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SavingsRequests_Users");

            entity.HasOne(d => d.SavingsType).WithMany(p => p.SavingsRequests)
                .HasForeignKey(d => d.SavingsTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SavingsRequest_SavingsType");
        });

        modelBuilder.Entity<SavingsType>(entity =>
        {
            entity.Property(e => e.ApplicationDeadline).HasColumnType("date");
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.EndDate).HasColumnType("date");
            entity.Property(e => e.StartDate).HasColumnType("date");
        });

        modelBuilder.Entity<TransactionLog>(entity =>
        {
            entity.Property(e => e.PersonId).HasMaxLength(12);
            entity.Property(e => e.TableName).HasMaxLength(50);
            entity.Property(e => e.TransactionDate).HasColumnType("datetime");

            entity.HasOne(d => d.Person).WithMany(p => p.TransactionLogs)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransactionLogs_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.PersonId);

            entity.ToTable(tb =>
                {
                    tb.HasTrigger("trg_AddUserLogin");
                    tb.HasTrigger("trg_PreventUserDeletion");
                });

            entity.HasIndex(e => e.EmailAddress, "IX_Email").IsUnique();

            entity.HasIndex(e => e.NumberId, "IX_NumberId").IsUnique();

            entity.Property(e => e.PersonId).HasMaxLength(12);
            entity.Property(e => e.Address1).HasMaxLength(150);
            entity.Property(e => e.Address2).HasMaxLength(150);
            entity.Property(e => e.ApprovedDate).HasColumnType("date");
            entity.Property(e => e.BankAccount).HasMaxLength(25);
            entity.Property(e => e.DateBirth).HasColumnType("date");
            entity.Property(e => e.EmailAddress).HasMaxLength(50);
            entity.Property(e => e.EnrollmentDate).HasColumnType("date");
            entity.Property(e => e.FirstName).HasMaxLength(20);
            entity.Property(e => e.LastName1).HasMaxLength(20);
            entity.Property(e => e.LastName2).HasMaxLength(20);
            entity.Property(e => e.Nationality).HasMaxLength(3);
            entity.Property(e => e.NumberId).HasMaxLength(12);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.PostalCode).HasMaxLength(10);
            entity.Property(e => e.RequestedDate).HasColumnType("date");
            entity.Property(e => e.WorkStartDate).HasColumnType("date");

            entity.HasOne(d => d.District).WithMany(p => p.Users)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_District");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Person_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
