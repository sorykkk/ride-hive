using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RideHiveApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarItems",
                columns: table => new
                {
                    CarId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OwnerId = table.Column<string>(type: "text", nullable: false),
                    Brand = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Color = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    NumberDoors = table.Column<int>(type: "integer", nullable: false),
                    NumberSeats = table.Column<int>(type: "integer", nullable: false),
                    YearProduction = table.Column<int>(type: "integer", nullable: false),
                    Course = table.Column<int>(type: "integer", nullable: false),
                    Fuel = table.Column<string>(type: "text", nullable: false),
                    Consumption = table.Column<float>(type: "real", nullable: true),
                    Drive = table.Column<string>(type: "text", nullable: false),
                    Transmission = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    Displacement = table.Column<float>(type: "real", nullable: false),
                    HorsePower = table.Column<int>(type: "integer", nullable: false),
                    Condition = table.Column<string>(type: "text", nullable: false),
                    VinNumber = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    OwnershipDocumentPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OwnershipDocumentContentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarItems", x => x.CarId);
                });

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
                name: "CarImages",
                columns: table => new
                {
                    CarImageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CarId = table.Column<int>(type: "integer", nullable: false),
                    ImagePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ImageContentType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarImages", x => x.CarImageId);
                    table.ForeignKey(
                        name: "FK_CarImages_CarItems_CarId",
                        column: x => x.CarId,
                        principalTable: "CarItems",
                        principalColumn: "CarId",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_CarImages_CarId",
                table: "CarImages",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CarImages_UploadedAt",
                table: "CarImages",
                column: "UploadedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CarItems_Brand_Model",
                table: "CarItems",
                columns: new[] { "Brand", "Model" });

            migrationBuilder.CreateIndex(
                name: "IX_CarItems_OwnerId",
                table: "CarItems",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CarItems_VinNumber",
                table: "CarItems",
                column: "VinNumber",
                unique: true);

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
                name: "CarImages");

            migrationBuilder.DropTable(
                name: "PostItems");

            migrationBuilder.DropTable(
                name: "CarItems");

            migrationBuilder.DropTable(
                name: "Owner");
        }
    }
}
