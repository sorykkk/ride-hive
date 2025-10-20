using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RideHiveApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnerEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostItems_Owner_OwnerId",
                table: "PostItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Owner",
                table: "Owner");

            migrationBuilder.RenameTable(
                name: "Owner",
                newName: "Owners");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Owners",
                table: "Owners",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Owners_Name",
                table: "Owners",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_PostItems_Owners_OwnerId",
                table: "PostItems",
                column: "OwnerId",
                principalTable: "Owners",
                principalColumn: "OwnerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostItems_Owners_OwnerId",
                table: "PostItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Owners",
                table: "Owners");

            migrationBuilder.DropIndex(
                name: "IX_Owners_Name",
                table: "Owners");

            migrationBuilder.RenameTable(
                name: "Owners",
                newName: "Owner");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Owner",
                table: "Owner",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostItems_Owner_OwnerId",
                table: "PostItems",
                column: "OwnerId",
                principalTable: "Owner",
                principalColumn: "OwnerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
