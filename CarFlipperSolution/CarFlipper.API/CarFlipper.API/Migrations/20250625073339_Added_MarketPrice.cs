using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarFlipper.API.Migrations
{
    /// <inheritdoc />
    public partial class Added_MarketPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ads_Url",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "EstimatedMarketValue",
                table: "Ads");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Ads",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ModelYear",
                table: "Ads",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Milage",
                table: "Ads",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsUnderpriced",
                table: "Ads",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Ads",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MarketPriceId",
                table: "Ads",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MarketPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Make = table.Column<string>(type: "TEXT", nullable: false),
                    Model = table.Column<string>(type: "TEXT", nullable: false),
                    ModelYearFrom = table.Column<int>(type: "INTEGER", nullable: false),
                    ModelYearTo = table.Column<int>(type: "INTEGER", nullable: false),
                    MilageFrom = table.Column<int>(type: "INTEGER", nullable: false),
                    MilageTo = table.Column<int>(type: "INTEGER", nullable: false),
                    Fuel = table.Column<string>(type: "TEXT", nullable: false),
                    Gearbox = table.Column<string>(type: "TEXT", nullable: false),
                    EstimatedPrice = table.Column<int>(type: "INTEGER", nullable: false),
                    SampleSize = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketPrices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ads_MarketPriceId",
                table: "Ads",
                column: "MarketPriceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ads_MarketPrices_MarketPriceId",
                table: "Ads",
                column: "MarketPriceId",
                principalTable: "MarketPrices",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ads_MarketPrices_MarketPriceId",
                table: "Ads");

            migrationBuilder.DropTable(
                name: "MarketPrices");

            migrationBuilder.DropIndex(
                name: "IX_Ads_MarketPriceId",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "MarketPriceId",
                table: "Ads");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Ads",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "ModelYear",
                table: "Ads",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "Milage",
                table: "Ads",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUnderpriced",
                table: "Ads",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "EstimatedMarketValue",
                table: "Ads",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ads_Url",
                table: "Ads",
                column: "Url",
                unique: true);
        }
    }
}
