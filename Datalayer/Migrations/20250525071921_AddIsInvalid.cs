using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datalayer.Migrations
{
    /// <inheritdoc />
    public partial class AddIsInvalid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVariableAttributes_VariableId",
                table: "ProductVariableAttributes");

            migrationBuilder.AddColumn<bool>(
                name: "IsInvalid",
                table: "OrderProducts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariableAttributes_VariableId_ProductAttributeValueId",
                table: "ProductVariableAttributes",
                columns: new[] { "VariableId", "ProductAttributeValueId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductVariableAttributes_VariableId_ProductAttributeValueId",
                table: "ProductVariableAttributes");

            migrationBuilder.DropColumn(
                name: "IsInvalid",
                table: "OrderProducts");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariableAttributes_VariableId",
                table: "ProductVariableAttributes",
                column: "VariableId");
        }
    }
}
