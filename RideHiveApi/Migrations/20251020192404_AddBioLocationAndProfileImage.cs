using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RideHiveApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBioLocationAndProfileImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ProfileImage",
                table: "AspNetUsers",
                type: "bytea",
                maxLength: 5242880,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageContentType",
                table: "AspNetUsers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImage",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfileImageContentType",
                table: "AspNetUsers");
        }
    }
}
