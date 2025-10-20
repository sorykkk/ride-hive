using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RideHiveApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBioAndLocationToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnershipDocumentData",
                table: "CarItems");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "CarImages");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "CarItems",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "OwnershipDocumentPath",
                table: "CarItems",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "CarImages",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "AspNetUsers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "AspNetUsers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Owner",
                columns: table => new
                {
                    OwnerId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owner", x => x.OwnerId);
                });

            migrationBuilder.CreateTable(
                name: "PostItems",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<string>(type: "text", nullable: false),
                    CarId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    SpecialRequirements = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Location = table.Column<string>(type: "text", nullable: false),
                    AvailableTimeSlots = table.Column<DateTime[]>(type: "timestamp with time zone[]", nullable: false),
                    PostedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Available = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostItems", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_PostItems_Owner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owner",
                        principalColumn: "OwnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostItems_OwnerId",
                table: "PostItems",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PostItems_PostedAt",
                table: "PostItems",
                column: "PostedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostItems");

            migrationBuilder.DropTable(
                name: "Owner");

            migrationBuilder.DropColumn(
                name: "OwnershipDocumentPath",
                table: "CarItems");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "CarImages");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "CarItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<byte[]>(
                name: "OwnershipDocumentData",
                table: "CarItems",
                type: "bytea",
                maxLength: 10485760,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "CarImages",
                type: "bytea",
                maxLength: 5242880,
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
