using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherStyler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class niepamietamcotubylo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutfitClothingItems_ClothingItems_ClothingItemId",
                table: "OutfitClothingItems");

            migrationBuilder.DropTable(
                name: "UsageHistoryClothingItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutfitClothingItems",
                table: "OutfitClothingItems");

            migrationBuilder.DropColumn(
                name: "WeatherCondition",
                table: "Outfits");

            migrationBuilder.AddColumn<Guid>(
                name: "ClothingItemEntityId",
                table: "UsageHistories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OutfitId",
                table: "UsageHistories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClothingItemEntityId",
                table: "Outfits",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "OutfitClothingItems",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OutfitClothingItems",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "OutfitClothingItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "OutfitClothingItems",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "OutfitClothingItems",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OutfitClothingItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WarmthLevel",
                table: "OutfitClothingItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutfitClothingItems",
                table: "OutfitClothingItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UsageHistories_ClothingItemEntityId",
                table: "UsageHistories",
                column: "ClothingItemEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_UsageHistories_OutfitId",
                table: "UsageHistories",
                column: "OutfitId");

            migrationBuilder.CreateIndex(
                name: "IX_Outfits_ClothingItemEntityId",
                table: "Outfits",
                column: "ClothingItemEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_OutfitClothingItems_OutfitId",
                table: "OutfitClothingItems",
                column: "OutfitId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutfitClothingItems_ClothingItems_ClothingItemId",
                table: "OutfitClothingItems",
                column: "ClothingItemId",
                principalTable: "ClothingItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Outfits_ClothingItems_ClothingItemEntityId",
                table: "Outfits",
                column: "ClothingItemEntityId",
                principalTable: "ClothingItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsageHistories_ClothingItems_ClothingItemEntityId",
                table: "UsageHistories",
                column: "ClothingItemEntityId",
                principalTable: "ClothingItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsageHistories_Outfits_OutfitId",
                table: "UsageHistories",
                column: "OutfitId",
                principalTable: "Outfits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutfitClothingItems_ClothingItems_ClothingItemId",
                table: "OutfitClothingItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Outfits_ClothingItems_ClothingItemEntityId",
                table: "Outfits");

            migrationBuilder.DropForeignKey(
                name: "FK_UsageHistories_ClothingItems_ClothingItemEntityId",
                table: "UsageHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_UsageHistories_Outfits_OutfitId",
                table: "UsageHistories");

            migrationBuilder.DropIndex(
                name: "IX_UsageHistories_ClothingItemEntityId",
                table: "UsageHistories");

            migrationBuilder.DropIndex(
                name: "IX_UsageHistories_OutfitId",
                table: "UsageHistories");

            migrationBuilder.DropIndex(
                name: "IX_Outfits_ClothingItemEntityId",
                table: "Outfits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutfitClothingItems",
                table: "OutfitClothingItems");

            migrationBuilder.DropIndex(
                name: "IX_OutfitClothingItems_OutfitId",
                table: "OutfitClothingItems");

            migrationBuilder.DropColumn(
                name: "ClothingItemEntityId",
                table: "UsageHistories");

            migrationBuilder.DropColumn(
                name: "OutfitId",
                table: "UsageHistories");

            migrationBuilder.DropColumn(
                name: "ClothingItemEntityId",
                table: "Outfits");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OutfitClothingItems");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OutfitClothingItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "OutfitClothingItems");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "OutfitClothingItems");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "OutfitClothingItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OutfitClothingItems");

            migrationBuilder.DropColumn(
                name: "WarmthLevel",
                table: "OutfitClothingItems");

            migrationBuilder.AddColumn<string>(
                name: "WeatherCondition",
                table: "Outfits",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutfitClothingItems",
                table: "OutfitClothingItems",
                columns: new[] { "OutfitId", "ClothingItemId" });

            migrationBuilder.CreateTable(
                name: "UsageHistoryClothingItems",
                columns: table => new
                {
                    UsageHistoryId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClothingItemId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageHistoryClothingItems", x => new { x.UsageHistoryId, x.ClothingItemId });
                    table.ForeignKey(
                        name: "FK_UsageHistoryClothingItems_ClothingItems_ClothingItemId",
                        column: x => x.ClothingItemId,
                        principalTable: "ClothingItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsageHistoryClothingItems_UsageHistories_UsageHistoryId",
                        column: x => x.UsageHistoryId,
                        principalTable: "UsageHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsageHistoryClothingItems_ClothingItemId",
                table: "UsageHistoryClothingItems",
                column: "ClothingItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutfitClothingItems_ClothingItems_ClothingItemId",
                table: "OutfitClothingItems",
                column: "ClothingItemId",
                principalTable: "ClothingItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
