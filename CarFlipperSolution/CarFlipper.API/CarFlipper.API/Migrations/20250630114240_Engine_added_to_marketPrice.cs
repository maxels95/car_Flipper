using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarFlipper.API.Migrations
{
    /// <inheritdoc />
    public partial class Engine_added_to_marketPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Engine",
                table: "MarketPrices",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Engine",
                table: "MarketPrices");
        }
    }
}
