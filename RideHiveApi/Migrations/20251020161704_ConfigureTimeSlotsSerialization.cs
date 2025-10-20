using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RideHiveApi.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureTimeSlotsSerialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AvailableTimeSlots",
                table: "PostItems",
                type: "text",
                nullable: false,
                oldClrType: typeof(DateTime[]),
                oldType: "timestamp with time zone[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime[]>(
                name: "AvailableTimeSlots",
                table: "PostItems",
                type: "timestamp with time zone[]",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
