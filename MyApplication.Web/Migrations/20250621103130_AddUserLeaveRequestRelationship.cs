using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApplication.Web.Migrations
{
    public partial class AddUserLeaveRequestRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_LeaveRequests_UserId",
                table: "LeaveRequests",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeaveRequests_Users_UserId",
                table: "LeaveRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeaveRequests_Users_UserId",
                table: "LeaveRequests");

            migrationBuilder.DropIndex(
                name: "IX_LeaveRequests_UserId",
                table: "LeaveRequests");
        }
    }
}
