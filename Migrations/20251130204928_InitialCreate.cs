using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HntdApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParanormalActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IconName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParanormalActivities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HauntedLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: true),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    History = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LocationTypeId = table.Column<int>(type: "integer", nullable: false),
                    ActivityLevelId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HauntedLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HauntedLocations_ActivityLevels_ActivityLevelId",
                        column: x => x.ActivityLevelId,
                        principalTable: "ActivityLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HauntedLocations_LocationTypes_LocationTypeId",
                        column: x => x.LocationTypeId,
                        principalTable: "LocationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HauntedLocations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LocationActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HauntedLocationId = table.Column<int>(type: "integer", nullable: false),
                    ParanormalActivityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationActivities_HauntedLocations_HauntedLocationId",
                        column: x => x.HauntedLocationId,
                        principalTable: "HauntedLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationActivities_ParanormalActivities_ParanormalActivityId",
                        column: x => x.ParanormalActivityId,
                        principalTable: "ParanormalActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFavorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    HauntedLocationId = table.Column<int>(type: "integer", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserFavorites_HauntedLocations_HauntedLocationId",
                        column: x => x.HauntedLocationId,
                        principalTable: "HauntedLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFavorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ActivityLevels",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Occasional unexplained occurrences", "Mild" },
                    { 2, "Regular paranormal activity reported", "Moderate" },
                    { 3, "Frequent and intense activity", "High" }
                });

            migrationBuilder.InsertData(
                table: "LocationTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Cemetery" },
                    { 2, "Hospital" },
                    { 3, "Asylum" },
                    { 4, "Prison" },
                    { 5, "House" },
                    { 6, "Hotel" },
                    { 7, "Battlefield" },
                    { 8, "Church" }
                });

            migrationBuilder.InsertData(
                table: "ParanormalActivities",
                columns: new[] { "Id", "Description", "IconName", "Name" },
                values: new object[,]
                {
                    { 1, "Visual ghost sightings", "ghost", "Apparitions" },
                    { 2, "Electronic voice phenomena", "mic", "EVP" },
                    { 3, "Unexplained temperature drops", "thermometer", "Cold Spots" },
                    { 4, "Items moving on their own", "move", "Object Movement" },
                    { 5, "Dark humanoid shapes", "shadow", "Shadow Figures" },
                    { 6, "Voices heard without source", "volume", "Disembodied Voices" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLevels_Name",
                table: "ActivityLevels",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HauntedLocations_ActivityLevelId",
                table: "HauntedLocations",
                column: "ActivityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_HauntedLocations_LocationTypeId",
                table: "HauntedLocations",
                column: "LocationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_HauntedLocations_Name",
                table: "HauntedLocations",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HauntedLocations_UserId",
                table: "HauntedLocations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationActivities_HauntedLocationId",
                table: "LocationActivities",
                column: "HauntedLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationActivities_ParanormalActivityId",
                table: "LocationActivities",
                column: "ParanormalActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationTypes_Name",
                table: "LocationTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParanormalActivities_Name",
                table: "ParanormalActivities",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_HauntedLocationId",
                table: "UserFavorites",
                column: "HauntedLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFavorites_UserId",
                table: "UserFavorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserId",
                table: "Users",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationActivities");

            migrationBuilder.DropTable(
                name: "UserFavorites");

            migrationBuilder.DropTable(
                name: "ParanormalActivities");

            migrationBuilder.DropTable(
                name: "HauntedLocations");

            migrationBuilder.DropTable(
                name: "ActivityLevels");

            migrationBuilder.DropTable(
                name: "LocationTypes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
