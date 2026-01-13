using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TalkyAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddMuteFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSystemMessage",
                table: "GroupMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMuted",
                table: "GroupMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSystemMessage",
                table: "GroupMessages");

            migrationBuilder.DropColumn(
                name: "IsMuted",
                table: "GroupMembers");
        }
    }
}
