using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RideHiveApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOwnershipDocumentToFilePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnershipDocumentData",
                table: "CarItems");

            migrationBuilder.AddColumn<string>(
                name: "OwnershipDocumentPath",
                table: "CarItems",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnershipDocumentPath",
                table: "CarItems");

            migrationBuilder.AddColumn<byte[]>(
                name: "OwnershipDocumentData",
                table: "CarItems",
                type: "bytea",
                maxLength: 10485760,
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
