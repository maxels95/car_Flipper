using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarFlipper.API.Migrations
{
    /// <inheritdoc />
    public partial class Added_Variables_To_Ad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Engine",
                table: "Ads",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Make",
                table: "Ads",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Ads",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RegionId",
                table: "Ads",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Engine",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "Make",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "Ads");
        }
    }
}
