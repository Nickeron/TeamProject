using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GamameKaiDernoume.Data.Migrations
{
    public partial class second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Interests_Posts_PostID",
                table: "Interests");

            migrationBuilder.DropIndex(
                name: "IX_Interests_PostID",
                table: "Interests");

            migrationBuilder.DropColumn(
                name: "PostID",
                table: "Interests");

            migrationBuilder.CreateTable(
                name: "PostInterest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PostID = table.Column<int>(nullable: true),
                    InterestID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostInterest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostInterest_Interests_InterestID",
                        column: x => x.InterestID,
                        principalTable: "Interests",
                        principalColumn: "InterestID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostInterest_Posts_PostID",
                        column: x => x.PostID,
                        principalTable: "Posts",
                        principalColumn: "PostID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostInterest_InterestID",
                table: "PostInterest",
                column: "InterestID");

            migrationBuilder.CreateIndex(
                name: "IX_PostInterest_PostID",
                table: "PostInterest",
                column: "PostID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostInterest");

            migrationBuilder.AddColumn<int>(
                name: "PostID",
                table: "Interests",
                nullable: true);

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
        }
    }
}
