using Entity.TableModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DAL.Configuration
{
    public class CallConfiguration : IEntityTypeConfiguration<Call>
    {
        public void Configure(EntityTypeBuilder<Call> builder)
        {
            
            builder.ToTable("Calls");

            
            builder.HasKey(x => x.Id);

            
            builder.Property(x => x.Id)
                .HasColumnType("int")
                .UseIdentityColumn(1, 1);

            
            // Name property removed


            builder.Property(x => x.Deleted)
                .HasColumnType("int")
                .HasDefaultValue(0);

            
            builder.Property(x => x.CallerId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.ReceiverId)
                .HasColumnType("int")
                .IsRequired();

            builder.Property(x => x.CallType)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50)
                .HasDefaultValue("voice")
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnType("nvarchar(50)")
                .HasMaxLength(50)
                .HasDefaultValue("ringing") 
                .IsRequired();

            builder.Property(x => x.StartedAt)
                .HasColumnType("datetime")
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();

            builder.Property(x => x.EndedAt)
                .HasColumnType("datetime")
                .IsRequired(false);

            builder.Property(x => x.Duration)
                .HasColumnType("int")
                .IsRequired(false);

          
            builder.HasIndex(x => new { x.Deleted })
                .IsUnique(false) // Deleted shouldn't be unique on its own generally, or maybe it was unique with Name. Assuming just index.
                .HasDatabaseName("idx_Deleted");

            
            builder.HasOne(x => x.Caller)
                .WithMany(x => x.InitiatedCalls) 
                .HasForeignKey(x => x.CallerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Receiver)
                .WithMany(x => x.ReceivedCalls) 
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            
            builder.HasIndex(x => x.CallerId)
                .HasDatabaseName("idx_CallerId");

            builder.HasIndex(x => x.ReceiverId)
                .HasDatabaseName("idx_ReceiverId");

            builder.HasIndex(x => x.StartedAt)
                .HasDatabaseName("idx_StartedAt");

            builder.HasIndex(x => x.Status)
                .HasDatabaseName("idx_Status");

           
            builder.HasIndex(x => new { x.CallerId, x.StartedAt })
                .HasDatabaseName("idx_Caller_Started");

            builder.HasIndex(x => new { x.ReceiverId, x.StartedAt })
                .HasDatabaseName("idx_Receiver_Started");
        }
    }
}