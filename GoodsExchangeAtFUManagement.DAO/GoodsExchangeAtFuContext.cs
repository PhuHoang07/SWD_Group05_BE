using System;
using System.Collections.Generic;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GoodsExchangeAtFUManagement.DAO
{

    public partial class GoodsExchangeAtFuContext : DbContext
    {
        public GoodsExchangeAtFuContext()
        {
        }

        public GoodsExchangeAtFuContext(DbContextOptions<GoodsExchangeAtFuContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Campus> Campuses { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Chat> Chats { get; set; }

        public virtual DbSet<ChatDetail> ChatDetails { get; set; }

        public virtual DbSet<CoinPack> CoinPacks { get; set; }

        public virtual DbSet<CoinTransaction> CoinTransactions { get; set; }

        public virtual DbSet<Otpcode> Otpcodes { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<PostMode> PostModes { get; set; }

        public virtual DbSet<ProductImage> ProductImages { get; set; }

        public virtual DbSet<ProductPost> ProductPosts { get; set; }

        public virtual DbSet<ProductTransaction> ProductTransactions { get; set; }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<Report> Reports { get; set; }

        public virtual DbSet<User> Users { get; set; }

        private string GetConnectionString()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();
            return configuration["ConnectionStrings:DefaultConnectionString"];
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Campus>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Campus__3214EC07ACC0EA5C");

                entity.ToTable("Campus");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Category__3214EC07AC9651E4");

                entity.ToTable("Category");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Chat__3214EC074A13EA49");

                entity.ToTable("Chat");

                entity.HasIndex(e => e.ProductPostId, "idx_chat_productpostid");

                entity.HasIndex(e => e.BuyerId, "idx_chat_userid");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.BuyerId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.ProductPostId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.Buyer).WithMany(p => p.Chats)
                    .HasForeignKey(d => d.BuyerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Chat__BuyerId__59063A47");

                entity.HasOne(d => d.ProductPost).WithMany(p => p.Chats)
                    .HasForeignKey(d => d.ProductPostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Chat__ProductPos__571DF1D5");
            });

            modelBuilder.Entity<ChatDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ChatDeta__3214EC078E054CC4");

                entity.HasIndex(e => e.ChatId, "idx_chatdetails_chatid");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.ChatId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Message).HasMaxLength(500);
                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.HasOne(d => d.Chat).WithMany(p => p.ChatDetails)
                    .HasForeignKey(d => d.ChatId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChatDetai__ChatI__5812160E");
            });

            modelBuilder.Entity<CoinPack>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__CoinPack__3214EC070CD21169");

                entity.ToTable("CoinPack");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.Price)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<CoinTransaction>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__CoinTran__3214EC07779433C4");

                entity.ToTable("CoinTransaction");

                entity.HasIndex(e => e.UserId, "idx_cointransaction_userid");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.CoinPackId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.TransactAt).HasColumnType("datetime");
                entity.Property(e => e.UserId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.CoinPack).WithMany(p => p.CoinTransactions)
                    .HasForeignKey(d => d.CoinPackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CoinTrans__CoinP__5CD6CB2B");

                entity.HasOne(d => d.User).WithMany(p => p.CoinTransactions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CoinTrans__UserI__5DCAEF64");
            });

            modelBuilder.Entity<Otpcode>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__OTPCode__3214EC0726F84CAA");

                entity.ToTable("OTPCode");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Otp)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasColumnName("OTP");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Otpcodes)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OTPCode__Created__59FA5E80");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Payment__3214EC0789059F77");

                entity.ToTable("Payment");

                entity.HasIndex(e => e.PostModeId, "idx_payment_postmodeid");

                entity.HasIndex(e => e.ProductPostId, "idx_payment_productpostid");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.PaymentDate).HasColumnType("datetime");
                entity.Property(e => e.PostModeId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Price)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.ProductPostId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.PostMode).WithMany(p => p.Payments)
                    .HasForeignKey(d => d.PostModeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Payment__PostMod__5535A963");

                entity.HasOne(d => d.ProductPost).WithMany(p => p.Payments)
                    .HasForeignKey(d => d.ProductPostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Payment__Product__52593CB8");
            });

            modelBuilder.Entity<PostMode>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__PostMode__3214EC073375D5C2");

                entity.ToTable("PostMode");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Duration)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Price)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Type).HasMaxLength(100);
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ProductI__3214EC0759F32378");

                entity.HasIndex(e => e.ProductPostId, "idx_productimages_productpostid");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.ProductPostId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Url)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProductPost).WithMany(p => p.ProductImages)
                    .HasForeignKey(d => d.ProductPostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIm__Produ__534D60F1");
            });

            modelBuilder.Entity<ProductPost>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ProductP__3214EC07FE12B541");

                entity.ToTable("ProductPost");

                entity.HasIndex(e => e.CategoryId, "idx_productpost_categoryid");

                entity.HasIndex(e => e.CreatedBy, "idx_productpost_userid");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.CampusId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.CategoryId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Description).HasMaxLength(2048);
                entity.Property(e => e.ExpiredDate).HasColumnType("datetime");
                entity.Property(e => e.PostModeId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Price)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Title).HasMaxLength(100);

                entity.HasOne(d => d.Campus).WithMany(p => p.ProductPosts)
                    .HasForeignKey(d => d.CampusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductPost_Campus");

                entity.HasOne(d => d.Category).WithMany(p => p.ProductPosts)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductPo__Categ__5629CD9C");

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.ProductPosts)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductPo__Creat__5070F446");

                entity.HasOne(d => d.PostMode).WithMany(p => p.ProductPosts)
                    .HasForeignKey(d => d.PostModeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductPo__PostM__5441852A");
            });

            modelBuilder.Entity<ProductTransaction>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__ProductT__3214EC07B84A33F3");

                entity.ToTable("ProductTransaction");

                entity.HasIndex(e => e.BuyerId, "idx_producttransaction_buyerid");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.BuyerId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Price)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.ProductPostId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.TransactAt).HasColumnType("datetime");

                entity.HasOne(d => d.Buyer).WithMany(p => p.ProductTransactions)
                    .HasForeignKey(d => d.BuyerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductTransaction_User");

                entity.HasOne(d => d.ProductPost).WithMany(p => p.ProductTransactions)
                    .HasForeignKey(d => d.ProductPostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductTr__Produ__5AEE82B9");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("RefreshToken");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.ExpiredDate).HasColumnType("datetime");
                entity.Property(e => e.Token)
                    .HasMaxLength(128)
                    .IsUnicode(false);
                entity.Property(e => e.UserId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RefreshToken_User");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Report__3214EC07C26DF6FF");

                entity.ToTable("Report");

                entity.HasIndex(e => e.ProductPostId, "idx_report_productpostid");

                entity.HasIndex(e => e.CreatedBy, "idx_report_userid");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Content).HasMaxLength(500);
                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Date).HasColumnType("datetime");
                entity.Property(e => e.ProductPostId)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Reports)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Report__CreatedB__4F7CD00D");

                entity.HasOne(d => d.ProductPost).WithMany(p => p.Reports)
                    .HasForeignKey(d => d.ProductPostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Report__ProductP__5165187F");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__User__3214EC0721D45743");

                entity.ToTable("User");

                entity.HasIndex(e => e.Email, "idx_user_email");

                entity.Property(e => e.Id)
                    .HasMaxLength(36)
                    .IsUnicode(false)
                    .IsFixedLength();
                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Fullname).HasMaxLength(100);
                entity.Property(e => e.Password)
                    .HasMaxLength(256)
                    .IsUnicode(false);
                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(12)
                    .IsUnicode(false);
                entity.Property(e => e.Role)
                    .HasMaxLength(36)
                    .IsUnicode(false);
                entity.Property(e => e.Salt)
                    .HasMaxLength(256)
                    .IsUnicode(false);
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}