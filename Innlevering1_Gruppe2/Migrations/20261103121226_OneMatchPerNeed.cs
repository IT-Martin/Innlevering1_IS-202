using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reachout.Migrations
{
    /// <inheritdoc />
    public partial class OneMatchPerNeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ResourceMatch_resource_need_id",
                table: "ResourceMatch",
                column: "resource_need_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ResourceMatch_resource_need_id",
                table: "ResourceMatch");
        }
    }
}
