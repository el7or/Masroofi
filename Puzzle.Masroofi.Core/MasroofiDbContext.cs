using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Puzzle.Masroofi.Core.Models;

namespace Puzzle.Masroofi.Core
{
    public partial class MasroofiDbContext : DbContext
    {
        private readonly IConfiguration configuration;

        public MasroofiDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public MasroofiDbContext(IConfiguration configuration, DbContextOptions<MasroofiDbContext> options)
                        : base(options)
        {
            this.configuration = configuration;
        }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<ActionsInRoles> ActionsInRoles { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<SystemPage> SystemPages { get; set; }
        public virtual DbSet<SystemPageAction> SystemPageActions { get; set; }
        public virtual DbSet<Parent> Parents { get; set; }
        public virtual DbSet<ParentWalletTransaction> ParentWalletTransactions { get; set; }
        public virtual DbSet<ParentLoginHistory> ParentLoginHistories { get; set; }
        public virtual DbSet<ParentPinCodeHistory> ParentPinCodeHistories { get; set; }
        public virtual DbSet<Son> Sons { get; set; }
        public virtual DbSet<ATMCard> ATMCards { get; set; }
        public virtual DbSet<ATMCardHistory> ATMCardHistories { get; set; }
        public virtual DbSet<ATMCardTransaction> ATMCardTransactions { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
        public virtual DbSet<POSMachine> POSMachines { get; set; }
        public virtual DbSet<POSMachineLoginHistory> POSMachineLoginHistories { get; set; }
        public virtual DbSet<POSMachinePinCodeHistory> POSMachinePinCodeHistories { get; set; }
        public virtual DbSet<POSMachineTransaction> POSMachineTransactions { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<CommissionSetting> CommissionSettings { get; set; }
        public virtual DbSet<TransactionCommission> TransactionCommissions { get; set; }
        public virtual DbSet<Governorate> Governorates { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("CompoundDbConnection"), x => x.UseNetTopologySuite());
            }

            optionsBuilder.UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("Users");

                entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Image).HasMaxLength(200);

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.CreationUser).IsRequired();

                entity.Property(e => e.IsActive);

                entity.Property(e => e.IsDeleted);

                entity.Property(e => e.IsVerified);

                entity.Property(e => e.Password).IsRequired().HasMaxLength(1000);

                entity.Property(e => e.UserType);

                entity.Property(e => e.Username).IsRequired().HasMaxLength(200);

                entity.Property(e => e.NameAr).IsRequired().HasMaxLength(200);

                entity.Property(e => e.NameEn).IsRequired().HasMaxLength(200);

                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))").IsRequired();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))").IsRequired();

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles");

                entity.Property(e => e.RoleId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.RoleArabicName).IsRequired().HasMaxLength(100);

                entity.Property(e => e.RoleEnglishName).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRoles");

                entity.Property(e => e.UserRoleId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.RoleId);

                entity.Property(e => e.UserId);

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.CreationUser).IsRequired();

                entity.Property(e => e.ModificationDate).HasColumnType("datetime");

                entity.HasOne(d => d.Role)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.Property(e => e.RefreshTokenId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.UserId);

                entity.Property(e => e.UserType);

                entity.Property(e => e.Created).HasColumnType("datetime");

                entity.Property(e => e.CreatedByIp).IsRequired().HasMaxLength(50);

                entity.Property(e => e.Expires).HasColumnType("datetime");

                entity.Property(e => e.ReplacedByToken).HasMaxLength(1000);

                entity.Property(e => e.Revoked).HasColumnType("datetime");

                entity.Property(e => e.RevokedByIp).HasMaxLength(50);

                entity.Property(e => e.Token).IsRequired().HasMaxLength(1000);
            });

            modelBuilder.Entity<ActionsInRoles>(entity =>
            {
                entity.Property(e => e.ActionsInRolesId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.RoleId);

                entity.Property(e => e.SystemPageActionId);

                entity.HasOne(d => d.Role)
                        .WithMany(p => p.ActionsInRoles)
                        .HasForeignKey(d => d.RoleId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SystemPageAction)
                        .WithMany(p => p.ActionsInRoles)
                        .HasForeignKey(d => d.SystemPageActionId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SystemPageAction>(entity =>
            {
                entity.Property(e => e.SystemPageActionId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.SystemPageId);

                entity.HasOne(d => d.SystemPage)
                        .WithMany(p => p.SystemPageActions)
                        .HasForeignKey(d => d.SystemPageId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

            });

            modelBuilder.Entity<SystemPage>(entity =>
            {
                entity.Property(e => e.SystemPageId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ParentPageId);

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))").IsRequired();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))").IsRequired();

                entity.HasQueryFilter(m => !m.IsDeleted);

                entity.HasOne(d => d.ParentPage)
                        .WithMany(p => p.SubPages)
                        .HasForeignKey(d => d.ParentPageId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

            });

            modelBuilder.Entity<Parent>(entity =>
            {
                entity.HasKey(e => e.ParentId);

                entity.ToTable("Parents");

                entity.Property(e => e.ParentId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.PinCode).IsRequired();

                entity.Property(e => e.WalletNumber).ValueGeneratedOnAdd().UseIdentityColumn(100001, 1);

                entity.Property(e => e.FullNameAr).HasMaxLength(200).IsRequired();

                entity.Property(e => e.FullNameEn).HasMaxLength(200).IsRequired();

                entity.Property(e => e.Gender).IsRequired();

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.CurrentBalance).HasPrecision(18, 2).HasDefaultValueSql("((0))")
                .IsConcurrencyToken();

                entity.Property(e => e.Timestamp).IsRowVersion();

                entity.Property(e => e.Birthdate).HasColumnType("date");

                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.Property(e => e.CreationUser).IsRequired();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))").IsRequired();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))").IsRequired();

                entity.HasOne(d => d.City)
                        .WithMany()
                        .HasForeignKey(d => d.CityId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<ParentLoginHistory>(entity =>
            {
                entity.HasKey(e => e.ParentLoginHistoryId);

                entity.ToTable("ParentLoginHistories");

                entity.Property(e => e.ParentLoginHistoryId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.LoginDate).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.HasOne(d => d.Parent)
                        .WithMany(d => d.ParentLoginHistories)
                        .HasForeignKey(d => d.ParentId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ParentPinCodeHistory>(entity =>
            {
                entity.HasKey(e => e.ParentPinCodeHistoryId);

                entity.ToTable("ParentPinCodeHistories");

                entity.Property(e => e.ParentPinCodeHistoryId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.PinCode).HasMaxLength(6).IsRequired();

                entity.Property(e => e.HistoryType).IsRequired();

                entity.Property(e => e.UpdatedOn).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.Property(e => e.UpdatedBy).IsRequired();

                entity.HasOne(d => d.Parent)
                        .WithMany(d => d.ParentPinCodeHistories)
                        .HasForeignKey(d => d.ParentId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Son>(entity =>
            {
                entity.HasKey(e => e.SonId);

                entity.ToTable("Sons");

                entity.Property(e => e.SonId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.SonNameAr).HasMaxLength(200).IsRequired();

                entity.Property(e => e.SonNameEn).HasMaxLength(200).IsRequired();

                entity.Property(e => e.Gender).IsRequired();

                entity.Property(e => e.Birthdate).HasColumnType("date").IsRequired();

                entity.Property(e => e.DailyLimit).HasPrecision(18, 2).IsRequired();

                entity.Property(e => e.CurrentBalance).HasPrecision(18, 2).HasDefaultValueSql("((0))")
                .IsConcurrencyToken();

                entity.Property(e => e.Timestamp).IsRowVersion();

                entity.Property(e => e.ImageUrl).IsRequired();

                entity.Property(e => e.Mobile).HasMaxLength(20);

                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.Property(e => e.CreationUser).IsRequired();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))").IsRequired();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))").IsRequired();

                entity.HasOne(d => d.Parent)
                        .WithMany(d => d.Sons)
                        .HasForeignKey(d => d.ParentId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.CurrentATMCard)
                        .WithMany()
                        .HasForeignKey(d => d.CurrentATMCardId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<ATMCard>(entity =>
            {
                entity.HasKey(e => e.ATMCardId);

                entity.ToTable("ATMCards");

                entity.Property(e => e.ATMCardId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.FirstName).HasMaxLength(200).IsRequired();

                entity.Property(e => e.MiddleName).HasMaxLength(200).IsRequired();

                entity.Property(e => e.LastName).HasMaxLength(200).IsRequired();

                entity.Property(e => e.ShortNumber).HasMaxLength(6);

                entity.Property(e => e.CardNumber).HasMaxLength(16);

                entity.Property(e => e.SecurityCode).HasMaxLength(3);

                entity.Property(e => e.ExpiryDate).HasColumnType("date");

                entity.Property(e => e.Status).HasDefaultValueSql("((1))").IsRequired();

                entity.Property(e => e.RejectedReason).HasMaxLength(500);

                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.Property(e => e.CreationUser).IsRequired();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))").IsRequired();

                entity.Property(e => e.CardId).ValueGeneratedOnAdd().UseIdentityColumn(100001, 1);

                entity.HasQueryFilter(m => !m.IsDeleted);

                entity.HasOne(d => d.Son)
                        .WithMany(d => d.ATMCards)
                        .HasForeignKey(d => d.SonId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ATMCardType)
                         .WithMany(d => d.ATMCards)
                         .HasForeignKey(d => d.ATMCardTypeId)
                         .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ATMCardType>(entity =>
            {
                entity.HasKey(e => e.ATMCardTypeId);

                entity.ToTable("ATMCardTypes");

                entity.Property(e => e.ATMCardTypeId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.TypeNameAr).HasMaxLength(200).IsRequired();

                entity.Property(e => e.TypeNameEn).HasMaxLength(200).IsRequired();

                entity.Property(e => e.FrontImageUrl).IsRequired();

                entity.Property(e => e.BackImageUrl).IsRequired();

                entity.Property(e => e.Cost).HasPrecision(18, 2).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.Property(e => e.CreationUser).IsRequired();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))").IsRequired();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))").IsRequired();

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<ATMCardHistory>(entity =>
            {
                entity.HasKey(e => e.ATMCardHistoryId);

                entity.ToTable("ATMCardHistories");

                entity.Property(e => e.ATMCardHistoryId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.Password).HasMaxLength(3);

                entity.Property(e => e.UpdatedOn).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.Property(e => e.UpdatedBy).IsRequired();

                entity.HasOne(d => d.ATMCard)
                        .WithMany(d => d.ATMCardHistories)
                        .HasForeignKey(d => d.ATMCardId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.HasKey(e => e.VendorId);

                entity.ToTable("Vendors");

                entity.Property(e => e.VendorId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.FullNameAr).HasMaxLength(200).IsRequired();

                entity.Property(e => e.FullNameEn).HasMaxLength(200).IsRequired();

                entity.Property(e => e.Phone1).HasMaxLength(20).IsRequired();

                entity.Property(e => e.ResponsiblePerson).HasMaxLength(200).IsRequired();

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Address).HasMaxLength(500).IsRequired();

                entity.Property(e => e.GoogleLocation).HasMaxLength(500).IsRequired();

                entity.Property(e => e.CurrentBalance).HasPrecision(18, 2).HasDefaultValueSql("((0))")
                .IsConcurrencyToken();

                entity.Property(e => e.Timestamp).IsRowVersion();

                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.Property(e => e.CreationUser).IsRequired();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))").IsRequired();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))").IsRequired();

                entity.HasOne(d => d.City)
                        .WithMany()
                        .HasForeignKey(d => d.CityId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<POSMachine>(entity =>
            {
                entity.HasKey(e => e.POSMachineId);

                entity.ToTable("POSMachines");

                entity.Property(e => e.POSMachineId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.Username).HasMaxLength(200).IsRequired();
                
                entity.HasIndex(e => e.Username).IsUnique();

                entity.Property(e => e.PinCode).IsRequired();

                entity.Property(e => e.POSModel).HasMaxLength(200);

                entity.Property(e => e.POSNumber).HasMaxLength(200);

                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.Property(e => e.CreationUser).IsRequired();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))").IsRequired();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))").IsRequired();

                entity.HasQueryFilter(m => !m.IsDeleted);

                entity.HasOne(d => d.Vendor)
                        .WithMany(d => d.POSMachines)
                        .HasForeignKey(d => d.VendorId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<POSMachineLoginHistory>(entity =>
            {
                entity.HasKey(e => e.POSMachineLoginHistoryId);

                entity.ToTable("POSMachineLoginHistories");

                entity.Property(e => e.POSMachineLoginHistoryId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.LoginDate).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.HasOne(d => d.POSMachine)
                        .WithMany(d => d.POSMachineLoginHistories)
                        .HasForeignKey(d => d.POSMachineId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<POSMachinePinCodeHistory>(entity =>
            {
                entity.HasKey(e => e.POSMachinePinCodeHistoryId);

                entity.ToTable("POSMachinePinCodeHistories");

                entity.Property(e => e.POSMachinePinCodeHistoryId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.PinCode).HasMaxLength(6).IsRequired();

                entity.Property(e => e.UpdatedOn).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.Property(e => e.UpdatedBy).IsRequired();

                entity.HasOne(d => d.POSMachine)
                        .WithMany(d => d.POSMachinePinCodeHistories)
                        .HasForeignKey(d => d.POSMachineId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ParentWalletTransaction>(entity =>
            {
                entity.HasKey(e => e.ParentWalletTransactionId);

                entity.ToTable("ParentWalletTransactions");

                entity.Property(e => e.ParentWalletTransactionId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.TransactionNumber).ValueGeneratedOnAdd();

                entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();

                entity.Property(e => e.PaymentType).IsRequired();

                entity.Property(e => e.TitleAr).HasMaxLength(200).IsRequired();

                entity.Property(e => e.TitleEn).HasMaxLength(200).IsRequired();

                entity.Property(e => e.DetailsAr).HasMaxLength(500).IsRequired();

                entity.Property(e => e.DetailsEn).HasMaxLength(500).IsRequired();

                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.Property(e => e.IsActive).IsRequired();

                entity.Property(e => e.TransactionReference).HasMaxLength(100);

                entity.Property(e => e.TransactionDataJson).HasMaxLength(1000);

                entity.HasOne(d => d.Parent)
                        .WithMany(d => d.ParentWalletTransactions)
                        .HasForeignKey(d => d.ParentId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Son)
                        .WithMany(d => d.ParentWalletTransactions)
                        .HasForeignKey(d => d.SonId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ATMCard)
                        .WithMany(d => d.ParentWalletTransactions)
                        .HasForeignKey(d => d.ATMCardId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Vendor)
                        .WithMany(d => d.ParentWalletTransactions)
                        .HasForeignKey(d => d.VendorId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.POSMachine)
                        .WithMany(d => d.ParentWalletTransactions)
                        .HasForeignKey(d => d.POSMachineId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.AdminUser)
                        .WithMany(d => d.ParentWalletTransactions)
                        .HasForeignKey(d => d.AdminUserId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ATMCardTransaction>(entity =>
            {
                entity.HasKey(e => e.ATMCardTransactionId);

                entity.ToTable("ATMCardTransactions");

                entity.Property(e => e.ATMCardTransactionId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.TransactionNumber).ValueGeneratedOnAdd();

                entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();

                entity.Property(e => e.PaymentType).IsRequired();

                entity.Property(e => e.TitleAr).HasMaxLength(200).IsRequired();

                entity.Property(e => e.TitleEn).HasMaxLength(200).IsRequired();

                entity.Property(e => e.DetailsAr).HasMaxLength(500).IsRequired();

                entity.Property(e => e.DetailsEn).HasMaxLength(500).IsRequired();

                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.HasOne(d => d.ATMCard)
                        .WithMany(d => d.ATMCardTransactions)
                        .HasForeignKey(d => d.ATMCardId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Son)
                        .WithMany(d => d.ATMCardTransactions)
                        .HasForeignKey(d => d.SonId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Parent)
                        .WithMany(d => d.ATMCardTransactions)
                        .HasForeignKey(d => d.ParentId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Vendor)
                        .WithMany(d => d.ATMCardTransactions)
                        .HasForeignKey(d => d.VendorId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.POSMachine)
                        .WithMany(d => d.ATMCardTransactions)
                        .HasForeignKey(d => d.POSMachineId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<POSMachineTransaction>(entity =>
            {
                entity.HasKey(e => e.POSMachineTransactionId);

                entity.ToTable("POSMachineTransactions");

                entity.Property(e => e.POSMachineTransactionId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.TransactionNumber).ValueGeneratedOnAdd();

                entity.Property(e => e.Amount).HasPrecision(18, 2).IsRequired();

                entity.Property(e => e.PaymentType).IsRequired();

                entity.Property(e => e.TitleAr).HasMaxLength(200).IsRequired();

                entity.Property(e => e.TitleEn).HasMaxLength(200).IsRequired();

                entity.Property(e => e.DetailsAr).HasMaxLength(500).IsRequired();

                entity.Property(e => e.DetailsEn).HasMaxLength(500).IsRequired();

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.HasOne(d => d.Vendor)
                        .WithMany(d => d.POSMachineTransactions)
                        .HasForeignKey(d => d.VendorId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.POSMachine)
                        .WithMany(d => d.POSMachineTransactions)
                        .HasForeignKey(d => d.POSMachineId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Parent)
                        .WithMany(d => d.POSMachineTransactions)
                        .HasForeignKey(d => d.ParentId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Son)
                        .WithMany(d => d.POSMachineTransactions)
                        .HasForeignKey(d => d.SonId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ATMCard)
                        .WithMany(d => d.POSMachineTransactions)
                        .HasForeignKey(d => d.ATMCardId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.AdminUser)
                        .WithMany(d => d.POSMachineTransactions)
                        .HasForeignKey(d => d.AdminUserId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ReferenceTransaction)
                        .WithMany(d => d.RefundTransactions)
                        .HasForeignKey(d => d.ReferenceTransactionId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CommissionSetting>(entity =>
            {
                entity.HasKey(e => e.CommissionSettingId);

                entity.ToTable("CommissionSettings");

                entity.Property(e => e.CommissionSettingId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.FromValue).HasPrecision(18, 2).IsRequired();

                entity.Property(e => e.ToValue).HasPrecision(18, 2).IsRequired();

                entity.Property(e => e.CommissionType).IsRequired();

                entity.Property(e => e.FixedCommission).HasPrecision(18, 2).IsRequired();

                entity.Property(e => e.PercentageCommission).IsRequired();

                entity.Property(e => e.VendorFixedCommission).HasPrecision(18, 2).IsRequired();

                entity.Property(e => e.VendorPercentageCommission).IsRequired();

                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.Property(e => e.CreationUser).IsRequired();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))").IsRequired();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))").IsRequired();

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<TransactionCommission>(entity =>
            {
                entity.HasKey(e => e.TransactionCommissionId);

                entity.ToTable("TransactionCommissions");

                entity.Property(e => e.TransactionCommissionId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.FixedCommissionValue).HasPrecision(18, 2).IsRequired();

                entity.Property(e => e.PercentageCommissionValue).HasPrecision(18, 2).IsRequired();

                entity.Property(e => e.TransactionValue).HasPrecision(18, 2).IsRequired();

                entity.Property(e => e.TransactionType).IsRequired();

                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.HasOne(d => d.ParentWalletTransaction)
                        .WithMany(d => d.TransactionCommissions)
                        .HasForeignKey(d => d.ParentWalletTransactionId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.POSMachineTransaction)
                        .WithMany(d => d.TransactionCommissions)
                        .HasForeignKey(d => d.POSMachineTransactionId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Vendor)
                        .WithMany(d => d.TransactionCommissions)
                        .HasForeignKey(d => d.VendorId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.POSMachine)
                        .WithMany(d => d.TransactionCommissions)
                        .HasForeignKey(d => d.POSMachineId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(e => e.CityId);

                entity.ToTable("Cities");

                entity.Property(e => e.CityNameAr).HasMaxLength(200).IsRequired();

                entity.Property(e => e.CityNameEn).HasMaxLength(200).IsRequired();

                entity.HasOne(d => d.Governorate)
                        .WithMany(d => d.Cities)
                        .HasForeignKey(d => d.GovernorateId)
                        .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Governorate>(entity =>
            {
                entity.HasKey(e => e.GovernorateId);

                entity.ToTable("Governorates");

                entity.Property(e => e.GovernorateNameAr).HasMaxLength(50).IsRequired();

                entity.Property(e => e.GovernorateNameEn).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<ParentMobileRegistration>(entity =>
            {
                entity.HasKey(e => e.ParentMobileRegistrationId);

                entity.ToTable("ParentMobileRegistrations");

                entity.Property(e => e.ParentMobileRegistrationId).HasDefaultValueSql("(NEWID())");

                entity.Property(e => e.RegisterId).IsRequired();

                entity.Property(e => e.RegisterType).HasMaxLength(50).IsRequired()
                .HasComment("Device type (Android,IOS,...)");

                entity.Property(e => e.CreationDate).HasDefaultValueSql("GETUTCDATE()").IsRequired();

                entity.Property(e => e.CreationUser).IsRequired();

                entity.Property(e => e.IsActive).HasDefaultValueSql("((1))").IsRequired();

                entity.Property(e => e.IsDeleted).HasDefaultValueSql("((0))").IsRequired();

                entity.HasOne(d => d.Parent)
                        .WithMany(d => d.ParentMobileRegistrations)
                        .HasForeignKey(d => d.ParentId)
                        .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasQueryFilter(m => !m.IsDeleted);
            });

            modelBuilder.Entity<ParentNotification>(entity =>
            {
                entity.HasKey(p => p.NotificationParentId);

                entity.ToTable("ParentNotifications");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(e => e.ModifiedDate)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true)
                    .IsRequired();

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false)
                    .IsRequired();

                entity.HasOne(q => q.Parent)
                    .WithMany(q => q.NotificationParents)
                    .HasForeignKey(q => q.ParentId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(q => q.NotificationSchedule)
                    .WithMany(q => q.ParentNotifications)
                    .HasForeignKey(q => q.NotificationScheduleId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<NotificationSchedule>(entity =>
            {
                entity.HasKey(p => p.NotificationScheduleId);

                entity.ToTable("NotificationSchedules");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(e => e.ModifiedDate)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedBy)
                    .HasMaxLength(50);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true)
                    .IsRequired();

                entity.Property(e => e.IsDeleted)
                    .HasDefaultValue(false)
                    .IsRequired();
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
