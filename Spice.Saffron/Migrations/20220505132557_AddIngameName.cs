using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spice.Saffron.Migrations
{
    public partial class AddIngameName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IngameName",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IngameName",
                table: "AspNetUsers");
        }
    }
}
