using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamerasInfo.Migrations
{
    /// <inheritdoc />
    public partial class CameraTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KmComplement",
                table: "Cameras",
                type: "NVARCHAR2(2000)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastVerification",
                table: "Cameras",
                type: "TIMESTAMP(7)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Cameras",
                type: "NVARCHAR2(2000)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KmComplement",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "LastVerification",
                table: "Cameras");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Cameras");
        }
    }
}
