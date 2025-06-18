using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Web.Migrations
{
    public partial class AddIsSuperUserToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSuperUser",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuperUser",
                table: "Users");
        }
    }
}
