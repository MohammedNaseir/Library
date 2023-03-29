using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace library.Data.Migrations
{
    public partial class imagePublicID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePublicID",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePublicID",
                table: "Books");
        }
    }
}
