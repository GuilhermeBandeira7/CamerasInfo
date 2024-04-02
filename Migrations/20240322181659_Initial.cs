using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CamerasInfo.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvailabilityConfigs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Value = table.Column<float>(type: "BINARY_FLOAT", nullable: false),
                    PingTime = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    PingsToOffline = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    VerificationTime = table.Column<double>(type: "BINARY_DOUBLE", nullable: false),
                    currentStatus = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailabilityConfigs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Camera",
                columns: table => new
                {
                    Id = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Name = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Ip = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Direction = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Highway = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    Parents = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    latLing = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Camera", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvailabilityConfigs");

            migrationBuilder.DropTable(
                name: "Camera");
        }
    }
}
