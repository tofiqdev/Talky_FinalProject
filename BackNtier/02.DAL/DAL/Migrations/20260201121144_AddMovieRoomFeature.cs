using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMovieRoomFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MovieRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YouTubeUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    YouTubeVideoId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CurrentTime = table.Column<double>(type: "float", nullable: false, defaultValue: 0.0),
                    IsPlaying = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Deleted = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieRooms_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovieRoomMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieRoomId = table.Column<int>(type: "int", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Deleted = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieRoomMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieRoomMessages_MovieRooms_MovieRoomId",
                        column: x => x.MovieRoomId,
                        principalTable: "MovieRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieRoomMessages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovieRoomParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieRoomId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Deleted = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieRoomParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieRoomParticipants_MovieRooms_MovieRoomId",
                        column: x => x.MovieRoomId,
                        principalTable: "MovieRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieRoomParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieRoomMessages_MovieRoomId",
                table: "MovieRoomMessages",
                column: "MovieRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieRoomMessages_SenderId",
                table: "MovieRoomMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieRoomMessages_SentAt",
                table: "MovieRoomMessages",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_MovieRoomParticipants_UserId",
                table: "MovieRoomParticipants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "idx_MovieRoomParticipant_Unique",
                table: "MovieRoomParticipants",
                columns: new[] { "MovieRoomId", "UserId", "Deleted" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieRooms_CreatedAt",
                table: "MovieRooms",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MovieRooms_CreatedById",
                table: "MovieRooms",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MovieRooms_IsActive",
                table: "MovieRooms",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieRoomMessages");

            migrationBuilder.DropTable(
                name: "MovieRoomParticipants");

            migrationBuilder.DropTable(
                name: "MovieRooms");
        }
    }
}
