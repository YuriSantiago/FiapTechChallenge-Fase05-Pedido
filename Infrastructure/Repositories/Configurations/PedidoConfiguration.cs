using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Configurations
{
    public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {

        public void Configure(EntityTypeBuilder<Pedido> builder)
        {

            builder.ToTable("Pedido");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnType("INT").UseIdentityColumn();
            builder.Property(c => c.DataInclusao).HasColumnName("DataInclusao").HasColumnType("DATETIME").IsRequired();
            builder.Property(c => c.PrecoTotal).HasColumnType("DECIMAL(18,2)").IsRequired();
            builder.Property(c => c.Status).HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(r => r.TipoEntrega).HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(r => r.DescricaoCancelamento).HasColumnType("VARCHAR(150)");
            builder.Property(c => c.UsuarioId).HasColumnType("INT").IsRequired();

            builder.HasOne(c => c.Usuario)
                .WithMany(r => r.Pedidos)
                .HasPrincipalKey(r => r.Id);

        }

    }
}
