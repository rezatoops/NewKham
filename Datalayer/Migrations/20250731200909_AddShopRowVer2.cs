using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datalayer.Migrations
{
    /// <inheritdoc />
    public partial class AddShopRowVer2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "ShopDesigns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BannerWonderImgId",
                table: "ShopDesigns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BannerWonderLink",
                table: "ShopDesigns",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnlyAvailable",
                table: "ShopDesigns",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ShopDesigns",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopDesigns_BannerWonderImgId",
                table: "ShopDesigns",
                column: "BannerWonderImgId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopDesigns_Medias_BannerWonderImgId",
                table: "ShopDesigns",
                column: "BannerWonderImgId",
                principalTable: "Medias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopDesigns_Medias_BannerWonderImgId",
                table: "ShopDesigns");

            migrationBuilder.DropIndex(
                name: "IX_ShopDesigns_BannerWonderImgId",
                table: "ShopDesigns");

            migrationBuilder.DropColumn(
                name: "BannerWonderImgId",
                table: "ShopDesigns");

            migrationBuilder.DropColumn(
                name: "BannerWonderLink",
                table: "ShopDesigns");

            migrationBuilder.DropColumn(
                name: "IsOnlyAvailable",
                table: "ShopDesigns");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ShopDesigns");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ShopDesigns",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
