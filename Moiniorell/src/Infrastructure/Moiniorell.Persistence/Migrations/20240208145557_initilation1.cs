using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Moiniorell.Persistence.Migrations
{
    public partial class initilation1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_CommentedPostId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_CommentedPostId",
                table: "Comments",
                column: "CommentedPostId",
                principalTable: "Posts",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_CommentedPostId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_CommentedPostId",
                table: "Comments",
                column: "CommentedPostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
