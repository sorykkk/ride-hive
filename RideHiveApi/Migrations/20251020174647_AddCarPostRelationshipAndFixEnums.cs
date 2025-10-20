using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RideHiveApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCarPostRelationshipAndFixEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PostItems_CarId",
                table: "PostItems",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostItems_CarItems_CarId",
                table: "PostItems",
                column: "CarId",
                principalTable: "CarItems",
                principalColumn: "CarId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostItems_CarItems_CarId",
                table: "PostItems");

            migrationBuilder.DropIndex(
                name: "IX_PostItems_CarId",
                table: "PostItems");
        }
    }
}
