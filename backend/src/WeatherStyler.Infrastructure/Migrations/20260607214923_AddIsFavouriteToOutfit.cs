using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherStyler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFavouriteToOutfit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavourite",
                table: "UsageHistories");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavourite",
                table: "Outfits",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFavourite",
                table: "Outfits");

            migrationBuilder.AddColumn<bool>(
                name: "IsFavourite",
                table: "UsageHistories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
