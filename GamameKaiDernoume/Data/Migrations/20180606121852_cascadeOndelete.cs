using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamProject.Data.Migrations
{
    public partial class cascadeOndelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_AspNetUsers_ReceiverId",
                table: "Friends");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_AspNetUsers_ReceiverId",
                table: "Friends",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_AspNetUsers_ReceiverId",
                table: "Friends");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_AspNetUsers_ReceiverId",
                table: "Friends",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
