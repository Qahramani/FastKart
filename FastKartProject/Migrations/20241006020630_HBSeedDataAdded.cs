using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastKartProject.Migrations
{
    public partial class HBSeedDataAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "HomeBanners",
                columns: new[] { "Id", "BottomText", "ImageUrl", "Title", "TopText" },
                values: new object[] { 1, "hello", "1.jpg", "Main Banner", "Bye" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HomeBanners",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
