using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherStyler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WarmthLevelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WarmthRatings");

            migrationBuilder.AddColumn<int>(
                name: "WarmthLevel",
                table: "ClothingItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ClothingSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClothingSlots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClothingSlotCategories",
                columns: table => new
                {
                    ClothingSlotId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClothingSlotCategories", x => new { x.ClothingSlotId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_ClothingSlotCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClothingSlotCategories_ClothingSlots_ClothingSlotId",
                        column: x => x.ClothingSlotId,
                        principalTable: "ClothingSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClothingSlotCategories_CategoryId",
                table: "ClothingSlotCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ClothingSlots_Name",
                table: "ClothingSlots",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClothingSlotCategories");

            migrationBuilder.DropTable(
                name: "ClothingSlots");

            migrationBuilder.DropColumn(
                name: "WarmthLevel",
                table: "ClothingItems");

            migrationBuilder.CreateTable(
                name: "WarmthRatings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClothingItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ArmsLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    CoreLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    LegsLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarmthRatings", x => x.Id);
                    table.CheckConstraint("CK_WarmthRatings_ArmsLevel_Range", "ArmsLevel BETWEEN 1 AND 10");
                    table.CheckConstraint("CK_WarmthRatings_CoreLevel_Range", "CoreLevel BETWEEN 1 AND 10");
                    table.CheckConstraint("CK_WarmthRatings_LegsLevel_Range", "LegsLevel BETWEEN 1 AND 10");
                    table.ForeignKey(
                        name: "FK_WarmthRatings_ClothingItems_ClothingItemId",
                        column: x => x.ClothingItemId,
                        principalTable: "ClothingItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarmthRatings_ClothingItemId",
                table: "WarmthRatings",
                column: "ClothingItemId",
                unique: true);
        }
    }
}
