using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Novvi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FUNCIONARIOS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Salario = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    Ativo = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FUNCIONARIOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PRODUTOS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    Preco = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    Ativo = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUTOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USUARIOS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Nome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Ativo = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USUARIOS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ENDERECOS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Cep = table.Column<string>(type: "NVARCHAR2(8)", maxLength: 8, nullable: false),
                    Complemento = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    IdUsuario = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ENDERECOS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ENDERECOS_USUARIOS_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "USUARIOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEDIDOS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Data = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Frete = table.Column<decimal>(type: "NUMBER(18,2)", nullable: false),
                    IdUsuario = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    IdFuncionario = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Ativo = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEDIDOS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PEDIDOS_FUNCIONARIOS_IdFuncionario",
                        column: x => x.IdFuncionario,
                        principalTable: "FUNCIONARIOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PEDIDOS_USUARIOS_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "USUARIOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PAGAMENTOS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TipoPagamento = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Horario = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    IdPedido = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAGAMENTOS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PAGAMENTOS_PEDIDOS_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "PEDIDOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PEDIDO_PRODUTO",
                columns: table => new
                {
                    PedidosId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProdutosId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PEDIDO_PRODUTO", x => new { x.PedidosId, x.ProdutosId });
                    table.ForeignKey(
                        name: "FK_PEDIDO_PRODUTO_PEDIDOS_PedidosId",
                        column: x => x.PedidosId,
                        principalTable: "PEDIDOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PEDIDO_PRODUTO_PRODUTOS_ProdutosId",
                        column: x => x.ProdutosId,
                        principalTable: "PRODUTOS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ENDERECOS_IdUsuario",
                table: "ENDERECOS",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_PAGAMENTOS_IdPedido",
                table: "PAGAMENTOS",
                column: "IdPedido",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PEDIDO_PRODUTO_ProdutosId",
                table: "PEDIDO_PRODUTO",
                column: "ProdutosId");

            migrationBuilder.CreateIndex(
                name: "IX_PEDIDOS_IdFuncionario",
                table: "PEDIDOS",
                column: "IdFuncionario");

            migrationBuilder.CreateIndex(
                name: "IX_PEDIDOS_IdUsuario",
                table: "PEDIDOS",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIOS_Email",
                table: "USUARIOS",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ENDERECOS");

            migrationBuilder.DropTable(
                name: "PAGAMENTOS");

            migrationBuilder.DropTable(
                name: "PEDIDO_PRODUTO");

            migrationBuilder.DropTable(
                name: "PEDIDOS");

            migrationBuilder.DropTable(
                name: "PRODUTOS");

            migrationBuilder.DropTable(
                name: "FUNCIONARIOS");

            migrationBuilder.DropTable(
                name: "USUARIOS");
        }
    }
}
