using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamProject.Data.Migrations
{
    public partial class skata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostInterest");

            migrationBuilder.AddColumn<int>(
                name: "InterestID",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PostID",
                table: "Interests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_InterestID",
                table: "Posts",
                column: "InterestID");

            migrationBuilder.CreateIndex(
                name: "IX_Interests_PostID",
                table: "Interests",
                column: "PostID");

            migrationBuilder.AddForeignKey(
                name: "FK_Interests_Posts_PostID",
                table: "Interests",
                column: "PostID",
                principalTable: "Posts",
                principalColumn: "PostID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Interests_InterestID",
                table: "Posts",
                column: "InterestID",
                principalTable: "Interests",
                principalColumn: "InterestID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interests_Posts_PostID",
                table: "Interests");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Interests_InterestID",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_InterestID",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Interests_PostID",
                table: "Interests");

            migrationBuilder.DropColumn(
                name: "InterestID",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "PostID",
                table: "Interests");

            migrationBuilder.CreateTable(
                name: "PostInterest",
                columns: table => new
                {
                    PostId = table.Column<int>(nullable: false),
                    InterestId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostInterest", x => new { x.PostId, x.InterestId });
                    table.ForeignKey(
                        name: "FK_PostInterest_Interests_InterestId",
                        column: x => x.InterestId,
                        principalTable: "Interests",
                        principalColumn: "InterestID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostInterest_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostInterest_InterestId",
                table: "PostInterest",
                column: "InterestId");
        }
    }
}
