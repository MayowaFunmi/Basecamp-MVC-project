using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BasecampMVC.Migrations
{
    /// <inheritdoc />
    public partial class threadMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessageThreads_ProjectId",
                table: "MessageThreads");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "MessageThreads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MessageThreads_ProjectId",
                table: "MessageThreads",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MessageThreads_ProjectId",
                table: "MessageThreads");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "MessageThreads");

            migrationBuilder.CreateIndex(
                name: "IX_MessageThreads_ProjectId",
                table: "MessageThreads",
                column: "ProjectId",
                unique: true);
        }
    }
}
