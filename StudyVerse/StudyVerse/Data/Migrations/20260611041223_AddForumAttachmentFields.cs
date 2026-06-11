using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyVerse.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddForumAttachmentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AttachmentFileName",
                table: "ForumPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentPath",
                table: "ForumPosts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "ForumPosts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentFileName",
                table: "ForumPosts");

            migrationBuilder.DropColumn(
                name: "AttachmentPath",
                table: "ForumPosts");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "ForumPosts");
        }
    }
}
