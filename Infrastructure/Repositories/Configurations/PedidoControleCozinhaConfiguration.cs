using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repositories.Configurations
{
    public class PedidoControleCozinhaConfiguration : IEntityTypeConfiguration<PedidoControleCozinha>
    {

        public void Configure(EntityTypeBuilder<PedidoControleCozinha> builder)
        {

            builder.ToTable("PedidoControleCozinha");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(c => c.DataInclusao).HasColumnName("DataInclusao").HasColumnType("DATETIME").IsRequired();
            builder.Property(c => c.NomeCliente).HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(c => c.Status).HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(c => c.PedidoId).HasColumnType("INT").IsRequired();

            builder.HasOne(pcc => pcc.Pedido)
                .WithOne(p => p.PedidoControleCozinha)
                .HasForeignKey<PedidoControleCozinha>(pcc => pcc.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

        }

    }
}
