using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repositories.Configurations
{
    public class PedidoItemConfiguration : IEntityTypeConfiguration<PedidoItem>
    {

        public void Configure(EntityTypeBuilder<PedidoItem> builder)
        {

            builder.ToTable("PedidoItem");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(c => c.DataInclusao).HasColumnName("DataInclusao").HasColumnType("DATETIME").IsRequired();
            builder.Property(c => c.PrecoTotal).HasColumnType("DECIMAL(18,2)").IsRequired();
            builder.Property(c => c.Quantidade).HasColumnType("INT").IsRequired();
            builder.Property(c => c.PedidoId).HasColumnType("INT").IsRequired();
            builder.Property(c => c.ProdutoId).HasColumnType("INT").IsRequired();

            builder.HasOne(pi => pi.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(pi => pi.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pi => pi.Produto)
            .WithMany(prod => prod.PedidoItens)
            .HasForeignKey(pi => pi.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
