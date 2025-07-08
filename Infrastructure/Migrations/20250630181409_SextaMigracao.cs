﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiapTechChallenge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SextaMigracao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PedidoControleCozinha",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PedidoId = table.Column<int>(type: "INT", nullable: false),
                    NomeCliente = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    Status = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    DataInclusao = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoControleCozinha", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoControleCozinha_Pedido_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PedidoControleCozinha_PedidoId",
                table: "PedidoControleCozinha",
                column: "PedidoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PedidoControleCozinha");
        }
    }
}
