using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datalayer.Migrations
{
    /// <inheritdoc />
    public partial class change : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariableAttributes_Variables_VariableId",
                table: "ProductVariableAttributes");

            migrationBuilder.AlterColumn<int>(
                name: "VariableId",
                table: "ProductVariableAttributes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariableAttributes_Variables_VariableId",
                table: "ProductVariableAttributes",
                column: "VariableId",
                principalTable: "Variables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariableAttributes_Variables_VariableId",
                table: "ProductVariableAttributes");

            migrationBuilder.AlterColumn<int>(
                name: "VariableId",
                table: "ProductVariableAttributes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariableAttributes_Variables_VariableId",
                table: "ProductVariableAttributes",
                column: "VariableId",
                principalTable: "Variables",
                principalColumn: "Id");
        }
    }
}
